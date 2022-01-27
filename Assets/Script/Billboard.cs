using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Billboard : MonoBehaviour {

    public Camera currCamera;

	// Use this for initialization
	void Start () {
        

    }
	
	// Update is called once per frame
	void Update () {
        
        if(currCamera)
        {
            transform.rotation = currCamera.transform.rotation;
        }
            
        
        
        
    }
}
