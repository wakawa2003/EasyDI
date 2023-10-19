using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EasyDI
{
    public class EasyDILog : MonoBehaviour
    {
        public static void LogError(string message)
        {
            Debug.LogError(message);
        }
        public static void Log(string message)
        {
            Debug.Log(message);
        }
        public static void LogWarning(string message)
        {
            Debug.LogWarning(message);
        }
    }
}
