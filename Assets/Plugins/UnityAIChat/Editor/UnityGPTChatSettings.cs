using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace NTY.UnityChatGPT.Editor
{
    [FilePath("UserSettings/UnityGPTChatSettings.asset",
        FilePathAttribute.Location.ProjectFolder)]
    public sealed class UnityGPTChatSettings : ScriptableSingleton<UnityGPTChatSettings>
    {
        public string apiKey = null;
        public void Save() => Save(true);
        void OnDisable() => Save();
    }
    
    sealed class UnityGPTChatSettingsProvider : SettingsProvider
    {
        public  UnityGPTChatSettingsProvider()
            : base("Project/Unity ChatGPT", SettingsScope.Project) {}

        public override void OnGUI(string search)
        {
            var settings = UnityGPTChatSettings.instance;
            var key = settings.apiKey;
            EditorGUI.BeginChangeCheck();
            key = EditorGUILayout.TextField("API Key", key);
            if (EditorGUI.EndChangeCheck())
            {
                settings.apiKey = key;
                settings.Save();
            }
        }

        [SettingsProvider]
        public static SettingsProvider CreateCustomSettingsProvider()
            => new UnityGPTChatSettingsProvider();
    }
}
