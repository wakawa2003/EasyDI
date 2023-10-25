using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EasyDI.Demo
{
    public class DemoCharacterInstaller : MonoInstaller
    {
        public string installString = "character install value";
        protected override void InstallBinding()
        {
            Debug.Log($"character install binding");
            Container.Bind<IMoveable>().To<CharacterBehaviour>().FromComponentInChild();
            Container.Bind<Vector3>().To<Vector3>().FromInstance(new Vector3(8,8,8));
            Container.Bind<string>().To<string>().FromInstance(installString);
            Container.Bind<float>().To<float>().FromInstance(7777);
        }
    }
}
