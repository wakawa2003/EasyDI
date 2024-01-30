using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace EasyDI
{
    [DefaultExecutionOrder(ExecutionOrderEasyDI.OrderProjectContext)]
    public class ProjectContext : ContextBase
    {

        #region Singleton
        private static ProjectContext ins;
        public static ProjectContext Ins
        {
            get
            {
                if (ins == null)
                {
                    var e = Resources.LoadAll<ProjectContext>("");
                    if (e.Count() > 0)
                    {
                        var s = Instantiate(e[0].gameObject).GetComponent<ProjectContext>();
                        s.gameObject.name = "Project Context";
                        s?.Awake();
                    }
                    else
                    {
                        var a = new GameObject("Project Context").AddComponent<ProjectContext>();
                        a?.Awake();

                    }
                }
                return ins;
            }
            set => ins = value;
        }
        #endregion
        public bool IsAutoSearchInstallerInThisGameObject = true;
        [SerializeField] public List<MonoInstaller> MonoInstaller = new List<MonoInstaller>();
        Dictionary<string, ContextBase> dictSceneContext = new Dictionary<string, ContextBase>();

        protected void Awake()
        {
            #region Singleton
            if (ins == null)
                ins = this;
            else
            {
                if (ins != this)
                    Destroy(gameObject);
                return;
            }

            #endregion
            Debug.Log($"project context awake!!");
            Init();
            DontDestroyOnLoad(this);

        }

        protected override void Init()
        {
            if (!isInit)
            {
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

        public ContextBase GetSceneContext(string nameScene)
        {
            if (dictSceneContext.ContainsKey(nameScene))
                return dictSceneContext[nameScene];
            EasyDILog.LogError($"Can't found Scene Context in Scene: {nameScene}!");
            return null;
        }

        public void AddSceneContext(string nameScene, ContextBase contextBase)
        {
            if (!dictSceneContext.ContainsKey(nameScene))
                dictSceneContext.Add(nameScene, contextBase);
            else
            {
                EasyDILog.LogError($"Exist more than 1 instance Scene Context in Scene: {nameScene}!");

            }
        }
        public void RemoveSceneContext(string nameScene)
        {
            if (dictSceneContext.ContainsKey(nameScene))
                dictSceneContext.Remove(nameScene);
            //else
            //{
            //    EasyDILog.LogError($"Can't found Scene Context in Scene: {nameScene}!");

            //}
        }

        protected override ContextBase GetParentContext()
        {
            return null;
        }

    }
}
