using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Framework.LogSystem
{
    /// <summary>
    /// 日志操作视图 ui 
    /// </summary>
    public class LogViewUI : BaseUI
    {
        [SerializeField]
        private GameObject _logInfoItemUITemplate;
        [SerializeField]
        private Transform _logInfoItemUIParent;
        private GameObjectPool _logInfoItemUIPool;
        private List<LogInfoItemUI> _logInfoItemUIs;
        private LogInfoItemUI _selectLogInfoItemUI;// 当前选择的 log ui
        [SerializeField]
        private int _logInfoItemUIShowMaxNum = 200;// 允许日志显示的最大数量

        [Space]
        [SerializeField]
        private Text _conditionText;
        [SerializeField]
        private Text _stackTraceText;

        [Space]
        [SerializeField]
        private Button _closeBtn;
        [SerializeField]
        private Button _cLearLogBtn, _copySelectedLogBtn, _copyAllLogBtn;
        [SerializeField]
        private Text _logNumText;// 显示日志数量

        /// <summary>
        /// 关闭时触发此事件
        /// </summary>
        public Action closeClickEvent;

        public GameObject logInfoItemUITemplate
        {
            get => _logInfoItemUITemplate;
            set
            {
                _logInfoItemUITemplate = value;
                _logInfoItemUIPool.template = value;
            }
        }

        protected override void Awake()
        {
            base.Awake();

            _logInfoItemUIPool ??= new GameObjectPool(1, _logInfoItemUITemplate);
            _logInfoItemUIs ??= new List<LogInfoItemUI>(_logInfoItemUIShowMaxNum);

        }

        protected override void Start()
        {
            base.Start();

            _logInfoItemUIPool.PreCreateInstanceAsync(100);

            AddEvent(_closeBtn, () =>
            {
                Enable(false);
                closeClickEvent?.Invoke();
            });
            AddEvent(_cLearLogBtn, () =>
            {
                // 清除
                ClearLog();
            });
            AddEvent(_copySelectedLogBtn, () =>
            {
                // 复制日志
                //_selectLogInfoItemUI
                if (_selectLogInfoItemUI)
                {
                    //GUIUtility.systemCopyBuffer = _selectLogInfoItemUI.logInfo.condition;
                    GUIUtility.systemCopyBuffer = _selectLogInfoItemUI.logInfo.ToText();
                }
            });
            AddEvent(_copyAllLogBtn, () =>
            {
                // 复制日志
                GUIUtility.systemCopyBuffer = LogInfo.ToFileFormatTextAll();
            });
        }

        private void Update()
        {
            if (_logNumText)
            {
                //_logNumText.text = $"{_logInfoItemUIs.Count}/{_logInfoItemUIShowMaxNum}";
                _logNumText.text = $"显示数量 {_logInfoItemUIs.Count}/{_logInfoItemUIShowMaxNum}，总量 {LogInfo.logInfoCount}";
            }
        }

        protected override void OnEnable()
        {
            base.OnEnable();

            // 更新日志显示
            UpdateLogUI();

            LogInfo.logMessageReceived += OnHandleLog;
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            LogInfo.logMessageReceived -= OnHandleLog;
        }

        public override void Init()
        {
            base.Init();

        }

        // 接收处理日志
        //private void OnHandleLog(string condition, string stackTrace, LogType type)
        public void OnHandleLog(LogInfo info)
        {
            AddLogUI(info);
        }

        /// <summary>
        /// 刷新日志 ui
        /// </summary>
        public void RefreshLogUI()
        {
            for (int i = 0; i < _logInfoItemUIs.Count; i++)
            {
                var _ui = _logInfoItemUIs[i];
                int n = i + 1;
                SetLogUIFills(_ui, n);
            }
        }
        // 设置填充区分色块
        void SetLogUIFills(LogInfoItemUI _ui, int n)
        {
            // 双数打开背景
            if (_ui)
            {
                _ui.isOpenBG = n % 2 == 0;
            }
        }

        /// <summary>
        /// 更新日志 ui
        /// </summary>
        public void UpdateLogUI()
        {
            ClearLog();

            // 取出限制数量的 日志
            var logs = TypePool.root.GetList<LogInfo>();
            int n = 0;
            for (int j = LogInfo.logInfos.Count - (1); j >= 0; j--)
            {
                ++n;
                if (n >= _logInfoItemUIShowMaxNum)
                {
                    break;
                }
                var info = LogInfo.logInfos[j];
                logs.Insert(0, info);
            }
            foreach (var info in logs)
            {
                AddLogUI(info);
            }
            // TODO方法二：计算开始索引


            TypePool.root.Return(logs);
        }

        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="info"></param>
        public void AddLogUI(LogInfo info)
        {
            var _ui = _logInfoItemUIPool.Get().GetComponent<LogInfoItemUI>();
            if (_ui != null)
            {
                _ui.gameObject.SetActive(true);
                _ui.transform.SetParent(_logInfoItemUIParent);
                _ui.SetLogInfo(info);
                _ui.clickEvent = () =>
                {
                    // 设置点击事件
                    SelectLogUI(_ui);
                };
                _logInfoItemUIs.Add(_ui);

                // 区分色块
                SetLogUIFills(_ui, _logInfoItemUIs.Count);

                // 检查是否超过最大数量
                while (_logInfoItemUIs.Count > _logInfoItemUIShowMaxNum && _logInfoItemUIs.Count > 0)
                {
                    RemoveLogUI(0);// 移除最老的
                }
            }
        }
        // 移除
        public void RemoveLogUI(int index)
        {
            var _ui = _logInfoItemUIs[index];
            _logInfoItemUIs.RemoveAt(index);
            if (_ui != null)
            {
                ReturnPool(_ui);
            }
        }
        // 返回对象池
        void ReturnPool(LogInfoItemUI _ui)
        {
            _logInfoItemUIPool.Return(_ui.gameObject);
        }
        // 选中
        public void SelectLogUI(LogInfoItemUI _ui)
        {
            if (_ui == _selectLogInfoItemUI)
            {
                return;
            }
            if (_ui == null)
            {
                if (!_selectLogInfoItemUI)
                {
                    _selectLogInfoItemUI.isSelected = false;
                }
            }
            else
            {
                foreach (var item in _logInfoItemUIs)
                {
                    item.isSelected = false;
                }
                _ui.isSelected = true;

                // 显示 log 详情
                ShowLogInfo(_ui.logInfo);
            }

            _selectLogInfoItemUI = _ui;
        }

        // 显示日志信息
        public void ShowLogInfo(LogInfo info)
        {
            if (info == null)
            {
                ExtendUtility.SetText(_conditionText, null);
                ExtendUtility.SetText(_stackTraceText, null);
            }
            else
            {
                ExtendUtility.SetText(_conditionText, info.condition);
                ExtendUtility.SetText(_stackTraceText, info.stackTrace);
            }
        }

        public void ClearLog()
        {
            foreach (var item in _logInfoItemUIs)
            {
                ReturnPool(item);
            }
            _logInfoItemUIs.Clear();

            ShowLogInfo(null);
        }
    }
}