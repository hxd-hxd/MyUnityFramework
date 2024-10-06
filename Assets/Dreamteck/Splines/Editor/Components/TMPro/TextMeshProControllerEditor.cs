namespace Dreamteck.Splines.Editor
{
    using UnityEngine;
    using System.Collections;
    using UnityEditor;
    using Dreamteck.Splines;

    [CustomEditor(typeof(TextMeshProController))]
    [CanEditMultipleObjects]
    public class TextMeshProControllerEditor : SplineUserEditor
    {

        protected override void BodyGUI()
        {
            base.BodyGUI();
            serializedObject.Update();
            SerializedProperty tmpro = serializedObject.FindProperty("_tmpro");
            SerializedProperty bendMode = serializedObject.FindProperty("_bendMode");
            SerializedProperty stretchMode = serializedObject.FindProperty("_stretchMode");
            SerializedProperty rotation = serializedObject.FindProperty("_rotation");
            SerializedProperty offset = serializedObject.FindProperty("_offset");


            EditorGUI.BeginChangeCheck();

            EditorGUILayout.PropertyField(tmpro, new GUIContent("Text Mesh"));
            EditorGUILayout.PropertyField(bendMode);
            EditorGUILayout.PropertyField(stretchMode);
            EditorGUILayout.PropertyField(rotation);
            EditorGUILayout.PropertyField(offset);


            if (EditorGUI.EndChangeCheck())
            {
                serializedObject.ApplyModifiedProperties();
                foreach(var user in users)
                {
                    user.Rebuild();
                }
            }
            
        }

        private void CustomRuleUI(ObjectControllerCustomRuleBase customRule)
        {
            SerializedObject serializedRule = new SerializedObject(customRule);
            SerializedProperty property = serializedRule.GetIterator();
            property.NextVisible(true);
            property.NextVisible(false);
            EditorGUI.BeginChangeCheck();
            do
            {
                EditorGUILayout.PropertyField(property);
            } while (property.NextVisible(false));
            if (EditorGUI.EndChangeCheck())
            {
                serializedRule.ApplyModifiedProperties();
            }
        }

    }


}
