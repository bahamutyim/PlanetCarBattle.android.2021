using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.ahyim.planet
{

    public class NewPlayerMenu : MonoBehaviour
    {

        public GameObject NewPlayerPanel;
        public GameObject messagePanel;

        private GameObject activePanel;

        // Use this for initialization
        void Start()
        {
            DisableAllPanel();
            activePanel = NewPlayerPanel;
            NewPlayerPanel.SetActive(true);

        }

        // Update is called once per frame
        void Update()
        {

        }

        public void ShowNewPlayerPanel()
        {
            //DisableAllPanel();
            //authoriseMethodPanel.SetActive(true);
            FadeOutAndFadeIn(activePanel, NewPlayerPanel);
        }

       


        private void DisableAllPanel()
        {
            if (NewPlayerPanel)
            {
                NewPlayerPanel.SetActive(false);
            }
            

        }

        private void FadeOutAndFadeIn(GameObject fadeOutPanel, GameObject fadeInPanel)
        {
            StartCoroutine(DoFadeInOut(fadeOutPanel, fadeInPanel));
        }

        private IEnumerator DoFadeInOut(GameObject fadeOutPanel, GameObject fadeInPanel)
        {
            activePanel = fadeInPanel;
            CanvasGroup canvasGroup = fadeOutPanel.GetComponent<CanvasGroup>();
            while (canvasGroup.alpha > 0)
            {
                canvasGroup.alpha -= Time.deltaTime * 4;
                yield return null;
            }
            fadeOutPanel.SetActive(false);
            canvasGroup = fadeInPanel.GetComponent<CanvasGroup>();
            canvasGroup.alpha = 0;
            fadeInPanel.SetActive(true);
            while (canvasGroup.alpha < 1)
            {
                canvasGroup.alpha += Time.deltaTime * 4;
                yield return null;
            }

            yield return null;
        }

        public void ShowMessage(string message)
        {
            UnityEngine.UI.Text messageText = messagePanel.transform.Find("Message").GetComponent<UnityEngine.UI.Text>();
            messageText.text = message;
            messagePanel.SetActive(true);
        }
        public void ShowMessage(string message, params object[] args)
        {
            ShowMessage(string.Format(message, args));

        }

        public void HideMessage()
        {
            messagePanel.SetActive(false);
        }
    }

}