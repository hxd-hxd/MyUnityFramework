// -------------------------
// 创建日期：2023/3/31 14:41:15
// -------------------------

using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

using Object = UnityEngine.Object;
using Random = UnityEngine.Random;
using Framework.Extend;
using System.Linq;

namespace Framework
{

    /// <summary>
    /// 默认实现的下拉菜单
    /// <para>请使用特定的 <see cref="DropdownPro_Text"/> 或 DropdownPro_TMPText</para>
    /// </summary>
    public class DropdownPro : DropdownProAbstract<MaskableGraphic> { }

    /// <summary>
    /// 下拉菜单
    /// </summary>
    [RequireComponent(typeof(RectTransform))]
    public abstract class DropdownProAbstract<Text> : Selectable, IPointerClickHandler, IEventSystemHandler, ISubmitHandler, ICancelHandler
        where Text : MaskableGraphic
        //TMP_Text , UnityEngine.UI.Text
    {
        // 已有
        [Space(16)]
        [Tooltip("菜单列表模板")]
        [SerializeField] protected RectTransform m_Template;
        [Tooltip("当前显示的文本")]
        [SerializeField] protected Text m_CaptionText;
        [Tooltip("在未选择选项时使用的占位符")]
        [SerializeField] protected GameObject m_Placeholder;

        [Space]
        [Tooltip("选项模板")]
        [SerializeField] protected GameObject m_Item;
        //[Tooltip("模板选项文本")]
        //[SerializeField] protected Text m_ItemText;

        // 美化
        [Space]
        [Tooltip("样式")]
        [SerializeField] protected SytleGroup m_SytleGroup;

        // 数据
        [Space]
        [SerializeField] protected int m_Value = 0;

        [Tooltip("选项数据列表")]
        [SerializeField] protected List<DropdownItemData> m_Options = new List<DropdownItemData>();

        // 实例化
        protected List<DropdownItem> m_OptionItems = new List<DropdownItem>();// 下拉菜单选项
        protected RectTransform dataListMenu;       // 实例化后的列表菜单
        protected ScrollRect m_ScrollRect;          // 滚动列表

        protected DropdownItem currentItem;         // 当前选项
        protected DropdownItemData currentData;     // 当前选项数据

        /// <summary>
        /// 背景阻断器
        /// </summary>
        protected GameObject m_Blocker;

        /// <summary>
        /// 选项选择事件
        /// <para>取消选中时 <see cref="DropdownItemData"/> 参数将为 <see cref="null"/></para>
        /// </summary>
        public event Action<DropdownItemData> SelectedEvent;
        /// <summary>
        /// 菜单显示时
        /// </summary>
        public event Action<bool> MenuEnableEvent;

        #region 优化

        [Header("清除时保留的选项上限")]
        [Header("优化设置")]
        [Space]
        [SerializeField] protected int itemReserveMax = 100;// 保留的选项上限，清除时

        protected Sectionalizer m_Precreator = new Sectionalizer();
        protected Sectionalizer m_Precleaner = new Sectionalizer();

        /// <summary>
        /// 预创建器
        /// </summary>
        protected Sectionalizer precreator
        {
            get { return m_Precreator; }
        }
        /// <summary>
        /// 预清除器
        /// </summary>
        protected Sectionalizer precleaner
        {
            get { return m_Precleaner; }
        }

        public int ItemReserveMax { get => itemReserveMax; set => itemReserveMax = value; }

        #endregion


        /// <summary>
        /// 根 Canvas 
        /// </summary>
        public virtual Canvas rootCanvas
        {
            get
            {
                return targetGraphic.canvas;
            }
        }

        /// <summary>
        /// 选项数据
        /// </summary>
        public List<DropdownItemData> Options { get { return m_Options; } protected set => m_Options = value; }
        /// <summary>
        /// 选项列表
        /// </summary>
        public List<DropdownItem> OptionItems { get { return m_OptionItems; } protected set => m_OptionItems = value; }

        /// <summary>
        /// <see cref="m_CaptionText"/> 
        /// </summary>
        public Text captionText
        {
            get
            {
                return m_CaptionText;
            }
            set
            {
                m_CaptionText = value;
            }
        }

        /// <summary>
        /// <see cref="m_CaptionText"/> 
        /// </summary>
        public string captionText_text
        {
            get
            {
                return TextUtility.GetText(m_CaptionText);
            }
            set
            {
                TextUtility.SetText(m_CaptionText, value);
            }
        }

        /// <summary>
        /// 下拉菜单
        /// </summary>
        public RectTransform Menu
        {
            get
            {
                return dataListMenu;
            }
        }

        /// <summary>
        /// 选项模板
        /// </summary>
        public GameObject ItemTemplate { get { return m_Item; } protected set => m_Item = value; }
        /// <summary>
        /// 在未选择选项时使用的占位符
        /// </summary>
        public GameObject Placeholder { get { return m_Placeholder; } protected set => m_Placeholder = value; }

        /// <summary>
        /// 样式
        /// </summary>
        public SytleGroup sytleGroup { get { return m_SytleGroup; } protected set => m_SytleGroup = value; }

        /// <summary>
        /// 当前数据项
        /// </summary>
        public DropdownItemData CurrentData { get => currentData; protected set => currentData = value; }
        /// <summary>
        /// 当前数据项
        /// </summary>
        public DropdownItem CurrentItem { get => currentItem; protected set => currentItem = value; }

        /// <summary>
        /// 有效的 <see cref="DropdownItemData"/>
        /// <para>如果当前 <see cref="CurrentData"/> 为 <see cref="null"/>，则根据 <see cref="Value"/> 获取一个有效的 <see cref="DropdownItemData"/></para>
        /// </summary>
        public virtual DropdownItemData ValidData
        {
            get
            {
                var _data = currentData ?? Options.TryIndex(m_Value);
                return _data;
            }
        }

        /// <summary>
        /// 在设置此值时根据索引显示内容
        /// </summary>
        public int Value
        {
            get
            {
                AmendValue();
                return m_Value;
            }
            set
            {
                SelectData(value);
            }
        }

        /// <summary>
        /// 数据列表大小
        /// </summary>
        public int Count
        {
            get
            {
                return Options.Count;
            }
        }

        /// <summary>
        /// 菜单是否显示
        /// </summary>
        public bool IsShowMenu
        {
            get
            {
                return dataListMenu ? dataListMenu.gameObject.activeSelf : false;
            }
        }


        protected override void Awake()
        {
            base.Awake();

        }

        protected override void Start()
        {
            base.Start();

            if (!m_Template) m_Template = transform.FindOf<RectTransform>("Template");
            if (!m_CaptionText) m_CaptionText = transform.FindOf<Text>("Label");
            if (!m_Placeholder) m_Placeholder = transform.FindOf("Placeholder")?.gameObject;
            if (!m_Item) m_Item = transform.FindOf("Item")?.gameObject;
            //if (!m_ItemText)    m_ItemText      = transform.FindOf<Text>("Item Label");

            precreator.InitializeStart(Coroutines.Instance, 1);
            precleaner.InitializeStart(Coroutines.Instance, 1);

            precreator.ExecuteEndEvent += LocateCurrent;

        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            precreator.Stop();
            precleaner.Stop();
        }

        public void OnCancel(BaseEventData eventData)
        {
            //Debug.Log("OnCancel");

            ShowMenu(false);
        }

        public void OnPointerClick(PointerEventData eventData)
        {

            if (eventData.pointerEnter == gameObject)
            {
                //Debug.Log("OnPointerClick");
                ShowMenu();
            }
        }

        public void OnSubmit(BaseEventData eventData)
        {
            //Debug.Log("OnSubmit");

            //ShowMenu();
        }


#if UNITY_EDITOR
        protected override void OnValidate()
        {
            base.OnValidate();

            if (IsActive())
            {
                RefreshValueShow();
            }
        }


        //private void Update()
        //{
        //    if (Input.GetKeyDown(KeyCode.KeypadPlus))
        //    {
        //        m_Value++;
        //        RefreshValueShow();
        //    }
        //}
#endif

        #region UI 和数据同时更改

        /// <summary>
        /// 添加选项
        /// </summary>
        /// <param name="itemData"></param>
        public void AddOptions(List<string> itemDatas)
        {
            foreach (var item in itemDatas)
            {
                AddOption(item);
            }
        }
        /// <summary>
        /// 添加选项
        /// </summary>
        /// <param name="itemData"></param>
        public void AddOptions(List<DropdownItemData> itemDatas)
        {
            foreach (var item in itemDatas)
            {
                AddOption(item);
            }
        }
        /// <summary>
        /// 添加选项
        /// </summary>
        /// <param name="itemData"></param>
        public void AddOptions(List<DropdownItemData> itemDatas, int index)
        {
            foreach (var item in itemDatas)
            {
                AddOption(item, index);
            }
        }

        /// <summary>
        /// 添加选项
        /// </summary>
        /// <param name="itemData"></param>
        public DropdownItem AddOption(string text)
        {
            return AddOption(new DropdownItemData(text));
        }
        /// <summary>
        /// 添加选项
        /// </summary>
        /// <param name="itemData"></param>
        public DropdownItem AddOption(string text, int index)
        {
            return AddOption(new DropdownItemData(text), index);
        }
        /// <summary>
        /// 添加选项
        /// </summary>
        /// <param name="itemData"></param>
        public DropdownItem AddOption(DropdownItemData itemData)
        {
            AddData(itemData);
            return AddItem(itemData);
        }
        /// <summary>
        /// 添加选项到指定位置
        /// </summary>
        /// <param name="itemData"></param>
        public DropdownItem AddOption(DropdownItemData itemData, int index)
        {
            AddData(itemData, index);

            var item = AddItem(itemData, index);

            AmendValue();

            return item;
        }

        public virtual void RemoveOption(DropdownItemData itemData)
        {
            RemoveData(itemData);
            RemoveItem(itemData);

            RefreshShow();
        }
        public virtual void RemoveOption(int index)
        {
            RemoveData(index);
            RemoveItem(index);

            RefreshShow();
        }

        /// <summary>
        /// 设置数据、选项位置
        /// </summary>
        /// <param name="itemData"></param>
        public virtual void SetOptionPos(int oldIndex, int posIndex)
        {
            SetDataPos(oldIndex, posIndex);
            SetItemPos(oldIndex, posIndex);

            AmendValue();
        }

        /// <summary>
        /// 选择选项
        /// </summary>
        /// <param name="item"></param>
        public virtual void SelectOption(int index)
        {
            SelectData(index);

            // 设置选项开关的显示
            SclectToggle(index);

            RefreshCurrentDataShow();
        }

        /// <summary>
        /// 选择选项
        /// </summary>
        /// <param name="item"></param>
        public virtual void SelectOption(DropdownItem item)
        {
            SelectOption(item.itemData, item);
        }
        /// <summary>
        /// 选择选项
        /// </summary>
        /// <param name="item"></param>
        public virtual void SelectOption(DropdownItemData itemData, DropdownItem item)
        {
            SelectData(itemData);

            // 设置选项开关的显示
            SclectToggle(item);

            RefreshCurrentDataShow();
        }

        /// <summary>
        /// 取消当前选择
        /// </summary>
        /// <param name="item"></param>
        public void CancelCurrentOption()
        {
            CancelCurrentOption(true);
        }
        /// <summary>
        /// 取消当前选择
        /// </summary>
        /// <param name="item"></param>
        public virtual void CancelCurrentOption(bool sendEvent)
        {
            CancelCurrentData(sendEvent);
            CancelCurrentToggle();

            // 修改值
            m_Value = m_Value > -1 && m_Value <= Options.Count ? -1 : m_Value;
            //AmendValue();

            RefreshCurrentDataShow();
        }


        /// <summary>
        /// 只根据值刷新显示
        /// <para>以数据为准刷新 UI，不管显没显示</para>
        /// </summary>
        public virtual void RefreshValueShow()
        {
            //SelectData(m_Value);
            SelectOption(m_Value);

        }

        /// <summary>
        /// 勾选时调用
        /// </summary>
        /// <param name="enable"></param>
        /// <param name="data"></param>
        /// <param name="item"></param>
        public virtual void OnToggle(bool enable, DropdownItemData data, DropdownItem item)
        {
            //Log.Striking($"{item}  勾选：{enable}");

            if (enable)// 勾选
            {
                SelectOption(data, item);
            }
            else
            {
                if (data == currentData)
                    CancelCurrentOption();
            }
        }

        #endregion

        #region 只设置数据

        /// <summary>
        /// 根据 <see cref="currentItemData"/> 来修正 <see cref="Value"/> 的值
        /// </summary>
        /// <returns>修正后的值</returns>
        public virtual int AmendValue()
        {
            int curIndex = Options.IndexOf(currentData);
            if (curIndex != -1 && curIndex != m_Value)
            {
                m_Value = curIndex;
            }
            return m_Value;
        }

        /// <summary>
        /// 添加数据
        /// </summary>
        /// <param name="itemData"></param>
        public bool AddData(string text)
        {
            return AddData(new DropdownItemData(text));
        }
        /// <summary>
        /// 添加数据
        /// </summary>
        /// <param name="itemData"></param>
        public bool AddData(DropdownItemData itemData)
        {
            if (Options.Contains(itemData))
            {
                //Debug.Log($"不可重复添加相同数据 {itemData.ItemLabel}");
                return false;
            }

            Options.Add(itemData);
            return true;
        }
        /// <summary>
        /// 添加数据到指定位置
        /// </summary>
        /// <param name="itemData"></param>
        public bool AddData(DropdownItemData itemData, int index)
        {
            if (Options.Contains(itemData))
            {
                //Debug.Log($"不可重复添加相同数据 {itemData.ItemLabel}");
                return false;
            }

            Options.Insert(index, itemData);
            return true;
        }

        public bool RemoveData(int index)
        {
            Options.TryIndex(index, out var item);
            return RemoveData(item);
        }
        public virtual bool RemoveData(DropdownItemData itemData)
        {
            if (itemData == currentData) CancelCurrentData();
            return Options.TryRemove(itemData);
        }
        /// <summary>
        /// 移除所有选项数据
        /// </summary>
        public void RemoveDataAll()
        {
            for (int i = 0; i < Options.Count; i++)
            {
                var item = Options[i];
                RemoveData(item);
            }
        }

        /// <summary>
        /// 设置选项位置
        /// </summary>
        /// <param name="itemData"></param>
        public virtual void SetDataPos(int oldIndex, int posIndex)
        {
            if (!Options.IndexValid(posIndex))
            {
                return;
            }

            if (oldIndex != posIndex)
            {
                Options.MoveIndex(posIndex, oldIndex);
            }
        }
        /// <summary>
        /// 设置选项位置
        /// </summary>
        /// <param name="itemData"></param>
        public virtual void SetDataPos(DropdownItemData item, int posIndex)
        {
            int index = Options.IndexOf(item);

            SetDataPos(index, posIndex);
        }

        /// <summary>
        /// 选择选项
        /// </summary>
        /// <param name="item"></param>
        public void SelectData(int index)
        {
            if (!Options.IndexValid(index))
            {
                CancelCurrentData(false);
                return;
            }

            Options.TryIndex(index, out var itemData);
            //m_Value = index;

            SelectData(itemData);
        }
        /// <summary>
        /// 选择选项
        /// </summary>
        /// <param name="item"></param>
        public virtual void SelectData(DropdownItemData itemData)
        {
            SelectData(itemData, true);
        }
        /// <summary>
        /// 选择选项
        /// </summary>
        /// <param name="item"></param>
        public virtual void SelectData(DropdownItemData itemData, bool sendEvent)
        {
            if (itemData != currentData)
            {
                currentData = itemData;

                AmendValue();

                if (sendEvent)
                {
                    itemData?.SelectedEvent?.Invoke();
                    SelectedEvent_Action();
                }
            }
        }

        /// <summary>
        /// 选择选项
        /// </summary>
        /// <param name="item"></param>
        public virtual void SelectData(Func<DropdownItemData, bool> condition)
        {
            SelectData(condition, true);
        }
        /// <summary>
        /// 选择选项
        /// </summary>
        /// <param name="item"></param>
        public virtual void SelectData(Func<DropdownItemData, bool> condition, bool sendEvent)
        {
            for (int i = 0; i < Options.Count; i++)
            {
                var item = Options[i];
                if ((bool)condition?.Invoke(item))
                {
                    SelectData(item, sendEvent);
                    break;
                }
            }
        }

        /// <summary>
        /// 取消当前选项数据
        /// </summary>
        /// <param name="item"></param>
        public virtual void CancelCurrentData()
        {
            DropdownItemData data = null;
            SelectData(data);
        }
        /// <summary>
        /// 取消当前选项数据
        /// </summary>
        /// <param name="item"></param>
        public virtual void CancelCurrentData(bool sendEvent)
        {
            DropdownItemData data = null;
            SelectData(data, sendEvent);
        }

        /// <summary>
        /// 执行 <see cref="SelectedEvent"/> ，发送 <see cref="currentData"/>
        /// </summary>
        protected virtual void SelectedEvent_Action()
        {
            SelectedEvent?.Invoke(currentData);
        }

        #endregion

        #region 只更改 UI

        // 创建菜单项实体
        protected virtual DropdownItem CreateOptionItem(DropdownItem item)
        {
            if (item == null || item.transform)
            {
                return item;
            }

            Transform menuItemParent = m_ScrollRect?.content;// 选项父节点
            //GameObject itemTemplate = m_ItemText?.transform.parent.gameObject;// 选项模板
            GameObject itemTemplate = ItemTemplate;// 选项模板

            if (!menuItemParent || !itemTemplate)
            {
                return null;
            }

            var _data = item.itemData;
            //var item = new DropdownItem(_data, Instantiate(itemTemplate, menuItemParent));// 直接创建

            // 预创建
            precreator.Add(() =>
            {
                if (!menuItemParent || !itemTemplate)
                {
                    return;
                }

                var _itemMenu = Instantiate(itemTemplate, menuItemParent);

                // 设置自己的位置
                int index = Options.IndexOf(_data);
                _itemMenu.transform.SetSiblingIndex(_itemMenu.transform.GetSiblingIndex() >= menuItemParent.transform.childCount ? _itemMenu.transform.childCount - 1 : index);

                item.SetItem(_itemMenu);

                item.isOn = currentData == _data;

                // 添加开关事件
                item.toggle.onValueChanged.AddListener((enable) =>
                {
                    //Debug.Log($"选项 {item.transform.name}：{enable}");

                    OnToggle(enable, _data, item);// 勾选

                });

                // 定位当前项位置
                LocateCurrent();

                // 设置样式
                sytleGroup.SetSytle(item, index);

            });

            return item;
        }
        protected DropdownItem CreateOptionItem(DropdownItemData itemData)
        {
            return CreateOptionItem(itemData, false);
        }
        protected virtual DropdownItem CreateOptionItem(DropdownItemData itemData, bool onCreateMenuItem)
        {
            var item = new DropdownItem(itemData);
            CreateOptionItem(item);
            return item;
        }

        /// <summary>
        /// 添加选项
        /// </summary>
        /// <param name="itemData"></param>
        public virtual DropdownItem AddItem(DropdownItemData itemData)
        {
            return AddItem(itemData, false);
        }
        /// <summary>
        /// 添加选项
        /// </summary>
        /// <param name="itemData"></param>
        public virtual DropdownItem AddItem(DropdownItemData itemData, bool onCreateMenuItem)
        {
            var item = CreateOptionItem(itemData, onCreateMenuItem);

            if (item != null)
            {
                OptionItems.Add(item);
            }

            return item;
        }
        /// <summary>
        /// 添加选项到指定位置
        /// </summary>
        /// <param name="itemData"></param>
        public virtual DropdownItem AddItem(DropdownItemData itemData, int index)
        {
            return AddItem(itemData, index, false);
        }
        /// <summary>
        /// 添加选项到指定位置
        /// </summary>
        /// <param name="itemData"></param>
        public virtual DropdownItem AddItem(DropdownItemData itemData, int index, bool onCreateMenuItem)
        {
            var item = AddItem(itemData, onCreateMenuItem);

            if (item != null)
            {
                SetItemPos(item, index);
            }

            return item;
        }


        public virtual bool RemoveItem(int index)
        {
            var item = OptionItems.TryIndex(index);

            return RemoveItem(item);
        }
        public virtual bool RemoveItem(DropdownItemData itemData)
        {
            var item = OptionItems.Find((_item) => _item.itemData == itemData);
            return RemoveItem(item);
        }
        public virtual bool RemoveItem(DropdownItem item)
        {
            if (item != null)
            {
                item.Destroy();
            }
            return OptionItems.TryRemove(item);
        }
        /// <summary>
        /// 移除所有选项
        /// </summary>
        public virtual void RemoveItemAll()
        {
            for (int i = 0; i < OptionItems.Count; i++)
            {
                var item = OptionItems[i];
                RemoveItem(item);
            }
        }

        /// <summary>
        /// 设置选项位置
        /// </summary>
        /// <param name="itemData"></param>
        public virtual void SetItemPos(int oldIndex, int posIndex)
        {
            OptionItems.TryIndex(oldIndex, out var item);
            SetItemPos(item, posIndex);
        }
        /// <summary>
        /// 设置选项位置
        /// </summary>
        /// <param name="itemData"></param>
        public virtual void SetItemPos(DropdownItem item, int posIndex)
        {
            Transform menuItemParent = m_ScrollRect?.content;// 选项父节点

            if (!OptionItems.IndexValid(posIndex) || !menuItemParent || posIndex >= menuItemParent.childCount)
            {
                return;
            }

            // 位置不相同
            if (item.transform.GetSiblingIndex() != posIndex)
            {
                item.transform.SetSiblingIndex(posIndex);
            }

            int index = OptionItems.IndexOf(item);

            if (index != posIndex)
            {
                OptionItems.Move(posIndex, item);
            }
        }

        /// <summary>
        /// 设置选择勾选
        /// </summary>
        /// <param name="item"></param>
        public void SclectToggle(int index)
        {
            OptionItems.TryIndex(index, out var item);
            SclectToggle(item);
        }
        /// <summary>
        /// 设置选择勾选
        /// </summary>
        /// <param name="item"></param>
        public void SclectToggle(DropdownItemData itemData)
        {
            var item = OptionItems.Find((_item) => _item.itemData == itemData);
            SclectToggle(item);
        }
        /// <summary>
        /// 设置选择勾选
        /// </summary>
        /// <param name="item"></param>
        public virtual void SclectToggle(DropdownItem item)
        {
            if (item == currentItem)
            {
                return;
            }

            CancelCurrentToggle();// 先取消之前的勾选

            currentItem = item;

            if (currentItem != null)
            {
                currentItem.isOn = true;
            }
        }
        /// <summary>
        /// 取消当前勾选
        /// </summary>
        /// <param name="item"></param>
        public virtual void CancelCurrentToggle()
        {
            DropdownItem oldDI = currentItem;

            if (oldDI != null)
            {
                oldDI.isOn = false;
            }

            currentItem = null;
        }

        /// <summary>
        /// 显示菜单
        /// </summary>
        public virtual void ShowMenu()
        {
            bool isShow = dataListMenu ? !dataListMenu.gameObject.activeSelf : true;
            ShowMenu(isShow);
        }
        /// <summary>
        /// 显示菜单
        /// </summary>
        /// <param name="isShow"></param>
        public virtual void ShowMenu(bool isShow)
        {
            if (IsShowMenu == isShow)
            {
                return;
            }

            // 没有就创建
            CreateMenu();

            dataListMenu.gameObject.SetActive(isShow);
            m_Blocker.SetActive(isShow);

            // 清除已有任务
            precreator.Clear();
            precleaner.Clear();

            // 显示的时候刷新选项
            if (isShow)
            {
                RefreshShow();
            }
            else
            {
                for (int i = OptionItems.Count - 1; i >= 0; i--)
                {
                    if (i <= itemReserveMax) break;

                    var item = OptionItems[i];
                    if (item == null || !item.gameObject) continue;
                    // 预清除
                    precleaner.Add(() =>
                    {
                        item.Destroy();
                    });
                }
            }

            MenuEnableEvent?.Invoke(isShow);
        }

        /// <summary>
        /// 创建菜单
        /// </summary>
        protected virtual void CreateMenu()
        {
            if (!dataListMenu)
            {
                if (!m_Template) return;

                Transform parent = m_Template.parent;
                //Transform parent = targetGraphic?.canvas.transform;

                dataListMenu = Instantiate(m_Template, parent).GetComponent<RectTransform>();
                dataListMenu.gameObject.SetActive(true);

                dataListMenu.name = $"{name} (Dropdown Item List)";

                m_ScrollRect = dataListMenu.GetComponent<ScrollRect>();


                // 找到这个下拉菜单所在的画布
                Canvas parentCanvas = rootCanvas;

                var _canvas = dataListMenu.gameObject.AddComponent<Canvas>();
                _canvas.overrideSorting = true;
                _canvas.sortingOrder = 30000;
                // popupCanvas used to assume the root canvas had the default sorting Layer, next line fixes (case 958281 - [UI] Dropdown list does not copy the parent canvas layer when the panel is opened)
                _canvas.sortingLayerID = parentCanvas.sortingLayerID;

                // 如果我们有一个父画布，为一致性应用相同的光线投射。
                SetGraphicRaycaster(dataListMenu.gameObject, parentCanvas);
            }

            CreateBlocker();
        }


        /// <summary>
        /// Create a blocker that blocks clicks to other controls while the dropdown list is open.
        /// </summary>
        /// <remarks>
        /// Override this method to implement a different way to obtain a blocker GameObject.
        /// </remarks>
        /// <returns>The created blocker object</returns>
        protected virtual GameObject CreateBlocker()
        {
            if (m_Blocker) return m_Blocker;

            // Create blocker GameObject.
            GameObject blocker = new GameObject("Blocker");
            blocker.SetActive(true);
            m_Blocker = blocker;

            // 找到这个下拉菜单所在的画布
            Canvas parentCanvas = rootCanvas;

            // Setup blocker RectTransform to cover entire root canvas area.
            RectTransform blockerRect = blocker.AddComponent<RectTransform>();
            //Transform parent = dataListMenu.transform;
            Transform parent = parentCanvas ? parentCanvas.transform : dataListMenu.transform;
            blockerRect.SetParent(parent, false);
            blockerRect.anchorMin = Vector3.zero;
            blockerRect.anchorMax = Vector3.one;
            blockerRect.sizeDelta = Vector2.zero;

            // Make blocker be in separate canvas in same layer as dropdown and in layer just below it.
            RefreshBlocker(false);

            // 如果我们有一个父画布，为一致性应用相同的光线投射。
            SetGraphicRaycaster(blocker, parentCanvas);

            // Add image since it's needed to block, but make it clear.
            Image blockerImage = blocker.AddComponent<Image>();
            blockerImage.color = Color.clear;

            // Add button since it's needed to block, and to close the dropdown when blocking area is clicked.
            Button blockerButton = blocker.AddComponent<Button>();
            blockerButton.onClick.AddListener(() =>
            {
                ShowMenu(false);
            });

            return blocker;
        }

        /// <summary>
        /// 设置光线投射
        /// </summary>
        /// <param name="target"></param>
        /// <param name="_canvas"></param>
        protected virtual void SetGraphicRaycaster(GameObject target, Canvas _canvas)
        {
            Canvas parentCanvas = _canvas ? _canvas : rootCanvas;

            if (parentCanvas != null)
            {
                Component[] components = parentCanvas.GetComponents<BaseRaycaster>();
                for (int i = 0; i < components.Length; i++)
                {
                    Type raycasterType = components[i].GetType();
                    if (target.GetComponent(raycasterType) == null)
                    {
                        target.AddComponent(raycasterType);
                    }
                }
            }
            else
            {
                // Add raycaster since it's needed to block.
                target.AddComponent<GraphicRaycaster>();
            }
        }

        /// <summary>
        /// 定位到当前选项
        /// </summary>
        public virtual void LocateCurrent()
        {
            LocateTarget(currentItem);
        }
        /// <summary>
        /// 定位到目标选项
        /// </summary>
        /// <param name="item"></param>
        public virtual bool LocateTarget(int itemIndex)
        {
            return LocateTarget(OptionItems.TryIndex(itemIndex));
        }
        /// <summary>
        /// 定位到目标选项
        /// </summary>
        /// <param name="item"></param>
        public virtual bool LocateTarget(DropdownItem item)
        {
            if (!OptionItems.Contains(item)) return false;
            if (!item.gameObject)
            {
                //Log.Error($"未创建选项实例：{item}");
                return false;
            }

            ScrollRect sr = m_ScrollRect;
            RectTransform rt = dataListMenu;

            float locationProportion = 0.3f;// 要显示的位置比例
            float practicalLocation = rt.position.y * locationProportion;// 实际位置

            Vector3 pos = sr.content.position;
            pos.y = sr.content.position.y - (item.rectTransform.position.y - rt.position.y) - practicalLocation;
            sr.content.position = pos;
            return true;
        }

        /// <summary>
        /// 根据菜单实体刷新背景阻断
        /// </summary>
        /// <param name="isShow"></param>
        protected virtual void RefreshBlocker(bool isShow)
        {
            Canvas blockerCanvas = m_Blocker.ExpectComponent<Canvas>();
            blockerCanvas.overrideSorting = true;

            if (dataListMenu)
            {
                Canvas dropdownCanvas = dataListMenu.GetComponent<Canvas>();
                blockerCanvas.sortingLayerID = dropdownCanvas.sortingLayerID;
                blockerCanvas.sortingOrder = dropdownCanvas.sortingOrder - 1;

            }

            m_Blocker.SetActive(isShow);
        }

        /// <summary>
        /// 刷新当前勾选
        /// </summary>
        public virtual void RefreshCurrentToggleShow()
        {
            if (currentItem == null || currentItem.itemData != currentData)
            {
                SclectToggle(currentData);
            }
            else
            {
                currentItem.isOn = true;
            }
        }

        /// <summary>
        /// 只根据当前的数据刷新显示
        /// <para>以数据为准刷新 UI，不管显没显示</para>
        /// </summary>
        public virtual void RefreshCurrentDataShow()
        {
            if (currentData != null)
            {
                if (m_CaptionText)
                {
                    captionText_text = currentData.text;
                }

                if (m_Placeholder) m_Placeholder.gameObject.SetActive(false);
            }
            else
            {
                if (m_CaptionText)
                {
                    captionText_text = "";
                }

                if (m_Placeholder) m_Placeholder.gameObject.SetActive(true);
            }
        }

        /// <summary>
        /// 刷新菜单显示
        /// <para>以数据为准刷新 UI，不管显没显示</para>
        /// </summary>
        public virtual void RefreshMenu()
        {
            if (dataListMenu)
            {
                int count = Mathf.Max(OptionItems.Count, Options.Count);// 选两个列表中最大的

                for (int i = 0; i < count; i++)
                {
                    // 获取对应位置的数据、选项
                    bool dataExist = Options.TryIndex(i, out var data);
                    bool itemExist = OptionItems.TryIndex(i, out var item);

                    //if (OptionItems.Count > i)
                    //{
                    //    var _data = Options[i];
                    //    var _item = OptionItems[i];
                    //    var _Idata = _item.itemData;
                    //    Debug.Log($"等于：{_data == _Idata}");
                    //    Debug.Log($"相等：{_data.Equals(_Idata)}");
                    //}

                    // 检查数据和选项
                    if (dataExist && !itemExist)
                    {
                        // 有数据无选项则创建
                        // 此种情况说明选项不够多，新添加到最后没毛病
                        AddItem(data, true);
                    }
                    else if (!dataExist && itemExist)
                    {
                        // 没有数据说明此项已删除
                        item.Destroy();
                        //OptionItems.Remove(item);
                        OptionItems[i] = null;
                    }
                    else
                    {
                        // 不存在都不存在的情况
                        // 走到这里说明数据和选项都存在
                        // 以数据项位置为准做排序

                        //if (item.itemData == data)// 检查数据项是否对应
                        if (item.itemData.Equals(data))// 检查数据项是否对应
                        {

                            // 都能对应上
                            // 不需要做任何操作

                        }
                        else
                        {
                            // 不对应则挪动选项位置

                            // 查找数据对应的选项
                            var sItem = OptionItems.Find(item => item.itemData == data);

                            if (sItem == null)
                            {
                                // 此位置都不为空，但没有和数据对应的选项
                                // 则在此位置创建一个新的选项
                                var _data = data;
                                AddItem(_data, i, true);
                            }
                            else
                            {
                                // 找到对应选项，则调整其排序
                                SetItemPos(sItem, i);
                            }
                        }
                    }

                }

                // 清除空项
                for (int i = 0; i < OptionItems.Count; i++)
                {
                    if (OptionItems[i] == null)
                    {
                        OptionItems.RemoveAt(i);
                        i--;
                    }
                }


                // 为避免重复创建相同菜单项，这里必须清除
                precreator.Clear();
                // 创建菜单选项实体
                foreach (var item in OptionItems)
                {
                    CreateOptionItem(item);
                }
            }

            // 如果当前选项数据是空的，则指定为 Value 对应的数据
            if (currentData == null)
            {
                //SelectOption(m_Value);
                RefreshValueShow();
            }

            // 定位当前项位置
            LocateCurrent();

            //SclectToggle(currentItemData);
            RefreshCurrentToggleShow();
        }

        /// <summary>
        /// 刷新整体显示
        /// <para>菜单和当前选项</para>
        /// <para>以数据为准刷新 UI，不管显没显示</para>
        /// </summary>
        public virtual void RefreshShow()
        {
            // 刷新 UI 菜单显示
            RefreshMenu();

            //SelectItem(m_Value);

            AmendValue();

            // 根据数据刷新
            // 刷新当前显示
            RefreshCurrentDataShow();
        }

        /// <summary>
        /// 刷新菜单项背景颜色显示
        /// </summary>
        public virtual void RefreshSytle()
        {
            m_SytleGroup.SetSytle(OptionItems);
        }


        /// <summary>
        /// Convenience method to explicitly destroy the previously generated blocker object
        /// </summary>
        /// <remarks>
        /// Override this method to implement a different way to dispose of a blocker GameObject that blocks clicks to other controls while the dropdown list is open.
        /// </remarks>
        protected virtual void DestroyBlocker()
        {
            GameObject blocker = m_Blocker;
            if (blocker) Destroy(blocker);
            m_Blocker = null;
        }

        #endregion


        // 下拉列表选项
        [Serializable]
        public class DropdownItem
        {
            /// <summary>
            /// 开关
            /// </summary>
            public Toggle m_Toggle;
            /// <summary>
            /// 选项显示
            /// </summary>
            public Text m_ItemLabel;
            /// <summary>
            /// 选项背景
            /// </summary>
            public Image bg;

            [SerializeField] protected DropdownItemData m_ItemData;

            /// <summary>
            /// 选项数据
            /// </summary>
            public DropdownItemData itemData
            {
                get => m_ItemData;
                set
                {
                    //Debug.Log($"设置前：{m_ItemData}\t设置后：{value}");
                    m_ItemData = value;
                }

            }

            public string itemLabel_text
            {
                get
                {
                    return TextUtility.GetText(m_ItemLabel);
                }
                set
                {
                    TextUtility.SetText(m_ItemLabel, value);
                }
            }

            public Toggle toggle
            {
                get
                {
                    return m_Toggle;
                }
                protected set
                {
                    m_Toggle = value;
                }
            }

            public virtual Transform transform
            {
                get => toggle?.transform;
            }

            public virtual GameObject gameObject
            {
                get => toggle?.gameObject;
            }

            public virtual RectTransform rectTransform
            {
                get; protected set;
            }

            public virtual bool isOn
            {
                get
                {
                    return m_Toggle ? m_Toggle.isOn : false;
                }
                set
                {
                    if (m_Toggle && m_Toggle.isOn != value)
                    {

                        m_Toggle.isOn = value;
                    }
                }
            }

            public DropdownItem() { }

            public DropdownItem(DropdownItemData itemData)
            {
                this.itemData = itemData;
            }

            public DropdownItem(DropdownItemData itemData, GameObject item) : this(itemData)
            {
                SetItem(item);
            }

            public virtual void SetItem(GameObject item)
            {
                if (item)
                {
                    item.SetActive(true);
                    rectTransform = item.GetComponent<RectTransform>();
                    this.toggle = item.GetComponent<Toggle>();
                    this.m_ItemLabel = item.transform.FindOf<Text>("Item Label");
                    this.bg = item.transform.FindOf<Image>("Item Background");

                    item.name = itemData.text;
                    itemLabel_text = itemData.text;
                }
            }

            /// <summary>
            /// 销毁菜单实体
            /// </summary>
            public virtual void Destroy()
            {
                if (gameObject)
                {
                    Object.Destroy(gameObject);

                    toggle = null;
                    m_ItemLabel = null;
                }
            }

            public override string ToString()
            {
                return itemData.ToString();
            }
        }

        // 下拉列表选项数据
        [Serializable]
        public class DropdownItemData
        {
            [SerializeField] protected string m_Text;

            /// <summary>
            /// 选中事件
            /// </summary>
            public Action SelectedEvent;

            /// <summary>
            /// 选项显示
            /// </summary>
            public string text { get => m_Text; set => m_Text = value; }

            public DropdownItemData(string text)
            {
                this.text = text;
            }

            public DropdownItemData()
            {

            }

            public override string ToString()
            {
                return text;
            }
        }


        /// <summary>
        /// 样式组
        /// </summary>
        [Serializable]
        public class SytleGroup
        {
            [SerializeField]
            private bool enable = true;

            [SerializeField]
            private List<DropdownItemSytle> sytles = new List<DropdownItemSytle>() {

                new DropdownItemSytle(Color.white),
                new DropdownItemSytle(new Color(242 / 255f, 242 / 255f, 242 / 255f)),
            };

            public bool Enable { get => enable; set => enable = value; }
            public List<DropdownItemSytle> Sytles { get => sytles; set => sytles = value; }

            /// <summary>
            /// 设置样式
            /// </summary>
            /// <param name="items"></param>
            public virtual void SetSytle(List<DropdownItem> items)
            {
                if (!enable) return;
                if (sytles.Count <= 0) return;

                var _items = new List<DropdownItem>();
                for (int i = 0; i < items.Count; i++)
                {
                    var item = items[i];
                    if (item.gameObject)
                        items.Add(item);
                }

                for (int i = 0; i < _items.Count; i++)
                {
                    var item = _items[i];
                    SetSytle(item, i);
                }
            }

            /// <summary>
            /// 设置样式
            /// </summary>
            /// <param name="item">目标</param>
            /// <param name="itemPlace">目标所在地</param>
            public virtual void SetSytle(DropdownItem item, int itemPlace)
            {
                if (!enable) return;
                if (sytles.Count <= 0) return;

                var sytle = sytles[itemPlace % sytles.Count];
                item.bg.color = sytle.color;
            }
        }

        /// <summary>
        /// 下拉菜单样式
        /// </summary>
        [Serializable]
        public class DropdownItemSytle
        {
            public Color color = Color.white;

            public DropdownItemSytle(Color color)
            {
                this.color = color;
            }

            public DropdownItemSytle()
            {

            }
        }


    }


    //#if UNITY_EDITOR
    //    using UnityEditor;
    //    using UnityEditor.UI;

    //    [CustomEditor(typeof(DropdownPro), true)]
    //    [CanEditMultipleObjects]
    //    class DropdownProInspector : SelectableEditor
    //    {
    //        DropdownPro my = null;

    //        int value = -1;

    //        private void Awake()
    //        {
    //            my = (DropdownPro)target;
    //            value = my.Value;
    //        }

    //        public override void OnInspectorGUI()
    //        {
    //            base.OnInspectorGUI();

    //            serializedObject.Update();

    //            DropdownPro my = (DropdownPro)target;

    //            GUILayout.Space(16);

    //            if (my.Value != value)
    //            {
    //                my.RefreshShow();
    //            }

    //            if (GUILayout.Button("刷新显示", GUILayout.MinHeight(32)))
    //            {
    //                my.RefreshShow();
    //            }
    //        }
    //    }
    //#endif

}