using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Framework.Test
{
    public class TestLog : MonoBehaviourSingleton<TestLog>
    {
        public Button logBtn;
        public InputField inputInfoUI;

        public Transform zi, zi3, zi4;
        public MinMax<string> _mmString;
        //public MinMax<int> _mmInt;
        //public MinMax<bool> _mmBool;
        //public MinMax<LogType> _mmEnum;
        //public MinMax<float> _mmFloat;
        //public MinMax<float> _mmFloat1;

        // Start is called before the first frame update
        void Start()
        {
            logBtn.onClick.AddListener(() =>
            {
                OnLog();
            });

            zi = transform.FindOf("zi1/zi2");
            zi3 = transform.FindOf("zi3");
            zi4 = transform.FindOf("zi3/zi4");
            
        }

        public void OnLog()
        {
            string input = inputInfoUI.text;

            Debug.Log($"��ͨ ��־��{input}");
            Debug.LogWarning($"���� ��־��{input}");
            Debug.LogError($"���� ��־��{input}");
            Debug.LogException(new Exception($"�쳣 ��־��{input}"));
            Debug.LogAssertion($"���� ��־��{input}");

            Debug.Log($"<color=#39FF00>����ɫ</color> ��ͨ ��־��{input}");
            Debug.LogWarning($"<color=#39FF00>����ɫ</color> ���� ��־��{input}");
            Debug.LogError($"<color=#39FF00>����ɫ</color> ���� ��־��{input}");
            Debug.LogException(new Exception($"<color=#39FF00>����ɫ</color> �쳣 ��־��{input}"));
            Debug.LogAssertion($"<color=#39FF00>����ɫ</color> ���� ��־��{input}");
        }
    }
}
