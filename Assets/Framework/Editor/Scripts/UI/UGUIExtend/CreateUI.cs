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
//using UEditorResources = UnityEditor.Experimental.EditorResources;// 无法加载

namespace Framework.Editor
{
    public static class CreateUI
    {
        public const string EditorResRootPath = "Framework/Prefabs/";
        //public const string FullRootPath = "Assets/Framework/Editor/Resources/Framework/Prefabs/";
        //public const string FullRootPath = "Assets/Framework/Prefabs/";
        public const string FullRootPath = "Prefabs/";

        public static GameObject Create(string rootPath, string name)
        {
            Object selectObj = Selection.objects.Length > 0 ? Selection.objects[0] : null;

            string path = Path.Combine(rootPath, name);
            //var dpPrefab = (GameObject)EditorGUIUtility.Load(path);
            //var dpPrefab = (GameObject)EditorGUIUtility.Load(name);
            //var dpPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            //var dpPrefab = Resources.Load<GameObject>(path);
            var dpPrefab = EditorResources.Load<GameObject>(path);
            //var dpPrefab = UEditorResources.Load<GameObject>(path);

            Debug.Log($"有 UI 预制体 \"{path}\"： {dpPrefab != null}，{(dpPrefab != null ? $"在 \"{AssetDatabase.GetAssetPath(dpPrefab)}\" " : null)}");

            var dp = Object.Instantiate(dpPrefab);
            //var dp = (GameObject)PrefabUtility.InstantiateAttachedAsset(dpPrefab);
            //var dp = (GameObject)PrefabUtility.InstantiatePrefab(dpPrefab);// 此实例化方式会保持对预制体原型的引用

            if (dp)
            {
                if (selectObj is GameObject go)
                {
                    dp.transform.SetParent(go.transform, false);
                }

                Selection.activeGameObject = dp;
            }

            return dp;
        }

        public static GameObject Create(string name)
        {
            return Create(FullRootPath, name);
        }
    }
}