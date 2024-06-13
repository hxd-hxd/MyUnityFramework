// -------------------------
// 创建日期：2023/4/7 11:29:14
// -------------------------

using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Framework
{
    /// <summary>
    /// 分部处理器
    /// </summary>
    public class Sectionalizer
    {
        [SerializeField] private MonoBehaviour actuator;

        [SerializeField] private int onceMax = 10;// 一次最多的数量
        [SerializeField] private List<Action> prepares = new List<Action>(100);   // 预备队列
        [SerializeField] private List<Action> executes = new List<Action>(100);   // 执行创建列表

        /// <summary>
        /// 执行结束事件
        /// <para>在 <see cref="Fill"/> 之后 <see cref="Prepares"/> 为空，并在 <see cref="Execute"/> 之后执行</para>
        /// </summary>
        public event Action ExecuteEndEvent;

        protected bool _isExecuteEndEvent = false;  // 执行结束

        protected Coroutine coroutinePrepare;

        /// <summary>
        /// 是否执行过 <see cref="ExecuteEndEvent"/>
        /// </summary>
        public bool isExecuteEndEvent
        {
            get { return _isExecuteEndEvent; }
        }

        public int OnceMax { get => onceMax; set => onceMax = value; }
        public MonoBehaviour Actuator { get => actuator; protected set => actuator = value; }
        public List<Action> Prepares { get => prepares; protected set => prepares = value; }
        public List<Action> Executes { get => executes; protected set => executes = value; }

        public Sectionalizer() { }

        /// <summary>
        /// 默认初始化
        /// </summary>
        /// <param name="actuator"></param>
        public Sectionalizer(MonoBehaviour actuator)
        {
            Initialize(actuator);
        }

        /// <summary>
        /// 默认初始化
        /// </summary>
        /// <param name="actuator"></param>
        public Sectionalizer(MonoBehaviour actuator, int onceMax) : this(actuator)
        {
            this.onceMax = onceMax;
        }

        public void Initialize(MonoBehaviour actuator)
        {
            Initialize(actuator, onceMax);
        }
        public void Initialize(MonoBehaviour actuator, int onceMax)
        {
            this.onceMax = onceMax;

            this.Actuator = actuator;
            Prepares.Clear();
            Executes.Clear();

            Stop();
        }
        /// <summary>
        /// 初始化并开始
        /// </summary>
        /// <param name="actuator"></param>
        public void InitializeStart(MonoBehaviour actuator)
        {
            Initialize(actuator);
            Start();
        }
        /// <summary>
        /// 初始化并开始
        /// </summary>
        /// <param name="actuator"></param>
        public void InitializeStart(MonoBehaviour actuator, int onceMax)
        {
            Initialize(actuator, onceMax);
            Start();
        }

        /// <summary>
        /// 向预备队列中添加
        /// </summary>
        /// <param name="datas"></param>
        public void Add(List<Action> datas)
        {
            foreach (var item in datas)
            {
                Add(item);
            }
        }
        /// <summary>
        /// 向预备队列中添加
        /// </summary>
        /// <param name="data"></param>
        public void Add(Action data)
        {
            if (!Prepares.Contains(data))
            {
                Prepares.Add(data);
            }
        }
        /// <summary>
        /// 在预备队列中移除
        /// </summary>
        /// <param name="data"></param>
        public void Remove(Action data)
        {
            if (Prepares.Contains(data))
            {
                Prepares.Remove(data);
            }
        }

        /// <summary>
        /// 清除所有
        /// </summary>
        public void Clear()
        {
            Prepares.Clear();
            Executes.Clear();
        }

        protected int fillBeforeNum = 0;        // 记录在填充之前的数量
        protected int fillAfterNum = 0;         // 记录在填充之后的数量
        protected bool end = false;             // 结束

        /// <summary>
        /// 向执行列表填充
        /// </summary>
        public void Fill()
        {
            if (Executes.Count >= OnceMax) return;

            fillBeforeNum = prepares.Count;

            for (int i = 0; i < Prepares.Count; i++)
            {
                if (Executes.Count >= OnceMax) return;

                var pre = Prepares[i];
                Executes.Add(pre);
                Prepares.Remove(pre);
                i--;
            }

            fillAfterNum = prepares.Count;

            end = fillBeforeNum == fillAfterNum;// 结束

            // 没有结束，并且已经执行过结束事件，则重置
            if (!end && _isExecuteEndEvent) _isExecuteEndEvent = false;
        }
        /// <summary>
        /// 执行
        /// </summary>
        public void Execute()
        {
            for (int i = 0; i < Executes.Count; i++)
            {
                var item = Executes[i];
                //ExecuteEvent?.Invoke(item);
                item?.Invoke();
            }
            Executes.Clear();

            if (end)
            {
                if (!_isExecuteEndEvent)
                {
                    _isExecuteEndEvent = true;
                    ExecuteEndEvent?.Invoke();
                }
            }
        }

        /// <summary>
        /// 停止
        /// </summary>
        public virtual void Stop()
        {
            if (coroutinePrepare != null)
            {
                StopCoroutine(coroutinePrepare);
                coroutinePrepare = null;
            }
        }
        /// <summary>
        /// 开始
        /// </summary>
        public virtual void Start()
        {
            if (!Actuator)
            {
                //Log.Error($"未指定 {nameof(actuator)}");
                return;
            }

            Stop();
            coroutinePrepare = StartCoroutine(OnStart());
        }
        private IEnumerator OnStart()
        {
            while (true)
            {
                Fill();
                Execute();
                yield return null;
            }
        }


        public Coroutine StartCoroutine(IEnumerator enumerator)
        {
            return Actuator?.StartCoroutine(enumerator);
        }
        public void StopCoroutine(Coroutine enumerator)
        {
            try
            {
                Actuator?.StopCoroutine(enumerator);
            }
            catch (Exception)
            {
                //Log.Error(e);
            }
        }
    }

}