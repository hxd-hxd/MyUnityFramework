// -------------------------
// 创建日期：2024/2/22 16:59:55
// -------------------------

using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Test
{
    public class TestLog : MonoBehaviour
    {
        public Color color = Color.white;

        void Start()
        {
            Coroutines.Delay(() =>
            {
                Debug.Log("普通 日志");
                Debug.LogWarning("警告 日志");
                Debug.LogError("错误 日志");
                Debug.LogException(new Exception("异常 日志"));
                Debug.LogAssertion("断言 日志");

                Debug.Log("<color=#39FF00>带颜色</color> 普通 日志");
                Debug.LogWarning("<color=#39FF00>带颜色</color> 警告 日志");
                Debug.LogError("<color=#39FF00>带颜色</color> 错误 日志");
                Debug.LogException(new Exception("<color=#39FF00>带颜色</color> 异常 日志"));
                Debug.LogAssertion("<color=#39FF00>带颜色</color> 断言 日志");
            });
        }

    }
}