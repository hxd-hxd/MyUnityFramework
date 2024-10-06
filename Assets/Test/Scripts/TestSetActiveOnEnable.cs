// -------------------------
// 创建日期：2023/3/6 18:22:45
// -------------------------

using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Test
{
    public class TestSetActiveOnEnable : MonoBehaviour
    {
        private void OnEnable()
        {
            Debug.Log($"启用脚本 {name} => TestSetActiveOnEnable");
        }

    }
}