// -------------------------
// 创建日期：2023/3/2 18:03:52
// -------------------------

using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Framework
{

    public static class AndroidSystem
    {
        /// <summary>
        /// 获取系统语言
        /// </summary>
        public static string GetDisplayLanguage()
        {
            using (AndroidJavaClass cls = new AndroidJavaClass("java.util.Locale"))
            {
                using (AndroidJavaObject locale = cls.CallStatic<AndroidJavaObject>("getDefault"))
                {
                    string language = locale?.Call<string>("getDisplayLanguage");

                    return language;
                }
            }
        }
    }
}