using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Framework.Editor
{

    [CustomPropertyDrawer(typeof(MinMaxRangeAttribute))]
    internal class MinMaxRangeAttributeDrawer : MinMaxDrawer
    {
        protected override void OnAttribute(Rect pos, SerializedProperty property, GUIContent label)
        {
            var range = attribute as MinMaxRangeAttribute;
            if (property.propertyType == SerializedPropertyType.Float)
            {
                EditorGUI.Slider(pos, property, range.min, range.max, label);
            }
            else if (property.propertyType == SerializedPropertyType.Integer)
            {
                EditorGUI.IntSlider(pos, property, (int)range.min, (int)range.max, label);
            }
            else
            {
                base.OnAttribute(pos, property, label);
                //EditorGUI.LabelField(pos, label.text, "Use MinMaxRangeAttribute with float or int.");
            }
        }
        protected override bool OnAttributeHint(SerializedProperty property, out string msg, out MessageType type)
        {
            msg = $"MinMaxRangeAttribute 不支持的类型 \"MinMax<{property.type}>\"，仅支持设定 MinMax<T> T 为 \"float 、int\" 等数值类型";
            if (!isMinMaxT)
            {
                msg = $"MinMaxRangeAttribute 不支持的类型 \"{property.type}\"，仅支持设定 \"float 、int\" 等数值类型";
            }
            //msg += "\r\n1...........................................1\r\n2...........................................2\r\n3...........................................3";
            type = MessageType.Error;
            return !(property.propertyType == SerializedPropertyType.Float
                || property.propertyType == SerializedPropertyType.Integer);
        }

        bool useDefault = false;
        protected override float GetAttributeHintY()
        {
            if (useDefault) return base.GetAttributeHintY();

            return singleLineHeight;
        }
        protected override float GetAttributeHintH(float msgLine)
        {
            if(useDefault) return base.GetAttributeHintH(msgLine);

            float h = singleLineHeight * msgLine;
            float phOnLabel = propertyHeight - singleLineHeight;// 去掉标题后的属性行高
            // 多行则覆盖，单行则随后
            if ((int)phOnLabel > 0)// 多行
            {
                float b = phOnLabel / singleLineHeight;// 去标行高所占行数
                // 取消息行多余的部分叠加
                //if (b < msgLine)
                //{
                //    lineCount += msgLine - b;
                //}

                lineCount += Math.Max(msgLine - b, 0);
                h = Math.Max(h, phOnLabel);
            }
            else
            {
                lineCount += msgLine;
            }
            return h;
        }
    }
}