using EasyDI.Demo;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EasyDI
{
    public class WeaponInstaller : MonoInstaller
    {
        protected override void InstallBinding()
        {
         
            ContainerBinding.Bind<string>("tag1").To<string>().FromInstance("Weapon for tag1");
            ContainerBinding.Bind<float>().To<float>().FromInstance(999999f);
        }
    }
}
