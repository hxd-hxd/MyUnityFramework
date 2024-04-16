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
            msg = $"MinMaxRangeAttribute ��֧�ֵ����� \"MinMax<{property.type}>\"����֧���趨 MinMax<T> T Ϊ \"float ��int\" ����ֵ����";
            if (!isMinMaxT)
            {
                msg = $"MinMaxRangeAttribute ��֧�ֵ����� \"{property.type}\"����֧���趨 \"float ��int\" ����ֵ����";
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
            float phOnLabel = propertyHeight - singleLineHeight;// ȥ�������������и�
            // �����򸲸ǣ����������
            if ((int)phOnLabel > 0)// ����
            {
                float b = phOnLabel / singleLineHeight;// ȥ���и���ռ����
                // ȡ��Ϣ�ж���Ĳ��ֵ���
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