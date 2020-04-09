using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace VolgaITHomeWorkULANOV
{
    static class Program
    {
        static string[][] ReadMap(string path, out int h, out int w)
        {
            StreamReader sr = new StreamReader(path);
            string line;
            string[][] map = null;
            int i = 0;
            h = 0; w = 0;
            while (!sr.EndOfStream)
            {
                line = sr.ReadLine();
                var info = line.Split(' ');
                if (i == 0)
                {

                    h = int.Parse(info[0]);
                    w = int.Parse(info[1]);
                    map = new string[h][];
                }
                else
                {
                    map[i - 1] = new string[w];
                    for (int j = 0; j < w; j++)
                        map[i - 1][j] = info[j];
                }
                i++;
            }
            sr.Close();
            return map;
        }
        /// <summary>
        /// Главная точка входа для приложения.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            if (args != null && args.Length > 0)
            {
                if (args[0].ToLower().Equals("-console"))
                {
                    string path = "mapFile.txt";
                    if (args.Length > 1)
                        path = args[1];
                    int h, w;
                    string[][] map = ReadMap(path, out h, out w);
                    int[] start = new int[2];
                    int[] finish = new int[2];
                    for (int i = 0; i < h; i++)
                        for (int j = 0; j < w; j++)
                        {
                            if (map[i][j].Equals("S"))
                            {
                                start[0] = i;
                                start[1] = j;
                            }
                            if (map[i][j].Equals("Q"))
                            {
                                finish[0] = i;
                                finish[1] = j;
                            }
                        }
                    Controller control = new Controller(map, new Player(start[0], start[1]), h, w);
                    List<string> road = control.GetFinalRoad(start, finish);
                    string result = $"{road.Count}\n";
                    foreach (var move in road)
                        result += move;
                    using (FileStream fstream = new FileStream("moves.txt", FileMode.OpenOrCreate))
                    {
                        byte[] array = System.Text.Encoding.Default.GetBytes(result);
                        fstream.Write(array, 0, array.Length);
                        Console.WriteLine("Ходы записаны в файл");
                    }
                }
                else
                {
                    try
                    {
                        Application.EnableVisualStyles();
                        Application.SetCompatibleTextRenderingDefault(false);
                        Application.Run(new Form1(args[0]));
                    }
                    catch
                    {
                        MessageBox.Show("Файл не найден, либо не подходит!", "Ошибка",
                            MessageBoxButtons.OK, 
                            MessageBoxIcon.Error, 
                            MessageBoxDefaultButton.Button1, 
                            MessageBoxOptions.DefaultDesktopOnly);
                    }
                }
            }
            else
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new Form1());
            }
        }
    }
}
