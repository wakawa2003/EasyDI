using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EasyDI
{
    [System.AttributeUsage(
         System.AttributeTargets.Property |
        System.AttributeTargets.Method |
        System.AttributeTargets.Field)
]
    public class InjectAttribute : System.Attribute
    {
        public string tag;
        public InjectAttribute()
        {
        }
        public InjectAttribute(string tag)
        {
            this.tag = tag;
        }
    }
}
