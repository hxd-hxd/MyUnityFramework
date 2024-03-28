// -------------------------
// 创建日期：2021/8/4 14:10:32
// -------------------------

using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BatchOperationToolsUtility
{
    /// <summary>
    /// 全局变量
    /// </summary>
    public static class BOTGlobalVariable
    {
        /// <summary>
        /// 要修改成的目标命名空间
        /// </summary>
        public static string namespaceValue;
        /// <summary>
        /// 是否修改
        /// </summary>
        public static bool modify = false;
        /// <summary>
        /// 是否添加
        /// </summary>
        public static bool add = true;

        /// <summary>
        /// 要操作的项列表
        /// </summary>
        public static List<OperationItem> namespaceFileList = new List<OperationItem>();

    }
}