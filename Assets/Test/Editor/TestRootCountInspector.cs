using System.Collections;
using System.Collections.Generic;
using Test;
using UnityEditor;
using UnityEngine;
using static UnityEditor.PlayerSettings;

namespace TestEditor
{
    [CustomEditor(typeof(TestRootCount))]
    public class TestRootCountInspector : Editor
    {
        List<GameObject> gos = new List<GameObject>();

        TestRootCount my => target as TestRootCount;

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            gos.Clear();
            my.gameObject.scene.GetRootGameObjects(gos);
            EditorGUILayout.LabelField($"场景跟物体数量：{gos.Count}");
            EditorGUILayout.LabelField($"层级物体数量：{my.transform.hierarchyCount}");
        }
    }
}