// -------------------------
// 创建日期：2023/3/24 10:33:47
// -------------------------

using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Diagnostics;

namespace Framework
{

    public static class WindowsSystem
    {
        /*
         请注意，在Windows平台上， 每个Unity实例使用单个进程，这意味着在调用Application.Quit)时，当前Unity程序将被关闭。 而在Mac上，当调用Application.Quit)时，资源由操作系统释放但Unity进程保持打开状态。
         上述示例中，通过使用Process.Start启动一个新进程，并通过首个命令行参数来确定要启动何种应用程序，以重启整个程序。
         */

        /// <summary>
        /// 重启当前应用程序
        /// </summary>
        public static void Restart()
        {
            // 退出应用程序
            Application.Quit();
            // 拼接启动应用程序的命令
            string[] arguments = Environment.GetCommandLineArgs();
            string path = arguments[0];
            // 开启子进程重启应用程序
            Process.Start(path, "");
        }
    }
}