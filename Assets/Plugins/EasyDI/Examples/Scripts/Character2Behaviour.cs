using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EasyDI
{
    public class Character2Behaviour : MonoInstaller
    {

        [Inject] public string stringInjected;
        [Inject] public Transform transformInjected;

        public override void InstallBinding()
        {
        }


    }
}
