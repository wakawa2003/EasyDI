using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EasyDI
{
    public class EasyDIUltilities
    {
        public static string BuildKeyInject(Type type, string tag) => $"{type.ToString()}+{tag}";
    }
}
