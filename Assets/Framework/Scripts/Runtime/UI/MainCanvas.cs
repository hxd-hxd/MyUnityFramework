// -------------------------
// 创建日期：2022/11/3 17:52:50
// -------------------------

using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Framework
{
    /// <summary>
    /// 主 画布
    /// </summary>
    public class MainCanvas : MonoBehaviour
    {
        public Canvas m_mainCanvas;

        private void Awake()
        {
            m_mainCanvas = gameObject.GetComponent<Canvas>();
            UIPanelManager.MainCanvas = m_mainCanvas;
        }

        void Start()
        {
            
        }

    }
}