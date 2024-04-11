using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.Text.RegularExpressions;

namespace Framework.Editor
{

    [CustomPropertyDrawer(typeof(MinMax<>))]
    internal class MinMaxDrawer : LineCountPropertyDrawer
    {
        protected bool isMinMaxT = true;

        public override void OnGUI(Rect pos, SerializedProperty property, GUIContent label)
        {
            base.OnGUI(pos, property, label);
            pos.height = singleLineHeight;
            label = EditorGUI.BeginProperty(pos, label, property);

            var min = property.FindPropertyRelative("min");
            var max = property.FindPropertyRelative("max");

            // 非 MinMax<T>
            //if (min == null || max == null)
            if (property.type != "MinMax`1")// 泛型类
            {
                isMinMaxT = false;
                OnAttribute(pos, property, label);
                OnAttributeHint(pos, property, label);

                EditorGUI.EndProperty();
                return;
            }

            if (IsUniline(min.propertyType))
            {
                isMinMaxT = true;
                //绘制标签
                float oldLabelWidth = EditorGUIUtility.labelWidth;
                EditorGUIUtility.labelWidth *= 0.75f;
                pos = EditorGUI.PrefixLabel(pos, GUIUtility.GetControlID(FocusType.Keyboard), label);
                EditorGUIUtility.labelWidth = oldLabelWidth;
                PropertyField1(pos, min, label);
                PropertyField1(pos, max, label, 1);
                OnAttributeHint(pos, min, label);
                ////EditorGUI.MultiPropertyField(pos, new GUIContent[]{ new GUIContent(min.displayName), new GUIContent(max.displayName) }, property, GUIContent.none);
            }
            else
            {
                isMinMaxT = false;
                OnAttribute(pos, property, new GUIContent(property.displayName));
                //OnAttributeHint(pos, min, label);
            }

            EditorGUI.EndProperty();
        }
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUI.GetPropertyHeight(property, label, true) * lineCount;
        }

        /// <summary>
        /// 处理特性
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="property"></param>
        /// <param name="label"></param>
        protected virtual void OnAttribute(Rect pos, SerializedProperty property, GUIContent label)
        {
            EditorGUI.PropertyField(pos, property, label, true);
        }
        /// <summary>
        /// 应用特性时的提示信息，只能用于自定义控制的行
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="property"></param>
        /// <param name="label"></param>
        protected virtual void OnAttributeHint(Rect pos, SerializedProperty property, GUIContent label)
        {
            if (OnAttributeHint(property, out string msg, out var msgType))
            {
                var br = pos;

                br.x = originalPos.x;
                br.width = originalPos.width;

                br.y += currentLineCountHeight;
                float line = MsgLine(msg, pos.width);// 计算消息所占的行数
                line = line < 2 ? 1.5f : line;
                br.height = singleLineHeight * line;
                lineCount += line;
                EditorGUI.HelpBox(br, msg, msgType);
                //EditorGUI.HelpBox(new Rect(pos.x, pos.y + currentLineCountHeight, pos.width, singleLineHeight), "新行", MessageType.Warning);
                //lineCount += 1;
            }
        }
        /// <summary>
        /// 应用特性时的提示信息
        /// </summary>
        /// <param name="property"></param>
        protected virtual bool OnAttributeHint(SerializedProperty property, out string msg, out MessageType type)
        {
            msg = null;
            type = MessageType.None;
            return false;
        }

        protected virtual void OnDefault(Rect pos, SerializedProperty property, GUIContent label)
        {
            EditorGUI.indentLevel++;
            OnAttribute(pos, property, label);
            EditorGUI.indentLevel--;
        }
        protected virtual void OnUniline(Rect pos, SerializedProperty property, GUIContent label, int level = 0)
        {
            int l = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;

            float unit = 4;
            float nameWidth = 30;// 名字宽度
            float vWidth = pos.width * 0.495f;// 值宽度
            float vOffsetX = (vWidth + unit) * level;// 值 x 偏移

            // 使用适应性的宽度
            var vRect = new Rect(pos.x + vOffsetX, pos.y, vWidth, pos.height);

            float oldLabelWidth = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = nameWidth;
            OnAttribute(vRect, property, label);
            EditorGUIUtility.labelWidth = oldLabelWidth;

            EditorGUI.indentLevel = l;
        }

        protected void PropertyField(Rect pos, SerializedProperty property, GUIContent label, int level = 0)
        {
            var displayName = new GUIContent(property.displayName);
            if (IsUniline(property.propertyType))
            {
                int l = EditorGUI.indentLevel;
                EditorGUI.indentLevel = 0;

                float unit = 4;
                float nameWidth = 30;// 名字宽度
                float vWidth = pos.width * 0.375f;// 值宽度
                float nameOffsetX = (nameWidth + vWidth + unit * 3) * level;// 名字 x 偏移
                float vOffsetX = (nameOffsetX) * level + nameWidth;// 值 x 偏移

                // 使用适应性的宽度
                var nameRect = new Rect(pos.x + nameOffsetX, pos.y, nameWidth, pos.height);
                var vRect = new Rect(pos.x + vOffsetX, pos.y, vWidth, pos.height);
                //EditorGUI.LabelField(new Rect(pos.x + nameOffsetX, pos.y, nameWidth, pos.height), displayName);
                EditorGUI.HandlePrefixLabel(pos, nameRect, displayName);
                //EditorGUI.PrefixLabel(nameRect/*, GUIUtility.GetControlID(FocusType.Keyboard, vRect)*/, displayName);

                OnAttribute(vRect, property, GUIContent.none);

                EditorGUI.indentLevel = l;
            }
            else
            {
                OnDefault(pos, property, displayName);
            }
        }
        protected void PropertyField1(Rect pos, SerializedProperty property, GUIContent label, int level = 0)
        {
            var displayName = new GUIContent(property.displayName);
            if (IsUniline(property.propertyType))
            {
                OnUniline(pos, property, displayName, level);
            }
            else
            {
                OnDefault(pos, property, displayName);
            }
        }

        protected bool IsUniline(SerializedPropertyType type)
        {
            //return false;
            return type == SerializedPropertyType.Float
                || type == SerializedPropertyType.Integer
                || type == SerializedPropertyType.Boolean
                || type == SerializedPropertyType.Enum
                //|| !isMinMaxT
                ;
        }

        /// <summary>
        /// 根据提供的信息和宽度计算所占行数（不支持富文本）
        /// </summary>
        /// <returns></returns>
        public int MsgLine(string msg, float width)
        {
            int result = 0;
            var strs = Regex.Split(msg, "\n");
            foreach (var str in strs)
            {
                result += MsgLineIgnoreNewlines(str, width);
            }
            return result;
        }
        /// <summary>
        /// 根据提供的信息和宽度计算所占行数（不支持富文本）
        /// </summary>
        /// <returns></returns>
        public int MsgLineIgnoreNewlines(string msg, float width)
        {
            int result = 1;
            if (msg != null)
            {
                int wCharNum = DivSur((int)width, 6);// 计算可容纳的字符数
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