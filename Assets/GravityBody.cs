using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityBody : MonoBehaviour {

    public GravityAttracor attactor;
    private Transform myTransform;
    private Rigidbody rb;

	// Use this for initialization
	void Start () {
        rb = gameObject.GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeRotation;
        rb.useGravity = false;
        myTransform = transform;

	}
	
	// Update is called once per frame
	void Update () {
        attactor.Attract(myTransform);
	}
}
