using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

public class FingerControl : MonoBehaviour {


   private Rigidbody dragRigibody;
    private GameObject dragObject;
    private unlockCar currUnlockCar;

    // Use this for initialization
    void Start () {
        currUnlockCar = GetComponentInChildren<unlockCar>();
        dragObject = GameObject.FindGameObjectsWithTag("dargObject")[0];
        dragRigibody = dragObject.transform.GetComponent<Rigidbody>();
        GetComponent<SpringJoint>().connectedBody = dragRigibody;
        //dragObject.transform.position = transform.position;
    }

    // Update is called once per frame
    private void FixedUpdate()
    {       
        float h = CrossPlatformInputManager.GetAxis("Horizontal");
        float v = CrossPlatformInputManager.GetAxis("Vertical");
        Debug.Log(string.Format("h={0},v={1}", h, v));
        //dragRigibody.AddForce( transform.forward * 10000*v );
        //dragRigibody.AddForce(transform.right * 10000 * h);
        //Vector3 vVector = transform.forward * 100 * v;
        //Vector3 hVector = transform.right * 100 * v;
        //dragRigibody.velocity = vVector;
        //currCarControl.CarMove(h, 0.0000000001f, 0f);
        dragRigibody.MovePosition(transform.TransformDirection(h , 0, v));
        currUnlockCar.updateSteering(h);
    }
}
