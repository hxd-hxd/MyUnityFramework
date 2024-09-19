// -------------------------
// 创建日期：2024/9/19 10:13:20
// -------------------------

using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;
using Object = UnityEngine.Object;

namespace Framework.Editor
{
    public class ExportUnitypackage
    {
        const string Export_Expanded_Name = ".unitypackage";
        const string Export_File_Default_Name = "Unitypackage" + Export_Expanded_Name;

        const string Root_Menu = "Assets/导出 Package/";
        const int Root_MenuIndex = 21;

        //[MenuItem(Root_Menu + "导出", false, Root_MenuIndex)]
        //public static void Export()
        //{
        //    var objs = Selection.objects;
        //    var fs = objs.Select(obj => AssetDatabase.GetAssetPath(obj)).ToArray();
        //    string outPath = null;
        //    outPath = EditorUtility.OpenFolderPanel("选择", $"{Path.GetDirectoryName(Application.dataPath)}", Export_File_Default_Name);
        //    AssetDatabase.ExportPackage(fs, outPath);
        //}

        [MenuItem(Root_Menu + "导出到当前文件夹（不包含依赖）", false, Root_MenuIndex)]
        public static void ExportToCurrentDir()
        {
            var obj = Selection.activeObject;
            var f = AssetDatabase.GetAssetPath(obj);
            string outPath = $"{Path.GetDirectoryName(f)}/{Path.GetFileNameWithoutExtension(Path.GetFileName(f))}{Export_Expanded_Name}";
            AssetDatabase.ExportPackage(f, outPath, ExportPackageOptions.Recurse);

            AssetDatabase.Refresh();

            var uPack = AssetDatabase.LoadAssetAtPath<Object>(outPath);
            Selection.activeObject = uPack;
            AssetDatabase.Refresh();
        }
    }
}