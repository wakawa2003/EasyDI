using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EasyDI.Demo
{
    public class DemoCharacter2Installer : MonoInstaller
    {
        public string installString = "character install value 2";
        public override void InstallBinding()
        {
            //Debug.Log($"character install binding");
            ContainerBinding.Bind<Transform>().To<Transform>().FromComponentInChild();
            //ContainerBinding.Bind<Vector3>().To<Vector3>().FromInstance(new Vector3(8, 8, 8));
            ContainerBinding.Bind<string>().To<string>().FromInstance(installString);
            ////ContainerBinding.Bind<string>("tag1").To<string>().FromInstance(installStringForTag1);
            //ContainerBinding.Bind<float>().To<float>().FromInstance(7777);
        }
    }
}
