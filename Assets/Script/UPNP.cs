using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Threading;
using Mono.Nat;

public class UPNP : MonoBehaviour {

    public static UPNP singleton;
    private List<Mapping> upnpMapingList = new List<Mapping>();

    private void Awake()
    {
        singleton = this;
        DontDestroyOnLoad(this);
        
    }

    // Use this for initialization
    void Start () {

        NatUtility.DeviceFound += DeviceFound;
        NatUtility.DeviceLost += DeviceLost;
        AddMapping();
        NatUtility.StartDiscovery();

    }

    private void AddMapping()
    {
        upnpMapingList.Add(new Mapping(Protocol.Udp, 7777, 7777));
    }

    private void DeviceFound(object sender, DeviceEventArgs args)
    {
        INatDevice device = args.Device;
        Debug.LogFormat("DeviceFound call");
        foreach( Mapping mapping in upnpMapingList )
        {
            try
            {
                device.CreatePortMap(mapping);
                Debug.LogFormat("DeviceFound create mapping, Protocol:{0}, PublicPort:{1}, PrivatePort:{2}", mapping.Protocol, mapping.PublicPort, mapping.PrivatePort);
            }
            catch(Exception ex)
            {
                Debug.LogFormat("DeviceFound Exception: {0}", ex.Message);
            }
        }
        
    }

    private void DeviceLost(object sender, DeviceEventArgs args)
    {
        INatDevice device = args.Device;
        Debug.LogFormat("DeviceLost call");
        foreach (Mapping mapping in upnpMapingList)
        {
            try
            {
                device.DeletePortMap(mapping);
                Debug.LogFormat("DeviceLost delete mapping, Protocol:{0}, PublicPort:{1}, PrivatePort:{2}", mapping.Protocol, mapping.PublicPort, mapping.PrivatePort);
            }
            catch(Exception ex)
            {
                Debug.LogFormat("DeviceLost Exception: {0}", ex.Message);
            }
        }
    }




}
