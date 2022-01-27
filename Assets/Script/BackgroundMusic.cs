using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.ahyim.planet
{
    public class BackgroundMusic : MonoBehaviour
    {
        public static BackgroundMusic singleInstance;

        public AudioSource audioSource;

        private void Awake()
        {
#if UNITY_ANDROID || UNITY_IOS
            Debug.LogFormat("BackgroundMusic awake at scene {0}",  UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);

            audioSource = GetComponent<AudioSource>();

            if (singleInstance != null && singleInstance != this)
            {
                if ( singleInstance.audioSource.clip.name == this.audioSource.clip.name)
                {
                    Debug.Log("Same AudioClip, destory this");
                    Destroy(this.gameObject);
                    return;
                }
                else
                {
                    Debug.Log("Different AudioClip, destory single instance");
                    Destroy(singleInstance.gameObject);
                    singleInstance = this;
                }
                
            }
            else
            {
                Debug.Log("no instance");
                singleInstance = this;
            }
            DontDestroyOnLoad(this.gameObject);
#endif
#if UNITY_STANDALONE
            Destroy(this);
#endif
        }

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
    }

}