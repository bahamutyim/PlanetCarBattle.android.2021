using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityAttracor : MonoBehaviour {

    public float gravity = -10;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    internal void Attract(Transform body)
    {
        Vector3 gravityUp = (body.position - transform.position).normalized;
        Vector3 bodyUp = body.up;

        body.GetComponent<Rigidbody>().AddForce(gravityUp * gravity);

        Quaternion targetRotation = Quaternion.FromToRotation(bodyUp, gravityUp) * body.rotation;
        body.rotation = Quaternion.Slerp(body.rotation, targetRotation, 50 * Time.deltaTime);

    }
}
