using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VolgaITHomeWorkULANOV
{
    public class Player
    {
        public int X { get; set; }
        public int Y { get; set; }

        public int HP { get; set; }
        public int StepCount { get; set; }

        public string imgPath = @"Map/player.jpg";

        public List<string> keys;
        public List<int[]> assembledHealsCoord;
        public Player(int x, int y)
        {
            X = x;
            Y = y;
            HP = 100;
            StepCount = 0;
            keys = new List<string>();
            assembledHealsCoord = new List<int[]>();
        }
    }
}
