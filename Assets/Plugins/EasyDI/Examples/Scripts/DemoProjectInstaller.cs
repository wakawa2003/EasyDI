using EasyDI;
using EasyDI.Demo;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Demo
{
    public class DemoProjectInstaller : MonoInstaller
    {
        protected override void InstallBinding()
        {
            Debug.Log($"project install binding");
            ContainerBinding.Bind<IMoveable>().To<CharacterBehaviour>().FromComponentInChild();
            ContainerBinding.Bind<Vector3>().To<Vector3>().FromInstance(new Vector3(4, 5, 6));
            ContainerBinding.Bind<string>().To<string>().FromInstance("project string");
            ContainerBinding.Bind<string>("tag1").To<string>().FromInstance("project string for tag1");
            ContainerBinding.Bind<float>().To<float>().FromInstance(999999f);
        }
    }
}
