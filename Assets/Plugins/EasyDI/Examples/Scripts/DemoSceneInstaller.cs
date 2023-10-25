using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EasyDI.Demo
{
    public class DemoSceneInstaller : MonoInstaller
    {
        protected override void InstallBinding()
        {
            Debug.Log("Scene install");
            //Container.Bind<string>().To<string>().FromInstance("Scene Demo string");
            Container.Bind<Vector3>().To<Vector3>().FromInstance(new Vector3(1, 1, 3));
        }
    }
}
