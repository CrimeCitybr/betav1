using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NTY.UnityChatGPT
{
    public static class UnityGPTWrapper
    {
        public const string Url = "https://api.openai.com/v1/chat/completions";
    }

        [Serializable]
        public struct ResponseMessage
        {
            public string role;
            public string content;
        }

        [Serializable]
        public struct ResponseChoice
        {
            public int index;
            public ResponseMessage message;
        }

        [Serializable]
        public struct Response
        {
            public string id;
            public ResponseChoice[] choices;
        }

        [Serializable]
        public struct RequestMessage
        {
            public string role;
            public string content;
        }

        [Serializable]
        public struct Request
        {
            public string model;

            // public string prompt;
            public RequestMessage[] messages;
        }
    
}
