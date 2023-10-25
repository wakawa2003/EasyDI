using EasyDI.Demo;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EasyDI.Demo
{
    public class CharacterBehaviour : MonoBehaviour, IMoveable
    {
        [SerializeField][Inject] string filedString1;
        [SerializeField][Inject] string filedString2;
        [field: SerializeField][Inject] public float Speed { get; set; }

        [Inject]
        public void Move(Vector3 pos)
        {
        }

        private void Start()
        {
            Debug.Log($"Start character");
        }
    }
}
