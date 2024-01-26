using EasyDI.Demo;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EasyDI.Demo
{
    public class CharacterBehaviour : MonoBehaviour, IMoveable
    {
        [SerializeField][Inject] string filedString1;
        [SerializeField][Inject("tag1")] string filedString2;
        [field: SerializeField][Inject] public float Speed { get; set; }
        [Inject] public Transform transformInjected;
        [Inject] public Transform transformInjected2;
        [Inject] public Transform transformInjected3;
        [Inject] public Transform transformInjected4;
        [Inject] public Transform transformInjected5;
        [Inject] public Transform transformInjected6;
        [Inject] public Transform transformInjected7;
        [Inject] public Transform transformInjected8;
        [Inject] public Transform transformInjected9;
        [Inject] public Transform transformInjected110;
        [Inject] public Transform transformInjected11;
        [Inject] public Transform transformInjected12;
        [Inject] public Transform transformInjected13;
        [Inject] public Transform transformInjected14;
        [Inject] public Transform transformInjected15;
        [Inject] public Transform transformInjected16;
        [Inject] public Transform transformInjected17;
        [Inject] public Transform transformInjected18;
        [Inject] public Transform transformInjected19;
        [Inject] public Transform transformInjected20;
        [Inject] public Transform transformInjected21;
        [Inject] public Transform transformInjected22;
        [Inject] public Transform transformInjected23;
        [Inject] public Transform transformInjected24;
        [Inject] public Transform transformInjected25;
        [Inject] public Transform transformInjected26;
        [Inject] public Transform transformInjected27;
        [Inject] public Transform transformInjected28;
        [Inject] public Transform transformInjected29;
        [Inject] public Transform transformInjected30;
        [Inject] public Transform transformInjected31;
        [Inject] public Transform transformInjected32;
        [Inject] public Transform transformInjected33;
        [Inject] public Transform transformInjected34;
        [Inject] public Transform transformInjected35;
        [Inject] public Transform transformInjected36;
        [Inject] public Transform transformInjected37;
        [Inject] public Transform transformInjected38;
        [Inject] public Transform transformInjected39;
        [Inject] public Transform transformInjected40;
        [Inject] public Transform transformInjected41;
        [Inject] public Transform transformInjected42;
        [Inject] public Transform transformInjected43;

        [Inject]
        public void Move(Vector3 pos)
        {
        }

        private void Start()
        {
            //Debug.Log($"Start character");
        }
    }
}
