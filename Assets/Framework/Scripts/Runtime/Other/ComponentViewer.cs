// -------------------------
// 创建日期：2023/4/11 15:27:24
// -------------------------

using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Framework
{
#if UNITY_EDITOR
    using System.Linq;
    using System.Reflection;
    using System.Text;
    using UnityEditor;
    using UnityEditorInternal;
    using Object = UnityEngine.Object;

    [CustomEditor(typeof(ComponentViewer))]
    public class ComponentViewerInspector : Editor
    {
        protected Object script;// 脚本资产
        protected SerializedProperty cTarget;

        protected GenericsTypeGUI cTargetGUI;
        protected GenericsTypeGUI _numGUI;

        bool cTargetSettingsFoldout { get => my.cTargetSettingsFoldout; set => my.cTargetSettingsFoldout = value; }
        bool showNonsupportMember { get => my.showNonsupportMember; set => my.showNonsupportMember = value; }
        bool showInheritRelation { get => my.showInheritRelation; set => my.showInheritRelation = value; }
        int maxDepth { get => my.maxDepth; set => my.maxDepth = value; }
        int minTextLine { get => my.minTextLine; set => my.minTextLine = value; }
        int maxTextLine { get => my.maxTextLine; set => my.maxTextLine = value; }

        ComponentViewer my => (ComponentViewer)target;

        // 此函数在脚本启动时调用
        protected virtual void Awake()
        {
            //Debug.Log($"{nameof(ComponentViewerInspector)}.Awake");

            script = MonoScript.FromMonoBehaviour(my);

            cTarget = serializedObject.FindProperty("_target");

            cTargetGUI = new GenericsTypeGUI(my.target, my, my.GetType().GetField("_target"
                , BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic));
            cTargetGUI.foldout = true;
            //cTargetGUI.showInheritRelation = false;
            cTargetGUI.setValueStartEvent = (_, v, _info) =>
            {
                Undo.RecordObject(my.target.gameObject, $"修改组件查看器目标 {_info?.MemberType} {_info?.Name}");
                Undo.RecordObject(my.target, $"修改组件查看器目标 {_info?.MemberType} {_info?.Name}");
                EditorUtility.SetDirty(my.target);
            };

            _numGUI = new GenericsTypeGUI(my._num, my, my.GetType().GetField("_num", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic));
            _numGUI.foldout = true;
            _numGUI.setValueStartEvent = (_, v, _info) =>
            {
                Undo.RecordObject(my, $"修改组件查看器 {_info.MemberType} {_info.Name}");
                EditorUtility.SetDirty(my);
            };

            Init();
        }

        protected virtual void OnEnable()
        {
            //Debug.Log(script);

        }

        public override void OnInspectorGUI()
        {
            //base.OnInspectorGUI();
            OnEditorTargetScriptGUI();

            OnTitleGUI();

            OnCTargetGUI();

            //_numGUI.OnGUI();

            OnHelpGUI();

            // 字段、属性
            OnCTargetMemberGUI();
        }

        protected virtual void Init()
        {
            cTargetGUI.target = my.target;
        }

        // 编辑器目标脚本文件
        protected virtual void OnEditorTargetScriptGUI()
        {
            if (script)
            {
                EditorGUI.BeginDisabledGroup(true);
                EditorGUILayout.ObjectField("Script", script, typeof(MonoScript), true);
                EditorGUI.EndDisabledGroup();
            }
        }

        // 标题
        protected virtual void OnTitleGUI()
        {
            EditorGUILayout.Space(2);
            EditorGUILayout.LabelField("组件成员查看器", EditorStyles.centeredGreyMiniLabel);
            EditorGUILayout.Space(2);
        }

        // 温馨提示
        protected virtual void OnHelpGUI()
        {
            EditorGUILayout.HelpBox("将暴露所有 “非公开的” 字段和属性，请谨慎修改！\r\n部分属性及字段修改将不能被 Unity 序列化！数据会丢失，且不支持撤回！\r\n有关 Unity 序列化规则可自行查阅资料，比较简单的的判断方式是：能在原组件的 Inspector 中显示的都是可序列化的（本组件除外）。", MessageType.Warning);
        }

        // 目标 gui
        protected virtual void OnCTargetGUI()
        {
            EditorGUI.BeginChangeCheck();
            Object oldTarget = cTarget.objectReferenceValue;
            EditorGUILayout.PropertyField(cTarget);
            bool change_cTarget = EditorGUI.EndChangeCheck();
            //change_cTarget = change_cTarget || oldTarget != cTarget.objectReferenceValue;

            // 切换目标
            if (change_cTarget)
            {
                // 先保存
                serializedObject.ApplyModifiedProperties();
                //Debug.Log(my.target as object == null);
                // 再初始化
                Init();
                //Debug.Log($"更改目标为：{(cTarget.objectReferenceValue as Component)?.gameObject.name}<{cTarget.objectReferenceValue?.GetType().Name}>。原目标：{(oldTarget as Component)?.gameObject.name}<{oldTarget?.GetType().Name}>");
            }

            OnCTargetScriptGUI();
        }
        // 查看器目标脚本文件
        protected virtual void OnCTargetScriptGUI()
        {
            if (my.target is MonoBehaviour monoScript)
            {
                Object cTargetScript = MonoScript.FromMonoBehaviour(monoScript);
                if (cTargetScript)
                {
                    EditorGUI.BeginDisabledGroup(true);
                    EditorGUILayout.ObjectField("Target Script", cTargetScript, typeof(MonoScript), true);
                    EditorGUI.EndDisabledGroup();
                }
            }

        }

        // 目标组件的成员（字段、属性） gui
        protected virtual void OnCTargetMemberGUI()
        {
            OnCTargetSettingsGUI();

            CheckCTargetChange();

            if (my.target == null)
            {
                EditorGUILayout.HelpBox("设置 Target 以操作字段、属性！", MessageType.Info);
            }
            else
            {
                cTargetGUI.OnGUI(true, false);
            }
        }
        // 检查目标变化，有变化则自动更新目标成员信息
        protected virtual void CheckCTargetChange()
        {
            /* 检查已有字段、属性的所属实例与现有目标是否相同
                用于解决类似以下问题：
                ArgumentException: Field _target defined on type Framework.ComponentViewer is not a field on the target object which is of type UnityEngine.Transform.
            */
            bool targetChange = cTargetGUI.CheckTargetChange(GetType(my.target));
            if (targetChange)
            {
                Init();
            }
        }
        // 设置查看器目标的 gui
        protected virtual void OnCTargetSettingsGUI()
        {
            cTargetSettingsFoldout = EditorGUILayout.BeginFoldoutHeaderGroup(cTargetSettingsFoldout, "设置");
            if (cTargetSettingsFoldout)
            {
                EditorGUI.indentLevel++;

                showNonsupportMember = EditorGUILayout.Toggle("显示不支持类型的成员", showNonsupportMember);

                showInheritRelation = EditorGUILayout.Toggle("显示类型成员的继承关系", showInheritRelation);

                maxDepth = EditorGUILayout.IntSlider("GUI 最大深度", maxDepth, 2, 100);

                //float minTextLine = cTargetGUI.minTextLine, maxTextLine = cTargetGUI.maxTextLine;
                //EditorGUI.MinMaxSlider("文本显示行数", ref minTextLine, ref maxTextLine, 2, 100);
                //cTargetGUI.minTextLine = (int)minTextLine;
                //cTargetGUI.maxTextLine = (int)maxTextLine;
                minTextLine = EditorGUILayout.IntSlider("最小文本显示行数", minTextLine, 2, 100);
                maxTextLine = EditorGUILayout.IntSlider("最大文本显示行数", maxTextLine, 2, 100);

                //if (GUILayout.Button("展开所有成员"))
                //{

                //}

                EditorGUI.indentLevel--;
            }
            EditorGUILayout.EndFoldoutHeaderGroup();

            cTargetGUI.showNonsupportMember = showNonsupportMember;
            cTargetGUI.showInheritRelation = showInheritRelation;

            cTargetGUI.maxDepth = maxDepth;
            maxDepth = cTargetGUI.maxDepth;

            cTargetGUI.minTextLine = minTextLine;
            cTargetGUI.maxTextLine = maxTextLine;
            minTextLine = cTargetGUI.minTextLine;
            maxTextLine = cTargetGUI.maxTextLine;
        }

        /// <summary>
        /// 获取类型，如果是可 <c>null</c> 类型且值为 <c>null</c>，则获取非实例类型
        /// </summary>
        /// <typeparam name="TIn"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static Type GetType<TIn>(TIn obj) => GenericsTypeGUI.GetType(obj);

        public struct TypeGUIArgs
        {
            public object owner;    // 成员所属者

            public MemberInfo info; // 成员的信息
            public string name;
            public GUIContent label;
            public bool showLabel;
            public bool readOnly;
            public bool showGUI;
            public bool isIndexer;  // 是否索引器
            public object value;    // 成员的值
            public Type type;       // 成员的类型
            public Type declareType;
            public Func<object, bool> setter;// gui 修改后的新值回调

            public FieldInfo fieldInfo => info as FieldInfo;
            public PropertyInfo propertyInfo => info as PropertyInfo;
            public MethodInfo methodInfo => info as MethodInfo;
            public EventInfo eventInfo => info as EventInfo;

            public GUIContent GetLabel()
            {
                if (showLabel)
                {
                    return label;
                }
                return GUIContent.none;
            }
        }

        public class Control<T>
        {
            public T value;

            public Control()
            {

            }

            public Control(T value)
            {
                this.value = value;
            }
        }
        public class ControlList<T> : Control<List<T>>
        {
            // 折页
            public bool foldout = true;

            public ControlList()
            {
                value = new List<T>(2);
            }

            public ControlList(List<T> value)
            {
                this.value = value;
            }
        }

        /// <summary>
        /// 自定义类型 gui
        /// </summary>
        public partial class GenericsTypeGUI
        {
            /*
                重写此类比较麻烦，需要同时提供 自动布局版 和 非自动布局版，如非必要不建议重写
            */

            protected object _objInstance;  // 成员所属的对象实例
            protected MemberInfo _info;     // 目标所属的成员，即 _objInstance 的成员
            protected object _target;       // 为其创建 gui 的目标

            protected ControlList<FieldInfo> _fieldInfos = new ControlList<FieldInfo>();
            protected ControlList<PropertyInfo> _propertyInfos = new ControlList<PropertyInfo>();
            protected Dictionary<Type, ControlList<FieldInfo>> _typeFieldInfos = new Dictionary<Type, ControlList<FieldInfo>>(3);
            protected Dictionary<Type, ControlList<PropertyInfo>> _typePropertyInfos = new Dictionary<Type, ControlList<PropertyInfo>>(3);

            protected bool _showFiled = true, _showProperty = true;
            protected bool _showNonsupportMember = true;// root
            protected bool _showReadonlyProperty = true;// root
            protected bool _showInheritRelation = true;
            protected bool _foldout = false, _fieldFoldout = true, _propertyFoldout = false;// 由于属性内部实现的不确定性，默认不展开，将不会对属性进行任何操作
            protected bool _showFieldFoldout = true, _showPropertyFoldout = true;
            protected int _minTextLine = 1, _maxTextLine = 5;// root
            protected Vector2 _scrollPosition;

            private int _maxDepth = 10;// root
            protected bool _allowSelfNested = false;// 允许自我嵌套的类型 // root

            protected GenericsTypeGUI _parent;
            protected List<GenericsTypeGUI> _childs = new List<GenericsTypeGUI>(1);
            protected List<ReorderableList> _rLists = new List<ReorderableList>(1);
            protected ReorderableList _rlist;

            protected Action<object, object, MemberInfo> _setValueStartEvent;// root
            protected Action<object, object, MemberInfo> _setValueEndEvent;// root
            protected Func<GenericsTypeGUI> _createChildEvent;// root
            // 自定义 gui 事件
            protected Action _fieldStartGUIEvent, _fieldEndGUIEvent;
            protected Action _propertyStartGUIEvent, _propertyEndGUIEvent;
            protected Action _memberStartGUIEvent, _memberEndGUIEvent;
            protected ActionRef<Rect> _fieldStartRectGUIEvent, _fieldEndRectGUIEvent;
            protected ActionRef<Rect> _propertyStartRectGUIEvent, _propertyEndRectGUIEvent;
            protected ActionRef<Rect> _memberStartRectGUIEvent, _memberEndRectGUIEvent;

            #region 属性

            #region root
            /// <summary>
            /// 开始为目标字段或属性设置值事件，包括为 <see cref="target"/> 赋值
            /// <para></para>arg1：旧值，arg2：新值，arg3：设置值的目标字段或属性
            /// <para></para>arg3 MemberInfo 属性说明：如果不可读，则旧值一直为 null，只有在可写时，该事件才会被调用
            /// </summary>
            /// <remarks>此属性始终是 <see cref="root"/> 的</remarks>
            public Action<object, object, MemberInfo> setValueStartEvent
            {
                get => root._setValueStartEvent;
                set => root._setValueStartEvent = value;
            }
            /// <summary>
            /// 结束为目标字段或属性设置值事件，包括为 <see cref="target"/> 赋值
            /// <para></para>arg1：旧值，arg2：新值，arg3：设置值的目标字段或属性
            /// <para></para>arg3 MemberInfo 属性说明：如果不可读，旧值一直为 null，只有在可写时，该事件才会被调用
            /// </summary>
            /// <remarks>此属性始终是 <see cref="root"/> 的</remarks>
            public Action<object, object, MemberInfo> setValueEndEvent
            {
                get => root._setValueEndEvent;
                set => root._setValueEndEvent = value;
            }
            /// <summary>创建子 gui 对象事件<para>此属性始终是 <see cref="root"/> 的</para></summary>
            /// <remarks>
            /// <para></para>return 新实例
            /// </remarks>
            public Func<GenericsTypeGUI> createChildEvent
            {
                get => root._createChildEvent;
                set => root._createChildEvent = value;
            }

            /// <summary>显示不支持的成员<para>此属性始终是 <see cref="root"/> 的</para></summary>
            public virtual bool showNonsupportMember
            {
                get => root._showNonsupportMember;
                set
                {
                    root._showNonsupportMember = value;
                    //// 设置子
                    //foreach (var item in _childs)
                    //{
                    //    item._showNonsupportMember = value;
                    //}
                }
            }
            /// <summary>显示只读属性<para>此属性始终是 <see cref="root"/> 的</para></summary>
            public virtual bool showReadonlyProperty
            {
                get => root._showReadonlyProperty;
                set
                {
                    root._showReadonlyProperty = value;
                }
            }

            /// <summary>允许自我嵌套的类型
            /// <para>自嵌：1、类中有自身类型作为成员；2、两个类有继承关系，且父类中有子类作为其成员</para>
            /// <para>此属性始终是 <see cref="root"/> 的</para>
            /// </summary>
            public virtual bool allowSelfNested
            {
                get => root._allowSelfNested;
                set
                {
                    root._allowSelfNested = value;
                }
            }

            /// <summary>当 <see cref="target"/> 是 <see cref="string"/> 时，显示的最小文本行数，不能小于 1<para>此属性始终是 <see cref="root"/> 的</para></summary>
            public int minTextLine
            {
                get => root._minTextLine;
                set
                {
                    root._minTextLine = value > 1 ? value : 1;
                    root._minTextLine = value > root._maxTextLine ? root._maxTextLine : value;
                }
            }
            /// <summary>当 <see cref="target"/> 是 <see cref="string"/> 时，显示的最大文本行数，不能小于 <see cref="minTextLine"/><para>此属性始终是 <see cref="root"/> 的</para></summary>
            public int maxTextLine
            {
                get => root._maxTextLine;
                set => root._maxTextLine = value > root.minTextLine ? value : root.minTextLine;
            }

            /// <summary>
            /// 允许的对象创建 gui 的最大深度，防止无限循环依赖，不小于 1
            /// <para>此属性始终是 <see cref="root"/> 的</para>
            /// </summary>
            public int maxDepth
            {
                get
                {
                    return root?._maxDepth ?? 0;
                }
                set
                {
                    if (root != null)
                    {
                        root._maxDepth = value > 0 ? value : 1;
                    }
                }
            }
            #endregion

            /// <summary>显示字段</summary>
            public bool showFiled { get => _showFiled; set => _showFiled = value; }
            /// <summary>显示属性</summary>
            public bool showProperty { get => _showProperty; set => _showProperty = value; }

            /// <summary>折页</summary>
            public bool foldout { get => _foldout; set => _foldout = value; }
            /// <summary>字段折页</summary>
            public bool fieldFoldout { get => _fieldFoldout; set => _fieldFoldout = value; }
            /// <summary>属性折页</summary>
            public bool propertyFoldout { get => _propertyFoldout; set => _propertyFoldout = value; }

            /// <summary>显示 字段 折页，否则直接显示 字段 </summary>
            public bool showFieldFoldout { get => _showFieldFoldout; set => _showFieldFoldout = value; }
            /// <summary>显示 属性 折页，否则直接显示 属性 </summary>
            public bool showPropertyFoldout { get => _showPropertyFoldout; set => _showPropertyFoldout = value; }

            /// <summary>显示继承关系</summary>
            public virtual bool showInheritRelation
            {
                get
                {
                    return _showInheritRelation;
                }
                set
                {
                    _showInheritRelation = value;
                }
            }

            public GenericsTypeGUI parent
            {
                get => _parent;
                protected set
                {
                    if (_parent == value || this == value) return;
                    var cp = _parent;
                    _parent = value;
                    cp?._childs.Remove(this);
                    if (value != null && !value._childs.Contains(this))
                    {
                        value?._childs.Add(this);
                    }
                }
            }
            /// <summary>根对象</summary>
            public GenericsTypeGUI root
            {
                get
                {
                    var p = this;
                    while (p._parent != null && p._parent != this)
                    {
                        p = p._parent;
                    }
                    return p;
                }
            }
            public bool IsRoot => root == this;

            /// <summary>当前对象的深度</summary>
            public int currentDepth
            {
                get
                {
                    int depth = 0;
                    var p = _parent;
                    while (p != null && p != this)
                    {
                        depth++;
                        p = p._parent;
                    }
                    return depth;
                }
            }
            /// <summary>总深度</summary>
            public int depth
            {
                get
                {
                    int depth = 0;
                    var p = root;
                    while (p != null && p._childs != null && p._childs.Count > 0)
                    {
                        depth++;
                        p = p._childs[0];
                    }
                    return depth;
                }
            }

            /// <summary>目标成员所属的对象实例</summary>
            /// <remarks>如果 <see cref="parent"/> 不为空的话，始终表示为 <see cref="parent"/> 的 <see cref="target"/>，因为其必定代表为父对象的某个成员</remarks>
            public object objInstance
            {
                get
                {
                    if (parent != null) return parent._target;

                    return _objInstance;
                }
                set
                {
                    if (parent == null)
                    {
                        _objInstance = value;
                    }
                }
            }
            /// <summary>如果设置了 <see cref="objInstance"/>，则必须为 <see cref="objInstance"/> 的成员</summary>
            public MemberInfo info
            {
                get => _info;
                set
                {
                    _info = value;
                }
            }
            /// <summary>
            /// 要创建 gui 的目标对象，如果设置了 <see cref="info"/>，则必须为其所表示的可赋值类型
            /// </summary>
            public virtual object target
            {
                get { return _target; }
                set
                {
                    if (_target == value) return;

                    Type oldType = type;
                    if (value is UnityEngine.Object uObj)// unity object 特殊处理
                    {
                        _target = uObj ? uObj : null;
                    }
                    else
                    {
                        _target = value;
                    }
                    if (type != oldType)
                    //if (value?.GetType() != oldType)
                    {
                        GetMemberInfo();
                    }
                }
            }
            /// <summary>目标 <see cref="target"/> 的实例类型，当 <see cref="target"/> 为 <paramref name="null"/>，<see cref="type"/> 也是 <paramref name="null"/></summary>
            public Type type => _target?.GetType();
            /// <summary>当 <see cref="info"/> 和 <see cref="target"/> 为 <paramref name="null"/> 时，将以此作为参考类型</summary>
            public Type referenceType { get; set; }

            /// <summary>是否列表容器</summary>
            public bool isIList => TypeCast(typeof(IList), type ?? referenceType, true);
            /// <summary>是否是元素</summary>
            public virtual bool isElement => parent?.isIList ?? false;
            /// <summary>
            /// 当自身是元素时 <see cref="isElement"/> = <paramref name="true"/>，获取索引
            /// </summary>
            public virtual int index
            {
                get
                {
                    int index = -1;
                    if (isElement)
                    {
                        index = parent._childs.IndexOf(this);
                    }
                    return index;
                }
            }
            /// <summary>当作为容器时，将以此作为元素参考类型</summary>
            public Type referenceElementType { get; set; }

            /// <summary>
            /// 标准类型，一般是声明时就指定的类型。例如：object obj = 1; 其实例类型为 Int32，标准类型为 object
            /// </summary>
            public Type standardType
            {
                get
                {
                    Type st = null;
                    if (_info != null)
                    {
                        st = GetType(_info);
                    }
                    else
                    {
                        st ??= referenceType;
                        st ??= referenceElementType;
                    }
                    return st;
                }
            }

            public List<GenericsTypeGUI> childs { get => _childs; }
            public ControlList<FieldInfo> fieldInfos { get => _fieldInfos; }
            public ControlList<PropertyInfo> propertyInfos { get => _propertyInfos; }
            public Dictionary<Type, ControlList<FieldInfo>> typeFieldInfos { get => _typeFieldInfos; }
            public Dictionary<Type, ControlList<PropertyInfo>> typePropertyInfos { get => _typePropertyInfos; }
            #endregion

            public GenericsTypeGUI() { }
            public GenericsTypeGUI(object target)
            {
                this.target = target;
                Init();
            }
            public GenericsTypeGUI(object target, object objInstance, MemberInfo info)
            {
                this.target = target;
                this.objInstance = objInstance;
                _info = info;
                Init();
            }

            #region 静态方法
            /// <summary>
            /// 类型转换
            /// <para>如果是可空类型，请使用 <see cref="TypeCast{TIn, TOut}(TIn, out TOut, Type, bool, bool)"/></para>
            /// </summary>
            /// <typeparam name="TIn"></typeparam>
            /// <typeparam name="TOut"></typeparam>
            /// <param name="inObj"></param>
            /// <param name="outObj"></param>
            /// <param name="inherit">true：有继承关系</param>
            /// <param name="isAssignable">true：可将 <paramref name="inObj"/> 赋值给 <paramref name="outObj"/></param>
            /// <returns>是否转换成功</returns>
            public static bool TypeCast<TIn, TOut>(TIn inObj, out TOut outObj, bool inherit = false, bool isAssignable = false)
            {
                outObj = default;
                return TypeCast(inObj, out outObj, GetType(inObj), inherit, isAssignable);
            }
            /// <summary>
            /// 类型转换
            /// </summary>
            /// <typeparam name="TIn">要转换的对象类型</typeparam>
            /// <typeparam name="TOut">转换后的对象类型</typeparam>
            /// <param name="inObj">要转换的值</param>
            /// <param name="outObj">转换后的值</param>
            /// <param name="inType">指定输入的类型</param>
            /// <param name="inherit">true：有继承关系</param>
            /// <param name="isAssignable">true：可将 <paramref name="inObj"/> 赋值给 <paramref name="outObj"/></param>
            /// <returns>是否转换成功</returns>
            public static bool TypeCast<TIn, TOut>(TIn inObj, out TOut outObj, Type inType, bool inherit = false, bool isAssignable = false)
            {
                if (inType == null)
                {
                    Debug.LogError("在类型转换时，指定的输入类型为 null，可能导致转换超出预期！");
                }

                object vObj = inObj;
                outObj = default;
                Type outType = GetType(outObj);
                bool isType = inType == outType;

                bool inIsOutSubclass = (inType?.IsSubclassOf(outType) ?? false);
                bool inIsOutAssignable = outType.IsInterface ? (outType?.IsAssignableFrom(inType) ?? false) : false;
                bool inIsOut = inObj is TOut;
                if (inherit) isType = isType || inIsOutSubclass || inIsOut/* || inIsOutAssignable */;// 是否有继承关系
                if (isAssignable) isType = isType || inIsOutAssignable;// 是否可赋值关系

                if (isType && vObj != null)
                {
                    outObj = (TOut)vObj;
                }
                return isType;
            }
            /// <summary>
            /// 类型转换
            /// </summary>
            /// <param name="outType">转换后的类型</param>
            /// <param name="inType">指定输入的类型</param>
            /// <param name="inherit">true：有继承关系</param>
            /// <returns>是否转换成功</returns>
            public static bool TypeCast(Type outType, Type inType, bool inherit = false)
            {
                bool isType = inType == outType;

                bool inIsOutSubclass = (inType?.IsSubclassOf(outType) ?? false);
                bool inIsOutAssignable = outType.IsInterface ? (outType?.IsAssignableFrom(inType) ?? false) : false;
                if (inherit) isType = isType || inIsOutSubclass || inIsOutAssignable;// 是否有继承关系

                return isType;
            }

            /// <summary>是否相同或存在直系关系（同类型，或其中一方继承另一方）</summary>
            public static bool IsLineal(Type t1, Type t2)
            {
                if (t1 == null || t2 == null) return false;
                return t1 == t2 || t1.IsSubclassOf(t2) || t2.IsSubclassOf(t1);
            }

            /// <summary>获取类型，如果是可 <c>null</c> 类型且值为 <c>null</c>，则获取非实例类型</summary>
            public static Type GetType<TIn>(TIn obj)
            {
                // 如果 T 是引用类型，且值是 null，则无法调用 .GetType()，此时要通过 typeof 来获取 Type
                Type type = typeof(TIn);
                if (obj is UnityEngine.Object uObj)
                {
                    type = uObj ? uObj.GetType() : type;
                }
                else
                    type = obj?.GetType() ?? type;
                return type;
            }
            public static Type GetType(MemberInfo info)
            {
                if (info is FieldInfo fieldInfo)
                {
                    return fieldInfo.FieldType;
                }
                else if (info is PropertyInfo propertyInfo)
                {
                    return propertyInfo.PropertyType;
                }
                else
                {
                    return null;
                }
            }

            public static bool IsReadOnly(PropertyInfo info) => !info.CanWrite && info.CanRead;
            public static bool IsWriteOnly(PropertyInfo info) => info.CanWrite && !info.CanRead;
            // 是否标记为过时
            public static bool IsObsoleteMember(MemberInfo info) => IsHasAttribute<ObsoleteAttribute>(info);
            // 是否有指定的特性
            public static bool IsHasAttribute<T>(MemberInfo info) where T : Attribute => IsHasAttribute(info, typeof(T), true);
            public static bool IsHasAttribute(MemberInfo info, Type attributeType, bool inherit) => info.GetCustomAttributes(attributeType, inherit).Length > 0;

            // 获取成员声明时的权限访问修饰符
            public static string GetMemberDeclareVisit(MemberInfo info)
            {
                string visit = null;
                if (info != null)
                {
                    switch (info.MemberType)
                    {
                        case MemberTypes.Field:
                            var fieldInfo = (FieldInfo)info;
                            if (fieldInfo.IsPublic)
                                visit = "public";
                            else if (fieldInfo.IsPrivate)
                                visit = "private";
                            else if (fieldInfo.IsFamily)
                                visit = "protected";
                            else if (fieldInfo.IsAssembly)
                                visit = "internal";
                            else if (fieldInfo.IsFamilyOrAssembly)
                                visit = "protected internal";
                            else if (fieldInfo.IsFamilyAndAssembly)
                                visit = "private protected";
                            break;
                        case MemberTypes.Property:
                            var propertyInfo = (PropertyInfo)info;
                            string getVisit = propertyInfo.CanRead ? $"get：{GetMethodDeclareVisit(propertyInfo.GetMethod)}" : null;
                            string setVisit = propertyInfo.CanWrite ? $"set：{GetMethodDeclareVisit(propertyInfo.SetMethod)}" : null;
                            string s = getVisit != null && setVisit != null ? "，" : null;
                            visit = $"{getVisit}{s}{setVisit}";
                            break;
                        case MemberTypes.Method:
                            visit = GetMethodDeclareVisit((MethodInfo)info);
                            break;
                    }
                }
                return visit;
            }
            protected static string GetMethodDeclareVisit(MethodInfo info)
            {
                string visit = null;
                if (info.IsPublic)
                    visit = "public";
                else if (info.IsPrivate)
                    visit = "private";
                else if (info.IsFamily)
                    visit = "protected";
                else if (info.IsAssembly)
                    visit = "internal";
                else if (info.IsFamilyOrAssembly)
                    visit = "protected internal";
                else if (info.IsFamilyAndAssembly)
                    visit = "private protected";
                return visit;
            }

            // gui
            static GUIContent _tempLabel = new GUIContent();
            protected static GUIContent GetTempGUIContent(string text)
            {
                _tempLabel.text = text;
                _tempLabel.image = null;
                _tempLabel.tooltip = null;
                return _tempLabel;
            }
            #endregion

            public virtual void Init()
            {
                GetMemberInfo();

                CreateSpecialHandling(this);
            }

            public virtual void OnGUI(bool bringLabel = true) => OnGUI(IsGenericsType(), bringLabel);
            public virtual void OnGUI(ref Rect rect, bool bringLabel = true) => OnGUI(ref rect, IsGenericsType(), bringLabel);
            public virtual void OnGUI(GUIContent label) => OnGUI(IsGenericsType(), label);
            public virtual void OnGUI(ref Rect rect, GUIContent label) => OnGUI(ref rect, IsGenericsType(), label);

            public virtual void OnGUI(bool showMember, GUIContent label)
            {
                if (showMember)
                {
                    OnTargetLabelMemberGUI(label);
                }
                else
                {
                    OnGUI(label, target, type, out var newV);
                    target = newV;
                }
            }
            public virtual void OnGUI(ref Rect rect, bool showMember, GUIContent label)
            {
                if (showMember)
                {
                    OnTargetLabelMemberGUI(ref rect, label);
                }
                else
                {
                    OnGUI(ref rect, label, target, type, out var newV);
                    target = newV;
                }
            }
            public virtual void OnGUI(bool showMember, bool bringLabel = true)
            {
                if (showMember)
                {
                    if (bringLabel)
                    {
                        OnTargetLabelMemberGUI();
                    }
                    else
                    {
                        OnMemberGUI();
                    }
                }
                else
                {
                    TypeGUI(_info, objInstance, bringLabel);
                }
            }
            public virtual void OnGUI(ref Rect rect, bool showMember, bool bringLabel = true)
            {
                if (showMember)
                {
                    if (bringLabel)
                    {
                        OnTargetLabelMemberGUI(ref rect);
                    }
                    else
                    {
                        OnMemberGUI(ref rect);
                    }
                }
                else
                {
                    TypeGUI(ref rect, _info, objInstance, bringLabel);
                }
            }

            /// <summary>
            /// 根据对象类型自动提供适合其的 gui
            /// </summary>
            /// <returns>gui 是否发生变化</returns>
            public virtual bool OnGUI(GUIContent label, object v, Type type, out object newV)
            {
                this.target = v;
                this.referenceType = type;
                bool change = TypeGUI(label, v, type, out newV);
                this.target = newV;
                return change;
            }
            public virtual bool OnGUI(ref Rect rect, GUIContent label, object v, Type type, out object newV)
            {
                this.target = v;
                this.referenceType = type;
                bool change = TypeGUI(ref rect, label, v, type, out newV);
                this.target = newV;
                return change;
            }

            //public void UnfoldAll()
            //{

            //}

            // 带标签
            protected virtual void OnTargetLabelMemberGUI(ref Rect rect) => OnTargetLabelMemberGUI(ref rect, GetLabel(_info?.Name, type, standardType, _info));
            protected virtual void OnTargetLabelMemberGUI(ref Rect rect, GUIContent label)
            {
                _foldout = EditorGUI.Foldout(rect, _foldout, label, true);
                rect.y += EditorGUIUtility.singleLineHeight;
                if (_foldout)
                {
                    rect.x += EditorGUIUtility.singleLineHeight;
                    OnMemberGUI(ref rect);
                    rect.x -= EditorGUIUtility.singleLineHeight;
                }
            }
            protected virtual void OnTargetLabelMemberGUI() => OnTargetLabelMemberGUI(GetLabel(_info?.Name, type, standardType, _info));
            protected virtual void OnTargetLabelMemberGUI(GUIContent label)
            {
                _foldout = EditorGUILayout.Foldout(_foldout, label, true);
                if (_foldout)
                {
                    EditorGUI.indentLevel += 1;
                    OnMemberGUI();
                    EditorGUI.indentLevel -= 1;
                }
            }
            // 目标组件的成员（字段、属性） gui
            protected virtual void OnMemberGUI(ref Rect rect)
            {
                CheckTargetChange();

                _memberStartRectGUIEvent?.Invoke(ref rect);

                // 字段
                if (typeFieldInfos.Count > 0 && _showFiled)
                    OnMemberGUI(ref rect, ref _fieldFoldout, _showFieldFoldout, "字段", typeFieldInfos
                    , (ref Rect _r) =>
                    {
                        if (IsRoot)
                            OnRootFieldStartGUI(ref _r);
                        _fieldStartRectGUIEvent?.Invoke(ref _r);
                    }
                    , (ref Rect _r) =>
                    {
                        if (IsRoot)
                            OnRootFieldEndGUI(ref _r);
                        _fieldEndRectGUIEvent?.Invoke(ref _r);
                    });

                // 属性
                if (typePropertyInfos.Count > 0 && _showProperty)
                    OnMemberGUI(ref rect, ref _propertyFoldout, _showPropertyFoldout, "属性", typePropertyInfos
                    , (ref Rect _r) =>
                    {
                        if (IsRoot)
                            OnRootPropertyStartGUI(ref _r);
                        _propertyStartRectGUIEvent?.Invoke(ref _r);
                    }
                    , (ref Rect _r) =>
                    {
                        if (IsRoot)
                            OnRootPropertyEndGUI(ref _r);
                        _propertyEndRectGUIEvent?.Invoke(ref _r);
                    });

                _memberEndRectGUIEvent?.Invoke(ref rect);
            }
            protected virtual void OnMemberGUI()
            {
                CheckTargetChange();

                _memberStartGUIEvent?.Invoke();

                // 字段
                if (typeFieldInfos.Count > 0 && _showFiled)
                    OnMemberGUI(ref _fieldFoldout, _showFieldFoldout, "字段", typeFieldInfos
                    , () =>
                    {
                        if (IsRoot)
                            OnRootFieldStartGUI();
                        _fieldStartGUIEvent?.Invoke();
                    }
                    , () =>
                    {
                        if (IsRoot)
                            OnRootFieldEndGUI();
                        _fieldEndGUIEvent?.Invoke();
                    });

                // 属性
                if (typePropertyInfos.Count > 0 && _showProperty)
                    OnMemberGUI(ref _propertyFoldout, _showPropertyFoldout, "属性", typePropertyInfos
                    , () =>
                    {
                        if (IsRoot)
                            OnRootPropertyStartGUI();
                        _propertyStartGUIEvent?.Invoke();
                    }
                    , () =>
                    {
                        if (IsRoot)
                            OnRootPropertyEndGUI();
                        _propertyEndGUIEvent?.Invoke();
                    });

                _memberEndGUIEvent?.Invoke();
            }
            // 成员区分字段、属性等阶段
            protected virtual void OnMemberGUI<T>(ref Rect rect, ref bool foldout, bool showFoldout, string label, Dictionary<Type, ControlList<T>> typeInfos, ActionRef<Rect> start = null, ActionRef<Rect> end = null) where T : MemberInfo
            {
                if (showFoldout)
                {
                    var ts = new GUIStyle(EditorStyles.foldout);
                    ts.fontStyle = FontStyle.Bold;
                    foldout = EditorGUI.Foldout(rect, foldout, label, true, ts);
                    rect.y += EditorGUIUtility.singleLineHeight;
                }
                //foldout = EditorGUILayout.BeginFoldoutHeaderGroup(foldout, label);
                if (foldout || !showFoldout)
                {
                    if (typeInfos.Count > 0)
                    {
                        OnMemberGUI(ref rect, typeInfos, start, end);
                    }
                    else
                    {
                        EditorGUI.LabelField(rect, $"没有 {label}");
                        rect.y += EditorGUIUtility.singleLineHeight;
                    }
                }
                //EditorGUILayout.EndFoldoutHeaderGroup();
            }
            protected virtual void OnMemberGUI<T>(ref bool foldout, bool showFoldout, string label, Dictionary<Type, ControlList<T>> typeInfos, Action start = null, Action end = null) where T : MemberInfo
            {
                if (showFoldout)
                {
                    var ts = new GUIStyle(EditorStyles.foldout);
                    ts.fontStyle = FontStyle.Bold;
                    foldout = EditorGUILayout.Foldout(foldout, label, true, ts);
                }
                //foldout = EditorGUILayout.BeginFoldoutHeaderGroup(foldout, label);
                if (foldout || !showFoldout)
                {
                    if (typeInfos.Count > 0)
                    {
                        OnMemberGUI(typeInfos, start, end);
                    }
                    else
                    {
                        EditorGUILayout.LabelField($"没有 {label}");
                    }
                }
                //EditorGUILayout.EndFoldoutHeaderGroup();
            }
            // 成员 gui
            protected virtual void OnMemberGUI<T>(ref Rect rect, Dictionary<Type, ControlList<T>> typeInfos, ActionRef<Rect> start = null, ActionRef<Rect> end = null) where T : MemberInfo
            {
                start?.Invoke(ref rect);

                var r = rect;

                int typeIndex = 0;
                //EditorGUI.indentLevel++;
                foreach (var infos in typeInfos)
                {
                    if (showInheritRelation)
                    {
                        var ts = new GUIStyle(EditorStyles.foldout);
                        ts.fontStyle = FontStyle.BoldAndItalic;
                        //ts.fontSize = (int)(ts.fontSize * 0.95f);
                        infos.Value.foldout = EditorGUI.Foldout(rect, infos.Value.foldout, infos.Key.FullName, true, ts);

                        rect.y += EditorGUIUtility.singleLineHeight;
                        if (infos.Value.foldout)
                        {
                            //EditorGUI.indentLevel++;
                            OnMemberGUI(ref rect, infos.Value);
                            //EditorGUI.indentLevel--;
                        }

                        ++typeIndex;

                        if (typeIndex < typeInfos.Count)
                        {
                            var s = new GUIStyle(EditorStyles.miniLabel);
                            s.normal.textColor = Color.yellow;
                            EditorGUI.LabelField(rect, "继承 ↓", s);

                            // TODO：这里不应该使用固定高度，应该计算动态的大小
                            rect.y += EditorGUIUtility.singleLineHeight;
                        }
                    }
                    else
                    {
                        OnMemberGUI(ref rect, infos.Value);
                    }
                }

                r.height += rect.y - r.y;// 高度 = 最终位置 - 初始位置
                if (IsRoot)
                {
                    GUI.Box(r, "");
                }

                end?.Invoke(ref rect);
            }
            protected virtual void OnMemberGUI<T>(Dictionary<Type, ControlList<T>> typeInfos, Action start = null, Action end = null) where T : MemberInfo
            {
                start?.Invoke();

                BeginVertical(IsRoot);
                {
                    int typeIndex = 0;
                    //EditorGUI.indentLevel++;
                    foreach (var infos in typeInfos)
                    {
                        if (showInheritRelation)
                        {
                            var ts = new GUIStyle(EditorStyles.foldout);
                            ts.fontStyle = FontStyle.BoldAndItalic;
                            //ts.fontSize = (int)(ts.fontSize * 0.95f);
                            infos.Value.foldout = EditorGUILayout.Foldout(infos.Value.foldout, infos.Key.FullName, true, ts);
                            if (infos.Value.foldout)
                            {
                                //EditorGUI.indentLevel++;
                                OnMemberGUI(infos.Value);
                                //EditorGUI.indentLevel--;
                            }

                            ++typeIndex;

                            if (typeIndex < typeInfos.Count)
                            {
                                var s = new GUIStyle(EditorStyles.miniLabel);
                                s.normal.textColor = Color.yellow;
                                EditorGUILayout.LabelField("继承 ↓", s);
                            }
                        }
                        else
                        {
                            OnMemberGUI(infos.Value);
                        }
                    }
                    //EditorGUI.indentLevel--;

                }
                EndVertical();

                end?.Invoke();
            }
            protected virtual void OnMemberGUI<T>(ref Rect rect, ControlList<T> infos) where T : MemberInfo
            {
                foreach (var info in infos.value)
                {
                    TypeGUI(ref rect, info);
                }
            }
            protected virtual void OnMemberGUI<T>(ControlList<T> infos) where T : MemberInfo
            {
                foreach (var info in infos.value)
                {
                    TypeGUI(info);
                }
            }
            protected void BeginVertical(bool isBox)
            {
                if (isBox)
                    EditorGUILayout.BeginVertical("box");
                else
                    EditorGUILayout.BeginVertical();
            }
            protected void EndVertical() => EditorGUILayout.EndVertical();

            // 检查目标变化，有变化则自动更新目标成员信息
            protected virtual void CheckTargetChange()
            {
                /* 检查已有字段、属性的所属实例与现有目标是否相同
                    用于解决类似以下问题：
                    ArgumentException: Field _target defined on type Framework.ComponentViewer is not a field on the target object which is of type UnityEngine.Transform.
                */
                if (CheckTargetChange(type))
                {
                    GetMemberInfo();// 不同时初始刷新
                }
            }
            public virtual bool CheckTargetChange(Type type)
            {
                MemberInfo info = (MemberInfo)fieldInfos.value.FirstOrDefault() ?? propertyInfos.value.FirstOrDefault();
                bool targetChange = info == null ? true : info.ReflectedType != type;
                return targetChange;
            }

            // 只能在 root 里显示，和对应的事件 _fieldStartGUIEvent 等不冲突，且在事件之前
            // 在 字段 gui 开始时的 gui
            protected virtual void OnRootFieldStartGUI(ref Rect rect)
            {
            }
            protected virtual void OnRootFieldStartGUI()
            {
            }
            // 在 字段 gui 结束时的 gui
            protected virtual void OnRootFieldEndGUI(ref Rect rect)
            {
            }
            protected virtual void OnRootFieldEndGUI()
            {
            }
            // 在 属性 gui 开始时的 gui
            protected virtual void OnRootPropertyStartGUI(ref Rect rect)
            {
                showReadonlyProperty = EditorGUI.ToggleLeft(rect, "显示只读属性（即只有 Get 访问器）", showReadonlyProperty);
                rect.y += EditorGUIUtility.singleLineHeight;
            }
            protected virtual void OnRootPropertyStartGUI()
            {
                showReadonlyProperty = EditorGUILayout.ToggleLeft("显示只读属性（即只有 Get 访问器）", showReadonlyProperty);
                //EditorGUILayout.Space(4);
            }
            // 在 属性 gui 结束时的 gui
            protected virtual void OnRootPropertyEndGUI(ref Rect rect)
            {
            }
            protected virtual void OnRootPropertyEndGUI()
            {
            }

            // 成员
            protected virtual void TypeGUI<TInfo>(ref Rect rect, TInfo info) where TInfo : MemberInfo
            {
                TypeGUI(ref rect, info, target, true);
            }
            protected virtual void TypeGUI<TInfo>(ref Rect rect, TInfo info, object owner, bool showLabel) where TInfo : MemberInfo
            {
                if (info is FieldInfo fieldInfo)
                {
                    TypeGUI(ref rect, fieldInfo, owner, showLabel);
                }
                else if (info is PropertyInfo propertyInfo)
                {
                    TypeGUI(ref rect, propertyInfo, owner, showLabel);
                }
                else
                {
                    Debug.LogWarning($"暂不支持成员类型：{info}");
                }
            }
            protected virtual void TypeGUI<TInfo>(TInfo info) where TInfo : MemberInfo
            {
                TypeGUI(info, target, true);
            }
            protected virtual void TypeGUI<TInfo>(TInfo info, object owner, bool showLabel) where TInfo : MemberInfo
            {
                if (info is FieldInfo fieldInfo)
                {
                    TypeGUI(fieldInfo, owner, showLabel);
                }
                else if (info is PropertyInfo propertyInfo)
                {
                    TypeGUI(propertyInfo, owner, showLabel);
                }
                else
                {
                    Debug.LogWarning($"暂不支持成员类型：{info}");
                }
            }
            // 字段
            protected virtual void TypeGUI(ref Rect rect, FieldInfo info) => TypeGUI(ref rect, info, target, true);
            protected virtual void TypeGUI(ref Rect rect, FieldInfo info, object owner, bool showLabel)
            {
                //Debug.Log($"字段：{info.Name}");
                if (IsObsoleteMember(info)) return;
                TypeGUI(ref rect, GetTypeGUIArgs(info, owner, showLabel));
            }
            protected virtual void TypeGUI(FieldInfo info) => TypeGUI(info, target, true);
            protected virtual void TypeGUI(FieldInfo info, object owner, bool showLabel)
            {
                //Debug.Log($"字段：{info.Name}");
                if (IsObsoleteMember(info)) return;
                TypeGUI(GetTypeGUIArgs(info, owner, showLabel));
            }
            // 属性
            protected virtual void TypeGUI(ref Rect rect, PropertyInfo info) => TypeGUI(ref rect, info, target, true);
            protected virtual void TypeGUI(ref Rect rect, PropertyInfo info, object owner, bool showLabel)
            {
                //Debug.Log($"属性：{info.Name}");
                if (IsObsoleteMember(info)) return;
                TypeGUI(ref rect, GetTypeGUIArgs(info, owner, showLabel));
            }
            protected virtual void TypeGUI(PropertyInfo info) => TypeGUI(info, target, true);
            protected virtual void TypeGUI(PropertyInfo info, object owner, bool showLabel)
            {
                //Debug.Log($"属性：{info.Name}");
                if (IsObsoleteMember(info)) return;
                TypeGUI(GetTypeGUIArgs(info, owner, showLabel));
            }
            // 类型信息
            protected virtual void TypeGUI(ref Rect rect, TypeGUIArgs args)
            {
                var info = args.info;
                var type = args.type;
                var name = args.name;
                var label = args.GetLabel();
                var showLabel = args.showLabel;
                var showGUI = args.showGUI;
                var readOnly = args.readOnly;
                var v = args.value;
                var setter = args.setter;

                if (!showGUI) return;

                // 处理索引器
                if (args.isIndexer)
                {
                    OnShowNonsupportMemberGUI(ref rect, label, "不支持操作索引器");
                    return;
                }

                EditorGUI.BeginDisabledGroup(readOnly);
                bool change = TypeGUI(ref rect, label, v, type, out object newV, info);
                EditorGUI.EndDisabledGroup();
                if (change)
                {
                    bool isSet = setter?.Invoke(newV) ?? false;
                    if (isSet)
                    {
                        // 值类型需要回溯
                        SyncSelfTargetValue();
                    }
                }
            }
            protected virtual void TypeGUI(TypeGUIArgs args)
            {
                var info = args.info;
                var type = args.type;
                var name = args.name;
                var label = args.GetLabel();
                var showLabel = args.showLabel;
                var showGUI = args.showGUI;
                var readOnly = args.readOnly;
                var v = args.value;
                var setter = args.setter;

                if (!showGUI) return;

                // 处理索引器
                if (args.isIndexer)
                {
                    OnShowNonsupportMemberGUI(label, "不支持操作索引器");
                    return;
                }

                EditorGUI.BeginDisabledGroup(readOnly);
                bool change = TypeGUI(label, v, type, out object newV, info);
                EditorGUI.EndDisabledGroup();
                if (change)
                {
                    bool isSet = setter?.Invoke(newV) ?? false;
                    if (isSet)
                    {
                        // 值类型需要回溯
                        SyncSelfTargetValue();
                    }
                }
            }

            // 以下是 TypeGUI 的最终处理方法
            protected virtual bool TypeGUI(ref Rect rect, GUIContent label, object v, Type type, out object newV, MemberInfo info = null)
            {
                newV = default;
                bool isGenericsType = false;// readme：是否由 本类 处理，而并非是 unity 提供的 api 直接处理
                bool isChange = false;      // readme：由 unity 直接处理的才能设置此值
                Type t = type;              // 记录原始类型
                type = v?.GetType() ?? type;// 以实例类型为准

                EditorGUI.BeginChangeCheck();

                isGenericsType = !UnityTypeGUI(ref rect, label, v, type, out newV, info);

                isChange = EditorGUI.EndChangeCheck();

                if (isGenericsType)
                {
                    // 处理容器类
                    if (TypeCast(v, out IList vIList, type, true))
                    {
                        newV = ListFieldGUI(ref rect, label, vIList, type, info);
                    }
                    else
                    {
                        // untiy 不直接提供 gui 的类型
                        newV = GenericsFieldGUI(ref rect, label, v, type, info);

                        //OnShowNonsupportMemberGUI(label);
                    }
                }
                else
                {
                    rect.y += EditorGUIUtility.singleLineHeight;
                }

                return isChange;
            }
            /// <summary>
            /// 根据对象类型自动提供适合其的 gui
            /// </summary>
            /// <returns>gui 是否发生变化</returns>
            /// <remarks>如果重写了此方法，扩展了类型 gui，必须也重写 <see cref="IsGenericsType(object, Type)"/> ，添加对应类型</remarks>
            protected virtual bool TypeGUI(GUIContent label, object v, Type type, out object newV, MemberInfo info = null)
            {
                newV = default;
                bool isGenericsType = false;// readme：是否由 本类 处理，而并非是 unity 提供的 api 直接处理
                bool isChange = false;      // readme：由 unity 直接处理的才能设置此值
                Type t = type;
                type = v?.GetType() ?? type;// 以实例类型为准

                EditorGUI.BeginChangeCheck();

                isGenericsType = !UnityTypeGUI(label, v, type, out newV, info);

                isChange = EditorGUI.EndChangeCheck();

                if (isGenericsType)
                {
                    // 处理容器类
                    if (TypeCast(v, out IList vIList, type, true))
                    {
                        ListFieldGUI(label, vIList, type, info);
                    }
                    else
                    {
                        // untiy 不直接提供 gui 的类型
                        newV = GenericsFieldGUI(label, v, type, info);

                        //OnShowNonsupportMemberGUI(label);
                    }
                }

                return isChange;
            }
            // 元素
            protected virtual bool TypeGUI(ref Rect rect, GUIContent label, object v, Type type, out object newV, int index)
            {
                newV = default;
                bool isGenericsType = false;// readme：是否由 本类 处理，而并非是 unity 提供的 api 直接处理
                bool isChange = false;      // readme：由 unity 直接处理的才能设置此值
                Type t = type;
                type = v?.GetType() ?? type;// 以实例类型为准

                EditorGUI.BeginChangeCheck();

                isGenericsType = !UnityTypeGUI(ref rect, label, v, type, out newV, index);

                isChange = EditorGUI.EndChangeCheck();

                if (isGenericsType)
                {
                    // 处理容器类
                    if (TypeCast(v, out IList vIList, type, true))
                    {
                        newV = ListFieldGUI(ref rect, label, vIList, type, index);
                    }
                    else
                    {
                        // untiy 不直接提供 gui 的类型
                        newV = GenericsFieldGUI(ref rect, label, v, type, index);

                        //OnShowNonsupportMemberGUI(label);
                    }
                }
                else
                {
                    rect.y += EditorGUIUtility.singleLineHeight;
                }

                return isChange;
            }
            // 由 unity 直接提供 gui 支持的
            protected virtual bool UnityTypeGUI(GUIContent label, object v, Type type, out object newV, MemberInfo info = null)
            {
                newV = default;
                bool r = true;

                if (TypeCast(v, out int vInt, type))
                {
                    newV = EditorGUILayout.IntField(label, vInt);
                }
                else if (TypeCast(v, out uint vUint, type))// 不直接支持
                {
                    int n = EditorGUILayout.IntField(label, (int)vUint);
                    newV = n < 0 ? 0u : (uint)n;
                    //Debug.Log($"{dispalyName}：{newV}");
                }
                else if (TypeCast(v, out long vLong, type))
                {
                    newV = EditorGUILayout.LongField(label, vLong);
                }
                else if (TypeCast(v, out ulong vUlong, type))// 不直接支持
                {
                    long n = EditorGUILayout.LongField(label, (long)vUlong);
                    newV = n < 0 ? 0ul : (ulong)n;
                }
                else if (TypeCast(v, out float vFloat, type))
                {
                    newV = EditorGUILayout.FloatField(label, vFloat);
                }
                else if (TypeCast(v, out double vDouble, type))
                {
                    newV = EditorGUILayout.DoubleField(label, vDouble);
                }
                else if (TypeCast(v, out bool vBool, type))
                {
                    newV = EditorGUILayout.Toggle(label, vBool);
                }
                else if (TypeCast(v, out string vString, type))
                {
                    label.tooltip = $"{label.tooltip}\r\n字符串长度：{vString?.Length ?? 0}";

                    // 是否属于 Component 的标签
                    if (IsLineal(this.type, typeof(Component)) && info != null && info.Name == "tag" && info.DeclaringType == typeof(Component))
                    {
                        newV = EditorGUILayout.TagField(label, vString);
                    }
                    else
                    {
                        newV = TextFieldGUI(label, vString, info);
                    }
                }
                else if (TypeCast(v, out char vChar, type))// 不直接支持
                {
                    string n = EditorGUILayout.TextField(label, new String(vChar, 1));
                    newV = n == null || n == "" ? default : n[0];
                }

                else if (TypeCast(v, out Hash128 vHash128, type))// 不直接支持
                {
                    string n = TextFieldGUI(label, vHash128.ToString(), info);
                    newV = Hash128.Parse(n);
                }

                else if (TypeCast(v, out LayerMask vLayerMask, type))
                {
                    vLayerMask.value = EditorGUILayout.LayerField(label, vLayerMask.value);
                    newV = vLayerMask;
                }

                else if (TypeCast(v, out Vector2 vVector2, type))
                {
                    newV = EditorGUILayout.Vector2Field(label, vVector2);
                }
                else if (TypeCast(v, out Vector2Int vVector2Int, type))
                {
                    newV = EditorGUILayout.Vector2IntField(label, vVector2Int);
                }
                else if (TypeCast(v, out Vector3 vVector3, type))
                {
                    newV = EditorGUILayout.Vector3Field(label, vVector3);
                }
                else if (TypeCast(v, out Vector3Int vVector3Int, type))
                {
                    newV = EditorGUILayout.Vector3IntField(label, vVector3Int);
                }
                else if (TypeCast(v, out Vector4 vVector4, type))
                {
                    newV = EditorGUILayout.Vector4Field(label, vVector4);
                }
                else if (TypeCast(v, out Quaternion vQuaternion, type))// 不直接支持
                {
                    var v4 = new Vector4(vQuaternion.x, vQuaternion.y, vQuaternion.z, vQuaternion.w);
                    v4 = EditorGUILayout.Vector4Field(label, v4);
                    newV = new Quaternion(v4.x, v4.y, v4.z, v4.w);
                }
                else if (TypeCast(v, out Color vColor, type))
                {
                    newV = EditorGUILayout.ColorField(label, vColor);
                }
                else if (TypeCast(v, out Color32 vColor32, type))
                {
                    newV = (Color32)EditorGUILayout.ColorField(label, vColor32);
                }
                else if (TypeCast(v, out Rect vRect, type))
                {
                    newV = EditorGUILayout.RectField(label, vRect);
                }
                else if (TypeCast(v, out RectInt vRectInt, type))
                {
                    newV = EditorGUILayout.RectIntField(label, vRectInt);
                }
                else if (TypeCast(v, out Bounds vBounds, type))
                {
                    newV = EditorGUILayout.BoundsField(label, vBounds);
                }
                else if (TypeCast(v, out BoundsInt vBoundsInt, type))
                {
                    newV = EditorGUILayout.BoundsIntField(label, vBoundsInt);
                }

                //// Matrix4x4 类型改为创建时 CreateGenericsTypeGUI 内联处理
                //else if (TypeCast(v, out Matrix4x4 vMatrix4x4, type))// 不直接支持
                //{
                //    var gui = ExpectChild(v, type);
                //    gui.target = v;
                //    gui.showProperty = false;
                //    newV = GenericsField(label, gui, type);
                //}

                else if (TypeCast(v, out Enum vEnum, type, true))// 允许继承转换
                {
                    // 有枚举标志位特性
                    if (IsHasAttribute<FlagsAttribute>(type))
                    {
                        newV = EditorGUILayout.EnumFlagsField(label, vEnum);
                    }
                    else
                        newV = EditorGUILayout.EnumPopup(label, vEnum);
                }

                else if (TypeCast(v, out AnimationCurve vAnimationCurve, type))
                {
                    newV = EditorGUILayout.CurveField(label, vAnimationCurve ?? new AnimationCurve());
                }
                else if (TypeCast(v, out Gradient vGradient, type))
                {
                    newV = EditorGUILayout.GradientField(label, vGradient ?? new Gradient());
                }

                else if (TypeCast(v, out Object vUnityObject, type, true))// 允许继承转换
                {
                    newV = EditorGUILayout.ObjectField(label, vUnityObject, type, true);
                }

                else
                {
                    r = false;
                }

                return r;
            }
            protected virtual bool UnityTypeGUI(ref Rect rect, GUIContent label, object v, Type type, out object newV, MemberInfo info = null)
            {
                newV = default;
                bool r = true;
                if (TypeCast(v, out int vInt, type))
                {
                    newV = EditorGUI.IntField(rect, label, vInt);
                }
                else if (TypeCast(v, out uint vUint, type))// 不直接支持
                {
                    int n = EditorGUI.IntField(rect, label, (int)vUint);
                    newV = n < 0 ? 0u : (uint)n;
                    //Debug.Log($"{dispalyName}：{newV}");
                }
                else if (TypeCast(v, out long vLong, type))
                {
                    newV = EditorGUI.LongField(rect, label, vLong);
                }
                else if (TypeCast(v, out ulong vUlong, type))// 不直接支持
                {
                    long n = EditorGUI.LongField(rect, label, (long)vUlong);
                    newV = n < 0 ? 0ul : (ulong)n;
                }
                else if (TypeCast(v, out float vFloat, type))
                {
                    newV = EditorGUI.FloatField(rect, label, vFloat);
                }
                else if (TypeCast(v, out double vDouble, type))
                {
                    newV = EditorGUI.DoubleField(rect, label, vDouble);
                }
                else if (TypeCast(v, out bool vBool, type))
                {
                    newV = EditorGUI.Toggle(rect, label, vBool);
                }
                else if (TypeCast(v, out string vString, type))
                {
                    label.tooltip = $"{label.tooltip}\r\n字符串长度：{vString?.Length ?? 0}";

                    // 是否属于 Component 的标签
                    if (IsLineal(this.type, typeof(Component)) && info != null && info.Name == "tag" && info.DeclaringType == typeof(Component))
                    {
                        newV = EditorGUI.TagField(rect, label, vString);
                    }
                    else
                    {
                        newV = TextFieldGUI(ref rect, label, vString, info);
                    }
                }
                else if (TypeCast(v, out char vChar, type))// 不直接支持
                {
                    string n = EditorGUI.TextField(rect, label, new String(vChar, 1));
                    newV = n == null || n == "" ? default : n[0];
                }

                else if (TypeCast(v, out Hash128 vHash128, type))// 不直接支持
                {
                    string n = TextFieldGUI(ref rect, label, vHash128.ToString(), info);
                    newV = Hash128.Parse(n);
                }

                else if (TypeCast(v, out LayerMask vLayerMask, type))
                {
                    vLayerMask.value = EditorGUI.LayerField(rect, label, vLayerMask.value);
                    newV = vLayerMask;
                }

                else if (TypeCast(v, out Vector2 vVector2, type))
                {
                    newV = EditorGUI.Vector2Field(rect, label, vVector2);
                }
                else if (TypeCast(v, out Vector2Int vVector2Int, type))
                {
                    newV = EditorGUI.Vector2IntField(rect, label, vVector2Int);
                }
                else if (TypeCast(v, out Vector3 vVector3, type))
                {
                    newV = EditorGUI.Vector3Field(rect, label, vVector3);
                }
                else if (TypeCast(v, out Vector3Int vVector3Int, type))
                {
                    newV = EditorGUI.Vector3IntField(rect, label, vVector3Int);
                }
                else if (TypeCast(v, out Vector4 vVector4, type))
                {
                    newV = EditorGUI.Vector4Field(rect, label, vVector4);
                }
                else if (TypeCast(v, out Quaternion vQuaternion, type))// 不直接支持
                {
                    var v4 = new Vector4(vQuaternion.x, vQuaternion.y, vQuaternion.z, vQuaternion.w);
                    v4 = EditorGUI.Vector4Field(rect, label, v4);
                    newV = new Quaternion(v4.x, v4.y, v4.z, v4.w);
                }
                else if (TypeCast(v, out Color vColor, type))
                {
                    newV = EditorGUI.ColorField(rect, label, vColor);
                }
                else if (TypeCast(v, out Color32 vColor32, type))
                {
                    newV = (Color32)EditorGUI.ColorField(rect, label, vColor32);
                }
                else if (TypeCast(v, out Rect vRect, type))
                {
                    newV = EditorGUI.RectField(rect, label, vRect);
                }
                else if (TypeCast(v, out RectInt vRectInt, type))
                {
                    newV = EditorGUI.RectIntField(rect, label, vRectInt);
                }
                else if (TypeCast(v, out Bounds vBounds, type))
                {
                    newV = EditorGUI.BoundsField(rect, label, vBounds);
                }
                else if (TypeCast(v, out BoundsInt vBoundsInt, type))
                {
                    newV = EditorGUI.BoundsIntField(rect, label, vBoundsInt);
                }

                else if (TypeCast(v, out Enum vEnum, type, true))// 允许继承转换
                {
                    // 有枚举标志位特性
                    if (IsHasAttribute<FlagsAttribute>(type))
                    {
                        newV = EditorGUI.EnumFlagsField(rect, label, vEnum);
                    }
                    else
                        newV = EditorGUI.EnumPopup(rect, label, vEnum);
                }

                else if (TypeCast(v, out AnimationCurve vAnimationCurve, type))
                {
                    newV = EditorGUI.CurveField(rect, label, vAnimationCurve ?? new AnimationCurve());
                }
                else if (TypeCast(v, out Gradient vGradient, type))
                {
                    newV = EditorGUI.GradientField(rect, label, vGradient ?? new Gradient());
                }

                else if (TypeCast(v, out Object vUnityObject, type, true))// 允许继承转换
                {
                    newV = EditorGUI.ObjectField(rect, label, vUnityObject, type, true);
                }

                else
                {
                    r = false;
                }

                return r;
            }
            protected virtual bool UnityTypeGUI(ref Rect rect, GUIContent label, object v, Type type, out object newV, int index)
            {
                rect.width -= EditorGUIUtility.singleLineHeight;
                //rect.x += EditorGUIUtility.singleLineHeight;

                newV = default;
                bool r = true;
                if (TypeCast(v, out int vInt, type))
                {
                    newV = EditorGUI.IntField(rect, label, vInt);
                }
                else if (TypeCast(v, out uint vUint, type))// 不直接支持
                {
                    int n = EditorGUI.IntField(rect, label, (int)vUint);
                    newV = n < 0 ? 0u : (uint)n;
                    //Debug.Log($"{dispalyName}：{newV}");
                }
                else if (TypeCast(v, out long vLong, type))
                {
                    newV = EditorGUI.LongField(rect, label, vLong);
                }
                else if (TypeCast(v, out ulong vUlong, type))// 不直接支持
                {
                    long n = EditorGUI.LongField(rect, label, (long)vUlong);
                    newV = n < 0 ? 0ul : (ulong)n;
                }
                else if (TypeCast(v, out float vFloat, type))
                {
                    newV = EditorGUI.FloatField(rect, label, vFloat);
                }
                else if (TypeCast(v, out double vDouble, type))
                {
                    newV = EditorGUI.DoubleField(rect, label, vDouble);
                }
                else if (TypeCast(v, out bool vBool, type))
                {
                    newV = EditorGUI.Toggle(rect, label, vBool);
                }
                else if (TypeCast(v, out string vString, type))
                {
                    label.tooltip = $"{label.tooltip}\r\n字符串长度：{vString?.Length ?? 0}";

                    // 是否属于 Component 的标签
                    if (IsLineal(this.type, typeof(Component)) && info != null && info.Name == "tag" && info.DeclaringType == typeof(Component))
                    {
                        newV = EditorGUI.TagField(rect, label, vString);
                    }
                    else
                    {
                        newV = TextFieldGUI(ref rect, label, vString, index);
                    }
                }
                else if (TypeCast(v, out char vChar, type))// 不直接支持
                {
                    string n = EditorGUI.TextField(rect, label, new String(vChar, 1));
                    newV = n == null || n == "" ? default : n[0];
                }

                else if (TypeCast(v, out Hash128 vHash128, type))// 不直接支持
                {
                    string n = TextFieldGUI(ref rect, label, vHash128.ToString(), index);
                    newV = Hash128.Parse(n);
                }

                else if (TypeCast(v, out LayerMask vLayerMask, type))
                {
                    vLayerMask.value = EditorGUI.LayerField(rect, label, vLayerMask.value);
                    newV = vLayerMask;
                }

                else if (TypeCast(v, out Vector2 vVector2, type))
                {
                    newV = EditorGUI.Vector2Field(rect, label, vVector2);
                }
                else if (TypeCast(v, out Vector2Int vVector2Int, type))
                {
                    newV = EditorGUI.Vector2IntField(rect, label, vVector2Int);
                }
                else if (TypeCast(v, out Vector3 vVector3, type))
                {
                    newV = EditorGUI.Vector3Field(rect, label, vVector3);
                }
                else if (TypeCast(v, out Vector3Int vVector3Int, type))
                {
                    newV = EditorGUI.Vector3IntField(rect, label, vVector3Int);
                }
                else if (TypeCast(v, out Vector4 vVector4, type))
                {
                    newV = EditorGUI.Vector4Field(rect, label, vVector4);
                }
                else if (TypeCast(v, out Quaternion vQuaternion, type))// 不直接支持
                {
                    var v4 = new Vector4(vQuaternion.x, vQuaternion.y, vQuaternion.z, vQuaternion.w);
                    v4 = EditorGUI.Vector4Field(rect, label, v4);
                    newV = new Quaternion(v4.x, v4.y, v4.z, v4.w);
                }
                else if (TypeCast(v, out Color vColor, type))
                {
                    newV = EditorGUI.ColorField(rect, label, vColor);
                }
                else if (TypeCast(v, out Color32 vColor32, type))
                {
                    newV = (Color32)EditorGUI.ColorField(rect, label, vColor32);
                }
                else if (TypeCast(v, out Rect vRect, type))
                {
                    newV = EditorGUI.RectField(rect, label, vRect);
                }
                else if (TypeCast(v, out RectInt vRectInt, type))
                {
                    newV = EditorGUI.RectIntField(rect, label, vRectInt);
                }
                else if (TypeCast(v, out Bounds vBounds, type))
                {
                    newV = EditorGUI.BoundsField(rect, label, vBounds);
                }
                else if (TypeCast(v, out BoundsInt vBoundsInt, type))
                {
                    newV = EditorGUI.BoundsIntField(rect, label, vBoundsInt);
                }

                else if (TypeCast(v, out Enum vEnum, type, true))// 允许继承转换
                {
                    // 有枚举标志位特性
                    if (IsHasAttribute<FlagsAttribute>(type))
                    {
                        newV = EditorGUI.EnumFlagsField(rect, label, vEnum);
                    }
                    else
                        newV = EditorGUI.EnumPopup(rect, label, vEnum);
                }

                else if (TypeCast(v, out AnimationCurve vAnimationCurve, type))
                {
                    newV = EditorGUI.CurveField(rect, label, vAnimationCurve ?? new AnimationCurve());
                }
                else if (TypeCast(v, out Gradient vGradient, type))
                {
                    newV = EditorGUI.GradientField(rect, label, vGradient ?? new Gradient());
                }

                else if (TypeCast(v, out Object vUnityObject, type, true))// 允许继承转换
                {
                    newV = EditorGUI.ObjectField(rect, label, vUnityObject, type, true);
                }

                else
                {
                    r = false;
                }

                return r;
            }

            [Obsolete]
            protected bool TypeCastList(Type type)
            {
                bool r = false;
                var label = GUIContent.none;

                Type typeIList = typeof(IList);
                Type typeIList1 = typeof(IList<>);
                //ReorderableList

                var genericTypeDefinition = type.IsGenericType ? type.GetGenericTypeDefinition() : null;
                var genericTypeDefinitionInterfaces = genericTypeDefinition?.GetInterfaces();
                var interfaces = type.GetInterfaces();

                //if (type == typeIList1)
                //{
                //    OnShowNonsupportMemberGUI(label, $"不支持该类型 IList<>1");
                //}
                //else if (typeIList1.IsAssignableFrom(type))
                //{
                //    OnShowNonsupportMemberGUI(label, $"不支持该类型 IList<>2");
                //}
                //else if (interfaces.Contains(typeIList1))
                //{
                //    OnShowNonsupportMemberGUI(label, $"不支持该类型 IList<>3");
                //}
                //else if (genericTypeDefinitionInterfaces?.Contains(typeIList1) ?? false)
                //{
                //    OnShowNonsupportMemberGUI(label, $"不支持该类型 IList<>4");
                //}
                //else if (type.GetInterface("IList^1") != null)
                //{
                //    OnShowNonsupportMemberGUI(label, $"不支持该类型 IList<>5");
                //}

                //else 
                if (type == typeIList || typeIList.IsAssignableFrom(type))
                {
                    OnShowNonsupportMemberGUI(label, $"不支持该类型 IList 1");
                }
                //else if (genericTypeDefinition != null && typeIList.IsAssignableFrom(genericTypeDefinition))
                //{
                //    OnShowNonsupportMemberGUI(label, $"不支持该类型 IList 3");
                //}

                return r;
            }

            /// 处理列表 gui
            // 作为元素
            protected virtual object ListFieldGUI(ref Rect rect, GUIContent label, IList v, Type type, int index)
            {
                // 怎么获取到代表 info 的 ReorderableList 实例
                // 判断是 target 的成员还是 objInstance 的成员
                GenericsTypeGUI gui = null;

                //// 判断是否自身
                //if (gui)
                //{
                //    gui = this;
                //}
                //else
                {
                    gui = ExpectChild(v, index, out bool find);
                    if (!find)
                    {
                        gui._foldout = v != null && v.Count <= 100;
                        gui.referenceType = type;
                    }
                }

                ListFieldGUI(ref rect, label, gui, v, type);
                return gui?.target;
            }
            protected virtual object ListFieldGUI(ref Rect rect, GUIContent label, IList v, Type type, MemberInfo info)
            {
                // 怎么获取到代表 info 的 ReorderableList 实例
                // 判断是 target 的成员还是 objInstance 的成员
                GenericsTypeGUI gui = null;

                // 判断是否自身
                if (info == null || info == this.info)
                {
                    gui = this;
                }
                else
                {
                    gui = ExpectChild(v, info, out bool find);
                    if (!find)
                    {
                        gui._foldout = v != null && v.Count <= 100;
                        gui.referenceType = type;
                    }
                }

                ListFieldGUI(ref rect, label, gui, v, type);
                return gui?.target;
            }
            // 作为元素
            protected virtual object ListFieldGUI(GUIContent label, IList v, Type type, int index)
            {
                // 怎么获取到代表 info 的 ReorderableList 实例
                // 判断是 target 的成员还是 objInstance 的成员
                GenericsTypeGUI gui = null;

                //// 判断是否自身
                //if (info == null || info == this.info)
                //{
                //    gui = this;
                //}
                //else
                {
                    gui = ExpectChild(v, index
                        , out bool find
                        );
                    if (!find)
                    {
                        gui._foldout = v != null && v.Count <= 100;
                        gui.referenceType = type;
                    }
                }

                ListFieldGUI(label, gui, v, type);
                return gui?.target;
            }
            protected virtual object ListFieldGUI(GUIContent label, IList v, Type type, MemberInfo info)
            {
                // 怎么获取到代表 info 的 ReorderableList 实例
                // 判断是 target 的成员还是 objInstance 的成员
                GenericsTypeGUI gui = null;

                // 判断是否自身
                if (info == null || info == this.info)
                {
                    gui = this;
                }
                else
                {
                    gui = ExpectChild(v, info
                        , out bool find
                        );
                    if (!find)
                    {
                        gui._foldout = v != null && v.Count <= 100;
                        gui.referenceType = type;
                    }
                }

                ListFieldGUI(label, gui, v, type);
                return gui?.target;
            }
            // 最终处理
            protected virtual void ListFieldGUI(ref Rect rect, GUIContent label, GenericsTypeGUI gui, IList v, Type type)
            {
                if (v == null)
                {
                    OnCreateTargetInstanceGUI(ref rect, label, gui, type);
                }
                else
                {
                    gui.target = v;
                    if (gui._rlist == null)
                    {
                        Type elementType = type.GetElementType();
                        // 如果是泛型，则应获取其泛型参数
                        if (elementType == null && type.IsGenericType)
                        {
                            elementType = type.GetGenericArguments()[0];
                        }

                        CreateReorderableListGUI(v, elementType, label, gui);
                    }
                    gui._rlist.list = v;

                    gui._foldout = EditorGUI.Foldout(rect, gui._foldout, label, true);
                    rect.y += EditorGUIUtility.singleLineHeight;
                    if (gui._foldout)
                    {
                        gui._rlist.DoList(rect);
                        // 加上列表高度
                        rect.y += gui._rlist.GetHeight();
                    }

                    //rect.y += EditorGUIUtility.singleLineHeight;
                }
            }
            protected virtual void ListFieldGUI(GUIContent label, GenericsTypeGUI gui, IList v, Type type)
            {
                if (v == null)
                {
                    OnCreateTargetInstanceGUI(label, gui, type);
                }
                else
                {
                    gui.target = v;
                    if (gui._rlist == null)
                    {
                        Type elementType = type.GetElementType();
                        // 如果是泛型，例如 List<>，则应获取其泛型参数
                        if (elementType == null && type.IsGenericType)
                        {
                            elementType = type.GetGenericArguments()[0];
                        }

                        CreateReorderableListGUI(v, elementType, label, gui);
                    }
                    gui._rlist.list = v;

                    gui._foldout = EditorGUILayout.Foldout(gui._foldout, label, true);
                    if (gui._foldout)
                    {
                        gui._rlist.DoLayoutList();
                    }
                }
            }
            /// <summary>
            /// 创建 ReorderableList 实例并做初始绑定
            /// </summary>
            /// <param name="v"></param>
            /// <param name="elementType"></param>
            /// <param name="label"></param>
            /// <param name="listGui"></param>
            /// <returns></returns>
            protected virtual ReorderableList CreateReorderableListGUI(IList v, Type elementType, GUIContent label, GenericsTypeGUI listGui)
            {
                // 保证每一个子元素都有 gui

                var rlist = new ReorderableList(v, elementType, true, false, true, true);

                listGui.referenceElementType = elementType;
                listGui._rlist = rlist;

                Rect eStart = default;
                Rect eEnd = default;
                Dictionary<int, float> tempElementHeightDic = new Dictionary<int, float>();

                // 绘制 元素
                rlist.drawElementCallback = (rect, index, isActive, isFocused) =>
                {
                    //Debug.Log(index);

                    // 使两个 gui 之间有间隙
                    rect.height = EditorGUIUtility.singleLineHeight;
                    rect.height -= 2;
                    rect.y += 4;
                    // 前面的拖拽留空
                    float fSpace = 10;
                    rect.width -= fSpace;
                    rect.x += fSpace;

                    eStart = rect;

                    //Type etype = gui.referenceType.GetElementType();

                    Type etype = rlist.list[index]?.GetType() ?? elementType;// 获取实际类型，否则使用指定元素类型
                    //Type etype = elementType;// TypeGUI 内部处理，否则使用上面的
                    var eValue = rlist.list[index];
                    var eGui = listGui.ExpectChild(eValue, index);// 创建子 gui
                    if (eGui.type != etype)
                    {
                        eGui.target = eValue;
                        eGui.Init();
                    }
                    eGui.referenceType = elementType;

                    //var eLabel = new GUIContent($"元素 {index}", $"类型：{etype.FullName}");
                    var eLabel = eGui.GetLabel($"元素 {index}", etype, elementType, null);
                    bool change = listGui.TypeGUI(ref rect, eLabel, eValue, etype, out var newV, index);
                    if (change)
                    {
                        setValueStartEvent?.Invoke(rlist.list, rlist.list, listGui._info);

                        rlist.list[index] = newV;

                        setValueEndEvent?.Invoke(rlist.list, rlist.list, listGui._info);
                        //gui.SetValue();
                        SyncSelfTargetValue();
                    }

                    rect.y += 4;
                    eEnd = rect;
                    tempElementHeightDic[index] = eEnd.y - eStart.y;
                };

                // 获取绘制 元素高度
                rlist.elementHeightCallback = (index) =>
                {
                    float h = EditorGUIUtility.singleLineHeight;
                    if (tempElementHeightDic.TryGetValue(index, out h))
                    {

                    }
                    return h;
                };

                // 重新排序 元素，只是通知，实际已经调整过
                rlist.onReorderCallbackWithDetails = (_, oldIndex, newIndex) =>
                {
                    //LogIList(rlist.list);
                    setValueStartEvent?.Invoke(rlist.list, rlist.list, listGui._info);

                    // 这里不需要交换，已经调整过
                    //var temp = rlist.list[oldIndex];
                    //rlist.list[oldIndex] = rlist.list[newIndex];
                    //rlist.list[newIndex] = temp;

                    //if (IsGenericsType(rlist.list[oldIndex], rlist.list[oldIndex]?.GetType() ?? elementType)
                    //|| IsGenericsType(rlist.list[newIndex], rlist.list[newIndex]?.GetType() ?? elementType))
                    {
                        var tempGUI = listGui._childs[oldIndex];
                        listGui._childs[oldIndex] = listGui._childs[newIndex];
                        listGui._childs[newIndex] = tempGUI;
                    }

                    setValueEndEvent?.Invoke(rlist.list, rlist.list, listGui._info);
                    SyncSelfTargetValue();
                    //LogIList(rlist.list);
                };

                // 添加 元素
                rlist.onAddCallback = (_) =>
                {
                    setValueStartEvent?.Invoke(rlist.list, rlist.list, listGui._info);

                    int index = rlist.index > -1 && rlist.index < rlist.count ? rlist.index : rlist.count - 1;
                    // 获取实际类型，否则使用指定元素类型
                    var e = rlist.list.Count > 0 ? rlist.list[rlist.list.Count - 1] : null;
                    Type etype = e?.GetType() ?? elementType;

                    object inst = CreateInstance(etype);

                    //if (IsGenericsType(inst, etype))
                    {
                        var eGui = listGui.AddChild(inst, null, index + 1);
                        eGui.referenceType = elementType;
                    }
                    if (listGui.type.IsArray)
                    {
                        // 因为数组大小是不可变的，处理：NotSupportedException: Collection was of a fixed size.
                        //Activator
                        var tempList = new ArrayList(rlist.list);
                        tempList.Insert(index + 1, inst);
                        rlist.list = tempList.ToArray(elementType);
                        listGui.target = rlist.list;
                        listGui.SetValue();
                    }
                    else
                    {
                        rlist.list.Insert(index + 1, inst);
                    }

                    setValueEndEvent?.Invoke(rlist.list, rlist.list, listGui._info);
                    //gui.SetValue();
                    SyncSelfTargetValue();
                };
                // 移除 元素
                rlist.onRemoveCallback = (_) =>
                {
                    if (rlist.list.Count < 1) return;

                    setValueStartEvent?.Invoke(rlist.list, rlist.list, listGui._info);

                    int index = rlist.index > -1 ? rlist.index : rlist.count - 1;

                    Type etype = rlist.list[index]?.GetType() ?? elementType;
                    //if (IsGenericsType(rlist.list[index], etype))
                    {
                        listGui.RemoveChild(index);
                    }
                    if (listGui.type.IsArray)
                    {
                        // 因为数组大小是不可变的，处理：NotSupportedException: Collection was of a fixed size.
                        var tempList = new ArrayList(rlist.list);
                        tempList.RemoveAt(index);
                        rlist.list = tempList.ToArray(elementType);
                        listGui.target = rlist.list;
                        listGui.SetValue();
                    }
                    else
                    {
                        rlist.list.RemoveAt(index);
                    }

                    setValueEndEvent?.Invoke(rlist.list, rlist.list, listGui._info);
                    //gui.SetValue();
                    SyncSelfTargetValue();
                };

                return rlist;
            }
            static void LogIList(IList v)
            {
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < v.Count; i++)
                {
                    var item = v[i];
                    sb.Append(item.ToString());
                    if (i < v.Count - 1) sb.Append(", ");
                }
                Debug.Log(sb.ToString());
            }

            // 经过处理的文本字段 gui
            protected virtual string TextFieldGUI(ref Rect rect, GUIContent label, string text, MemberInfo info = null)
            {
                if (text != null
                    && (text.Length > 50
                    || text.Contains("\n")
                    || text.Contains("\t")
                    //|| text.Split('\t').Length > 10
                    ))
                {
                    var r = rect;

                    float currW = rect.width;
                    float s_h = EditorStyles.textArea.CalcHeight(GetTempGUIContent(text), currW);
                    float minH = EditorGUIUtility.singleLineHeight * minTextLine;
                    float maxH = EditorGUIUtility.singleLineHeight * maxTextLine;
                    float r_h = Math.Max(Math.Min(s_h, maxH), minH);// 滑动高度最小、最大值

                    bool f = true;
                    GenericsTypeGUI gui = info == null ? this : ExpectChild(text, info, out f);
                    if (!f)
                    {
                        gui.foldout = s_h <= maxH;
                    }
                    gui.foldout = EditorGUI.Foldout(rect, gui.foldout, label, true);

                    if (gui.foldout)
                    {
                        rect.y += EditorGUIUtility.singleLineHeight;

                        Rect viewRect = rect;

                        rect.height = r_h;

                        viewRect.height = Math.Max(s_h, minH);// 文本高度最小值
                        gui._scrollPosition = GUI.BeginScrollView(rect, gui._scrollPosition
                            , viewRect
                            );

                        text = EditorGUI.TextArea(viewRect, text);
                        //text = GUI.TextArea(rect, text);

                        GUI.EndScrollView();

                        rect.y += rect.height - EditorGUIUtility.singleLineHeight;
                        rect.height = r.height;
                    }
                }
                else
                {
                    text = EditorGUI.TextField(rect, label, text);
                }

                //rect.y += EditorGUIUtility.singleLineHeight;

                return text;
            }
            protected virtual string TextFieldGUI(ref Rect rect, GUIContent label, string text, int index)
            {
                if (text != null
                    && (text.Length > 50
                    || text.Contains("\n")
                    || text.Contains("\t")
                    //|| text.Split('\t').Length > 10
                    ))
                {
                    var r = rect;

                    float currW = rect.width;
                    float s_h = EditorStyles.textArea.CalcHeight(GetTempGUIContent(text), currW);
                    float minH = EditorGUIUtility.singleLineHeight * minTextLine;
                    float maxH = EditorGUIUtility.singleLineHeight * maxTextLine;
                    float r_h = Math.Max(Math.Min(s_h, maxH), minH);

                    bool f = true;
                    GenericsTypeGUI gui = ExpectChild(text, index, out f);
                    if (!f)
                    {
                        gui.foldout = s_h <= maxH;
                    }
                    gui.foldout = EditorGUI.Foldout(rect, gui.foldout, label, true);

                    if (gui.foldout)
                    {
                        rect.y += EditorGUIUtility.singleLineHeight;

                        Rect viewRect = rect;

                        rect.height = r_h;

                        viewRect.height = Math.Max(s_h, minH);
                        gui._scrollPosition = GUI.BeginScrollView(rect, gui._scrollPosition
                            , viewRect
                            );

                        text = EditorGUI.TextArea(viewRect, text);
                        //text = GUI.TextArea(rect, text);

                        GUI.EndScrollView();

                        rect.y += rect.height - EditorGUIUtility.singleLineHeight;
                        rect.height = r.height;

                    }
                }
                else
                {
                    text = EditorGUI.TextField(rect, label, text);
                }

                //rect.y += EditorGUIUtility.singleLineHeight;

                return text;
            }
            protected virtual string TextFieldGUI(GUIContent label, string text, MemberInfo info = null)
            {
                if (text != null
                    && (text.Length > 50
                    || text.Contains("\n")
                    || text.Contains("\t")
                    //|| text.Split('\t').Length > 10
                    ))
                {
                    //Rect rect = GUILayoutUtility.GetLastRect();
                    //float currW = rect.width;
                    float currW = EditorGUIUtility.currentViewWidth;
                    float s_h = EditorStyles.textArea.CalcHeight(GetTempGUIContent(text), currW);
                    float minH = EditorGUIUtility.singleLineHeight * minTextLine;
                    float maxH = EditorGUIUtility.singleLineHeight * maxTextLine;
                    float r_h = Math.Max(Math.Min(s_h, maxH), minH);

                    bool f = true;
                    GenericsTypeGUI gui = info == null ? this : ExpectChild(text, info, out f);
                    if (!f)
                    {
                        gui.foldout = s_h <= maxH;
                    }
                    gui.foldout = EditorGUILayout.Foldout(gui.foldout, label, true);

                    if (gui.foldout)
                    {
                        gui._scrollPosition = EditorGUILayout.BeginScrollView(gui._scrollPosition
                            , GUILayout.MaxHeight(r_h)
                            );

                        text = EditorGUILayout.TextArea(text, GUILayout.MinHeight(Math.Max(s_h, minH)));
                        //text = GUILayout.TextArea(text);

                        EditorGUILayout.EndScrollView();
                    }
                }
                else
                {
                    text = EditorGUILayout.TextField(label, text);
                }
                return text;
            }

            /// <summary>显示不支持成员的 gui</summary>
            protected void OnShowNonsupportMemberGUI(ref Rect rect, GUIContent label) => OnShowNonsupportMemberGUI(ref rect, label, $"不支持该类型");
            /// <summary>显示不支持成员的 gui</summary>
            protected void OnShowNonsupportMemberGUI(ref Rect rect, GUIContent label, string text)
            {
                if (showNonsupportMember)
                {
                    var r = rect;

                    r.width *= 0.5f;
                    EditorGUI.PrefixLabel(rect, label);
                    r.x += r.width;
                    EditorGUI.LabelField(r, text);

                    rect.y += EditorGUIUtility.singleLineHeight;
                }
            }
            /// <summary>显示不支持成员的 gui</summary>
            protected void OnShowNonsupportMemberGUI(GUIContent label) => OnShowNonsupportMemberGUI(label, $"不支持该类型");
            /// <summary>显示不支持成员的 gui</summary>
            protected void OnShowNonsupportMemberGUI(GUIContent label, string text)
            {
                if (showNonsupportMember)
                {
                    //EditorGUILayout.SelectableLabel($"{displayName}，不支持该类型。");
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.PrefixLabel(label);
                    int l = EditorGUI.indentLevel;
                    EditorGUI.indentLevel = 0;
                    EditorGUILayout.LabelField(text);
                    EditorGUI.indentLevel = l;
                    EditorGUILayout.EndHorizontal();
                }
            }

            /// <summary>可为该类型创建实例</summary>
            protected virtual bool CanCreateInstanceGenerics(Type type)
            {
                return type.IsArray || type.GetConstructor(BindingFlags.Instance | BindingFlags.Public, null, Type.EmptyTypes, null) != null// 有不带参数的构造函数
                    ;
            }

            /// <summary><see cref="target"/> 是否自定义类型</summary>
            public bool IsGenericsType() => IsGenericsType(target, type ?? GetType(info));
            /// <summary>是否自定义类型</summary>
            public bool IsGenericsType(object v) => IsGenericsType(v, GetType(v));
            /// <summary>是否自定义类型</summary>
            /// <remarks>如果重写了 <see cref="TypeGUI(GUIContent, object, Type, out object, MemberInfo)"/>，扩展了类型 gui，必须也重写本方法，添加对应类型</remarks>
            public virtual bool IsGenericsType(object v, Type type)
            {
                Type t = v?.GetType() ?? type;
                bool r =
                       TypeCast(typeof(int), t)
                    || TypeCast(typeof(uint), t)
                    || TypeCast(typeof(long), t)
                    || TypeCast(typeof(ulong), t)
                    || TypeCast(typeof(float), t)
                    || TypeCast(typeof(double), t)
                    || TypeCast(typeof(bool), t)
                    || TypeCast(typeof(string), t)
                    || TypeCast(typeof(char), t)

                    || TypeCast(typeof(Hash128), t)
                    || TypeCast(typeof(LayerMask), t)
                    || TypeCast(typeof(Vector2), t)
                    || TypeCast(typeof(Vector2Int), t)
                    || TypeCast(typeof(Vector3), t)
                    || TypeCast(typeof(Vector3Int), t)
                    || TypeCast(typeof(Vector4), t)
                    || TypeCast(typeof(Quaternion), t)
                    || TypeCast(typeof(Color), t)
                    || TypeCast(typeof(Color32), t)
                    || TypeCast(typeof(Rect), t)
                    || TypeCast(typeof(RectInt), t)
                    || TypeCast(typeof(Bounds), t)
                    || TypeCast(typeof(BoundsInt), t)

                    || TypeCast(typeof(Enum), t)
                    || TypeCast(typeof(AnimationCurve), t)
                    || TypeCast(typeof(Gradient), t)
                    || TypeCast(typeof(Object), t)

                #region 
                    //TypeCast(v, out int _, type)
                    //|| TypeCast(v, out uint _, type)
                    //|| TypeCast(v, out long _, type)
                    //|| TypeCast(v, out ulong _, type)
                    //|| TypeCast(v, out float _, type)
                    //|| TypeCast(v, out double _, type)
                    //|| TypeCast(v, out bool _, type)
                    //|| TypeCast(v, out string _, type)
                    //|| TypeCast(v, out char _, type)

                    //|| TypeCast(v, out Hash128 _, type)
                    //|| TypeCast(v, out LayerMask _, type)
                    //|| TypeCast(v, out Vector2 _, type)
                    //|| TypeCast(v, out Vector2Int _, type)
                    //|| TypeCast(v, out Vector3 _, type)
                    //|| TypeCast(v, out Vector3Int _, type)
                    //|| TypeCast(v, out Vector4 _, type)
                    //|| TypeCast(v, out Quaternion _, type)
                    //|| TypeCast(v, out Color _, type)
                    //|| TypeCast(v, out Color32 _, type)
                    //|| TypeCast(v, out Rect _, type)
                    //|| TypeCast(v, out RectInt _, type)
                    //|| TypeCast(v, out Bounds _, type)
                    //|| TypeCast(v, out BoundsInt _, type)

                    //|| TypeCast(v, out Enum _, type, true)
                    //|| TypeCast(v, out AnimationCurve _, type)
                    //|| TypeCast(v, out Gradient _, type)
                    //|| TypeCast(v, out Object _, type, true) 
                #endregion
                    ;

                return !r;// 不能转换为上述类型，则以通用类型显示 gui
            }

            /// <summary>其他自定义类型字段</summary>
            public virtual object GenericsFieldGUI(ref Rect rect, GUIContent label, object v, Type type, MemberInfo info)
            {
                GenericsTypeGUI gui = null;

                // 处理循环依赖
                if (CheckGenericsFieldType(type, out bool lineal, out bool allowSelfLoop, out bool depthStandards))
                {
                    gui = ExpectChild(v, info);

                    if (v == null)
                    {
                        OnCreateTargetInstanceGUI(ref rect, label, gui, type);
                    }
                    else
                    {
                        gui.target = v;

                        if (gui.isElement)
                        {
                            gui.OnGUI(ref rect, label);
                        }
                        else
                        {
                            gui.OnGUI(ref rect, true);
                        }
                    }
                }
                else
                {
                    if (!depthStandards)
                        OnShowNonsupportMemberGUI(ref rect, label, $"已达深度上限 {maxDepth}");
                    else if (!allowSelfLoop)
                        OnShowNonsupportMemberGUI(ref rect, label, $"不支持自循环类型");
                    else
                        OnShowNonsupportMemberGUI(ref rect, label);
                }

                return gui?.target;
            }
            public virtual object GenericsFieldGUI(ref Rect rect, GUIContent label, object v, Type type, int index)
            {
                GenericsTypeGUI gui = null;

                // 处理循环依赖
                if (CheckGenericsFieldType(type, out bool lineal, out bool allowSelfLoop, out bool depthStandards))
                {
                    gui = ExpectChild(v, index);

                    if (v == null)
                    {
                        OnCreateTargetInstanceGUI(ref rect, label, gui, type);
                    }
                    else
                    {
                        gui.target = v;

                        if (gui.isElement)
                        {
                            gui.OnGUI(ref rect, label);
                        }
                        else
                        {
                            gui.OnGUI(ref rect, true);
                        }
                    }
                }
                else
                {
                    if (!depthStandards)
                        OnShowNonsupportMemberGUI(ref rect, label, $"已达深度上限 {maxDepth}");
                    else if (!allowSelfLoop)
                        OnShowNonsupportMemberGUI(ref rect, label, $"不支持自循环类型");
                    else
                        OnShowNonsupportMemberGUI(ref rect, label);
                }

                return gui?.target;
            }
            /// <summary>其他自定义类型字段</summary>
            public virtual object GenericsFieldGUI(GUIContent label, object v, Type type, MemberInfo info)
            {
                GenericsTypeGUI gui = null;

                // 处理循环依赖
                if (CheckGenericsFieldType(type, out bool lineal, out bool allowSelfLoop, out bool depthStandards))
                {
                    gui = ExpectChild(v, info);

                    if (v == null)
                    {
                        OnCreateTargetInstanceGUI(label, gui, type);
                    }
                    else
                    {
                        gui.target = v;

                        if (gui.isElement)
                        {
                            gui.OnGUI(label);
                        }
                        else
                        {
                            gui.OnGUI(true);
                        }
                    }
                }
                else
                {
                    if (!depthStandards)
                        OnShowNonsupportMemberGUI(label, $"已达深度上限 {maxDepth}");
                    else if (!allowSelfLoop)
                        OnShowNonsupportMemberGUI(label, $"不支持自循环类型");
                    else
                        OnShowNonsupportMemberGUI(label);
                }

                return gui?.target;
            }
            public virtual object GenericsFieldGUI(GUIContent label, object v, Type type, int index)
            {
                GenericsTypeGUI gui = null;

                // 处理循环依赖
                if (CheckGenericsFieldType(type, out bool lineal, out bool allowSelfLoop, out bool depthStandards))
                {
                    gui = ExpectChild(v, index);

                    if (v == null)
                    {
                        OnCreateTargetInstanceGUI(label, gui, type);
                    }
                    else
                    {
                        gui.target = v;

                        if (gui.isElement)
                        {
                            gui.OnGUI(label);
                        }
                        else
                        {
                            gui.OnGUI(true);
                        }
                    }
                }
                else
                {
                    if (!depthStandards)
                        OnShowNonsupportMemberGUI(label, $"已达深度上限 {maxDepth}");
                    else if (!allowSelfLoop)
                        OnShowNonsupportMemberGUI(label, $"不支持自循环类型");
                    else
                        OnShowNonsupportMemberGUI(label);
                }

                return gui?.target;
            }

            /// <summary>检查指定自定义类型和自身类型的关系</summary>
            protected virtual bool CheckGenericsFieldType(Type type, out bool lineal, out bool allowSelfLoop, out bool depthStandards)
            {
                // 处理循环依赖
                // 当 type 继承 this.type 时必定循环
                lineal = IsLineal(type, this.type);// 是否有直接关系
                allowSelfLoop = lineal && allowSelfNested;
                depthStandards = currentDepth < maxDepth;
                bool r = (!lineal       // 没有直接关系
                    || allowSelfLoop)   // 有直接关系，但允许自循环
                    && depthStandards   // 深度未达上限
                    ;
                return r;
            }

            /// <summary>为 <see cref="target"/> 创建实例 gui</summary>
            protected virtual void OnCreateTargetInstanceGUI(ref Rect rect, GUIContent label, GenericsTypeGUI gui, Type type)
            {
                var r = rect;
                r.width *= 0.5f;
                EditorGUI.PrefixLabel(r, label);
                r.x += r.width;
                // 如果值是空的，则让其创建一个
                if (CanCreateInstanceGenerics(type))
                {
                    if (GUI.Button(r, "创建实例"))
                    {
                        CreateTargetInstance(gui, type);
                        gui._foldout = true;
                    }
                }
                else
                {
                    EditorGUI.LabelField(r, "该类型不支持直接创建实例");
                }

                rect.y += EditorGUIUtility.singleLineHeight;
            }
            /// <summary>为 <see cref="target"/> 创建实例 gui</summary>
            protected virtual void OnCreateTargetInstanceGUI(GUIContent label, GenericsTypeGUI gui, Type type)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.PrefixLabel(label);
                // 如果值是空的，则让其创建一个
                if (CanCreateInstanceGenerics(type))
                {
                    if (GUILayout.Button("创建实例"))
                    {
                        CreateTargetInstance(gui, type);
                        gui._foldout = true;
                    }
                }
                else
                {
                    int l = EditorGUI.indentLevel;
                    EditorGUI.indentLevel = 0;
                    EditorGUILayout.LabelField("该类型不支持直接创建实例");
                    EditorGUI.indentLevel = l;
                }
                EditorGUILayout.EndHorizontal();
            }


            /// <summary>期望获取子 <see cref="GenericsTypeGUI"/>，查找是否已有成员的 gui 实例，没有则创建</summary>
            protected virtual GenericsTypeGUI ExpectChild(object v, MemberInfo info)
            {
                var gui = ExpectChild(v, info, out bool _);
                return gui;
            }
            /// <summary>期望获取子 <see cref="GenericsTypeGUI"/>，查找是否已有成员的 gui 实例，没有则创建</summary>
            /// <remarks><paramref name="find"/>：true 是找到已有的，否则是创建的</remarks>
            protected virtual GenericsTypeGUI ExpectChild(object v, MemberInfo info, out bool find)
            {
                find = true;
                var gui = _childs.Find(v => v._info == info);
                if (gui == null)
                {
                    find = false;
                    gui = AddChild(v, info);
                }
                return gui;
            }

            protected virtual GenericsTypeGUI ExpectChild(object v, int index)
            {
                var gui = ExpectChild(v, index, out bool _);
                return gui;
            }
            // 未找到会在最后添加一个，而非索引处
            protected virtual GenericsTypeGUI ExpectChild(object v, int index, out bool find)
            {
                find = GetChild(index, out GenericsTypeGUI gui);
                if (!find)
                {
                    gui = AddChild(v, null);
                }
                return gui;
            }
            protected virtual bool GetChild(int index, out GenericsTypeGUI gui)
            {
                gui = null;
                if (index > -1 && _childs.Count > index)
                {
                    gui = _childs[index];
                    return true;
                }
                return false;
            }

            /// <summary>移除子 <see cref="GenericsTypeGUI"/></summary>
            protected virtual void RemoveChild(int index)
            {
                if (index > -1 && _childs.Count > index)
                {
                    _childs.RemoveAt(index);
                }
            }
            /// <summary>添加子 <see cref="GenericsTypeGUI"/></summary>
            protected virtual GenericsTypeGUI AddChild(object v, MemberInfo info)
            {
                var gui = CreateGenericsTypeGUIInstance(v, info);
                _childs.Add(gui);
                gui.parent = this;
                gui.foldout = false;
                gui.showInheritRelation = false;
                return gui;
            }
            protected virtual GenericsTypeGUI AddChild(object v, MemberInfo info, int index)
            {
                var gui = CreateGenericsTypeGUIInstance(v, info);
                _childs.Insert(index, gui);
                gui.parent = this;
                gui.foldout = false;
                gui.showInheritRelation = false;
                return gui;
            }
            /// <summary>创建 <see cref="GenericsTypeGUI"/></summary>
            protected virtual GenericsTypeGUI CreateGenericsTypeGUIInstance(object v, MemberInfo info)
            {
                var gui = createChildEvent?.Invoke();
                gui ??= new GenericsTypeGUI();
                gui.target = v;
                gui._info = info;
                gui.Init();
                return gui;
            }

            /// <summary>为 <see cref="target"/> 创建实例</summary>
            protected virtual GenericsTypeGUI CreateTargetInstance(GenericsTypeGUI gui, Type type)
            {
                gui.target = CreateInstance(type);
                gui.SetValue();
                return gui;
            }

            /// <summary>
            /// 创建指定类型的实例
            /// </summary>
            /// <param name="type"></param>
            /// <returns></returns>
            public virtual object CreateInstance(Type type)
            {
                object obj = null;
                if (type.IsArray)
                {
                    obj = Array.CreateInstance(type.GetElementType(), 0);
                }
                else
                {
                    obj = Activator.CreateInstance(type);
                }
                return obj;
            }

            /// <summary>
            /// 创建时需要特殊处理，在 <see cref="Init"/> 最后调用
            /// </summary>
            /// <param name="gui"></param>
            protected virtual void CreateSpecialHandling(GenericsTypeGUI gui)
            {
                // Matrix4x4 已解决有索引器引起的异常问题
                //if (gui.type == typeof(Matrix4x4) || GetType(gui.info) == typeof(Matrix4x4))
                //{
                //    gui.showProperty = false;
                //    gui.showFieldFoldout = false;
                //    gui._memberEndGUIEvent -= MemberEndGUITypeEvent_Inline_NoShowPropertyGUI;
                //    gui._memberEndGUIEvent += MemberEndGUITypeEvent_Inline_NoShowPropertyGUI;
                //    gui._memberEndRectGUIEvent -= MemberEndGUITypeEvent_Inline_NoShowPropertyGUI;
                //    gui._memberEndRectGUIEvent += MemberEndGUITypeEvent_Inline_NoShowPropertyGUI;
                //}
            }
            // 内联
            protected void MemberEndGUITypeEvent_Inline_NoShowPropertyGUI()
            {
                //EditorGUILayout.LabelField("该类型不支持显示属性");
                EditorGUILayout.HelpBox("该类型不支持显示属性", MessageType.Info);
            }
            protected void MemberEndGUITypeEvent_Inline_NoShowPropertyGUI(ref Rect rect)
            {
                //EditorGUILayout.LabelField("该类型不支持显示属性");
                EditorGUI.HelpBox(rect, "该类型不支持显示属性", MessageType.Info);
                rect.y += EditorGUIUtility.singleLineHeight;
            }

            // 获取标签
            protected virtual GUIContent GetLabel(TypeGUIArgs args, bool showDepth = false)
            {
                var label = GetLabel(args.name, args.type, args.declareType, args.info, showDepth);
                //label.tooltip = $"{label.tooltip}\r\n{}";
                return label;
            }
            protected virtual GUIContent GetLabel(string name, Type type, MemberInfo info, bool showDepth = false) => GetLabel(name, type, type, info, showDepth);
            // type：实例类型；declareType：声名时的类型
            protected virtual GUIContent GetLabel(string name, Type type, Type declareType, MemberInfo info, bool showDepth = false)
            {
                string dispalyNameS = name != null && name.Length < 15 ? "\t" : null;
                string dispalyTypeName = type != null ? $"{dispalyNameS}（{type.Name}）" : null;
                string dispalyName = $"{name}{dispalyTypeName}";

                StringBuilder tooltip = new StringBuilder();
                if (type != declareType)
                    tooltip.Append($"声明类型：{declareType.FullName}").AppendLine();
                if (type != null)
                    tooltip.Append($"实例类型：{type.FullName}");
                if (info != null)
                    tooltip.AppendLine().Append($@"成员访问修饰符：{GetMemberDeclareVisit(info)}");
                //if (IsGenericsType(target))
                if (showDepth)
                {
                    tooltip.AppendLine().Append($"gui 深度：当前：{currentDepth}，总深度：{depth}");
                }
                var label = new GUIContent(dispalyName, tooltip.ToString());
                return label;
            }

            /// <summary>
            /// 同步自身目标值，主要用于值类型同步
            /// </summary>
            protected virtual void SyncSelfTargetValue()
            {
                var t = this;
                while (t != null && t.parent != null && t.objInstance != null && t.type.IsValueType)
                {
                    //t.SetValue(t.objInstance, t._info, t.target);
                    t.SetValue();
                    t = t.parent;
                }
            }
            /// <summary>自身 <see cref="info"/> 成员或 <see cref="index"/> 索引元素设置值</summary>
            protected virtual bool SetValue()
            {
                if (parent != null && objInstance != null)
                {
                    // 成员
                    if (SetValue(objInstance, _info, target))
                    {
                        return true;
                    }
                    else
                    {
                        // 元素
                        return SetValue(objInstance, parent.type, parent.referenceElementType, index, target);
                    }
                }
                return false;
            }
            /// <summary>给目标成员设置值</summary>
            protected virtual bool SetValue(object target, Type targetType, Type elmentType, int index, object value)
            {
                if (TypeCast(target, out IList vIList, targetType, true))
                {
                    if (!IsLineal(elmentType, value?.GetType())) return false;

                    object oldValue = vIList[index];
                    setValueStartEvent?.Invoke(oldValue, value, null);
                    vIList[index] = value;
                    setValueStartEvent?.Invoke(oldValue, value, null);
                    return true;
                }
                return false;
            }
            /// <summary>给目标成员设置值</summary>
            protected virtual bool SetValue(object target, MemberInfo info, object value)
            {
                if (target == null || info == null) return false;

                if (info is FieldInfo fieldInfo)
                {
                    //if (fieldInfo.FieldType != value?.GetType()) return false;
                    if (!IsLineal(fieldInfo.FieldType, value?.GetType())) return false;

                    object oldValue = fieldInfo.GetValue(target);
                    setValueStartEvent?.Invoke(oldValue, value, info);
                    fieldInfo.SetValue(target, value);
                    setValueEndEvent?.Invoke(oldValue, value, info);
                    return true;
                }
                else if (info is PropertyInfo propertyInfo)
                {
                    //if (propertyInfo.PropertyType != value?.GetType()) return false;
                    if (!IsLineal(propertyInfo.PropertyType, value?.GetType())) return false;

                    if (propertyInfo.CanWrite)
                    {
                        object oldValue = propertyInfo.GetValue(target);
                        setValueStartEvent?.Invoke(oldValue, value, info);
                        propertyInfo.SetValue(target, value);
                        setValueEndEvent?.Invoke(oldValue, value, info);
                    }
                    return propertyInfo.CanWrite;
                }
                else
                {
                    return false;
                }
            }

            // 获取 TypeGUIArgs 实例，info 为 owner 的成员
            public TypeGUIArgs GetTypeGUIArgs(FieldInfo info) => GetTypeGUIArgs(info, target);
            public TypeGUIArgs GetTypeGUIArgs(PropertyInfo info) => GetTypeGUIArgs(info, target);
            public TypeGUIArgs GetTypeGUIArgs(FieldInfo info, object owner, bool showLabel = true)
            {
                object value = owner != null ? info.GetValue(owner) : null;
                Type type = value?.GetType() ?? info.FieldType;
                var r = new TypeGUIArgs();
                r.owner = owner;
                r.info = info;
                r.name = info.Name;
                r.readOnly = info.IsInitOnly;
                r.showGUI = true;
                r.isIndexer = false;
                r.value = value;
                r.type = type;
                r.declareType = info.FieldType;
                r.showLabel = showLabel;
                r.setter = (newValue) =>
                {
                    setValueStartEvent?.Invoke(value, newValue, info);
                    info.SetValue(owner, newValue);
                    setValueStartEvent?.Invoke(value, newValue, info);
                    return true;
                };
                r.label = GetLabel(r, IsGenericsType(value, type));
                return r;
            }
            public TypeGUIArgs GetTypeGUIArgs(PropertyInfo info, object owner, bool showLabel = true)
            {
                var r = new TypeGUIArgs();
                bool readOnly = IsReadOnly(info);
                object value = null;
                try
                {
                    // 处理索引器
                    //var m = info.GetMethod ?? info.SetMethod;
                    var ips = info.GetIndexParameters();
                    if (ips.Length == 0)
                    {
                        //value = !IsWriteOnly(info) && owner != null ? info.GetValue(owner) : null;
                        if (!IsWriteOnly(info) && owner != null)
                        {
                            // 收集 unity 的错误消息
                            void _gather_error_func(string condition, string stackTrace, LogType type)
                            {
                                if (type == LogType.Error || type == LogType.Assert)
                                {
                                    Debug.LogError($"获取属性 \"{info.Name}\" 值时错误！！！");
                                }
                            }
                            Application.logMessageReceived += _gather_error_func;

                            value = info.GetValue(owner);

                            Application.logMessageReceived -= _gather_error_func;
                        }
                    }
                    r.isIndexer = ips.Length != 0;
                }
                catch (Exception e)
                {
                    Debug.LogError($"获取属性 \"{info.Name}\" 值时异常：{e}");
                }
                Type type = value?.GetType() ?? info.PropertyType;
                r.owner = owner;
                r.info = info;
                r.name = info.Name;
                r.readOnly = readOnly;
                r.showGUI = !readOnly || (showReadonlyProperty && readOnly);
                r.value = value;
                r.type = type;
                r.declareType = info.PropertyType;
                r.showLabel = showLabel;
                r.setter = (newValue) =>
                {
                    if (info.CanWrite)
                    {
                        setValueStartEvent?.Invoke(value, newValue, info);
                        info.SetValue(owner, newValue);
                        setValueStartEvent?.Invoke(value, newValue, info);
                        return true;
                    }
                    return false;
                };
                r.label = GetLabel(r, IsGenericsType(value, type));
                return r;
            }

            /// <summary>
            /// 获取 <see cref="target"/> 目标的字段、属性成员
            /// </summary>
            protected virtual void GetMemberInfo()
            {
                _fieldInfos.value.Clear();
                _propertyInfos.value.Clear();
                _typeFieldInfos.Clear();
                _typePropertyInfos.Clear();

                if (type != null)
                {
                    FieldInfo[] fields = type.GetFields(
                        BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
                    var properties = type.GetProperties(
                        BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
                    _fieldInfos.value.AddRange(fields);
                    _propertyInfos.value.AddRange(properties);

                    // 将 字段 按照声明类型分类
                    for (int i = 0; i < fields.Length; i++)
                    {
                        var v = fields[i];
                        if (!_typeFieldInfos.TryGetValue(v.DeclaringType, out var ts))
                        {
                            _typeFieldInfos[v.DeclaringType] = ts = new ControlList<FieldInfo>();
                        }
                        ts.value.Add(v);
                    }
                    // 将 属性 按照声明类型分类
                    for (int i = 0; i < properties.Length; i++)
                    {
                        var v = properties[i];
                        if (!_typePropertyInfos.TryGetValue(v.DeclaringType, out var ts))
                        {
                            _typePropertyInfos[v.DeclaringType] = ts = new ControlList<PropertyInfo>();
                        }
                        ts.value.Add(v);
                    }
                }
            }

            public virtual void Clear()
            {
                _objInstance = null;
                _info = null;
                target = null;
                _fieldInfos.value.Clear();
                _propertyInfos.value.Clear();
                _typeFieldInfos.Clear();
                _typePropertyInfos.Clear();

                parent = null;
                _childs.Clear();
                _rlist = null;
                _rLists.Clear();

                _setValueStartEvent = null;
                _setValueEndEvent = null;
                _createChildEvent = null;

                _fieldStartGUIEvent = null;
                _fieldEndGUIEvent = null;
                _propertyStartGUIEvent = null;
                _propertyEndGUIEvent = null;
                _memberStartGUIEvent = null;
                _memberEndGUIEvent = null;
            }


            protected delegate void ActionRef<T>(ref T t);
            protected delegate void ActionOut<T>(out T t);
            protected delegate void ActionRef<T1, T2>(ref T1 t1, ref T2 t2);
            protected delegate void ActionOut<T1, T2>(out T1 t1, out T2 t2);
        }
    }
#endif


    public class ComponentViewer : MonoBehaviour
    {
        [SerializeField]
        private Component _target;
        [NonSerialized]
        public int _num;

        [HideInInspector]
        public bool cTargetSettingsFoldout = true;
        [HideInInspector]
        public bool showNonsupportMember = true;
        [HideInInspector]
        public bool showInheritRelation = true;
        [HideInInspector]
        public int maxDepth = 10, minTextLine = 1, maxTextLine = 5;

        public Component target { get => _target; set => _target = value; }

        //public T GetFieldValue<T>(string name)
        //{
        //    return default;
        //}
    }
}