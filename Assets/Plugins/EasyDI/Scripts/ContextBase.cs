using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EasyDI
{
    public class ContextBase : MonoBehaviour
    {
        Dictionary<Type, object> dictTypeAndData = new Dictionary<Type, object>();
        [SerializeField] List<MonoInstaller> monoInstaller = new List<MonoInstaller>();

        public List<MonoInstaller> MonoInstaller { get => monoInstaller; private set => monoInstaller = value; }
        public Dictionary<Type, object> DictTypeAndData { get => dictTypeAndData; private set => dictTypeAndData = value; }
    }
}
