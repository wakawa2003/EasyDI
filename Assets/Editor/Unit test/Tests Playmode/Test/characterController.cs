using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EasyDI.UnitTest
{
    public class characterController : MonoBehaviour, ICharacter
    {
        [Inject] public string StringProperties1 { get; set; }
        [Inject] public string stringField1;
        [Inject] public classIsSingleton classIsSingleton;
        [Inject(tags.tag1)] public string stringFieldTag1;
        [Inject(tags.tag2)] public string stringFieldTag2;
        [Inject(tags.tagSingleton)] public string stringFieldSingletonHasTag;

        public string stringInMethod;
        public int intInmethod;

        [Inject]
        void Method(string param1)
        {
            this.stringInMethod = param1;
        }

        [Inject]
        void Method2(string param1, int intInmethod)
        {
            this.stringInMethod = param1;
            this.intInmethod = intInmethod;
        }

    }

    public interface ICharacter { }
}
