using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class unlockCar : MonoBehaviour {

    public List<AxleInfo> axleInfos;

    // Use this for initialization
    void Start()
    {
        foreach (AxleInfo axleInfo in axleInfos)
        {
            axleInfo.leftWheel.motorTorque = 0.000001f;
            axleInfo.rightWheel.motorTorque = 0.000001f;
        }
    }

    // Update is called once per frame
    void Update()
    {
        foreach (AxleInfo axleInfo in axleInfos)
        {
            ApplyLocalPositionToVisuals(axleInfo.leftWheel);
            ApplyLocalPositionToVisuals(axleInfo.rightWheel);
        }
    }

    public void updateSteering(float steering)
    {
        foreach (AxleInfo axleInfo in axleInfos)
        {
            if (axleInfo.steering)
            {
                axleInfo.leftWheel.steerAngle = 30f * steering;
                axleInfo.rightWheel.steerAngle = 30f * steering;
            }

            
        }
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

    [System.Serializable]
    public class AxleInfo
    {
        public WheelCollider leftWheel;
        public WheelCollider rightWheel;
        public bool motor;
        public bool steering;
        

    }
}
