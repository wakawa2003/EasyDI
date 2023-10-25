using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace EasyDI
{
    [DefaultExecutionOrder(ExecutionOrderEasyDI.OrderSceneContext)]
    public class SceneContext : ContextBase
    {
        protected override void Awake()
        {
            ProjectContext.Ins.AddSceneContext(gameObject.scene.name, this);
            base.Awake();
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
