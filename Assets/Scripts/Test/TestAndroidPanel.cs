// -------------------------
// 创建日期：2023/3/2 18:02:40
// -------------------------

using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Framework;

namespace Test
{
    public class TestAndroidPanel : MonoBehaviour
    {
        Text showMsg, appMonitorMsg;

        private void Awake()
        {
            Log.Debuger("Awake");
        }

        // Start is called before the first frame update
        void Start()
        {
            Log.Debuger("Start");

            showMsg = transform.FindOf<Text>("showMsg");
            appMonitorMsg = transform.FindOf<Text>("appMonitorMsg");

            transform.FindOf<Button>("getSystemLanguageBtn")?.onClick.AddListener(() =>
            {
                Show();
            });

            Show();


        }

        private void Update()
        {
            if (appMonitorMsg)
            {
                appMonitorMsg.text = 
$@"
进入后台的时间：{ApplicationMonitor.EnterBackstageTime}
进入前台的时间：{ApplicationMonitor.EnterFrontDeskTime}
时间间隔：{ApplicationMonitor.ActivateTimeSpan.TotalSeconds}（秒）
";
            }
        }

        private void OnApplicationPause(bool pause)
        {
            Log.Yellow($"当前应用暂停：{pause}");
        }


        // 显示获取到的系统语言
        void Show()
        {
            if (PlatformUtility.platform == Platform.Android)
            {
                showMsg.text = $"当前系统语言：{Environment.NewLine}{AndroidSystem.GetDisplayLanguage()}";
            }
            else
            {
                showMsg.text = "当前非 安卓（Android） 平台";
            }
        }
    }
}