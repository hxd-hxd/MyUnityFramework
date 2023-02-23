using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public static class AvatarUtility
{

    /// <summary>
	/// Only for merge materials.
	/// 仅用于合并材料。
	/// </summary>
	public static int combine_texture_max = 512;
    public static string combine_diffuse_texture = "_MainTex";
    public static string combine_default_shader = "Mobile/Diffuse";

    /// <summary>
    /// 将 SkinnedMeshRenderers 组合在一起并共享一个骨架。
    /// 合并材质会减少调用次数，但它会增加内存的大小。
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
        // 合并材质
        if (combine)
        {
            newMaterial = new Material(Shader.Find(combine_default_shader));
            oldUV = new List<Vector2[]>();
            // merge the texture
            // 合并贴图
            List<Texture2D> Textures = new List<Texture2D>();
            for (int i = 0; i < materials.Count; i++)
            {
                Textures.Add(materials[i].GetTexture(combine_diffuse_texture) as Texture2D);
            }

            newDiffuseTex = new Texture2D(combine_texture_max, combine_texture_max, TextureFormat.RGBA32, true);
            Rect[] uvs = newDiffuseTex.PackTextures(Textures.ToArray(), 0);
            newMaterial.mainTexture = newDiffuseTex;

            // reset uv
            // 重置 uv 
            Vector2[] uva, uvb;
            for (int j = 0; j < combineInstances.Count; j++)
            {
                uva = combineInstances[j].mesh.uv;
                uvb = new Vector2[uva.Length];
                for (int k = 0; k < uva.Length; k++)
                {
                    uvb[k] = new Vector2((uva[k].x * uvs[j].width) + uvs[j].x, (uva[k].y * uvs[j].height) + uvs[j].y);// 计算合并后的 uv 位置（根据偏移计算）
                }
                oldUV.Add(combineInstances[j].mesh.uv); // 记录原 uv 
                combineInstances[j].mesh.uv = uvb;      // 应用计算后的 uv 
            }
        }

        // 创建一个新的 SkinnedMeshRenderer
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
    /// 与目标共享骨骼
    /// <para>切换衣服等绑定骨骼的模型，与主模型骨骼绑定，已同步动画</para>
    /// </summary>
    /// <param name="selfSkin"></param>
    /// <param name="target"></param>
    public static void ShareSkeletonInstanceWith(SkinnedMeshRenderer selfSkin, GameObject target)
    {
        Transform[] newBones = new Transform[selfSkin.bones.Length];
        for (int i = 0; i < selfSkin.bones.GetLength(0); ++i)
        {
            GameObject bone = selfSkin.bones[i].gameObject;

            // 目标的SkinnedMeshRenderer.bones保存的只是目标mesh相关的骨骼,要获得目标全部骨骼,可以通过查找的方式.
            newBones[i] = FindChildRecursion(target.transform, bone.name);
        }

        selfSkin.bones = newBones;
    }

    // 递归查找
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
