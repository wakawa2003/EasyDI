using System;
using System.Collections;
using System.Collections.Generic;
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

        public BindType<t> Bind<t>()
        {
            BindInfor bindInfor = new BindInfor();
            var bindType = new BindType<t>(bindInfor);
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


        /// <summary>
        /// Method how to get instance.
        /// </summary>
        public enum EGetInstanceMethod
        {
            UnSet, OnlyThisGameObject, ItSelfAndComponentInChild
        }

        /// <summary>
        /// Method how to treat instance after get it.
        /// </summary>
        public enum EnumTreatWithInstanceMethod
        {
            UnSet, Singleton, Transient
        }
    }
    public class BindType<a>
    {
        BindInfor bindInfor;
        //constructor
        public BindType(BindInfor bindInfor)
        {
            this.bindInfor = bindInfor;
        }

        public ToType<b> To<b>() where b : a
        {
            return new ToType<b>(bindInfor);
        }
    }
    public class ToType<a>
    {
        BindInfor bindInfor;

        //constructor
        public ToType(BindInfor bindInfor)
        {
            this.bindInfor = bindInfor;
        }

        public void FromComponentInChild()
        {
            bindInfor.GetInstanceMethod = BindInfor.EGetInstanceMethod.ItSelfAndComponentInChild;
        }

        public void FromThisGameObject()
        {
            bindInfor.GetInstanceMethod = BindInfor.EGetInstanceMethod.OnlyThisGameObject;
        }
    }


}
