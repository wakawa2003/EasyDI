using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace EasyDI
{
    public class EasyDICache
    {
        Dictionary<Type, ContainerTypeInject> dictInject = new Dictionary<Type, ContainerTypeInject>();
        private static EasyDICache instance;

        public static EasyDICache Instance
        {
            get
            {
                if (instance == null)
                    instance = new EasyDICache();
                return instance;
            }
        }

        private EasyDICache()
        {
            Debug.Log($"EasyDI Cache Init!!!");

        }




        public void AddInjectClass(Type type, List<MemberInfo> memberInfoList, List<InjectAttribute> injectAttributeList)
        {
            if (!dictInject.ContainsKey(type))
            {
                dictInject.Add(type, new ContainerTypeInject(type, memberInfoList, injectAttributeList));
            }

        }

        public bool HasClass(Type type)
        {
            return dictInject.ContainsKey(type);
        }


        public ContainerTypeInject GetContainerTypeInject(Type type)
        {
            if (dictInject.ContainsKey(type))
            {
                return dictInject[type];
            }
            EasyDILog.LogError($"Doesn't contain type: {type.Name}");
            return null;
        }

        public class ContainerTypeInject
        {
            public Type Type;
            public List<MemberInfo> MemberList;
            public List<InjectAttribute> InjectAttributeList;

            private ContainerTypeInject()
            {
            }

            public ContainerTypeInject(Type type, List<MemberInfo> memberList, List<InjectAttribute> injectAttributeList)
            {
                Type = type;
                MemberList = memberList;
                InjectAttributeList = injectAttributeList;
            }
        }
    }
}
