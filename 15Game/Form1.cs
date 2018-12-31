using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace _15Game
{
    public partial class Form1 : Form
    {
        Button[] buttons;
        Point unVis;//location of empty spot
        Point[] locationsForWin = new Point[15];//help checking if the user won
        Button movingButton = null;
        Point saveLastLocationOfMoving; //Will store next unvis location
        int buttonSize = 100;
        public Form1()
        {
            InitializeComponent();
            createMenu();
            createButtonsForNewGame();
        }
        public void createMenu()
        {
            MenuStrip menuStrip = new MenuStrip();
            menuStrip.Location = new Point(0, 0);
            menuStrip.Name = "MenuStrip";
            ToolStripMenuItem newGame = new ToolStripMenuItem();
            menuStrip.Items.Add(newGame);
            newGame.Name = "newGame";
            newGame.Text = "New Game";
            newGame.Click += createNewGameButton;
            this.Controls.Add(menuStrip);

        }
        public void createButtonsForNewGame()
        {
            buttons = new Button[15];
            Random r = new Random();
            int top = 25, left = 0, p = 0;//p helps creating 4*4 table
            for (int i = 0; i < 15; i++, p++)
            {
                buttons[i] = new Button();
                buttons[i].Text = (i + 1).ToString();
                buttons[i].BackColor = Color.FromArgb(r.Next(20,250), r.Next(20,250), r.Next(20,250));
                buttons[i].Font = new Font("Arial", 12.0F, FontStyle.Regular);
                buttons[i].Click += buttonClick;
                if (p == 4)
                {
                    top += buttonSize;
                    left = 0;
                    p = 0;
                }
                buttons[i].Left = left;
                buttons[i].Top = top;
                buttons[i].Width = buttonSize;
                buttons[i].Height = buttonSize;
                left += buttons[i].Width;
                locationsForWin[i] = buttons[i].Location;
                this.Controls.Add(buttons[i]);
            }

            Randomize();
            unVis = new Point(left, top);

        }

        private void createNewGameButton(object sender, EventArgs e)
        {
            createNewGame();
        }

        void createNewGame()
        {
            foreach (Button b in buttons)
            {
                this.Controls.Remove(b);
                b.Dispose();
            }
            createButtonsForNewGame();

        }

        private void buttonClick(object sender, EventArgs e)
        {
            Button b = (sender as Button);
            movingButton = b;
            if (checkIfNeighborOfUnvis(b.Location))
            {

                saveLastLocationOfMoving = b.Location;
                lockAll(); //prevent clicking when button moves
                timer1.Start();

            }

        }
        private bool checkIfNeighborOfUnvis(Point clickedButtonLoc)
        {
            return unVis.X == clickedButtonLoc.X + buttonSize && unVis.Y == clickedButtonLoc.Y
                || unVis.X == clickedButtonLoc.X - buttonSize && unVis.Y == clickedButtonLoc.Y
                || unVis.X == clickedButtonLoc.X && unVis.Y == clickedButtonLoc.Y - buttonSize
                || unVis.X == clickedButtonLoc.X && unVis.Y == clickedButtonLoc.Y + buttonSize;
        }

        Boolean checkIfFinish()
        {
            for (int i = 0; i < 15; i++)
            {
                if (buttons[i].Location.X != locationsForWin[int.Parse(buttons[i].Text) - 1].X ||
                    buttons[i].Location.Y != locationsForWin[int.Parse(buttons[i].Text) - 1].Y)
                {
                    return false;
                }
            }
            return true;
        }
        Boolean checkIfGameOver()
        {
            int index = 1, locationOfIndex = 0;
            while (index < 14)
            {
                for (int i = 0; i < 15; i++)
                {
                    if (buttons[i].Text == index.ToString())
                    {
                        locationOfIndex = i;
                        break;
                    }

                }
                if (buttons[locationOfIndex].Location.X != locationsForWin[index - 1].X || buttons[locationOfIndex].Location.Y != locationsForWin[index - 1].Y)
                    return false;
                index++;
            }
            int ind14 = -1, ind15 = -1;
            for (int i = 0; i < 15; i++)
            {
                if (buttons[i].Text == "14")
                    ind14 = i;
                if (buttons[i].Text == "15")
                    ind15 = i;
            }
            if (buttons[ind14].Location.X == locationsForWin[14].X && buttons[ind14].Location.Y == locationsForWin[14].Y &&
                buttons[ind15].Location.X == locationsForWin[13].X && buttons[ind15].Location.Y == locationsForWin[13].Y)
                return true;
            return false;
        }
        void Randomize()
        {
            List<int> numbers = Enumerable.Range(1, 15).ToList();
            Random r = new Random();
            numbers.Sort((x, y) => r.Next(-1, 1));//shuffle
            for (int i = 0; i < 15; i++)
                buttons[i].Text = numbers[i] + "";
        }

        void lockAll()
        {
            for (int i = 0; i < 15; i++)
            {
                buttons[i].Click -= buttonClick;
            }
        }
        void freeAll()
        {
            for (int i = 0; i < 15; i++)
                buttons[i].Click += buttonClick;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (movingButton.Location.X == unVis.X && movingButton.Location.Y == unVis.Y)
            {
                timer1.Stop();
                unVis = saveLastLocationOfMoving;
                freeAll();
                if (checkIfFinish())
                {
                    offerNewGame("You Won! New game?");
                }
                if (checkIfGameOver())
                {
                    offerNewGame("No solution! New game?");
                }
                return;
            }

            moveButton();
        }
        private void moveButton()
        {
            if (movingButton.Location.X < unVis.X)
                movingButton.Location = new Point(movingButton.Location.X + 10, movingButton.Location.Y);
            else if (movingButton.Location.X > unVis.X)
                movingButton.Location = new Point(movingButton.Location.X - 10, movingButton.Location.Y);
            else if (movingButton.Location.Y < unVis.Y)
                movingButton.Location = new Point(movingButton.Location.X, movingButton.Location.Y + 10);
            else if (movingButton.Location.Y > unVis.Y)
                movingButton.Location = new Point(movingButton.Location.X, movingButton.Location.Y - 10);
        }
        private void offerNewGame(string message) {
            DialogResult dialogResult = MessageBox.Show(message, "Game Over!", MessageBoxButtons.YesNo);
            if (dialogResult == DialogResult.Yes)
            {
                createNewGame();
            }
            else if (dialogResult == DialogResult.No)
            {
                Application.Exit();
            }
        }
       

    }
}

    

