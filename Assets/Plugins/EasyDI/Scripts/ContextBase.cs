using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace EasyDI
{
    [DisallowMultipleComponent]
    public abstract class ContextBase : MonoBehaviour
    {
        ContainerBinding containerBinding = new ContainerBinding();
        [SerializeField] List<IInstallerBase> installerList = new List<IInstallerBase>();

        public List<IInstallerBase> InstallerList { get => installerList; private set => installerList = value; }
        public ContainerBinding ContainerBinding { get => containerBinding; }
        ContextBase contextParent;

        protected abstract ContextBase GetParentContext();
        protected bool isInit = false;
        protected virtual void Init()
        {
            if (!isInit)
            {
                //Debug.Log($"{gameObject.name} INITTTT");
                //containerInjectMember = new ContainerInjectMember();
                containerBinding = new ContainerBinding();
                contextParent = GetParentContext();
                GetAllBindInforFromInstallerList();
                isInit = true;
            }
        }


        protected virtual void OnDestroy()
        {

        }

        /// <summary>
        /// combine bind conditional.
        /// </summary>
        private void GetAllBindInforFromInstallerList()
        {
            if (InstallerList.Count() == 0)
                EasyDILog.LogWarning($"Context has name: {name} doesn't have InstallerList");
            foreach (var installer in InstallerList)
            {
                installer.Init();
                foreach (var item in installer.ContainerBinding.Dict_InjectName_And_BindInfor)
                {
                    containerBinding.AddBinding(item.Key, item.Value);
                }

            }
        }

        public void InjectFor(object obj)
        {
            InjectFor(obj, containerBinding.Dict_InjectName_And_BindInfor);
        }

        public void InjectFor(object objectNeedInject, Dictionary<string, BindInfor> inforFromChildContext)
        {
            Init();
            Dictionary<string, BindInfor> newInforFromChildContextAndThis = new Dictionary<string, BindInfor> { };
            _combineConditions(ref newInforFromChildContextAndThis, inforFromChildContext, containerBinding.Dict_InjectName_And_BindInfor);

            if (contextParent != null)
            {
                //b1: tong hop infor condition from child and it self
                //b2: call => contextParent.InjectFor(obj, inforThisAndChild);
                contextParent.InjectFor(objectNeedInject, newInforFromChildContextAndThis);
            }
            else
            {
                //inject cho obj ngoai tru nhung infor child
                List<MemberInfo> memberInfoOut = new List<MemberInfo>();
                List<InjectAttribute> injectAttributeOut = new List<InjectAttribute> { };
                GetAllMemberNeedInject(objectNeedInject.GetType(), memberInfoOut, injectAttributeOut);
                for (int i = 0; i < memberInfoOut.Count; i++)
                {
                    var memberInfor = memberInfoOut[i];
                    var injectAttribute = injectAttributeOut[i];
                    //Debug.Log($"type member: {item.DeclaringType.FullName}");
                    //Debug.Log($"member has Inject:{item}");

                    _setDataForMember(objectNeedInject, memberInfor, injectAttribute);
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

                void _setForField(object value, MemberInfo member, InjectAttribute injectAttribute)
                {
                    var filedType = (member as FieldInfo);
                    BindInfor bindInfor = null;
                    var key = EasyDIUltilities.BuildKeyInject(filedType.FieldType, injectAttribute.Tag);

                    if (_tryGetConditionFromThisAndChild(key, out bindInfor))
                    {
                        if (checkWherePredict(bindInfor.WherePredict, objectNeedInject, member))
                        {
                            var data = _getObjectDataFromBindInfor(value, bindInfor, member);
                            filedType.SetValue(value, data);
                        }
                    }
                    else
                    {
                        EasyDILog.LogError($"Can't find binding {filedType.FieldType.Name} for field: {filedType.Name}!!");
                    }
                }

                void _setForMethod(object obj, MemberInfo member, InjectAttribute injectAttribute)
                {
                    var methodInfor = (member as MethodInfo);
                    var @params = methodInfor.GetParameters();
                    object[] args = new object[@params.Length];
                    for (int i = 0; i < @params.Length; i++)
                    {
                        BindInfor bindInfor = null;
                        var item = @params[i].ParameterType;
                        var key = EasyDIUltilities.BuildKeyInject(item, injectAttribute.Tag);
                        if (_tryGetConditionFromThisAndChild(key, out bindInfor))
                        {
                            //check wherePredict bofore Inject
                            if (checkWherePredict(bindInfor.WherePredict, objectNeedInject, member))
                            {
                                var data = _getObjectDataFromBindInfor(obj, bindInfor, member);
                                args[i] = data;

                            }
                        }
                        else
                        {
                            EasyDILog.LogError($"Can't find binding key {key} for Method: {methodInfor.Name}!!");
                        }
                    }
                    methodInfor.Invoke(obj, args);
                }
                void _setForProperties(object obj, MemberInfo member, InjectAttribute injectAttribute)
                {
                    var proType = ((PropertyInfo)member);
                    BindInfor bindInfor = null;
                    var key = EasyDIUltilities.BuildKeyInject(proType.PropertyType, injectAttribute.Tag);
                    if (_tryGetConditionFromThisAndChild(key, out bindInfor))
                    {
                        if (checkWherePredict(bindInfor.WherePredict, objectNeedInject, member))
                        {
                            var data = _getObjectDataFromBindInfor(obj, bindInfor, member);
                            proType.SetValue(obj, data);
                        }
                    }
                    else
                    {
                        EasyDILog.LogError($"Can't find binding {proType.PropertyType.Name} for properties: {proType.Name}!!");
                    }
                }


                static object _getObjectDataFromBindInfor(object instanceNeedInject, BindInfor bindInfor, MemberInfo memberInfoNeedInject)
                {
                    object r = null;
                    switch (bindInfor.TreatWithInstanceMethod)
                    {
                        case BindInfor.EnumTreatWithInstanceMethod.UnSet:
                            goto case BindInfor.EnumTreatWithInstanceMethod.Transient;
                            break;
                        case BindInfor.EnumTreatWithInstanceMethod.Singleton:
                            if (_checkFromInstanceCache(out r, memberInfoNeedInject))
                            {
                                if (r == null)
                                {
                                    EasyDILog.LogError($"Inject Singleton has type \'{memberInfoNeedInject.Name}\' but value is null. Auto get new intance from predict!!");
                                    r = _getDataAndAddToCache(instanceNeedInject, bindInfor, memberInfoNeedInject);
                                }
                            }
                            else
                            {
                                //add new value to cache
                                r = _getDataAndAddToCache(instanceNeedInject, bindInfor, memberInfoNeedInject);
                            }

                            break;
                        case BindInfor.EnumTreatWithInstanceMethod.Transient:
                            r = _getObjectFromCustomPredict(instanceNeedInject, bindInfor, memberInfoNeedInject);
                            break;
                        default:
                            break;
                    }

                    return r;

                    static bool _checkFromInstanceCache(out object outData, MemberInfo memberInfo)
                    {
                        var type = memberInfo.GetUnderlyingType();
                        if (EasyDICache.Instance.HasInstanceCache(type))
                        {
                            outData = EasyDICache.Instance.GetInstanceCache(type);
                            return true;
                        }
                        else
                        {
                            outData = null;
                            return false;
                        }
                    }

                    static object _getObjectFromCustomPredict(object instanceNeedInject, BindInfor bindInfor, MemberInfo memberInfoNeedInject)
                    {
                        //Debug.Log($"bind: {instance.GetType()}");
                        if (bindInfor.CustomGetInstancePredict != null && bindInfor.ObjectData == null)
                            return bindInfor.CustomGetInstancePredict?.Invoke(instanceNeedInject, memberInfoNeedInject);
                        if (bindInfor.CustomGetInstancePredict == null && bindInfor.ObjectData != null)
                        {
                            return bindInfor.ObjectData;

                        }
                        EasyDILog.LogError("Both bindInfor.CustomGetInstancePredict and bindInfor.ObjectData not Null!!");
                        return bindInfor.ObjectData;
                    }

                    static object _getDataAndAddToCache(object instanceNeedInject, BindInfor bindInfor, MemberInfo memberInfoNeedInject)
                    {
                        object r = _getObjectFromCustomPredict(instanceNeedInject, bindInfor, memberInfoNeedInject);
                        EasyDICache.Instance.AddInstanceCache(memberInfoNeedInject.GetUnderlyingType(), r);
                        return r;
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

            bool checkWherePredict(Func<object, MemberInfo, bool> wherePredict, object instance, MemberInfo memberInfo)
            {
                if (wherePredict != null)
                {
                    return wherePredict.Invoke(instance, memberInfo);
                }
                else
                {
                    EasyDILog.LogError("WherePredict is Null, please don't set to Null!!!!");
                }
                return false;
            }
        }

        public static void GetAllMemberNeedInject(Type type, List<MemberInfo> memberInfoOut, List<InjectAttribute> injectAttributeOut)
        {
            var cache = EasyDICache.Instance;

            //searching in cache
            if (cache.HasClass(type))
            {
                var t = cache.GetContainerTypeInject(type);
                memberInfoOut = t.MemberList;
                injectAttributeOut = t.InjectAttributeList;
            }
            else//resolve if not found
            {
                var list = type.FindMembers(MemberTypes.Field | MemberTypes.Method | MemberTypes.Property, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance, filer, "ReferenceEquals");
                cache.AddInjectClass(type, memberInfoOut, injectAttributeOut);

            }

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

