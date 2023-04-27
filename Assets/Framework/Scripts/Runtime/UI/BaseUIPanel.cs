// -------------------------
// 创建日期：2022/10/27 10:13:13
// -------------------------

using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Framework
{
    /// <summary>
    /// UI 界面基类
    /// </summary>
    public abstract class BaseUIPanel : MonoBehaviour
    {
        [Tooltip("是否初始化")]
        [SerializeField] protected bool m_init;

        [Tooltip("所属画布")]
        [SerializeField] protected Canvas _canvas;

        [Tooltip("是否被占用")]
        [SerializeField] protected bool isOccupy;// 是否被占用
        [Tooltip("自动置顶")]
        [SerializeField] protected bool autoPanelTopmost = true;// 自动置顶
        [Tooltip("常驻界面")]
        [SerializeField] protected bool permanent = false;
        [Tooltip("此界面是否启用")]
        [SerializeField] protected bool isEnable = true;
        [Tooltip("是否阻挡其他射线")]
        [SerializeField] protected bool isResistRay = true;
        [Tooltip("界面排序等级。界面排序考虑到各种情况较为复杂，如遇到此选项影响不到或有问题的界面，请考虑调整界面使用策略。")]
        [SerializeField] protected int sortOrderLevel = 0;

        public Action OnEnableEvent;
        public Action OnDisableEvent;
        public Action OnDestroyEvent;

        // 是否执行过初始化？
        protected bool isInited { get; set; } = false;

        /// <summary>
        /// 是否初始化
        /// </summary>
        public bool init
        {
            get
            {
                return m_init;
            }
            protected set
            {
                m_init= value;
            }
        }
        /// <summary>
        /// 是否被占用
        /// <para>ps：如果界面标识被占用，就无法通过 <see cref="UIPanelManager.GetPanel"/> 获取到，直到占用方取消占用</para>
        /// <para>请谨慎使用，以免引发不必要的麻烦</para>
        /// </summary>
        public virtual bool IsOccupy { get => isOccupy; set => isOccupy = value; }
        /// <summary>
        /// 自动置顶
        /// </summary>
        public virtual bool AutoPanelTopmost { get => autoPanelTopmost; set => autoPanelTopmost = value; }
        /// <summary>
        /// 界面所属画布
        /// </summary>
        public Canvas canvas { get => _canvas; set => _canvas = value; }

        /// <summary>
        /// 此界面是否启用
        /// <para>ps：此设置只是一个额外的标识，不会真的影响界面的启用，启用界面请使用 <see cref="Enable(bool)"/></para>
        /// </summary>
        public virtual bool IsEnable
        {
            get
            {
                if(gameObject==null) return false;
                return gameObject.activeInHierarchy && isEnable;
            }
            set
            {
                isEnable = value;
            }
        }
        /// <summary>
        /// 常驻界面
        /// </summary>
        public bool Permanent { get => permanent; set => permanent = value; }
        /// <summary>
        /// 是否阻挡其他射线
        /// <para>注意：获取时同时启用 <see cref="IsEnable"/> 才有效</para>
        /// </summary>
        public virtual bool IsResistRay
        {
            get
            {
                return isResistRay && IsEnable;
            }
            set
            {
                isResistRay = value;
            }
        }

        /// <summary>
        /// 界面排序等级
        /// <para>注意：会影响到 <see cref="UIPanelManager.PanelTopmost(BaseUIPanel, bool, bool)"/> 的排序策略</para>
        /// <para>界面排序考虑到各种情况较为复杂，如遇到此选项影响不到或有问题的界面，请考虑调整界面使用策略</para>
        /// <para>默认最会在同级、低级中排序</para>
        /// </summary>
        public virtual int SortOrderLevel
        {
            get
            {
                return sortOrderLevel;
            }
            set
            {
                sortOrderLevel = value;
            }
        }

        protected virtual void Awake()
        {
            UIPanelManager.Register(this);

        }

        protected virtual void Start()
        {
            //Debug.Log($"{this} Start");

        }

        protected virtual void OnEnable()
        {
            //Debug.Log($"{this} OnEnable");
            UIPanelManager.GetCanvas(this);

            if (autoPanelTopmost)
                this.PanelTopmost();

            OnEnableEvent?.Invoke();
        }

        protected virtual void FixedUpdate()
        {

        }

        protected virtual void OnDestroy()
        {
            OnDestroyEvent?.Invoke();

        }

        protected virtual void OnDisable()
        {
            OnDisableEvent?.Invoke();

        }

        /// <summary>
        /// 初始化界面
        /// </summary>
        ///// <returns>成功初始化返回 true，已经初始化或初始化失败返回 false</returns>
        public virtual void Init()
        {
            if (init)
            {
                return;
            }
            
            init = true;
        }

        /// <summary>
        /// 启用界面
        /// <para>ps：已启用则禁用，反之亦然</para>
        /// </summary>
        public virtual void Enable()
        {
            Enable(!gameObject.activeSelf);
        }
        /// <summary>
        /// 启用界面
        /// </summary>
        /// <param name="enable"></param>
        public virtual void Enable(bool enable)
        {
            if (gameObject.activeSelf == enable) return;

            gameObject.SetActive(enable);

        }
        /// <summary>
        /// 打开面板
        /// </summary>
        public virtual void OnShow()
        { }
        /// <summary>
        /// 打开面板
        /// </summary>
        public virtual void OnShow<T>(T info)
        { }
        public virtual void OnShow<T>(T info, T infos)
        { }
        public virtual void OnShow<T>(T info,T infos,T infoss)
        { }
        public virtual void OnShow(Dictionary<string,object> info)
        { }
        public virtual void OnShow(Dictionary<string, object> info, UnityAction action)
        { }
        public virtual void OnShow(Dictionary<string, object> info, UnityAction action,UnityAction unityAction)
        { }
        /// <summary>
        /// 销毁界面
        /// </summary>
        public virtual void Destroy()
        {
            this.DestroyPanel();
        }
        /// <summary>
        /// 隱藏面板
        /// </summary>
        public virtual void OnHide()
        {
            OnEnable();
        }

    }
}