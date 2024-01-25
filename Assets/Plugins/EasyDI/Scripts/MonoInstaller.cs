using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EasyDI
{
    public abstract class MonoInstaller : MonoBehaviour, IInstallerBase
    {
        public ContainerBinding ContainerBinding { get; set; } = new ContainerBinding();
        public bool isInit { get; set; } = false;

        public string InstallerName => gameObject.name;

        public abstract void InstallBinding();

    }
}
