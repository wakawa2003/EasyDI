using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EasyDI.Demo
{
    public class DemoSceneInstaller : MonoInstaller
    {
        public override void InstallBinding()
        {
            Debug.Log($"{gameObject.scene.name} install binding");
            //Container.Bind<string>().To<string>().FromInstance("Scene Demo string");
            ContainerBinding.Bind<Vector3>().To<Vector3>().FromInstance(new Vector3(1, 1, 3));
        }
    }
}
