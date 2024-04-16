using System.Collections;
using System.Collections.Generic;
using System;
using System.Text.RegularExpressions;

using UnityEditor;
using UnityEngine;

namespace Framework.Editor
{

    [CustomPropertyDrawer(typeof(MinMax<>))]
    public class MinMaxDrawer : LineCountPropertyDrawer
    {
        /// <summary>
        /// 是否 <see cref="MinMax{T}"/>
        /// </summary>
        protected bool isMinMaxT = true;
        /// <summary>
        /// <see cref="MinMax{T}"/> 泛型 T 的类型
        /// </summary>
        protected SerializedPropertyType minMaxTType;

        public override void OnGUI(Rect pos, SerializedProperty property, GUIContent label)
        {
            base.OnGUI(pos, property, label);
            pos.height = singleLineHeight;
            label = EditorGUI.BeginProperty(pos, label, property);

            // 非 MinMax<T> 泛型类
            //if (min == null || max == null)
            if (property.type != "MinMax`1")
            {
                isMinMaxT = false;
                OnAttribute(pos, property, label);
                OnAttributeHint(pos, property);

                EditorGUI.EndProperty();
                return;
            }

            var min = property.FindPropertyRelative("min");
            var max = property.FindPropertyRelative("max");

            isMinMaxT = true;
            minMaxTType = min.propertyType;
            if (IsUniline(min.propertyType))
            {
                //绘制标签
                // 会导致后续的 gui x 轴位置改变
                EditorGUIUtilityExtend.SetLabelWidth(EditorGUIUtility.labelWidth *= 0.75f,
                    () => pos = EditorGUI.PrefixLabel(pos, GUIUtility.GetControlID(FocusType.Keyboard), label));
                PropertyField1(pos, min, label);
                PropertyField1(pos, max, label, 1);
                OnAttributeHint(pos, min);
                ////EditorGUI.MultiPropertyField(pos, new GUIContent[]{ new GUIContent(min.displayName), new GUIContent(max.displayName) }, property, GUIContent.none);
            }
            else
            {
                OnAttribute(pos, property, new GUIContent(property.displayName));
                OnAttributeHint(pos, min);
            }

            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            // 调用父类更新
            base.GetPropertyHeight(property, label);

            // 修正属性高度
            AmendPropertyHeight();

            return totalHeight;
        }
        /// <summary>修正属性高度</summary>
        protected virtual void AmendPropertyHeight()
        {
            // 处理 Vector4 内联布局的高度显示问题
            if (IsUnilineSerializedPropertyType(SerializedPropertyType.Vector4)
                && minMaxTType == SerializedPropertyType.Vector4)
            {
                // 除非 Unity 在后续版本中做了修改，否则这里的数值是固定的
                if ((int)propertyHeight == 58) propertyHeight = 18;
                else if ((int)propertyHeight == 138 || (int)propertyHeight == 218) propertyHeight = 100;
            }

        }

        /// <summary>
        /// 处理特性，最终进行
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
        protected virtual void OnAttributeHint(Rect pos, SerializedProperty property)
        {
            if (OnAttributeHint(property, out string msg, out var msgType))
            {
                var br = GetAttributeHintRect(pos, msg);
                var old_contentColor = GUI.contentColor;
                var old_backgroundColor = GUI.backgroundColor;
                //var old_color = GUI.color;
                //GUI.color = Color.yellow;
                GUI.backgroundColor = Color.yellow;
                //GUI.contentColor = new Color(1,1,102/255f, 1);
                GUI.contentColor = new Color(1, 1, 60 / 255f, 1);
                //GUI.contentColor = new Color(1, 70 / 255f, 70 / 255f, 1);
                //GUI.contentColor = new Color(1, 123 / 255f, 0, 1);
                EditorGUI.HelpBox(br, msg, msgType);
                //GUI.color = old_color;
                GUI.backgroundColor = old_backgroundColor;
                GUI.contentColor = old_contentColor;
                //EditorGUI.HelpBox(new Rect(pos.x, pos.y + currentLineCountHeight, pos.width, singleLineHeight), "新行", MessageType.Warning);
                //lineCount += 1;
            }
        }
        /// <summary>应用特性时的提示信息</summary>
        protected virtual bool OnAttributeHint(SerializedProperty property, out string msg, out MessageType type)
        {
            msg = null;
            type = MessageType.None;
            return false;
        }
        /// <summary>应用特性时的提示信息矩形定义</summary>
        protected virtual Rect GetAttributeHintRect(Rect pos, string msg)
        {
            var br = pos;
            br.x = originalPos.x;
            br.width = originalPos.width;

            float msgLine = TextLine(msg, pos.width);// 计算消息所占的行数
            msgLine = msgLine < 2 ? 1.5f : msgLine;
            br.height = GetAttributeHintH(msgLine);

            //br.y += currentLineCountHeight;
            br.y += GetAttributeHintY();

            return br;
        }
        /// <summary>应用特性时的提示信息矩形定义 y 轴位置</summary>
        protected virtual float GetAttributeHintY()
        {
            return propertyHeight;// 始终保持在 gui 最后
        }
        /// <summary>应用特性时的提示信息矩形定义高度</summary>
        protected virtual float GetAttributeHintH(float msgLine)
        {
            lineCount += msgLine;

            float height = singleLineHeight * msgLine;
            return height;
        }

        /// <summary>默认绘制</summary>
        protected virtual void OnDefault(Rect pos, SerializedProperty property, GUIContent label)
        {
            //EditorGUI.indentLevel++;
            OnAttribute(pos, property, label);
            //EditorGUI.indentLevel--;
        }
        /// <summary>
        /// 执行单行绘制
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="property"></param>
        /// <param name="label"></param>
        /// <param name="level"></param>
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

            EditorGUIUtilityExtend.SetLabelWidth(nameWidth,
                    () => OnAttribute(vRect, property, label));

            EditorGUI.indentLevel = l;
        }

        [Obsolete]
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
                // 如果不满足单行绘制条件，则使用默认绘制
                OnDefault(pos, property, displayName);
            }
        }

        /// <summary>是否在一行中显示</summary>
        protected bool IsUniline(SerializedPropertyType type)
        {
            //return false;
            return IsUnilineSerializedPropertyType(type)
                //|| !isMinMaxT
                ;
        }
        /// <summary>指定的 <see cref="SerializedPropertyType"/> 是否在一行中显示</summary>
        protected bool IsUnilineSerializedPropertyType(SerializedPropertyType type)
        {
            //return false;
            return type == SerializedPropertyType.Float
                || type == SerializedPropertyType.Integer
                || type == SerializedPropertyType.Boolean
                || type == SerializedPropertyType.Enum
                || type == SerializedPropertyType.Color
                || type == SerializedPropertyType.Vector2 || type == SerializedPropertyType.Vector2Int
                || type == SerializedPropertyType.Vector3 || type == SerializedPropertyType.Vector3Int
                //|| type == SerializedPropertyType.Vector4
                //|| !isMinMaxT
                ;
        }
    }
}