using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace Snake_Game_Project
{
    public class ComputerSnake
    {
        public List<Circle> Body { get; set; }
        public string Name { get; set; }
        public Directions Direction { get; set; }
        public Color HeadColor1 { get; set; }
        public Color HeadColor2 { get; set; }
        public Color BodyColor1 { get; set; }
        public Color BodyColor2 { get; set; }
        public bool IsAlive { get; set; }
        private Random rnd;
        private int moveCounter;
        private int changeDirectionInterval;

        public ComputerSnake(string name, int startX, int startY, Color headColor1, Color headColor2, Color bodyColor1, Color bodyColor2)
        {
            Name = name;
            Body = new List<Circle>();
            rnd = new Random(Guid.NewGuid().GetHashCode());
            IsAlive = true;
            moveCounter = 0;
            changeDirectionInterval = rnd.Next(10, 30);

            Circle head = new Circle { x = startX, y = startY };
            Body.Add(head);

            Circle body1 = new Circle { x = startX, y = startY };
            Body.Add(body1);

            Circle body2 = new Circle { x = startX, y = startY };
            Body.Add(body2);

            Direction = (Directions)rnd.Next(0, 4);

            HeadColor1 = headColor1;
            HeadColor2 = headColor2;
            BodyColor1 = bodyColor1;
            BodyColor2 = bodyColor2;
        }

        public void Move(int maxX, int maxY, Circle food, List<ComputerSnake> otherSnakes, List<Circle> playerSnake)
        {
            if (!IsAlive) return;

            moveCounter++;
            if (moveCounter >= changeDirectionInterval)
            {
                ChangeDirectionIntelligently(maxX, maxY, food, otherSnakes, playerSnake);
                moveCounter = 0;
                changeDirectionInterval = rnd.Next(10, 30);
            }

            // Calculate next position BEFORE moving
            int nextX = Body[0].x;
            int nextY = Body[0].y;

            switch (Direction)
            {
                case Directions.Right:
                    nextX++;
                    break;
                case Directions.Left:
                    nextX--;
                    break;
                case Directions.Up:
                    nextY--;
                    break;
                case Directions.Down:
                    nextY++;
                    break;
            }

            // Check if next position would be out of bounds
            if (nextX < 0 || nextY < 0 || nextX >= maxX || nextY >= maxY)
            {
                // Try to find a valid direction to stay within bounds
                List<Directions> validDirections = GetValidDirections(maxX, maxY, otherSnakes, playerSnake);
                if (validDirections.Count > 0)
                {
                    Direction = validDirections[rnd.Next(validDirections.Count)];
                    // Recalculate next position with new direction
                    nextX = Body[0].x;
                    nextY = Body[0].y;
                    switch (Direction)
                    {
                        case Directions.Right:
                            nextX++;
                            break;
                        case Directions.Left:
                            nextX--;
                            break;
                        case Directions.Up:
                            nextY--;
                            break;
                        case Directions.Down:
                            nextY++;
                            break;
                    }
                }
                else
                {
                    // No valid direction - don't kill the snake, just keep current position
                    return;
                }
            }

            // Check if next position would hit another snake or self
            if (!IsSafePosition(nextX, nextY, otherSnakes, playerSnake))
            {
                // Try to find a valid direction
                List<Directions> validDirections = GetValidDirections(maxX, maxY, otherSnakes, playerSnake);
                if (validDirections.Count > 0)
                {
                    Direction = validDirections[rnd.Next(validDirections.Count)];
                    // Recalculate next position with new direction
                    nextX = Body[0].x;
                    nextY = Body[0].y;
                    switch (Direction)
                    {
                        case Directions.Right:
                            nextX++;
                            break;
                        case Directions.Left:
                            nextX--;
                            break;
                        case Directions.Up:
                            nextY--;
                            break;
                        case Directions.Down:
                            nextY++;
                            break;
                    }
                }
                else
                {
                    // Trapped - die
                    IsAlive = false;
                    return;
                }
            }

            // Now actually move the snake
            for (int i = Body.Count - 1; i >= 0; i--)
            {
                if (i == 0)
                {
                    Body[i].x = nextX;
                    Body[i].y = nextY;

                    // Check if food is eaten
                    if (Body[0].x == food.x && Body[0].y == food.y)
                    {
                        Eat();
                    }
                }
                else
                {
                    Body[i].x = Body[i - 1].x;
                    Body[i].y = Body[i - 1].y;
                }
            }
        }

        private void ChangeDirectionIntelligently(int maxX, int maxY, Circle food, List<ComputerSnake> otherSnakes, List<Circle> playerSnake)
        {
            List<Directions> validDirections = GetValidDirections(maxX, maxY, otherSnakes, playerSnake);

            if (validDirections.Count > 0)
            {
                if (rnd.Next(0, 100) < 60)
                {
                    Directions bestDirection = GetDirectionTowardFood(food, validDirections);
                    if (bestDirection != (Directions)(-1))
                    {
                        Direction = bestDirection;
                        return;
                    }
                }

                Direction = validDirections[rnd.Next(validDirections.Count)];
            }
        }

        private List<Directions> GetValidDirections(int maxX, int maxY, List<ComputerSnake> otherSnakes, List<Circle> playerSnake)
        {
            int headX = Body[0].x;
            int headY = Body[0].y;

            List<Directions> validDirections = new List<Directions>();

            // Check Right
            if (Direction != Directions.Left && headX + 1 < maxX && IsSafePosition(headX + 1, headY, otherSnakes, playerSnake))
                validDirections.Add(Directions.Right);

            // Check Left
            if (Direction != Directions.Right && headX - 1 >= 0 && IsSafePosition(headX - 1, headY, otherSnakes, playerSnake))
                validDirections.Add(Directions.Left);

            // Check Up
            if (Direction != Directions.Down && headY - 1 >= 0 && IsSafePosition(headX, headY - 1, otherSnakes, playerSnake))
                validDirections.Add(Directions.Up);

            // Check Down
            if (Direction != Directions.Up && headY + 1 < maxY && IsSafePosition(headX, headY + 1, otherSnakes, playerSnake))
                validDirections.Add(Directions.Down);

            return validDirections;
        }

        private Directions GetDirectionTowardFood(Circle food, List<Directions> validDirections)
        {
            int headX = Body[0].x;
            int headY = Body[0].y;

            List<Directions> towardFood = new List<Directions>();

            if (food.x > headX && validDirections.Contains(Directions.Right))
                towardFood.Add(Directions.Right);
            if (food.x < headX && validDirections.Contains(Directions.Left))
                towardFood.Add(Directions.Left);
            if (food.y < headY && validDirections.Contains(Directions.Up))
                towardFood.Add(Directions.Up);
            if (food.y > headY && validDirections.Contains(Directions.Down))
                towardFood.Add(Directions.Down);

            if (towardFood.Count > 0)
                return towardFood[rnd.Next(towardFood.Count)];

             return (Directions)(-1);
        }

        private bool IsSafePosition(int x, int y, List<ComputerSnake> otherSnakes, List<Circle> playerSnake)
        {
            // Don't check self-collision with body segments that are stacked (at initialization)
            foreach (var segment in Body.Skip(1))
            {
                if (segment.x == x && segment.y == y)
                {
                    // Allow if all body segments are at the same position (initialization state)
                    bool allSame = Body.All(s => s.x == Body[0].x && s.y == Body[0].y);
                    if (!allSame)
                        return false;
                }
            }

            foreach (var segment in playerSnake)
            {
                if (segment.x == x && segment.y == y)
                    return false;
            }

            foreach (var snake in otherSnakes)
            {
                if (snake != this && snake.IsAlive)
                {
                    foreach (var segment in snake.Body)
                    {
                        if (segment.x == x && segment.y == y)
                            return false;
                    }
                }
            }

            return true;
        }

        private void Eat()
        {
            Circle newSegment = new Circle();
            newSegment.x = Body[Body.Count - 1].x;
            newSegment.y = Body[Body.Count - 1].y;
            Body.Add(newSegment);
        }
    }
}