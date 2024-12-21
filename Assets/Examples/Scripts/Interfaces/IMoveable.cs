using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EasyDI.Demo
{
    public interface IMoveable
    {
        float Speed { get; set; }
        void Move(Vector3 pos);
    }
}
