using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal;
using UnityEngine;

namespace EasyDI.UnitTest
{
    public class characterInstaller : MonoInstaller
    {
        public static string stringDefault = "string from character context";
        public static int intInMethod = 6663;
        public static float buffSpeedValue1 = 5;
        public static float buffSpeedValue2 = 7;

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
            ContainerBinding.Decore<iSpeed>().To<buffSpeed2>().FromInstance(new buffSpeed2()).AsTransient();
            //ContainerBinding.Decore<iSpeed>().To<buffSpeed>().FromInstance(new buffSpeed()).AsTransient();
            //ContainerBinding.Decore<iSpeed>().To<buffSpeed2>().FromInstance(buff3).AsTransient();
            //ContainerBinding.Decore<iSpeed>().To<buffSpeed2>().FromInstance(new buffSpeed2()).AsTransient();
            //ContainerBinding.Decore<iSpeed>().To<buffSpeedEND>().FromInstance(new buffSpeedEND()).AsTransient();
            //lam tiep case decore tren hirarchy

        }

    }

    public class buffSpeed : iSpeed
    {
        [Inject] public iSpeed iSpeedDecore { get; set; }
        public float Speed { get => iSpeedDecore == null ? characterInstaller.buffSpeedValue1 : iSpeedDecore.Speed + characterInstaller.buffSpeedValue1; set { } }
    }
    public class buffSpeed2 : iSpeed
    {
        [Inject] public iSpeed iSpeedDecore { get; set; }
        public float Speed { get => iSpeedDecore == null ? characterInstaller.buffSpeedValue2 : iSpeedDecore.Speed + characterInstaller.buffSpeedValue2; set { } }
    }

}
