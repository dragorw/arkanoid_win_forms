using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Pong
{
    public enum State
    {
        EMPTY,
        PLATFORM_LEFT,
        PLATFORM_CENTER,
        PLATFORM_RIGHT,
        BLOCK_1,
        BLOCK_2,
        BLOCK_3,
        BLOCK_4,
        BLOCK_5,
        BLOCK_RIGHT,
        BALL,
    }

    public partial class Pong : Form
    {
        // mapa a jeji velikost
        const int mapWidth = 50;
        const int mapHeight = 50;
        public State[,] map = new State[mapHeight, mapWidth];

        // parametry pozice kulicky a platformy
        public int dirX = 0;
        public int dirY = 0;
        public int plaformX;
        public int plaformY;
        public int ballX;
        public int ballY;
        public int score;
        
        // uzivatelske rozhrani
        public Image spriteSet; 
        public Label scoreLabel;      
        public State[] blocks = { State.BLOCK_1, State.BLOCK_2,
                   State.BLOCK_3, State.BLOCK_4, State.BLOCK_5 };

        public Pong()
        {
            // umisteni prvku uzivatelskeho rozhrani a ovladacich prvku
            InitializeComponent();                      
            scoreLabel = new Label();
            scoreLabel.Location = new Point((mapWidth + 3) * 20, 50);
            scoreLabel.Text = "Score: " + score;            
            navodLabel.Location = new Point((mapWidth + 1) * 20, 550);            
            this.Controls.Add(scoreLabel);
            timer1.Tick += new EventHandler(update);
            this.KeyUp += new KeyEventHandler(inputCheck);
            startButton.Location = new Point((mapWidth + 3) * 20, 200);
            pauseButton.Location = new Point((mapWidth + 3) * 20, 250);
            exitButton.Location  = new Point((mapWidth + 3) * 20, 300);            

            Init();
        }                
        // realizace pohybu platformy pomoci tlacitek         
        private void inputCheck(object sender, KeyEventArgs e)
        {           
            map[plaformY, plaformX] = State.EMPTY;
            map[plaformY, plaformX + 1] = State.EMPTY;
            map[plaformY, plaformX + 2] = State.EMPTY;
            switch (e.KeyCode)
            {
                case Keys.D:
                    if (plaformX + 1 < mapWidth - 3)
                        plaformX++;
                    break;
                case Keys.A:
                    if (plaformX > 0)
                        plaformX--;
                    break;
            }
            map[plaformY, plaformX] = State.PLATFORM_LEFT;
            map[plaformY, plaformX + 1] = State.PLATFORM_CENTER;
            map[plaformY, plaformX + 2] = State.PLATFORM_RIGHT;
        }
        // metoda vytvoreni novych cihel
        public void AddLIne()
        {
            for (int i = mapHeight -2; i >0; i--)
            {
                for (int j = 0; j < mapWidth; j += 2)
                {
                    map[i, j] = map[i - 1, j];                    
                }
            }
            Random r = new Random();
            for (int i = 0; i < mapHeight / 3; i++)
            {
                for (int j = 0; j < mapWidth; j += 2)
                {
                    int currPlatform = r.Next(1, 5);
                    switch (currPlatform)
                    {
                        case 1:
                            map[i, j] = State.BLOCK_1;
                            break;
                        case 2:
                            map[i, j] = State.BLOCK_2;
                            break;
                        case 3:
                            map[i, j] = State.BLOCK_3;
                            break;
                        case 4:
                            map[i, j] = State.BLOCK_4;
                            break;
                        case 5:
                            map[i, j] = State.BLOCK_5;
                            break;
                    }
                    map[i, j + 1] = State.BLOCK_RIGHT;
                }
            }
        }
        // nahodile vytvoreni cihel 
        public void GeneratePlatforms()
        {
            Random r = new Random();
            for (int i = 0; i < mapHeight / 3; i++)
            {

                for (int j = 0; j < mapWidth; j += 2)
                {
                    int currPlatform = r.Next(1, 5);
                    switch (currPlatform)
                    {
                        case 1:
                            map[i, j] = State.BLOCK_1;
                            break;
                        case 2:
                            map[i, j] = State.BLOCK_2;
                            break;
                        case 3:
                            map[i, j] = State.BLOCK_3;
                            break;
                        case 4:
                            map[i, j] = State.BLOCK_4;
                            break;
                        case 5:
                            map[i, j] = State.BLOCK_5;
                            break;
                    }
                    map[i, j + 1] = State.BLOCK_RIGHT;
                }
            }
        }
        
        private void update(object sender, EventArgs e)
        {
            // pri padu mice pod platformu hra konci
            timer1.Stop();            
            if (ballY + dirY > mapHeight - 1)
            {
                MessageBox.Show("Konec hry \npro novou hru zmačknete start");
                return;
            }
            map[ballY, ballX] = 0;
            if (!IsCollide())
            ballX += dirX;
            if (!IsCollide())
            ballY += dirY;
            map[ballY, ballX] = State.BALL;

            map[plaformY, plaformX] = State.PLATFORM_LEFT;
            map[plaformY, plaformX + 1] = State.PLATFORM_CENTER;
            map[plaformY, plaformX + 2] = State.PLATFORM_RIGHT;

            Invalidate();
            timer1.Start();
        }        
        // metoda pohybu mice a kolize z cihlami a platformou 
        public bool IsCollide()
        {
            bool isColliding = false;
            if (ballX + dirX > mapWidth - 1 || ballX + dirX < 0)
            {
                dirX *= -1;
                isColliding = true;                           
            }
            if (ballY  + dirY < 0)
            {
                dirY *= -1;
                isColliding = true;
            }

            if (map[ballY + dirY, ballX] != State.EMPTY)
            {
                bool addScore = false;
                isColliding = true;

                if (map[ballY + dirY, ballX] == State.BLOCK_RIGHT)
                {
                    map[ballY + dirY, ballX] = State.EMPTY;
                    map[ballY + dirY, ballX - 1] = State.EMPTY;
                    addScore = true;
                }
                else if (blocks.Contains(map[ballY + dirY, ballX]))
                {
                    map[ballY + dirY, ballX] = State.EMPTY;
                    map[ballY + dirY, ballX + 1] = State.EMPTY;
                    addScore = true;
                }
                if (addScore)
                {
                    score += 50;
                    if (score % 2000 == 0 && score > 0)
                    {
                        AddLIne();
                    }
                }
                    dirY *= -1;            
            }
            if (map[ballY, ballX + dirX] != State.EMPTY)
            {
                bool addScore = false;
                isColliding = true;
                if (map[ballY, ballX + dirX] == State.BLOCK_RIGHT)
                {
                    map[ballY, ballX + dirX] = State.EMPTY;
                    map[ballY, ballX + dirX - 1] = State.EMPTY;
                    addScore = true;
                }
                else if (blocks.Contains(map[ballY, ballX + dirX]))
                {
                    map[ballY, ballX + dirX] = State.EMPTY;
                    map[ballY, ballX + dirX + 1] = State.EMPTY;
                    addScore = true;
                }
                if (addScore)
                {
                    score += 50;
                    if (score % 2000 == 0 && score > 0)
                    {
                        AddLIne();
                    }
                }
                dirX *= -1;
            }
            scoreLabel.Text = "Score: " + score;
            return isColliding;        
        }      
        // inicializace programu 
        public void Init()
        {            
            this.Width = (mapWidth + 10) * 20;
            this.Height = (mapHeight + 2) * 20;            
            spriteSet = Image.FromFile(Application.StartupPath + "\\image\\sprite.png");        

            timer1.Interval = 90;  
            score = 0;                        
            scoreLabel.Text = "Score: "+score;
            
            for (int i = 0; i < mapHeight; i++)
            {
                for (int j = 0; j < mapWidth; j++)
                {
                    map[i, j] = State.EMPTY;
                }
            }

            plaformX = (mapWidth - 1) / 2;
            plaformY = mapHeight - 1;

            map[plaformY, plaformX] = State.PLATFORM_LEFT;
            map[plaformY, plaformX + 1] = State.PLATFORM_CENTER;
            map[plaformY, plaformX + 2] = State.PLATFORM_RIGHT;

            ballY = plaformY - 1;
            ballX = plaformX + 1;
            map[ballY, ballX] = State.BALL;

            dirX = 1;
            dirY = -1;

            GeneratePlatforms();
            timer1.Start();
        }            
        // vykreslovani objektu a hraciho pole 
        private void OnPaint(object sender, PaintEventArgs e)
        {
            DrawMap(e.Graphics);
            DrawArea(e.Graphics);
        }
        // ohraniceni hraciho pole 
        public void DrawArea(Graphics g)
        {
            g.DrawRectangle(Pens.Black, new Rectangle(0, 0, mapWidth * 20, mapHeight * 20));
        }
        // vykreslovani platformy a cihel
        public void DrawMap(Graphics g)
        {
            for (int i = 0; i < mapHeight; i++)
            {
                for (int j = 0; j < mapWidth; j++)
                {
                    
                    if (map[i, j] == State.PLATFORM_LEFT)
                    {
                        g.DrawImage(spriteSet, new Rectangle(new Point(j * 20, i * 20),
                            new Size(75, 20)), 398, 17, 150, 50, GraphicsUnit.Pixel);
                    }
                    if (map[i, j] == State.BALL)
                    {
                        g.DrawImage(spriteSet, new Rectangle(new Point(j * 20, i * 20),
                            new Size(20, 20)), 806, 548, 73, 73, GraphicsUnit.Pixel);
                    }
                    if (map[i, j] == State.BLOCK_1)
                    {
                        g.DrawImage(spriteSet, new Rectangle(new Point(j * 20, i * 20),
                            new Size(40, 20)), 20, 16, 170, 59, GraphicsUnit.Pixel);
                    }
                    if (map[i, j] == State.BLOCK_2)
                    {
                        g.DrawImage(spriteSet, new Rectangle(new Point(j * 20, i * 20),
                            new Size(40, 20)), 20, 92, 170, 59, GraphicsUnit.Pixel);
                    }
                    if (map[i, j] == State.BLOCK_3)
                    {
                        g.DrawImage(spriteSet, new Rectangle(new Point(j * 20, i * 20),
                            new Size(40, 20)), 20, 162, 170, 59, GraphicsUnit.Pixel);
                    }
                    if (map[i, j] == State.BLOCK_4)
                    {
                        g.DrawImage(spriteSet, new Rectangle(new Point(j * 20, i * 20),
                            new Size(40, 20)), 20, 244, 170, 59, GraphicsUnit.Pixel);
                    }
                    if (map[i, j] == State.BLOCK_5)
                    {
                        g.DrawImage(spriteSet, new Rectangle(new Point(j * 20, i * 20),
                            new Size(40, 20)), 20, 318, 170, 59, GraphicsUnit.Pixel);
                    }
                }
            }
        }

        // ovladaci tlacitka
        private void startButton_Click(object sender, EventArgs e)
        {
            Init();
        }
        private void exitButton_Click(object sender, EventArgs e)
        {
            System.Environment.Exit(0);
        }
        private void pauseButton_Click(object sender, EventArgs e)
        {
            if (timer1.Enabled)
            {
                timer1.Stop();
            }
            else
            {
                timer1.Start();
            }

        }
    }

}
