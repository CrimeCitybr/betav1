using UnityEngine;
using System.Reflection;
using CC;

public class CharacterDebug : MonoBehaviour
{
    void Start()
    {
        var customizer = FindAnyObjectByType<CharacterCustomization>();

        if (customizer == null)
        {
            Debug.LogError("❌ CharacterCustomization não encontrado na cena.");
            return;
        }

        Debug.Log("🔎 Inspecionando campos de CharacterCustomization:");

        FieldInfo[] fields = customizer.GetType().GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);

        foreach (FieldInfo field in fields)
        {
            string fieldName = field.Name;
            string fieldType = field.FieldType.Name;
            object value = field.GetValue(customizer);

            string valueStr = value != null ? value.ToString() : "null";

            Debug.Log($"🧩 {fieldName} ({fieldType}) = {valueStr}");
        }

        Debug.Log("✅ Inspeção finalizada.");
    }
}
