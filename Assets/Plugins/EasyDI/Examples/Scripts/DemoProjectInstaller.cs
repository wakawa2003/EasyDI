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
            Container.Bind<IMoveable>().To<CharacterBehaviour>().FromComponentInChild();
            Container.Bind<Vector3>().To<Vector3>().FromInstance(new Vector3(4, 5, 6));
            Container.Bind<string>().To<string>().FromInstance("project string");
            Container.Bind<float>().To<float>().FromInstance(999999f);
        }
    }
}
