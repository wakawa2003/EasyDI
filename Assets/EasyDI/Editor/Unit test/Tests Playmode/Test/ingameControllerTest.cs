using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EasyDI.UnitTest
{
    public class classIsSingleton

    {
        public string id;

        private classIsSingleton()
        {
        }

        public classIsSingleton(string id)
        {
            this.id = id;
        }

    }
}
