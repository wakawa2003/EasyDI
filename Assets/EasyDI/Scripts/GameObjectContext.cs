using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace EasyDI
{
    [DefaultExecutionOrder(ExecutionOrderEasyDI.OrderGameObjectContext)]
    public class GameObjectContext : ContextBase
    {
        public bool IsAutoSearchInstallerInThisGameObject = true;
        [SerializeField] public List<MonoInstaller> MonoInstaller = new List<MonoInstaller>();

        protected override void Init()
        {
            if (!isInit)
            {

                //Debug.Log($"game object context awake!!");
                if (IsAutoSearchInstallerInThisGameObject)
                    if (MonoInstaller.FindAll(_ => _ != null).Count() == 0)
                        foreach (var installer in GetComponents<MonoInstaller>())
                        {
                            if (!MonoInstaller.Contains(installer))
                            {
                                MonoInstaller.Add(installer);
                            }
                        }

                InstallerList.AddRange(MonoInstaller);
                //Debug.Log($"{gameObject.name} INSTALLER LIST COUNT: {InstallerList.Count()}");
                base.Init();

                foreach (Component t in GetComponents<Component>())
                {
                    InjectFor(t);
                }
            }
        }

        protected virtual void Awake()
        {
            Init();
        }

        protected override ContextBase GetParentContext()
        {
            var l = GetComponentsInParent<ContextBase>().ToList();
            if (l.Contains(this))
                l.Remove(this);
            return l.FirstOrDefault() ?? ProjectContext.Ins.GetSceneContext(gameObject.scene.name);
        }
    }
}
