using System;
using System.Collections;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;

namespace NTY.UnityChatGPT.Editor
{
    static class UnityGPTUtility
    {
        private static Task<string> _asyncOperation;
        
        static string CreateChatRequestJSON(string prompt)
        {
            //Creating new json req
            var msg = new RequestMessage(); 
            msg.role = "user";
            msg.content = prompt;

            
            var req = new Request();
            //Setting model req 
            req.model = "gpt-3.5-turbo";
            req.messages = new [] { msg };

            return JsonUtility.ToJson(req);
        }
        
        public static string SendChat(string message,Action<string> action)
        {
            if (_asyncOperation == null)
            {
                _asyncOperation = GetAsyncResponse(message,action);
            }
            return "";
        }

        public static void StopAsyncOperation()
        {
            if (_asyncOperation != null)
            {
                _asyncOperation.Dispose();
            }
            _asyncOperation = null;
        }
        

        public static async Task<string> GetAsyncResponse(string message,Action<string> action)
        {
            // Async operation code...
            using var post = UnityWebRequest.Post
                (UnityGPTWrapper.Url,CreateChatRequestJSON(message),"application/json");
            
            //Add Auth + Bearer with api key to header
            post.SetRequestHeader
                ("Authorization", "Bearer " + UnityGPTChatSettings.instance.apiKey );
            //Sending request
            var req = post.SendWebRequest();
            while (!req.isDone)
            {
                await Task.Delay(1);
            }
            var json = post.downloadHandler.text;
            var data = JsonUtility.FromJson<Response>(json);
            action?.Invoke(data.choices[0].message.content);
            _asyncOperation = null;
            return "";
        }

    }
}
