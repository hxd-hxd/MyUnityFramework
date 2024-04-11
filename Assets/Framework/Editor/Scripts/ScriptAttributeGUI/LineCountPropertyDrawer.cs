using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Framework.Editor
{
    /// <summary>
    /// 行数属性绘制器
    /// <para>例
    /// <code>
    /// public override void OnGUI(Rect pos, SerializedProperty property, GUIContent label)
    /// {
    ///    base.OnGUI(pos, property, label);
    ///    pos.height = singleLineHeight;
    /// }
    /// </code>
    /// </para>
    /// </summary>
    public abstract class LineCountPropertyDrawer : PropertyDrawer
    {
        /// <summary>
        /// 行数，每添加新行之后，都要相应增加此数
        /// <para></para>注意：是之后
        /// </summary>
        protected float lineCount = 1;
        protected Rect _originalPos;

        /// <summary>
        /// 单行行高（18）
        /// </summary>
        protected float singleLineHeight => EditorGUIUtility.singleLineHeight;
        /// <summary>
        /// 当前行数对应的高度，可用于定位最新行 y 的位置
        /// <para>例：pos.y + currentLineCountHeight 就表示最新行位置</para>
        /// </summary>
        protected float currentLineCountHeight => lineCount * singleLineHeight;
        /// <summary>原始矩形位置</summary>
        protected Rect originalPos => _originalPos;

        public override void OnGUI(Rect pos, SerializedProperty property, GUIContent label)
        {
            _originalPos = pos;
            lineCount = 1;
            pos.height = singleLineHeight;
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUI.GetPropertyHeight(property, label, true) * lineCount;
        }
    }
}