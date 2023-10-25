using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices.WindowsRuntime;
using Unity.VisualScripting;
using UnityEngine;

namespace EasyDI
{
    [DisallowMultipleComponent]
    public abstract class ContextBase : MonoBehaviour
    {
        Container container = new Container();
        [SerializeField] List<MonoInstaller> monoInstallerList = new List<MonoInstaller>();

        public List<MonoInstaller> MonoInstallerList { get => monoInstallerList; private set => monoInstallerList = value; }
        public Container Container { get => container; }
        ContextBase contextParent;

        protected abstract ContextBase GetParentContext();
        protected virtual void Awake()
        {
            container = new Container();
            contextParent = GetParentContext();
            GetAllBindInforFromInstallerList();
        }
        protected virtual void OnDestroy()
        {

        }
        /// <summary>
        /// combine bind conditional.
        /// </summary>
        private void GetAllBindInforFromInstallerList()
        {
            foreach (var monoInstaller in monoInstallerList)
            {
                monoInstaller.Init();
                foreach (var item in monoInstaller.Container.DictTypeAndData)
                {
                    container.AddTypeAndInfor(item.Key, item.Value);
                }

            }
        }

        public void InjectFor(object obj)
        {
            InjectFor(obj, container.DictTypeAndData);
        }

        public void InjectFor(object obj, Dictionary<Type, BindInfor> inforFromChildContext)
        {
            Dictionary<Type, BindInfor> newInforFromChildContextAndThis = new Dictionary<Type, BindInfor> { };
            _combineConditions(ref newInforFromChildContextAndThis, inforFromChildContext, container.DictTypeAndData);

            if (contextParent != null)
            {
                //b1: tong hop infor condition tu child va chinh no
                //b2: goi => contextParent.InjectFor(obj, inforThisAndChild);
                contextParent.InjectFor(obj, newInforFromChildContextAndThis);
            }
            else
            {
                //inject cho obj ngoai tru nhung infor child
                var l = getAllMemberNeedInject(obj.GetType());
                foreach (var memberInfor in l)
                {
                    //Debug.Log($"type member: {item.DeclaringType.FullName}");
                    //Debug.Log($"member has Inject:{item}");
                    _setData(obj, memberInfor);
                }
            }

            bool _tryGetConditionFromThisAndChild(Type type, out BindInfor bindInfor)
            {
                return newInforFromChildContextAndThis.TryGetValue(type, out bindInfor);


            }

            void _setData(object obj, MemberInfo member)
            {
                switch (member.MemberType)
                {
                    case MemberTypes.Event:
                        break;
                    case MemberTypes.Field:
                        _setForField(obj, member);
                        break;
                    case MemberTypes.Method:
                        _setForMethod(member);
                        break;
                    case MemberTypes.Property:
                        _setForProperties(obj, member);
                        break;
                    default:
                        throw new ArgumentException
                        (
                         "Input MemberInfo must be if type EventInfo, FieldInfo, MethodInfo, or PropertyInfo"
                        );
                }

                void _setForField(object obj, MemberInfo member)
                {
                    var filedType = (member as FieldInfo);
                    BindInfor infor = null;
                    if (_tryGetConditionFromThisAndChild(filedType.FieldType, out infor))
                    {
                        Debug.Log($"Tim thay member can inject: {member}");
                        filedType.SetValue(obj, infor.ObjectData);
                    }
                    else
                    {
                        EasyDILog.LogError($"Can't find binding {filedType.FieldType} for field: {filedType}!!");
                    }
                }

                void _setForMethod(MemberInfo member)
                {
                    var methodInfor = (member as MethodInfo);
                    var @params = methodInfor.GetParameters();
                    object[] args = new object[@params.Length];
                    for (int i = 0; i < @params.Length; i++)
                    {
                        BindInfor infor = null;
                        var item = @params[i].ParameterType;
                        if (_tryGetConditionFromThisAndChild(item, out infor))
                        {
                            Debug.Log($"Tim thay member can inject: {member}");
                            args[i] = infor.ObjectData;
                        }
                        else
                        {
                            EasyDILog.LogError($"Can't find binding {item} for Method: {methodInfor}!!");
                        }
                    }
                    methodInfor.Invoke(obj, args);
                }
                void _setForProperties(object obj, MemberInfo member)
                {
                    var proType = ((PropertyInfo)member);
                    BindInfor infor = null;
                    if (_tryGetConditionFromThisAndChild(proType.PropertyType, out infor))
                    {
                        Debug.Log($"Tim thay member can inject: {member}");
                        proType.SetValue(obj, infor.ObjectData);
                    }
                    else
                    {
                        EasyDILog.LogError($"Can't find binding {proType.PropertyType} for properties: {proType}!!");
                    }
                }

            }

            static void _combineConditions(ref Dictionary<Type, BindInfor> newInforFromChildContextAndThis, Dictionary<Type, BindInfor> dict1, Dictionary<Type, BindInfor> dict2)
            {
                //can phai tao dict moi

                foreach (var a in dict1)
                {
                    if (!newInforFromChildContextAndThis.ContainsKey(a.Key))
                        newInforFromChildContextAndThis.Add(a.Key, a.Value);
                }
                foreach (var a in dict2)
                {
                    if (!newInforFromChildContextAndThis.ContainsKey(a.Key))
                        newInforFromChildContextAndThis.Add(a.Key, a.Value);
                }

            }
        }

        private MemberInfo[] getAllMemberNeedInject(Type type)
        {

            var list = type.FindMembers(System.Reflection.MemberTypes.Field | System.Reflection.MemberTypes.Method | System.Reflection.MemberTypes.Property, System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance, filer, "ReferenceEquals");
            return list;
            bool filer(MemberInfo m, object filterCriteria)
            {
                return m.CustomAttributes.Any(_ => _.AttributeType == typeof(InjectAttribute));

            }

        }


    }
}