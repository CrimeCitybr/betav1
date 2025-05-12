using UnityEngine;
using System.IO;
using System.Reflection;
using System.Linq;
using System.Collections;
using CC;

public class CharacterLoader : MonoBehaviour
{
    private CharacterCustomization customizer;

    void Start()
    {
        customizer = FindAnyObjectByType<CharacterCustomization>();

        if (customizer == null)
        {
            Debug.LogError("❌ CharacterCustomization não encontrado na cena!");
            return;
        }

        string latestCharacter = GetLatestJsonFileName();

        if (!string.IsNullOrEmpty(latestCharacter))
        {
            StartCoroutine(DelayedLoadCharacter(latestCharacter));
        }
        else
        {
            Debug.LogWarning("⚠️ Nenhum personagem salvo encontrado para carregar.");
        }
    }

    // ✅ Método público pro botão UI (OnClick)
    public void OnClickLoadCharacter()
    {
        string latestCharacter = GetLatestJsonFileName();

        if (!string.IsNullOrEmpty(latestCharacter))
        {
            StartCoroutine(DelayedLoadCharacter(latestCharacter));
        }
        else
        {
            Debug.LogWarning("⚠️ Nenhum personagem salvo encontrado para carregar.");
        }
    }

    // ⏳ Aguarda sistema estar pronto antes de aplicar o JSON
    private IEnumerator DelayedLoadCharacter(string name)
    {
        yield return null; // espera um frame (caso ainda esteja inicializando)

        // Espera o campo interno 'StoredCharacterData' ser inicializado
        FieldInfo storedDataField = null;
        int timeout = 30; // segurança para evitar loop infinito

        while (storedDataField == null && timeout-- > 0)
        {
            storedDataField = customizer.GetType().GetField(
                "StoredCharacterData",
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic
            );

            yield return new WaitForSeconds(0.1f);
        }

        if (storedDataField == null)
        {
            Debug.LogError("❌ Campo 'StoredCharacterData' nunca foi encontrado.");
            yield break;
        }

        // Aguarda um segundo extra para garantir que a UI e listas estejam prontas
        yield return new WaitForSeconds(1f);

        LoadCharacter(name);
    }

    // 💾 Carrega o JSON salvo
    private void LoadCharacter(string nameWithoutExtension)
    {
        string path = Path.Combine(Application.persistentDataPath, nameWithoutExtension + ".json");

#if UNITY_EDITOR
        if (!File.Exists(path))
        {
            string editorPath = Path.Combine(Application.dataPath, "CharacterCustomizer/SavedCharacters", nameWithoutExtension + ".json");
            if (File.Exists(editorPath))
                path = editorPath;
        }
#endif

        if (!File.Exists(path))
        {
            Debug.LogError($"❌ Arquivo '{nameWithoutExtension}.json' não encontrado em:\n{path}");
            return;
        }

        string json = File.ReadAllText(path);
        CC_SaveData saveData = JsonUtility.FromJson<CC_SaveData>(json);

        if (saveData == null || saveData.SavedCharacters.Count == 0)
        {
            Debug.LogError("❌ JSON inválido ou sem personagens.");
            return;
        }

        CC_CharacterData characterData = saveData.SavedCharacters[0];

        // 🧠 Setar 'StoredCharacterData'
        FieldInfo storedDataField = customizer.GetType().GetField(
            "StoredCharacterData",
            BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic
        );

        if (storedDataField != null)
        {
            storedDataField.SetValue(customizer, characterData);
            Debug.Log($"✅ Dados de '{characterData.CharacterName}' aplicados ao StoredCharacterData.");
        }
        else
        {
            Debug.LogError("❌ Campo 'StoredCharacterData' não encontrado.");
            return;
        }

        // 🧩 Chamar ApplyCharacterVars()
        MethodInfo applyMethod = customizer.GetType().GetMethod(
            "ApplyCharacterVars",
            BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic
        );

        if (applyMethod != null)
        {
            applyMethod.Invoke(customizer, new object[] { characterData });
            Debug.Log($"🎨 Personagem '{characterData.CharacterName}' carregado com sucesso!");
        }
        else
        {
            Debug.LogError("❌ Método 'ApplyCharacterVars' não encontrado!");
        }
    }

    // 🔍 Pega o nome do último JSON salvo
    private string GetLatestJsonFileName()
    {
        string[] possibleDirs = {
            Application.persistentDataPath,
#if UNITY_EDITOR
            Path.Combine(Application.dataPath, "CharacterCustomizer/SavedCharacters")
#endif
        };

        foreach (string dir in possibleDirs)
        {
            if (!Directory.Exists(dir)) continue;

            var files = Directory.GetFiles(dir, "*.json");

            if (files.Length > 0)
            {
                string latestFile = files.OrderByDescending(File.GetLastWriteTime).FirstOrDefault();
                if (!string.IsNullOrEmpty(latestFile))
                {
                    string name = Path.GetFileNameWithoutExtension(latestFile);
                    Debug.Log($"📦 Último JSON detectado: {name}");
                    return name;
                }
            }
        }

        return null;
    }
}
