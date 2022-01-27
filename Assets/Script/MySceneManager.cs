using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace com.ahyim.planet
{
    public class MySceneManager : MonoBehaviour
    {
        public GameObject loadingPanel;
        public List<GameObject> otherPanelList;
        public UnityEngine.UI.Text loadText;
        public UnityEngine.UI.Image loadImage;
        public void LoadScene(string sceneName)
        {
            SceneManager.LoadScene(sceneName);
        }
        public static void LoadSceneStatic(string sceneName)
        {
            SceneManager.LoadScene(sceneName);
        }

        public void StartLoadNextScreen(string sceneName)
        {
            loadingPanel.SetActive(true);
            foreach(GameObject otherPanel in otherPanelList)
            {
                otherPanel.SetActive(false);
            }
            StartCoroutine(DisplayLoadingScreen(sceneName));
        }

        public IEnumerator DisplayLoadingScreen (string sceneName)
        {
            AsyncOperation async = SceneManager.LoadSceneAsync(sceneName);
            while (!async.isDone)
            {
                loadText.text = string.Format("{0}%", async.progress * 100 );
                loadImage.transform.localScale = new Vector2(async.progress, 1);
                yield return null;
                
            }
        }

    }
}
