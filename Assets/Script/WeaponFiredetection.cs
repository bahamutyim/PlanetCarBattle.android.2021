using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.ahyim.planet
{
    public class WeaponFiredetection : MonoBehaviour
    {

        public WeaponAControl weaponAControl;

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        //private void OnCollisionStay(Collision collision)
        //{
        //    Debug.LogFormat("Collision obj name: {0}", collision.gameObject.name);
        //    if (collision.gameObject.name == "CarCollider")
        //    {
        //        Debug.LogFormat("Fire obj name: {0}", collision.gameObject.name);
        //        weaponAControl.Fire();
        //    }
        //}
        //private void OnCollisionEnter(Collision collision)
        //{
        //    Debug.LogFormat("Collision obj name: {0}", collision.gameObject.name);
        //    if (collision.gameObject.name == "CarCollider")
        //    {
        //        Debug.LogFormat("Fire obj name: {0}", collision.gameObject.name);
        //        weaponAControl.Fire();
        //    }
        //}
        private void OnTriggerStay(Collider other)
        {
            
            //if (other.gameObject.name == "CarCollider")
            //{
                
            //    weaponAControl.Fire(other.gameObject.transform.position);
            //}
        }

    } 
}
