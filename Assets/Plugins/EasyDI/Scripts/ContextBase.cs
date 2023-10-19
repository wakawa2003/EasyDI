using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EasyDI
{
    public class ContextBase : MonoBehaviour
    {
        Container container = new Container();
        [SerializeField] List<MonoInstaller> monoInstallerList = new List<MonoInstaller>();

        public List<MonoInstaller> MonoInstallerList { get => monoInstallerList; private set => monoInstallerList = value; }
        public Container Container { get => container; }
        ContextBase contextParent;

        protected virtual void Awake()
        {
            container = new Container();
            GetAllBindInforFromInstallerList();
        }

        private void GetAllBindInforFromInstallerList()
        {
            foreach (var monoInstaller in monoInstallerList)
            {
                foreach (var item in monoInstaller.Container.DictTypeAndData)
                {
                    container.AddTypeAndInfor(item.Key, item.Value);
                }

            }
        }

        public void InjectFor(object obj, Type type, Dictionary<Type, BindInfor> inforFromChildContext)
        {
            if (contextParent != null)
            {
                //b1: tong hop infor condition tu child va chinh no
                //b2: goi => contextParent.InjectFor(obj, type, inforThisAndChild);


            }
            else
            {
                //inject cho obj ngoai tru nhung infor child
            }
        }
    }
}
