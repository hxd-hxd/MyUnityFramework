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
    public abstract class BaseUIPanel : BaseUI
    {
        [Tooltip("是否初始化")]
        [SerializeField] protected bool _isInit;

        [Tooltip("所属画布")]
        [SerializeField] protected Canvas _canvas;

        [Tooltip("是否被占用")]
        [SerializeField] protected bool isOccupy;// 是否被占用
        [Tooltip("自动置顶")]
        [SerializeField] protected bool autoPanelTopmost = true;// 自动置顶
        [Tooltip("常驻界面")]
        [SerializeField] protected bool permanent = false;
        [Tooltip("此界面是否启用")]
        [SerializeField] protected bool _isEnable = true;
        [Tooltip("是否阻挡其他射线")]
        [SerializeField] protected bool isResistRay = true;
        [Tooltip("界面排序等级。界面排序考虑到各种情况较为复杂，如遇到此选项影响不到或有问题的界面，请考虑调整界面使用策略。")]
        [SerializeField] protected int sortOrderLevel = 0;

        public Action OnEnableEvent;
        public Action OnDisableEvent;
        public Action OnDestroyEvent;

        /// <summary>
        /// 是否初始化
        /// </summary>
        public bool isInit
        {
            get
            {
                return _isInit;
            }
            protected set
            {
                _isInit= value;
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
        public override bool isEnable
        {
            get
            {
                if(gameObject==null) return false;
                return gameObject.activeInHierarchy && _isEnable;
            }
            set
            {
                _isEnable = value;
            }
        }
        /// <summary>
        /// 常驻界面
        /// </summary>
        public bool Permanent { get => permanent; set => permanent = value; }
        /// <summary>
        /// 是否阻挡其他射线
        /// <para>注意：获取时同时启用 <see cref="isEnable"/> 才有效</para>
        /// </summary>
        public virtual bool IsResistRay
        {
            get
            {
                return isResistRay && isEnable;
            }
            set
            {
                isResistRay = value;
            }
        }

        /// <summary>
        /// 界面排序等级
        /// <para>注意：会影响到 <see cref="UIPanelManager.PanelTopmost(BaseUIPanel, bool, bool)"/> 的排序策略。</para>
        /// <para>界面排序考虑到各种情况较为复杂，如遇到此选项影响不到或有问题的界面，请考虑调整界面使用策略。</para>
        /// <para>默认最会在同级、低级中排序。</para>
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

        protected override void Awake()
        {
            base.Awake();

            UIPanelManager.Register(this);

        }

        protected override void Start()
        {
            base.Start();
            //Debug.Log($"{this} Start");

        }

        protected override void OnEnable()
        {
            base.OnEnable();

            //Debug.Log($"{this} OnEnable");
            UIPanelManager.GetCanvas(this);

            if (autoPanelTopmost)
                this.PanelTopmost();

            OnEnableEvent?.Invoke();
        }

        protected virtual void FixedUpdate()
        {

        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            UIPanelManager.RemovePanel(this);

            OnDestroyEvent?.Invoke();

        }

        protected override void OnDisable()
        {
            OnDisableEvent?.Invoke();

        }

        /// <summary>
        /// 初始化界面
        /// </summary>
        ///// <returns>成功初始化返回 true，已经初始化或初始化失败返回 false</returns>
        public override void Init()
        {
            if (isInit)
            {
                return;
            }
            
            isInit = true;
        }

        ///// <summary>
        ///// 启用界面
        ///// <para>ps：已启用则禁用，反之亦然</para>
        ///// </summary>
        //public virtual void Enable()
        //{
        //    Enable(!gameObject.activeSelf);
        //}
        /// <summary>
        /// 启用界面
        /// </summary>
        /// <param name="enable"></param>
        public override void Enable(bool enable)
        {
            if (gameObject.activeSelf == enable) return;

            gameObject.SetActive(enable);

        }

        /// <summary>
        /// 销毁界面
        /// </summary>
        public override void Destroy()
        {
            base.Destroy();
            this.DestroyPanel();
        }

    }
}