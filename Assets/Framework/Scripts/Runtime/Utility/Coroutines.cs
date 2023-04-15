using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 协程
/// <para>注意：不要直接调用 <see cref="MonoBehaviour.StopAllCoroutines"/></para>
/// </summary>
[DisallowMultipleComponent]
public class Coroutines : MonoBehaviour
{

    static Coroutines instance;
    public static Coroutines Instance
    {
        get => instance;
    }

    [SerializeField]
    private int useNum;

    static Dictionary<string, Coroutine> coroutineDic = new Dictionary<string, Coroutine>();

    /// <summary>
    /// 协程使用的次数
    /// </summary>
    public int UseNum
    {
        get => useNum;
        private set => useNum = value;
    }


    [RuntimeInitializeOnLoadMethod]
    static void OnRuntimeMethodLoad()
    {
        GameObject go = new GameObject();
        go.name = "Coroutines";
        go.AddComponent<Coroutines>();
    }

    private void Awake()
    {
        instance = this;
        DontDestroyOnLoad(this);
    }

    /// <summary>
    /// 开启协程
    /// </summary>
    /// <param name="coroutine">要开启的协程</param>
    /// <returns></returns>
    public Coroutine BeginCoroutine(IEnumerator coroutine)
    {
        Coroutine cor = StartCoroutine(coroutine);
        UseNum++;
        return cor;
    }

    /// <summary>
    /// 关闭所有协程
    /// </summary>
    public void StopCoroutinesAll()
    {
        StopAllCoroutines();
    }

    /// <summary>
    /// 关闭协程
    /// </summary>
    /// <param name="coroutine">要关闭的协程</param>
    public void StopCoroutines(IEnumerator coroutine)
    {
        StopCoroutine(coroutine);
    }


    protected IEnumerator DelayCoroutine(float time, System.Action e)
    {
        yield return Yielder.WaitForSeconds(time);
        e?.Invoke();
    }
    protected IEnumerator DelayCoroutine(System.Action e)
    {
        yield return null;
        e?.Invoke();
    }

    /// <summary>
    /// 延时处理
    /// </summary>
    /// <param name="time">延迟的时间</param>
    /// <param name="e">要处理的事件</param>
    public Coroutine OnDelay(float time, System.Action e)
    {
        return StartCoroutine(DelayCoroutine(time, e));
    }

    /// <summary>
    /// 在下一帧处理
    /// </summary>
    /// <param name="e">要处理的事件</param>
    public Coroutine OnDelay(System.Action e)
    {
        return StartCoroutine(DelayCoroutine(e));
    }

    /// <summary>
    /// 停止协程
    /// </summary>
    /// <param name="_coroutine"></param>
    public static void StopCor(ref Coroutine _coroutine)
    {
        if (_coroutine != null)
        {
            Instance?.StopCoroutine(_coroutine);
            _coroutine = null;
        }
    }

    /// <summary>
    /// 延时处理
    /// </summary>
    /// <param name="time"></param>
    /// <param name="e"></param>
    /// <returns></returns>
    public static Coroutine Delay(float time, System.Action e)
    {
        if (instance == null) return null;
        return Instance.StartCoroutine(Instance.DelayCoroutine(time, e));
    }
    /// <summary>
    /// 延时一帧处理
    /// </summary>
    /// <param name="e"></param>
    /// <returns></returns>
    public static Coroutine Delay(System.Action e)
    {
        if (instance == null) return null;
        return Instance.StartCoroutine(Instance.DelayCoroutine(e));
    }
}

/// <summary>
/// 协程等待（推荐使用，可起到一定优化作用）
/// </summary>
public static class Yielder
{
    static Dictionary<float, WaitForSeconds> waits = new Dictionary<float, WaitForSeconds>();

    /// <summary>
    /// 使用次数
    /// </summary>
    public static int UseNum
    {
        get; private set;
    }

    /// <summary>
    /// 使用缩放时间在给定的秒数内挂起协程执行。
    /// </summary>
    /// <param name="seconds">间隔</param>
    /// <returns></returns>
    public static WaitForSeconds WaitForSeconds(float seconds)
    {
        UseNum++;

        if (!waits.ContainsKey(seconds))
        {
            waits.Add(seconds, new WaitForSeconds(seconds));
        }

        return waits[seconds];
    }

    /// <summary>
    /// 清除
    /// </summary>
    public static void Clear()
    {
        waits.Clear();
    }
}