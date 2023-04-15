// -------------------------
// 创建日期：2023/3/29 17:11:30
// -------------------------

using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Framework
{

    public static class PlatformUtility
    {

        /// <summary>
        /// 当前平台
        /// </summary>
        public static Platform platform
        {
            get
            {
#if UNITY_EDITOR
                if (UnityEditor.EditorUserBuildSettings.activeBuildTarget == UnityEditor.BuildTarget.Android)
                    return Platform.Android;
                else if (UnityEditor.EditorUserBuildSettings.activeBuildTarget == UnityEditor.BuildTarget.iOS)
                    return Platform.IPhone;
                else if (UnityEditor.EditorUserBuildSettings.activeBuildTarget == UnityEditor.BuildTarget.WebGL)
                    return Platform.WebGL;
                else
                    return Platform.PC;
#else
                if (Application.platform == RuntimePlatform.Android)
                    return Platform.Android;
                else if (Application.platform == RuntimePlatform.IPhonePlayer)
                    return Platform.IPhone;
                else if (Application.platform == RuntimePlatform.WebGLPlayer)
                    return Platform.WebGL;
                else
                    return Platform.PC;
#endif
            }
        }

    }
}