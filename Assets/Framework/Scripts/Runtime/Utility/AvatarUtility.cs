using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public static class AvatarUtility
{

    /// <summary>
	/// Only for merge materials.
	/// �����ںϲ����ϡ�
	/// </summary>
	public static int combine_texture_max = 512;
    public static string combine_diffuse_texture = "_MainTex";
    public static string combine_default_shader = "Mobile/Diffuse";

    /// <summary>
    /// �� SkinnedMeshRenderers �����һ�𲢹���һ���Ǽܡ�
    /// �ϲ����ʻ���ٵ��ô����������������ڴ�Ĵ�С��
    /// </summary>
    /// <param name="skeleton">combine meshes to this skeleton(a gameobject)</param>
    /// <param name="meshes">meshes need to be merged</param>
    /// <param name="combine">merge materials or not</param>
	public static void CombineSkinObject(GameObject skeleton, SkinnedMeshRenderer[] meshes, bool combine = false)
    {

        // Fetch all bones of the skeleton
        List<Transform> transforms = new List<Transform>();
        transforms.AddRange(skeleton.GetComponentsInChildren<Transform>(true));

        List<Material> materials = new List<Material>();                        //the list of materials
        List<CombineInstance> combineInstances = new List<CombineInstance>();   //the list of meshes
        List<Transform> bones = new List<Transform>();                          //the list of bones

        // Below informations only are used for merge materilas(bool combine = true)
        List<Vector2[]> oldUV = null;
        Material newMaterial = null;
        Texture2D newDiffuseTex = null;

        // Collect information from meshes
        for (int i = 0; i < meshes.Length; i++)
        {
            SkinnedMeshRenderer smr = meshes[i];
            materials.AddRange(smr.materials); // Collect materials

            // Collect meshes
            for (int sub = 0; sub < smr.sharedMesh.subMeshCount; sub++)
            {
                CombineInstance ci = new CombineInstance();
                ci.mesh = smr.sharedMesh;
                ci.subMeshIndex = sub;
                combineInstances.Add(ci);
            }

            // Collect bones
            for (int j = 0; j < smr.bones.Length; j++)
            {

                for (int tBase = 0; tBase < transforms.Count; tBase++)
                {
                    if (smr.bones[j].name.Equals(transforms[tBase].name))
                    {
                        bones.Add(transforms[tBase]);
                        break;
                    }
                }
            }
        }

        // merge materials
        // �ϲ�����
        if (combine)
        {
            newMaterial = new Material(Shader.Find(combine_default_shader));
            oldUV = new List<Vector2[]>();
            // merge the texture
            // �ϲ���ͼ
            List<Texture2D> Textures = new List<Texture2D>();
            for (int i = 0; i < materials.Count; i++)
            {
                Textures.Add(materials[i].GetTexture(combine_diffuse_texture) as Texture2D);
            }

            newDiffuseTex = new Texture2D(combine_texture_max, combine_texture_max, TextureFormat.RGBA32, true);
            Rect[] uvs = newDiffuseTex.PackTextures(Textures.ToArray(), 0);
            newMaterial.mainTexture = newDiffuseTex;

            // reset uv
            // ���� uv 
            Vector2[] uva, uvb;
            for (int j = 0; j < combineInstances.Count; j++)
            {
                uva = combineInstances[j].mesh.uv;
                uvb = new Vector2[uva.Length];
                for (int k = 0; k < uva.Length; k++)
                {
                    uvb[k] = new Vector2((uva[k].x * uvs[j].width) + uvs[j].x, (uva[k].y * uvs[j].height) + uvs[j].y);// ����ϲ���� uv λ�ã�����ƫ�Ƽ��㣩
                }
                oldUV.Add(combineInstances[j].mesh.uv); // ��¼ԭ uv 
                combineInstances[j].mesh.uv = uvb;      // Ӧ�ü����� uv 
            }
        }

        // ����һ���µ� SkinnedMeshRenderer
        SkinnedMeshRenderer oldSKinned = skeleton.GetComponent<SkinnedMeshRenderer>();
        if (oldSKinned != null)
        {
            GameObject.DestroyImmediate(oldSKinned);
        }
        SkinnedMeshRenderer r = skeleton.AddComponent<SkinnedMeshRenderer>();
        r.sharedMesh = new Mesh();
        r.sharedMesh.name = "merge";
        r.sharedMesh.CombineMeshes(combineInstances.ToArray(), combine, false);// Combine meshes
        r.bones = bones.ToArray();// Use new bones

        if (combine)
        {
            r.material = newMaterial;
            for (int i = 0; i < combineInstances.Count; i++)
            {
                combineInstances[i].mesh.uv = oldUV[i];
            }
        }
        else
        {
            r.materials = materials.ToArray();
        }
    }


    /// <summary>
    /// ��Ŀ�깲�����
    /// <para>�л��·��Ȱ󶨹�����ģ�ͣ�����ģ�͹����󶨣���ͬ������</para>
    /// </summary>
    /// <param name="selfSkin"></param>
    /// <param name="target"></param>
    public static void ShareSkeletonInstanceWith(SkinnedMeshRenderer selfSkin, GameObject target)
    {
        Transform[] newBones = new Transform[selfSkin.bones.Length];
        for (int i = 0; i < selfSkin.bones.GetLength(0); ++i)
        {
            GameObject bone = selfSkin.bones[i].gameObject;

            // Ŀ���SkinnedMeshRenderer.bones�����ֻ��Ŀ��mesh��صĹ���,Ҫ���Ŀ��ȫ������,����ͨ�����ҵķ�ʽ.
            newBones[i] = FindChildRecursion(target.transform, bone.name);
        }

        selfSkin.bones = newBones;
    }

    // �ݹ����
    public static Transform FindChildRecursion(Transform t, string name)
    {
        foreach (Transform child in t)
        {
            if (child.name == name)
            {
                return child;
            }
            else
            {
                Transform ret = FindChildRecursion(child, name);
                if (ret != null)
                    return ret;
            }
        }

        return null;
    }
}
