using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.ahyim.planet
{
    public class centreGravity : MonoBehaviour
    {

        public GameObject centre;
        public float gravity = -9.8f;
        private Rigidbody rb;



        // Use this for initialization
        void Start()
        {
            Debug.LogFormat("{0} is start with tag:{1}", name, tag);
            centre = GameObject.FindWithTag("centre");
            if (!centre)
            {
                Debug.LogError("centre is null");
            }
            rb = gameObject.GetComponent<Rigidbody>();
            //rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
            rb.useGravity = false;



        }

        // Update is called once per frame
        void Update()
        {
            
            if (centre != null)
            {

                
                Vector3 direction = (transform.position - centre.transform.position).normalized;
                Debug.DrawRay(transform.position, direction, Color.red);
                rb.AddForce(direction * gravity * rb.mass);

                

                //Quaternion toRotation = Quaternion.FromToRotation(transform.up, direction) * transform.rotation;
                //Debug.Log(toRotation);
                //transform.rotation = toRotation;// Quaternion.Slerp(transform.rotation, toRotation, 50 * Time.deltaTime);





            }
        }
    }
}
