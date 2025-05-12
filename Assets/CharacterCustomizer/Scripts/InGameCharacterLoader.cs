using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CC;

public class InGameCharacterLoader : MonoBehaviour
{
    [Header("Referência do TPS Character")]
    public GameObject tpsCharacter;

    [Header("Nomes de Presets Ativáveis")]
    public string[] characterNames;

    private Dictionary<string, string> presetMap = new Dictionary<string, string>()
    {
        { "CC_Male", "CCMH_Male" },
        { "CC_Female", "CCMH_Female" },
        { "CC_Male_African", "CCMH_Male_African" },
        { "CC_Female_African", "CCMH_Female_African" },
        { "CC_Male_Asian", "CCMH_Male_Asian" },
        { "CC_Female_Asian", "CCMH_Female_Asian" },
    };

    void Start()
    {
        StartCoroutine(LoadCharacter());
    }

    IEnumerator LoadCharacter()
    {
        string path = GetCharacterJsonPath();
        if (string.IsNullOrEmpty(path))
        {
            Debug.LogWarning("Personagem salvo não encontrado.");
            yield break;
        }

        string json = File.ReadAllText(path);
        var saveData = JsonUtility.FromJson<CC_SaveData>(json);
        if (saveData?.SavedCharacters == null || saveData.SavedCharacters.Count == 0)
        {
            Debug.LogError("Arquivo de personagem inválido.");
            yield break;
        }

        var charData = saveData.SavedCharacters[0];
        string targetName = presetMap.ContainsKey(charData.CharacterPrefab) ? presetMap[charData.CharacterPrefab] : charData.CharacterPrefab;

        GameObject activeCharacter = null;

        foreach (var name in characterNames)
        {
            var go = tpsCharacter.transform.Find($"CharacterParent/{name}")?.gameObject;
            if (go == null) continue;

            bool isTarget = name == targetName;
            go.SetActive(isTarget);
            if (isTarget) activeCharacter = go;
        }

        if (activeCharacter == null)
        {
            Debug.LogError($"❌ Não foi possível ativar o preset: {targetName}");
            yield break;
        }

        yield return null; // aguarda o GameObject ativar completamente

        var customization = activeCharacter.GetComponentInChildren<CharacterCustomization>(true);

        if (customization == null)
        {
            Debug.LogError("❌ CharacterCustomization não encontrado.");
            yield break;
        }

        // Garantir que a MainMesh esteja setada antes de aplicar os dados
        if (customization.MainMesh == null)
        {
            customization.MainMesh = customization
                .GetComponentsInChildren<SkinnedMeshRenderer>(true)
                .FirstOrDefault(s => s.name.ToLower().Contains("body") || s.name.ToLower().Contains("skin"));
        }

        if (customization.MainMesh == null)
        {
            Debug.LogError("❌ MainMesh não foi encontrado. Verifique se o modelo contém um SkinnedMeshRenderer de corpo.");
            yield break;
        }

        // Esperar bones carregarem (caso seja necessário)
        yield return new WaitUntil(() =>
            customization.GetComponentsInChildren<SkinnedMeshRenderer>(true)
            .Any(s => s.bones != null && s.bones.Length > 0));

        // Ativar script
        customization.enabled = true;

        yield return null;

        try
        {
            customization.ApplyCharacterVars(charData);
            Debug.Log("✅ Personagem carregado e aplicado com sucesso.");
        }
        catch (System.Exception e)
        {
            Debug.LogError("❌ Erro ao aplicar personagem: " + e.Message);
            Debug.Log(JsonUtility.ToJson(charData, true));
        }

        // Ocultar partes do corpo padrão
        HideBaseBodyParts();
    }

    void HideBaseBodyParts()
    {
        string[] parts = { "Arms", "Body", "HeadGlass", "Torso" };
        foreach (var part in parts)
        {
            var t = tpsCharacter.transform.Find(part);
            if (t) t.gameObject.SetActive(false);
        }
    }

    string GetCharacterJsonPath()
    {
        string filename = "Admin.json";
        string path = Path.Combine(Application.persistentDataPath, filename);
        if (File.Exists(path)) return path;

#if UNITY_EDITOR
        path = Path.Combine(Application.dataPath, "CharacterCustomizer/SavedCharacters", filename);
        if (File.Exists(path)) return path;
#endif

        return null;
    }
}
