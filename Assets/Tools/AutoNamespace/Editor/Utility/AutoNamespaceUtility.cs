using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace AutoNamespace
{
    public static class AutoNamespaceUtility
    {
        //static NamespaceConfig nc;

        /// <summary>
        /// 允许配置的动态路径
        /// <para>ps：修改时的格式限定为："/你要修改的目录"</para>
        /// </summary>
        public static string DynamicPath
        {
            get
            {
                return ReadConfigure().DynamicPath;
            }
            set
            {
                NamespaceConfig nc = ReadConfigure();
                nc.DynamicPath = value;
                Save(nc, Constant.ConfigurePath);
            }
        }
        /// <summary>
        /// 脚本模板路径
        /// </summary>
        public static string ScriptTemplatePath
        {
            get
            {
                NamespaceConfig nc = ReadConfigure();
                return CorrectPath(nc.ScriptTemplatePath);
            }
            internal set
            {
                NamespaceConfig nc = ReadConfigure();
                nc.ScriptTemplatePath = value;
                Save(nc, Constant.ConfigurePath);
            }
        }
        /// <summary>
        /// 脚本模板路径
        /// </summary>
        public static string ScriptTemplatePath_NotNamespace
        {
            get
            {
                return CorrectPath(ReadConfigure().ScriptTemplatePath_NotNamespace);
            }
            internal set
            {
                NamespaceConfig nc = ReadConfigure();
                nc.ScriptTemplatePath_NotNamespace = value;
                Save(nc, Constant.ConfigurePath);
            }
        }
        /// <summary>
        /// 静态脚本模板路径
        /// </summary>
        public static string StaticScriptTemplatePath_NotNamespace
        {
            get
            {
                NamespaceConfig nc = ReadConfigure();
                return CorrectPath(nc.StaticScriptTemplatePath_NotNamespace);
            }
            internal set
            {
                NamespaceConfig nc = ReadConfigure();
                nc.StaticScriptTemplatePath_NotNamespace = value;
                Save(nc, Constant.ConfigurePath);
            }
        }
        /// <summary>
        /// 编辑器脚本模板路径
        /// </summary>
        public static string EditorScriptTemplatePath_NotNamespace
        {
            get
            {
                NamespaceConfig nc = ReadConfigure();
                return CorrectPath(nc.EditorScriptTemplatePath_NotNamespace);
            }
            internal set
            {
                NamespaceConfig nc = ReadConfigure();
                nc.EditorScriptTemplatePath_NotNamespace = value;
                Save(nc, Constant.ConfigurePath);
            }
        }
        /// <summary>
        /// 编辑器窗口脚本模板路径
        /// </summary>
        public static string EditorWindowScriptTemplatePath_NotNamespace
        {
            get
            {
                NamespaceConfig nc = ReadConfigure();
                return CorrectPath(nc.EditorWindowScriptTemplatePath_NotNamespace);
            }
            internal set
            {
                NamespaceConfig nc = ReadConfigure();
                nc.EditorWindowScriptTemplatePath_NotNamespace = value;
                Save(nc, Constant.ConfigurePath);
            }
        }
        public static string MenuScriptTemplatePath
        {
            get
            {
                NamespaceConfig nc = ReadConfigure();
                return CorrectPath(nc.MenuScriptTemplatePath);
            }
            internal set
            {
                NamespaceConfig nc = ReadConfigure();
                nc.MenuScriptTemplatePath = value;
                Save(nc, Constant.ConfigurePath);
            }
        }
        public static string MenuOptionTemplatePath
        {
            get
            {
                NamespaceConfig nc = ReadConfigure();
                return CorrectPath(nc.MenuOptionTemplatePath);
            }
            internal set
            {
                NamespaceConfig nc = ReadConfigure();
                nc.MenuOptionTemplatePath = value;
                Save(nc, Constant.ConfigurePath);
            }
        }
        /// <summary>
        /// 自动生成脚本时的目录
        /// <para>ps：修改时的格式限定为："/你要修改的目录" + AutoCreateDirectory</para>
        /// </summary>
        public static string AutoCreateDirectory
        {
            get
            {
                NamespaceConfig nc = ReadConfigure();
                return CorrectPath(nc.AutoCreateDirectory);
            }
            internal set
            {
                NamespaceConfig nc = ReadConfigure();
                nc.AutoCreateDirectory = Application.dataPath + value;
                Save(nc, Constant.ConfigurePath);
            }
        }


        public static NamespaceConfig Read(string path)
        {
            string json = File.ReadAllText(path);
            return JsonUtility.FromJson<NamespaceConfig>(json);
        }
        public static void Save(NamespaceConfig nc, string path)
        {
            string json = JsonUtility.ToJson(nc, true);
            nc.NamespaceList = null;
            File.WriteAllText(path, json);
        }

        public static NamespaceConfig ReadConfigure()
        {
            return Read(Constant.ConfigurePath);
        }

        public static string CorrectPath(string path)
        {
            NamespaceConfig nc = ReadConfigure();
            return string.Format(path, nc.DynamicPath);
        }

        /// <summary>
        /// 写入或者创建文件
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="content"></param>
        public static void WriteOrCreateFile(string filePath, string content)
        {
            string fullFileName = Path.GetFileName(filePath);
            string directoryName = Path.GetDirectoryName(filePath);

            if (fullFileName.Split('.').Length < 2) return;

            if (!Directory.Exists(directoryName))
            {
                Directory.CreateDirectory(directoryName);
                using (File.Create(filePath)) { }
            }
            else
            {
                if (!File.Exists(filePath))
                {
                    using (File.Create(filePath)) { }
                }
            }

            File.WriteAllText(filePath, content);
        }


        public static bool Enable()
        {
            return Read(Constant.ConfigurePath).enable;
        }

        public static List<string> MenuList()
        {
            return Read(Constant.ConfigurePath).NamespaceList;
        }

        // 检查列表中的命名空间格式，并移除格式不正确的
        public static void CheckNamespaceFormatRemove(List<string> namespaceList)
        {
            //for (int i = 0; i < namespaceList.Count; i++)
            //{
            //    if (CheckNamespaceFormat(namespaceList[i]))
            //    {

            //    }
            //}
            namespaceList.RemoveAll((value) => { return !CheckNamespaceFormat(value); });
        }
        // 检查填入的命名空间格式是否正确
        public static bool CheckNamespaceFormat(string namespaceFormat)
        {

            return true;
        }


        private static string GetSelectedPathOrFallback()
        {
            string path = "Assets";
            foreach (UnityEngine.Object obj in Selection.GetFiltered(typeof(UnityEngine.Object), SelectionMode.Assets))
            {
                path = AssetDatabase.GetAssetPath(obj);
                if (!string.IsNullOrEmpty(path) && File.Exists(path))
                {
                    path = Path.GetDirectoryName(path);
                    break;
                }
            }
            return path;
        }

        public static void CreateCSharpScriptAssetBy(string namespaceValue)
        {
            CreateCSharpScriptAssetBy(namespaceValue, ScriptTemplatePath);
        }
        public static void CreateCSharpScriptAssetBy(string namespaceValue, string resourceFile)
        {
            CreateCSharpScriptAssetBy(namespaceValue, resourceFile, "NewBehaviourScript");
        }
        /// <summary>
        /// 创建 C# 脚本资源
        /// </summary>
        /// <param name="namespaceValue">命名空间</param>
        /// <param name="resourceFile">脚本资源文件模板路径</param>
        /// <param name="defaultName">脚本默认名称</param>
        public static void CreateCSharpScriptAssetBy(string namespaceValue, string resourceFile, string defaultName)
        {
            GlobalVariable.namespaceValue = namespaceValue;
            string locationPath = GetSelectedPathOrFallback();
            ProjectWindowUtil.StartNameEditingIfProjectWindowExists(0,
            ScriptableObject.CreateInstance<CreateCSharpScriptAsset>(),
            locationPath + "/" + defaultName + ".cs",
            null, resourceFile);
        }
        public static void CreateCSharpScriptAsset(string namespaceValue)
        {
            CreateCSharpScriptAssetBy(namespaceValue);
        }
    }

}