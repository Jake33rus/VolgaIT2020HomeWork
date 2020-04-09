using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VolgaITHomeWorkULANOV
{
    public class Fire
    {
        int x, y, damage;

        public Fire(int x, int y, int damage)
        {
            this.x = x;
            this.y = y;
            this.damage = damage * 20;
        }

        public int X { get => x;}
        public int Y { get => y;}

        public int Damage { get => damage; }
    }
}
