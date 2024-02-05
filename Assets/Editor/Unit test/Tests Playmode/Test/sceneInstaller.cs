using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace EasyDI.UnitTest
{
    public class sceneInstaller : MonoInstaller
    {
        public static string stringInstallForTag2 = "scene instaler string tag2";
        public static string idSingleton;
        public static string stringTagForTagSingleton = "value for string singleton";

        public override void InstallBinding()
        {
            ContainerBinding.Bind<string>(tags.tag2).To<string>().CustomGetInstance((obj, mem) =>
            {
                return stringInstallForTag2;
            });

            ContainerBinding.Bind<string>(tags.tagSingleton).To<string>().CustomGetInstance((obj, mem) =>
            {
                return stringTagForTagSingleton;
            }).AsSingleton();

            ContainerBinding.Bind<ingameControllerTest>().To<ingameControllerTest>().CustomGetInstance((a, b) =>
            {
                return FindObjectOfType<ingameControllerTest>();
            }).AsSingleton();


            ContainerBinding.Bind<classIsSingleton>().To<classIsSingleton>().CustomGetInstance((a, b) =>
            {
                Debug.Log($"custom get instance in singleton!!");
                idSingleton = GUID.Generate().ToString();
                return new classIsSingleton(idSingleton);
            }).AsSingleton();

        }


    }
}
