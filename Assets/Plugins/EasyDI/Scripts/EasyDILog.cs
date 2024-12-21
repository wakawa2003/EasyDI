using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EasyDI
{
    public class EasyDILog : MonoBehaviour
    {
        const string prefix = "<color=red>EasyDI: </color>";
        public static void LogError(string message)
        {
            Debug.LogError(prefix + message);
        }
        public static void Log(string message)
        {
            Debug.Log(prefix + message);
        }
        public static void LogWarning(string message)
        {
            Debug.LogWarning(prefix + message);
        }
    }
}
