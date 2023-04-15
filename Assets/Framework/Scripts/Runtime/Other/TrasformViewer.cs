// -------------------------
// 创建日期：2023/4/11 15:27:24
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
    using UnityEngine.UIElements;

    [CustomEditor(typeof(TrasformViewer))]
    class TrasformViewerInspector : Editor
    {
        Vector3 position;
        Vector3 eulerAngles;

        Vector3 localPosition;
        Vector3 localEulerAngles;
        Vector3 localScale;

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            var my = (TrasformViewer)target;

            position = my.transform.position;
            eulerAngles = my.transform.eulerAngles;
            localPosition = my.transform.localPosition;
            localEulerAngles = my.transform.localEulerAngles;
            localScale = my.transform.localScale;

            GUILayout.Space(8);
            GUILayout.Label("Transform 属性", "BoldLabel");

            EditorGUI.BeginChangeCheck();
            position = EditorGUILayout.Vector3Field("Position", position);
            if(EditorGUI.EndChangeCheck()) my.transform.position = position;

            EditorGUI.BeginChangeCheck();
            eulerAngles = EditorGUILayout.Vector3Field("Rotation", my.transform.eulerAngles);
            if(EditorGUI.EndChangeCheck()) my.transform.eulerAngles = eulerAngles;

            GUI.enabled = false;
            EditorGUILayout.Vector3Field("LossyScale", my.transform.lossyScale);
            GUI.enabled = true;


            GUILayout.Space(8);
            GUILayout.Label("本地属性", "BoldLabel");

            EditorGUI.BeginChangeCheck();
            localPosition = EditorGUILayout.Vector3Field("Position", localPosition);
            if(EditorGUI.EndChangeCheck()) my.transform.localPosition = localPosition;

            EditorGUI.BeginChangeCheck();
            localEulerAngles = EditorGUILayout.Vector3Field("Rotation", localEulerAngles) ;
            if(EditorGUI.EndChangeCheck()) my.transform.localEulerAngles = localEulerAngles;

            EditorGUI.BeginChangeCheck();
            localScale = EditorGUILayout.Vector3Field("Scale", localScale);
            if(EditorGUI.EndChangeCheck()) my.transform.localScale = localScale;
        }
    }
#endif

    public class TrasformViewer : MonoBehaviour
    {

    }
}