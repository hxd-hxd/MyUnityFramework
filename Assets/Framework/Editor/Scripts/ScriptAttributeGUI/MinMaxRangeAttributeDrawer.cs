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
                base.OnAttribute(pos, property, label);
        }
        protected override bool OnAttributeHint(SerializedProperty property, out string msg, out MessageType type)
        {
            msg = $"MinMaxRangeAttribute 不支持的类型 \"MinMax<{property.type}>\"，仅支持设定 MinMax<T> T 为 \"float 、int\" 等数值类型";
            if (!isMinMaxT)
            {
                msg = $"MinMaxRangeAttribute 不支持的类型 \"{property.type}\"，仅支持设定 \"float 、int\" 等数值类型";
            }
            type = MessageType.Error;
            return !(property.propertyType == SerializedPropertyType.Float
                || property.propertyType == SerializedPropertyType.Integer);
        }
    }
}