using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEditor;
using System.IO;
using Framework.LogSystem;
using System.Linq;

namespace Framework.Editor
{
    public class LogSystemEditor
    {
        const string RootPath = "Framework/打开日志目录/";
        const string LogLoaderRootPath = "Framework/日志 UI 自动加载/";

        const string LOG_SYSTEM_AUTO_LOADER_DISABLE = "LOG_SYSTEM_AUTO_LOADER_DISABLE";

        [MenuItem(RootPath + "编辑器", false, 10)]
        public static void OpenFolder_LogEditor()
        {
            OpenFolder.Execute(Path.GetDirectoryName(LogInfo.logFilePath_Eitor));
        }

        [MenuItem(RootPath + "移动平台", false, 10)]
        public static void OpenFolder_LogMobile()
        {
            OpenFolder.Execute(Path.GetDirectoryName(LogInfo.logFilePath_Mobile));
        }

        [MenuItem(RootPath + "通用平台（PC等）", false, 10)]
        public static void OpenFolder_LogStandard()
        {
            OpenFolder.Execute(Path.GetDirectoryName(LogInfo.logFilePath_Standard));
        }

        [MenuItem(LogLoaderRootPath + "启用", false, 11)]
        public static void EnableLogSystemAutoLoader()
        {
            var buildTargetGroup = EditorUserBuildSettings.selectedBuildTargetGroup;
            PlayerSettings.GetScriptingDefineSymbolsForGroup(buildTargetGroup, out var defines);
            if (defines != null)
            {
                List<string> list = new List<string>(defines);
                if (defines.Contains(LOG_SYSTEM_AUTO_LOADER_DISABLE))
                {
                    list.Remove(LOG_SYSTEM_AUTO_LOADER_DISABLE);
                    PlayerSettings.SetScriptingDefineSymbolsForGroup(buildTargetGroup, list.ToArray());
                }
            }
        }
        [MenuItem(LogLoaderRootPath + "禁用", false, 12)]
        public static void DisableLogSystemAutoLoader()
        {
            var buildTargetGroup = EditorUserBuildSettings.selectedBuildTargetGroup;
            PlayerSettings.GetScriptingDefineSymbolsForGroup(buildTargetGroup, out var defines);
            if (defines != null)
            {
                List<string> list = new List<string>(defines);
                if (!defines.Contains(LOG_SYSTEM_AUTO_LOADER_DISABLE))
                {
                    list.Add(LOG_SYSTEM_AUTO_LOADER_DISABLE);
                    PlayerSettings.SetScriptingDefineSymbolsForGroup(buildTargetGroup, list.ToArray());
                }
            }
        }
        [MenuItem(LogLoaderRootPath + "启用", true)]
        static bool LogSystemAutoLoader_Checked()
        {
            GetLogSystemAutoLoaderDefine(out _, out bool contains);
            Menu.SetChecked(LogLoaderRootPath + "启用", !contains);
            Menu.SetChecked(LogLoaderRootPath + "禁用", contains);
            return true;
        }
        public static void GetLogSystemAutoLoaderDefine(out string defFormat, out bool contains)
        {
            var buildTargetGroup = EditorUserBuildSettings.selectedBuildTargetGroup;
            defFormat = PlayerSettings.GetScriptingDefineSymbolsForGroup(buildTargetGroup);
            contains = defFormat.Contains(LOG_SYSTEM_AUTO_LOADER_DISABLE);
        }
    }
}