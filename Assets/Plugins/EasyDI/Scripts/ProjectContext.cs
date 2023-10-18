using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace EasyDI
{
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

        private void Awake()
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
        }


    }
}
