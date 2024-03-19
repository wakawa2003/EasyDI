using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EasyDI.UnitTest
{
    public interface iSpeed
    {
        [Inject] iSpeed iSpeedDecore { get; set; }
        public float Speed { get; set; }
    }
}
