using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EasyDI.UnitTest
{
    public interface iSpeed : IEasyDIDecore<iSpeed>
    {
        public float Speed { get; set; }
    }
}
