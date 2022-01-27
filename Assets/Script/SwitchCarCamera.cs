using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace com.ahyim.planet
{
    public class SwitchCarCamera : NetworkBehaviour
    {

        private GameObject disabledCamera;
        private Camera carCamera;
        public static Camera currentCamera;

        // Use this for initialization
        void Start()
        {
            

            if (!isLocalPlayer)
            {
                GetComponentInChildren<Camera>().gameObject.SetActive(false);
            }
            else
            {

                //GameObject net = GameObject.FindGameObjectsWithTag("networkManager")[0];
                //if (net == null)
                //{
                //    Debug.Log("net is null");
                //    return;
                //}
                //disabledCamera = net.transform.Find("TempCamera").gameObject;
                if (Camera.current)
                {
                    disabledCamera = Camera.current.gameObject;
                    if (disabledCamera)
                    {
                        disabledCamera.SetActive(false);
                    }
                }

                carCamera = GetComponentInChildren<Camera>();
                currentCamera = carCamera;

                currentCamera.gameObject.SetActive(true);
                //GetComponentInChildren<Billboard>().currCamera = currentCamera;

            }
        }


        void OnDestroy()
        {
            if (isLocalPlayer && disabledCamera != null)
            {
                disabledCamera.transform.position = carCamera.transform.position;
                disabledCamera.transform.rotation = carCamera.transform.rotation;

                disabledCamera.SetActive(true);
                disabledCamera = null;
            }
        }
        
    }
}
