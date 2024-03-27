using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using UnityEngine;

namespace Framework.LogSystem
{
    /// <summary>
    /// ��־��Ϣ
    /// </summary>
    [Serializable]
    public sealed partial class LogInfo : ITypePoolObject
    {
        private static int _logCount = 0;
        private static int _errorCount = 0;
        private static int _exceptionCount = 0;
        private static int _warningCount = 0;
        private static int _assertCount = 0;


        private static List<LogInfo> _logInfos = new List<LogInfo>(1000);
        //public static Dictionary<LogType, Color> 

        /// <summary>
        /// ���� ��־��Ϣ��¼ʱ�ᴥ�����¼�
        /// </summary>
        public static event Action<LogInfo> logMessageReceived;

        [SerializeField]
        DateTime _time = default;
        [SerializeField]
        [TextArea(3, 10)]
        string _condition = null;
        [SerializeField]
        [TextArea(3, 10)]
        string _stackTrace = null;
        [SerializeField]
        LogType _logType = LogType.Log;
        [SerializeField]
        Color _logColor = Color.white;

        /// <summary>
        /// ��ͨ ��־����
        /// </summary>
        public static int LogCount { get => _logCount; private set => _logCount = value; }
        /// <summary>
        /// ���� ��־����
        /// </summary>
        public static int ErrorCount { get => _errorCount; private set => _errorCount = value; }
        /// <summary>
        /// �쳣 ��־����
        /// </summary>
        public static int ExceptionCount { get => _exceptionCount; private set => _exceptionCount = value; }
        /// <summary>
        /// ���� ��־����
        /// </summary>
        public static int WarningCount { get => _warningCount; private set => _warningCount = value; }
        /// <summary>
        /// ���� ��־����
        /// </summary>
        public static int AssertCount { get => _assertCount; private set => _assertCount = value; }

        /// <summary>
        /// ��־��Ϣ������
        /// </summary>
        public static int logInfoCount => _logInfos.Count;
        /// <summary>
        /// ��־��Ϣ�б�
        /// </summary>
        public static List<LogInfo> logInfos => _logInfos;

        public DateTime time { get => _time; set => _time = value; }
        public string condition { get => _condition; set => _condition = value; }
        /// <summary>
        /// ��ջ
        /// </summary>
        public string stackTrace { get => _stackTrace; set => _stackTrace = value; }
        /// <summary>
        /// ��־����
        /// </summary>
        public LogType logType
        {
            get => _logType;
            set
            {
                _logType = value;
            }
        }
        /// <summary>
        /// ��־��ɫ
        /// </summary>
        public Color logColor
        {
            get => _logColor;
            set
            {
                _logColor = value;
            }
        }

        public LogInfo(string condition, string stackTrace, LogType logType)
        {
            Set(condition, stackTrace, logType);
        }

        /// <summary>ע�ᵽ Unity ����־������</summary>
        public static void Login()
        {
            Application.logMessageReceived -= LogInfo.Received;
            Application.logMessageReceived += LogInfo.Received;
        }
        /// <summary>�� Unity ����־������ע��</summary>
        public static void Logout()
        {
            Application.logMessageReceived -= LogInfo.Received;
        }

        /// <summary>
        /// ��ȡ��Ӧ log ���͵���ɫ
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static Color GetLogColor(LogType type)
        {
            Color color = Color.white;
            switch (type)
            {
                case LogType.Error:
                case LogType.Assert:
                case LogType.Exception:
                    color = Color.red;
                    break;
                case LogType.Warning:
                    //color = Color.yellow;
                    ColorUtility.TryParseHtmlString("#FF9100", out color);
                    break;
                case LogType.Log:
                    break;
                default:
                    break;
            }
            return color;
        }

        /// <summary>
        /// ��¼��־��Ϣ
        /// </summary>
        public static void Received(string condition, string stackTrace, LogType logType)
        {
            var info = new LogInfo(condition, stackTrace, logType);
            Received(info);
        }
        /// <summary>
        /// ��¼��־��Ϣ
        /// </summary>
        /// <param name="info"></param>
        public static void Received(LogInfo info)
        {
            if (info == null) return;
            if (!logInfos.Contains(info))
            {
                logInfos.Add(info);
                // ��������
                switch (info.logType)
                {
                    case LogType.Error:
                        _errorCount += 1;
                        break;
                    case LogType.Assert:
                        _assertCount += 1;
                        break;
                    case LogType.Warning:
                        _warningCount += 1;
                        break;
                    case LogType.Log:
                        _logCount += 1;
                        break;
                    case LogType.Exception:
                        _exceptionCount += 1;
                        break;
                    default:
                        break;
                }

                AddLogToFile(info);// ����־ע��ʱ��ӵ��ļ�
                //logMessageReceived?.Invoke(info._condition, info._stackTrace, info._logType);
                logMessageReceived?.Invoke(info);
            }
        }

        /// <summary>
        /// ������־��Ϣ
        /// <para>ע�� <see cref="time"/> �� <see cref="logColor"/> ���Զ�����</para>
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="stackTrace"></param>
        /// <param name="logType"></param>
        public void Set(string condition, string stackTrace, LogType logType)
        {
            time = DateTime.Now;
            this.condition = condition;
            this.stackTrace = stackTrace;
            this.logType = logType;
            logColor = GetLogColor(logType);
        }

        // "========================================================================"
        /// <summary>
        /// ת�����ı���ʽ���洢����־�ļ�ʱ��ʹ�ô˸�ʽ��
        /// <para>��ʽ�ο���[����] + [����] + ��־���� + ��ջ</para>
        /// <code>
        /// [2024/02/26 09:44:15][Log]  1 ��ͨ ��־ ��ǰʱ�䣺2024/2/26 9:44:15
        /// UnityEngine.Debug:Log(object)
        /// TestLog:LogTime() (at Assets/Demo/Scripts/Test/TestLog.cs:20)
        /// UnityEngine.EventSystems.EventSystem:Update() (at Library/PackageCache/com.unity.ugui@1.0.0/Runtime/EventSystem/EventSystem.cs:501)
        /// </code>
        /// </summary>
        /// <returns></returns>
        public string ToText()
        {
            var sb = TypePool.root.Get<StringBuilder>();

            // �����ı���ʽ
            //sb.Append(ToConditionText());
            // ʱ��
            sb.Append("[").Append(time.ToTimeText()).Append("]");
            // ������ͱ�ʶ
            sb.Append("[").Append(_logType).Append("]");
            // �ո�
            sb.Append("  ");
            // log ��Ϣ
            sb.Append(condition);
            // ��ջ��Ϣ
            sb.AppendLine();
            sb.Append(_stackTrace);

            string t = sb.ToString();
            TypePool.root.Return(sb);
            return t;
        }
        /// <summary>
        /// ת�����ı���ʽ
        /// <para>��ʽ�ο���[����] + ��־����</para>
        /// <code>
        /// [2024/02/26 09:44:15]  1 ��ͨ ��־ ��ǰʱ�䣺2024/2/26 9:44:15
        /// </code>
        /// </summary>
        /// <returns></returns>
        public string ToConditionText()
        {
            var sb = TypePool.root.Get<StringBuilder>();

            // �����ı���ʽ
            // ʱ��
            //sb.Append("[").Append(time.ToTimeText()).Append("]");
            sb.Append("[").Append(time.ToHourBehindTimeText()).Append("]");
            // �ո�
            sb.Append("  ");
            sb.Append(condition);

            string t = sb.ToString();
            TypePool.root.Return(sb);
            return t;
        }

        void ITypePoolObject.Clear()
        {
            condition = null;
            stackTrace = null;
            _logColor = Color.white;
            _logType = LogType.Log;
        }
    }
}