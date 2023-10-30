using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices.WindowsRuntime;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Windows;

namespace EasyDI
{
    [DisallowMultipleComponent]
    public abstract class ContextBase : MonoBehaviour
    {
        ContainerBinding containerBinding = new ContainerBinding();
        //ContainerInjectMember containerInjectMember = new ContainerInjectMember();
        [SerializeField] List<MonoInstaller> monoInstallerList = new List<MonoInstaller>();

        public List<MonoInstaller> MonoInstallerList { get => monoInstallerList; private set => monoInstallerList = value; }
        public ContainerBinding ContainerBinding { get => containerBinding; }
        //public ContainerInjectMember ContainerInjectMember { get => containerInjectMember; }

        ContextBase contextParent;

        protected abstract ContextBase GetParentContext();
        bool isInit = false;
        protected void Init()
        {
            if (!isInit)
            {
                //containerInjectMember = new ContainerInjectMember();
                containerBinding = new ContainerBinding();
                contextParent = GetParentContext();
                GetAllBindInforFromInstallerList();
                isInit = true;
            }
        }

        protected virtual void Awake()
        {
            Init();
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
                foreach (var item in monoInstaller.ContainerBinding.Dict_InjectName_And_BindInfor)
                {
                    containerBinding.AddBinding(item.Key, item.Value);
                }

            }
        }

        public void InjectFor(object obj)
        {
            InjectFor(obj, containerBinding.Dict_InjectName_And_BindInfor);
        }

        public void InjectFor(object obj, Dictionary<string, BindInfor> inforFromChildContext)
        {
            Init();
            Dictionary<string, BindInfor> newInforFromChildContextAndThis = new Dictionary<string, BindInfor> { };
            _combineConditions(ref newInforFromChildContextAndThis, inforFromChildContext, containerBinding.Dict_InjectName_And_BindInfor);

            if (contextParent != null)
            {
                //b1: tong hop infor condition from child and it self
                //b2: call => contextParent.InjectFor(obj, inforThisAndChild);
                contextParent.InjectFor(obj, newInforFromChildContextAndThis);
            }
            else
            {
                //inject cho obj ngoai tru nhung infor child
                List<MemberInfo> memberInfoOut = new List<MemberInfo>();
                List<InjectAttribute> injectAttributeOut = new List<InjectAttribute> { };
                getAllMemberNeedInject(obj.GetType(), memberInfoOut, injectAttributeOut);
                for (int i = 0; i < memberInfoOut.Count; i++)
                {
                    var memberInfor = memberInfoOut[i];
                    var injectAttribute = injectAttributeOut[i];
                    //Debug.Log($"type member: {item.DeclaringType.FullName}");
                    //Debug.Log($"member has Inject:{item}");
                    _setDataForMember(obj, memberInfor, injectAttribute);
                }
            }

            bool _tryGetConditionFromThisAndChild(string key, out BindInfor bindInfor)
            {
                return newInforFromChildContextAndThis.TryGetValue(key, out bindInfor);
            }

            void _setDataForMember(object obj, MemberInfo member, InjectAttribute injectAttribute)
            {
                switch (member.MemberType)
                {
                    case MemberTypes.Event:
                        break;
                    case MemberTypes.Field:
                        _setForField(obj, member, injectAttribute);
                        break;
                    case MemberTypes.Method:
                        _setForMethod(obj, member, injectAttribute);
                        break;
                    case MemberTypes.Property:
                        _setForProperties(obj, member, injectAttribute);
                        break;
                    default:
                        throw new ArgumentException
                        (
                         "Input MemberInfo must be if type EventInfo, FieldInfo, MethodInfo, or PropertyInfo"
                        );
                }

                void _setForField(object obj, MemberInfo member, InjectAttribute injectAttribute)
                {
                    var filedType = (member as FieldInfo);
                    BindInfor infor = null;
                    var key = EasyDIUltilities.BuildKeyInject(filedType.FieldType, injectAttribute.Tag);

                    if (_tryGetConditionFromThisAndChild(key, out infor))
                    {
                        filedType.SetValue(obj, infor.ObjectData);
                    }
                    else
                    {
                        EasyDILog.LogError($"Can't find binding {filedType.FieldType} for field: {filedType}!!");
                    }
                }

                void _setForMethod(object obj, MemberInfo member, InjectAttribute injectAttribute)
                {
                    var methodInfor = (member as MethodInfo);
                    var @params = methodInfor.GetParameters();
                    object[] args = new object[@params.Length];
                    for (int i = 0; i < @params.Length; i++)
                    {
                        BindInfor infor = null;
                        var item = @params[i].ParameterType;
                        var key = EasyDIUltilities.BuildKeyInject(item, injectAttribute.Tag);
                        if (_tryGetConditionFromThisAndChild(key, out infor))
                        {
                            args[i] = infor.ObjectData;
                        }
                        else
                        {
                            EasyDILog.LogError($"Can't find binding {item} for Method: {methodInfor}!!");
                        }
                    }
                    methodInfor.Invoke(obj, args);
                }
                void _setForProperties(object obj, MemberInfo member, InjectAttribute injectAttribute)
                {
                    var proType = ((PropertyInfo)member);
                    BindInfor infor = null;
                    var key = EasyDIUltilities.BuildKeyInject(proType.PropertyType, injectAttribute.Tag);
                    if (_tryGetConditionFromThisAndChild(key, out infor))
                    {
                        proType.SetValue(obj, infor.ObjectData);
                    }
                    else
                    {
                        EasyDILog.LogError($"Can't find binding {proType.PropertyType} for properties: {proType}!!");
                    }
                }

            }

            static void _combineConditions(ref Dictionary<string, BindInfor> outDict, Dictionary<string, BindInfor> dict1, Dictionary<string, BindInfor> dict2)
            {
                //can phai tao dict moi

                foreach (var a in dict1)
                {
                    if (!outDict.ContainsKey(a.Key))
                        outDict.Add(a.Key, a.Value);
                }
                foreach (var a in dict2)
                {
                    if (!outDict.ContainsKey(a.Key))
                        outDict.Add(a.Key, a.Value);
                }

            }
        }

        protected void getAllMemberNeedInject(Type type, List<MemberInfo> memberInfoOut, List<InjectAttribute> injectAttributeOut)
        {

            var list = type.FindMembers(MemberTypes.Field | MemberTypes.Method | MemberTypes.Property, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance, filer, "ReferenceEquals");

            bool filer(MemberInfo m, object filterCriteria)
            {
                var attList = m.GetCustomAttributes<InjectAttribute>(false);

                if (attList.Count() > 0)
                {
                    memberInfoOut.Add(m);
                    injectAttributeOut.Add(attList.First());
                }
                return attList.Count() > 0;
            }

        }


    }
}