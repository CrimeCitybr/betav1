using System;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor;

namespace NTY.UnityChatGPT.Editor
{
    public class UnityGPTWindow : EditorWindow
    {

        public string promptInput = "Ask your question...";
        private string promptOutput = "";

        private Button sendPromptButton;
        private Button clearFieldsButton;
        private TextField promptInputTextField;
        private TextField outputTextField;
        private VisualElement loadingImage;
        
        
        private ScrollView inputScrollView;
        private ScrollView outputScrollView;

        private float loaderRotationSpeed = 10f;
        private float currentZAngle = 0;
        private int minOutputFieldHeight = 800;
        private int minInputFieldHeight = 100;
        private bool waitingForResponse = false;
        
        bool apiKeySet
            => !string.IsNullOrEmpty(UnityGPTChatSettings.instance.apiKey);
        
        const string apiError =
            "API Key not set. Please check the project settings " +
            "(Edit > Project Settings > Unity GPT Chat > API Key).";
        
        [MenuItem("NTY/Unity ChatGPT")]
        static void Init()
        {
            UnityGPTWindow window = (UnityGPTWindow) GetWindow(typeof(UnityGPTWindow));
            window.name = "NTY ChatGPT";
            window.titleContent.text = "Unity ChatGPT";
            window.Show();
        }

        private void CreateGUI()
        {
            if (apiKeySet)
            InitUXML();
        }

        private void OnGUI()
        {
            if (!apiKeySet) EditorGUILayout.HelpBox(apiError, MessageType.Error);
        }

        private void Update()
        {
            if (waitingForResponse)
            {
                currentZAngle += loaderRotationSpeed * Time.deltaTime;
                loadingImage.transform.rotation = Quaternion.Euler(0f,0f,currentZAngle);
            }   
        }


        //Init uxml window and ui elements
        private void InitUXML()
        {
            var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Plugins/UnityAIChat/EditorWindow/NTGPTChat.uxml");
            VisualElement visualElement = visualTree.Instantiate();
            rootVisualElement.Add(visualElement);

            sendPromptButton = rootVisualElement.Q<Button>("Send");
            clearFieldsButton = rootVisualElement.Q<Button>("Clear");
            
            promptInputTextField = rootVisualElement.Q<TextField>("InputTextField");
            outputTextField = rootVisualElement.Q<TextField>("OutputTextField");
            
            outputScrollView = rootVisualElement.Q<ScrollView>("OutputScrollView");
            inputScrollView = rootVisualElement.Q<ScrollView>("InputScrollView");
            

            outputScrollView.contentContainer.style.minHeight = minOutputFieldHeight;
            inputScrollView.contentContainer.style.minHeight = minInputFieldHeight;
            

            promptInputTextField.value = promptInput;
            // promptInputTextField.;
            
            loadingImage = rootVisualElement.Q("Loader");
            
            SetOutputText(promptOutput);
            sendPromptButton.clicked += GenerateResponse;
            clearFieldsButton.clicked += ClearFields;
        }

        //Send prompt request
        void GenerateResponse()
        {
            promptOutput += promptInputTextField.value + "\n" + "\n";
            UnityGPTUtility.SendChat(promptInputTextField.value,(i) => AddResponseToOutputText(i));
            waitingForResponse = true;
            SetLoaderImageVisible(true);
        }

        void AddResponseToOutputText(string response)
        {
            promptOutput += response.Trim() + "\n" + "\n";
            SetOutputText(promptOutput);
        }

        void SetOutputText(string outputText)
        {
            outputTextField.value = outputText;
            waitingForResponse = false;
            SetLoaderImageVisible(false);
        }

        void SetLoaderImageVisible(bool state)
        {
            loadingImage.visible = state;
        }

        void ClearFields()
        {
            promptOutput = "";
            promptInputTextField.value = "";
            UnityGPTUtility.StopAsyncOperation();
            currentZAngle = 0;
            SetOutputText("");
        }

    }
}
