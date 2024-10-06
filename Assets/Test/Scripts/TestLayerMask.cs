// -------------------------
// 创建日期：2023/3/6 18:22:45
// -------------------------

using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Test
{
#if UNITY_EDITOR
using UnityEditor;
    [CustomEditor(typeof(TestLayerMask))]
    class TestLayerMaskEditor : Editor
    {
        TestLayerMask my => target as TestLayerMask;

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            EditorGUILayout.LabelField($"层的值：{my.mask.value}");
            my.mask.value = EditorGUILayout.IntField("设置层的值", my.mask.value);


        }
    }
#endif
    public class TestLayerMask : MonoBehaviour
    {
        public LayerMask mask = new LayerMask() { value = 100 };

    }
}