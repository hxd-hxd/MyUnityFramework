using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using UnityEngine;

namespace Framework.LogSystem
{
    // ��־�ļ�����
    public partial class LogInfo
    {
        const string _logFileName = "Log.txt";
        static string _logFileUniversalDir = "Log Info";// ͨ��Ŀ¼
        // ������ʱ�������
        static string _logFileName_DateTimeFormat = $"Log {DateTime.Now.ToDayFrontTimeText("-")}.txt";

        /// <summary>
        /// �༭��
        /// </summary>
        public static string logFilePath_Eitor
        {
            get
            {
                string _path = _GetLogFilePath_SetRoot(Path.Combine(Application.dataPath, ".."));
                return _path;
            }
        }
        /// <summary>
        /// �ƶ�ƽ̨
        /// </summary>
        public static string logFilePath_Mobile
        {
            get
            {
                string _path = _GetLogFilePath_SetRoot(Application.persistentDataPath);
                return _path;
            }
        }
        /// <summary>
        /// ������׼ͨ��ƽ̨
        /// </summary>
        public static string logFilePath_Standard
        {
            get
            {
                string _path = _GetLogFilePath_SetRoot(Application.streamingAssetsPath);
                return _path;
            }
        }
        /// <summary>
        /// ��־�ļ�·��
        /// </summary>
        public static string logFilePath
        {
            get
            {
                string _path = null;
#if UNITY_EDITOR
                _path = logFilePath_Eitor;
#elif UNITY_ANDROID || UNITY_IPHONE
            _path = logFilePath_Mobile;
#else
            _path = logFilePath_Standard;
#endif
                if (!File.Exists(_path))
                {
                    File.Create(_path).Dispose();
                }

                return _path;
            }
        }
        // ��ȡ��־�ļ����Ŀ¼
        static string _GetLogFilePath_SetRoot(string rootDir)
        {
            string pDir = Path.Combine(rootDir, _logFileUniversalDir);
            string _path = Path.Combine(pDir, _logFileName_DateTimeFormat);
            if (!Directory.Exists(pDir))
            {
                Directory.CreateDirectory(pDir);
            }
            if (!File.Exists(_path))
            {
                File.Create(_path).Dispose();
            }
            return _path;
        }

        /// <summary>
        /// ����ǰ������־ת�����ļ���ʽ���ı�
        /// </summary>
        /// <returns></returns>
        public static string ToFileFormatTextAll()
        {
            if (logInfoCount <= 0) return null;

            var sb = TypePool.root.Get<StringBuilder>();
            //sb.Append(logInfos[0].ToText());
            for (int i = 0; i < logInfos.Count; i++)
            {
                var info = logInfos[i];
                sb.Append(info.ToFileFormat());
            }

            string t = sb.ToString();
            TypePool.root.Return(sb);
            return t;
        }

        /// <summary>
        /// �����־���ļ�
        /// </summary>
        /// <param name="info"></param>
        public static void AddLogToFile(LogInfo info)
        {
            if (info != null)
            {
                File.AppendAllText(logFilePath, info.ToFileFormat());
            }
        }

        /// <summary>
        /// ת�����ļ���ʽ
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        private string ToFileFormat()
        {
            // �ָ���
            // ========================================================================
            return $@"
{ToText()}";
        }
    }
}