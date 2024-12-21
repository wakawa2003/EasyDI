using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace EasyDI.UnitTest
{
    public class characterController : MonoBehaviour, iCharacter, iSpeed, iHealth
    {
        [Inject] public string StringProperties1 { get; set; }

        public float Speed { get => (this as iSpeed).Decore == null ? 0 : (this as iSpeed).Decore.Speed; set { } }

        [Inject] iSpeed IEasyDIDecore<iSpeed>.Decore { get; set; }
        iSpeed IEasyDIDecore<iSpeed>.PrevDecore { get; set; }

        [Inject] public iHealth Decore { get; set; }
        public iHealth PrevDecore { get; set; }
        public int _health { get; set; }
        public int _maxHealth { get; set; }

        [Inject] public classIsSingleton classIsSingleton;
        [Inject(tags.tag1)] public string stringFieldTag1;
        [Inject(tags.tag2)] public string stringFieldTag2;
        [Inject(tags.tagSingleton)] public string stringFieldSingletonHasTag;

        public string stringInMethod;
        public int intInmethod;

        [Inject(tags.tagStringMethod1)]
        void Method(string param1)
        {
            this.stringInMethod = param1;
        }

        [Inject(tags.tagStringMethod2)]
        void Method2(string param1, int intInmethod)
        {
            this.stringInMethod = param1;
            this.intInmethod = intInmethod;
        }

    }

}
