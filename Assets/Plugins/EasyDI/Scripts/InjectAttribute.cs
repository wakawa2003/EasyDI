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
        string tag;
        public string Tag => tag;

        public InjectAttribute()
        {
        }
        public InjectAttribute(string tag)
        {
            this.tag = tag;
        }

        public override string ToString()
        {
            return $"attribute tag: {tag}";
        }
    }
}
