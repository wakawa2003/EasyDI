using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace EasyDI
{
    public class Character2Behaviour : MonoInstaller
    {

        [Inject] public string stringInjected;

     
        public override void InstallBinding()
        {
        }


    }
}
