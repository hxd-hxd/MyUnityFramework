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
    using System.Runtime.InteropServices;
    using System.Text;
    using UnityEditor;
    using Object = UnityEngine.Object;

    [CustomEditor(typeof(ComponentViewer))]
    public class ComponentViewerInspector : Editor
    {
        protected Object script;// 脚本资产
        protected SerializedProperty cTarget;

        protected GenericsTypeGUI cTargetGUI;
        protected GenericsTypeGUI _numGUI;

        protected static bool showNonsupportMember = true;

        ComponentViewer my => (ComponentViewer)target;

        // 此函数在脚本启动时调用
        protected virtual void Awake()
        {
            Debug.Log($"{nameof(ComponentViewerInspector)}.Awake");

            script = MonoScript.FromMonoBehaviour(my);

            cTarget = serializedObject.FindProperty("_target");

            cTargetGUI = new GenericsTypeGUI(my.target, my, my.GetType().GetField("_target"
                , BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic));
            cTargetGUI.foldout = true;
            //cTargetGUI.showInheritRelation = false;
            cTargetGUI.setValueStartEvent = (_, v, _info) =>
            {
                Undo.RecordObject(my.target, $"修改组件查看器目标 {_info.MemberType} {_info.Name}");
                EditorUtility.SetDirty(my.target);
            };

            _numGUI = new GenericsTypeGUI(my._num, my, my.GetType().GetField("_num", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic));
            _numGUI.foldout = true;
            _numGUI.setValueStartEvent = (_, v, _info) =>
            {
                Undo.RecordObject(target, $"修改组件查看器 {_info.MemberType} {_info.Name}");
                EditorUtility.SetDirty(target);
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
            EditorGUILayout.HelpBox("将暴露所有 “非公开的” 字段和属性，请谨慎修改！\r\n部分属性及字段修改将不能被 Unity 序列化！且不支持撤回！\r\n有关 Unity 序列化规则可自行查阅资料，比较简单的的判断方式是：能在原组件的 Inspector 中显示的都是可序列化的（本组件除外）。", MessageType.Warning);
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
            showNonsupportMember = EditorGUILayout.Toggle("显示不支持类型的成员", showNonsupportMember);
            cTargetGUI.showNonsupportMember = showNonsupportMember;

            CheckCTargetChange();

            cTargetGUI.OnGUI(true, false);
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

        /// <summary>
        /// 获取类型，如果是可 <c>null</c> 类型且值为 <c>null</c>，则获取非实例类型
        /// </summary>
        /// <typeparam name="TIn"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
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

        public struct TypeGUIArgs
        {
            public object target;   // 成员所属者

            public MemberInfo info; // 成员的信息
            public string name;
            public GUIContent label;
            public bool showLabel;
            public bool readOnly;
            public bool showGUI;
            public object value;// 成员的值
            public Type type;   // 成员的类型
            public Func<object, bool> setter;// gui 修改后的新值回调

            public TypeGUIArgs(object target, MemberInfo info, string name, bool readOnly, bool showGUI, object value, Type type, Func<object, bool> setter)
            {
                this.target = target;
                this.info = info;
                this.name = name;
                this.readOnly = readOnly;
                this.showGUI = showGUI;
                this.value = value;
                this.type = type;
                this.setter = setter;

                showLabel = true;
                label = GUIContent.none;
            }
            public TypeGUIArgs(object target, MemberInfo info, string name, GUIContent label, bool showLabel, bool readOnly, bool showGUI, object value, Type type, Func<object, bool> setter)
            {
                this.target = target;
                this.info = info;
                this.name = name;
                this.readOnly = readOnly;
                this.showGUI = showGUI;
                this.value = value;
                this.type = type;
                this.setter = setter;

                this.showLabel = showLabel;
                this.label = label;
            }

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

            // 折页
            public bool foldout = true;

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
        public class GenericsTypeGUI
        {
            protected object _objInstance;// 成员所属的对象实例

            protected object _target;
            protected MemberInfo _info;// 目标所属的成员

            protected ControlList<FieldInfo> _fieldInfos = new ControlList<FieldInfo>();
            protected ControlList<PropertyInfo> _propertyInfos = new ControlList<PropertyInfo>();
            protected Dictionary<Type, ControlList<FieldInfo>> _typeFieldInfos = new Dictionary<Type, ControlList<FieldInfo>>(3);
            protected Dictionary<Type, ControlList<PropertyInfo>> _typePropertyInfos = new Dictionary<Type, ControlList<PropertyInfo>>(3);

            protected bool _showNonsupportMember = true;
            protected bool _showReadonlyProperty = true;
            protected bool _showInheritRelation = true;
            protected bool _foldout = false, _fieldFoldout = true, _propertyFoldout = true;

            private int _maxDepth = 10;
            protected bool _allowSelfNested = false;// 允许自我嵌套的类型

            protected GenericsTypeGUI _parent;
            protected List<GenericsTypeGUI> _childs = new List<GenericsTypeGUI>(1);

            protected Action<object, object, MemberInfo> _setValueStartEvent;
            protected Action<object, object, MemberInfo> _setValueEndEvent;
            protected Func<GenericsTypeGUI> _createChildGUIEvent;

            /// <summary>
            /// 开始为目标字段或属性设置值事件，包括为 <see cref="target"/> 赋值
            /// <para></para>arg1：旧值，arg2：新值，arg3：设置值的目标字段或属性
            /// <para></para>属性：如果不可读，则旧值一直为 null，只有在可写时，该事件才会被调用
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
            /// <para></para>如果是属性：如果不可读，旧值一直为 null，只有在可写时，该事件才会被调用
            /// </summary>
            /// <remarks>此属性始终是 <see cref="root"/> 的</remarks>
            public Action<object, object, MemberInfo> setValueEndEvent
            {
                get => root._setValueEndEvent;
                set => root._setValueEndEvent = value;
            }
            /// <summary>创建子 gui 对象事件</summary>
            /// <remarks>此属性始终是 <see cref="root"/> 的</remarks>
            public Func<GenericsTypeGUI> createChildGUIEvent
            {
                get => root._createChildGUIEvent;
                set => root._createChildGUIEvent = value;
            }

            public bool foldout { get => _foldout; set => _foldout = value; }

            /// <summary>显示不支持的成员</summary>
            /// <remarks>此属性始终是 <see cref="root"/> 的</remarks>
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
            /// <summary>显示只读属性</summary>
            /// <remarks>此属性始终是 <see cref="root"/> 的</remarks>
            public virtual bool showReadonlyProperty
            {
                get => root._showReadonlyProperty;
                set
                {
                    root._showReadonlyProperty = value;
                }
            }
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
            /// <summary>允许自我嵌套的类型
            /// <para>自嵌：1、类中有自身类型作为成员；2、即两个类有继承关系，且父类中有子类作为其成员</para>
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

            /// <summary>目标成员所属的对象实例</summary>
            /// <remarks>如果 <see cref="parent"/> 不为空的话，始终表示为 <see cref="parent"/> 的 <see cref="target"/></remarks>
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

            public GenericsTypeGUI parent
            {
                get => _parent;
                protected set
                {
                    if (_parent == value || this == value) return;
                    var cp = _parent;
                    _parent = value;
                    cp?._childs.Remove(this);
                    value?._childs.Add(this);
                }
            }
            /// <summary>
            /// 根对象
            /// </summary>
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
            /// <summary>
            /// 允许的对象创建 gui 的最大深度，防止循环依赖
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
                        root._maxDepth = value;
                }
            }
            /// <summary>
            /// 当前对象的深度
            /// </summary>
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
            /// <summary>
            /// 总深度
            /// </summary>
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

            /// <summary>
            /// 要创建 gui 的目标对象
            /// </summary>
            public object target
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
            /// <summary>
            /// 目标 <see cref="target"/> 的类型
            /// </summary>
            public Type type => _target?.GetType();
            public ControlList<FieldInfo> fieldInfos { get => _fieldInfos; }
            public ControlList<PropertyInfo> propertyInfos { get => _propertyInfos; }
            public Dictionary<Type, ControlList<FieldInfo>> typeFieldInfos { get => _typeFieldInfos; }
            public Dictionary<Type, ControlList<PropertyInfo>> typePropertyInfos { get => _typePropertyInfos; }

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
            /// <para>如果是可空类型，请使用 <see cref="TypeCast{TIn, TOut}(TIn, out TOut, Type, bool)"/></para>
            /// </summary>
            /// <typeparam name="TIn"></typeparam>
            /// <typeparam name="TOut"></typeparam>
            /// <param name="obj"></param>
            /// <param name="v"></param>
            /// <param name="inherit">true：有继承关系</param>
            /// <returns>是否转换成功</returns>
            public static bool TypeCast<TIn, TOut>(TIn obj, out TOut v, bool inherit = false)
            {
                v = default;
                return TypeCast(obj, out v, GetType(obj), inherit);
            }
            /// <summary>
            /// 类型转换
            /// </summary>
            /// <typeparam name="TIn">要转换的对象类型</typeparam>
            /// <typeparam name="TOut">转换后的对象类型</typeparam>
            /// <param name="obj">要转换的值</param>
            /// <param name="v">转换后的值</param>
            /// <param name="inType">指定输入的类型</param>
            /// <param name="inherit">true：有继承关系</param>
            /// <returns>是否转换成功</returns>
            public static bool TypeCast<TIn, TOut>(TIn obj, out TOut v, Type inType, bool inherit = false)
            {
                object vObj = obj;
                v = default;
                Type outType = GetType(v);
                bool isType = inType == outType;
                if (inherit) isType = isType || inType.IsSubclassOf(outType) || obj is TOut;// 是否有继承关系
                if (isType && vObj != null)
                {
                    v = (TOut)vObj;
                }
                return isType;
            }
            /// <summary>
            /// 获取类型，如果是可 <c>null</c> 类型且值为 <c>null</c>，则获取非实例类型
            /// </summary>
            /// <typeparam name="TIn"></typeparam>
            /// <param name="obj"></param>
            /// <returns></returns>
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

            /// <summary>
            /// 是否相同或存在直系关系
            /// </summary>
            /// <param name="t1"></param>
            /// <param name="t2"></param>
            /// <returns></returns>
            public static bool IsLineal(Type t1, Type t2)
            {
                if (t1 == null || t2 == null) return false;
                return t1 == t2 || t1.IsSubclassOf(t2) || t2.IsSubclassOf(t1);
            }

            public static bool IsReadOnly(PropertyInfo info) => !info.CanWrite && info.CanRead;
            public static bool IsWriteOnly(PropertyInfo info) => info.CanWrite && !info.CanRead;
            // 是否标记为过时
            public static bool IsObsoleteMember(MemberInfo info) => IsHasAttribute<ObsoleteAttribute>(info);
            public static bool IsHasAttribute<T>(MemberInfo info) where T : Attribute => info.GetCustomAttributes(typeof(T), true).Length > 0;

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

            #endregion

            public virtual void Init()
            {
                GetMemberInfo();
            }

            public virtual void OnGUI(bool bringLabel = true) => OnGUI(IsGenericsType(), bringLabel);
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

            // 带标签
            protected virtual void OnTargetLabelMemberGUI()
            {
                _foldout = EditorGUILayout.Foldout(_foldout, GetLabel(_info?.Name, type, _info), true);
                if (_foldout)
                {
                    EditorGUI.indentLevel += 1;
                    OnMemberGUI();
                    EditorGUI.indentLevel -= 1;
                }
            }
            // 目标组件的成员（字段、属性） gui
            protected virtual void OnMemberGUI()
            {
                CheckTargetChange();

                // 字段
                if (typeFieldInfos.Count > 0)
                    OnMemberGUI(ref _fieldFoldout, "字段", typeFieldInfos
                    , () =>
                    {
                        if (IsRoot)
                            OnFieldStartGUI();
                    }
                    , () =>
                    {
                        if (IsRoot)
                            OnFieldEndGUI();
                    });

                // 属性
                if (typePropertyInfos.Count > 0)
                    OnMemberGUI(ref _propertyFoldout, "属性", typePropertyInfos
                        , () =>
                        {
                            if (IsRoot)
                                OnPropertyStartGUI();
                        }
                        , () =>
                        {
                            if (IsRoot)
                                OnPropertyEndGUI();
                        });
            }
            // 成员区分字段、属性等阶段
            protected virtual void OnMemberGUI<T>(ref bool foldout, string label, Dictionary<Type, ControlList<T>> typeInfos, Action start = null, Action end = null) where T : MemberInfo
            {
                {
                    var ts = new GUIStyle(EditorStyles.foldout);
                    ts.fontStyle = FontStyle.Bold;
                    foldout = EditorGUILayout.Foldout(foldout, label, true, ts);
                }
                //foldout = EditorGUILayout.BeginFoldoutHeaderGroup(foldout, label);
                if (foldout)
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

            // 只能在 root 里显示
            // 在 字段 gui 开始时的 gui
            protected virtual void OnFieldStartGUI()
            {

            }
            // 在 字段 gui 结束时的 gui
            protected virtual void OnFieldEndGUI()
            {

            }
            // 在 属性 gui 开始时的 gui
            protected virtual void OnPropertyStartGUI()
            {
                showReadonlyProperty = EditorGUILayout.ToggleLeft("显示只读属性（即只有 Get 访问器）", showReadonlyProperty);
                //EditorGUILayout.Space(4);
            }
            // 在 属性 gui 结束时的 gui
            protected virtual void OnPropertyEndGUI()
            {

            }

            // 成员
            protected virtual void TypeGUI<TInfo>(TInfo info) where TInfo : MemberInfo
            {
                TypeGUI(info, target, true);
            }
            protected virtual void TypeGUI<TInfo>(TInfo info, object target, bool showLabel) where TInfo : MemberInfo
            {
                if (info is FieldInfo fieldInfo)
                {
                    TypeGUI(fieldInfo, target, showLabel);
                }
                else if (info is PropertyInfo propertyInfo)
                {
                    TypeGUI(propertyInfo, target, showLabel);
                }
                else
                {
                    Debug.LogWarning($"暂不支持成员类型：{info}");
                }
            }
            // 字段
            protected virtual void TypeGUI(FieldInfo info) => TypeGUI(info, target, true);
            protected virtual void TypeGUI(FieldInfo info, object target, bool showLabel)
            {
                //Debug.Log($"字段：{info.Name}");
                if (IsObsoleteMember(info)) return;
                TypeGUI(GetTypeGUIArgs(info, target, showLabel));
            }
            // 属性
            protected virtual void TypeGUI(PropertyInfo info) => TypeGUI(info, target, true);
            protected virtual void TypeGUI(PropertyInfo info, object target, bool showLabel)
            {
                //Debug.Log($"属性：{info.Name}");
                if (IsObsoleteMember(info)) return;
                TypeGUI(GetTypeGUIArgs(info, target, showLabel));
            }
            // 类型信息
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
            /// <summary>
            /// 根据对象类型自动提供适合其的 gui
            /// </summary>
            /// <returns>gui 是否发生变化</returns>
            /// <remarks>如果重写了此方法，扩展了类型 gui，必须也重写 <see cref="IsGenericsType(object, Type)"/> ，添加对应类型</remarks>
            protected virtual bool TypeGUI(GUIContent label, object v, Type type, out object newV, MemberInfo info = null)
            {
                newV = default;
                bool isGenericsType = false;
                bool isChange = false;

                EditorGUI.BeginChangeCheck();
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
                    newV = EditorGUILayout.TextField(label, vString);
                }
                else if (TypeCast(v, out char vChar, type))// 不直接支持
                {
                    string n = EditorGUILayout.TextField(label, new String(vChar, 1));
                    newV = n == null || n == "" ? default : n[0];
                }

                else if (TypeCast(v, out Hash128 vHash128, type))// 不直接支持
                {
                    string n = EditorGUILayout.TextField(label, vHash128.ToString());
                    newV = Hash128.Parse(n);
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

                else if (TypeCast(v, out Enum vEnum, type, true))// 允许继承转换
                {
                    // 有枚举标志位特性
                    if (type.GetCustomAttributes(typeof(FlagsAttribute), false).Length > 0)
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
                    isGenericsType = true;
                }

                isChange = EditorGUI.EndChangeCheck();

                if (isGenericsType)
                {
                    // untiy 不直接提供 gui 的类型
                    //ReorderableList
                    newV = GenericsField(label, v, type, info);

                    //OnshowNonsupportMemberGUI(label);

                }

                //return EditorGUI.EndChangeCheck();
                return isChange;
            }

            // 显示不支持类型的 gui
            protected void OnshowNonsupportMemberGUI(GUIContent label) => OnshowNonsupportMemberGUI(label, $"不支持该类型");
            protected void OnshowNonsupportMemberGUI(GUIContent label, string text)
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

            /// <summary><see cref="target"/> 是否自定义类型</summary>
            public bool IsGenericsType() => IsGenericsType(target);
            /// <summary>是否自定义类型</summary>
            public bool IsGenericsType(object v) => IsGenericsType(v, GetType(v));
            /// <summary>是否自定义类型</summary>
            /// <remarks>如果重写了 <see cref="TypeGUI(GUIContent, object, Type, out object, MemberInfo)"/>，扩展了类型 gui，必须也重写 <see cref="IsGenericsType(object, Type)"/> ，添加对应类型</remarks>
            public virtual bool IsGenericsType(object v, Type type)
            {
                bool r = TypeCast(v, out int _, type)
                    || TypeCast(v, out uint _, type)
                    || TypeCast(v, out long _, type)
                    || TypeCast(v, out ulong _, type)
                    || TypeCast(v, out float _, type)
                    || TypeCast(v, out double _, type)
                    || TypeCast(v, out bool _, type)
                    || TypeCast(v, out string _, type)
                    || TypeCast(v, out char _, type)

                    || TypeCast(v, out Hash128 _, type)
                    || TypeCast(v, out Vector2 _, type)
                    || TypeCast(v, out Vector2Int _, type)
                    || TypeCast(v, out Vector3 _, type)
                    || TypeCast(v, out Vector3Int _, type)
                    || TypeCast(v, out Quaternion _, type)
                    || TypeCast(v, out Color _, type)
                    || TypeCast(v, out Color32 _, type)
                    || TypeCast(v, out Rect _, type)
                    || TypeCast(v, out RectInt _, type)
                    || TypeCast(v, out Bounds _, type)
                    || TypeCast(v, out BoundsInt _, type)
                    || TypeCast(v, out RectInt _, type)

                    || TypeCast(v, out Enum _, type, true)
                    || TypeCast(v, out AnimationCurve _, type)
                    || TypeCast(v, out Gradient _, type)
                    || TypeCast(v, out Object _, type, true)
                    ;

                return !r;// 不能转换为上述类型，则以通用类型显示 gui
            }

            /// <summary>其他自定义类型字段</summary>
            public virtual object GenericsField(GUIContent label, object v, Type type, MemberInfo info)
            {
                GenericsTypeGUI gui = null;

                // 处理循环依赖
                // 当 type 继承 this.type 时必定循环
                bool lineal = IsLineal(type, this.type);// 是否有直接关系
                bool allowSelfLoop = lineal && allowSelfNested;
                bool depthStandards = currentDepth < maxDepth;
                if ((!lineal ||         // 没有直接关系
                    allowSelfLoop)      // 有直接关系，但允许自循环
                    && depthStandards   // 深度未达上限
                    )
                {
                    gui = _childs.Find(v => v._info == info);
                    if (gui == null)
                    {
                        gui = AddChild(v, info);
                    }

                    if (v == null)
                    {
                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.PrefixLabel(label);
                        // 如果值是空的，则让其创建一个
                        if (type.GetConstructor(BindingFlags.Instance | BindingFlags.Public, null, Type.EmptyTypes, null) != null)// 有不带参数的构造函数
                        {
                            if (GUILayout.Button("创建实例"))
                            {
                                gui.target = Activator.CreateInstance(type);
                                //SyncSelfTargetValue();
                                if (parent != null && objInstance != null)
                                {
                                    SetValue(objInstance, _info, target);
                                }
                            }
                        }
                        else
                        {
                            EditorGUILayout.LabelField("该类型不支持直接创建实例。");
                        }
                        EditorGUILayout.EndHorizontal();
                    }
                    else
                    {
                        gui.target = v;
                        gui.OnGUI(true);
                    }
                }
                else
                {
                    if (!depthStandards)
                        OnshowNonsupportMemberGUI(label, $"已达深度上限 {maxDepth}");
                    else if (!allowSelfLoop)
                        OnshowNonsupportMemberGUI(label, $"不支持自循环类型");
                    else
                        OnshowNonsupportMemberGUI(label);
                }

                return gui?.target;
            }
            /// <summary>创建 <see cref="GenericsTypeGUI"/></summary>
            protected virtual GenericsTypeGUI AddChild(object target, MemberInfo info)
            {
                var gui = CreateGenericsTypeGUI(target, info);
                gui.parent = this;
                _childs.Add(gui);
                return gui;
            }
            /// <summary>创建 <see cref="GenericsTypeGUI"/></summary>
            protected virtual GenericsTypeGUI CreateGenericsTypeGUI(object target, MemberInfo info)
            {
                var gui = createChildGUIEvent?.Invoke();
                gui ??= new GenericsTypeGUI();
                gui.target = target;
                gui._info = info;
                gui.Init();
                gui.foldout = false;
                gui.showInheritRelation = false;
                return gui;
            }

            // 获取标签
            protected virtual GUIContent GetLabel(TypeGUIArgs args, bool showDepth = false)
            {
                return GetLabel(args.name, args.type, args.info, showDepth);
            }
            protected virtual GUIContent GetLabel(string name, Type type, MemberInfo info, bool showDepth = false)
            {
                string dispalyNameS = name != null && name.Length < 15 ? "\t" : null;
                string dispalyTypeName = type != null ? $"{dispalyNameS}（{type.Name}）" : null;
                string dispalyName = $"{name}{dispalyTypeName}";

                StringBuilder tooltip = new StringBuilder();
                tooltip.Append(type != null ? $"类型：{type.FullName}" : null);
                tooltip.AppendLine();
                tooltip.Append(info != null ? $@"成员访问修饰符：{GetMemberDeclareVisit(info)}" : null);
                //if (IsGenericsType(target))
                if (showDepth)
                {
                    tooltip.AppendLine();
                    tooltip.Append($"gui 深度：当前：{currentDepth}，总深度：{depth}");
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
                    t.SetValue(t.objInstance, t._info, t.target);
                    t = t.parent;
                }
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

            // 获取 TypeGUIArgs 实例，info 为 target 的成员
            public TypeGUIArgs GetTypeGUIArgs(FieldInfo info) => GetTypeGUIArgs(info, target);
            public TypeGUIArgs GetTypeGUIArgs(PropertyInfo info) => GetTypeGUIArgs(info, target);
            public TypeGUIArgs GetTypeGUIArgs(FieldInfo info, object target, bool showLabel = true)
            {
                object value = target != null ? info.GetValue(target) : null;
                Type type = info.FieldType;
                var r = new TypeGUIArgs(
                    target
                    , info
                    , info.Name
                    , false
                    , true
                    , value
                    , type
                    , (newValue) =>
                    {
                        setValueStartEvent?.Invoke(value, newValue, info);
                        info.SetValue(target, newValue);
                        setValueStartEvent?.Invoke(value, newValue, info);
                        return true;
                    }
                    );
                r.label = GetLabel(r, IsGenericsType(value, type));
                r.showLabel = showLabel;
                return r;
            }
            public TypeGUIArgs GetTypeGUIArgs(PropertyInfo info, object target, bool showLabel = true)
            {
                bool readOnly = IsReadOnly(info);
                object value = !IsWriteOnly(info) && target != null ? info.GetValue(target) : null;
                Type type = info.PropertyType;
                var r = new TypeGUIArgs(
                    target
                    , info
                    , info.Name
                    , readOnly
                    , !readOnly || (showReadonlyProperty && readOnly)
                    , value
                    , type
                    , (newValue) =>
                    {
                        if (info.CanWrite)
                        {
                            setValueStartEvent?.Invoke(value, newValue, info);
                            info.SetValue(target, newValue);
                            setValueStartEvent?.Invoke(value, newValue, info);
                            return true;
                        }
                        return false;
                    }
                    );
                r.label = GetLabel(r, IsGenericsType(value, type));
                r.showLabel = showLabel;
                return r;
            }

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
                target = null;
                _info = null;
                _fieldInfos.value.Clear();
                _propertyInfos.value.Clear();
                _typeFieldInfos.Clear();
                _typePropertyInfos.Clear();

                parent = null;
                _childs.Clear();

                setValueStartEvent = null;
                setValueEndEvent = null;
                _createChildGUIEvent = null;
            }
        }
    }
#endif


    public class ComponentViewer : MonoBehaviour
    {
        [SerializeField]
        private Component _target;
        public int _num;

        public Component target { get => _target; set => _target = value; }

        //public T GetFieldValue<T>(string name)
        //{
        //    return default;
        //}
    }
}