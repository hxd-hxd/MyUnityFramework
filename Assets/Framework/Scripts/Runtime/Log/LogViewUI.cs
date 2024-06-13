using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Framework.LogSystem
{
    /// <summary>
    /// ��־������ͼ ui 
    /// </summary>
    public class LogViewUI : BaseUI
    {
        [SerializeField]
        private GameObject _logInfoItemUITemplate;
        [SerializeField]
        private Transform _logInfoItemUIParent;
        private GameObjectPool _logInfoItemUIPool;
        private List<LogInfoItemUI> _logInfoItemUIs;
        private LogInfoItemUI _selectLogInfoItemUI;// ��ǰѡ��� log ui
        [SerializeField]
        private int _logInfoItemUIShowMaxNum = 200;// ������־��ʾ���������

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
        private Text _logNumText;// ��ʾ��־����

        /// <summary>
        /// �ر�ʱ�������¼�
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
                // ���
                ClearLog();
            });
            AddEvent(_copySelectedLogBtn, () =>
            {
                // ������־
                //_selectLogInfoItemUI
                if (_selectLogInfoItemUI)
                {
                    //GUIUtility.systemCopyBuffer = _selectLogInfoItemUI.logInfo.condition;
                    GUIUtility.systemCopyBuffer = _selectLogInfoItemUI.logInfo.ToText();
                }
            });
            AddEvent(_copyAllLogBtn, () =>
            {
                // ������־
                GUIUtility.systemCopyBuffer = LogInfo.ToFileFormatTextAll();
            });
        }

        private void Update()
        {
            if (_logNumText)
            {
                //_logNumText.text = $"{_logInfoItemUIs.Count}/{_logInfoItemUIShowMaxNum}";
                _logNumText.text = $"��ʾ���� {_logInfoItemUIs.Count}/{_logInfoItemUIShowMaxNum}������ {LogInfo.logInfoCount}";
            }
        }

        protected override void OnEnable()
        {
            base.OnEnable();

            // ������־��ʾ
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

        // ���մ�����־
        //private void OnHandleLog(string condition, string stackTrace, LogType type)
        public void OnHandleLog(LogInfo info)
        {
            AddLogUI(info);
        }

        /// <summary>
        /// ˢ����־ ui
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
        // �����������ɫ��
        void SetLogUIFills(LogInfoItemUI _ui, int n)
        {
            // ˫���򿪱���
            if (_ui)
            {
                _ui.isOpenBG = n % 2 == 0;
            }
        }

        /// <summary>
        /// ������־ ui
        /// </summary>
        public void UpdateLogUI()
        {
            ClearLog();

            // ȡ������������ ��־
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
            // TODO�����������㿪ʼ����


            TypePool.root.Return(logs);
        }

        /// <summary>
        /// ���
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
                    // ���õ���¼�
                    SelectLogUI(_ui);
                };
                _logInfoItemUIs.Add(_ui);

                // ����ɫ��
                SetLogUIFills(_ui, _logInfoItemUIs.Count);

                // ����Ƿ񳬹��������
                while (_logInfoItemUIs.Count > _logInfoItemUIShowMaxNum && _logInfoItemUIs.Count > 0)
                {
                    RemoveLogUI(0);// �Ƴ����ϵ�
                }
            }
        }
        // �Ƴ�
        public void RemoveLogUI(int index)
        {
            var _ui = _logInfoItemUIs[index];
            _logInfoItemUIs.RemoveAt(index);
            if (_ui != null)
            {
                ReturnPool(_ui);
            }
        }
        // ���ض����
        void ReturnPool(LogInfoItemUI _ui)
        {
            _logInfoItemUIPool.Return(_ui.gameObject);
        }
        // ѡ��
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

                // ��ʾ log ����
                ShowLogInfo(_ui.logInfo);
            }

            _selectLogInfoItemUI = _ui;
        }

        // ��ʾ��־��Ϣ
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