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
            Container.Bind<IMoveable>().To<CharacterBehaviour>().FromComponentInChild();
            
        }
    }
}
