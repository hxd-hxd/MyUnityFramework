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
    using System.Linq;

#if UNITY_EDITOR
    using System.Reflection;
    using UnityEditor;
    using Object = UnityEngine.Object;

    [CustomEditor(typeof(ComponentViewer))]
    public class ComponentViewerInspector : Editor
    {
        protected Object script;// 脚本资产
        protected SerializedProperty cTarget;
        protected List<FieldInfo> fieldInfos = new List<FieldInfo>(2);
        protected List<PropertyInfo> propertyInfos = new List<PropertyInfo>(2);
        protected Dictionary<Type, ControlList<FieldInfo>> typeFieldInfos = new Dictionary<Type, ControlList<FieldInfo>>(3);
        protected Dictionary<Type, ControlList<PropertyInfo>> typePropertyInfos = new Dictionary<Type, ControlList<PropertyInfo>>(3);

        protected static bool showReadonlyProperty = true;// 显示只读属性
        protected static bool fieldFoldout = true, propertyFoldout = true;

        ComponentViewer my => (ComponentViewer)target;

        protected virtual void OnEnable()
        {
            script = MonoScript.FromMonoBehaviour(my);

            cTarget = serializedObject.FindProperty("_target");

            Init();
        }

        public override void OnInspectorGUI()
        {
            //base.OnInspectorGUI();
            if (script)
            {
                EditorGUI.BeginDisabledGroup(true);
                EditorGUILayout.ObjectField("Script", script, typeof(MonoScript), true);
                EditorGUI.EndDisabledGroup();
            }

            OnTitleGUI();

            OnCTargetGUI();

            OnHelpGUI();

            // 字段、属性
            OnCTargetMemberGUI();
        }

        protected virtual void Init()
        {
            fieldInfos.Clear();
            propertyInfos.Clear();
            typeFieldInfos.Clear();
            typePropertyInfos.Clear();

            if (my.target != null)
            {
                Type type = my.target.GetType();
                FieldInfo[] fields = type.GetFields(
                    BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
                var properties = type.GetProperties(
                    BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
                fieldInfos.AddRange(fields);
                propertyInfos.AddRange(properties);

                // 将 字段 按照声明类型分类
                for (int i = 0; i < fields.Length; i++)
                {
                    var v = fields[i];
                    if (!typeFieldInfos.TryGetValue(v.DeclaringType, out var ts))
                    {
                        typeFieldInfos[v.DeclaringType] = ts = new ControlList<FieldInfo>();
                    }
                    ts.value.Add(v);
                }
                // 将 属性 按照声明类型分类
                for (int i = 0; i < properties.Length; i++)
                {
                    var v = properties[i];
                    if (!typePropertyInfos.TryGetValue(v.DeclaringType, out var ts))
                    {
                        typePropertyInfos[v.DeclaringType] = ts = new ControlList<PropertyInfo>();
                    }
                    ts.value.Add(v);
                }
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
            EditorGUILayout.HelpBox("将暴露所有 “非公开的” 字段和属性，请谨慎修改！", MessageType.Warning);
        }
        // 目标 gui
        protected virtual void OnCTargetGUI()
        {
            EditorGUI.BeginChangeCheck();
            Object oldTarget = cTarget.objectReferenceValue;
            EditorGUILayout.PropertyField(cTarget);
            bool change_cTarget = EditorGUI.EndChangeCheck();

            // 切换目标
            if (change_cTarget)
            {
                // 先保存
                serializedObject.ApplyModifiedProperties();
                // 再初始化
                Init();
                //Debug.Log($"更改目标为：{(cTarget.objectReferenceValue as Component)?.gameObject.name}<{cTarget.objectReferenceValue?.GetType().Name}>。原目标：{(oldTarget as Component)?.gameObject.name}<{oldTarget?.GetType().Name}>");
            }

            // 目标脚本文件
            OnCTargetScriptGUI();
        }
        // 目标脚本文件
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
            /* 检查已有字段、属性的所属实例与现有目标是否相同
                用于解决类似以下问题：
                ArgumentException: Field _target defined on type Framework.ComponentViewer is not a field on the target object which is of type UnityEngine.Transform.
            */
            MemberInfo info = (MemberInfo)fieldInfos.FirstOrDefault() ?? propertyInfos.FirstOrDefault();
            bool targetSame = info == null ? true : info.ReflectedType == GetType(my.target);
            if (!targetSame)
            {
                Init();// 不同时初始刷新
            }

            // 字段
            //EditorGUILayout.LabelField("字段", EditorStyles.boldLabel);
            //fieldFoldout = EditorGUILayout.Foldout(fieldFoldout, "字段");
            OnMemberGUI(ref fieldFoldout, "字段", typeFieldInfos);

            // 属性
            OnMemberGUI(ref propertyFoldout, "属性", typePropertyInfos,
                () =>
            {
                showReadonlyProperty = EditorGUILayout.ToggleLeft("显示只读属性（即只有 Get 访问器）", showReadonlyProperty);
                //EditorGUILayout.Space(4);
            });
        }
        protected virtual void OnMemberGUI<T>(ref bool foldout, string label, Dictionary<Type, ControlList<T>> typeInfos, Action start = null, Action end = null) where T : MemberInfo
        {
            foldout = EditorGUILayout.BeginFoldoutHeaderGroup(foldout, label);
            if (foldout)
            {
                if (typeInfos.Count > 0)
                {
                    start?.Invoke();

                    EditorGUILayout.BeginVertical("box");
                    {
                        int typeIndex = 0;
                        //EditorGUI.indentLevel++;
                        foreach (var infos in typeInfos)
                        {
                            var ts = new GUIStyle(EditorStyles.foldout);
                            ts.fontStyle = FontStyle.BoldAndItalic;
                            //ts.fontSize = (int)(ts.fontSize * 0.95f);
                            infos.Value.foldout = EditorGUILayout.Foldout(infos.Value.foldout, infos.Key.FullName, ts);
                            if (infos.Value.foldout)
                            {
                                EditorGUILayout.BeginVertical("box");
                                //EditorGUI.indentLevel++;
                                foreach (var info in infos.Value.value)
                                {
                                    TypeGUI(info);
                                }
                                //EditorGUI.indentLevel--;
                                EditorGUILayout.EndVertical();
                            }

                            ++typeIndex;

                            if (typeIndex < typeInfos.Count)
                            {
                                var s = new GUIStyle(EditorStyles.miniLabel);
                                s.normal.textColor = Color.yellow;
                                EditorGUILayout.LabelField("继承 ↓", s);
                            }
                        }
                        //EditorGUI.indentLevel--;

                    }
                    EditorGUILayout.EndVertical();

                    end?.Invoke();
                }
                else
                {
                    EditorGUILayout.LabelField($"没有 {label}");
                }
            }
            EditorGUILayout.EndFoldoutHeaderGroup();
        }

        // 成员
        protected virtual void TypeGUI<T>(T info) where T : MemberInfo
        {
            if (info is FieldInfo fieldInfo)
            {
                TypeGUI(fieldInfo);
            }
            else if (info is PropertyInfo propertyInfo)
            {
                TypeGUI(propertyInfo);
            }
        }
        // 字段
        protected virtual void TypeGUI(FieldInfo info)
        {
            //Debug.Log($"字段：{info.Name}");
            if (info.GetCustomAttributes(typeof(ObsoleteAttribute), true).Length > 0) return;
            TypeGUI(
                info.Name
                , false
                , true
                , info.GetValue(my.target)
                , info.FieldType
                , (value) =>
                {
                    Undo.RecordObject(my.target, $"修改组件查看器目标字段：{info.Name}");
                    EditorUtility.SetDirty(my.target);
                    info.SetValue(my.target, value);
                }
                );
        }
        // 属性
        protected virtual void TypeGUI(PropertyInfo info)
        {
            //Debug.Log($"属性：{info.Name}");
            if (info.GetCustomAttributes(typeof(ObsoleteAttribute), true).Length > 0) return;
            bool reafOnly = IsReadOnly(info);
            TypeGUI(
                info.Name
                , reafOnly
                , !reafOnly || (showReadonlyProperty && reafOnly)
                , info.GetValue(my.target)
                , info.PropertyType
                , (value) =>
                {
                    Undo.RecordObject(my.target, $"修改组件查看器目标属性：{info.Name}");
                    EditorUtility.SetDirty(my.target);
                    info.SetValue(my.target, value);
                }
                );
        }
        // 类型信息
        protected virtual void TypeGUI(string name, bool readOnly, bool showGUI, object v, Type type, Action<object> setter)
        {
            if (!showGUI) return;

            string dispalyName = $"{name}（{type.Name}）";
            object newV = default;

            //if(name == "_target")
            //{
            //    Debug.Log("");
            //}
            EditorGUI.BeginChangeCheck();
            EditorGUI.BeginDisabledGroup(readOnly);
            if (TypeCast(v, out int vInt))
            {
                newV = EditorGUILayout.IntField(dispalyName, vInt);
            }
            else if (TypeCast(v, out uint vUint))
            {
                int n = EditorGUILayout.IntField(dispalyName, (int)vUint);
                newV = n < 0 ? 0u : (uint)n;
                //Debug.Log($"{dispalyName}：{newV}");
            }
            else if (TypeCast(v, out long vLong))
            {
                newV = EditorGUILayout.LongField(dispalyName, vLong);
            }
            else if (TypeCast(v, out ulong vUlong))
            {
                long n = EditorGUILayout.LongField(dispalyName, (long)vUlong);
                newV = n < 0 ? 0ul : (ulong)n;
            }
            else if (TypeCast(v, out float vFloat))
            {
                newV = EditorGUILayout.FloatField(dispalyName, vFloat);
            }
            else if (TypeCast(v, out double vDouble))
            {
                newV = EditorGUILayout.DoubleField(dispalyName, vDouble);
            }
            else if (TypeCast(v, out bool vBool))
            {
                newV = EditorGUILayout.Toggle(dispalyName, vBool);
            }
            else if (TypeCast(v, out string vString))
            {
                newV = EditorGUILayout.TextField(dispalyName, vString);
            }
            else if (TypeCast(v, out char vChar))
            {
                string n = EditorGUILayout.TextField(dispalyName, new String(vChar, 1));
                newV = n == null || n == "" ? default : n[0];
            }

            else if (TypeCast(v, out Vector2 vVector2))
            {
                newV = EditorGUILayout.Vector2Field(dispalyName, vVector2);
            }
            else if (TypeCast(v, out Vector2Int vVector2Int))
            {
                newV = EditorGUILayout.Vector2IntField(dispalyName, vVector2Int);
            }
            else if (TypeCast(v, out Vector3 vVector3))
            {
                newV = EditorGUILayout.Vector3Field(dispalyName, vVector3);
            }
            else if (TypeCast(v, out Vector3Int vVector3Int))
            {
                newV = EditorGUILayout.Vector3IntField(dispalyName, vVector3Int);
            }
            else if (TypeCast(v, out Vector4 vVector4))
            {
                newV = EditorGUILayout.Vector4Field(dispalyName, vVector4);
            }
            else if (TypeCast(v, out Color vColor))
            {
                newV = EditorGUILayout.ColorField(dispalyName, vColor);
            }
            else if (TypeCast(v, out Color32 vColor32))
            {
                newV = (Color32)EditorGUILayout.ColorField(dispalyName, vColor32);
            }
            else if (TypeCast(v, out Rect vRect))
            {
                newV = EditorGUILayout.RectField(dispalyName, vRect);
            }
            else if (TypeCast(v, out RectInt vRectInt))
            {
                newV = EditorGUILayout.RectIntField(dispalyName, vRectInt);
            }
            else if (TypeCast(v, out Bounds vBounds))
            {
                newV = EditorGUILayout.BoundsField(dispalyName, vBounds);
            }
            else if (TypeCast(v, out BoundsInt vBoundsInt))
            {
                newV = EditorGUILayout.BoundsIntField(dispalyName, vBoundsInt);
            }

            else if (TypeCast(v, out Enum vEnum, true))// 允许继承转换
            {
                // 有枚举标志位特性
                if (type.GetCustomAttributes(typeof(FlagsAttribute), false).Length > 0)
                {
                    newV = EditorGUILayout.EnumFlagsField(dispalyName, vEnum);
                }
                else
                    newV = EditorGUILayout.EnumPopup(dispalyName, vEnum);
            }

            //else if (TypeCast(v, out AnimationCurve vAnimationCurve))
            //{
            //    newV = EditorGUILayout.CurveField(dispalyName, vAnimationCurve);
            //}
            else if (type == typeof(AnimationCurve))
            {
                newV = EditorGUILayout.CurveField(dispalyName, v != null ? v as AnimationCurve : new AnimationCurve());
            }
            //else if (TypeCast(v, out Gradient vGradient))
            //{
            //    newV = EditorGUILayout.GradientField(dispalyName, vGradient);
            //}
            //else if (TypeCast(v, out Gradient vGradient, type))
            //{
            //    newV = EditorGUILayout.GradientField(dispalyName, vGradient);
            //}
            else if (type == typeof(Gradient))
            {
                newV = EditorGUILayout.GradientField(dispalyName, v != null ? v as Gradient : new Gradient());
            }

            //else if (TypeCast(v as Object, out Object vUnityObject, true))// 允许继承转换
            //{
            //    newV = EditorGUILayout.ObjectField(dispalyName, vUnityObject, type, false);
            //}
            else if (type.IsSubclassOf(typeof(Object)))
            {
                if (TypeCast(v as Object, out Object vUnityObject, true))// 允许继承转换
                {
                    newV = EditorGUILayout.ObjectField(dispalyName, vUnityObject, type, false);
                }
            }
            else
            {
                // untiy 不直接提供 gui 的类型
                //Quaternion
                EditorGUILayout.SelectableLabel($"{dispalyName}，不支持该类型。");
            }
            EditorGUI.EndDisabledGroup();
            if (EditorGUI.EndChangeCheck())
            {
                setter(newV);
            }
        }

        /// <summary>
        /// 类型转换
        /// </summary>
        /// <typeparam name="TIn"></typeparam>
        /// <typeparam name="TOut"></typeparam>
        /// <param name="obj"></param>
        /// <param name="v"></param>
        /// <param name="inherit">true：有继承关系</param>
        /// <returns></returns>
        protected bool TypeCast<TIn, TOut>(TIn obj, out TOut v, bool inherit = false)
        {
            object vObj = obj;
            v = default;
            Type inType = GetType(obj);
            Type outType = GetType(v);
            bool isType = inType == outType;
            if (inherit) isType = isType || obj is TOut;// 是否有继承关系
            if (isType)
            {
                v = (TOut)vObj;
            }
            return isType;
        }
        protected bool TypeCast<TIn, TOut>(TIn obj, out TOut v, Type outType, bool inherit = false)
        {
            object vObj = obj;
            v = default;
            Type inType = GetType(obj);
            bool isType = inType == outType;
            if (inherit) isType = isType || outType.IsSubclassOf(inType) || obj is TOut;// 是否有继承关系
            if (isType)
            {
                v = (TOut)vObj;
            }
            return isType;
        }
        /// <summary>
        /// 获取类型，如果是可 null 类型且值为 null，则获取非实例类型
        /// </summary>
        /// <typeparam name="TIn"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        protected Type GetType<TIn>(TIn obj)
        {
            // 如果 T 是引用类型，且值是 null，则无法调用 .GetType()，此时要通过 typeof 来获取 Type
            Type inType = obj?.GetType() ?? typeof(TIn);
            return inType;
        }
        protected bool IsReadOnly(PropertyInfo info) => !info.CanWrite && info.CanRead;

        protected class Control<T>
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
        protected class ControlList<T> : Control<List<T>>
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