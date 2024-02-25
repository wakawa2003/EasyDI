using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EasyDI.Demo
{
    public class DemoCharacterInstaller : MonoInstaller
    {
        public string installString = "character install value";
        public string installStringForTag1 = "character string tag1";
        public override void InstallBinding()
        {
            Debug.Log($"{gameObject.name} install binding");
            ContainerBinding.Bind<IMoveable>().To<CharacterBehaviour>().FromComponentInChild();
            ContainerBinding.Bind<Vector3>().To<Vector3>().FromInstance(new Vector3(8, 8, 8));
            ContainerBinding.Bind<string>().To<string>().FromInstance(installString);
            ContainerBinding.Bind<string>("tag1").To<string>().FromInstance(installStringForTag1);
            ContainerBinding.Bind<float>().To<float>().FromInstance(7777);
            //ContainerBinding.Bind<float>(isDecore: true).To<float>().FromInstance(7777);
        }
    }
}
