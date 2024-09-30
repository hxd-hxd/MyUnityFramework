// -------------------------
// 创建日期：2022/10/21 18:42:17
// -------------------------

using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
//using System.Diagnostics;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

namespace Framework
{
    /// <summary>
    /// 包含一些扩展的实用方法
    /// </summary>
    public static partial class ExtendUtility
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
            var t = FindOf(obj, name);
            return t ? t.GetComponent<T>() : null;
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
        /// 在所有子节点中，找到名称中包含指定名字的第一个，默认采用模糊搜索，不区分大小写
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="containName"></param>
        /// <returns></returns>
        public static Transform FindOfContainName(this GameObject obj, string containName, bool fuzzySearch = true)
        {
            return FindOfContainName(obj.transform, containName, fuzzySearch);
        }
        /// <summary>
        /// 在所有子节点中，找到名称中包含指定名字的第一个，默认采用模糊搜索，不区分大小写
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="containName"></param>
        /// <returns></returns>
        public static Transform FindOfContainName(this Transform obj, string containName, bool fuzzySearch = true)
        {
            if (!obj) return null;

            var cName = fuzzySearch ? containName.ToLower() : containName;

            foreach (Transform item in obj)
            {
                string name = item.name;
                if (fuzzySearch)
                {
                    name = name.ToLower();
                }
                if (name.Contains(cName))
                {
                    return item;
                }
            }

            Transform target = null;
            foreach (Transform item in obj)
            {
                target = FindOfContainName(item, containName);
                if (target) break;
            }

            return target;
        }
        /// <summary>
        /// 在所有子节点中，找到名称中包含指定名字的第一个对应的组件，默认采用模糊搜索，不区分大小写
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="containName"></param>
        /// <returns></returns>
        public static T FindOfContainName<T>(this Transform obj, string containName, bool fuzzySearch = true) where T : Component
        {
            var t = FindOfContainName(obj, containName, fuzzySearch);
            return t ? t.GetComponent<T>() : null;
        }

        /// <summary>
        /// 期望有此组件，没有则添加
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="transform"></param>
        /// <returns></returns>
        public static T ExpectComponent<T>(this Component self) where T : Component
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

        public static void ResetTransformLocal(this GameObject t) => ResetTransformLocal(t.transform);
        /// <summary>
        /// 重置 <see cref="Transform"/> 本地变换
        /// </summary>
        /// <param name="t"></param>
        public static void ResetTransformLocal(this Transform t)
        {
            t.localEulerAngles = Vector3.zero;
            t.localPosition = Vector3.zero;
            t.localScale = Vector3.one;
        }
        public static void ResetTransformLocal(this GameObject t, Transform parent) => ResetTransformLocal(t.transform, parent);
        public static void ResetTransformLocal(this Transform t, Transform parent)
        {
            t.SetParent(parent);
            ResetTransformLocal(t);
        }


        /// <summary>
        /// 获取宽度，考虑缩放 <see cref="Transform.lossyScale"/> 的影响
        /// </summary>
        /// <param name="self"></param>
        /// <returns></returns>
        public static float GetWidth(this RectTransform self)
        {
            return self ? self.rect.width * self.lossyScale.x : 0;
        }
        /// <summary>
        /// 获取宽度，考虑缩放 <see cref="Transform.lossyScale"/> 的影响
        /// </summary>
        /// <param name="self"></param>
        /// <returns></returns>
        public static float GetHeight(this RectTransform self)
        {
            return self ? self.rect.height * self.lossyScale.y : 0;
        }
        /// <summary>
        /// 获取宽高，考虑缩放 <see cref="Transform.lossyScale"/> 的影响
        /// </summary>
        /// <param name="self"></param>
        /// <returns></returns>
        public static Vector2 GetWidthHeight(this RectTransform self)
        {
            if (!self) return default;
            return new Vector2(self.rect.width * self.lossyScale.x, self.rect.height * self.lossyScale.y);
        }
        /// <summary> 
        /// 获取视觉上的（矩形的）中心世界坐标，该坐标无视 <see cref="RectTransform.pivot"/> 的偏移
        /// </summary>
        /// <param name="self"></param>
        /// <returns></returns>
        public static Vector2 GetVisionCentralPosition(this RectTransform self)
        {
            if (self == null) return default;

            var pos = self.position;// 当前位置
            var pivot = self.pivot;

            /* 计算公式
                中心位置 = 当前位置 + （中心支点 - 当前支点） * （矩形边长）
            */
            var newPos = Vector2.zero;
            newPos.x = pos.x + (0.5f - pivot.x) * self.GetWidth();
            newPos.y = pos.y + (0.5f - pivot.y) * self.GetHeight();

            return newPos;
        }
        /// <summary> 
        /// 获取视觉上的矩形的主要顶点，该坐标无视 <see cref="RectTransform.pivot"/> 的偏移
        /// </summary>
        /// <param name="self"></param>
        /// <returns></returns>
        public static void GetRectPrimaryVertex(this RectTransform self
            , out Vector3 center
            , out Vector3 leftDown
            , out Vector3 leftUp
            , out Vector3 rightDown
            , out Vector3 rightUp
            )
        {
            center = default; leftDown = default; leftUp = default; rightDown = default; rightUp = default;
            if (self == null) return;

            // 无视支点偏移的中点坐标
            center = GetVisionCentralPosition(self);

            // 宽、高的一半
            float wHalf = self.GetWidth() / 2;
            float hHalf = self.GetHeight() / 2;

            // 计算矩形四个顶点位置
            leftDown = new Vector3(
               center.x - wHalf, center.y - hHalf, center.z);
            leftUp = new Vector3(
               center.x - wHalf, center.y + hHalf, center.z);
            rightUp = new Vector3(
               center.x + wHalf, center.y + hHalf, center.z);
            rightDown = new Vector3(
               center.x + wHalf, center.y - hHalf, center.z);
        }

        /// <summary>
        /// 计算中点坐标
        /// </summary>
        /// <param name="target"></param>
        /// <param name="v"></param>
        /// <returns></returns>
        public static Vector2 Midpoint(this Vector2 target, Vector2 v)
        {
            return (target - v) / 2 + v;
        }
        /// <summary>
        /// 计算中点坐标
        /// </summary>
        /// <param name="target"></param>
        /// <param name="v"></param>
        /// <returns></returns>
        public static Vector3 Midpoint(this Vector3 target, Vector3 v)
        {
            return (target - v) / 2 + v;
        }
        /// <summary>
        /// 计算中点坐标
        /// </summary>
        /// <param name="target"></param>
        /// <param name="v"></param>
        /// <returns></returns>
        public static Vector3 Midpoint(this Vector4 target, Vector4 v)
        {
            return (target - v) / 2 + v;
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
            Coroutine coroutine = Coroutines.BeginCoroutine(LoopPlayAnimCoroutine(animator, animCallBack, judge));
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
        /// <param name="pv">进度值(再此回调中返回真实进度值)</param>
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
                else if (canvasGroup.alpha > aTarget)// 目标要变大
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
            Coroutines.BeginCoroutine(UIVanish(canvasGroup, aTarget, vanishDuration, null));
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
            Coroutines.BeginCoroutine(UIVanish(canvasGroup, aTarget, vanishDuration, finish));
        }

        /// <summary>
        /// 获取渲染此图形的相机
        /// </summary>
        /// <param name="g"></param>
        /// <returns></returns>
        public static Camera GetCamera(this Graphic g)
        {
            var cvs = g ? g.canvas : null;
            return GetCamera(cvs);
        }
        /// <summary>
        /// 获取渲染此画布的相机
        /// </summary>
        /// <param name="g"></param>
        /// <returns></returns>
        public static Camera GetCamera(this Canvas cvs)
        {
            Camera uiRCam = null;
            if (cvs)
            {
                switch (cvs.renderMode)
                {
                    case RenderMode.WorldSpace:
                        break;
                    // 
                    case RenderMode.ScreenSpaceOverlay:
                        uiRCam = Camera.main;
                        if (uiRCam == null)
                        {
                            var cs = TypePool.root.GetArray<Camera>(Camera.allCamerasCount);
                            Camera.GetAllCameras(cs);
                            uiRCam = cs[0];
                            TypePool.root.Return(cs);
                        }
                        break;
                    case RenderMode.ScreenSpaceCamera:
                        uiRCam = cvs.worldCamera;
                        if (uiRCam == null)
                        {
                            // 如果 Canvas.worldCamera == null，则实际 Canvas.renderMode 会是 RenderMode.ScreenSpaceOverlay，所以这里不需要处理，因为代码不会运行到这里
                        }
                        break;
                    default:
                        break;
                }
            }
            return uiRCam;
        }

        static IEnumerator DelayCoroutine(float time, System.Action e)
        {
            yield return Yielder.WaitForSeconds(time);
            e?.Invoke();
        }
        static IEnumerator DelayCoroutine(System.Action e)
        {
            yield return null;
            e?.Invoke();
        }
        /// <summary>
        /// 延时处理
        /// </summary>
        /// <param name="time"></param>
        /// <param name="e"></param>
        /// <returns></returns>
        public static Coroutine Delay(this MonoBehaviour instance, float time, System.Action e)
        {
            if (instance == null) return null;
            return instance.StartCoroutine(DelayCoroutine(time, e));
        }
        /// <summary>
        /// 延时一帧处理
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        public static Coroutine Delay(this MonoBehaviour instance, System.Action e)
        {
            if (instance == null) return null;
            return instance.StartCoroutine(DelayCoroutine(e));
        }

        // 省去重复判断的麻烦
        /// <summary>
        /// 获取 <see cref="Component.gameObject"/> 的激活状态
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        public static bool GetActive(Component c)
        {
            return c != null ? c.gameObject.activeSelf : false;
        }
        /// <summary>
        /// 设置 <see cref="Component.gameObject"/> 的激活状态
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        public static void SetActive(Component c, bool value)
        {
            if (c) c.gameObject.SetActive(value);
        }
        /// <summary>
        /// 获取 <see cref="Text.text"/> 文本
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        public static string GetText(Text c)
        {
            return c != null ? c.text : null;
        }
        /// <summary>
        /// 设置 <see cref="Text.text"/> 文本
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        public static void SetText(Text c, string value)
        {
            if (c) c.text = value;
        }
        /// <summary>
        /// 获取 <see cref="InputField.text"/> 文本
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        public static string GetText(InputField c)
        {
            return c != null ? c.text : null;
        }
        /// <summary>
        /// 设置 <see cref="InputField.text"/> 文本
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        public static void SetText(InputField c, string value)
        {
            if (c) c.text = value;
        }

        /// <summary>
        /// 尝试查找
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="vs"></param>
        /// <param name="condition"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public static bool TryFind<T>(this IList<T> vs, Func<T, bool> condition, out T result)
        {
            bool found = false;
            result = default;
            foreach (var v in vs)
            {
                if (condition(v))
                {
                    result = v;
                    found = true;
                    break;
                }
            }
            return found;
        }
        /// <summary>
        /// 尝试索引
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="vs"></param>
        /// <param name="index"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public static bool TryIndex<T>(this IList<T> vs, int index, out T result)
        {
            result = default(T);
            if (index < 0 || index >= vs.Count)
            {
                return false;
            }

            result = vs[index];
            return true;
        }

        public static T ToEnum<T>(this string v) where T : struct, Enum
        {
            Enum.TryParse(v, out T result);
            return result;
        }

        #region 日期时间
        /// <summary>
        /// 转换成 文本格式时间
        /// <para>如：0001/01/01 08:00:00，0001年/01月/01日 08时:00分:00秒</para>
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public static string ToTimeText(this DateTime t)
        {
            return $"{ToDayFrontTimeText(t)} {ToHourBehindTimeText(t)}";
        }
        /// <summary>
        /// 转换成 文本格式时间
        /// <para>如：0001/01/01 08:00:00，0001年/01月/01日 08时:00分:00秒</para>
        /// <para>如：0001-01-01 08:00:00，0001年-01月-01日 08时:00分:00秒</para>
        /// </summary>
        /// <param name="t"></param>
        /// <param name="dateSeparator"></param>
        /// <param name="timeSeparator"></param>
        /// <returns></returns>
        public static string ToTimeText(this DateTime t, string dateSeparator, string timeSeparator)
        {
            return $"{ToDayFrontTimeText(t, dateSeparator)} {ToHourBehindTimeText(t, timeSeparator)}";
        }
        /// <summary>
        /// 转换成 文本格式时间
        /// <para>如：0001/01/01，0001年/01月/01日</para>
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public static string ToDayFrontTimeText(this DateTime t)
        {
            return ToDayFrontTimeText(t, "/");
        }
        /// <summary>
        /// 转换成 文本格式时间
        /// <para>如：0001/01/01，0001年/01月/01日</para>
        /// <para>0001-01-01，0001年-01月-01日</para>
        /// </summary>
        /// <param name="t"></param>
        /// <param name="separator">分隔符</param>
        /// <returns></returns>
        public static string ToDayFrontTimeText(this DateTime t, string separator)
        {
            return $"{TimeSupplement(t.Year, 4)}{separator}{TimeSupplement(t.Month)}{separator}{TimeSupplement(t.Day)}";
        }
        /// <summary>
        /// 转换成 文本格式时间
        /// <para>如：08:00:00，08时:00分:00秒</para>
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public static string ToHourBehindTimeText(this DateTime t)
        {
            return ToHourBehindTimeText(t, ":");
        }
        public static string ToHourBehindTimeText(this DateTime t, string separator)
        {
            return $"{TimeSupplement(t.Hour)}{separator}{TimeSupplement(t.Minute)}{separator}{TimeSupplement(t.Second)}";
        }
        /// <summary>
        /// 时间位补充，如：1年/1月/1日，补充后 0001年/01月/01日
        /// </summary>
        /// <param name="t"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public static string TimeSupplement(int t, int max = 2)
        {
            string ts = t.ToString();
            int num = max - ts.Length;
            if (num > 0)
            {
                ts = $"{new string('0', num)}{ts}";
            }
            return ts;
        }

        #endregion

        #region 文件处理


        /// <summary>
        /// 创建文件时，遇到相同文件的处理方式（文件名和扩展名都相同就是想同文件）
        /// </summary>
        public static FileCreateSameFileHandleMode fileCreateSameFileHandleMode = FileCreateSameFileHandleMode.Skip;

        /// <summary>
        /// 拷贝源文件夹里的所有文件和文件夹 到 目标文件夹
        /// </summary>
        /// <param name="sourceDirName"></param>
        /// <param name="destDirName"></param>
        /// <param name="clearDestDir">清空目标文件夹</param>
        public static void CopyDirInnerAll(string sourceDirName, string destDirName, bool clearDestDir)
        {
            _CopyDirInnerAll(sourceDirName, destDirName, clearDestDir, false);
        }
        /// <summary>
        /// 拷贝源文件夹里的所有文件和文件夹 到 目标文件夹
        /// </summary>
        /// <param name="sourceDirName"></param>
        /// <param name="destDirName"></param>
        /// <param name="clearDestDir">清空目标文件夹</param>
        public static void CopyDirInnerAll_Mobile(string sourceDirName, string destDirName, bool clearDestDir)
        {
            _CopyDirInnerAll(sourceDirName, destDirName, clearDestDir, true);
        }
        /// <summary>
        /// 拷贝源文件夹里的所有文件和文件夹 到 目标文件夹
        /// </summary>
        /// <param name="sourceDirName"></param>
        /// <param name="destDirName"></param>
        /// <param name="clearDestDir">清空目标文件夹</param>
        static void _CopyDirInnerAll(string sourceDirName, string destDirName, bool clearDestDir, bool isMobile)
        {
            if (!Directory.Exists(sourceDirName))
            {
                Debug.LogError($"原文件夹不存在：{sourceDirName}");
                return;
            }
            if (!Directory.Exists(destDirName))
            {
                //Debug.LogError($"目标文件夹不存在：{destDirName}");
                //return;
                Directory.CreateDirectory(destDirName);
            }

            if (clearDestDir)
            {
                ClearDir(destDirName);
            }

            _CopyDirInnerAllDirectly(sourceDirName, destDirName, isMobile);
        }

        /// <summary>
        /// 拷贝源文件夹里的所有文件和文件夹 到 目标文件夹
        /// </summary>
        /// <param name="sourceDirName"></param>
        /// <param name="destDirName"></param>
        /// <param name="clearDestDir">清空目标文件夹</param>
        public static void CopyDirInnerAll(string sourceDirName, string destDirName, bool clearDestDir, FileFiltrateGroup fileFiltrateGroup)
        {
            _CopyDirInnerAll(sourceDirName, destDirName, clearDestDir, fileFiltrateGroup, false);
        }
        /// <summary>
        /// 拷贝源文件夹里的所有文件和文件夹 到 目标文件夹（目标文件夹不能是 <see cref="Application.streamingAssetsPath"/>）
        /// <para>在移动设备中</para>
        /// </summary>
        /// <param name="sourceDirName"></param>
        /// <param name="destDirName"></param>
        /// <param name="clearDestDir">清空目标文件夹</param>
        public static void CopyDirInnerAll_Mobile(string sourceDirName, string destDirName, bool clearDestDir, FileFiltrateGroup fileFiltrateGroup)
        {
            _CopyDirInnerAll(sourceDirName, destDirName, clearDestDir, fileFiltrateGroup, true);
        }
        static void _CopyDirInnerAll(string sourceDirName, string destDirName, bool clearDestDir, FileFiltrateGroup fileFiltrateGroup, bool isMobile)
        {
            if (!Directory.Exists(sourceDirName))
            {
                Debug.LogError($"原文件夹不存在：{sourceDirName}");
                return;
            }
            if (!Directory.Exists(destDirName))
            {
                //Debug.LogError($"目标文件夹不存在：{destDirName}");
                //return;
                Directory.CreateDirectory(destDirName);
            }

            if (clearDestDir)
            {
                ClearDir(destDirName);
            }

            _CopyDirInnerAllDirectly(sourceDirName, destDirName, fileFiltrateGroup, isMobile);
        }

        // 递归拷贝所有文件和文件夹
        static void _CopyDirInnerAllDirectly(string sourceDirName, string destDirName, bool isMobile)
        {
            if (!Directory.Exists(sourceDirName))
            {
                Debug.LogError($"原文件夹不存在：{sourceDirName}");
                return;
            }
            if (!Directory.Exists(destDirName))
            {
                Directory.CreateDirectory(destDirName);
            }

            // 包含的文件
            Action<string, string> _Func_ = CopyDirInnerFile;
            if (isMobile) _Func_ = CopyDirInnerFile_Mobile;
            _Func_(sourceDirName, destDirName);

            // 包含的文件夹
            var dirs = Directory.GetDirectories(sourceDirName);
            foreach (var v in dirs)
            {
                string name = Path.GetFileName(v);
                string result = Path.Combine(destDirName, name);
                Directory.CreateDirectory(result);

                _CopyDirInnerAllDirectly(v, result, isMobile);// 递归
            }
        }
        static void _CopyDirInnerAllDirectly(string sourceDirName, string destDirName, FileFiltrateGroup fileFiltrateGroup, bool isMobile)
        {
            if (!Directory.Exists(sourceDirName))
            {
                Debug.LogError($"原文件夹不存在：{sourceDirName}");
                return;
            }
            if (!Directory.Exists(destDirName))
            {
                Directory.CreateDirectory(destDirName);
            }

            // 包含的文件
            Action<string, string, FileFiltrateGroup> _Func_ = CopyDirInnerFile;
            if (isMobile) _Func_ = CopyDirInnerFile_Mobile;
            _Func_(sourceDirName, destDirName, fileFiltrateGroup);

            // 包含的文件夹
            var dirs = Directory.GetDirectories(sourceDirName);
            foreach (var v in dirs)
            {
                string name = Path.GetFileName(v);
                string result = Path.Combine(destDirName, name);
                Directory.CreateDirectory(result);

                _CopyDirInnerAllDirectly(v, result, fileFiltrateGroup, isMobile);// 递归
            }
        }

        /// <summary>
        /// 拷贝 文件夹里的所有文件 到 目标文件夹
        /// </summary>
        public static void CopyDirInnerFile(string sourceDirName, string destDirName)
        {
            _CopyDirInnerFile(sourceDirName, destDirName, false);
        }
        /// <summary>
        /// 拷贝 文件夹里的所有文件 到 目标文件夹
        /// </summary>
        public static void CopyDirInnerFile_Mobile(string sourceDirName, string destDirName)
        {
            _CopyDirInnerFile(sourceDirName, destDirName, true);
        }
        /// <summary>
        /// 拷贝 文件夹里的所有文件 到 目标文件夹
        /// </summary>
        /// <param name="sourceFileName"></param>
        /// <param name="destDirName"></param>
        static void _CopyDirInnerFile(string sourceDirName, string destDirName, bool isMobile)
        {
            if (!Directory.Exists(sourceDirName))
            {
                Debug.LogError($"原文件夹不存在：{sourceDirName}");
                return;
            }
            if (!Directory.Exists(destDirName))
            {
                Directory.CreateDirectory(destDirName);
            }

            // 拷贝方法
            Action<string, string> _Func_ = CopyFileToDir;
            if (isMobile) _Func_ = CopyFileToDir_Mobile;

            // 拷贝
            var vs = Directory.GetFiles(sourceDirName);
            foreach (var v in vs)
            {
                _Func_(v, destDirName);
            }
        }

        /// <summary>
        /// 拷贝 文件夹里的所有文件 到 目标文件夹
        /// </summary>
        public static void CopyDirInnerFile(string sourceDirName, string destDirName, FileFiltrateGroup fileFiltrateGroup)
        {
            _CopyDirInnerFile(sourceDirName, destDirName, fileFiltrateGroup, false);
        }
        /// <summary>
        /// 拷贝 文件夹里的所有文件 到 目标文件夹
        /// <para>在移动设备中</para>
        /// </summary>
        public static void CopyDirInnerFile_Mobile(string sourceDirName, string destDirName, FileFiltrateGroup fileFiltrateGroup)
        {
            _CopyDirInnerFile(sourceDirName, destDirName, fileFiltrateGroup, true);
        }
        /// <summary>
        /// 拷贝 文件夹里的所有文件 到 目标文件夹
        /// </summary>
        static void _CopyDirInnerFile(string sourceDirName, string destDirName, FileFiltrateGroup fileFiltrateGroup, bool isMobile)
        {
            if (!Directory.Exists(sourceDirName))
            {
                Debug.LogError($"原文件夹不存在：{sourceDirName}");
                return;
            }
            if (!Directory.Exists(destDirName))
            {
                Directory.CreateDirectory(destDirName);
            }

            // 拷贝方法
            Action<string, string> _Func_ = CopyFileToDir;
            if (isMobile) _Func_ = CopyFileToDir_Mobile;

            // 文件筛选信息
            var filesFiltrate = fileFiltrateGroup.fileTypes;
            var filtrateMode = fileFiltrateGroup.fileFiltrateMode;

            // 拷贝
            if (filesFiltrate != null && filesFiltrate.Count > 0)
            {
                var vs = Directory.GetFiles(sourceDirName);
                foreach (var v in vs)
                {
                    string fe = Path.GetExtension(v);// 扩展名
                    bool c = filesFiltrate.Contains(fe);
                    switch (filtrateMode)
                    {
                        case FileFiltrateMode.Confirm:
                            // 包含才拷贝
                            if (c)
                            {
                                _Func_(v, destDirName);
                            }
                            break;
                        case FileFiltrateMode.Ignore:
                            // 不包含才拷贝
                            if (!c)
                            {
                                _Func_(v, destDirName);
                            }
                            break;
                        default:
                            break;
                    }
                }
            }
            else
            {
                _CopyDirInnerFile(sourceDirName, destDirName, fileFiltrateGroup, isMobile);
            }
        }

        /// <summary>
        /// 拷贝 文件 到 目标文件夹
        /// </summary>
        public static void CopyFileToDir(string sourceFileName, string destDirName)
        {
            _CopyFileToDir(sourceFileName, destDirName, false, (destFileName) =>
            {
                File.Copy(sourceFileName, destFileName);
            });
        }
        /// <summary>
        /// 拷贝 文件 到 目标文件夹
        /// <para>在移动设备中</para>
        /// </summary>
        public static void CopyFileToDir_Mobile(string sourceFileName, string destDirName)
        {
            _CopyFileToDir(sourceFileName, destDirName, true, (destFileName) =>
            {
                // 使用 unity api 访问文件
                var req = UnityWebRequest.Get(sourceFileName);
                req.SendWebRequest();
                while (!req.isDone) { }
                //if(!File.Exists(destFileName)) File.Create(destFileName).Close();
                File.WriteAllBytes(destFileName, req.downloadHandler.data);
            });
        }
        static void _CopyFileToDir(string sourceFileName, string destDirName, bool useUnityApi, Action<string> copyFunc)
        {
            if (!File.Exists(sourceFileName))
            {
                Debug.LogError($"源文件夹不存在：{sourceFileName}");
                return;
            }
            if (!Directory.Exists(destDirName))
            {
                //Directory.CreateDirectory(destDirName);
                Debug.LogError($"目标文件夹不存在：{destDirName}");
                return;
            }
            if (copyFunc == null)
            {
                return;
            }

            string fileName = Path.GetFileName(sourceFileName);
            string destFileName = Path.Combine(destDirName, fileName);// 目标文件

            // 目标文件已经存在
            if (File.Exists(destFileName))
            {
                switch (fileCreateSameFileHandleMode)
                {
                    case FileCreateSameFileHandleMode.Replace:
                        File.Delete(destFileName);
                        File.Create(destFileName).Close();
                        break;
                    case FileCreateSameFileHandleMode.Skip:
                    case FileCreateSameFileHandleMode.Contrast:
                        return;
                    default:
                        break;
                }
            }

            copyFunc(destFileName);
        }

        /// <summary>
        /// 获取目录中所有文件
        /// <para>注意文件格式，全部用 "/" </para>
        /// </summary>
        /// <param name="relativePath">true：以相对路径的形式返回</param>
        /// <returns></returns>
        public static List<string> GetFilesAll(string dir, bool relativePath)
        {
            var files = new List<string>();
            dir = dir.Replace("\\", "/");
            if (dir.EndsWith("/"))
            {
                dir = dir.Substring(0, dir.Length - 1);
            }
            _GetFilesAll(dir, relativePath, dir, files);
            return files;
        }
        /// <summary>
        /// 获取目录中所有文件
        /// <para>注意文件格式，全部用 "/" </para>
        /// </summary>
        /// <param name="relativePath">true：以相对路径的形式返回</param>
        /// <returns></returns>
        public static List<string> GetFilesAll(string dir, bool relativePath, FileFiltrateGroup fileFiltrateGroup)
        {
            var files = new List<string>();
            dir = dir.Replace("\\", "/");
            if (dir.EndsWith("/"))
            {
                dir = dir.Substring(0, dir.Length - 1);
            }
            _GetFilesAll(dir, relativePath, dir, fileFiltrateGroup, files);
            return files;
        }

        // relativeDirName：相对路径的名称
        static void _GetFilesAll(string sourceDirName, bool relativePath, string relativeDirName, List<string> files)
        {
            if (!Directory.Exists(sourceDirName))
            {
                Debug.LogError($"原文件夹不存在：{sourceDirName}");
                return;
            }

            // 包含的文件
            var _fs = Directory.GetFiles(sourceDirName).Select((_fName) =>
            {
                _fName = _fName.Replace("\\", "/");
                var r = _fName;

                if (relativePath)
                {
                    if (_fName.StartsWith(relativeDirName))
                    {
                        int length = _fName.Length - relativeDirName.Length - 1;
                        r = _fName.Substring(relativeDirName.Length + 1, length);// 从相对路径之后截取
                    }
                }

                return r;
            }).ToList();

            if (_fs.Count > 0)
                files.AddRange(_fs);

            // 包含的文件夹
            var dirs = Directory.GetDirectories(sourceDirName);
            foreach (var v in dirs)
            {
                _GetFilesAll(v, relativePath, relativeDirName, files);// 递归
            }
        }
        static void _GetFilesAll(string sourceDirName, bool relativePath, string relativeDirName, FileFiltrateGroup fileFiltrateGroup, List<string> files)
        {
            if (!Directory.Exists(sourceDirName))
            {
                Debug.LogError($"原文件夹不存在：{sourceDirName}");
                return;
            }
            // 包含的文件
            var _fs = Directory.GetFiles(sourceDirName).Select((_fName) =>
            {
                _fName = _fName.Replace("\\", "/");
                var r = _fName;

                if (relativePath)
                {
                    if (_fName.StartsWith(relativeDirName))
                    {
                        int length = _fName.Length - relativeDirName.Length - 1;
                        r = _fName.Substring(relativeDirName.Length + 1, length);// 从相对路径之后截取
                    }
                }

                return r;
            }).ToList();

            // 文件筛选信息
            var filesFiltrate = fileFiltrateGroup.fileTypes;
            var filtrateMode = fileFiltrateGroup.fileFiltrateMode;
            if (filesFiltrate != null && filesFiltrate.Count > 0)
            {
                var vs = _fs;
                for (int i = 0; i < vs.Count; i++)
                {
                    var v = vs[i];
                    string fe = Path.GetExtension(v);// 扩展名
                    bool c = filesFiltrate.Contains(fe);
                    switch (filtrateMode)
                    {
                        case FileFiltrateMode.Confirm:
                            // 包含
                            if (!c)
                            {
                                vs.RemoveAt(i);
                                --i;
                            }
                            break;
                        case FileFiltrateMode.Ignore:
                            // 不包含
                            if (c)
                            {
                                vs.RemoveAt(i);
                                --i;
                            }
                            break;
                        default:
                            break;
                    }
                }
            }

            if (_fs.Count > 0)
                files.AddRange(_fs);

            // 包含的文件夹
            var dirs = Directory.GetDirectories(sourceDirName);
            foreach (var v in dirs)
            {
                _GetFilesAll(v, relativePath, relativeDirName, fileFiltrateGroup, files);// 递归
            }
        }

        /// <summary>
        /// 清空文件夹
        /// </summary>
        /// <param name="dir"></param>
        public static void ClearDir(string dir)
        {
            if (!Directory.Exists(dir))
            {
                Debug.LogError($"文件夹不存在：{dir}");
                return;
            }

            ClearDirDir(dir);
            ClearDirFile(dir);
        }
        /// <summary>
        /// 清除文件夹里的子文件夹
        /// </summary>
        /// <param name="dir"></param>
        public static void ClearDirDir(string dir)
        {
            if (!Directory.Exists(dir))
            {
                Debug.LogError($"文件夹不存在：{dir}");
                return;
            }

            var vs = Directory.GetDirectories(dir);
            foreach (var v in vs)
            {
                Directory.Delete(v, true);
            }
        }
        /// <summary>
        /// 清除文件夹里的文件
        /// </summary>
        /// <param name="dir"></param>
        public static void ClearDirFile(string dir)
        {
            if (!Directory.Exists(dir))
            {
                Debug.LogError($"文件夹不存在：{dir}");
                return;
            }

            var vs = Directory.GetFiles(dir);
            foreach (var v in vs)
            {
                File.Delete(v);
            }
        }


        /// <summary>
        /// 获取请求完成的 <see cref="UnityWebRequest"/>
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static UnityWebRequest GetUnityWebRequestDone(string path)
        {
            var req = UnityWebRequest.Get(path);
            req.SendWebRequest();
            while (!req.isDone) { }
            return req;
        }
        public static List<string> UnityApi_GetDirs(string path)
        {
            //Directory.GetDirectories(path);
            var dirs = TypePool.root.GetList<string>();
            var req = GetUnityWebRequestDone(path);
            if (req.result == UnityWebRequest.Result.Success)
            {

            }
            else
            {
                Debug.LogError($"请求失败：{path}");
            }
            return dirs;
        }
        #endregion


        /// <summary>
        /// 文件筛选处理组
        /// </summary>
        public struct FileFiltrateGroup
        {
            /// <summary>
            /// 文件类型列表，格式：".txt"
            /// </summary>
            public List<string> fileTypes;
            public FileFiltrateMode fileFiltrateMode;

            public FileFiltrateGroup(FileFiltrateMode fileFiltrateMode) : this()
            {
                fileTypes = new List<string>(2);
                this.fileFiltrateMode = fileFiltrateMode;
            }

            public FileFiltrateGroup(List<string> fileType, FileFiltrateMode fileFiltrateMode)
            {
                this.fileTypes = fileType;
                this.fileFiltrateMode = fileFiltrateMode;
            }
        }

        /// <summary>
        /// 文件筛选处理方式
        /// </summary>
        public enum FileFiltrateMode
        {
            /// <summary>
            /// 确定
            /// </summary>
            Confirm,
            /// <summary>
            /// 忽略
            /// </summary>
            Ignore,
        }
        /// <summary>
        /// 创建文件时，遇到相同文件的处理方式（文件名和扩展名都相同就是想同文件）
        /// </summary>
        public enum FileCreateSameFileHandleMode
        {
            /// <summary>替换掉同名文件</summary>
            Replace,
            /// <summary>跳过不创建</summary>
            Skip,
            /// <summary>比较文件内容差异，有差异则替换</summary>
            /// <remarks>暂未实现，默认跳过</remarks>
            Contrast,
        }
    }
}