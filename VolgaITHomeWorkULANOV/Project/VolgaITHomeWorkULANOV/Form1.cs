using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using MessageBox = System.Windows.Forms.MessageBox;
using MessageBoxOptions = System.Windows.Forms.MessageBoxOptions;

namespace VolgaITHomeWorkULANOV
{
    public partial class Form1 : Form
    {
        Dictionary<String, String> mapImage;
        string currDir = @"Map\";
        string[][] map;
        int[,] way;
        Player player;
        int h, w, weight, height, delta, step = 0, hp = 100;
        Label lStep, lHp;
        List<List<PictureBox>> pictureBoxes = new List<List<PictureBox>>();
        string keyReg = @"[a-e]";
        string doorReg = @"[A-E]";
        string fireReg = @"[1-5]";
        List<string> road;
        string mapPath;
        public Form1(string map = "mapFile.txt")
        {
            mapPath = map;
            InitializeComponent();
        }

        public void Start()
        {
            mapImage = new Dictionary<string, string>();
            mapImage.Add(".", "floor.jpg");
            mapImage.Add("X", "wall.jpg");
            mapImage.Add("S", "start.jpg");
            mapImage.Add("Q", "exit.jpg");
            mapImage.Add("door", "door.jpg");
            mapImage.Add("key", "key.jpg");
            mapImage.Add("fire", "fire.jpg");
            mapImage.Add("H", "heal.jpg");
            ReadMap();
        }
        public string GetImgPath(string symbol)
        {
            return currDir+mapImage[symbol];
        }

        public void ReadMap()
        {
            StreamReader sr = new StreamReader(mapPath);
            string line;
            int i = 0;
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
                else {
                    map[i - 1] = new string[w];
                    for (int j = 0; j < w; j++)
                    {
                        map[i - 1][j] = info[j];
                    } 
                }
                i++;  
            }
            sr.Close();
        }
        private void Form1_Shown(object sender, EventArgs e)
        {
            RoadVisualisation(road);
        }

        private void ChangeIcon(int x, int y)
        {
            if (Regex.IsMatch(map[x][y], keyReg) || Regex.IsMatch(map[x][y], doorReg))
                map[x][y] = ".";
        }
        private void RoadVisualisation(List<string> road)
        {
            ReadMap();
            Bitmap image = new Bitmap(player.imgPath);
            pictureBoxes[player.X][player.Y].Image = image;
            Refresh();
            step = 0;
            Thread.Sleep(100);
            int x = player.X, y = player.Y;
            foreach(var move in road)
            {
                if (move.Equals("R"))
                {
                    y += 1;
                    ChangeIcon(x, y);
                }
                else if(move.Equals("L"))
                {
                    y -= 1;
                    ChangeIcon(x, y);
                }
                else if(move.Equals("U"))
                {
                    x -= 1;
                    ChangeIcon(x, y);
                }
                else if(move.Equals("D"))
                {
                    x += 1;
                    ChangeIcon(x, y);
                }
                if (Regex.IsMatch(map[x][y], fireReg))
                {
                    hp -= int.Parse(map[x][y]) * 20;
                    lHp.Text = $"HP: {hp}";
                    if (int.Parse(lHp.Text.Replace("HP: ", String.Empty)) <= 0 )
                        MessageBox.Show("Персонаж погиб", "Информация", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);

                }
                if (map[x][y].Equals("H"))
                {
                    hp = 100;
                    lHp.Text = $"HP: {hp}";
                    map[x][y] = ".";
                }
                step++;
                lStep.Text = $"Step: {step}";
                DrawMap();
                pictureBoxes[x][y].Image = image;
                Refresh();
                Thread.Sleep(100);
            }
            MessageBox.Show("Персонаж нашел выход!", "Информация", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
        }

        private void DrawMap()
        {
            string symbol;
            Bitmap image;
            for (int i = 0; i < h; i++)
            {
                for (int j = 0; j < w; j++)
                {
                    symbol = map[i][j];
                    if (Regex.IsMatch(symbol, keyReg))
                    {
                        image = new Bitmap(GetImgPath("key"));
                        pictureBoxes[i][j].Image = image;
                        Graphics g = Graphics.FromImage(pictureBoxes[i][j].Image);
                        g.DrawString(symbol, new Font("Arial", 250), Brushes.Red, 2, 2);
                    }
                    else if (Regex.IsMatch(symbol, doorReg))
                    {
                        image = new Bitmap(GetImgPath("door"));
                        pictureBoxes[i][j].Image = image;
                        Graphics g = Graphics.FromImage(pictureBoxes[i][j].Image);
                        g.DrawString(symbol, new Font("Arial", 200), Brushes.Red, 2, 2);
                    }
                    else if (Regex.IsMatch(symbol, fireReg))
                    {
                        image = new Bitmap(GetImgPath("fire"));
                        pictureBoxes[i][j].Image = image;
                    }
                    else
                    {
                        image = new Bitmap(GetImgPath(map[i][j]));
                        pictureBoxes[i][j].Image = image;
                    }
                }
            }
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            Start();
            Size resolution = System.Windows.Forms.Screen.PrimaryScreen.Bounds.Size;
            delta = (int)(resolution.Height / w / 1.25);
            height = h * delta + 50;
            weight = w * delta;
            PictureBox header = new PictureBox();
            lHp = new Label();
            lStep = new Label();
            lHp.Location = new Point(weight / 2 - weight / 4, 25);
            lStep.Location = new Point(weight / 2 + weight / 4, 25);
            lHp.Text = $"HP: {hp}";
            lStep.Text = $"Step: {step}";
            lHp.BackColor = Color.DarkGray;
            lStep.BackColor = Color.DarkGray;
            lHp.Font = new Font(lHp.Font, lHp.Font.Style | FontStyle.Bold);
            lStep.Font = new Font(lStep.Font, lStep.Font.Style | FontStyle.Bold);
            Controls.Add(lHp);
            Controls.Add(lStep);
            header.Location = new Point(0, 0);
            header.Size = new Size(weight, 50);
            header.BackColor = Color.DarkGray;
            Controls.Add(header);
            Size = new Size(weight + 15, height + 38);
            for (int x = 50; x < height; x += delta)
            {
                List<PictureBox> boxes = new List<PictureBox>();
                for (int y = 0; y < weight; y += delta)
                {
                    PictureBox pictureBox = new PictureBox();
                    pictureBox.Location = new Point(y, x);
                    pictureBox.SizeMode = PictureBoxSizeMode.StretchImage;
                    pictureBox.Size = new Size(delta, delta);
                    Controls.Add(pictureBox);
                    boxes.Add(pictureBox);
                }
                pictureBoxes.Add(boxes);
            }
            DrawMap();
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
            player = new Player(start[0], start[1]);
            Controller controler = new Controller(map, player, h, w);
            road = controler.GetFinalRoad(start, finish);
            Console.WriteLine();
            foreach(var move in road)
                Console.WriteLine(move);
        }
    }
}
