using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EasyDI
{

    /// <summary>
    /// Only contain for field, properties, method,...
    /// </summary>
    public class ContainerInjectMember : MonoBehaviour
    {
        public Dictionary<string, InjectInfor> Dict_InjectKey_And_InjectInfor { get; private set; } = new Dictionary<string, InjectInfor>();
        public void AddTypeAndInfor(string injectKey, InjectInfor injectInfor)
        {
            if (Dict_InjectKey_And_InjectInfor.ContainsKey(injectKey))
            {
                EasyDILog.LogError($"Exist more than one {injectKey} binding in this container!");
            }
            else
            {
                Dict_InjectKey_And_InjectInfor.Add(injectKey, injectInfor);
            }
        }

    }
    public class InjectInfor
    {
        private InjectInfor()
        {
        }

        public InjectInfor(Type type, string tag)
        {
            Type = type;
            Tag = tag;
        }

        public Type Type { get; private set; }
        public string Tag { get; private set; }

        public override string ToString()
        {
            return $"{Type.ToString()}%{Tag}";
        }
    }

}
