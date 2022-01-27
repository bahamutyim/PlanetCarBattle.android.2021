using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace com.ahyim.planet
{
    public class SpeedUIText : MonoBehaviour
    {

        public static SpeedUIText singleton;

        public CarControl.SpeedType speedType = CarControl.SpeedType.KPH;

        private Text speedText;

        private void Awake()
        {
            singleton = this;
        }

        // Use this for initialization
        void Start()
        {
            speedText = GetComponent<Text>();

        }

        // Update is called once per frame
        void Update()
        {

        }

        public void UpdateSpeed(float sppeed)
        {
            if (speedText)
            {
                speedText.text = string.Format("{0} {1}", Mathf.FloorToInt( sppeed), speedType);
            }
        }
    }

}