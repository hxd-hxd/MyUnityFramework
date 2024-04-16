using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
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
        protected float lineCount = 0;
        protected float _singleLineHeight = EditorGUIUtility.singleLineHeight;
        protected float _propertyHeight = 0;
        protected Rect _originalPos;

        /// <summary>���ַ����</summary>
        /// <remarks>���������� <see cref="TextLine(string, float)"/> �ڲ��ַ�����</remarks>
        public int singleCharWidth = 8;

        /// <summary>
        /// �����иߣ�Ĭ�� 18��
        /// </summary>
        protected float singleLineHeight { get => _singleLineHeight; set => _singleLineHeight = value; }
        /// <summary>Ĭ�����Ը߶�</summary>
        protected float propertyHeight { get => _propertyHeight;  set => _propertyHeight = value; }
        /// <summary>
        /// ��ǰ������Ӧ�ĸ߶ȣ������ڶ�λ������ y ��λ��
        /// <para></para><see cref="lineCount"/> * <see cref="singleLineHeight"/>
        /// <para>����pos.y + currentLineCountHeight �ͱ�ʾ������λ��</para>
        /// </summary>
        protected float currentLineCountHeight => lineCount * singleLineHeight;
        /// <summary>�ܸ߶ȣ��Զ��� + Ĭ������</summary>
        /// <remarks><see cref="currentLineCountHeight"/> + <see cref="propertyHeight"/></remarks>
        protected float totalHeight => currentLineCountHeight + propertyHeight;
        /// <summary>ԭʼ����λ��</summary>
        protected Rect originalPos => _originalPos;

        public override void OnGUI(Rect pos, SerializedProperty property, GUIContent label)
        {
            _originalPos = pos;
            lineCount = 0;
            pos.height = singleLineHeight;
        }
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            propertyHeight = EditorGUI.GetPropertyHeight(property, label, true);
            //Debug.Log($"{property.displayName}\tph��{propertyHeight}��\totalHeight��{totalHeight}");
            return totalHeight;
        }

        /// <summary>
        /// �����ṩ���ı��Ϳ�ȼ�����ռ����
        /// ����֧�ָ��ı���ʹ�� <see cref="singleCharWidth"/> �޸ļ���ʱ�ĵ�����ȣ�Ҳ���ܴ���ͬ�ַ��� gui ��ʾ�ϵĿ�Ȳ�ͬ������ ���磺l �� L��
        /// </summary>
        /// <returns></returns>
        public virtual int TextLine(string msg, float width)
        {
            int result = 0;
            msg = msg.Replace("\r\n", "\n");
            var strs = Regex.Split(msg, "\n");
            foreach (var str in strs)
            {
                result += TextLineIgnoreNewlines(str, width);
            }
            return result;
        }
        /// <summary>
        /// �����ṩ���ı��Ϳ�ȼ�����Ի��з���ռ����
        /// ����֧�ָ��ı���ʹ�� <see cref="singleCharWidth"/> �޸ļ���ʱ�ĵ�����ȣ�Ҳ���ܴ���ͬ�ַ��� gui ��ʾ�ϵĿ�Ȳ�ͬ������ ���磺l �� L��
        /// </summary>
        /// <returns></returns>
        public int TextLineIgnoreNewlines(string msg, float width)
        {
            int result = 1;
            if (msg != null)
            {
                int wCharNum = DivSur((int)width, singleCharWidth);// ��������ɵ��ַ���
                int line = DivSur(msg.Length, wCharNum);
                if (line > 0) result = line;
            }
            return result;
        }

        /// <summary>
        /// ����������
        /// <para>����10 / 4 = 3��DivSur(10, 4) return 3</para>
        /// </summary>
        /// <returns></returns>
        public int DivSur(int a, int b)
        {
            if (b == 0) return 0;
            //int result = Math.DivRem(a, b, out int sur);
            int result = a / b;
            if (a % b != 0) result += 1;
            return result;
        }
    }
}