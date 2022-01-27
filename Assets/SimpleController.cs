using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleController : MonoBehaviour {

    public float moveSpeed = 15;
    private Vector3 moveDir;

    private Rigidbody rb;

	// Use this for initialization
	void Start () {
        rb = gameObject.GetComponent<Rigidbody>();
	}
	
	// Update is called once per frame
	void Update () {
        moveDir = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")).normalized;
        Debug.Log(moveDir);
        
	}


    void FixedUpdate()
    {
        rb.MovePosition(rb.position + transform.TransformDirection(moveDir) * moveSpeed * Time.deltaTime);
    }
}
