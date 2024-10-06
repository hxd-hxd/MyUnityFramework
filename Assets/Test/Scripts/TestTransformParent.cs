// -------------------------
// 创建日期：2024/9/3 10:57:35
// -------------------------

using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Framework;

namespace Test
{
#if UNITY_EDITOR
    using UnityEditor;

    [CustomEditor(typeof(TestTransformParent))]
    class TestTransformParentInspector : Editor
    {
        TestTransformParent my => (TestTransformParent)target;

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            //Undo.RecordObject(target, "");
            //Undo.RecordObject(this, "");

            if (my)
            {

                EditorGUILayout.ObjectField("根节点", my.transform.root, typeof(Transform), true);
                EditorGUILayout.ObjectField("父节点", my.transform.parent, typeof(Transform), true);

            }

        }

    }
#endif
    public class TestTransformParent : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {

        }

    }
}