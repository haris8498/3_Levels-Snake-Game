using System.Drawing;

namespace Snake_Game_Project
{
    public class Obstacle
    {
        public int x { get; set; }
        public int y { get; set; }
        public int width { get; set; }
        public int height { get; set; }

        public Obstacle(int x, int y, int width = 1, int height = 1)
        {
            this.x = x;
            this.y = y;
            this.width = width;
            this.height = height;
        }

        public bool Contains(int posX, int posY)
        {
            return posX >= x && posX < x + width && posY >= y && posY < y + height;
        }
    }
}
