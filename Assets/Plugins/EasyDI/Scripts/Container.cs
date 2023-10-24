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
    public class Container
    {
        Dictionary<Type, BindInfor> dictTypeAndData = new Dictionary<Type, BindInfor>();

        public Dictionary<Type, BindInfor> DictTypeAndData { get => dictTypeAndData; private set => dictTypeAndData = value; }

        public BindReturn<t> Bind<t>()
        {
            BindInfor bindInfor = new BindInfor();
            var bindType = new BindReturn<t>(bindInfor);
            var type = typeof(t);

            AddTypeAndInfor(type, bindInfor);
            return bindType;
        }

        public void AddTypeAndInfor(Type type, BindInfor bindInfor)
        {
            if (DictTypeAndData.ContainsKey(type))
            {
                EasyDILog.LogError($"Exist more than one {type} binding in this container!");
            }
            else
            {
                DictTypeAndData.Add(type, bindInfor);
            }
        }


    }

    public class BindInfor
    {
        /// <summary>
        /// Not Null when set in binding.
        /// </summary>
        public object ObjectData { get; set; }
        public EGetInstanceMethod GetInstanceMethod { get; set; }
        public EnumTreatWithInstanceMethod TreatWithInstanceMethod { get; set; }
        public Func<object, MemberInfo, object> CustomGetInstancePredict { get; set; }
        public Func<object, MemberInfo, bool> WherePredict { get; set; }

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

        public ToReturn<b> To<b>() where b : a
        {
            return new ToReturn<b>(bindInfor);
        }
    }
    public class ToReturn<a>
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
            return fromReturn;
        }

        public FromReturn<a> FromThisGameObject()
        {
            bindInfor.GetInstanceMethod = BindInfor.EGetInstanceMethod.OnlyThisGameObject;
            return fromReturn;
        }


        public FromReturn<a> FromThisAndParent()
        {
            bindInfor.GetInstanceMethod = BindInfor.EGetInstanceMethod.ItSelfAndComponentInParent;
            return fromReturn;
        }

        public FromReturn<a> FromInstance(a value)
        {
            bindInfor.ObjectData = value;
            return fromReturn;
        }
        
        public void Where(Func<object, MemberInfo, bool> func)
        {
            bindInfor.WherePredict = func;
        }
        
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
