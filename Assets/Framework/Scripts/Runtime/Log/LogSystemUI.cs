using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Framework.LogSystem
{
    /// <summary>
    /// 日志系统 ui
    /// </summary>
    public class LogSystemUI : BaseUI
    {
        private FpsCounter m_FpsCounter = null;

        LogBuoyUI logBuoy => UIManager.ExpectGetUI<LogBuoyUI>();
        LogViewUI logView => UIManager.ExpectGetUI<LogViewUI>();

        public FpsCounter fpsCounter { get => m_FpsCounter; private set => m_FpsCounter = value; }

        protected override void Awake()
        {
            base.Awake();

            m_FpsCounter = new FpsCounter();

            LogInfo.Login();
            //LogInfo.logMessageReceived += logView.OnHandleLog;
        }

        protected override void Start()
        {
            base.Start();

            if (logBuoy != null)
            {
                logBuoy.logClickEvent += () =>
                {
                    logView.Enable(true);
                };
            }
            if (logView != null)
            {
                logView.closeClickEvent += () =>
                {
                    logBuoy.Enable(true);
                };
            }
        }

        private void Update()
        {
            m_FpsCounter.Update();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            LogInfo.Logout();
            //LogInfo.logMessageReceived -= logView.OnHandleLog;
        }

        public void OpenLogBuoy()
        {
            if (logBuoy) logBuoy.Enable(true);
            if (logView) logView.Enable(false);
        }
        public void OpenLogView()
        {
            if (logBuoy) logBuoy.Enable(false);
            if (logView) logView.Enable(true);
        }
    }
}