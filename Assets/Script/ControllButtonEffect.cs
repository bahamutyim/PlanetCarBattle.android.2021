using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.ahyim.planet
{
    public class ControllButtonEffect : MonoBehaviour
    {

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        public void RoateTranformToX45(Transform transform)
        {
            transform.Rotate(Vector3.left, 45.0f);
        }

        public void RoateTranformToZero(Transform transform)
        {
            transform.rotation = Quaternion.identity;
        }

        public void ShrinkSize(Transform transform)
        {
            transform.localScale = new Vector3(0.8f, 0.8f, 1f);
        }

        public void RestoreSize(Transform transform)
        {
            transform.localScale = new Vector3(1, 1, 1);
        }
    } 
}
