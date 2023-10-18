using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EasyDI
{
    public abstract class MonoInstaller : MonoBehaviour
    {
        Container container;

        public Container Container { get => container; private set => container = value; }

        protected void InstallBinding()
        {
            throw new System.Exception("Need implement!");
        }
    }
}
