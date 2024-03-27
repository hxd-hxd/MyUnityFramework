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

            Debug.Log($"普通 日志：{input}");
            Debug.LogWarning($"警告 日志：{input}");
            Debug.LogError($"错误 日志：{input}");
            Debug.LogException(new Exception($"异常 日志：{input}"));
            Debug.LogAssertion($"断言 日志：{input}");

            Debug.Log($"<color=#39FF00>带颜色</color> 普通 日志：{input}");
            Debug.LogWarning($"<color=#39FF00>带颜色</color> 警告 日志：{input}");
            Debug.LogError($"<color=#39FF00>带颜色</color> 错误 日志：{input}");
            Debug.LogException(new Exception($"<color=#39FF00>带颜色</color> 异常 日志：{input}"));
            Debug.LogAssertion($"<color=#39FF00>带颜色</color> 断言 日志：{input}");
        }
    }
}
