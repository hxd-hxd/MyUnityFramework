using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Test;

namespace TestEditor
{
    [CustomEditor(typeof(TestTransformSortOrder))]
    public class TestTransformSortOrderInspector : Editor
    {
        TestTransformSortOrder my => target as TestTransformSortOrder;

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (GUILayout.Button("通过兄弟索引调整到最后"))
            {
                my.transform.SetSiblingIndex(my.transform.parent.childCount);
            }
        }

    }
}