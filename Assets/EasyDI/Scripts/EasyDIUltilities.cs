using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

namespace EasyDI
{
    public class EasyDIUltilities
    {
        public static string BuildKeyInject(Type type, string tag)
        {
            return $"{type.ToString()}+{tag}";
        }
    }
}
