using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Framework.Test
{
    public class TestLog : MonoBehaviour
    {
       public  Button logBtn;
       public  InputField inputInfoUI;

        // Start is called before the first frame update
        void Start()
        {
            logBtn.onClick.AddListener(() =>
            {
                OnLog();
            });
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
