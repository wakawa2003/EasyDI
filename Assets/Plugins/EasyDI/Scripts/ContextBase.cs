using Codice.CM.SEIDInfo;
using PlasticGui.Configuration.CloudEdition;
using PlasticPipe.PlasticProtocol.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEditorInternal;
using UnityEngine;

namespace EasyDI
{
    [DisallowMultipleComponent]
    public abstract class ContextBase : MonoBehaviour
    {
        ContainerBinding containerBinding;
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
                containerBinding = new ContainerBinding(name);
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
                EasyDILog.LogWarning($"Context has name: \'{name}\' doesn't have InstallerList");
            foreach (var installer in InstallerList)
            {
                installer.Init();

                //combine normal
                foreach (var item in installer.ContainerBinding.Dict_InjectName_And_BindInfor)
                {
                    containerBinding.AddBinding(item.Key, item.Value, false);
                }

                //combine for decore
                foreach (var item in installer.ContainerBinding.Dict_ListBindInforDecore)
                {
                    foreach (var item2 in item.Value)
                    {
                        containerBinding.AddBinding(item.Key, item2, true);
                    }
                }

            }
        }

        public void InjectFor(object obj)
        {
            InjectFor(obj, containerBinding.Dict_InjectName_And_BindInfor, containerBinding.Dict_ListBindInforDecore);
        }

        public void InjectFor(object objectNeedInject, Dictionary<string, BindInfor> bindInfor_FromChildContext, Dictionary<string, List<BindInfor>> bindInfor_Decore_FromChildContext)
        {
            //Neu la decore thi:
            //      B1: combine cac BindInfor la Decore thanh 1 list roi gui list len parentContext
            //      B2: khi den root parent thi: uu tien inject cho cac BindInfor binh thuong roi moi den BinInfor La Decore


            //neu
            Init();


            Dictionary<string, BindInfor> newBindInforFromChildContextAndThis = new Dictionary<string, BindInfor> { };
            Dictionary<string, List<BindInfor>> newBindInforDecoreFromChildContext = new();
            _combineConditions(ref newBindInforFromChildContextAndThis, bindInfor_FromChildContext, containerBinding.Dict_InjectName_And_BindInfor);
            _combineConditionsDecore(ref newBindInforDecoreFromChildContext, bindInfor_Decore_FromChildContext, containerBinding.Dict_ListBindInforDecore);

            if (contextParent != null)
            {
                //b1: tong hop infor condition from child and it self
                //b2: call => contextParent.InjectFor(obj, inforThisAndChild);
                contextParent.InjectFor(objectNeedInject, newBindInforFromChildContextAndThis, newBindInforDecoreFromChildContext);
            }
            else
            {
                //inject cho obj ngoai tru nhung infor child
                List<MemberInfo> memberInfoOut = new List<MemberInfo>();
                List<InjectAttribute> injectAttributeOut = new List<InjectAttribute> { };
                GetAllMemberNeedInject(objectNeedInject.GetType(), ref memberInfoOut, ref injectAttributeOut);
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
                return newBindInforFromChildContextAndThis.TryGetValue(key, out bindInfor);
            }


            bool _tryGetDecoreFromThisAndChild(string key, out List<BindInfor> decoreList)
            {
                return newBindInforDecoreFromChildContext.TryGetValue(key, out decoreList);
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

                //return lastest decore object
                static object _decore(object obj, List<BindInfor> decoreList, Action<object, MemberInfo, object> setdataPredict)
                {
                    int i = 0;

                    foreach (BindInfor bind in decoreList)
                    {
                        Debug.Log($"decore List[{i}]: {bind.ID}");
                        i++;
                        if (obj != null)
                        {
                            var t = _decoreSingle(obj, bind, setdataPredict);
                            if (t != null)//skip this bind if data = null
                            {
                                obj = t;
                            }

                        }
                    }
                    return obj;

                    //return new obj 
                    static object _decoreSingle(object obj, BindInfor bindInfor, Action<object, MemberInfo, object> onSetdataPredict)
                    {
                        Debug.Log($"decore obj: {obj.GetHashCode()}");
                        var member = _getMemberIsDecoratorInObject(obj, bindInfor.TypeTarget);
                        if (member != null)
                        {
                            //var fieldType = (member as FieldInfo);
                            if (checkWherePredict(bindInfor.WherePredict, obj, member))
                            {
                                var data = _getObjectDataFromBindInfor(obj, bindInfor, member);
                                //Debug.Log($"data obj: {data.GetHashCode()}");
                                onSetdataPredict?.Invoke(obj, member, data);

                                return data;
                            }
                        }
                        Debug.Log($"nulllll");
                        return null;
                    }

                    static MemberInfo _getMemberIsDecoratorInObject(object obj, Type typeDecore)
                    {
                        List<MemberInfo> memberInfoOut = new List<MemberInfo>();
                        List<InjectAttribute> injectAttributeOut = new List<InjectAttribute> { };
                        //Debug.Log($"type111: {obj.GetType()}");
                        GetAllMemberNeedInject(obj.GetType(), ref memberInfoOut, ref injectAttributeOut);
                        //Debug.Log($"found members: {memberInfoOut.Count}");
                        //foreach (var item in memberInfoOut)
                        //{
                        //    Debug.Log($"member type: {item.GetUnderlyingType().Name}");

                        //}
                        //Debug.Log($"typeDecore: {typeDecore.Name}");
                        var decore = memberInfoOut.Find(_ => _.GetUnderlyingType().Name == typeDecore.Name);
                        //if (decore == null)
                        //    Debug.Log($"cant find");
                        return decore;
                    }
                }

                void _setForField(object obj, MemberInfo member, InjectAttribute injectAttribute)
                {
                    var filedType = (member as FieldInfo);
                    BindInfor bindInfor = null;
                    var key = EasyDIUltilities.BuildKeyInject(filedType.FieldType, injectAttribute.Tag);

                    if (_tryGetConditionFromThisAndChild(key, out bindInfor))
                    {
                        if (checkWherePredict(bindInfor.WherePredict, objectNeedInject, member))
                        {
                            var data = _getObjectDataFromBindInfor(obj, bindInfor, member);
                            //set value
                            filedType.SetValue(obj, data);

                            // start decore handle
                            if (data != null)
                            {
                                List<BindInfor> decoreList = new List<BindInfor>();
                                if (_tryGetDecoreFromThisAndChild(key, out decoreList))
                                {
                                    _decore(data, decoreList, (obj, member, data) =>
                                     {
                                         var fieldType = (member as FieldInfo);
                                         fieldType.SetValue(obj, data);
                                     });
                                }
                            }
                            //end decore handle



                        }
                    }
                    else
                    {
                        EasyDILog.LogError($"Can't find binding {filedType.FieldType.Name} for field: {filedType.Name}!!");
                    }

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

                            // start decore handle
                            if (data != null)
                            {
                                List<BindInfor> decoreList = new List<BindInfor>();
                                if (_tryGetDecoreFromThisAndChild(key, out decoreList))
                                {
                                    _decore(data, decoreList, (obj, member, data) =>
                                   {
                                       var fieldType = (member as PropertyInfo);
                                       fieldType.SetValue(obj, data);
                                   });
                                }
                            }
                            //end decore handle



                        }
                    }
                    else
                    {
                        EasyDILog.LogError($"Can't find binding {proType.PropertyType.Name} for properties: {proType.Name}!!");
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


                static object _getObjectDataFromBindInfor(object instanceNeedInject, BindInfor bindInfor, MemberInfo memberInfoNeedInject)
                {
                    object r = null;
                    switch (bindInfor.TreatWithInstanceMethod)
                    {
                        case BindInfor.EnumTreatWithInstanceMethod.UnSet:
                            goto case BindInfor.EnumTreatWithInstanceMethod.Transient;
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

            static void _combineConditionsDecore(ref Dictionary<string, List<BindInfor>> outDict, Dictionary<string, List<BindInfor>> dict1, Dictionary<string, List<BindInfor>> dict2)
            {

                //Debug.Log($"{nameof(dict1)}:");
                //logDict(dict1);
                //Debug.Log($"{nameof(dict2)}:");
                //logDict(dict2);
                //Debug.Log($"end");
                //can phai tao dict moi
                foreach (var a in dict1)
                {
                    if (!outDict.ContainsKey(a.Key))
                        outDict.Add(a.Key, a.Value.ToList());
                }
                foreach (var a in dict2)
                {
                    //if (!outDict.ContainsKey(a.Key))
                    //    outDict.Add(a.Key, a.Value.ToList());
                    if (outDict.ContainsKey(a.Key))
                    {
                        foreach (var newValue in dict2[a.Key])
                        {
                            if (!outDict[a.Key].Contains(newValue))
                                outDict[a.Key].Add(newValue);

                        }
                    }
                    else
                        outDict.Add(a.Key, a.Value.ToList());
                }


                void logDict(Dictionary<string, List<BindInfor>> dict1)
                {
                    foreach (var item in dict1)
                    {

                        Debug.Log($"key: \'{item}\'");
                        foreach (var v in item.Value)
                        {

                            Debug.Log($"---value: {v.ID}");
                        }
                    }
                }
            }

            static bool checkWherePredict(Func<object, MemberInfo, bool> wherePredict, object instance, MemberInfo memberInfo)
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


        /// <summary>
        /// find all field, properties, method mark [Inject] attribute.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="memberInfoOut"></param>
        /// <param name="injectAttributeOut"></param>
        public static void GetAllMemberNeedInject(Type type, ref List<MemberInfo> memberInfoOut, ref List<InjectAttribute> injectAttributeOut)
        {
            var cache = EasyDICache.Instance;

            //tiep (cache tra ve sai cho decore)
            //searching in cache
            if (cache.HasClass(type))
            {
                var t = cache.GetContainerTypeInject(type);
                memberInfoOut = t.MemberList;
                injectAttributeOut = t.InjectAttributeList;

                Debug.LogWarning($"get cache type: {t.Type.Name}");
                foreach (MemberInfo memberInfo in memberInfoOut)
                {

                    Debug.LogWarning($"----member name: {memberInfo.Name}");
                    Debug.LogWarning($"----member type: {memberInfo.GetUnderlyingType().Name}");
                }
            }
            else//resolve if not found
            {
                Dictionary<string, string> dictKey = new();
                List<MemberInfo> memberFoundList = new();
                List<InjectAttribute> injectAttributeFOundList = new();

                var list = type.FindMembers(MemberTypes.Field | MemberTypes.Method | MemberTypes.Property, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance, filer, "ReferenceEquals");

                if (memberFoundList.Count > 0)
                    if (injectAttributeFOundList.Count > 0)
                    {
                        cache.AddInjectClass(type, memberFoundList, injectAttributeFOundList);
                        memberInfoOut = memberFoundList;
                        injectAttributeOut = injectAttributeFOundList;
                    }
                bool filer(MemberInfo m, object filterCriteria)
                {
                    var attList = m.GetCustomAttribute<InjectAttribute>(false);

                    bool isValid = false;
                    if (attList != null)
                    {

                        isValid = true;
                        switch (m.MemberType)
                        {
                            case MemberTypes.All:
                                break;
                            case MemberTypes.Constructor:
                                break;
                            case MemberTypes.Custom:
                                break;
                            case MemberTypes.Event:
                                break;
                            case MemberTypes.Field:
                                isValid = _checkValid(type, m.GetUnderlyingType().ToString(), dictKey, attList);
                                break;
                            case MemberTypes.Method:
                                var methodInfor = (m as MethodInfo);
                                var @params = methodInfor.GetParameters();
                                foreach (var item in @params)
                                {
                                    if (!_checkValid(type, item.ParameterType.ToString(), dictKey, attList))
                                    {
                                        isValid = false;
                                    }
                                }
                                break;
                            case MemberTypes.NestedType:
                                break;
                            case MemberTypes.Property:
                                isValid = _checkValid(type, m.GetUnderlyingType().ToString(), dictKey, attList);
                                break;
                            case MemberTypes.TypeInfo:
                                break;
                            default:
                                break;
                        }

                        if (isValid)
                        {
                            memberFoundList.Add(m);
                            injectAttributeFOundList.Add(attList);
                        }
                    }
                    return isValid;
                }
            }


            static bool _checkValid(Type type, string typeMember, Dictionary<string, string> dictKey, InjectAttribute att)
            {

                //ensure only one member has tag Inject("tag") per one Type per one Object.
                var key = type + att.Tag + typeMember;
                if (dictKey.ContainsKey(key))
                {

                    Debug.LogError($"Contains more than one member \"{typeMember}\" has [Inject(\'{att.Tag}\')] in class \"{type}\"");
                    return false;
                }
                else
                {
                    dictKey.Add(key, typeMember);
                }

                return true;

            }


        }


    }
}

