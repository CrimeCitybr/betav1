using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace JUTPS.Utilities
{
    [AddComponentMenu("JU TPS/Scene Management/Scene Loader")]
    public class LevelLoader : MonoBehaviour
    {
        [Header("Scene Settings")]
        public string LevelName = "Sample Scene";
        public int LevelBuildID = -1;

        [Tooltip("Se verdadeiro, a cena será carregada automaticamente ao iniciar")]
        public bool LoadOnAwake = false;

        private bool hasLoaded = false; // Proteção para não carregar mais de uma vez

        void Awake()
        {
            // Carrega automaticamente apenas se for explicitamente ativado
            if (LoadOnAwake && !hasLoaded)
            {
                hasLoaded = true;
                LoadLevel();
            }
        }

        /// <summary>
        /// Carrega a cena definida no Inspector.
        /// </summary>
        public void LoadLevel()
        {
            if (hasLoaded) return;
            hasLoaded = true;

            if (LevelBuildID > -1)
            {
                Debug.Log($"[LevelLoader] Carregando cena por ID: {LevelBuildID}");
                SceneManager.LoadScene(LevelBuildID);
            }
            else if (!string.IsNullOrEmpty(LevelName))
            {
                Debug.Log($"[LevelLoader] Carregando cena por nome: {LevelName}");
                SceneManager.LoadScene(LevelName);
            }
            else
            {
                Debug.LogWarning("[LevelLoader] Nenhuma cena definida para carregar!");
            }
        }

        /// <summary>
        /// Carrega uma cena específica por nome.
        /// </summary>
        public void LoadLevel(string levelName)
        {
            Debug.Log($"[LevelLoader] Carregando cena dinamicamente: {levelName}");
            SceneManager.LoadScene(levelName);
        }

        /// <summary>
        /// Carrega uma cena específica por ID.
        /// </summary>
        public void LoadLevel(int levelID)
        {
            Debug.Log($"[LevelLoader] Carregando cena por ID: {levelID}");
            SceneManager.LoadScene(levelID);
        }

        /// <summary>
        /// Aguarda um tempo antes de carregar a cena definida.
        /// </summary>
        public void LoadLevelInSeconds(float seconds)
        {
            Debug.Log($"[LevelLoader] Carregando cena em {seconds} segundos");
            Invoke(nameof(LoadLevel), seconds);
        }
    }
}
