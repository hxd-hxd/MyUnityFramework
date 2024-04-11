using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Framework.Editor
{
    /// <summary>
    /// �������Ի�����
    /// <para>��
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
        /// ������ÿ�������֮�󣬶�Ҫ��Ӧ���Ӵ���
        /// <para></para>ע�⣺��֮��
        /// </summary>
        protected float lineCount = 1;
        protected Rect _originalPos;

        /// <summary>
        /// �����иߣ�18��
        /// </summary>
        protected float singleLineHeight => EditorGUIUtility.singleLineHeight;
        /// <summary>
        /// ��ǰ������Ӧ�ĸ߶ȣ������ڶ�λ������ y ��λ��
        /// <para>����pos.y + currentLineCountHeight �ͱ�ʾ������λ��</para>
        /// </summary>
        protected float currentLineCountHeight => lineCount * singleLineHeight;
        /// <summary>ԭʼ����λ��</summary>
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