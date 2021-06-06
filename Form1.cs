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

namespace gameProject
{
    public partial class Form1 : Form
    {
        public Bitmap PlayerShip = Resource.PlayerShip;
        public List<Bullet> listBullets = new List<Bullet>();
        private static Point PlayerLocation = new Point(400, 580);
        private int _score = 0;

        public List<Point> enemyPositions = new List<Point>();
        public List<Enemy> enemies = new List<Enemy>();

        public static int level = 1;
        

        //public Boss Boss = new Boss(new Point(350, 100));
        public List<Bullet> BossBullet = new List<Bullet>();
        public bool IsPlaying = false;
        public bool IsPause = false;
        public int lives = 5;
        private int k = 1;

        public List<Boss> listBoss = new List<Boss>();

        public bool IsAlive = true;

        public Form1()
        {
            InitializeComponent();            
            CreateEnemies();            
            this.KeyDown += new KeyEventHandler(input);
            timer1.Start();
        }

        private void input(object sender, KeyEventArgs e)
        {
            switch(e.KeyCode)
            {
                case Keys.Right:
                    if (PlayerLocation.X < 880 && !IsPause)
                        PlayerLocation.X+=20;
                    break;
                case Keys.Left:
                    if (PlayerLocation.X-1 > 0 && !IsPause)
                        PlayerLocation.X-=20;
                    break;
                case Keys.Space: 
                    if(!IsPause)
                        listBullets.Add(new Bullet(PlayerLocation,Resource.PixelBullet));
                    break;
                case Keys.Escape:
                    if (IsPause) IsPause = false;
                    else IsPause = true;
                    break;
            }
        }
        

        private void timer1_Tick(object sender, EventArgs e)
        {
            if(lives == 0)
            {
                IsPlaying = false;
                label1.Visible = true;
                label1.Text = "Конец игры. \n" + labelScore.Text;
            }

            if(!IsPause)
            { 
                foreach(var _bullet in listBullets)
                {
                    if (_bullet.IsAlive && _bullet.BulletLocation.Y > -100)
                    {
                        _bullet.BulletLocation.Y -= 20;
                    }
                }

                foreach(var _boss in listBoss)
                {
                    if (_boss.IsAlive)
                    {
                        if (_boss.BossLocation.X < 100) k = 1;
                        if (_boss.BossLocation.X > 800) k = -1;
                        _boss.BossLocation.X += 10 * k;
                    }
                }

                foreach(var _bulletEnemy in BossBullet)
                {
                    if(_bulletEnemy.IsAlive)
                        _bulletEnemy.BulletLocation.Y += 20;
                    if (_bulletEnemy.BulletLocation.Y > 900) _bulletEnemy.IsAlive = false;
                }
            }

            

            bool flag= false;
            foreach (var _boss in listBoss)
            {
                if (_boss.IsAlive)
                {
                    foreach (var _bullet in BossBullet)
                    {
                        if (IsAlive && IsColide(PlayerLocation, _bullet.BulletLocation, new Size(110, 110)))
                        {
                            _bullet.IsAlive = false;
                            BossBullet.Remove(_bullet);
                            lives--;
                            break;
                        }
                    }

                    foreach (var _bullet in listBullets)
                    {
                        if (_boss.IsAlive && IsColide(_boss.BossLocation, _bullet.BulletLocation, _boss.Size))
                        {
                            _bullet.IsAlive = false;
                            listBullets.Remove(_bullet);
                            if (_boss.Lives == 0)
                            {
                                _boss.IsAlive = false;
                                flag = true;
                                AddScore(1000);
                            }
                            else
                                _boss.Lives--;
                            break;
                        }
                    }
                }
                if(flag) listBoss.Remove(_boss);
                break;
            }
            

            foreach (var enemy in enemies)
            {
                foreach (var _bullet in listBullets)
                {
                    if (enemy.IsAlive && IsColide(enemy.EnemyLocation, _bullet.BulletLocation, enemy.Size))
                    {
                        _bullet.IsAlive = false;
                        listBullets.Remove(_bullet);
                        enemy.IsAlive = false;
                        AddScore(100);
                        break;
                    }                    
                }
                
            }            
            Refresh();
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            int levelBots = level % 3;
            if (IsPlaying)
            {                
                g.DrawImage(PlayerShip, new Rectangle(PlayerLocation.X, PlayerLocation.Y, 65, 95));

                foreach (var _bullet in listBullets)
                {
                    if (_bullet.IsAlive && _bullet.BulletLocation.Y > -10)
                        g.DrawImage(_bullet.Image, new Rectangle(_bullet.BulletLocation, new Size(20, 20)));
                }
                var deadCount = 0;

                foreach (var enemy in enemies)
                {
                    if (enemy.IsAlive)
                        g.DrawImage(enemy.Image, new Rectangle(enemy.EnemyLocation, new Size(70, 90)));
                    else
                        deadCount++;
                }

                for (int i = 1; i <= lives; i++)
                    g.DrawImage(Resource.Heart, new Rectangle(new Point(50 * i, 50), new Size(50,50)));

                foreach(var _boss in listBoss)
                {
                    _boss.IsAlive = true;
                    g.DrawImage(_boss.Image, new Rectangle(_boss.BossLocation, new Size(100, 100)));
                    foreach (var _bullet in BossBullet)
                    {
                        if (_bullet.IsAlive)
                            g.DrawImage(_bullet.Image, new Rectangle(_bullet.BulletLocation, new Size(20, 20)));
                    }
                }

                Random r = new Random();

                if (levelBots == 0)
                {
                    listBoss.Add(new Boss(new Point(r.Next(200, 700), 100)));
                    level++;
                }

                if (deadCount == 5)
                {
                    CreateEnemies();
                    deadCount = 0;
                    level++;
                    timer2.Interval = (int)(3100 * (1 - 0.3 * (levelBots-1)));
                }
            }
        }

        public void CreateEnemies()
        {
            enemies.Clear();
            enemyPositions.Clear();
            Random r = new Random();
            enemyPositions.Add(new Point(r.Next(0, 250), r.Next(0, 130)));
            enemyPositions.Add(new Point(r.Next(260, 550), r.Next(0, 130)));
            enemyPositions.Add(new Point(r.Next(550,850), r.Next(0, 130)));
            enemyPositions.Add(new Point(r.Next(100, 400), r.Next(140, 270)));
            enemyPositions.Add(new Point(r.Next(420, 700), r.Next(140, 270)));
            foreach (var tempPoint in enemyPositions)
                enemies.Add(new Enemy(tempPoint));
            
        }

        private void AddScore(int score)
        {
            _score += score;
            labelScore.Text = "Score: " + _score.ToString();
        }

        private bool IsColide(Point enemyPoint, Point bulletPoint, Size SizeEnemy)
        {
            if (enemyPoint.X < bulletPoint.X && enemyPoint.X + SizeEnemy.Width > bulletPoint.X
                && enemyPoint.Y-60  < bulletPoint.Y && enemyPoint.Y+SizeEnemy.Height > bulletPoint.Y)
                return true;
            return false;

        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            
            if (!IsPause)
            {
                foreach (var enemy in enemies)
                {
                    enemy.EnemyLocation.Y += (20);
                    if (enemy.EnemyLocation.Y > 700 && enemy.IsAlive)
                    {
                        enemy.IsAlive = false;
                        lives--;
                    }
                }               
                
            }

            for (int i=0;i<listBullets.Count;i++)
            {
                if (listBullets[i].BulletLocation.Y == -10)
                    listBullets.RemoveAt(i);
            }

        }

        private void button1_Click(object sender, EventArgs e)
        {
            IsPlaying = true;            
            this.Controls.Remove(sender as Button);
            labelScore.Visible = true;
        }

        private void timer3_Tick(object sender, EventArgs e)
        {
            foreach(var _boss in listBoss)
                BossBullet.Add(new Bullet(new Point(_boss.BossLocation.X + 50, _boss.BossLocation.Y + 55), Resource.EnemyBullet));
        }
    }

    public class Bullet
    {
        public Bitmap Image;
        public Point BulletLocation = new Point();
        public bool IsAlive = true;
        public Bullet(Point PlayerLocation, Bitmap ImageBull)
        {
            BulletLocation = new Point(PlayerLocation.X+23,PlayerLocation.Y);
            Image = ImageBull;
        }
    }
    public class Enemy
    {
        public Bitmap Image = Resource.EnemyShip;
        public Point EnemyLocation = new Point();
        public bool IsAlive = true;
        public Size Size = new Size(70, 90);
        public Enemy(Point Location)
        {
            EnemyLocation = Location;            
        }
    }
    public class Boss
    {
        public Bitmap Image = Resource.BossShip;
        public Point BossLocation = new Point();
        public bool IsAlive = true;
        public int Lives = 10;
        public Size Size = new Size(100, 110);
        public Boss(Point Location)
        {
            BossLocation = Location;
            IsAlive = true;
        }
    }
}
