// -------------------------
// 创建日期：2024/3/25 17:04:00
// -------------------------

using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Framework
{
    /// <summary>
    /// ui 接口
    /// </summary>
    public interface IUI
    {
        /// <summary>
        /// 名称
        /// </summary>
        string name { get; set; }
        /// <summary>
        /// 是否启用
        /// </summary>
        bool isEnable { get; set; }


        /// <summary>
        /// 启用
        /// </summary>
        void Enable();
        /// <summary>
        /// 启用
        /// </summary>
        /// <param name="isEnable"></param>
        void Enable(bool isEnable);
    }
}