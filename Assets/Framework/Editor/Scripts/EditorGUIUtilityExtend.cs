using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Framework.Editor
{
    /// <summary>
    /// 对 <see cref="EditorGUIUtility"/> 的扩展
    /// </summary>
    public static class EditorGUIUtilityExtend
    {
        /// <summary>
        /// 设置标签宽度 <see cref="EditorGUIUtility.labelWidth"/>，执行回调后还原
        /// </summary>
        /// <param name="w"></param>
        /// <param name="action"></param>
        public static void SetLabelWidth(float w, Action action)
        {
            float oldw = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = w;
            action?.Invoke();
            EditorGUIUtility.labelWidth = oldw;
        }
    }
}