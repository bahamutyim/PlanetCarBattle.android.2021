using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace com.ahyim.planet
{
    public class GameMenu : MonoBehaviour
    {

        public static GameMenu singleton;

        public GameObject gameUIPanel;
        public GameObject gameOverPanel;
        public Text scoreText;
        public AudioSource gameOverAudioSource;

        // Use this for initialization
        void Start()
        {
            singleton = this;
            DisableAllPanel();
            gameUIPanel.SetActive(true);

        }

        // Update is called once per frame
        void Update()
        {

        }

        private void DisableAllPanel()
        {
            gameUIPanel.SetActive(false);
            gameOverPanel.SetActive(false);
        }

        public void ShowGameOverPanel(float delayTime)
        {
            DisableAllPanel();
            StartCoroutine(delayShowGameOverPanel(delayTime));


        }

        public IEnumerator delayShowGameOverPanel(float delayTime)
        {
            yield return new WaitForSeconds(delayTime);
            gameOverPanel.SetActive(true);
            gameOverAudioSource.Play();

        }

        public void udpateGameOverScore(int score)
        {
            scoreText.text = string.Format("{0:,0}", score);
        }
    } 
}
