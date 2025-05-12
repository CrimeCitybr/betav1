using UnityEngine;
using System.Reflection;
using CC;

public class CharacterDataScanner : MonoBehaviour
{
    void Start()
    {
        CharacterCustomization customizer = FindAnyObjectByType<CharacterCustomization>();

        if (customizer == null)
        {
            Debug.LogError("❌ CharacterCustomization não encontrado!");
            return;
        }

        Debug.Log("🔍 SCANNER — Listando todos os campos que contenham CC_CharacterData:");

        FieldInfo[] allFields = customizer.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

        foreach (FieldInfo field in allFields)
        {
            if (field.FieldType == typeof(CC_CharacterData))
            {
                object val = field.GetValue(customizer);
                Debug.Log($"✅ ENCONTRADO: {field.Name} (CC_CharacterData) = {(val != null ? "OK" : "null")}");
            }
        }

        Debug.Log("✅ Fim da varredura.");
    }
}
