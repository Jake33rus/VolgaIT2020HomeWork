using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace VolgaITHomeWorkULANOV
{
    public class Controller
    {
        string[][] map;
        List<Way> ways;
        Player player;
        int h, w;
        List<string> finalRoad;
        string keyReg = @"[a-e]";
        string doorReg = @"[A-E]";
        string fireReg = @"[1-5]";

        public Controller(string[][] map, Player player, int h, int w)
        {
            this.map = map;
            this.player = player;
            this.h = h;
            this.w = w;
            ways = new List<Way>();
            finalRoad = new List<string>();
        }

        public List<string> GetFinalRoad(int[] start, int[] finish)
        {
            SearchRoad(start, finish);
            ways.Reverse();
            foreach(var way in ways)
            {
                int playerX = way.FinishCoord[0], playerY = way.FinishCoord[1];
                int stepNum = way.Steps;
                int[,] map = way.Road;
                while (stepNum > 0)
                {
                    stepNum--;
                    if (playerX + 1 < h && map[playerX + 1, playerY] == stepNum)
                    {
                        playerX += 1;
                        finalRoad.Add("U");
                    }
                    else if (playerY + 1 < w && map[playerX, playerY + 1] == stepNum)
                    {
                        playerY += 1;
                        finalRoad.Add("L");
                    }
                    else if (playerX - 1 >= 0 && map[playerX - 1, playerY] == stepNum)
                    {
                        playerX -= 1;
                        finalRoad.Add("D");
                    }
                    else if (playerY - 1 >= 0 && map[playerX, playerY - 1] == stepNum)
                    {
                        playerY -= 1;
                        finalRoad.Add("R");
                    }
                }
            }
            finalRoad.Reverse();
            return finalRoad;
        }
        private int DamageOnRoad(int[,] tempWay, int step, int[] coord)
        {
            int playerX = coord[0], playerY = coord[1];
            int stepNum = step;
            int[,] way = tempWay;
            int HP = 100;
            while (stepNum > 0)
            {
                stepNum--;
                if (playerX + 1 < h && way[playerX + 1, playerY] == stepNum)
                {
                    playerX += 1;
                }
                else if (playerY + 1 < w && way[playerX, playerY + 1] == stepNum)
                {
                    playerY += 1;
                }
                else if (playerX - 1 >= 0 && way[playerX - 1, playerY] == stepNum)
                {
                    playerX -= 1;
                }
                else if (playerY - 1 >= 0 && way[playerX, playerY - 1] == stepNum)
                {
                    playerY -= 1;
                }
                if (Regex.IsMatch(map[playerX][playerY], fireReg))
                    HP -= int.Parse(map[playerX][playerY]) * 20;
                if (map[playerX][playerY].Equals("H"))
                    HP = 100;
            }
            return HP;
        }

        private bool SearchRoad(int[] start, int[] finish)
        {
            int[,] tempWay = new int[h, w];
            bool add = true, flag;
            int x, y, step = 0;
            int maxStep = h * w;
            List<Key> keys = new List<Key>();
            List<Fire> fires = new List<Fire>();
            for (x = 0; x < h; x++)
            {
                for (y = 0; y < w; y++)
                {
                    if (Regex.IsMatch(map[x][y], doorReg))
                    {
                        flag = true;
                        foreach (var key in player.keys)
                        {
                            string uKey = key.ToUpper();
                            if (map[x][y].Equals(uKey))
                            {
                                tempWay[x, y] = -1;
                                map[x][y] = ".";
                                flag = false;
                                break;
                            }
                        }
                        if (flag)
                            tempWay[x, y] = -2;
                    }
                    else if (map[x][y].Equals("X") || map[x][y].Equals("5"))
                        tempWay[x, y] = -2;
                    else if (Regex.IsMatch(map[x][y], keyReg))
                    {
                        int symbol = 0 - (int)map[x][y][0];
                        tempWay[x, y] = symbol;
                        keys.Add(new Key(x, y, symbol));
                    }
                    else if (map[x][y].Equals("H"))
                    {
                        tempWay[x, y] = -2000;
                        foreach (var heal in player.assembledHealsCoord)
                            if (x == heal[0] && y == heal[1])
                                tempWay[x, y] = -1;
                    }
                    else if (Regex.IsMatch(map[x][y], fireReg))
                    {
                        int damage = int.Parse(map[x][y]);
                        if (player.HP <= damage*20)
                            tempWay[x, y] = -2;
                        else
                        {
                            fires.Add(new Fire(x, y, damage));
                            tempWay[x, y] = -1000;
                        }
                    }
                    else
                        tempWay[x, y] = -1;
                }
            }
            tempWay[start[0], start[1]] = 0;
            int fireDamage = 0;
            while (add)
            {
                for (x = 0; x < h; x++)
                    for (y = 0; y < w; y++)
                    {
                        if (tempWay[x, y] == step)
                        {
                            if (y - 1 >= 0 && tempWay[x - 1, y] != -2 && tempWay[x - 1, y] <= -1 )
                            {
                                if (tempWay[x - 1, y] == -1000)
                                {
                                    var fire = fires.FirstOrDefault(f => f.X == x - 1 && f.Y == y);
                                    if(fire != null)
                                    {
                                        fireDamage = fire.Damage;
                                        player.HP = player.HP - fire.Damage;
                                        tempWay[x - 1, y] = step + 1;
                                    }
                                }
                                else
                                    tempWay[x - 1, y] = step + 1;
                            }
                            if (x - 1 >= 0 && tempWay[x, y - 1] != -2 && tempWay[x, y - 1] <= -1 )
                            {
                                if (tempWay[x, y - 1] == -1000)
                                {
                                    var fire = fires.FirstOrDefault(f => f.X == x && f.Y == y - 1);
                                    if (fire != null)
                                    {
                                        fireDamage = fire.Damage;
                                        player.HP = player.HP - fire.Damage;
                                        tempWay[x, y - 1] = step + 1;
                                    }
                                }
                                else 
                                    tempWay[x, y - 1] = step + 1;
                            }
                            if (y + 1 < w && tempWay[x + 1, y] != -2 && tempWay[x + 1, y] <= -1 )
                            {
                                if (tempWay[x + 1, y] == -1000)
                                {
                                    var fire = fires.FirstOrDefault(f => f.X == x + 1 && f.Y == y);
                                    if (fire != null)
                                    {
                                        fireDamage = fire.Damage;
                                        player.HP = player.HP - fire.Damage;
                                        tempWay[x + 1, y] = step + 1;
                                    }
                                }
                                else 
                                    tempWay[x + 1, y] = step + 1;
                            }
                            if (x + 1 < h && tempWay[x, y + 1] != -2 && tempWay[x, y + 1] <= -1)
                            {
                                if (tempWay[x, y + 1] == -1000)
                                {
                                    var fire = fires.FirstOrDefault(f => f.X == x && f.Y == y + 1);
                                    if (fire != null)
                                    {
                                        fireDamage = fire.Damage;
                                        player.HP = player.HP - fire.Damage;
                                        tempWay[x, y + 1] = step + 1;
                                    }
                                }
                                else 
                                    tempWay[x, y + 1] = step + 1;
                            }
                        }
                    }
                step++;
                if (tempWay[finish[0], finish[1]] != -1)
                {
                    ways.Add(new Way(step, tempWay, finish));
                    return false;
                }
                else if (player.HP < 100)
                {
                    for (int i = 0; i < h; i++)
                        for (int j = 0; j < w; j++)
                            if (map[i][j].Equals("H") && tempWay[i, j] != -2000)
                            {
                                player.HP = 100;
                                player.assembledHealsCoord.Add(new int[2] { i, j });
                                map[i][j] = ".";
                                ways.Add(new Way(step, tempWay, new int[2] { i, j }));
                                SearchRoad(new int[2] { i, j }, finish);
                                return false;
                            }
                }
                if (keys != null)
                {
                    foreach (var key in keys)
                    {
                        if (tempWay[key.X, key.Y] != key.Symbol)
                        {
                            int symbol = -1 * key.Symbol;
                            player.keys.Add(((char)symbol).ToString());
                            ways.Add(new Way(step, tempWay, new int[2] { key.X, key.Y }));
                            map[key.X][key.Y] = ".";
                            player.HP = DamageOnRoad(tempWay, step, new int[2] { key.X, key.Y });
                            SearchRoad(new int[] { key.X, key.Y }, finish);
                            return false;
                        }
                    }
                }
                if (step >= maxStep)
                {
                    MessageBox.Show("Выход не найден!", "Информация", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                    Environment.Exit(0);
                }

            }
            return false;
        }


    }
}
