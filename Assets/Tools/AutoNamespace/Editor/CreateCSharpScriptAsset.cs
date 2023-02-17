using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEditor;
using UnityEditor.ProjectWindowCallback;
using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace AutoNamespace
{
    /// <summary>
    /// 用于创建 C# 脚本
    /// </summary>
    public class CreateCSharpScriptAsset : EndNameEditAction
    {
        public override void Action(int instanceId, string pathName, string resourceFile)
        {
            UnityEngine.Object o = CreateScriptAssetFromTemplate(pathName, resourceFile);
            ProjectWindowUtil.ShowCreatedAsset(o);
        }

        internal static UnityEngine.Object CreateScriptAssetFromTemplate(string pathName, string resourceFile)
        {
            string fullPath = Path.GetFullPath(pathName);
            StreamReader streamReader = new StreamReader(resourceFile);
            string text = streamReader.ReadToEnd();
            streamReader.Close();
            string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(pathName);

            // 替换文件名
            text = Regex.Replace(text, Constant.CreateTime, DateTime.Now.ToLocalTime().ToString());
            text = Regex.Replace(text, Constant.Namespace, GlobalVariable.namespaceValue);
            text = Regex.Replace(text, Constant.ScriptName, fileNameWithoutExtension);

            UTF8Encoding encoding = new UTF8Encoding(true, false);
            StreamWriter streamWriter = new StreamWriter(fullPath, false, encoding);
            streamWriter.Write(text);
            streamWriter.Close();
            AssetDatabase.ImportAsset(pathName);
            return AssetDatabase.LoadAssetAtPath(pathName, typeof(UnityEngine.Object));
        }
    }
}