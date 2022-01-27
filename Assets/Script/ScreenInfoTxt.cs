using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace com.ahyim.planet
{
    public class ScreenInfoTxt : MonoBehaviour
    {

        public Text screenInfoText;
        private string temp;

        // Use this for initialization
        void Start()
        {
            temp = string.Format("{0}x{1}\nDPI:{2}\nFPS:", Screen.currentResolution.width, Screen.currentResolution.height, Screen.dpi);
            screenInfoText.text = temp;

            temp = temp + "{0:0.00}";

        }

        // Update is called once per frame
        void Update()
        {
            screenInfoText.text = string.Format(temp, (int) 1 / Time.deltaTime);
        }
    } 
}
