// -------------------------
// 创建日期：2023/4/26 11:30:42
// -------------------------

using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Framework
{
    /// <summary>
    /// 加载场景时不销毁
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class DontDestroyOnLoad : MonoBehaviour
    {
        private void Awake()
        {
            if (gameObject.transform.root == transform)
                DontDestroyOnLoad(gameObject);
        }
    }
}