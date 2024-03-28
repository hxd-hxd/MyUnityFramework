// -------------------------
// 创建日期：2021/8/3 15:45:54
// -------------------------

using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BatchOperationToolsUtility
{
    public static class BOTConstant
    {
        public const string configPath = "Assets/Tools/BatchOperationToolsUtility/Editor/BOTConfigure.txt";

        public const string ns = "namespace";

        public const string DefaultName = "None (CSharp)";

        public static readonly TextAsset DefaultTextAsset = new TextAsset() { name = DefaultName };
        
        /// <summary>
        /// 当前支持修改的文件
        /// </summary>
        public static readonly List<string> FileExtensionList = new List<string>()
        {
            ".cs",
        };

    }
}