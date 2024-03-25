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
            ContainerBinding.Bind<string>(tags.tagStringMethod1).To<string>().FromInstance(stringDefault);
            ContainerBinding.Bind<string>(tags.tagStringMethod2).To<string>().FromInstance(stringDefault);
            ContainerBinding.Bind<int>(tags.tagStringMethod2).To<int>().FromInstance(characterInstaller.intInMethod);
            ContainerBinding.Bind<ICharacter>().To<characterController>().FromInstance(GetComponent<characterController>());

            var buff1 = new buffSpeed();
            var buff2 = new buffSpeed();
            var buff3 = new buffSpeed2();

            Debug.Log($"buff1: {buff1.GetHashCode()}");
            Debug.Log($"buff2: {buff2.GetHashCode()}");
            Debug.Log($"buff3: {buff3.GetHashCode()}");
            ContainerBinding.Bind<iSpeed>().To<buffSpeed>().FromInstance(buff1).AsTransient();
            ContainerBinding.Decore<iSpeed>().To<buffSpeed>().FromInstance(buff2).AsTransient();
            ContainerBinding.Decore<iSpeed>().To<buffSpeed2>().FromInstance(buff3).AsTransient();
            ContainerBinding.Decore<iSpeed>().To<buffSpeed2>().FromInstance(new buffSpeed2()).AsTransient();
            ContainerBinding.Decore<iSpeed>().To<buffSpeed2>().FromInstance(new buffSpeed2()).AsTransient();


        }

    }

    class buffSpeed : iSpeed
    {
        [Inject] public iSpeed iSpeedDecore { get; set; }
        public float Speed { get => iSpeedDecore.Speed + 11; set { } }
    }
    class buffSpeed2 : iSpeed
    {
        [Inject] public iSpeed iSpeedDecore { get; set; }
        public float Speed { get => iSpeedDecore.Speed + 4; set { } }
    }
}
