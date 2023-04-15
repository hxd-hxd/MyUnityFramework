// -------------------------
// 创建日期：2023/4/12 11:27:50
// -------------------------

using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Framework.Editor;
using UnityEditor;

namespace Framework.TextMeshProExpress
{
    public class MenuOptions
    {
        [MenuItem("GameObject/UI/Dropdown Pro - TMP Text", priority = 2036)]
        static void AddDropdown(MenuCommand menuCommand)
        {
            var go = CreateUI.Create("Assets/Framework/ThirdExpressTools/TextMeshProExpress/Prefabs/", "DropdownPro TMP.prefab");
        }
    }
}