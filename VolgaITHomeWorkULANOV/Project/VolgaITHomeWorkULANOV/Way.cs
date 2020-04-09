using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VolgaITHomeWorkULANOV
{
    public class Way
    {
        int steps;
        int[,] road;
        int[] finishCoord;

        public Way(int steps, int[,] road, int[] finishCoord)
        {
            this.steps = steps;
            this.road = road;
            this.finishCoord = finishCoord;
        }

        public int[,] Road { get => road; }
        public int Steps { get => steps;}
        public int[] FinishCoord { get => finishCoord; }
    }
}
