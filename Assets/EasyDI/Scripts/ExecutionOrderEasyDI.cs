using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EasyDI
{
    public class ExecutionOrderEasyDI
    {
        public const int OrderProjectContext = -99999;
        public const int OrderSceneContext = OrderProjectContext + 1;
        public const int OrderGameObjectContext = OrderSceneContext + 1;
    }
}
