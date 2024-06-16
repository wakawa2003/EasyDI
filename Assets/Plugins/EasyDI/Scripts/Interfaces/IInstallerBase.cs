using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EasyDI
{
    public interface IInstallerBase
    {
        string InstallerName { get; }
        public ContainerBinding ContainerBinding { get; set; }

        public bool isInit { get; set; }
        public void Init()
        {
            if (!isInit)
                InstallBinding();
            isInit = true;
        }

        public void InstallBinding();

    }
}
