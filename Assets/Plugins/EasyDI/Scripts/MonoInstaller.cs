using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EasyDI
{
    public abstract class MonoInstaller : MonoBehaviour
    {
        ContainerBinding containerBinding = new ContainerBinding();

        public ContainerBinding ContainerBinding { get => containerBinding; private set => containerBinding = value; }

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
