using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EasyDI.UnitTest
{
    public class characterInstaller : MonoInstaller, ICharacter
    {
        public static string stringDefault = "string from character context";

        public override void InstallBinding()
        {
            ContainerBinding.Bind<string>().To<string>().FromInstance(stringDefault);
            ContainerBinding.Bind<string>(tags.tag1).To<string>().FromInstance(stringDefault);
            ContainerBinding.Bind<ICharacter>().To<characterController>().FromInstance(GetComponent<characterController>());
        }

    }


}
