using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EasyDI.UnitTest
{
    public class gunController : MonoBehaviour
    {
        [Inject] public iCharacter characterOwner;
        [Inject] public string stringField;
        [Inject] public classIsSingleton classIsSingleton;
    }
}
