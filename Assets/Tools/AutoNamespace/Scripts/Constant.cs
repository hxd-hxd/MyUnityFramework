using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

namespace AutoNamespace
{
    public static class Constant
    {
        public const string DefaultNamespace = "AutoNamespace";             // 默认命名空间
        public const string DefaultMenuScriptName = "AutoNamespace_Menu";   // 默认菜单脚本名称
        public const int DefaultMenuPriority = 1000;                        // 默认菜单优先级
        public const int RootMenuPriority = 81;                             // 根菜单优先级

        public const string CreateTime = "#_CREATE_TIME_#";
        public const string Namespace = "#_NAME_SPACE_#";
        public const string ScriptName = "#_SCRIPT_NAME_#";
        public const string MenuName = "#_MENUN_NAME_#";
        public const string MenuFuncName = "#_MENUN_FUNC_NAME_#";
        /// <summary>
        /// 优先级
        /// </summary>
        public const string MenuPriority = "#_MENUN_PRIORITY_#";
        public const string AddCode = "#_ADD_CODES_#";

        /// <summary>
        /// 配置文件的路径
        /// </summary>
        public static readonly string ConfigurePath = Application.dataPath + "/Tools" + "/AutoNamespace/Configure/Configure.txt";
        //public static readonly string AutoCreateDirectory = Application.dataPath + "/AutoNamespace/Scripts/AutoCreate/";// 用于存放自动创建脚本的目录
        //public static readonly string ScriptTemplatePath = "Assets/AutoNamespace/Templates/ScriptTemplate.txt";
        //public static readonly string MenuScriptTemplatePath = "Assets/AutoNamespace/Templates/MenuScriptTemplate.txt";
        //public static readonly string MenuOptionTemplatePath = "Assets/AutoNamespace/Templates/MenuOptionTemplate.txt";

        // 检查输入的命名空间中是否包含无效字符
        public static readonly List<string> NamespaceInvalids = new List<string>
        {
            ",",
            "/",
            "\\",
            "'",
            "\"",
            "?",
            "!",
        };

        public static class Menu
        {
            public const string MenuAutoNamespacePath = "Assets/Create/C# AutoNamespace";
            public const string MenuNotNamespacePath = "Assets/Create/C# NotNamespace";
        }

    }

}