using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EasyDI.UnitTest
{
    public interface iHealth : IEasyDIDecore<iHealth>
    {
        public int _health { get; set; }
        int Health { get => (Decore != null ? Decore.Health : 0) + _health; set => _health = value; }
        public int _maxHealth { get; set; }
        int MaxHealth { get => (Decore != null ? Decore.MaxHealth : 0) + _maxHealth; set => _maxHealth = value; }

        public class Temp : iHealth
        {
            [Inject] public iHealth Decore { get; set; }
            public iHealth PrevDecore { get; set; }
            public int _health { get; set; } = 10;
            public int _maxHealth { get; set; }
        }
    }
}
