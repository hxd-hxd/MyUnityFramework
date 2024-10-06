// -------------------------
// 创建日期：2024/9/3 10:57:35
// -------------------------

using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Test
{
    using Framework;
#if UNITY_EDITOR
    using UnityEditor;
    using UnityEngine.EventSystems;
    using UnityEngine.UI;

    [CustomEditor(typeof(TestUICamera))]
    class TestUICameraInspector : Editor
    {
        TestUICamera my => (TestUICamera)target;

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            //Undo.RecordObject(target, "");
            //Undo.RecordObject(this, "");

            if (my)
            {
                var g = my.GetComponent<Graphic>();
                var cvs = g ? g.canvas : null;
                var cam = cvs ? cvs.worldCamera : null;

                EditorGUILayout.ObjectField("画布", cvs, typeof(Canvas), true);
                EditorGUILayout.EnumPopup("画布渲染模式", cvs.renderMode);
                EditorGUILayout.ObjectField("主相机", Camera.main, typeof(Camera), true);
                EditorGUILayout.ObjectField("当前渲染相机", Camera.current, typeof(Camera), true);
                EditorGUILayout.ObjectField("画布世界相机", cam, typeof(Camera), true);

                EditorGUILayout.Space();
                Camera uiRCam = ExtendUtility.GetCamera(cvs);
                EditorGUILayout.ObjectField("渲染此 ui 的相机", uiRCam, typeof(Camera), true);
            }

        }

    }
#endif
    public class TestUICamera : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {

        }

    }
}