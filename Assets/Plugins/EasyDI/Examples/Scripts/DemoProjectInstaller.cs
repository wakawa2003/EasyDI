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
            Debug.Log($"install binding");
            Container.Bind<IMoveable>().To<CharacterBehaviour>().FromComponentInChild();
            Container.Bind<Vector3>().To<Vector3>().FromComponentInChild();

        }
    }
}
