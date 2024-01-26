using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EasyDI.UnitTest
{
    public class characterController : MonoBehaviour, ICharacter
    {
        [Inject] public string StringProperties1 { get; set; }
        [Inject] public string stringField1;
        [Inject("tag1")] public string stringFieldTag1;
        public string stringInMethod;

        [Inject]
        void Method(string param1)
        {
            this.stringInMethod = param1;
        }

    }

    public interface ICharacter { }
}
