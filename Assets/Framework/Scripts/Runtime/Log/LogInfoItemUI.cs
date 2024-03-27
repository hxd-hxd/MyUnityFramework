using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace Framework.LogSystem
{
    /// <summary>
    /// ��־��Ϣ ui ��
    /// </summary>
    public class LogInfoItemUI : MonoBehaviour, ITypePoolObject
    {
        [SerializeField]
        private Image _bg, _selectUI;
        [SerializeField]
        private Text _textUI;
        [SerializeField]
        private Button _btnUI;

        [SerializeField]
        private LogInfo _logInfo;

        /// <summary>
        /// ����¼�
        /// </summary>
        public Action clickEvent;

        /// <summary>
        /// �Ƿ�򿪱���
        /// </summary>
        public bool isOpenBG
        {
            get { return ExtendUtility.GetActive(_bg); }
            set { ExtendUtility.SetActive(_bg, value); }
        }
        /// <summary>
        /// ѡ��״̬
        /// </summary>
        public bool isSelected
        {
            get { return ExtendUtility.GetActive(_selectUI); }
            set { ExtendUtility.SetActive(_selectUI, value); }
        }
        /// <summary>
        /// �ı�
        /// </summary>
        public string text
        {
            get { return ExtendUtility.GetText(_textUI); }
            set { ExtendUtility.SetText(_textUI, value); }
        }
        public LogInfo logInfo
        {
            get { return _logInfo; }
            set { SetLogInfo(_logInfo); }
        }

        void ITypePoolObject.Clear()
        {
            isOpenBG = false;
            isSelected = false;
            text = null;
            SetLogInfo(null);
        }

        void Start()
        {
            if (_btnUI)
            {
                _btnUI.onClick.AddListener(() =>
                {
                    clickEvent?.Invoke();
                });
            }
        }

        public void SetLogInfo(LogInfo info)
        {
            _logInfo = info;
            if (info != null)
            {
                text = info.ToConditionText();
                SetTextColor();

                var sb = TypePool.root.Get<StringBuilder>();
                // ʱ��
                sb.Append("[").Append(info.time.ToHourBehindTimeText()).Append("]");
                // ������ͱ�ʶ
                sb.Append("[").Append(info.logType).Append("]");
                name = sb.ToString();
                TypePool.root.Return(sb);
            }
        }

        public void SetTextColor()
        {
            if (_textUI && _logInfo != null)
            {
                _textUI.color = _logInfo.logColor;
            }
        }
    }
}