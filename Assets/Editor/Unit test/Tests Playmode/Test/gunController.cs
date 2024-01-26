using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EasyDI.UnitTest
{
    public class gunController : MonoBehaviour
    {
        [Inject] public ICharacter characterOwner;
        [Inject] public string stringField;
    }
}
