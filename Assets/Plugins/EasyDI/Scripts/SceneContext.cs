using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace EasyDI
{
    [DefaultExecutionOrder(ExecutionOrderEasyDI.OrderSceneContext)]
    public class SceneContext : ContextBase
    {
        public bool IsAutoSearchInstallerInThisGameObject = true;
        [SerializeField] public List<MonoInstaller> MonoInstaller = new List<MonoInstaller>();

        protected void Awake()
        {
            Debug.Log($" {gameObject.scene.name} scene context awake!!");
            Init();
        }

        protected override void Init()
        {
            if (!isInit)
            {
                ProjectContext.Ins.AddSceneContext(gameObject.scene.name, this);
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
            }
            base.Init();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            ProjectContext.Ins.RemoveSceneContext(gameObject.scene.name);

        }

        protected override ContextBase GetParentContext()
        {
            return ProjectContext.Ins;

        }


    }
}
