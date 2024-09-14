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
    [CustomEditor(typeof(TestSetActive))]
    class TestSetActiveEditor : Editor
    {
        TestSetActive my => target as TestSetActive;

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            EditorGUILayout.Space();

            my.active = EditorGUILayout.Toggle("设置目标激活状态", my.active);

            if (GUILayout.Button("设置到目标"))
            {
                if (my.target)
                {
                    my.target.SetActive(my.active);
                }
            }
        }
    }
#endif
    public class TestSetActive : MonoBehaviour
    {
        
        public bool active;
        public GameObject target;

    }
}