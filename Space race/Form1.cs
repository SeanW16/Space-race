using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Media;
//Sean Woods, Wed May 22th 2024, basic space race game
namespace Space_race
{
    public partial class Form1 : Form
    {
        //All shapes
        Rectangle player1 = new Rectangle(150, 575, 10, 20);
        Rectangle player2 = new Rectangle(450, 575, 10, 20);
        Rectangle bottomZone = new Rectangle(0, 550, 600, 3);
        
        //All lists
        List<Rectangle> ballList = new List<Rectangle>();
        List<int> ballSpeeds = new List<int>();
        
        Random randGen = new Random();

        //All sound payers
        SoundPlayer explosion = new SoundPlayer(Properties.Resources.crashSound);
        SoundPlayer point = new SoundPlayer(Properties.Resources.pointSound);
        SoundPlayer win = new SoundPlayer(Properties.Resources.winSound);

        //All ints
        int randValue = 0;
        int ballSize = 10;
        int player1Score = 0;
        int player2Score = 0;
        int playerSpeed1 = 5;
        int playerSpeed2 = 5;
        
        //All bools
        bool wPressed = false;
        bool sPressed = false;
        bool upPressed = false;
        bool downPressed = false;

        //All brushs
        SolidBrush redBrush = new SolidBrush(Color.Red);
        SolidBrush whiteBrush = new SolidBrush(Color.White);
        SolidBrush blackBrush = new SolidBrush(Color.Black);
     
        public Form1()
        {
            InitializeComponent();
        }

        private void gameTimer_Tick(object sender, EventArgs e)
        {
            //move balls down the screen
            for (int i = 0; i < ballList.Count(); i++)
            {
                //get new position of y
                int x = ballList[i].X + ballSpeeds[i];

                //update the ball object
                ballList[i] = new Rectangle(x, ballList[i].Y, ballSize, ballSize);
            }

            randValue = randGen.Next(1, 100);

            //spawn balls on left side
            if (randValue < 20)
            {
                randValue = randGen.Next(30, this.Height);
                Rectangle ball = new Rectangle(0, randValue, ballSize, ballSize);
                ballList.Add(ball);
                ballSpeeds.Add(randGen.Next(4, 8));
            } 
            
            //spawn balls on right side                  
            if (randValue < 40)
            {
               randValue = randGen.Next(30, this.Height);
               Rectangle ball = new Rectangle(600, randValue, ballSize, ballSize);
               ballList.Add(ball);
               ballSpeeds.Add(randGen.Next(-8, -4));
            } 
            
            //remove ball from list if it has gone off the screen
            for (int i = 0; i < ballList.Count(); i++)
            {
                if (ballList[i].X > 600)
                {
                    ballList.RemoveAt(i);
                    ballSpeeds.RemoveAt(i);
                }
            }
            for (int i = 0; i < ballList.Count(); i++)
            {
                if (ballList[i].Y > 540)
                {
                    ballList.RemoveAt(i);
                    ballSpeeds.RemoveAt(i);
                }
            }

            //check for collision between ball and player
            for (int i = 0; i < ballList.Count(); i++)
            {
                if (ballList[i].IntersectsWith(player1))
                {
                    player1.Y = 575;
                    ballList.RemoveAt(i);
                    ballSpeeds.RemoveAt(i);
                    SoundPlayer player = new SoundPlayer(Properties.Resources.crashSound);
                    explosion.Play();
                }
                if (ballList[i].IntersectsWith(player2))
                {
                    player2.Y = 575;
                    ballList.RemoveAt(i);
                    ballSpeeds.RemoveAt(i);
                    explosion.Play();
                }
            }

            //move player 1
            if (wPressed == true && player1.Y < 590)
            {
                player1.Y -= playerSpeed1;
                if (player1.Y == 0)
                {
                    player1.Y = 575;
                }
            }
            if (sPressed == true && player1.Y < this.Height - player1.Height)
            {
                player1.Y += playerSpeed1;
            }

            //move player 2
            if (upPressed == true && player2.Y < 590)
            {
                player2.Y -= playerSpeed2;
                if (player2.Y == 0)
                {
                    player2.Y = 575;
                }
            }
            if (downPressed == true && player1.Y < this.Height - player2.Height)
            {
                player2.Y += playerSpeed2;
            }

            //check to see if player hits the top
            if (player1.Y == 5)
            {
                player1.Y = 575;
                player1Score++;
                player1ScoreLabel.Text = $"{player1Score}";
                point.Play();
            }
            if (player2.Y == 5)
            {
                player2.Y = 575;
                player2Score++;
                player2ScoreLabel.Text = $"{player2Score}";
                point.Play();
            }

            //check to see who wins
            if (player1Score == 3)
            {
                winLabel.Text = "Player 1 Wins";
                win.Play();
                gameTimer.Stop();
            }
            if (player2Score == 3)
            {
                winLabel.Text = "Player 2 Wins";
                win.Play();
                gameTimer.Stop();
            }
            Refresh();
        }

        private void Form1_KeyUp(object sender, KeyEventArgs e)
        {
            //Key controls 
            switch (e.KeyCode)
            {
                case Keys.W:
                    wPressed = false;
                    break;
                case Keys.S:
                    sPressed = false;
                    break;
                case Keys.Up:
                    upPressed = false;
                    break;
                case Keys.Down:
                    downPressed = false;
                    break;
            }
        }
        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            //Key controls
            switch (e.KeyCode)
            {
                case Keys.W:
                    wPressed = true;
                    break;
                case Keys.S:
                    sPressed = true;
                    break;
                case Keys.Up:
                    upPressed = true;
                    break;
                case Keys.Down:
                    downPressed = true;
                    break;
                case Keys.Escape:
                    if (gameTimer.Enabled == false)
                    {
                        Application.Exit();
                    }
                    break;
                case Keys.Space:
                    if (gameTimer.Enabled == false)
                    {
                        InitializeGame();
                    }
                    break;
            }
        }
        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            if (gameTimer.Enabled == false && player1Score == 3 || player2Score == 3)
            {
                titleLabel.Text = "GAME OVER";
                subtitleLabel.Text = "Press Space to Replay or Esc to Exit";
                player1ScoreLabel.Text = "";
                player2ScoreLabel.Text = "";

                if (player1Score == 3)
                {
                    winLabel.Text = "Player 1 Wins";
                }
                if (player2Score == 3)
                {
                    winLabel.Text = "Player 2 Wins";
                }
            }
            else if (gameTimer.Enabled == true)
            {
                //update labels
                player1ScoreLabel.Text = $"Score: {player1Score}";
                player2ScoreLabel.Text = $"Score: {player2Score}";

                //draw shapes
                e.Graphics.FillRectangle(whiteBrush, player2);
                e.Graphics.FillRectangle(whiteBrush, player1);
                e.Graphics.FillRectangle(redBrush, bottomZone);

                for (int i = 0; i < ballList.Count(); i++)
                {
                    e.Graphics.FillEllipse(whiteBrush, ballList[i]);
                }
            }
            else
            {                
                player1ScoreLabel.Text = "";
                player2ScoreLabel.Text = "";
                titleLabel.Text = "SPACE RACE";
                winLabel.Text = "";
                subtitleLabel.Text = "Press Space to Start or Esc to Exit";
            }
        }
        public void InitializeGame()
        {
            //reset everything for start of game
            titleLabel.Text = "";
            subtitleLabel.Text = "";

            gameTimer.Enabled = true;

            player1Score = 0;
            player2Score = 0;
            winLabel.Text = "";

            ballList.Clear();
            ballSpeeds.Clear();

            player1 = new Rectangle(150, 575, 10, 20);
            player2 = new Rectangle(450, 575, 10, 20);            
        }
    }
}
