using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Unity.VisualScripting;
using UnityEngine;

namespace EasyDI
{
    public class ContextBase : MonoBehaviour
    {
        Container container = new Container();
        [SerializeField] List<MonoInstaller> monoInstallerList = new List<MonoInstaller>();

        public List<MonoInstaller> MonoInstallerList { get => monoInstallerList; private set => monoInstallerList = value; }
        public Container Container { get => container; }
        ContextBase contextParent;

        protected virtual void Awake()
        {
            container = new Container();
            GetAllBindInforFromInstallerList();
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

        public void InjectFor(object obj, Type type, Dictionary<Type, BindInfor> inforFromChildContext)
        {

            if (contextParent != null)
            {
                //b1: tong hop infor condition tu child va chinh no
                //b2: goi => contextParent.InjectFor(obj, type, inforThisAndChild);

            }
            else
            {
                //inject cho obj ngoai tru nhung infor child
                var l = getAllMemberNeedInject(obj);
                foreach (var memberInfor in l)
                {
                    //Debug.Log($"type member: {item.DeclaringType.FullName}");
                    //Debug.Log($"member has Inject:{item}");
                    _setData(obj, memberInfor);
                }
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
                        var proType = ((PropertyInfo)member);
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
                    if (container.DictTypeAndData.TryGetValue(filedType.FieldType, out infor))
                    {
                        Debug.Log($"Tim thay member can inject: {member}");
                        filedType.SetValue(obj, infor.ObjectData);
                    }
                    else
                    {
                        EasyDILog.LogError($"Can't find binding {filedType.FieldType} for Method: {filedType}!!");
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
                        if (container.DictTypeAndData.TryGetValue(item, out infor))
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

            }
        }

        private MemberInfo[] getAllMemberNeedInject(object obj)
        {
            var type = obj.GetType();

            var list = type.FindMembers(System.Reflection.MemberTypes.Field | System.Reflection.MemberTypes.Method | System.Reflection.MemberTypes.Property, System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance, filer, "ReferenceEquals");
            return list;
            bool filer(MemberInfo m, object filterCriteria)
            {
                return m.CustomAttributes.Any(_ => _.AttributeType == typeof(InjectAttribute));

            }

        }


    }
}