using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EasyDI
{
    public abstract class MonoInstaller : MonoBehaviour
    {
        Container container = new Container();

        public Container Container { get => container; private set => container = value; }

        bool isInit = false;
        public void Init()
        {
            if (!isInit)
                InstallBinding();
            isInit = true;
        }

        protected virtual void InstallBinding()
        {
            throw new System.Exception("Need implement!");
        }
    }
}
