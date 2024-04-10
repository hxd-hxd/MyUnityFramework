// -------------------------
// 创建日期：2024/4/8 11:22:15
// -------------------------

using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Framework
{
#if UNITY_EDITOR
    using UnityEditor;
    using UnityEngine.SocialPlatforms;
    using UnityEngine.UIElements;

    [CustomPropertyDrawer(typeof(MinMax<>))]
    internal class MinMaxDrawer : PropertyDrawer
    {
        protected float _lineNum = 1;

        public override void OnGUI(Rect pos, SerializedProperty property, GUIContent label)
        {
            _lineNum = 1;
            label = EditorGUI.BeginProperty(pos, label, property);

            var min = property.FindPropertyRelative("min");
            var max = property.FindPropertyRelative("max");

            if (IsUniline(min.propertyType))
            {
                //绘制标签
                pos = EditorGUI.PrefixLabel(pos, GUIUtility.GetControlID(FocusType.Keyboard), label);
                PropertyField(pos, min, label);
                PropertyField(pos, max, label, 1);
                //var br = pos;
                //br.y += EditorGUIUtility.singleLineHeight;
                //_lineNum += 1;
                //EditorGUI.HelpBox(br, "666", MessageType.Info);
                ////EditorGUI.MultiPropertyField(pos, new GUIContent[]{ new GUIContent(min.displayName), new GUIContent(max.displayName) }, property, GUIContent.none);
            }
            else
            {
                EditorGUI.PropertyField(pos, property, new GUIContent(property.displayName), true);
            }
            //Debug.Log($"特性：{attribute}");
            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUI.GetPropertyHeight(property, label, true) * _lineNum;
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
                float vWidth = pos.width * 0.3f;// 值宽度
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
                EditorGUI.indentLevel++;
                //EditorGUILayout.PropertyField(property, displayName, true);
                EditorGUI.PropertyField(pos, property, new GUIContent(property.displayName), true);
                //EditorGUIUtility
                EditorGUI.indentLevel--;
            }
        }
        protected virtual void OnAttribute(Rect pos, SerializedProperty property, GUIContent label)
        {
            //Debug.Log(attribute is RangeAttribute);
            EditorGUI.PropertyField(pos, property, GUIContent.none);
        }

        protected bool IsUniline(SerializedPropertyType type)
        {
            //return false;
            return type == SerializedPropertyType.Float
                || type == SerializedPropertyType.Integer
                || type == SerializedPropertyType.Boolean
                || type == SerializedPropertyType.Enum
                ;
        }
    }


    [CustomPropertyDrawer(typeof(RangeAttribute))]
    internal class MinMax_RangeAttribute_Drawer : MinMaxDrawer
    {
        protected override void OnAttribute(Rect pos, SerializedProperty property, GUIContent label)
        {
            var range = attribute as RangeAttribute;
            if (property.propertyType == SerializedPropertyType.Float)
            {
                EditorGUI.Slider(pos, property, range.min, range.max, label);
            }
            else if (property.propertyType == SerializedPropertyType.Integer)
            {
                EditorGUI.IntSlider(pos, property, (int)range.min, (int)range.max, label);
            }
            else
                base.OnAttribute(pos, property, label);
        }
    }
#endif

    [Serializable]
    public struct MinMax<T>
    {
        public T min, max;

        //[SerializeField]
        //private T _min,_max;

        //public T min
        //{
        //    get { return _min; }
        //    set { _min = value; }
        //}

        //public T max
        //{
        //    get { return _max; }
        //    set { _max = value; }
        //}

        public MinMax(T min, T max)
        {
            this.min = min;
            this.max = max;
        }
    }
}