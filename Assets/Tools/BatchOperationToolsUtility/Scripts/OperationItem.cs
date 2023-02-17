// -------------------------
// 创建日期：2021/8/3 17:14:39
// -------------------------

using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BatchOperationToolsUtility
{

    /// <summary>
    /// 操作项
    /// </summary>
    [System.Serializable]
    public class OperationItem
    {
        public string pathAsset;
        public string pathFull;

        //public TextAsset file = new TextAsset() { name = BOTConstant.DefaultName };
        [NonSerialized]
        public TextAsset file = BOTConstant.DefaultTextAsset;

    }
}