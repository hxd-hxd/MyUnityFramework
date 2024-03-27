using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using UnityEngine;

namespace Framework.LogSystem
{
    /// <summary>
    /// 日志信息
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
        /// 接收 日志信息记录时会触发此事件
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
        /// 普通 日志数量
        /// </summary>
        public static int LogCount { get => _logCount; private set => _logCount = value; }
        /// <summary>
        /// 错误 日志数量
        /// </summary>
        public static int ErrorCount { get => _errorCount; private set => _errorCount = value; }
        /// <summary>
        /// 异常 日志数量
        /// </summary>
        public static int ExceptionCount { get => _exceptionCount; private set => _exceptionCount = value; }
        /// <summary>
        /// 警告 日志数量
        /// </summary>
        public static int WarningCount { get => _warningCount; private set => _warningCount = value; }
        /// <summary>
        /// 断言 日志数量
        /// </summary>
        public static int AssertCount { get => _assertCount; private set => _assertCount = value; }

        /// <summary>
        /// 日志信息总数量
        /// </summary>
        public static int logInfoCount => _logInfos.Count;
        /// <summary>
        /// 日志信息列表
        /// </summary>
        public static List<LogInfo> logInfos => _logInfos;

        public DateTime time { get => _time; set => _time = value; }
        public string condition { get => _condition; set => _condition = value; }
        /// <summary>
        /// 堆栈
        /// </summary>
        public string stackTrace { get => _stackTrace; set => _stackTrace = value; }
        /// <summary>
        /// 日志类型
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
        /// 日志颜色
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

        /// <summary>注册到 Unity 的日志监视器</summary>
        public static void Login()
        {
            Application.logMessageReceived -= LogInfo.Received;
            Application.logMessageReceived += LogInfo.Received;
        }
        /// <summary>从 Unity 的日志监视器注销</summary>
        public static void Logout()
        {
            Application.logMessageReceived -= LogInfo.Received;
        }

        /// <summary>
        /// 获取对应 log 类型的颜色
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
        /// 记录日志信息
        /// </summary>
        public static void Received(string condition, string stackTrace, LogType logType)
        {
            var info = new LogInfo(condition, stackTrace, logType);
            Received(info);
        }
        /// <summary>
        /// 记录日志信息
        /// </summary>
        /// <param name="info"></param>
        public static void Received(LogInfo info)
        {
            if (info == null) return;
            if (!logInfos.Contains(info))
            {
                logInfos.Add(info);
                // 计算数量
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

                AddLogToFile(info);// 有日志注册时添加到文件
                //logMessageReceived?.Invoke(info._condition, info._stackTrace, info._logType);
                logMessageReceived?.Invoke(info);
            }
        }

        /// <summary>
        /// 设置日志信息
        /// <para>注意 <see cref="time"/> 和 <see cref="logColor"/> 会自动设置</para>
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
        /// 转换成文本格式（存储到日志文件时可使用此格式）
        /// <para>格式参考：[日期] + [类型] + 日志内容 + 堆栈</para>
        /// <code>
        /// [2024/02/26 09:44:15][Log]  1 普通 日志 当前时间：2024/2/26 9:44:15
        /// UnityEngine.Debug:Log(object)
        /// TestLog:LogTime() (at Assets/Demo/Scripts/Test/TestLog.cs:20)
        /// UnityEngine.EventSystems.EventSystem:Update() (at Library/PackageCache/com.unity.ugui@1.0.0/Runtime/EventSystem/EventSystem.cs:501)
        /// </code>
        /// </summary>
        /// <returns></returns>
        public string ToText()
        {
            var sb = TypePool.root.Get<StringBuilder>();

            // 定义文本格式
            //sb.Append(ToConditionText());
            // 时间
            sb.Append("[").Append(time.ToTimeText()).Append("]");
            // 添加类型标识
            sb.Append("[").Append(_logType).Append("]");
            // 空格
            sb.Append("  ");
            // log 信息
            sb.Append(condition);
            // 堆栈信息
            sb.AppendLine();
            sb.Append(_stackTrace);

            string t = sb.ToString();
            TypePool.root.Return(sb);
            return t;
        }
        /// <summary>
        /// 转换成文本格式
        /// <para>格式参考：[日期] + 日志内容</para>
        /// <code>
        /// [2024/02/26 09:44:15]  1 普通 日志 当前时间：2024/2/26 9:44:15
        /// </code>
        /// </summary>
        /// <returns></returns>
        public string ToConditionText()
        {
            var sb = TypePool.root.Get<StringBuilder>();

            // 定义文本格式
            // 时间
            //sb.Append("[").Append(time.ToTimeText()).Append("]");
            sb.Append("[").Append(time.ToHourBehindTimeText()).Append("]");
            // 空格
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