using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EasyDI.UnitTest
{
    public class projectInstaller : MonoInstaller
    {
        public static string stringDefault = "string from project context";

        public override void InstallBinding()
        {
            ContainerBinding.Bind<string>().To<string>().FromInstance(stringDefault);
        }


    }
}
