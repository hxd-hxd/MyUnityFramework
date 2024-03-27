using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEditor;
using System.IO;
using Framework.LogSystem;

namespace Framework.Editor
{
    public class LogSystemEditor
    {
        const string RootPath = "Framework/����־Ŀ¼/";

        [MenuItem(RootPath + "�༭��", false, 10)]
        public static void OpenFolder_LogEditor()
        {
            OpenFolder.Execute(Path.GetDirectoryName(LogInfo.logFilePath_Eitor));
        }

        [MenuItem(RootPath + "�ƶ�ƽ̨", false, 10)]
        public static void OpenFolder_LogMobile()
        {
            OpenFolder.Execute(Path.GetDirectoryName(LogInfo.logFilePath_Mobile));
        }

        [MenuItem(RootPath + "ͨ��ƽ̨��PC�ȣ�", false, 10)]
        public static void OpenFolder_LogStandard()
        {
            OpenFolder.Execute(Path.GetDirectoryName(LogInfo.logFilePath_Standard));
        }
    }
}