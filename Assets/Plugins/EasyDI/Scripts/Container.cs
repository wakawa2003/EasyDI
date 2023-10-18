using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EasyDI
{
    public class Container
    {
        Dictionary<Type, object> dictTypeAndData = new Dictionary<Type, object>();
  
        public Dictionary<Type, object> DictTypeAndData { get => dictTypeAndData; private set => dictTypeAndData = value; }
    }
}
