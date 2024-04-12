using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.Text.RegularExpressions;
using System;
using static UnityEditor.PlayerSettings;

namespace Framework.Editor
{

    [CustomPropertyDrawer(typeof(MinMax<>))]
    internal class MinMaxDrawer : LineCountPropertyDrawer
    {
        /// <summary>
        /// �Ƿ� <see cref="MinMax{T}"/>
        /// </summary>
        protected bool isMinMaxT = true;

        public override void OnGUI(Rect pos, SerializedProperty property, GUIContent label)
        {
            base.OnGUI(pos, property, label);
            pos.height = singleLineHeight;
            label = EditorGUI.BeginProperty(pos, label, property);

            var min = property.FindPropertyRelative("min");
            var max = property.FindPropertyRelative("max");

            // �� MinMax<T> ������
            //if (min == null || max == null)
            if (property.type != "MinMax`1")
            {
                isMinMaxT = false;
                OnAttribute(pos, property, label);
                OnAttributeHint(pos, property);

                EditorGUI.EndProperty();
                return;
            }

            isMinMaxT = true;
            if (IsUniline(min.propertyType))
            {
                //���Ʊ�ǩ
                // �ᵼ�º����� gui x ��λ�øı�
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

        /// <summary>
        /// �������ԣ����ս���
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="property"></param>
        /// <param name="label"></param>
        protected virtual void OnAttribute(Rect pos, SerializedProperty property, GUIContent label)
        {
            EditorGUI.PropertyField(pos, property, label, true);
        }

        /// <summary>
        /// Ӧ������ʱ����ʾ��Ϣ��ֻ�������Զ�����Ƶ���
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
                //EditorGUI.HelpBox(new Rect(pos.x, pos.y + currentLineCountHeight, pos.width, singleLineHeight), "����", MessageType.Warning);
                //lineCount += 1;
            }
        }
        /// <summary>Ӧ������ʱ����ʾ��Ϣ</summary>
        protected virtual bool OnAttributeHint(SerializedProperty property, out string msg, out MessageType type)
        {
            msg = null;
            type = MessageType.None;
            return false;
        }
        /// <summary>Ӧ������ʱ����ʾ��Ϣ���ζ���</summary>
        protected virtual Rect GetAttributeHintRect(Rect pos, string msg)
        {
            var br = pos;
            br.x = originalPos.x;
            br.width = originalPos.width;

            float msgLine = TextLine(msg, pos.width);// ������Ϣ��ռ������
            msgLine = msgLine < 2 ? 1.5f : msgLine;
            br.height = GetAttributeHintH(msgLine);

            //br.y += currentLineCountHeight;
            br.y += GetAttributeHintY();

            return br;
        }
        protected virtual float GetAttributeHintY()
        {
            return propertyHeight;// ʼ�ձ����� gui ���
        }
        protected virtual float GetAttributeHintH(float msgLine)
        {
            lineCount += msgLine;

            float height = singleLineHeight * msgLine;
            return height;
        }

        /// <summary>
        /// ��������㵥�л�ִ��������ʹ��Ĭ�ϻ���
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="property"></param>
        /// <param name="label"></param>
        protected virtual void OnDefault(Rect pos, SerializedProperty property, GUIContent label)
        {
            //EditorGUI.indentLevel++;
            OnAttribute(pos, property, label);
            //EditorGUI.indentLevel--;
        }
        /// <summary>
        /// ִ�е��л���
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
            float nameWidth = 30;// ���ֿ��
            float vWidth = pos.width * 0.495f;// ֵ���
            float vOffsetX = (vWidth + unit) * level;// ֵ x ƫ��

            // ʹ����Ӧ�ԵĿ��
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
                float nameWidth = 30;// ���ֿ��
                float vWidth = pos.width * 0.375f;// ֵ���
                float nameOffsetX = (nameWidth + vWidth + unit * 3) * level;// ���� x ƫ��
                float vOffsetX = (nameOffsetX) * level + nameWidth;// ֵ x ƫ��

                // ʹ����Ӧ�ԵĿ��
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

    }
}