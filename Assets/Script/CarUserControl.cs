using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace com.ahyim.planet
{
    public class CarUserControl : NetworkBehaviour
    {
        public float smoothTurnTime = 0.3f;

        private float accelerationRatio;
        private float breakRatio;
        private float steeringRatio;
        private CarControl currCarControl;
        private float currentSteeringRatio;
        private float currentBreakRatio;
        private float turnSpeedVelocityChange;
        private Text playerInfomationText;
        private const int maxMass = 4000;
        private bool isAIPlayer = false;
        private int rayCastLayerMark = 1;
        private Vector3 leftLocalPoint;
        private Vector3 rightLocalPoint;
        private Vector3 behindLocalPoint;
        private float aiDetectRange = 100.0f;
        private float castBehindLength = 2.0f;
        
        private int hitCount = 0;
        private int lowSpeedCount = 0;
        private int upSideDownCount = 0;
        private Transform lastDetectedTransform;

        private Rigidbody rigidbody;
        private NetPlayerManager netPlayerManager;


        [SyncVar(hook = "OnChangePower")]
        public int power;
        
        [SyncVar(hook = "OnChangeWeight")]
        public int weight;

        private static CarUserControl currPlayer;

        //for debug
        public string status;

        // Use this for initialization
        void Start()
        {
            currCarControl = GetComponentInChildren<CarControl>();
            netPlayerManager = GetComponent<NetPlayerManager>();
            //For fix power assigned from server before get CarControl reference. 
            OnChangePower(power);
            
            if (isLocalPlayer)
            {
                currPlayer = this;
            }

            rayCastLayerMark = LayerMask.GetMask("Envoirnment", "Mounting");

            

            leftLocalPoint = new Vector3(-1.2f, 0f, 0f);
            rightLocalPoint = new Vector3(1.2f, 0f, 0f);
            behindLocalPoint = new Vector3(0f, 0f, -castBehindLength);
            aiDetectRange += castBehindLength;

            rigidbody = GetComponent<Rigidbody>();
        }

        // Update is called once per frame
        void Update()
        {

        }

        private void FixedUpdate()
        {
            if (isLocalPlayer)
            {
                accelerationRatio = CrossPlatformInputManager.GetAxis("Vertical");
                if (accelerationRatio >= 0)
                {
                    breakRatio = 0;
                    currentBreakRatio = 0;
                }
                else
                {
                    breakRatio = -accelerationRatio;
                    accelerationRatio = 0;
                    currentBreakRatio = Mathf.SmoothDamp(currentBreakRatio, breakRatio, ref turnSpeedVelocityChange, smoothTurnTime);
                }
                steeringRatio = CrossPlatformInputManager.GetAxis("Horizontal");

                if (steeringRatio != 0 || currentSteeringRatio > 0.01 || currentSteeringRatio < -0.01)
                {
                    currentSteeringRatio = Mathf.SmoothDamp(currentSteeringRatio, steeringRatio, ref turnSpeedVelocityChange, smoothTurnTime);
                }
                else
                {
                    currentSteeringRatio = steeringRatio;
                }
                //Debug.Log(currentSteeringRatio + ", " + accelerationRatio + ", " + currentBreakRatio);
                currCarControl.CarMove(currentSteeringRatio, accelerationRatio, currentBreakRatio);
            }
            else if (isServer && isAIPlayer )
            {
                RaycastHit hit;
                float aiSteering;
                float acceleration;
                float breakRatio;


                Vector3 behindCastPosition = transform.TransformPoint(behindLocalPoint);

                if (netPlayerManager.targetPlayer)
                {

                    Debug.DrawLine(transform.position, netPlayerManager.targetPlayer.transform.position, Color.blue);
                    Vector3 targetPlayerLocalPosition = transform.InverseTransformPoint(netPlayerManager.targetPlayer.transform.position);

                    if (targetPlayerLocalPosition.x > 0.1)
                    {
                        aiSteering = 1.0f;
                    }
                    else if(targetPlayerLocalPosition.x < -0.1)
                    {
                        aiSteering = -1.0f;
                    }
                    else
                    {
                        aiSteering = 0.0f;
                    }
                    if (currCarControl.CurrentSpeed > 40.0f)
                    {
                        acceleration = 0.0f;
                        breakRatio = 1.0f;
                    }
                    else if (currCarControl.CurrentSpeed < 10.0f && currCarControl.CurrentSpeed > -10.0f)
                    {
                        aiSteering = -1.0f;
                        acceleration = 0.0f;
                        breakRatio = 1.0f;
                    }
                    else
                    {
                        acceleration = 0.3f;
                        breakRatio = 0.0f;
                    }
                    
                    //Debug.LogFormat("{0} follow target position:{1}, acceleration{2}, breakRatio{3}, aiSteering{4}", netPlayerManager.playerName , targetPlayerLocalPosition, acceleration, breakRatio, aiSteering);

                }

                else if (Physics.BoxCast(behindCastPosition, new Vector3(1.5f,1.5f,1.5f), transform.TransformDirection(Vector3.forward),  out hit, transform.rotation, aiDetectRange, rayCastLayerMark))
                {
                    hitCount++;
                    Transform currDetectedTransform = hit.collider.transform;

                    

                    if (!lastDetectedTransform  
                        || lastDetectedTransform.Equals( currDetectedTransform )
                        || (transform.position - currDetectedTransform.position).sqrMagnitude - (transform.position - lastDetectedTransform.position).sqrMagnitude <0  
                        )
                    {
                        Debug.DrawLine(behindCastPosition, hit.collider.transform.position, Color.red);
                        lastDetectedTransform = currDetectedTransform;
                        Vector3 hitLocalPosition = transform.InverseTransformPoint(hit.collider.transform.position);
                        if (hitLocalPosition.x >= 0)
                        {
                            aiSteering = -1f;
                        }
                        else
                        {
                            aiSteering = 1f;
                        }

                        float sqrDistance = (transform.position - hit.point).sqrMagnitude;

                        //Debug.LogFormat("Name:{0}, aiSteering:{1}, sqrDistance:{2}", netPlayerManager.name, aiSteering, sqrDistance);

                        if (sqrDistance < 5.0f * 5.0f)
                        {
                            aiSteering = -aiSteering;
                            acceleration = 0f;
                            breakRatio = 1f;
                        }
                        else if (sqrDistance < 20.0f * 20.0f)
                        {
                            if (currCarControl.CurrentSpeed <= 30.0f)
                            {
                                acceleration = 0.3f;
                                breakRatio = 0.0f;
                            }
                            else
                            {
                                acceleration = 0.0f;
                                breakRatio = 1.0f;
                            }
                        }
                        else if (sqrDistance < 50.0f * 50.0f)
                        {
                            if (currCarControl.CurrentSpeed <= 60.0f)
                            {
                                acceleration = 0.3f;
                                breakRatio = 0.0f;
                            }
                            else
                            {
                                acceleration = 0.0f;
                                breakRatio = 1.0f;
                            }
                        }
                        else
                        {
                            acceleration = 0.3f;
                            breakRatio = 0.0f;
                        }
                    }
                    else
                    {
                        Debug.DrawLine(behindCastPosition, hit.collider.transform.position, Color.red);
                        Debug.DrawLine(behindCastPosition, lastDetectedTransform.position, Color.green);
                        if (hitCount > 0)
                        {
                            hitCount--;
                        }
                        aiSteering = 0f;
                        if (currCarControl.CurrentSpeed > 50.0f)
                        {
                            acceleration = 0.0f;
                            breakRatio = 0.5f;
                        }
                        else
                        {
                            
                            acceleration = 0.3f;
                            breakRatio = 0.0f;
                        }

                        Vector3 lastDetectedLocalPosition = transform.InverseTransformPoint(lastDetectedTransform.position);
                        if (lastDetectedLocalPosition.z <= 0)
                        {
                            lastDetectedTransform = null;
                        }
                        
                    }
                    



                }
                else
                {
                    if (hitCount >0)
                    {
                        hitCount--;
                    }
                    aiSteering = 0.0f;
                    acceleration = 0.6f;
                    breakRatio = 0.0f;
                }
                currCarControl.CarMove(aiSteering, acceleration, breakRatio);

                //if (Physics.SphereCast(transform.TransformPoint( leftLocalPoint ), 0.1f, transform.TransformDirection(Vector3.forward), out hit, aiDetectRange, rayCastLayerMark))
                //{
                //    aiSteering = 1f; // turn left steering
                //    Debug.DrawRay(transform.TransformPoint(leftLocalPoint), transform.TransformDirection(Vector3.forward) * hit.distance, Color.red);
                //    Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * aiDetectRange, Color.white);
                //    Debug.DrawRay(transform.TransformPoint(rightLocalPoint), transform.TransformDirection(Vector3.forward) * aiDetectRange, Color.white);
                //    //Debug.LogFormat("Did Hit Left on {0}, hitCount:{1}", hit.collider.name, hitCount);
                //    if (currCarControl.CurrentSpeed < 20 && hit.distance > 2 && hit.distance < 5)
                //    {
                //        currCarControl.CarMove(aiSteering, 1.0f, 0.0f);
                //    }
                //    else
                //    {
                //        currCarControl.CarMove(aiSteering, 0.0f, 1.0f);
                //    }
                    
                //    hitCount++;
                //    //status = "left hit";

                //}
                //else if (Physics.SphereCast(transform.TransformPoint(rightLocalPoint), 0.1f, transform.TransformDirection(Vector3.forward), out hit, aiDetectRange, rayCastLayerMark))
                //{
                //    aiSteering = -1f; // turn right steering
                //    Debug.DrawRay(transform.TransformPoint(rightLocalPoint), transform.TransformDirection(Vector3.forward) * hit.distance, Color.red);
                //    Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * aiDetectRange, Color.white);
                //    Debug.DrawRay(transform.TransformPoint(leftLocalPoint), transform.TransformDirection(Vector3.forward) * aiDetectRange, Color.white);
                //    //Debug.LogFormat("Did Hit Right on {0}, hitCount:{1}", hit.collider.name, hitCount);
                //    if (currCarControl.CurrentSpeed < 20 && hit.distance > 2 && hit.distance < 5)
                //    {
                //        currCarControl.CarMove(aiSteering, 1.0f, 0.0f);
                //    }
                //    else
                //    {
                //        currCarControl.CarMove(aiSteering, 0.0f, 1.0f);
                //    }
                //    hitCount++;
                //    //status = "right hit";
                //}
                //else if (Physics.SphereCast(transform.position, 0.2f, transform.TransformDirection(Vector3.forward), out hit, aiDetectRange, rayCastLayerMark))
                //{

                //    Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * hit.distance, Color.red);
                //    Debug.DrawRay(transform.TransformPoint(leftLocalPoint), transform.TransformDirection(Vector3.forward) * aiDetectRange, Color.white);
                //    Debug.DrawRay(transform.TransformPoint(rightLocalPoint), transform.TransformDirection(Vector3.forward) * aiDetectRange, Color.white);
                //    //Debug.LogFormat("Did Hit Center on {0}, hitCount:{1}", hit.collider.name, hitCount);
                //    if (currCarControl.CurrentSpeed < 20 && hit.distance > 2 && hit.distance < 5)
                //    {
                //        currCarControl.CarMove(aiSteering, 1.0f, 0.0f);
                //    }
                //    else
                //    {
                //        currCarControl.CarMove(aiSteering, 0.0f, 1.0f);
                //    }

                //    hitCount++;
                //    //status = "center hit";
                //}
                //else
                //{
                //    Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * aiDetectRange, Color.white);
                //    Debug.DrawRay(transform.TransformPoint(leftLocalPoint), transform.TransformDirection(Vector3.forward) * aiDetectRange, Color.white);
                //    Debug.DrawRay(transform.TransformPoint(rightLocalPoint), transform.TransformDirection(Vector3.forward) * aiDetectRange, Color.white);
                    
                //    if (hitCount > 0)
                //    {
                //        hitCount--;
                //    }
                //    //status = "no hit";
                   
                //    currCarControl.CarMove(0f, 1f, 0f);
                    
                    
                //}
                if (hitCount > 360)
                {
                    Debug.Log("hitCount over 300, turn around");
                    hitCount = 0;
                    
                    transform.Rotate(0, UnityEngine.Random.Range(0f, 360f), 0, Space.Self);
                    
                    //transform.localRotation *= Quaternion.Euler(0, 180, 0);
                    //status = "hitCount over 300";
                }
                if (currCarControl.CurrentSpeed < 1.0f && !netPlayerManager.targetPlayer)
                {
                    lowSpeedCount++;
                    if (lowSpeedCount > 1800)
                    {
                        Debug.Log("lowSpeedCount over 1800, turn around");
                        transform.Rotate(0, UnityEngine.Random.Range(0f, 360f), 0, Space.Self);
                        
                        lowSpeedCount = 0;

                        //status = "lowSpeedCount over 1800";
                    }
                }
                else if (lowSpeedCount >0)
                {
                    lowSpeedCount--;
                }


            }

            //if (playerInfomationText)
            //{
            //    playerInfomationText.text = string.Format("x:{0:0.00},y:{1:0.00}z={2:0.00}\n{3:0.0000}{4}\nh:{5},v:{6}",
            //        transform.position.x, transform.position.y, transform.position.z,
            //        currCarControl.CurrentSpeed, currCarControl.CurrentSpeedType,
            //        CrossPlatformInputManager.GetAxis("Horizontal"), CrossPlatformInputManager.GetAxis("Vertical")
            //        );
            //}
        }

        public static void UnSwpanPlayer()
        {
            
            currPlayer.UnSwpan();


        }
        public void UnSwpan()
        {
            Debug.LogFormat("unswpan ned id:{0}", gameObject.GetComponent<NetworkIdentity>().netId);
            try
            {
                CmdUnSwpanPayer(gameObject.GetComponent<NetworkIdentity>().netId);
            }
            catch(Exception e )
            {
                Debug.LogException(e);
            }
            
        }
        [Command]
        public void CmdUnSwpanPayer(NetworkInstanceId netId)
        {
            Debug.Log("UnSpawn player");
            NetworkServer.UnSpawn(this.gameObject);
            //TargetDisconnect(connectionToClient, 0);
            
            //NetworkServer.Destroy(NetworkServer.objects[netId].gameObject);
            
        }
        [TargetRpc]
        private void TargetDisconnect(NetworkConnection target, int score)
        {
            Debug.LogFormat("killed, Score: {0}", score);
            NetworkGameController.singleton.StopClient();
            
        }

        public void OnChangePower(int power)
        {
            this.power = power;
            if (currCarControl)
            {
                currCarControl.SetPower(power);
            }
            
        }
        public void OnChangeWeight(int weight)
        {
            this.weight = weight;
            transform.GetComponent<Rigidbody>().mass = maxMass / 100 * weight;
            Debug.LogFormat("weight:{0}, mass:{1}", weight, transform.GetComponent<Rigidbody>().mass);
            
        }
        public void TurnOnAI()
        {
            isAIPlayer = true;
        }

        public bool IsAIPlayer
        {
            get { return isAIPlayer; }
        }

        private void OnCollisionStay(Collision collision)
        {
            if (collision.transform.name == "plaentSphere_6div")
            {
                Debug.Log("collision to plaentSphere_6div");
                upSideDownCount++;
                if (upSideDownCount > 1800)
                {
                    upSideDownCount = 0;
                    //transform.position = transform.TransformDirection(new Vector3(0f, -1f, 0f));
                    transform.Rotate(0f, 0f, 180f, Space.Self);
                    //rigidbody.AddTorque(transform.forward * UnityEngine.Random.Range(0f, 5f));
                }

            }
        }

        private void OnCollisionEnter(Collision collision)
        {
            currCarControl.CarMove(0f, 0f, 0f);
            rigidbody.velocity = Vector3.zero;
        }
    }
}
