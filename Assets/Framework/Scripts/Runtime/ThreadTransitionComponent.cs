using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// 线程过渡组件
/// <para>ps：用于解决在 unity 中，其他线程无法调用主线程的问题</para>
/// </summary>
public class ThreadTransitionComponent : MonoBehaviour
{
    //是否已经初始化
    static bool isInitialized;

    private static ThreadTransitionComponent _ins;

    public static ThreadTransitionComponent Instance
    {
        get
        {
            Initialize();
            return _ins;
        }
    }

    //初始化
    public static void Initialize()
    {
        if (!isInitialized)
        {
            if (!Application.isPlaying)
                return;

            isInitialized = true;
            _ins = new GameObject("[Thread Transition]").AddComponent<ThreadTransitionComponent>();

            DontDestroyOnLoad(_ins.gameObject);
        }
    }

    void Awake()
    {
        _ins = this;
        isInitialized = true;
    }

    //单个执行单元（无延迟）
    struct NoDelayedQueueItem
    {
        public Action<object> action;
        public object param;
    }

    //全部执行列表（无延迟）
    List<NoDelayedQueueItem> listNoDelayActions = new List<NoDelayedQueueItem>();


    //单个执行单元（有延迟）
    struct DelayedQueueItem
    {
        public Action<object> action;
        public object param;
        public float time;
    }

    //全部执行列表（有延迟）
    List<DelayedQueueItem> listDelayedActions = new List<DelayedQueueItem>();

    List<Action> listAction = new List<Action>();


    /// <summary>
    /// 加入到主线程执行队列（无延迟）
    /// </summary>
    /// <param name="taction"></param>
    public static void QueueOnMainThread(Action taction)
    {
        lock (Instance)
        {
            if (!Instance)
            {
                Debug.Log("<color=red>Error：线程过渡组件 未初始化</color>");
                return;
            }
        }

        lock (Instance.listAction)
        {
            Instance.listAction.Add(taction);
        }
    }

    /// <summary>
    /// 加入到主线程执行队列（无延迟）
    /// </summary>
    /// <param name="taction"></param>
    /// <param name="param"></param>
    public static void QueueOnMainThread(Action<object> taction, object param)
    {
        QueueOnMainThread(taction, param, 0f);
    }

    /// <summary>
    /// 加入到主线程执行队列（有延迟）
    /// </summary>
    /// <param name="action"></param>
    /// <param name="param"></param>
    /// <param name="time"></param>
    public static void QueueOnMainThread(Action<object> action, object param, float time)
    {
        lock (Instance)
        {
            if (!Instance)
            {
                Debug.LogWarning("<color=red>Error：线程过渡组件 未初始化</color>");
                return;
            }
        }

        if (time != 0)
        {
            lock (Instance.listDelayedActions)
            {
                Instance.listDelayedActions.Add(new DelayedQueueItem
                    { time = Time.time + time, action = action, param = param });
            }
        }
        else
        {
            lock (Instance.listNoDelayActions)
            {
                Instance.listNoDelayActions.Add(new NoDelayedQueueItem { action = action, param = param });
            }
        }
    }


    //当前执行的无延时函数链
    List<Action> actions = new List<Action>();

    //当前执行的无延时函数链
    List<NoDelayedQueueItem> currentActions = new List<NoDelayedQueueItem>();

    //当前执行的有延时函数链
    List<DelayedQueueItem> currentDelayed = new List<DelayedQueueItem>();

    void Update()
    {
        if (listAction.Count > 0)
        {
            lock (listAction)
            {
                actions.Clear();
                actions.AddRange(listAction);
                listAction.Clear();
            }

            for (int i = 0; i < actions.Count; i++)
            {
                actions[i]?.Invoke();
            }
        }

        if (listNoDelayActions.Count > 0)
        {
            lock (listNoDelayActions)
            {
                currentActions.Clear();
                currentActions.AddRange(listNoDelayActions);
                listNoDelayActions.Clear();
            }

            for (int i = 0; i < currentActions.Count; i++)
            {
                currentActions[i].action(currentActions[i].param);
            }
        }

        if (listDelayedActions.Count > 0)
        {
            lock (listDelayedActions)
            {
                currentDelayed.Clear();
                currentDelayed.AddRange(listDelayedActions.Where(d => Time.time >= d.time));
                for (int i = 0; i < currentDelayed.Count; i++)
                {
                    listDelayedActions.Remove(currentDelayed[i]);
                }
            }

            for (int i = 0; i < currentDelayed.Count; i++)
            {
                currentDelayed[i].action(currentDelayed[i].param);
            }
        }
    }

    //void OnDisable()
    //{
    //    if (_ins == this)
    //    {
    //        _ins = null;
    //    }
    //}
}