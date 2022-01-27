using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace com.ahyim.planet
{
    public class HealthUIBar : MonoBehaviour
    {
        public static HealthUIBar singleton;

        private Slider healthBarSlider;
        

        private void Awake()
        {
            singleton = this;
            //Debug.Log("HealthUIBar assigned to singleton");
        }
        // Use this for initialization
        void Start()
        {
            healthBarSlider = GetComponent<Slider>();
            healthBarSlider.value = 1;

        }

        // Update is called once per frame
        void Update()
        {

        }

        public void UpdateHealthBar(float healthRatio)
        {
            //Debug.LogFormat("update HealthBar, health ratio: {0}", healthRatio);
            healthBarSlider.value = healthRatio;
        }

    } 
}
