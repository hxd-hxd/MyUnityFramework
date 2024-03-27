using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Framework.LogSystem
{
    /// <summary>
    /// ��־���� ui 
    /// </summary>
    public class LogBuoyUI : BaseUI
    {
        [SerializeField]
        private Button _logBtn;
        [SerializeField]
        private Text _fpsText;
        [SerializeField]
        private LogSystemUI _logSystemUI;

        /// <summary>
        /// ��� log ��ťʱ�������¼�
        /// </summary>
        public Action logClickEvent;

        protected override void Start()
        {
            base.Start();

            _logBtn = transform.FindOf<Button>("LogBtn");
            _fpsText = _logBtn.transform.FindOf<Text>("Text");

            if (!_logSystemUI)
                _logSystemUI = UIManager.ExpectGetUI<LogSystemUI>();

            AddEvent(_logBtn, OnLogClick);
        }

        protected virtual void Update()
        {
            string fpsT = "����־";
            if (_logSystemUI && _logSystemUI.fpsCounter != null)
            {
                fpsT = $"FPS��{_logSystemUI.fpsCounter.CurrentFps:F}";
                // ��ɫ
                Color color = LogInfo.GetLogColor(LogType.Log);
                if (LogInfo.ErrorCount > 0
                    || LogInfo.ExceptionCount > 0
                    || LogInfo.AssertCount > 0
                    )
                {
                    color = LogInfo.GetLogColor(LogType.Error);
                }
                else if (LogInfo.WarningCount > 0)
                {
                    color = LogInfo.GetLogColor(LogType.Warning);
                }
                fpsT = $"<color=#{ColorUtility.ToHtmlStringRGBA(color)}>{fpsT}</color>";
            }
            ExtendUtility.SetText(_fpsText, fpsT);
        }

        protected virtual void OnLogClick()
        {
            Enable(false);
            logClickEvent?.Invoke();
        }
    }
    
}