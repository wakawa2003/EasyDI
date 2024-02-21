using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EasyDI.UnitTest
{
    public class characterInstaller : MonoInstaller, ICharacter
    {
        public static string stringDefault = "string from character context";
        public static int intInMethod = 6663;

        public override void InstallBinding()
        {
            ContainerBinding.Bind<string>().To<string>().FromInstance(stringDefault);
            ContainerBinding.Bind<int>().To<int>().FromInstance(intInMethod);
            ContainerBinding.Bind<string>(tags.tag1).To<string>().FromInstance(stringDefault);
            ContainerBinding.Bind<ICharacter>().To<characterController>().FromInstance(GetComponent<characterController>());
        }

    }


}
