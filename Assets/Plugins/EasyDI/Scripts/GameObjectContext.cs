using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace EasyDI
{
    [DefaultExecutionOrder(ExecutionOrderEasyDI.OrderGameObjectContext)]
    public class GameObjectContext : ContextBase
    {
        protected override void Awake()
        {
            base.Awake();

            foreach (Component t in GetComponents<Component>())
            {
                InjectFor(t);
            }
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
