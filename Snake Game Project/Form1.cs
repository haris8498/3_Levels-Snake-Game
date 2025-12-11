using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Snake_Game_Project
{
    public partial class Form1 : Form
    {
        private List<Circle> Snake = new List<Circle>();
        private Circle food = new Circle();
     private Random rnd = new Random();
        private int animationCounter = 0;
        private List<ComputerSnake> computerSnakes = new List<ComputerSnake>();
      private int numberOfComputerSnakes = 0;
        private GameMode currentGameMode = GameMode.Easy;
        private List<Obstacle> obstacles = new List<Obstacle>();
     private int currentLevel = 1;
     private int levelStartScore = 0;
        private const int SCORE_PER_LEVEL = 1000;
        private const int MAX_LEVELS = 30;
        private bool isCountingDown = false;
        private int countdownValue = 3;
     private Timer countdownTimer;
        private bool hasBoundaries = true;

        // Level info labels
        private Label lblLevelInfo;
        private Panel pnlProgressBar;
      private int lastDisplayedScore = -1;
        private Label lblDifficultyInfo;

      public Form1()
   {
            InitializeComponent();

     this.KeyPreview = true;
      this.FormBorderStyle = FormBorderStyle.Sizable;
            this.WindowState = FormWindowState.Normal;
        this.Resize += Form1_Resize;
   this.Load += Form1_Load;

        new Settings();
  gameTimer.Interval = 1000 / Settings.Speed;
    gameTimer.Tick += updateScreen;

   countdownTimer = new Timer();
            countdownTimer.Interval = 1000;
countdownTimer.Tick += CountdownTimer_Tick;

         // Create difficulty info label
 lblDifficultyInfo = new Label();
            lblDifficultyInfo.AutoSize = true;
            lblDifficultyInfo.Font = new Font("Segoe UI", 14, FontStyle.Bold);
       lblDifficultyInfo.ForeColor = Color.FromArgb(255, 211, 42);
            lblDifficultyInfo.BackColor = Color.Transparent;
            lblDifficultyInfo.Visible = true;
 this.Controls.Add(lblDifficultyInfo);
   lblDifficultyInfo.BringToFront();

            // Create level info label
            lblLevelInfo = new Label();
         lblLevelInfo.AutoSize = true;
 lblLevelInfo.Font = new Font("Segoe UI", 11, FontStyle.Bold);
            lblLevelInfo.ForeColor = Color.FromArgb(255, 211, 42);
            lblLevelInfo.BackColor = Color.Transparent;
  lblLevelInfo.Visible = false;
            this.Controls.Add(lblLevelInfo);
   lblLevelInfo.BringToFront();

    // Create progress bar panel
   pnlProgressBar = new Panel();
      pnlProgressBar.Size = new Size(200, 15);
      pnlProgressBar.BackColor = Color.Transparent;
     pnlProgressBar.Visible = false;
            typeof(Panel).InvokeMember("DoubleBuffered",
   System.Reflection.BindingFlags.SetProperty | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic,
          null, pnlProgressBar, new object[] { true });
  pnlProgressBar.Paint += PnlProgressBar_Paint;
      this.Controls.Add(pnlProgressBar);
      pnlProgressBar.BringToFront();

            ShowSetupDialog();
            StartCountdown();
 }

      private void Form1_Resize(object sender, EventArgs e)
 {
            RepositionControls();
        }

      private void RepositionControls()
 {
       if (pbCanvas == null) return;

   int margin = 20;
   int topMargin = 80;

  pbCanvas.Location = new Point(margin, topMargin);
            pbCanvas.Size = new Size(
             this.ClientSize.Width - (margin * 2),
        this.ClientSize.Height - topMargin - margin
     );

      if (label1 != null)
  {
                label1.Location = new Point(margin, 10);
   }

        if (label2 != null)
    {
  label2.Location = new Point(margin, label1.Bottom + 5);
        }

        if (lblDifficultyInfo != null)
          {
    lblDifficultyInfo.Location = new Point(margin, label2.Bottom + 5);
            }

   if (lblLevelInfo != null)
{
             lblLevelInfo.Location = new Point(pbCanvas.Right - 320, label1.Top);
       }

     if (pnlProgressBar != null)
            {
        pnlProgressBar.Location = new Point(pbCanvas.Right - 210, label1.Bottom + 5);
     }

            if (label3 != null)
            {
             label3.Location = new Point(
    pbCanvas.Left + (pbCanvas.Width - label3.Width) / 2,
  pbCanvas.Top + (pbCanvas.Height - label3.Height) / 2
             );
}

            pbCanvas.Invalidate();
        }

 private void PnlProgressBar_Paint(object sender, PaintEventArgs e)
  {
        Graphics g = e.Graphics;
         g.SmoothingMode = SmoothingMode.AntiAlias;

      int currentLevelScore = Settings.Score - levelStartScore;
            int barWidth = pnlProgressBar.Width;
        int barHeight = pnlProgressBar.Height;

  using (SolidBrush bgBrush = new SolidBrush(Color.FromArgb(40, 40, 40)))
      {
    g.FillRectangle(bgBrush, 0, 0, barWidth, barHeight);
            }

     float progress = (float)currentLevelScore / SCORE_PER_LEVEL;
    int progressWidth = (int)(barWidth * Math.Min(progress, 1.0f));

         if (progressWidth > 0)
            {
                using (LinearGradientBrush progressBrush = new LinearGradientBrush(
  new Rectangle(0, 0, progressWidth, barHeight),
   Color.FromArgb(76, 209, 55),
        Color.FromArgb(46, 179, 25),
 LinearGradientMode.Horizontal))
      {
            g.FillRectangle(progressBrush, 0, 0, progressWidth, barHeight);
             }
            }

    using (Pen borderPen = new Pen(Color.FromArgb(255, 211, 42), 2))
   {
      g.DrawRectangle(borderPen, 0, 0, barWidth - 1, barHeight - 1);
            }
  }

        private void UpdateLevelInfo()
        {
        int currentLevelScore = Settings.Score - levelStartScore;

            if (lastDisplayedScore != currentLevelScore)
     {
 lastDisplayedScore = currentLevelScore;
      lblLevelInfo.Text = $"Level: {currentLevel}/{MAX_LEVELS}  |  Score: {currentLevelScore}/{SCORE_PER_LEVEL}";
     pnlProgressBar.Invalidate();
    }
  }

      private void StartCountdown()
        {
            isCountingDown = true;
  countdownValue = 3;
            gameTimer.Stop();
            countdownTimer.Start();
 pbCanvas.Invalidate();
        }

    private void CountdownTimer_Tick(object sender, EventArgs e)
      {
        countdownValue--;

  if (countdownValue <= 0)
   {
                countdownTimer.Stop();
       isCountingDown = false;
         gameTimer.Start();
    startGame();
          }

            pbCanvas.Invalidate();
        }

        private void ShowSetupDialog()
        {
         using (GameSetupForm setupForm = new GameSetupForm())
            {
      if (setupForm.ShowDialog() == DialogResult.OK)
                {
             numberOfComputerSnakes = setupForm.NumberOfComputerSnakes;
      currentGameMode = setupForm.SelectedGameMode;
            ConfigureGameDifficulty();

   if (numberOfComputerSnakes > 0)
 {
            InitializeComputerSnakes(setupForm.ComputerSnakeNames);
     }
   }
    }
      }

   private void ConfigureGameDifficulty()
        {
    switch (currentGameMode)
   {
 case GameMode.Easy:
        hasBoundaries = false;
          Settings.Speed = 10;
     lblDifficultyInfo.Text = "Difficulty: EASY (No Walls!)";
     lblDifficultyInfo.ForeColor = Color.FromArgb(76, 209, 55);
           break;

         case GameMode.Normal:
          hasBoundaries = true;
            Settings.Speed = 13;
    GenerateObstacles(10);
  lblDifficultyInfo.Text = "Difficulty: NORMAL (Watch out for Rocks!)";
                lblDifficultyInfo.ForeColor = Color.FromArgb(255, 193, 7);
      break;

        case GameMode.Difficult:
  hasBoundaries = true;
  Settings.Speed = 16;
           GenerateObstacles(20);
      lblDifficultyInfo.Text = "Difficulty: DIFFICULT (The Cage!)";
   lblDifficultyInfo.ForeColor = Color.FromArgb(244, 67, 54);
       break;
        }

            gameTimer.Interval = 1000 / Settings.Speed;
        }

  private void GenerateObstacles(int obstacleCount)
        {
obstacles.Clear();
          int maxXPos = pbCanvas.Size.Width / Settings.Width;
            int maxYPos = pbCanvas.Size.Height / Settings.Height;

  for (int i = 0; i < obstacleCount; i++)
            {
    bool validPosition = false;
   Obstacle newObstacle = null;
       int attempts = 0;

     while (!validPosition && attempts < 100)
           {
    attempts++;
                int minSize = 2;
      int maxSize = currentGameMode == GameMode.Difficult ? 8 : 5;
int width = rnd.Next(minSize, maxSize + 1);
            int height = rnd.Next(minSize, maxSize + 1);
    int x = rnd.Next(5, maxXPos - width - 5);
    int y = rnd.Next(5, maxYPos - height - 5);

       newObstacle = new Obstacle(x, y, width, height);
     validPosition = true;

          for (int sx = 8; sx <= 12; sx++)
                  {
               for (int sy = 8; sy <= 12; sy++)
         {
   if (newObstacle.Contains(sx, sy))
     {
  validPosition = false;
       break;
        }
    }
       if (!validPosition) break;
   }

 if (validPosition && newObstacle.Contains(food.x, food.y))
     {
 validPosition = false;
          }

          if (validPosition)
     {
  foreach (var obstacle in obstacles)
            {
  if (ObstaclesOverlapWithMargin(newObstacle, obstacle, 2))
    {
     validPosition = false;
          break;
        }
     }
   }
       }

      if (validPosition && newObstacle != null)
    {
     obstacles.Add(newObstacle);
          }
}
        }

    private bool ObstaclesOverlapWithMargin(Obstacle obs1, Obstacle obs2, int margin)
        {
        return !(obs1.x + obs1.width + margin <= obs2.x ||
         obs2.x + obs2.width + margin <= obs1.x ||
        obs1.y + obs1.height + margin <= obs2.y ||
         obs2.y + obs2.height + margin <= obs1.y);
  }

private void InitializeComputerSnakes(string[] names)
        {
    computerSnakes.Clear();
    int maxXPos = pbCanvas.Size.Width / Settings.Width;
  int maxYPos = pbCanvas.Size.Height / Settings.Height;

          Color[] headColors1 = { Color.FromArgb(255, 87, 34), Color.FromArgb(103, 58, 183), Color.FromArgb(33, 150, 243) };
       Color[] headColors2 = { Color.FromArgb(230, 74, 25), Color.FromArgb(81, 45, 168), Color.FromArgb(21, 101, 192) };
 Color[] bodyColors1 = { Color.FromArgb(255, 138, 101), Color.FromArgb(179, 157, 219), Color.FromArgb(144, 202, 249) };
            Color[] bodyColors2 = { Color.FromArgb(255, 112, 67), Color.FromArgb(149, 117, 205), Color.FromArgb(100, 181, 246) };

     for (int i = 0; i < names.Length; i++)
        {
      int startX = rnd.Next(5, maxXPos - 5);
             int startY = rnd.Next(5, maxYPos - 5);

      ComputerSnake compSnake = new ComputerSnake(
        names[i], startX, startY,
    headColors1[i % headColors1.Length],
    headColors2[i % headColors2.Length],
           bodyColors1[i % bodyColors1.Length],
          bodyColors2[i % bodyColors2.Length]
   );

     computerSnakes.Add(compSnake);
       }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
     RepositionControls();
        }

        private void startGame()
  {
          label3.Visible = false;
 new Settings();
     ConfigureGameDifficulty();
    Snake.Clear();

      Circle head = new Circle { x = 10, y = 10 };
            Snake.Add(head);

     for (int i = 0; i < 10; i++)
            {
          Circle body = new Circle();
              Snake.Add(body);
            }

      food = new Circle { x = rnd.Next(2, pbCanvas.Size.Width / Settings.Width - 2), y = rnd.Next(2, pbCanvas.Size.Height / Settings.Height - 2) };

            if (numberOfComputerSnakes > 0)
            {
     string[] names = new string[numberOfComputerSnakes];
                for (int i = 0; i < computerSnakes.Count && i < numberOfComputerSnakes; i++)
                {
            names[i] = computerSnakes[i].Name;
      }
      for (int i = computerSnakes.Count; i < numberOfComputerSnakes; i++)
        {
    names[i] = "AI Snake " + (i + 1);
    }
 InitializeComputerSnakes(names);
      }

   currentLevel = 1;
    levelStartScore = 0;
            UpdateLevelInfo();
        }

        private void updateScreen(object sender, EventArgs e)
        {
       if (Settings.GameOver)
  {
              if (!label3.Visible)
      {
      label3.Visible = true;
             }
      return;
 }

         if (isCountingDown)
      {
  return;
   }

  int maxXPos = pbCanvas.Size.Width / Settings.Width;
  int maxYPos = pbCanvas.Size.Height / Settings.Height;

    foreach (var compSnake in computerSnakes)
        {
       if (compSnake.IsAlive)
              {
  compSnake.Move(maxXPos, maxYPos, food, computerSnakes, Snake);
     }
 }

            for (int i = Snake.Count - 1; i >= 0; i--)
    {
      if (i == 0)
     {
            switch (Settings.direction)
             {
         case Directions.Right:
       Snake[i].x++;
                break;
       case Directions.Left:
   Snake[i].x--;
   break;
       case Directions.Up:
      Snake[i].y--;
          break;
           case Directions.Down:
          Snake[i].y++;
         break;
       }

               if (hasBoundaries)
               {
  if (Snake[i].x < 0 || Snake[i].y < 0 || Snake[i].x >= maxXPos || Snake[i].y >= maxYPos)
        {
      Die();
            }
           }
           else
         {
         if (Snake[i].x < 0) Snake[i].x = maxXPos - 1;
            if (Snake[i].x >= maxXPos) Snake[i].x = 0;
          if (Snake[i].y < 0) Snake[i].y = maxYPos - 1;
      if (Snake[i].y >= maxYPos) Snake[i].y = 0;
               }

         foreach (var obstacle in obstacles)
         {
 if (obstacle.Contains(Snake[i].x, Snake[i].y))
    {
    Die();
  }
         }

           for (int j = 1; j < Snake.Count; j++)
          {
        if (Snake[i].x == Snake[j].x && Snake[i].y == Snake[j].y)
     {
    Die();
  }
     }

        foreach (var compSnake in computerSnakes)
        {
          if (compSnake.IsAlive)
       {
           foreach (var segment in compSnake.Body)
     {
       if (Snake[i].x == segment.x && Snake[i].y == segment.y)
 {
             Die();
     }
        }
            }
           }

                if (Snake[0].x == food.x && Snake[0].y == food.y)
      {
      Eat();
           }
        }
       else
       {
          Snake[i].x = Snake[i - 1].x;
      Snake[i].y = Snake[i - 1].y;
   }
}

    pbCanvas.Invalidate();
        }

    private void Eat()
   {
       Circle body = new Circle
         {
        x = Snake[Snake.Count - 1].x,
   y = Snake[Snake.Count - 1].y
            };

     Snake.Add(body);
  Settings.Score += Settings.Points;
            label2.Text = Settings.Score.ToString();
UpdateLevelInfo();

          int maxXPos = pbCanvas.Size.Width / Settings.Width;
  int maxYPos = pbCanvas.Size.Height / Settings.Height;
    bool validFoodPosition = false;
        int attempts = 0;

     while (!validFoodPosition && attempts < 100)
         {
           attempts++;
     food = new Circle { x = rnd.Next(2, maxXPos - 2), y = rnd.Next(2, maxYPos - 2) };
      validFoodPosition = true;

    foreach (var segment in Snake)
        {
        if (food.x == segment.x && food.y == segment.y)
        {
         validFoodPosition = false;
      break;
  }
    }

             if (validFoodPosition)
 {
        foreach (var compSnake in computerSnakes)
        {
                if (compSnake.IsAlive)
          {
  foreach (var segment in compSnake.Body)
               {
                if (food.x == segment.x && food.y == segment.y)
          {
     validFoodPosition = false;
    break;
          }
           }
        }
  if (!validFoodPosition) break;
     }
                }

      if (validFoodPosition)
 {
                foreach (var obstacle in obstacles)
        {
        if (obstacle.Contains(food.x, food.y))
          {
              validFoodPosition = false;
     break;
         }
                    }
                }
            }

            animationCounter = 0;
        }

        private void Die()
        {
    Settings.GameOver = true;
            label3.Text = "Game Over!\nPress Enter to Restart";
    label3.Location = new Point(
          pbCanvas.Left + (pbCanvas.Width - label3.Width) / 2,
            pbCanvas.Top + (pbCanvas.Height - label3.Height) / 2
            );
     label3.Visible = true;
        }

        private void updateGraphics(object sender, PaintEventArgs e)
        {
            Graphics canvas = e.Graphics;
    canvas.SmoothingMode = SmoothingMode.AntiAlias;

            if (isCountingDown)
     {
       string countdownText = countdownValue > 0 ? countdownValue.ToString() : "GO!";
        using (Font countdownFont = new Font("Segoe UI", 72, FontStyle.Bold))
                {
  SizeF textSize = canvas.MeasureString(countdownText, countdownFont);
   float x = (pbCanvas.Width - textSize.Width) / 2;
         float y = (pbCanvas.Height - textSize.Height) / 2;

  using (SolidBrush shadowBrush = new SolidBrush(Color.FromArgb(100, 0, 0, 0)))
    {
          canvas.DrawString(countdownText, countdownFont, shadowBrush, x + 5, y + 5);
    }

   using (SolidBrush textBrush = new SolidBrush(Color.FromArgb(255, 211, 42)))
               {
     canvas.DrawString(countdownText, countdownFont, textBrush, x, y);
            }
                }
      return;
      }

 // Draw obstacles
    foreach (var obstacle in obstacles)
       {
  for (int ox = 0; ox < obstacle.width; ox++)
                {
     for (int oy = 0; oy < obstacle.height; oy++)
         {
       int drawX = (obstacle.x + ox) * Settings.Width;
         int drawY = (obstacle.y + oy) * Settings.Height;

      using (LinearGradientBrush brush = new LinearGradientBrush(
   new Rectangle(drawX, drawY, Settings.Width, Settings.Height),
    Color.FromArgb(120, 120, 120),
        Color.FromArgb(80, 80, 80),
  LinearGradientMode.Vertical))
        {
      canvas.FillEllipse(brush, drawX, drawY, Settings.Width, Settings.Height);
              }

           using (Pen outlinePen = new Pen(Color.FromArgb(60, 60, 60), 2))
          {
         canvas.DrawEllipse(outlinePen, drawX, drawY, Settings.Width, Settings.Height);
            }
   }
        }
            }

        // Draw player snake
            for (int i = 0; i < Snake.Count; i++)
            {
          Brush snakeBrush;
    if (i == 0)
       {
      snakeBrush = new LinearGradientBrush(
        new Rectangle(Snake[i].x * Settings.Width, Snake[i].y * Settings.Height, Settings.Width, Settings.Height),
               Color.FromArgb(76, 209, 55),
    Color.FromArgb(56, 142, 60),
    LinearGradientMode.Vertical);
  }
     else
                {
        snakeBrush = new LinearGradientBrush(
            new Rectangle(Snake[i].x * Settings.Width, Snake[i].y * Settings.Height, Settings.Width, Settings.Height),
     Color.FromArgb(139, 195, 74),
              Color.FromArgb(104, 159, 56),
           LinearGradientMode.Vertical);
     }

         canvas.FillEllipse(snakeBrush, Snake[i].x * Settings.Width, Snake[i].y * Settings.Height, Settings.Width, Settings.Height);

         using (Pen outlinePen = new Pen(Color.FromArgb(27, 94, 32), 2))
       {
        canvas.DrawEllipse(outlinePen, Snake[i].x * Settings.Width, Snake[i].y * Settings.Height, Settings.Width, Settings.Height);
         }

                if (i == 0)
          {
         int eyeSize = Settings.Width / 4;
                  int eyeOffset = Settings.Width / 4;

      canvas.FillEllipse(Brushes.White, Snake[i].x * Settings.Width + eyeOffset, Snake[i].y * Settings.Height + eyeOffset / 2, eyeSize, eyeSize);
 canvas.FillEllipse(Brushes.White, Snake[i].x * Settings.Width + Settings.Width - eyeOffset - eyeSize, Snake[i].y * Settings.Height + eyeOffset / 2, eyeSize, eyeSize);
  canvas.FillEllipse(Brushes.Black, Snake[i].x * Settings.Width + eyeOffset + 1, Snake[i].y * Settings.Height + eyeOffset / 2 + 1, eyeSize - 2, eyeSize - 2);
        canvas.FillEllipse(Brushes.Black, Snake[i].x * Settings.Width + Settings.Width - eyeOffset - eyeSize + 1, Snake[i].y * Settings.Height + eyeOffset / 2 + 1, eyeSize - 2, eyeSize - 2);
         }

       snakeBrush.Dispose();
        }

     // Draw computer snakes
  foreach (var compSnake in computerSnakes)
      {
     if (!compSnake.IsAlive) continue;

          for (int i = 0; i < compSnake.Body.Count; i++)
        {
         Brush brush;
             if (i == 0)
 {
              brush = new LinearGradientBrush(
           new Rectangle(compSnake.Body[i].x * Settings.Width, compSnake.Body[i].y * Settings.Height, Settings.Width, Settings.Height),
     compSnake.HeadColor1,
      compSnake.HeadColor2,
              LinearGradientMode.Vertical);
        }
        else
         {
     brush = new LinearGradientBrush(
       new Rectangle(compSnake.Body[i].x * Settings.Width, compSnake.Body[i].y * Settings.Height, Settings.Width, Settings.Height),
      compSnake.BodyColor1,
      compSnake.BodyColor2,
   LinearGradientMode.Vertical);
       }

    canvas.FillEllipse(brush, compSnake.Body[i].x * Settings.Width, compSnake.Body[i].y * Settings.Height, Settings.Width, Settings.Height);

            using (Pen outlinePen = new Pen(Color.FromArgb(50, 50, 50), 2))
         {
              canvas.DrawEllipse(outlinePen, compSnake.Body[i].x * Settings.Width, compSnake.Body[i].y * Settings.Height, Settings.Width, Settings.Height);
}

     if (i == 0)
        {
     int eyeSize = Settings.Width / 4;
   int eyeOffset = Settings.Width / 4;

           canvas.FillEllipse(Brushes.White, compSnake.Body[i].x * Settings.Width + eyeOffset, compSnake.Body[i].y * Settings.Height + eyeOffset / 2, eyeSize, eyeSize);
          canvas.FillEllipse(Brushes.White, compSnake.Body[i].x * Settings.Width + Settings.Width - eyeOffset - eyeSize, compSnake.Body[i].y * Settings.Height + eyeOffset / 2, eyeSize, eyeSize);
         canvas.FillEllipse(Brushes.Black, compSnake.Body[i].x * Settings.Width + eyeOffset + 1, compSnake.Body[i].y * Settings.Height + eyeOffset / 2 + 1, eyeSize - 2, eyeSize - 2);
            canvas.FillEllipse(Brushes.Black, compSnake.Body[i].x * Settings.Width + Settings.Width - eyeOffset - eyeSize + 1, compSnake.Body[i].y * Settings.Height + eyeOffset / 2 + 1, eyeSize - 2, eyeSize - 2);
  }

  brush.Dispose();
        }
            }

    // Draw food
            animationCounter++;
  float scale = 1.0f + (float)Math.Sin(animationCounter * 0.1) * 0.1f;
      int foodSize = (int)(Settings.Width * scale);
            int offset = (Settings.Width - foodSize) / 2;

            using (LinearGradientBrush foodBrush = new LinearGradientBrush(
       new Rectangle(food.x * Settings.Width + offset, food.y * Settings.Height + offset, foodSize, foodSize),
  Color.FromArgb(255, 82, 82),
 Color.FromArgb(198, 40, 40),
      LinearGradientMode.Vertical))
        {
 canvas.FillEllipse(foodBrush, food.x * Settings.Width + offset, food.y * Settings.Height + offset, foodSize, foodSize);
          }

using (Pen foodOutline = new Pen(Color.FromArgb(183, 28, 28), 2))
{
         canvas.DrawEllipse(foodOutline, food.x * Settings.Width + offset, food.y * Settings.Height + offset, foodSize, foodSize);
       }
 }

        private void keyisdown(object sender, KeyEventArgs e)
      {
            if (e.KeyCode == Keys.Left && Settings.direction != Directions.Right)
      {
 Settings.direction = Directions.Left;
  }
  else if (e.KeyCode == Keys.Right && Settings.direction != Directions.Left)
      {
     Settings.direction = Directions.Right;
    }
            else if (e.KeyCode == Keys.Up && Settings.direction != Directions.Down)
 {
        Settings.direction = Directions.Up;
    }
            else if (e.KeyCode == Keys.Down && Settings.direction != Directions.Up)
        {
   Settings.direction = Directions.Down;
          }
       else if (e.KeyCode == Keys.Enter && Settings.GameOver)
    {
         RestartGame();
            }
        }

        private void keyisup(object sender, KeyEventArgs e)
   {
      }

        private void RestartGame()
   {
            Settings.GameOver = false;
    label3.Visible = false;
            ShowSetupDialog();
  StartCountdown();
        }

        private void label1_Click(object sender, EventArgs e)
 {
  }

        private void label3_Click(object sender, EventArgs e)
        {
   }
    }
}
