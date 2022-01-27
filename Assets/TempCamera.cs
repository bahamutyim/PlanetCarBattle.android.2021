using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class TempCamera : NetworkManager
{
    public GameObject tempCamera;

    

    public override void OnClientConnect(NetworkConnection conn)
    {
        base.OnClientConnect(conn);
        //tempCamera.SetActive(false);
    }

    public override void OnClientDisconnect(NetworkConnection conn)
    {
        base.OnClientDisconnect(conn);
        //tempCamera.SetActive(true);
    }

    public override void OnServerConnect(NetworkConnection conn)
    {
        base.OnServerConnect(conn);
        //tempCamera.SetActive(false);
    }

    public override void OnServerDisconnect(NetworkConnection conn)
    {
        base.OnServerDisconnect(conn);
        //tempCamera.SetActive(true);
    }

    // Use this for initialization
    void Start () {
        
    }
	
	
}
