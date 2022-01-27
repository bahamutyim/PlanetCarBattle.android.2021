using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace com.ahyim.planet
{
    public class CarControl : MonoBehaviour
    {

        public  enum SpeedType
        {
            MPH,
            KPH
        }
        public enum AxlePosition
        {
            Front,
            Rear
        }


        [SerializeField] private List<AxleInfo> axleInfos;
        private float maxMotorTorque = 4000f;
        [SerializeField] private float maxSteeringAngle = 45f;
        [SerializeField] private float downForce = 200f;
        [SerializeField] private float topSpeed = 300f;
        private float breakTorque = 4000f;
        private float reverseTorque = 200f; // will be change to half of maxMotor
        [SerializeField] private float steerHelper = 0.65f;
        [SerializeField] private float slipLimit = 0.5f;
        [SerializeField] private float tractionControl = 1f;
        [SerializeField] private Transform centerOfMass;
        [SerializeField] private static int NoOfGears = 5;
        [SerializeField] private float m_RevRangeBoundary = 1f;

        private int totalMotorWheel = 0;
        private Rigidbody attachedRigibody;
        private float currentSpeed;
        private SpeedType speedType = SpeedType.KPH;
        private float speedFactor;
        private float oldRotation;
        private float currentTorque;
        //private float maxWheelTorque;
        private float axleTorqueFactorLimit = 0.3f;
        private int gearNum;
        private float gearFactor;
        private string rootName;

        private SpeedUIText speedUIText;

        private float wheelSpinSlipLimit = 0.5f;
        

        public float Revs { get; private set; }
        public float AccelInput { get; private set; }
        public float CurrentSpeed { get { return currentSpeed; } }
        public float MaxSpeed { get { return topSpeed; } }
        public SpeedType CurrentSpeedType { get { return speedType; } }

        //for debug
        public float tSteeringRatio;
        public float tAccelerationRatio;
        public float tBreakRatio;

        // Use this for initialization
        void Start()
        {
            Debug.Log("CarControl start");
            foreach (AxleInfo axleInfo in axleInfos)
            {
                if (axleInfo.motor)
                {
                    totalMotorWheel += 2;
                }
                else
                {
                    axleInfo.leftWheel.motorTorque = 0f;
                    axleInfo.rightWheel.motorTorque = 0f;
                    axleInfo.leftWheel.brakeTorque = 0f;
                    axleInfo.rightWheel.brakeTorque = 0f;
                }

            }
           
            attachedRigibody = axleInfos[0].leftWheel.attachedRigidbody;
            if (attachedRigibody)
            { 
                Debug.Log("attachedRigibody exist ");
            }
            else
            {
                Debug.Log("attachedRigibody is null ");
            }
            //currentTorque = maxMotorTorque - (tractionControl * maxMotorTorque);
            currentTorque = maxMotorTorque;

            reverseTorque = maxMotorTorque / totalMotorWheel / 2f;
            //maxWheelTorque = maxMotorTorque / totalMotorWheel;
            attachedRigibody.centerOfMass = centerOfMass.localPosition;
            if ( speedType == SpeedType.KPH)
            {
                speedFactor = 3.6f;
            }
            else
            {
                speedFactor = 2.23693629f;
            }
            rootName = transform.root.name;

            speedUIText = SpeedUIText.singleton;


        }

        // Update is called once per frame
        void Update()
        {
            if (attachedRigibody)
            {
                foreach (AxleInfo axleInfo in axleInfos)
                {
                    ApplyLocalPositionToVisuals(axleInfo.leftWheel);
                    ApplyLocalPositionToVisuals(axleInfo.rightWheel);
                }
            }
        }

        public void SetPower(int power)
        {
            maxMotorTorque = maxMotorTorque / 100 * power;
            currentTorque = maxMotorTorque;
            Debug.LogFormat("maxMotorTorque:{0}", maxMotorTorque);
        }

        public void ApplyLocalPositionToVisuals(WheelCollider collider)
        {
            if (collider.transform.childCount == 0)
            {
                return;
            }

            Transform visualWheel = collider.transform.GetChild(0);

            Vector3 position;
            Quaternion rotation;
            collider.GetWorldPose(out position, out rotation);

            visualWheel.transform.position = position;
            visualWheel.transform.rotation = rotation;
        }

        public void CarMove(float steeringRatio, float accelerationRatio, float breakRatio)
        {
            this.tSteeringRatio = steeringRatio;
            this.tAccelerationRatio = accelerationRatio;
            this.tBreakRatio = breakRatio;


            float motor = currentTorque * accelerationRatio / totalMotorWheel;
            float steering = maxSteeringAngle * steeringRatio;
            float breakForce = breakTorque * breakRatio;
            AccelInput = accelerationRatio; 

            
            //Debug.Log( string.Format("Name={0}, Motor={1:0.0000}, steering={2:0.0000}, breakForce={3:0.0000}, currentTorque={4:0.0000} ", rootName, motor, steering, breakForce, currentTorque));

            int currentAxleNum = 0;


            SteerHelper();
            CalculateSpeed();

            foreach (AxleInfo axleInfo in axleInfos)
            {
                string tempStr = "";
                currentAxleNum++;

                WheelHit leftWheelhit;
                WheelHit rightWheelhit;
                float leftForwardSlip = 0f;
                float rightForwardSlip = 0f;
                float totalForwardSlip = 0f;

                axleInfo.leftWheel.GetGroundHit(out leftWheelhit);
                leftForwardSlip = leftWheelhit.forwardSlip;
                axleInfo.rightWheel.GetGroundHit(out rightWheelhit);
                rightForwardSlip = rightWheelhit.forwardSlip;
                totalForwardSlip = leftForwardSlip + rightForwardSlip;

                if (axleInfo.steering)
                {
                    axleInfo.leftWheel.steerAngle = steering;
                    axleInfo.rightWheel.steerAngle = steering;
                }
                if (axleInfo.motor)
                {
                    //if ( steeringRatio != 0 && axleInfo.axlePosition == AxlePosition.Front)
                    //{
                    //    axleInfo.axleTorqueFactor = 0.7f;
                    //}
                    //else if (steeringRatio != 0 && axleInfo.axlePosition == AxlePosition.Rear)
                    //{
                    //    axleInfo.axleTorqueFactor = 0.3f;
                    //}
                    


                    

                    // if axle over the slip limit, reduce torque till 0.5
                    if (axleInfo.axleTorqueFactor > axleTorqueFactorLimit && rightForwardSlip > slipLimit || leftForwardSlip > slipLimit)
                    {
                        axleInfo.axleTorqueFactor -= 0.1f;
                        if (axleInfo.axleTorqueFactor < axleTorqueFactorLimit)
                        {
                            axleInfo.axleTorqueFactor = axleTorqueFactorLimit;
                        }
                    }
                    else if (axleInfo.axleTorqueFactor < 1f)
                    {
                        axleInfo.axleTorqueFactor += 0.01f;
                        if (axleInfo.axleTorqueFactor > 1f)
                        {
                            axleInfo.axleTorqueFactor = 1f;
                        }
                    }

                    //if (totalForwardSlip == 0)
                    //{
                    //    axleInfo.leftTorqueFactor = 1;
                    //    axleInfo.rightTorqueFactor = 1;
                    //}
                    //else
                    //{
                    //    axleInfo.leftTorqueFactor = Mathf.Clamp( leftForwardSlip * 2 / (totalForwardSlip), 0.5f, 1f);
                    //    axleInfo.rightTorqueFactor = Mathf.Clamp( rightForwardSlip * 2 / (totalForwardSlip), 0.5f, 1f);
                    //}

                    //float sidewaysSlipLimiy = 0.05f;


                    //if (leftWheelhit.sidewaysSlip < -sidewaysSlipLimiy || rightWheelhit.sidewaysSlip < -sidewaysSlipLimiy)
                    //{
                    //    axleInfo.leftTorqueFactor -= 0.1f;
                    //    axleInfo.rightTorqueFactor = 0.5f;
                    //    if (axleInfo.leftTorqueFactor < 0.2f)
                    //    {
                    //        axleInfo.leftTorqueFactor = 0.2f;
                    //    }

                    //}
                    //else if (leftWheelhit.sidewaysSlip > sidewaysSlipLimiy || rightWheelhit.sidewaysSlip > sidewaysSlipLimiy)
                    //{
                    //    axleInfo.leftTorqueFactor = 0.5f;
                    //    axleInfo.rightTorqueFactor -= 0.1f;
                    //    if (axleInfo.rightTorqueFactor < 0.2f)
                    //    {
                    //        axleInfo.rightTorqueFactor = 0.2f;
                    //    }
                    //}
                    //else
                    //{
                    //    axleInfo.leftTorqueFactor = 1f;
                    //    axleInfo.rightTorqueFactor = 1f;
                    //}



                    //axleInfo.leftTorqueFactor = AdjustWheelTorque(wheelhit.forwardSlip, axleInfo.leftTorqueFactor);
                    axleInfo.leftWheel.motorTorque = motor * axleInfo.axleTorqueFactor * axleInfo.leftTorqueFactor;

                    //axleInfo.rightWheel.GetGroundHit(out wheelhit);
                    //axleInfo.rightTorqueFactor = AdjustWheelTorque(wheelhit.forwardSlip, axleInfo.rightTorqueFactor);
                    axleInfo.rightWheel.motorTorque = motor * axleInfo.axleTorqueFactor * axleInfo.rightTorqueFactor;
                    //tempStr += tempInt + ". Left forward/side slip/factor/motor=" + leftWheelhit.forwardSlip + "/" + leftWheelhit.sidewaysSlip + "/" + axleInfo.leftTorqueFactor + "/" + axleInfo.leftWheel.motorTorque + "\n";
                    //tempStr += "   Right forward/side slip/factor/motor=" + rightWheelhit.forwardSlip + "/" + rightWheelhit.sidewaysSlip + "/" + axleInfo.rightTorqueFactor + "/" + axleInfo.rightWheel.motorTorque + "\n";

                    axleInfo.leftWheel.brakeTorque = breakForce;
                    axleInfo.rightWheel.brakeTorque = breakForce;

                    

                    CheckForWheelSpin(leftWheelhit, axleInfo.leftWheelEffects);
                    CheckForWheelSpin(rightWheelhit, axleInfo.rightWheelEffects);

                }

                if (breakRatio > 0 && (currentSpeed <= 5 || Vector3.Angle(transform.forward, attachedRigibody.velocity) > 50f))
                {
                    axleInfo.leftWheel.brakeTorque = 0;
                    axleInfo.rightWheel.brakeTorque = 0;
                    if (axleInfo.motor)
                    {
                        axleInfo.leftWheel.motorTorque = -reverseTorque * breakRatio;
                        axleInfo.rightWheel.motorTorque = -reverseTorque * breakRatio;
                    }
                }

                //tempStr += string.Format("name={0}", rootName);
                //tempStr += string.Format("Axle({0}). axleTorqueFactor={1:0.0000}\n", currentAxleNum, axleInfo.axleTorqueFactor);
                //tempStr += string.Format("Axle({0}). Left  forward/side slip|factor|motor|break={1:0.0000}/{2:0.0000}|{3:0.0000}|{4:0.0000}|{5:0.0000}\n", currentAxleNum, leftWheelhit.forwardSlip, leftWheelhit.sidewaysSlip, axleInfo.leftTorqueFactor, axleInfo.leftWheel.motorTorque, axleInfo.leftWheel.brakeTorque);
                //tempStr += string.Format("Axle({0}). Right forward/side slip|factor|motor|break={1:0.0000}/{2:0.0000}|{3:0.0000}|{4:0.0000}|{5:0.0000}\n", currentAxleNum, rightWheelhit.forwardSlip, rightWheelhit.sidewaysSlip, axleInfo.rightTorqueFactor, axleInfo.rightWheel.motorTorque, axleInfo.rightWheel.brakeTorque);
                //tempStr += string.Format("attachedRigibody.velocity:{0}", attachedRigibody.velocity);
                //Debug.Log(tempStr);


                //ApplyLocalPositionToVisuals(axleInfo.leftWheel);
                //ApplyLocalPositionToVisuals(axleInfo.rightWheel);
            }
            

            CapSpeed();
            //if (currentSpeed > 5 && Vector3.Angle(transform.forward, attachedRigibody.velocity) < 50f)
            //{
            //    foreach (AxleInfo axleInfo in axleInfos)
            //    {
            //        axleInfo.leftWheel.brakeTorque = breakForce;
            //        axleInfo.rightWheel.brakeTorque = breakForce;
            //    }
            //}
            //else if (breakRatio > 0)
            //{
            //    foreach (AxleInfo axleInfo in axleInfos)
            //    {
            //        axleInfo.leftWheel.brakeTorque = 0;
            //        axleInfo.rightWheel.brakeTorque = 0;

            //        if (axleInfo.motor)
            //        {
            //            axleInfo.leftWheel.motorTorque = -reverseTorque * breakRatio;
            //            axleInfo.rightWheel.motorTorque = -reverseTorque * breakRatio;
            //        }
                    
                    

            //    }

            //}

            CalculateRevs();
            GearChanging();
            AddDownForce();
            //TractionControl();
            
        }
        private void CalculateSpeed()
        {
            currentSpeed =  Vector3.Dot( attachedRigibody.velocity, transform.forward) * speedFactor;
            //Debug.Log(string.Format("name={0}, Speed = {1}{2}", rootName, currentSpeed, speedType));
            speedUIText.UpdateSpeed(currentSpeed);
        }

        // this is used to add more grip in relation to speed
        private void AddDownForce()
        {
            attachedRigibody.AddForce(
                -transform.up *
                downForce *
                attachedRigibody.velocity.magnitude
                );
        }

        private void CapSpeed()
        {
            if (currentSpeed > topSpeed)
            {
                currentTorque -= 10f;
                if (currentTorque < 0f)
                {
                    currentTorque = 0f;
                }

            }
            else if (currentTorque < maxMotorTorque)
            {
                currentTorque += 10f;
                if (currentTorque > maxMotorTorque)
                {
                    currentTorque = maxMotorTorque;
                }
            }

            


        }

        private void SteerHelper()
        {

            foreach (AxleInfo axleInfo in axleInfos)
            {
                WheelHit wheelhit;
                axleInfo.leftWheel.GetGroundHit(out wheelhit);
                if (wheelhit.normal == Vector3.zero)
                {
                    return; // wheels arent on the ground so dont realign the rigidbody velocity
                }
                axleInfo.rightWheel.GetGroundHit(out wheelhit);
                if (wheelhit.normal == Vector3.zero)
                {
                    return; // wheels arent on the ground so dont realign the rigidbody velocity
                }
            }

            // this if is needed to avoid gimbal lock problems that will make the car suddenly shift direction
            if (Mathf.Abs(oldRotation - transform.eulerAngles.y) < 20f)
            {
                var turnadjust = (transform.eulerAngles.y - oldRotation) * steerHelper;
                Quaternion velRotation = Quaternion.AngleAxis(turnadjust, Vector3.up);
                attachedRigibody.velocity = velRotation * attachedRigibody.velocity;
                //Debug.Log(string.Format("attachedRigibody.velocity={0},attachedRigibody.rotation={1} ", attachedRigibody.velocity, attachedRigibody.rotation));
            }
            oldRotation = transform.eulerAngles.y;
        }

        // crude traction control that reduces the power to wheel if the car is wheel spinning too much
        private void TractionControl()
        {
            WheelHit wheelHit;


            foreach (AxleInfo axleInfo in axleInfos)
            {

                if (axleInfo.motor)
                {
                    axleInfo.leftWheel.GetGroundHit(out wheelHit);
                    AdjustTorque(wheelHit.forwardSlip);
                    axleInfo.rightWheel.GetGroundHit(out wheelHit);
                    AdjustTorque(wheelHit.forwardSlip);
                }
            }

        }


        private void AdjustTorque(float forwardSlip)
        {
            if (forwardSlip >= slipLimit && currentTorque >= 0)
            {
                currentTorque -= 10 * tractionControl;
            }
            else
            {
                currentTorque += 10 * tractionControl;
                if (currentTorque > maxMotorTorque)
                {
                    currentTorque = maxMotorTorque;
                }
            }
        }
        private float AdjustWheelTorque(float forwardSlip, float TorqueFactor)
        {
            if (forwardSlip >= slipLimit && TorqueFactor > 0)
            {
                TorqueFactor -= 0.2f;
            }
            else if (TorqueFactor < 1)
            {
                TorqueFactor += 0.2f;
            }

            return TorqueFactor;
        }
        private void CalculateRevs()
        {
            // calculate engine revs (for display / sound)
            // (this is done in retrospect - revs are not used in force/power calculations)
            CalculateGearFactor();
            var gearNumFactor = gearNum / (float)NoOfGears;
            var revsRangeMin = ULerp(0f, m_RevRangeBoundary, CurveFactor(gearNumFactor));
            var revsRangeMax = ULerp(m_RevRangeBoundary, 1f, gearNumFactor);
            Revs = ULerp(revsRangeMin, revsRangeMax, gearFactor);
        }
        private void CalculateGearFactor()
        {
            float f = (1 / (float)NoOfGears);
            // gear factor is a normalised representation of the current speed within the current gear's range of speeds.
            // We smooth towards the 'target' gear factor, so that revs don't instantly snap up or down when changing gear.
            var targetGearFactor = Mathf.InverseLerp(f * gearNum, f * (gearNum + 1), Mathf.Abs(currentSpeed / MaxSpeed));
            gearFactor = Mathf.Lerp(gearFactor, targetGearFactor, Time.deltaTime * 5f);
        }
        private void GearChanging()
        {
            float f = Mathf.Abs(currentSpeed / MaxSpeed);
            float upgearlimit = (1 / (float)NoOfGears) * (gearNum + 1);
            float downgearlimit = (1 / (float)NoOfGears) * gearNum;

            if (gearNum > 0 && f < downgearlimit)
            {
                gearNum--;
            }

            if (f > upgearlimit && (gearNum < (NoOfGears - 1)))
            {
                gearNum++;
            }
        }
        // checks if the wheels are spinning and is so does three things
        // 1) emits particles
        // 2) plays tiure skidding sounds
        // 3) leaves skidmarks on the ground
        // these effects are controlled through the WheelEffects class
        private void CheckForWheelSpin(WheelHit wheelHit, WheelEffects wheelEffects)
        {
            // is the tire slipping above the given threshhold
            if (Mathf.Abs(wheelHit.forwardSlip) >= wheelSpinSlipLimit || Mathf.Abs(wheelHit.sidewaysSlip) >= wheelSpinSlipLimit)
            {
                //Debug.Log("Slipping");
                wheelEffects.EmitTyreSmoke();

                // avoiding all four tires screeching at the same time
                // if they do it can lead to some strange audio artefacts
                if (!AnySkidSoundPlaying())
                {
                    //wheelEffects.GetComponent<AudioSource>().Play();
                    wheelEffects.PlayAudio();
                }


            }
            else
            {
                // if it wasnt slipping stop all the audio
                if (wheelEffects.PlayingAudio)
                {
                    wheelEffects.StopAudio();
                }
                // end the trail generation
                wheelEffects.EndSkidTrail();
            }
            
        }
        // simple function to add a curved bias towards 1 for a value in the 0-1 range
        private static float CurveFactor(float factor)
        {
            return 1 - (1 - factor) * (1 - factor);
        }
        // unclamped version of Lerp, to allow value to exceed the from-to range
        private static float ULerp(float from, float to, float value)
        {
            return (1.0f - value) * from + value * to;
        }

        private bool AnySkidSoundPlaying()
        {
            foreach(AxleInfo axleInfo in axleInfos  )
            {
                if (axleInfo.leftWheelEffects.PlayingAudio || axleInfo.rightWheelEffects.PlayingAudio)
                {
                    return true;
                }
            }
            
            return false;
        }

        [System.Serializable]
        public class AxleInfo
        {


            public WheelCollider leftWheel;
            public WheelCollider rightWheel;
            public WheelEffects leftWheelEffects;
            public WheelEffects rightWheelEffects;
            public bool motor;
            public bool steering;
            public float leftTorqueFactor = 1f;
            public float rightTorqueFactor = 1f;
            public float axleTorqueFactor = 1f;
            public AxlePosition axlePosition;
        }
    }
}
