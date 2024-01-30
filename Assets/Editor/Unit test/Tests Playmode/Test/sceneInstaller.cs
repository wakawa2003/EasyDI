using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EasyDI.UnitTest
{
    public class sceneInstaller : MonoInstaller
    {
        public static string stringInstallForTag2 = "scene instaler string tag2";

        public override void InstallBinding()
        {
            ContainerBinding.Bind<string>("tag2").To<string>().CustomGetInstance((obj, mem) =>
            {
                return stringInstallForTag2;
            });
        }


    }
}
