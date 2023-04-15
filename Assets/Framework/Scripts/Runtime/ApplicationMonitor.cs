// -------------------------
// 创建日期：2023/3/6 15:41:31
// -------------------------

using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Framework
{
    /// <summary>
    /// 程序监视器
    /// </summary>
    public sealed class ApplicationMonitor : MonoBehaviour
    {
        /// <summary>
        /// 游戏进入前台时触发
        /// </summary>
        public static event Action EnterFrontDeskEvent;
        /// <summary>
        /// 游戏进入后台时触发
        /// </summary>
        public static event Action EnterBackstageEvent;
        /// <summary>
        /// 进入前台的时间
        /// <para>此事件在 Awake 之前 Start 之后触发</para>
        /// </summary>
        public static DateTime EnterFrontDeskTime { get; private set; } = DateTime.Now;
        /// <summary>
        /// 进入后台的时间
        /// <para>此事件在 Awake 之前 Start 之后触发</para>
        /// </summary>
        public static DateTime EnterBackstageTime { get; private set; } = DateTime.Now;
        /// <summary>
        /// 程序进入后台，再从后台进入前台的时间跨度
        /// </summary>
        public static TimeSpan ActivateTimeSpan
        {
            get
            {
                return EnterFrontDeskTime - EnterBackstageTime;
            }
        }

        [RuntimeInitializeOnLoadMethod]
        static void Run()
        {
            ApplicationMonitor monitor = new GameObject("[Framework Application Monitor]").AddComponent<ApplicationMonitor>();
            DontDestroyOnLoad(monitor);
        }

        // 注意此方法在 Start 之前调用
        private void OnApplicationPause(bool pause)
        {
            // 安卓端 可用来检测应用进入后台，从后台返回
            if (pause)
            {
                EnterBackstageTime = DateTime.Now;

                EnterBackstageEvent?.Invoke();
            }
            else
            {
                EnterFrontDeskTime = DateTime.Now;

                EnterFrontDeskEvent?.Invoke();
            }
        }


        void Start()
        {
        
        }

    }
}