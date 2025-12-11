using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace Snake_Game_Project
{
    public partial class GameSetupForm : Form
    {
        public GameMode SelectedGameMode { get; private set; }
        public int NumberOfComputerSnakes { get; private set; }
        public string[] ComputerSnakeNames { get; private set; }

        private Button btnEasy;
        private Button btnNormal;
        private Button btnDifficult;
        private Label lblTitle;

        public GameSetupForm()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.Text = "Super Snake Game - Main Menu";
            this.Size = new Size(700, 600);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(146, 208, 80); // Lime green background

            // Enable double buffering for smooth graphics
            this.DoubleBuffered = true;
            this.Paint += GameSetupForm_Paint;

            // Title Label
            lblTitle = new Label
            {
                Text = "SUPER SNAKE GAME",
                Location = new Point(50, 40),
                Size = new Size(600, 60),
                Font = new Font("Comic Sans MS", 32, FontStyle.Bold),
                ForeColor = Color.White,
                TextAlign = ContentAlignment.MiddleCenter,
                BackColor = Color.Transparent
            };
            this.Controls.Add(lblTitle);

            // Easy Button (Green)
            btnEasy = CreateDifficultyButton(
     "EASY\n(No Walls!)",
                new Point(200, 150),
     Color.FromArgb(76, 175, 80),
   Color.FromArgb(56, 142, 60)
     );
  btnEasy.Click += (s, e) => StartGame(GameMode.Easy);
      this.Controls.Add(btnEasy);

   // Normal Button (Yellow/Orange)
btnNormal = CreateDifficultyButton(
     "NORMAL\n(Watch out for Rocks!)",
                new Point(200, 270),
      Color.FromArgb(255, 193, 7),
           Color.FromArgb(245, 124, 0)
            );
btnNormal.Click += (s, e) => StartGame(GameMode.Normal);
     this.Controls.Add(btnNormal);

            // Difficult Button (Red)
         btnDifficult = CreateDifficultyButton(
    "DIFFICULT\n(The Cage!)",
                new Point(200, 390),
     Color.FromArgb(244, 67, 54),
       Color.FromArgb(198, 40, 40)
     );
            btnDifficult.Click += (s, e) => StartGame(GameMode.Difficult);
            this.Controls.Add(btnDifficult);
        }

private void GameSetupForm_Paint(object sender, PaintEventArgs e)
 {
            Graphics g = e.Graphics;
         g.SmoothingMode = SmoothingMode.AntiAlias;

            // Draw decorative snakes around the menu
            DrawDecorativeSnake(g, new Point(30, 100), new Point(30, 250), Color.FromArgb(100, 180, 50));
       DrawDecorativeSnake(g, new Point(640, 150), new Point(640, 300), Color.FromArgb(100, 180, 50));
   DrawDecorativeSnake(g, new Point(80, 480), new Point(200, 520), Color.FromArgb(100, 180, 50));
            DrawDecorativeSnake(g, new Point(500, 500), new Point(620, 480), Color.FromArgb(100, 180, 50));
        }

        private void DrawDecorativeSnake(Graphics g, Point start, Point end, Color snakeColor)
 {
   // Draw a simple curved snake decoration
        using (Pen snakePen = new Pen(snakeColor, 20))
            {
          snakePen.StartCap = LineCap.Round;
              snakePen.EndCap = LineCap.Round;
     g.DrawBezier(snakePen, start, 
    new Point((start.X + end.X) / 2, start.Y),
            new Point((start.X + end.X) / 2, end.Y),
       end);
         }

   // Draw snake head
     using (SolidBrush headBrush = new SolidBrush(Color.FromArgb(80, 150, 40)))
    {
      g.FillEllipse(headBrush, end.X - 12, end.Y - 12, 24, 24);
      }

       // Draw eyes
            g.FillEllipse(Brushes.White, end.X - 7, end.Y - 5, 6, 6);
            g.FillEllipse(Brushes.White, end.X + 1, end.Y - 5, 6, 6);
          g.FillEllipse(Brushes.Black, end.X - 5, end.Y - 3, 3, 3);
   g.FillEllipse(Brushes.Black, end.X + 3, end.Y - 3, 3, 3);

      // Draw tongue
            using (Pen tonguePen = new Pen(Color.Red, 2))
      {
        g.DrawLine(tonguePen, end.X, end.Y + 8, end.X - 3, end.Y + 15);
  g.DrawLine(tonguePen, end.X, end.Y + 8, end.X + 3, end.Y + 15);
    }
        }

   private Button CreateDifficultyButton(string text, Point location, Color topColor, Color bottomColor)
     {
  Button btn = new Button
          {
       Text = text,
       Location = location,
     Size = new Size(300, 90),
      Font = new Font("Comic Sans MS", 16, FontStyle.Bold),
           ForeColor = text.Contains("NORMAL") ? Color.Black : Color.White,
                FlatStyle = FlatStyle.Flat,
   Cursor = Cursors.Hand,
        BackColor = topColor
            };

        btn.FlatAppearance.BorderSize = 4;
   btn.FlatAppearance.BorderColor = Color.FromArgb(60, 60, 60);

            // Add gradient effect
     btn.Paint += (s, e) =>
        {
   Button b = s as Button;
                Graphics g = e.Graphics;
     g.SmoothingMode = SmoothingMode.AntiAlias;

         Rectangle rect = b.ClientRectangle;
         
  // Draw gradient background
   using (LinearGradientBrush brush = new LinearGradientBrush(
         rect, topColor, bottomColor, LinearGradientMode.Vertical))
                {
           g.FillRectangle(brush, rect);
     }

                // Draw glossy highlight
              Rectangle highlightRect = new Rectangle(rect.X + 10, rect.Y + 10, rect.Width - 20, rect.Height / 3);
      using (LinearGradientBrush highlightBrush = new LinearGradientBrush(
                    highlightRect, 
 Color.FromArgb(100, 255, 255, 255),
           Color.FromArgb(0, 255, 255, 255),
     LinearGradientMode.Vertical))
     {
 using (GraphicsPath path = GetRoundedRectangle(highlightRect, 15))
           {
     g.FillPath(highlightBrush, path);
            }
                }

           // Draw border
          using (Pen borderPen = new Pen(Color.FromArgb(60, 60, 60), 4))
  {
            g.DrawRectangle(borderPen, 2, 2, rect.Width - 4, rect.Height - 4);
     }

      // Draw text
          using (StringFormat sf = new StringFormat())
         {
         sf.Alignment = StringAlignment.Center;
        sf.LineAlignment = StringAlignment.Center;
      
      // Draw text shadow
                    using (SolidBrush shadowBrush = new SolidBrush(Color.FromArgb(100, 0, 0, 0)))
     {
  g.DrawString(b.Text, b.Font, shadowBrush, 
         new Rectangle(rect.X + 3, rect.Y + 3, rect.Width, rect.Height), sf);
      }
 
           // Draw main text
           using (SolidBrush textBrush = new SolidBrush(b.ForeColor))
        {
  g.DrawString(b.Text, b.Font, textBrush, rect, sf);
 }
            }
            };

            // Hover effect
            btn.MouseEnter += (s, e) =>
            {
             btn.Size = new Size(310, 95);
      btn.Location = new Point(location.X - 5, location.Y - 2);
            };

            btn.MouseLeave += (s, e) =>
    {
           btn.Size = new Size(300, 90);
     btn.Location = location;
            };

   return btn;
      }

     private GraphicsPath GetRoundedRectangle(Rectangle rect, int radius)
        {
            GraphicsPath path = new GraphicsPath();
       path.AddArc(rect.X, rect.Y, radius, radius, 180, 90);
            path.AddArc(rect.Right - radius, rect.Y, radius, radius, 270, 90);
            path.AddArc(rect.Right - radius, rect.Bottom - radius, radius, radius, 0, 90);
    path.AddArc(rect.X, rect.Bottom - radius, radius, radius, 90, 90);
    path.CloseFigure();
            return path;
        }

        private void StartGame(GameMode mode)
        {
    SelectedGameMode = mode;

   // Configure game based on difficulty
            switch (mode)
            {
   case GameMode.Easy:
            // No AI snakes, no obstacles
         NumberOfComputerSnakes = 0;
     ComputerSnakeNames = new string[0];
 break;

            case GameMode.Normal:
  // 2 AI snakes
    NumberOfComputerSnakes = 2;
   ComputerSnakeNames = new string[] { "Bilal", "Shoaib" };
        break;

     case GameMode.Difficult:
        // 3 AI snakes
          NumberOfComputerSnakes = 3;
        ComputerSnakeNames = new string[] { "Bilal", "Shoaib", "Gohar" };
        break;
    }

        this.DialogResult = DialogResult.OK;
   this.Close();
     }
    }
}