// -------------------------
// 创建日期：2022/10/21 18:42:17
// -------------------------

using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

/// <summary>
/// 包含一些扩展的操作方法
/// </summary>
public static class ExtendUtility
{
    /// <summary>
    /// 在所有子节点中找到指定名称的第一个
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="name"></param>
    /// <returns></returns>
    public static Transform FindOf(this Transform obj, string name)
    {
        Transform target = obj.Find(name);

        if (!target)
        {
            for (int i = 0; i < obj.childCount; i++)
            {
                target = FindOf(obj.GetChild(i), name);
                if (target) return target;
            }
        }

        return target;
    }
    /// <summary>
    /// 尝试在所有子节点中找到指定名称的第一个
    /// </summary>
    /// <returns></returns>
    public static bool TryFindOf(this Transform obj, string name, out Transform result)
    {
        result = obj.FindOf(name);

        return result;
    }
    /// <summary>
    /// 在所有子节点中找到指定名称的第一个，并获取其上指定组件
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="name"></param>
    /// <returns></returns>
    public static T FindOf<T>(this Transform obj, string name) where T : Component
    {
        return FindOf(obj, name)?.GetComponent<T>();
    }
    /// <summary>
    /// 在所有子节点中找到指定名称的第一个，并获取其上指定组件
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="obj"></param>
    /// <param name="name"></param>
    /// <param name="result"></param>
    /// <returns></returns>
    public static bool TryFindOf<T>(this Transform obj, string name, out T result) where T : Component
    {
        result = obj.FindOf<T>(name);

        return result;
    }
    /// <summary>
    /// 在所有子节点中找到指定名称的第一个
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="name"></param>
    /// <returns></returns>
    public static Transform FindOf(this GameObject obj, string name)
    {
        return FindOf(obj.transform, name);
    }
    /// <summary>
    /// 尝试在所有子节点中找到指定名称的第一个
    /// </summary>
    /// <returns></returns>
    public static bool TryFindOf(this GameObject obj, string name, out Transform result)
    {
        result = obj.FindOf(name);

        return result;
    }

    /// <summary>
    /// 在所有子节点中，找到名称中包含指定名字的第一个
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="containName"></param>
    /// <returns></returns>
    public static Transform FindOfContainName(this Transform obj, string containName)
    {
        Transform target = null;
        foreach (Transform item in obj)
        {
            if (item.name.Contains(containName))
            {
                return item;
            }
            else
            {
                target = FindOfContainName(obj, containName);
            }
        }

        return target;
    }

    /// <summary>
    /// 期望有此组件，没有则添加
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="transform"></param>
    /// <returns></returns>
    public static T ExpectComponent<T>(this Transform self) where T : Component
    {
        return ExpectComponent<T>(self.gameObject);
    }

    /// <summary>
    /// 期望有此组件，没有则添加
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="transform"></param>
    /// <returns></returns>
    public static T ExpectComponent<T>(this GameObject self) where T : Component
    {
        //T cp = self.GetComponent<T>() ?? self.AddComponent<T>();
        T cp = self.GetComponent<T>();
        if (!cp)
        {
            cp = self.AddComponent<T>();
        }
        return cp;
    }

    /// <summary>
    /// 转为 Texture2D
    /// </summary>
    /// <param name="sprite"></param>
    /// <returns></returns>
    public static Texture2D ToTexture2D(this Sprite sprite)
    {
        var targetTex = new Texture2D((int)sprite.rect.width, (int)sprite.rect.height);
        var pixels = sprite.texture.GetPixels(
            (int)sprite.textureRect.x,
            (int)sprite.textureRect.y,
            (int)sprite.textureRect.width,
            (int)sprite.textureRect.height);
        targetTex.SetPixels(pixels);
        targetTex.Apply();
        return targetTex;
    }
    /// <summary>
    /// 转为 Sprite
    /// </summary>
    /// <param name="t2d"></param>
    /// <returns></returns>
    public static Sprite ToSprite(this Texture2D t2d)
    {
        Sprite s = Sprite.Create(t2d, new Rect(0, 0, t2d.width, t2d.height), Vector2.zero);
        return s;
    }

    /// <summary>
    /// 循环播放动画
    /// </summary>
    /// <param name="animator"></param>
    /// <param name="animCallBack"></param>
    /// <param name="judge">自定义判断</param>
    /// <returns>Coroutine</returns>
    public static Coroutine LoopPlayAnim(this Animator animator, Action animCallBack, Func<bool> judge)
    {
        Coroutine coroutine = Coroutines.Instance.StartCoroutine(LoopPlayAnimCoroutine(animator, animCallBack, judge));
        return coroutine;
    }
    /// <summary>
    /// 循环播放动画
    /// </summary>
    /// <param name="animator"></param>
    /// <param name="animCallBack"></param>
    /// <param name="judge">自定义判断</param>
    /// <returns></returns>
    public static IEnumerator LoopPlayAnimCoroutine(this Animator animator, Action animCallBack, Func<bool> judge)
    {
        if (!animator)
        {
            yield break;
        }

        while (true)
        {
            if (!animator) yield break;
            yield return null;
            if (!animator) yield break;

            AnimatorStateInfo info = animator.GetCurrentAnimatorStateInfo(0);
            if (info.normalizedTime >= 1.0f && judge())
            {
                animCallBack?.Invoke();
            }
        }
    }


    /// <summary>
    /// 计算场景加载时刷新的进度条
    /// </summary>
    /// <param name="operation">场景加载数据</param>
    /// <param name="minProgressTime"></param>
    /// <param name="progressAcion">更新进度时的回调</param>
    /// <param name="progressFinish">更新进度完成的回调</param>
    /// <returns></returns>
    public static IEnumerator Progress(AsyncOperation operation, float minProgressTime, Action<float> progressAcion, Action progressFinish)
    {
        operation.allowSceneActivation = false;// 使场景加载完成后不启动

        float progressTime = 0f;
        float progress = 0f;

        while (operation.progress < 0.9f && !operation.isDone)// 进度加载完成后是 0.9 ，而不是 1 
        {
            progress = operation.progress;
            progressTime += Time.deltaTime;
            if (progressTime < minProgressTime)
            {
                progress = progressTime / (operation.progress + minProgressTime);
            }

            progressAcion?.Invoke(progress);
            yield return null;
        }

        // 补全进度条
        float currentVelocity = 0;
        while (progress <= 0.99f && progressTime < minProgressTime)
        {
            progress = Mathf.SmoothDampAngle(progress, 1, ref currentVelocity, minProgressTime - progressTime);
            progressAcion?.Invoke(progress);
            yield return null;
        }

        // 进度完成
        progress = 1;
        progressAcion?.Invoke(progress);
        operation.allowSceneActivation = true;

        yield return null;

        // 进度条完成
        progressFinish?.Invoke();

        // 界面消失
        //yield return UIVanish();
    }


    /// <summary>
    /// 计算场景加载时刷新的进度条
    /// </summary>
    /// <param name="pv">进度值(再次回调中返回真实进度值)</param>
    /// <param name="minProgressTime">最小的进度持续时间（不会小于真实进度）</param>
    /// <param name="progressAcion">更新进度时的回调</param>
    /// <param name="progressFinish">更新进度完成的回调</param>
    /// <returns></returns>
    public static IEnumerator Progress(Func<float> pv, float minProgressTime, Action<float> progressAcion, Action progressFinish)
    {
        float progressTime = 0f;
        float progress = 0f;

        while (pv() < 0.9f)// 进度加载完成后是 0.9 ，而不是 1 
        {
            progress = pv();
            progressTime += Time.deltaTime;
            if (progressTime < minProgressTime)
            {
                progress = progressTime / (pv() + minProgressTime);
            }

            progressAcion?.Invoke(progress);
            yield return null;
        }

        // 补全进度条
        float currentVelocity = 0;
        while (progress <= 0.99f && progressTime < minProgressTime)
        {
            progress = Mathf.SmoothDampAngle(progress, 1, ref currentVelocity, minProgressTime - progressTime);
            progressAcion?.Invoke(progress);
            yield return null;
        }

        // 进度完成
        progress = 1;
        progressAcion?.Invoke(progress);

        yield return null;

        // 进度条完成
        progressFinish?.Invoke();

        // 界面消失
        //yield return UIVanish();
    }

    /// <summary>
    /// UI 整体透明度变化
    /// </summary>
    /// <param name="canvasGroup">要变化的 <see cref="CanvasGroup"/></param>
    /// <param name="aTarget">透明度变化到的目标值 <see cref="CanvasGroup.alpha"/></param>
    /// <param name="vanishDuration">变化的持续时间</param>
    /// <param name="finish">完成时回调</param>
    /// <returns></returns>
    public static IEnumerator UIVanish(this CanvasGroup canvasGroup, float aTarget, float vanishDuration)
    {
        yield return UIVanish(canvasGroup, aTarget, vanishDuration, null);
    }
    /// <summary>
    /// UI 整体透明度变化
    /// </summary>
    /// <param name="canvasGroup">要变化的 <see cref="CanvasGroup"/></param>
    /// <param name="aTarget">透明度变化到的目标值 <see cref="CanvasGroup.alpha"/></param>
    /// <param name="vanishDuration">变化的持续时间</param>
    /// <param name="finish">完成时回调</param>
    /// <returns></returns>
    public static IEnumerator UIVanish(this CanvasGroup canvasGroup, float aTarget, float vanishDuration, Action finish)
    {
        float currentVelocity = 0;

        float approachValue = 0.01f;// 近似的阈值

        // 执行条件
        Func<bool> actionCondition = () =>
        {
            if (canvasGroup.alpha > aTarget)// 目标要变小
            {
                return canvasGroup.alpha >= aTarget + approachValue;
            }
            else if(canvasGroup.alpha > aTarget)// 目标要变大
            {
                return canvasGroup.alpha <= aTarget - approachValue;
            }
            return false;
        };

        while (actionCondition())
        {
            canvasGroup.alpha = Mathf.SmoothDampAngle(canvasGroup.alpha, aTarget, ref currentVelocity, vanishDuration);
            yield return null;
        }

        canvasGroup.alpha = aTarget;

        finish?.Invoke();

        //Destroy(gameObject);
    }

    /// <summary>
    /// UI 整体透明度变化
    /// </summary>
    /// <param name="canvasGroup">要变化的 <see cref="CanvasGroup"/></param>
    /// <param name="aTarget">透明度变化到的目标值 <see cref="CanvasGroup.alpha"/></param>
    /// <param name="vanishDuration">变化的持续时间</param>
    /// <returns></returns>
    public static void CanvasGroupVanish(this CanvasGroup canvasGroup, float aTarget, float vanishDuration)
    {
        Coroutines.Instance.StartCoroutine(UIVanish(canvasGroup, aTarget, vanishDuration, null));
    }
    /// <summary>
    /// UI 整体透明度变化
    /// </summary>
    /// <param name="canvasGroup">要变化的 <see cref="CanvasGroup"/></param>
    /// <param name="aTarget">透明度变化到的目标值 <see cref="CanvasGroup.alpha"/></param>
    /// <param name="vanishDuration">变化的持续时间</param>
    /// <param name="finish">完成时回调</param>
    /// <returns></returns>
    public static void CanvasGroupVanish(this CanvasGroup canvasGroup, float aTarget, float vanishDuration, Action finish)
    {
        Coroutines.Instance.StartCoroutine(UIVanish(canvasGroup, aTarget, vanishDuration, finish));
    }
}
