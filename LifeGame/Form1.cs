using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LifeGame
{
    public partial class Form1 : Form
    {
        private Graphics graphics;
        private int resolution;
        private bool[,] field;
        private int rows;
        private int cols;

        private int countGenerations = 0;

        public Form1()
        {
            InitializeComponent();
        }
      
        // Function that fill Array filed, turn of buutoms and inputs while timer turn on 
        public void StartGame()
        {
            if (timer1.Enabled)
            {
                return;
            }

            countGenerations = 0;

            Text = $"Generation: {countGenerations}";

            nudResolution.Enabled = false;
            nudDensity.Enabled = false;

            resolution = (int)nudResolution.Value;

            // game fields array
            rows = pictureBox1.Height / resolution;
            cols = pictureBox1.Width / resolution;
            field = new bool[cols, rows];

            Random random = new Random();
            for (int x = 0; x < cols; x++)
            {
                for (int y = 0; y < rows; y++)
                {
                    field[x, y] = random.Next((int)nudDensity.Value) == 0;
                }
            }

            pictureBox1.Image = new Bitmap(pictureBox1.Width, pictureBox1.Height);
            graphics = Graphics.FromImage(pictureBox1.Image);

            timer1.Start();
        }

        //function daw rectangles from array field
        private void DrawGeneration()
        {
            graphics.Clear(Color.Black);

            bool[,] nextField = new bool[cols, rows];

            for (int x = 0; x < cols; x++)
            {
                for (int y = 0; y < rows; y++)
                {
                    int counter = countNeighbours(x, y);
                    bool hasLife = field[x, y];

                    if(!hasLife && counter == 3)
                    {
                        nextField[x, y] = true;
                    }
                    else if(hasLife && (counter < 2 || counter > 3))
                    {
                        nextField[x, y] = false;
                    }
                    else
                    {
                        nextField[x, y] = field[x, y];
                    }

                    if (hasLife)
                    {
                        graphics.FillRectangle(Brushes.Crimson, x * resolution, y * resolution, resolution, resolution);
                    }
                }
            }
                field = nextField;
                pictureBox1.Refresh();

            Text = $"Generation: {++countGenerations}";
        }

        private int countNeighbours(int x, int y)
        {
            int count = 0;

            for (int i = -1; i < 2; i++)
            {
                for (int j = -1; j < 2; j++)
                {
                    int col = (x + i + cols) % cols;
                    int row = (y + j + rows) % rows;

                    bool isSelf = col == x && row == y;
                    bool hasLife = field[col, row];

                    if (hasLife && !isSelf)
                    {
                        count++;
                    }
                }
            }
            return count;
        }

        //Stop game and turn on inputs
        private void StopGame()
        {
            if (!timer1.Enabled)
            {
                return;
            }
            else
            {
                timer1.Stop();
                nudDensity.Enabled = true;
                nudResolution.Enabled = true;
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            DrawGeneration();
        }

        private void start_Click(object sender, EventArgs e)
        {
            StartGame();
        }

        private void stop_Click(object sender, EventArgs e)
        {
            StopGame();
        }

        //Add or delete rectangle
        private void MouseAction(MouseEventArgs e, bool result)
        {
            int x = e.Location.X / resolution;
            int y = e.Location.Y / resolution;
            if (ValidMousePosition(x, y))
            {
                field[x, y] = result;
            }
        }

        // Validate out of bounds array
        private bool ValidMousePosition(int x, int y)
        {
            return x >= 0 && y >= 0 && x < cols && y < rows;
        }

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            if (!timer1.Enabled)
            {
                return;
            }

            if (e.Button == MouseButtons.Left)
            {
                MouseAction(e,true);
            }

            if (e.Button == MouseButtons.Right)
            {
                MouseAction(e,false);
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Text = "Life Game";
        }
    }
}
