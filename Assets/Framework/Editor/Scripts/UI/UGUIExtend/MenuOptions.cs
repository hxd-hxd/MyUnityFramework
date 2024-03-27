// -------------------------
// 创建日期：2023/4/10 18:46:20
// -------------------------

using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEditor;
using Object = UnityEngine.Object;

namespace Framework.Editor
{
    public class MenuOptions
    {
        [MenuItem("GameObject/UI/Dropdown Pro - Text", priority = 2036)]
        static void AddDropdown(MenuCommand menuCommand)
        {
            var go = CreateUI.Create("DropdownPro Text.prefab");
            Selection.activeGameObject = go;
        }

        //[MenuItem("GameObject/UI/TestSphere", priority = 2036)]
        //static void TestSphere(MenuCommand menuCommand)
        //{
        //    Object obj = Selection.objects.Length > 0 ? Selection.objects[0] : null;

        //    //var dpPrefab = EditorResources.Load<GameObject>($"{RootPath}DropdownPro Text");
        //    //var dpPrefab = EditorGUIUtility.Load($"Sphere.prefab");
        //    var dpPrefab = EditorGUIUtility.Load($"Sphere");
        //    //var dpPrefab = AssetDatabase.LoadAssetAtPath<GameObject>($"{FullRootPath}DropdownPro Text.prefab");
        //    var dp = (GameObject)Object.Instantiate(dpPrefab);

        //    if (obj == null)
        //    {

        //    }
        //    else
        //    {
        //        if (obj is GameObject go)
        //        {
        //            dp.transform.SetParent(go.transform, false);
        //        }
        //    }

        //}

        //[MenuItem("Examples/Load Editor Texture Example")]
        static void loadExample()
        {
            Texture tex = (Texture)EditorGUIUtility.Load("aboutwindow.mainheader");
            Debug.Log("Got: " + tex.name + " !");

            MeshFilter mf = null;
            Renderer r = null;
            //GameObject go = GameObject.Find("Cube") ?? new GameObject("Cube");
            GameObject go = GameObject.Find("Cube") ?? ObjectFactory.CreatePrimitive(PrimitiveType.Cube);

            mf = go.ExpectComponent<MeshFilter>();
            r = go.ExpectComponent<MeshRenderer>();
            if (!mf.sharedMesh)
            {
                //mf.mesh = new Mesh();
                CreateCube(mf.sharedMesh = new Mesh());
                mf.mesh = mf.sharedMesh;
            }
            r.material = r.material ?? new Material(Shader.Find("Standard"));
            r.material.mainTexture = tex;

            //go.GetComponent<Renderer>().sharedMaterial.mainTexture = null;

            Selection.activeGameObject = go;
        }
        public static void CreateCube(Mesh mesh)
        {
            Vector3[] vertices = {
                new Vector3 (0, 0, 0),
                new Vector3 (1, 0, 0),
                new Vector3 (1, 1, 0),
                new Vector3 (0, 1, 0),
                new Vector3 (0, 1, 1),
                new Vector3 (1, 1, 1),
                new Vector3 (1, 0, 1),
                new Vector3 (0, 0, 1),
            };

            int[] triangles = {
                0, 2, 1, //face front
			    0, 3, 2,
                2, 3, 4, //face top
			    2, 4, 5,
                1, 2, 5, //face right
			    1, 5, 6,
                0, 7, 4, //face left
			    0, 4, 3,
                5, 4, 7, //face back
			    5, 7, 6,
                0, 6, 7, //face bottom
			    0, 1, 6
            };

            mesh.Clear();
            mesh.vertices = vertices;
            mesh.triangles = triangles;
            mesh.Optimize();
            mesh.RecalculateNormals();
        }
    }
}