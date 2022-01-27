using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace com.ahyim.planet
{ 
    public class DebugText : MonoBehaviour {

        //TextMesh textMesh;
        private UnityEngine.UI.Text debugText;

        private int index = 1;

        // Use this for initialization
        void Start()
        {
            debugText = gameObject.GetComponent<UnityEngine.UI.Text>();
        }

        void OnEnable()
        {
            Application.logMessageReceived += LogMessage;
        }

        void OnDisable()
        {
            Application.logMessageReceived -= LogMessage;
        }

        public void LogMessage(string message, string stackTrace, LogType type)
        {
            if (debugText)
            {
                debugText.text = index + ". " + message + "\n" + debugText.text;
                index++;

                if (debugText.text.Length > 800)
                {
                    debugText.text = debugText.text.Substring(0, 800);
                }
            }
            //if (debugText.text.Length > 300)
            //{
            //    debugText.text = message + "\n";
            //}
            //else
            //{
            //    debugText.text += message + "\n";
            //}
        }
    }
}
