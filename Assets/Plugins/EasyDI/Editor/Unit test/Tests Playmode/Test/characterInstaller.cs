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
            ContainerBinding.Bind<iCharacter>().To<characterController>().FromInstance(GetComponent<characterController>());


            ContainerBinding.Bind<iSpeed>().To<buffSpeed>().CustomGetInstance((a, b) => new buffSpeed());
            ContainerBinding.Decore<iSpeed>().To<buffSpeed>().CustomGetInstance((a, b) => new buffSpeed());
            ContainerBinding.Decore<iSpeed>().To<buffSpeed2>().CustomGetInstance((a, b) => new buffSpeed2());
            //ContainerBinding.Decore<iSpeed>().To<buffSpeed>().FromInstance(new buffSpeed()).AsTransient();
            //ContainerBinding.Decore<iSpeed>().To<buffSpeed2>().FromInstance(buff3).AsTransient();
            //ContainerBinding.Decore<iSpeed>().To<buffSpeed2>().FromInstance(new buffSpeed2()).AsTransient();
            //ContainerBinding.Decore<iSpeed>().To<buffSpeedEND>().FromInstance(new buffSpeedEND()).AsTransient();
            //lam tiep case decore tren hirarchy

        }

    }

    public class buffSpeed : iSpeed
    {
        [Inject] public iSpeed Decore { get; set; }
        public iSpeed PrevDecore { get; set; }
        public float Speed { get => Decore == null ? characterInstaller.buffSpeedValue1 : Decore.Speed + characterInstaller.buffSpeedValue1; set { } }

    }
    public class buffSpeed2 : iSpeed
    {
        public iSpeed PrevDecore { get; set; }
        [Inject] public iSpeed Decore { get; set; }
        public float Speed { get => Decore == null ? characterInstaller.buffSpeedValue2 : Decore.Speed + characterInstaller.buffSpeedValue2; set { } }

        public void GetAnimationClips(List<AnimationClip> results)
        {
            throw new System.NotImplementedException();
        }
    }

}
