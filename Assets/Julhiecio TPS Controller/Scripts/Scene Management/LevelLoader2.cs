using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace JUTPS.Utilities
{
    [AddComponentMenu("JU TPS/Scene Management/Scene Loader")]
    public class LevelLoader2 : MonoBehaviour
    {
        [Header("Scene Loading")]
        public string LevelName = "CC_Showcase_Scene";
        public int LevelBuildID = -1;
        public bool LoadOnAwake = false;

        [Header("Optional Fade Transition")]
        public bool UseFade = false;
        public Image FadeImage;
        public float FadeDuration = 1f;

        void Awake()
        {
            if (LoadOnAwake)
            {
                LoadLevel();
            }
        }

        public void LoadLevel()
        {
            if (UseFade && FadeImage != null)
            {
                StartCoroutine(FadeAndLoad());
            }
            else
            {
                LoadScene();
            }
        }

        public void LoadLevel(string levelName)
        {
            SceneManager.LoadScene(levelName);
        }

        public void LoadLevel(int levelID)
        {
            SceneManager.LoadScene(levelID);
        }

        public void LoadLevelInSeconds(float Seconds)
        {
            Invoke(nameof(LoadLevel), Seconds);
        }

        private void LoadScene()
        {
            if (LevelBuildID > -1)
            {
                SceneManager.LoadScene(LevelBuildID);
            }
            else
            {
                SceneManager.LoadScene(LevelName);
            }
        }

        private IEnumerator FadeAndLoad()
        {
            // Fade to black
            float timer = 0f;
            Color color = FadeImage.color;
            while (timer < FadeDuration)
            {
                timer += Time.deltaTime;
                color.a = Mathf.Clamp01(timer / FadeDuration);
                FadeImage.color = color;
                yield return null;
            }

            LoadScene();
        }
    }
}
