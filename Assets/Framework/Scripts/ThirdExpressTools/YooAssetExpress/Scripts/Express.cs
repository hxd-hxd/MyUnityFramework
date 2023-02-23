// -------------------------
// 创建日期：2023/2/20 17:54:21
// -------------------------

using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using YooAsset;

namespace Framework.YooAssetExpress
{
    /// <summary>
    /// 扩展库
    /// </summary>
    public static class Express
    {
        /// <summary>
        /// 计算场景加载时刷新的进度条
        /// </summary>
        /// <param name="operation"></param>
        /// <param name="minProgressTime">最小的进度持续时间（不会小于真实进度）</param>
        /// <param name="progressAcion">更新进度时的回调</param>
        /// <param name="progressFinish">更新进度完成的回调</param>
        /// <returns></returns>
        public static IEnumerator Progress(this SceneOperationHandle operation, float minProgressTime, Action<float> progressAcion, Action progressFinish)
        {

            yield return ExtendUtility.Progress(() =>
            {
               return operation.Progress;
            },
            minProgressTime,
            progressAcion,
            progressFinish);
        }
    }
}