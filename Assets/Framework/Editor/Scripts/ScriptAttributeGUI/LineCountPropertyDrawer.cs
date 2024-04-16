using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
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
        protected float lineCount = 0;
        protected float _singleLineHeight = EditorGUIUtility.singleLineHeight;
        protected float _propertyHeight = 0;
        protected Rect _originalPos;

        /// <summary>单字符宽度</summary>
        /// <remarks>可用于设置 <see cref="TextLine(string, float)"/> 内部字符计算</remarks>
        public int singleCharWidth = 8;

        /// <summary>
        /// 单行行高（默认 18）
        /// </summary>
        protected float singleLineHeight { get => _singleLineHeight; set => _singleLineHeight = value; }
        /// <summary>默认属性高度</summary>
        protected float propertyHeight { get => _propertyHeight;  set => _propertyHeight = value; }
        /// <summary>
        /// 当前行数对应的高度，可用于定位最新行 y 的位置
        /// <para></para><see cref="lineCount"/> * <see cref="singleLineHeight"/>
        /// <para>例：pos.y + currentLineCountHeight 就表示最新行位置</para>
        /// </summary>
        protected float currentLineCountHeight => lineCount * singleLineHeight;
        /// <summary>总高度，自定义 + 默认属性</summary>
        /// <remarks><see cref="currentLineCountHeight"/> + <see cref="propertyHeight"/></remarks>
        protected float totalHeight => currentLineCountHeight + propertyHeight;
        /// <summary>原始矩形位置</summary>
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
            //Debug.Log($"{property.displayName}\tph：{propertyHeight}，\totalHeight：{totalHeight}");
            return totalHeight;
        }

        /// <summary>
        /// 根据提供的文本和宽度计算所占行数
        /// （不支持富文本，使用 <see cref="singleCharWidth"/> 修改计算时的单个宽度，也不能处理不同字符在 gui 显示上的宽度不同的问题 例如：l 和 L）
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
        /// 根据提供的文本和宽度计算忽略换行符所占行数
        /// （不支持富文本，使用 <see cref="singleCharWidth"/> 修改计算时的单个宽度，也不能处理不同字符在 gui 显示上的宽度不同的问题 例如：l 和 L）
        /// </summary>
        /// <returns></returns>
        public int TextLineIgnoreNewlines(string msg, float width)
        {
            int result = 1;
            if (msg != null)
            {
                int wCharNum = DivSur((int)width, singleCharWidth);// 计算可容纳的字符数
                int line = DivSur(msg.Length, wCharNum);
                if (line > 0) result = line;
            }
            return result;
        }

        /// <summary>
        /// 有余数的商
        /// <para>例；10 / 4 = 3，DivSur(10, 4) return 3</para>
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