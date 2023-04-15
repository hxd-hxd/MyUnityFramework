// -------------------------
// 创建日期：2022/11/25 19:27:09
// -------------------------

using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.Runtime.InteropServices;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Framework
{

    /// <summary>
    /// 应用程序
    /// </summary>
    public static class ApplicationUtility
    {


        /// <summary>
        /// 是否有网络
        /// <para>注意：此方法只在 Unity 编辑器中有效</para>
        /// </summary>
        public static bool NetworkAvailable
        {
            get
            {
                return Application.internetReachability == NetworkReachability.ReachableViaLocalAreaNetwork ||
                    Application.internetReachability == NetworkReachability.ReachableViaCarrierDataNetwork;
            }
        }

        [DllImport("wininet")]
        private extern static bool InternetGetConnectedState(out int connectionDescription, int reservedValue);

        /// <summary>
        /// C#判断是否联网
        /// </summary>
        /// <returns></returns>
        public static bool IsConnectedInternet()
        {
            int i = 0;
            if (InternetGetConnectedState(out i, 0))
                return true;
            else
                return false;
        }

        //public static void 

        /// <summary>
        /// 退出
        /// </summary>
        public static void Quit()
        {
            Application.Quit();

#if UNITY_EDITOR
            EditorApplication.isPlaying = false;
#endif
        }
    }

}