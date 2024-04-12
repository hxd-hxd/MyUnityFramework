using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Framework.Editor
{
    /// <summary>
    /// �� <see cref="EditorGUIUtility"/> ����չ
    /// </summary>
    public static class EditorGUIUtilityExtend
    {
        /// <summary>
        /// ���ñ�ǩ��� <see cref="EditorGUIUtility.labelWidth"/>��ִ�лص���ԭ
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