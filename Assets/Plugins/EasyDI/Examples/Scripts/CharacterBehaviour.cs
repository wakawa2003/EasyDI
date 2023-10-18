using EasyDI.Demo;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EasyDI.Demo
{
    public class CharacterBehaviour : MonoBehaviour, IMoveable
    {
        [field: SerializeField] public float Speed { get; set; }

        public void Move(Vector3 pos)
        {
        }

        private void Start()
        {
            Debug.Log($"Start character");
        }
    }
}
