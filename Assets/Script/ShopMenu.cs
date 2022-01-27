using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_ANDROID || UNITY_IOS
using UnityEngine.Purchasing;
#endif

namespace com.ahyim.planet
{
    public class ShopMenu : MonoBehaviour
    {


        private MainMenu mainMenu;
        // Use this for initialization
        void Start()
        {
            mainMenu = GetComponent<MainMenu>();
        }

        // Update is called once per frame
        void Update()
        {

        }

        public void Buy1000Gold()
        {
            
        }


        
    } 
}
