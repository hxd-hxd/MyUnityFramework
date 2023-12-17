using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Prob
{

    [CustomEditor(typeof(TestProb))]
    public class TestProbInspector : Editor
    {
        TestProb testProb;
        IProb probItem;
        string CompleteNode;
        static string tValue;
        float probValue;

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            EditorGUI.BeginDisabledGroup(!Application.isPlaying);
            {
                testProb = (TestProb)target;
                serializedObject.Update();

                EditorGUILayout.BeginVertical("box");
                {
                    EditorGUILayout.LabelField("获取概率项信息（调用 FindProbItem(string)）");

                    EditorGUILayout.BeginHorizontal();
                    {
                        EditorGUILayout.Space(10, false);
                        tValue = EditorGUILayout.TextField(tValue);

                        //probItem = testProb.pdd.FindProbItem(tValue);
                        ////probValue = testProb.pdd.GetTypesRealProb(tValue);
                        //probValue = probItem == null ? 0 : probItem.RealProb();

                        if (GUILayout.Button("获取信息", GUILayout.MinWidth(100)))
                        {

                            probItem = testProb.pdd.FindProbItem(tValue);

                            probValue = probItem == null ? 0 : probItem.RealProb();
                        }

                    }
                    EditorGUILayout.EndHorizontal();

                    EditorGUILayout.LabelField("（完整节点 调用 CompleteNode()）");
                    EditorGUILayout.BeginHorizontal();
                    {
                        EditorGUILayout.Space(25, false);
                        EditorGUILayout.LabelField("完整节点", "BoldLabel", GUILayout.MaxWidth(100));
                        EditorGUILayout.LabelField(probItem == null ? "" : probItem.CompleteNode());

                    }
                    EditorGUILayout.EndHorizontal();

                    EditorGUILayout.LabelField("（真实概率 调用 IProb.RealProb()）");
                    EditorGUILayout.BeginHorizontal();
                    {
                        EditorGUILayout.Space(25, false);
                        EditorGUILayout.LabelField("真实概率", GUILayout.MaxWidth(100));
                        EditorGUILayout.LabelField(probValue.ToString() + "%");

                    }
                    EditorGUILayout.EndHorizontal();
                }
                EditorGUILayout.EndVertical();

                EditorGUILayout.LabelField("此处随机 调用的是 GetSonInclude()");
                EditorGUILayout.BeginHorizontal();
                {
                    if (GUILayout.Button("随机"))
                    {
                        Debug.Log(testProb.pdd.GetSonInclude());
                    }
                    if (GUILayout.Button("随机 100 次"))
                    {
                        for (int i = 0; i < 100; i++)
                        {
                            Debug.Log(testProb.pdd.GetSonInclude());
                        }
                    }
                }
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.Space();
                EditorGUILayout.BeginHorizontal();
                {
                    if (GUILayout.Button("保存成 json "))
                    {
                        testProb.SaveJson();
                        AssetDatabase.Refresh();
                    }
                    if (GUILayout.Button("读取 json "))
                    {
                        testProb.ReadJson();
                    }
                    if (GUILayout.Button("清除"))
                    {
                        testProb.Clear();
                    }
                }
                EditorGUILayout.EndHorizontal();
            }
            EditorGUI.EndDisabledGroup();
        }

    }
}