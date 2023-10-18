using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EasyDI
{
    public class EntryEasyDI : MonoBehaviour
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void OnBeforeSceneLoad()
        {
            Debug.Log("First scene loading: Before Awake is called.");
            var a = ProjectContext.Ins;
        }

    }
}
