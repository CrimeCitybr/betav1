using UnityEngine;
using TMPro;
using CC;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

public class CharacterSaveButton : MonoBehaviour
{
    public TMP_InputField characterNameField;
    private CharacterCustomization customizer;

    void Start()
    {
        customizer = FindAnyObjectByType<CharacterCustomization>();

        if (customizer == null)
        {
            Debug.LogError("❌ CharacterCustomization não encontrado na cena!");
        }
    }

    public void SaveCharacter()
    {
        string name = characterNameField != null ? characterNameField.text.Trim() : "DefaultCharacter";

        if (string.IsNullOrEmpty(name))
        {
            Debug.LogWarning("⚠️ Digite um nome para salvar o personagem.");
            return;
        }

        if (customizer == null)
        {
            Debug.LogError("❌ CharacterCustomization está nulo!");
            return;
        }

        // ✅ Acessa o campo real com os dados do personagem (confirmado via scanner)
        FieldInfo dataField = customizer.GetType().GetField("StoredCharacterData", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

        if (dataField == null)
        {
            Debug.LogError("❌ Campo 'StoredCharacterData' não encontrado dentro de CharacterCustomization.");
            return;
        }

        var characterData = dataField.GetValue(customizer) as CC_CharacterData;

        if (characterData == null)
        {
            Debug.LogError("❌ StoredCharacterData está null. Verifique se o personagem já foi montado.");
            return;
        }

        characterData.CharacterName = name;

        // Cria o container e adiciona o personagem
        CC_SaveData saveData = new CC_SaveData();
        saveData.SavedCharacters.Add(characterData);

        // Serializa
        string json = JsonUtility.ToJson(saveData, true);
        string savePath = Path.Combine(Application.persistentDataPath, name + ".json");

        try
        {
            File.WriteAllText(savePath, json);
            Debug.Log($"✅ Personagem '{name}' salvo com sucesso em: {savePath}");
        }
        catch (System.Exception ex)
        {
            Debug.LogError("❌ Erro ao salvar JSON: " + ex.Message);
        }

#if UNITY_EDITOR
        string editorSaveDir = Path.Combine(Application.dataPath, "CharacterCustomizer/SavedCharacters");
        string editorSavePath = Path.Combine(editorSaveDir, name + ".json");

        try
        {
            if (!Directory.Exists(editorSaveDir))
                Directory.CreateDirectory(editorSaveDir);

            File.Copy(savePath, editorSavePath, true);
            Debug.Log($"📦 Copiado para: {editorSavePath}");
        }
        catch (System.Exception ex)
        {
            Debug.LogWarning("⚠️ Erro ao copiar para o editor: " + ex.Message);
        }
#endif
    }
}
