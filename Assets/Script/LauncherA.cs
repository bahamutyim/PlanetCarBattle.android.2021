using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.ahyim.planet
{
    public class LauncherA : MonoBehaviour
    {

        // Use this for initialization
        void Start()
        {
            WeaponAControl weaponAControl = GetComponentInParent<WeaponAControl>();
            if (weaponAControl)
            {
                weaponAControl.launcher = transform;
            }
            
    
    }

        // Update is called once per frame
        void Update()
        {

        }
    } 
}
