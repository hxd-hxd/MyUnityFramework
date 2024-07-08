// -------------------------
// 创建日期：2024/4/2 9:25:57
// -------------------------

using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;

namespace Framework
{
#if UNITY_EDITOR
    using UnityEditor;

    [CustomEditor(typeof(DragUI))]
    [CanEditMultipleObjects]
    public partial class DragUIInspector : Editor
    {
        protected static bool _showDragGUI = true;
        protected static bool _showMidpointGUI = true;
        protected static bool _showDragRestrictZoneGUI = true;

        protected GUIStyle labelGUIStyle = new GUIStyle();
        protected GUIStyle labelGUIStyle1 = new GUIStyle();
        protected GUIStyle labelGUIStyle2 = new GUIStyle();
        protected GUIStyle labelGUIStyle2Red = new GUIStyle();
        protected GUIStyle labelGUIStyle3 = new GUIStyle();

        RectHandles _dragTragetRectHandles, _dragRestrictZoneRectHandles;

        DragUI my => (DragUI)target;

        protected virtual void OnEnable()
        {
            labelGUIStyle.normal.textColor = Color.green;

            ColorUtility.TryParseHtmlString("#EAEAEA", out var color);
            labelGUIStyle1.normal.textColor = color;
            labelGUIStyle1.alignment = TextAnchor.MiddleCenter;

            labelGUIStyle2.normal.textColor = Color.green;
            labelGUIStyle2.alignment = TextAnchor.LowerCenter;
            labelGUIStyle2Red.normal.textColor = Color.red;
            labelGUIStyle2Red.alignment = TextAnchor.LowerCenter;
            //Color color1 = Color.black;

            labelGUIStyle3.normal.textColor = Color.yellow;
            labelGUIStyle3.alignment = TextAnchor.UpperCenter;

            _dragTragetRectHandles ??= new RectHandles();
            _dragRestrictZoneRectHandles ??= new RectHandles();

            //HandleUtility.placeObjectCustomPasses -= OnPlaceObjectDelegate;
            //HandleUtility.placeObjectCustomPasses += OnPlaceObjectDelegate;
            //SceneView
        }

        private void OnDisable()
        {
            //HandleUtility.placeObjectCustomPasses -= OnPlaceObjectDelegate;
        }

        bool OnPlaceObjectDelegate(Vector2 guiPosition, out Vector3 position, out Vector3 normal)
        {
            position = guiPosition; normal = guiPosition.normalized;
            Debug.Log(guiPosition);
            return true;
        }

        protected virtual void OnSceneGUI()
        {
            if (!my.target) return;

            var detectRectOffsetX = my.GetDetectRectOffsetX();
            var detectRectOffsetY = my.GetDetectRectOffsetY();

            my.target.GetRectPrimaryVertex(out var vcPos, out var leftDown, out var leftUp, out var rightDown, out var rightUp);
            leftDown.x -= detectRectOffsetX.min;
            leftDown.y -= detectRectOffsetY.min;
            leftUp.x -= detectRectOffsetX.min;
            leftUp.y += detectRectOffsetY.max;
            rightUp.x += detectRectOffsetX.max;
            rightUp.y += detectRectOffsetY.max;
            rightDown.x += detectRectOffsetX.max;
            rightDown.y -= detectRectOffsetY.min;

            _dragTragetRectHandles.leftDown = leftDown;
            _dragTragetRectHandles.leftUp = leftUp;
            _dragTragetRectHandles.rightDown = rightDown;
            _dragTragetRectHandles.rightUp = rightUp;

            if (_showDragGUI)
            {
                // 越界限制
                bool isRestrictLeft = my.isDraggingRestrict && my.isDraggingRestrictLeft;
                bool isRestrictRight = my.isDraggingRestrict && my.isDraggingRestrictRight;
                bool isRestrictUp = my.isDraggingRestrict && my.isDraggingRestrictUp;
                bool isRestrictDown = my.isDraggingRestrict && my.isDraggingRestrictDown;
                string restrictDisableStr = "<color=red>（禁用限制）</color>";

                /// scene 控件
                // 标题
                Handles.Label(new Vector2(_dragTragetRectHandles.upCenter.x, _dragTragetRectHandles.upCenter.y + 16), $"DragUI：拖拽隔离区偏移边界{(my.detectRectOffsetUseRatio ? $"（偏移使用比例）" : null)}", labelGUIStyle3);

                // 矩形视觉中心点
                if (_showMidpointGUI)
                {
                    Handles.Label(vcPos, $"RectTransform 中点坐标\r\n{vcPos}", labelGUIStyle);
                    Handles.color = Color.green;
                    Handles.DotHandleCap(0, vcPos, my.target.rotation, 1, EventType.Repaint);
                }

                // 边界描述
                Handles.Label(_dragTragetRectHandles.leftCenter,
                    $"x min{(!isRestrictLeft ? restrictDisableStr : null)}\r\n（{(my.detectRectOffsetUseRatio ? $"{my.detectRectOffsetMinRatio.x}：" : null)}{detectRectOffsetX.min}）", labelGUIStyle2);
                Handles.Label(_dragTragetRectHandles.rightCenter,
                    $"x max{(!isRestrictRight ? restrictDisableStr : null)}\r\n（{(my.detectRectOffsetUseRatio ? $"{my.detectRectOffsetMaxRatio.x}：" : null)}{detectRectOffsetX.max}）", labelGUIStyle2);
                Handles.Label(_dragTragetRectHandles.downCenter,
                    $"y min{(!isRestrictDown ? restrictDisableStr : null)}\r\n（{(my.detectRectOffsetUseRatio ? $"{my.detectRectOffsetMinRatio.y}：" : null)}{detectRectOffsetY.min}）", labelGUIStyle2);
                Handles.Label(_dragTragetRectHandles.upCenter,
                    $"y max{(!isRestrictUp ? restrictDisableStr : null)}\r\n（{(my.detectRectOffsetUseRatio ? $"{my.detectRectOffsetMaxRatio.y}：" : null)}{detectRectOffsetY.max}）", labelGUIStyle2);

                _dragTragetRectHandles.rotation = my.target.rotation;
                _dragTragetRectHandles.OnSceneGUI();

                // 控制
                //EditorGUI.BeginChangeCheck();
                //var lcp = Handles.DoPositionHandle(leftCenter, Quaternion.identity);
                //if (EditorGUI.EndChangeCheck())
                //{
                //    Debug.Log($"{leftCenter}，{lcp}");
                //    Undo.RecordObject(my, "Change detectRectOffsetMin.x");
                //    var droc = _detectRectOffsetMin;
                //    droc.x += lcp.x - leftCenter.x;
                //    my.detectRectOffsetMin = droc;
                //    Debug.Log(lcp.x - leftCenter.x);
                //}

                /// 拖拽限制区
                if (_showDragRestrictZoneGUI)
                {

                    my.GetDragRestrictZone(out var drzX, out var drzY);
                    _dragRestrictZoneRectHandles.leftDown = new Vector3(drzX.min, drzY.min);
                    _dragRestrictZoneRectHandles.leftUp = new Vector3(drzX.min, drzY.max);
                    _dragRestrictZoneRectHandles.rightDown = new Vector3(drzX.max, drzY.min);
                    _dragRestrictZoneRectHandles.rightUp = new Vector3(drzX.max, drzY.max);

                    _dragRestrictZoneRectHandles.handlesColorSide = Color.red;
                    _dragRestrictZoneRectHandles.showSideLeft = isRestrictLeft;
                    _dragRestrictZoneRectHandles.showSideRight = isRestrictRight;
                    _dragRestrictZoneRectHandles.showSideDown = isRestrictDown;
                    _dragRestrictZoneRectHandles.showSideUp = isRestrictUp;
                    _dragRestrictZoneRectHandles.OnSceneGUI();

                    var _dragRestrictZoneLabelGUIStyle = new GUIStyle(labelGUIStyle3);
                    _dragRestrictZoneLabelGUIStyle.alignment = TextAnchor.UpperLeft;
                    Handles.Label(new Vector2(_dragRestrictZoneRectHandles.leftUp.x, _dragRestrictZoneRectHandles.upCenter.y), $"DragUI：拖拽限制区边界（{my.dragRestrictZoneMode}）", _dragRestrictZoneLabelGUIStyle);
                }
            }

            Handles.BeginGUI();
            EditorGUILayout.BeginVertical("Badge", GUILayout.MaxWidth(150));
            //EditorGUILayout.BeginVertical(GUI.skin.box, GUILayout.MaxWidth(150));
            //EditorGUILayout.BeginVertical(EditorStyles.helpBox, GUILayout.MaxWidth(150));
            {

                _showDragGUI = GUILayout.Toggle(_showDragGUI, "显示拖拽编辑");
                //EditorGUI.indentLevel += 1;
                //EditorGUI.indentLevel = 1;
                _showMidpointGUI = GUILayout.Toggle(_showMidpointGUI, $"显示拖拽目标中点坐标");
                //EditorGUI.indentLevel -= 1;
                //EditorGUI.indentLevel =0;
                _showDragRestrictZoneGUI = GUILayout.Toggle(_showDragRestrictZoneGUI, $"显示拖拽限制区");
            }
            EditorGUILayout.EndVertical();
            Handles.EndGUI();
        }



        void SetHandlesColor(Color color, Action action)
        {
            var oldColor = Handles.color;
            Handles.color = color;
            action?.Invoke();
            Handles.color = oldColor;
        }

        void _TestShowMidpoint()
        {
            //for (int i = 0; i < my.v2s.Count; i++)
            //{
            //    for (int j = i + 1; j < my.v2s.Count - 1; j++)
            //    {

            //    }
            //}
        }

        /// <summary>
        /// 矩形控件，由矩形顶点控制
        /// </summary>
        public class RectHandles
        {
            public Quaternion rotation;
            public Vector3 leftDown, leftUp, rightDown, rightUp;
            public bool showSideLeft = true;
            public bool showSideRight = true;
            public bool showSideDown = true;
            public bool showSideUp = true;

            public Color handlesColor = Color.white;
            public Color handlesColorSide = Color.green;
            public Color handlesColorSideCenter = Color.green;

            // 四边中点
            public Vector3 leftCenter => leftDown.Midpoint(leftUp);
            public Vector3 rightCenter => rightDown.Midpoint(rightUp);
            public Vector3 downCenter => leftDown.Midpoint(rightDown);
            public Vector3 upCenter => leftUp.Midpoint(rightUp);

            public virtual void OnSceneGUI()
            {
                Color oldColor = Handles.color;
                Handles.color = handlesColor;

                // 连接顶点线段，构成矩形边界
                Handles.color = handlesColorSide;
                if (showSideLeft) Handles.DrawLine(leftDown, leftUp);
                if (showSideDown) Handles.DrawLine(leftDown, rightDown);
                if (showSideRight) Handles.DrawLine(rightDown, rightUp);
                if (showSideUp) Handles.DrawLine(rightUp, leftUp);
                //var rPos = pos;
                //rPos.x -= rect.width / 2;
                //rect.position = rPos;
                //Handles.DrawSolidRectangleWithOutline(rect, new Color(1,1,1, 0), Color.white);
                Handles.color = handlesColor;

                // 边界中点
                Handles.color = handlesColorSideCenter;
                Handles.DotHandleCap(0, leftCenter, rotation, 1, EventType.Repaint);
                Handles.DotHandleCap(0, rightCenter, rotation, 1, EventType.Repaint);
                Handles.DotHandleCap(0, downCenter, rotation, 1, EventType.Repaint);
                Handles.DotHandleCap(0, upCenter, rotation, 1, EventType.Repaint);
                Handles.color = handlesColor;
                //HandleUtility
                Handles.color = oldColor;
            }
        }
    }
#endif

    /// <summary>
    /// 拖拽 UI
    /// </summary>
    //[ExecuteInEditMode]
    [ExecuteAlways]
    public class DragUI
        : MonoBehaviour
    {
        [Tooltip("拖拽目标")]
        [SerializeField] private RectTransform _target;
        [Tooltip("拖拽检测区域")]
        [SerializeField] private RectTransform _detectRectTransform;// 检测区域

        private EventTrigger _detectEventTrigger;
        private Vector2 _pointerInitPos, _targetInitPos;// 拖拽开始时记录初始位置

        // 这里是对拖拽目标的限制，不是拖拽限制区
        [Header("对目标的拖拽限制（注意：均不考虑旋转）")]
        [Tooltip("是否限制拖拽目标")]
        [SerializeField] private bool _isDraggingRestrict;
        [SerializeField] private bool _isDraggingRestrictUp = true;
        [SerializeField] private bool _isDraggingRestrictDown = true;
        [SerializeField] private bool _isDraggingRestrictLeft = true;
        [SerializeField] private bool _isDraggingRestrictRight = true;
        //public Rect _detectRectOffset;
        [Header("检测拖拽矩形的偏移（负值向内，正值向外）")]
        [SerializeField] private Vector2 _detectRectOffsetMin;
        [SerializeField] private Vector2 _detectRectOffsetMax;
        [Header("偏移值使用百分比（1 = 100%）")]
        [SerializeField] private bool _detectRectOffsetUseRatio = true;
        [SerializeField] private Vector2 _detectRectOffsetMinRatio, _detectRectOffsetMaxRatio;

        [Header("拖拽限制区域设置")]
        [Tooltip("将限制区颠倒，即 左、右互换，上、下互换。\r\n此功能仅用于一些特殊用途，例如：使用百分比偏移值时，将偏移全部设置成 -1，保持拖拽矩形比限制区大，这时可以使拖拽区始终包围限制区。")]
        [SerializeField] private bool _dragRestrictZoneReversal;// 将限制区颠倒
        [SerializeField] private DragRestrictZoneMode _dragRestrictZoneMode;
        [SerializeField] private MinMax<float> _dragRestrictZoneX, _dragRestrictZoneY;
        [SerializeField] private RectTransform _dragRestrictZoneTarget;
        //[SerializeField] private MinMax<float>  _dragRestrictZoneY;

        //[Space] public List<Vector2> v2s = new List<Vector2>();

        // 事件
        [Space(16)]
        public UnityEvent beginDragEvent;
        /// <summary>拖拽中事件</summary>
        /// <remarks>参数 0：true：拖拽成功</remarks>
        public UnityEvent<bool> dragEvent;
        public UnityEvent endDragEvent;

        public RectTransform target { get { return _target; } set { _target = value; } }
        public RectTransform detectRectTransform { get { return _detectRectTransform; } set { _detectRectTransform = value; } }
        public bool isDraggingRestrict { get { return _isDraggingRestrict; } set { _isDraggingRestrict = value; } }
        public bool isDraggingRestrictUp { get => _isDraggingRestrictUp; set => _isDraggingRestrictUp = value; }
        public bool isDraggingRestrictDown { get => _isDraggingRestrictDown; set => _isDraggingRestrictDown = value; }
        public bool isDraggingRestrictLeft { get => _isDraggingRestrictLeft; set => _isDraggingRestrictLeft = value; }
        public bool isDraggingRestrictRight { get => _isDraggingRestrictRight; set => _isDraggingRestrictRight = value; }
        public Vector2 detectRectOffsetMin { get { return _detectRectOffsetMin; } set { _detectRectOffsetMin = value; } }
        public Vector2 detectRectOffsetMax { get { return _detectRectOffsetMax; } set { _detectRectOffsetMax = value; } }
        public bool detectRectOffsetUseRatio { get => _detectRectOffsetUseRatio; set => _detectRectOffsetUseRatio = value; }
        public Vector2 detectRectOffsetMinRatio { get => _detectRectOffsetMinRatio; set => _detectRectOffsetMinRatio = value; }
        public Vector2 detectRectOffsetMaxRatio { get => _detectRectOffsetMaxRatio; set => _detectRectOffsetMaxRatio = value; }
        /// <summary>将限制区颠倒，即 左、右互换，上、下互换，此功能仅用于一些特殊用途。例如：使用百分比偏移值时，将偏移全部设置成 -1，保持拖拽矩形比限制区大，这时可以使拖拽区始终包围限制区。</summary>
        public bool dragRestrictZoneReversal { get => _dragRestrictZoneReversal; set => _dragRestrictZoneReversal = value; }
        /// <summary>限制拖拽活动区域的方式</summary>
        public DragRestrictZoneMode dragRestrictZoneMode { get => _dragRestrictZoneMode; set => _dragRestrictZoneMode = value; }
        /// <summary>拖拽限制区域固定值 x 轴，<see cref="dragRestrictZoneMode"/> 为 <see cref="DragRestrictZoneMode.Fixed"/> 时生效</summary>
        public MinMax<float> dragRestrictZoneX { get => _dragRestrictZoneX; set => _dragRestrictZoneX = value; }
        /// <summary>拖拽限制区域固定值 y 轴，<see cref="dragRestrictZoneMode"/> 为 <see cref="DragRestrictZoneMode.Fixed"/> 时生效</summary>
        public MinMax<float> dragRestrictZoneY { get => _dragRestrictZoneY; set => _dragRestrictZoneY = value; }
        /// <summary>拖拽限制区域固定值目标，<see cref="dragRestrictZoneMode"/> 为 <see cref="DragRestrictZoneMode.Target"/> 时生效</summary>
        public RectTransform dragRestrictZoneTarget { get => _dragRestrictZoneTarget; set => _dragRestrictZoneTarget = value; }

        public float targetWidth => target ? target.rect.width * target.lossyScale.x : 0;
        public float targetHeight => target ? target.rect.height * target.lossyScale.y : 0;


        protected virtual void Start()
        {
            Init();
        }

        public virtual void Init()
        {
            if (!_target) _target = GetComponent<RectTransform>();
            if (!_detectRectTransform) _detectRectTransform = GetComponent<RectTransform>();

            if (Application.isPlaying)
            {
                if (_detectRectTransform != null)
                {
                    _detectEventTrigger = _detectRectTransform.ExpectComponent<EventTrigger>();
                }
                if (_detectEventTrigger != null)
                {
                    _detectEventTrigger.AddEvent(EventTriggerType.BeginDrag, OnBeginDrag);
                    _detectEventTrigger.AddEvent(EventTriggerType.EndDrag, OnEndDrag);
                    _detectEventTrigger.AddEvent(EventTriggerType.Drag, OnDrag);
                }
            }

        }

        protected virtual void OnBeginDrag(BaseEventData data)
        {
            if (_target)
            {
                // 指针 按下
                var eData = data as PointerEventData;
                // 记录指针初始位置
                _pointerInitPos = eData.position;
                _targetInitPos = _target.position;
            }

            beginDragEvent?.Invoke();
        }
        protected virtual void OnEndDrag(BaseEventData data)
        {

            endDragEvent?.Invoke();
        }
        protected virtual void OnDrag(BaseEventData data)
        {
            bool canDrag = true;
            for (int _ = 0; _ < 1; _++)// 这里的 for 没有其他作用，仅用于跳出执行条件，以保证后续代码执行
            {
                if (_target)
                {
                    // 如有其他事件接口，则不拖拽
                    var eventSystemHandlers = _target.GetComponents<IEventSystemHandler>();
                    if (eventSystemHandlers != null)
                    {
                        bool hasESHEnabled = false;
                        foreach (var eventSystemHandler in eventSystemHandlers)
                        {
                            if (eventSystemHandler is Behaviour b)
                            {
                                if (b != _detectEventTrigger && b.isActiveAndEnabled)
                                {
                                    hasESHEnabled = true;
                                    break;
                                }
                            }
                        }
                        if (eventSystemHandlers.Length > 1 && hasESHEnabled)
                        {
                            canDrag = false;
                            break;
                        }
                    }

                    // 拖拽
                    var eData = data as PointerEventData;
                    // 计算指针位置差值
                    var pOffsetPos = eData.position - _pointerInitPos;
                    var newPos = pOffsetPos + _targetInitPos;
                    // 添加屏幕内限制，使目标保持在屏幕内
                    // 指针移动差值就是目标移动的位置
                    _target.position = newPos;
                    if (_isDraggingRestrict)
                    {
                        AmendPos();
                    }
                }
            }

            dragEvent?.Invoke(canDrag);
        }

        /// <summary>
        /// 修正位置
        /// <para>无视 <see cref="isDraggingRestrict"/>，但边界的禁用限制依然有效</para>
        /// </summary>
        protected virtual void AmendPos()
        {
            if (_target == null) return;

            var pos = _target.position;

            bool isSOx = SlopOverX(pos, out var sox);
            bool isSOy = SlopOverY(pos, out var soy);
            if (isSOx && !(sox.min != 0 && sox.max != 0))
            {
                if (_isDraggingRestrictRight)
                {
                    pos.x -= sox.max;

                }
                if (_isDraggingRestrictLeft)
                {
                    pos.x -= sox.min;

                }
            }
            if (isSOy && !(soy.min != 0 && soy.max != 0))
            {
                if (_isDraggingRestrictUp)
                {

                    pos.y -= soy.max;
                }
                if (_isDraggingRestrictDown)
                {

                    pos.y -= soy.min;
                }
            }

            _target.position = pos;
        }

        /// <summary>
        /// 检查是否超出屏幕
        /// </summary>
        /// <returns></returns>
        protected bool IsSlopOver(Vector2 pos)
        {
            if (_target == null) return false;

            return SlopOverX(pos, out _) || SlopOverY(pos, out _);
        }

        /// <summary>
        /// 获取通过计算的 x 的偏移
        /// </summary>
        /// <returns></returns>
        public MinMax<float> GetDetectRectOffsetX()
        {
            var offset = GetDetectRectOffsetFixedX();
            if (_detectRectOffsetUseRatio)
            {
                offset = GetDetectRectOffsetRatioX();
            }
            return offset;
        }
        /// <summary>
        /// 获取通过固定值计算的 y 的偏移
        /// </summary>
        /// <returns></returns>
        public MinMax<float> GetDetectRectOffsetFixedX()
        {
            var offset = new MinMax<float>(_detectRectOffsetMin.x, _detectRectOffsetMax.x);
            return offset;
        }
        /// <summary>
        /// 获取通过比例计算的 x 的偏移
        /// </summary>
        /// <returns></returns>
        public MinMax<float> GetDetectRectOffsetRatioX()
        {
            return new MinMax<float>(
                _target.GetWidth() * _detectRectOffsetMinRatio.x
                , _target.GetWidth() * _detectRectOffsetMaxRatio.x
                );
        }

        /// <summary>
        /// 获取通过比例计算的 y 的偏移
        /// </summary>
        /// <returns></returns>
        public MinMax<float> GetDetectRectOffsetY()
        {
            var offset = GetDetectRectOffsetFixedY();
            if (_detectRectOffsetUseRatio)
            {
                offset = GetDetectRectOffsetRatioY();
            }
            return offset;
        }
        /// <summary>
        /// 获取通过固定值计算的 y 的偏移
        /// </summary>
        /// <returns></returns>
        public MinMax<float> GetDetectRectOffsetFixedY()
        {
            var offset = new MinMax<float>(_detectRectOffsetMin.y, _detectRectOffsetMax.y);
            return offset;
        }
        /// <summary>
        /// 获取通过比例计算的 y 的偏移
        /// </summary>
        /// <returns></returns>
        public MinMax<float> GetDetectRectOffsetRatioY()
        {
            return new MinMax<float>(
                _target.GetHeight() * _detectRectOffsetMinRatio.y
                , _target.GetHeight() * _detectRectOffsetMaxRatio.y
                );
        }

        /// <summary>
        /// 检查边界是否超出屏幕
        /// </summary>
        /// <returns></returns>
        protected virtual bool SlopOver(Vector2 pos, out MinMax<float> sovX, out MinMax<float> sovY)
        {
            sovX = default; sovY = default;
            if (_target == null) return false;

            GetDragRestrictZone(out var restrictX, out var restrictY);
            return SlopOver(_target, pos
                , restrictX, restrictY
                , GetDetectRectOffsetX(), GetDetectRectOffsetY()
                , out sovX, out sovY
                );
        }
        /// <summary>
        /// 检查 x 轴边界是否超出屏幕
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="sov"></param>
        /// <returns></returns>
        protected virtual bool SlopOverX(Vector2 pos, out MinMax<float> sov)
        {
            sov = default;
            if (_target == null) return false;

            GetDragRestrictZone(out var restrict, out _);
            return SlopOverX(_target, pos
                , restrict
                , GetDetectRectOffsetX()
                , out sov);
        }
        /// <summary>
        /// 检查 y 轴边界是否超出屏幕
        /// </summary>
        protected virtual bool SlopOverY(Vector2 pos, out MinMax<float> sov)
        {
            sov = default;
            if (_target == null) return false;

            GetDragRestrictZone(out _, out var restrict);
            return SlopOverY(_target, pos
                , restrict
                , GetDetectRectOffsetY()
                , out sov);
        }

        /// <summary>
        /// 检查边界是否超出限制
        /// </summary>
        protected virtual bool SlopOver(RectTransform target, Vector2 pos
            , MinMax<float> restrictX, MinMax<float> restrictY
            , MinMax<float> offsetX, MinMax<float> offsetY
            , out MinMax<float> sovX, out MinMax<float> sovY)
        {
            var reslut = false;
            sovX = default; sovY = default;
            if (target == null) return reslut;

            var pivot = target.pivot;
            float sideW = target.GetWidth();
            float sideH = target.GetHeight();

            bool reslutX = SlopOverAxis(sideW, pivot.x, pos.x, restrictX, offsetX, out sovX);
            bool reslutY = SlopOverAxis(sideH, pivot.y, pos.y, restrictY, offsetY, out sovY);
            reslut = reslutX || reslutY;
            return reslut;
        }
        /// <summary>
        /// 检查 x 轴边界是否超出限制
        /// </summary>
        protected virtual bool SlopOverX(RectTransform target, Vector2 pos, MinMax<float> restrict, MinMax<float> offset, out MinMax<float> sov)
        {
            var reslut = false;
            sov = default;
            if (target == null) return reslut;

            var pivot = target.pivot;
            float side = target.GetWidth();

            return SlopOverAxis(side, pivot.x, pos.x, restrict, offset, out sov);
        }
        /// <summary>
        /// 检查 y 轴边界是否超出限制
        /// </summary>
        protected virtual bool SlopOverY(RectTransform target, Vector2 pos, MinMax<float> restrict, MinMax<float> offset, out MinMax<float> sov)
        {
            var reslut = false;
            sov = default;
            if (target == null) return reslut;

            var pivot = target.pivot;
            float side = target.GetHeight();

            return SlopOverAxis(side, pivot.y, pos.y, restrict, offset, out sov);
        }
        /// <summary>
        /// 检查轴是否超出限制的范围
        /// </summary>
        /// <param name="side">边长</param>
        /// <param name="pivot">支点，影响位置</param>
        /// <param name="pos">位置</param>
        /// <param name="restrict">限制范围</param>
        /// <param name="sov">超出的值（min 始终是负值，max 始终是正值，0 表示未超出）</param>
        /// <returns></returns>
        protected bool SlopOverAxis(float side, float pivot, float pos, MinMax<float> restrict, MinMax<float> offset, out MinMax<float> sov)
        {
            var reslut = false;
            sov = default;

            // 需要将支点也计算在内，实际位置是支点偏移后的
            // 反向支点
            float pivotReverse = 1 - pivot;

            // 计算边缘的位置
            float max = pos + pivotReverse * side;
            float min = pos - pivot * side;

            // 计算偏移
            max += offset.max;
            min -= offset.min;

            if (_dragRestrictZoneReversal)
            {
                if (min <= restrict.max)
                {
                    sov.min = min - restrict.max;// 始终是负值
                    reslut = true;
                }
                if (max >= restrict.min)
                {
                    sov.max = max - restrict.min;// 始终是正值
                    reslut = true;
                }
            }
            else
            {

                if (min <= restrict.min)
                {
                    sov.min = min - restrict.min;// 始终是负值
                    reslut = true;
                }
                if (max >= restrict.max)
                {
                    sov.max = max - restrict.max;// 始终是正值
                    reslut = true;
                }
            }
            return reslut;
        }

        /// <summary>
        /// 获取拖拽限制区域
        /// </summary>
        /// <param name="drzX"></param>
        /// <param name="drzY"></param>
        /// <returns></returns>
        public bool GetDragRestrictZone(out MinMax<float> drzX, out MinMax<float> drzY)
            => GetDragRestrictZone(_dragRestrictZoneMode, out drzX, out drzY);
        public bool GetDragRestrictZone(DragRestrictZoneMode mode, out MinMax<float> drzX, out MinMax<float> drzY)
        {
            bool r = true;
            drzX = default; drzY = default;
            if (mode == DragRestrictZoneMode.Sreen)
            {
                drzX = new MinMax<float>(0, Screen.width);
                drzY = new MinMax<float>(0, Screen.height);
            }
            else if (mode == DragRestrictZoneMode.Fixed)
            {
                drzX = _dragRestrictZoneX;
                drzY = _dragRestrictZoneY;
            }
            else if (mode == DragRestrictZoneMode.Target)
            {
                if (_dragRestrictZoneTarget)
                {
                    _dragRestrictZoneTarget.GetRectPrimaryVertex(
                        out _
                        , out var leftDown
                        , out _
                        , out _
                        , out var rightUp
                        );
                    drzX = new MinMax<float>(leftDown.x, rightUp.x);
                    drzY = new MinMax<float>(leftDown.y, rightUp.y);
                }
                else
                {
                    r = false;
                    //Debug.LogError("dragRestrictZoneTarget 为空，缺少\"拖拽限制限制区域目标\"。");
                }
            }
            return r;
        }

        /// <summary>
        /// 限制拖拽活动区域的方式
        /// </summary>
        public enum DragRestrictZoneMode
        {
            /// <summary>
            /// 屏幕
            /// </summary>
            Sreen,
            /// <summary>
            /// 固定值范围
            /// </summary>
            Fixed,
            /// <summary>
            /// 设置的目标区域
            /// </summary>
            Target
        }

        [Serializable]
        struct Test
        {
            public string name;
            public int num;
            public DragRestrictZoneMode mode;
        }
    }
}