// -------------------------
// 创建日期：2023/4/12 11:20:05
// -------------------------

using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Object = UnityEngine.Object;

namespace Framework.Editor
{
    public static class CreateUI
    {
        public const string EditorResRootPath = "Framework/Prefabs/";
        //public const string FullRootPath = "Assets/Framework/Editor/Resources/Framework/Prefabs/";
        public const string FullRootPath = "Assets/Framework/Prefabs/";

        public static GameObject Create(string rootPath, string name)
        {
            Object obj = Selection.objects.Length > 0 ? Selection.objects[0] : null;

            //var dpPrefab = EditorGUIUtility.Load($"{RootPath}DropdownPro Text.prefab");
            var dpPrefab = AssetDatabase.LoadAssetAtPath<GameObject>($"{rootPath}{name}");
            var dp = Object.Instantiate(dpPrefab);

            if (obj is GameObject go)
            {
                dp.transform.SetParent(go.transform, false);
            }

            Selection.activeGameObject = dp;

            return dp;
        }

        public static GameObject Create(string name)
        {
            return Create(FullRootPath, name);
        }
    }
}