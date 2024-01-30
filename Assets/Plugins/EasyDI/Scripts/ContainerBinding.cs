using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace EasyDI
{
    /// <summary>
    /// Only save binding!
    /// </summary>
    public class ContainerBinding
    {

        public Dictionary<string, BindInfor> Dict_InjectName_And_BindInfor { get; private set; } = new Dictionary<string, BindInfor>();

        public BindReturn<t> Bind<t>(string tag = "")
        {
            BindInfor bindInfor = new BindInfor();
            var bindType = new BindReturn<t>(bindInfor);
            var key = EasyDIUltilities.BuildKeyInject(typeof(t), tag);
            AddBinding(key, bindInfor);
            return bindType;
        }

        public void AddBinding(string injectKey, BindInfor bindInfor)
        {
            if (Dict_InjectName_And_BindInfor.ContainsKey(injectKey))
            {
                EasyDILog.LogError($"Exist more than one \"{injectKey}\" binding in this container!");
            }
            else
            {
                Dict_InjectName_And_BindInfor.Add(injectKey, bindInfor);
            }
        }

    }


    public class BindInfor
    {
        /// <summary>
        /// Not Null when set in binding.
        /// </summary>
        object objectData;

        public object ObjectData
        {
            get => objectData; set => objectData = value;
        }

        public EGetInstanceMethod GetInstanceMethod { get; set; }
        public EnumTreatWithInstanceMethod TreatWithInstanceMethod { get; set; }
        /// <summary>
        /// object: instance object.
        /// member: member need inject in object
        /// </summary>
        public Func<object, MemberInfo, object> CustomGetInstancePredict { get; set; }
        public Func<object, MemberInfo, bool> WherePredict { get; set; } = delegate { return true; };//default is alway true

        //object getObjectData(object instance)
        //{
        //    CustomGetInstancePredict.
        //}

        /// <summary>
        /// Method how to get instance.
        /// </summary>
        public enum EGetInstanceMethod
        {
            UnSet, OnlyThisGameObject, ItSelfAndComponentInChild, ItSelfAndComponentInParent
        }

        /// <summary>
        /// Method how to treat instance after get it.
        /// </summary>
        public enum EnumTreatWithInstanceMethod
        {
            UnSet, Singleton, Transient
        }

    }
    public class BindReturn<a>
    {
        BindInfor bindInfor;

        //constructor
        public BindReturn(BindInfor bindInfor)
        {
            this.bindInfor = bindInfor;
        }

        public ToReturn<a, b> To<b>() where b : a
        {
            return new ToReturn<a, b>(bindInfor);
        }
    }
    public class ToReturn<t, a>
    {
        BindInfor bindInfor;
        FromReturn<a> fromReturn;

        //constructor
        public ToReturn(BindInfor bindInfor)
        {
            this.bindInfor = bindInfor;
            fromReturn = new FromReturn<a>(bindInfor);
        }

        public FromReturn<a> FromComponentInChild()
        {
            bindInfor.GetInstanceMethod = BindInfor.EGetInstanceMethod.ItSelfAndComponentInChild;
            bindInfor.CustomGetInstancePredict = (ins, member) =>
            {
                if (ins is MonoBehaviour)
                {
                    return (ins as MonoBehaviour).GetComponentInChildren<t>();
                }
                else
                {
                    EasyDILog.LogError($"{ins.GetType().Name} is Not Monobehaviour!!!!");
                }
                return null;
            };
            return fromReturn;
        }

        public FromReturn<a> FromThisGameObject()
        {
            bindInfor.GetInstanceMethod = BindInfor.EGetInstanceMethod.OnlyThisGameObject;
            bindInfor.CustomGetInstancePredict = (ins, member) =>
            {
                if (ins is MonoBehaviour)
                {
                    return (ins as MonoBehaviour).GetComponent<t>();
                }
                else
                {
                    EasyDILog.LogError($"{ins.GetType().Name} is Not Monobehaviour!!!!");
                }
                return null;
            };
            return fromReturn;
        }


        public FromReturn<a> FromThisAndParent()
        {
            bindInfor.GetInstanceMethod = BindInfor.EGetInstanceMethod.ItSelfAndComponentInParent;
            bindInfor.CustomGetInstancePredict = (ins, member) =>
            {
                if (ins is MonoBehaviour)
                {
                    return (ins as MonoBehaviour).GetComponentInParent<t>();
                }
                else
                {
                    EasyDILog.LogError($"{ins.GetType().Name} is Not Monobehaviour!!!!");
                }
                return null;
            };
            return fromReturn;
        }

        public FromReturn<a> FromInstance(a value)
        {
            bindInfor.ObjectData = value;
            return fromReturn;
        }


        /// <summary>
        /// <see langword="object"/> : instance object
        /// <para> <see langword="MemberInfo"/> : member </para>
        /// </summary>
        /// <param name="func"></param>
        public void Where(Func<object, MemberInfo, bool> func)
        {
            bindInfor.WherePredict = func;
        }

        /// <summary>
        /// <see langword="object"/> : instance object
        /// <para> <see langword="MemberInfo"/> : member </para>
        /// </summary>
        /// <param name="func"></param>
        public void CustomGetInstance(Func<object, MemberInfo, object> func)
        {
            bindInfor.CustomGetInstancePredict = func;
        }
    }

    public class FromReturn<a>
    {
        BindInfor bindInfor;

        //constructor
        public FromReturn(BindInfor bindInfor)
        {
            this.bindInfor = bindInfor;
        }

    }

}
