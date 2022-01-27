using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

namespace com.ahyim.planet
{
    public class BackCamera : MonoBehaviour
    {

        public GameObject focusObject;
        public Rigidbody carRigiBody;
        private float distance = 7;
        private float height = 0;
        private float smoothTurnTime = 0.2f;
        private float maxDistance = 20;


        private float currentTurning = 0;
        private float turnSpeedVelocityChange;
        private Rigidbody targetRigibody;
        private float targetVelocity;
        private float targetTurning = 0;
        private float initDistance;
        private float zoomStep = 0.15f;

        private float zoomInput;

        // Use this for initialization
        void Start()
        {
            //targetRigibody = target.GetComponent<Rigidbody>();
            initDistance = distance;
            
            //only show gameobject 1in 150 on envirnment layer
            Camera camera = GetComponent<Camera>();
            float[] distances = new float[32];
            distances[10] = 150;
            camera.layerCullDistances = distances;
        }

        // Update is called once per frame
        void Update()
        {
            
            //if (targetVelocity < 20)
            //{
            //    targetTurning = h * 3 * targetVelocity / 20;
            //}
            //else
            //{
            //    targetTurning = h * 3;
            //}

            targetTurning = Mathf.Clamp(carRigiBody.angularVelocity.y, -1f, 1f) * 2;

            currentTurning = Mathf.SmoothDamp(currentTurning, targetTurning, ref turnSpeedVelocityChange, smoothTurnTime);
            //Debug.Log(targetRigibody.velocity.magnitude); 

            //Debug.Log( string.Format("currentTurning={0},targetTurning={1}", currentTurning, targetTurning));
            transform.position = focusObject.transform.TransformPoint(currentTurning, height, -distance);
            //transform.Rotate(upDownAngle, 0, 0);
            //transform.LookAt(focusObject.transform);

            Vector3 camPosition = focusObject.transform.InverseTransformPoint(transform.position);
            



            //Debug.LogFormat("camPosition-x:{0},y:{1},z{2}", camPosition.x, camPosition.y, camPosition.z);


            float degree = Mathf.Atan2(-camPosition.y, -camPosition.z) * Mathf.Rad2Deg;

            //Debug.LogFormat("Atan2 of {0}/{1} = {2}", -camPosition.z, -camPosition.y, degree);



            //Quaternion toRotation = Quaternion.FromToRotation(weaponATransform.up, nearestPlayer.transform.position);
            transform.localEulerAngles = new Vector3(  -degree, 0, 0);

        }
        private void FixedUpdate()
        {
            zoomInput = CrossPlatformInputManager.GetAxis("Zoom");

            if (zoomInput > 0 && distance > 0)
            {
                if (initDistance + height < maxDistance)
                {
                    distance += zoomStep;
                    Debug.LogFormat("Zoom out decrease distance, initDistance:{0},distance:{1},height:{2},maxDistance:{3}", initDistance, distance, height, maxDistance);
                }
                else
                {
                    distance -= zoomStep;
                    Debug.LogFormat("Zoom out increase distance, initDistance:{0},distance:{1},height:{2},maxDistance:{3}", initDistance, distance, height, maxDistance);
                }
                height += zoomStep;
            }
            else if (zoomInput < 0 && height > 0)
            {
                if (initDistance + height < maxDistance)
                {
                    distance -= zoomStep;
                    Debug.LogFormat("Zoom in decrease distance, initDistance:{0},distance:{1},height:{2},maxDistance:{3}", initDistance, distance, height, maxDistance);
                }
                else
                {
                    distance += zoomStep;
                    Debug.LogFormat("Zoom in increase distance, initDistance:{0},distance:{1},height:{2},maxDistance:{3}", initDistance, distance, height, maxDistance);
                }
                height -= zoomStep;
            }

        }
    }
}
