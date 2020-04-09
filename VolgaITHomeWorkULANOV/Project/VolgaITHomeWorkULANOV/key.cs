using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VolgaITHomeWorkULANOV
{
    public class Key
    {
        int x, y, symbol;
        public int X { get => x;}
        public int Y { get => y;  }
        public int Symbol { get => symbol;}
        public Key(int x, int y, int symbol)
        {
            this.x = x;
            this.y = y;
            this.symbol = symbol;
        }
    }
}
