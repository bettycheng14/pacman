using System;
using SplashKitSDK;

namespace pacman
{
    public abstract class Character : IGameCharacter
    {
        public double X { get; set; }
        public double Y { get; set; }
        public double Width { get; set; }
        public double Height { get; set; }
        public int Direction { get; set; }
        public double Speed { get; set; }
        public Map Map { get; set; }

        protected Character(double x, double y, double width, double height, double speed, Map map)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
            Speed = speed;
            Map = map;
        }

        public abstract void MoveProcess();

        public virtual bool CheckCollisions()
        {
            // Get the four corners of the character's bounding box
            int topLeftX = GetMapX();
            int topLeftY = GetMapY();
            int bottomRightX = (int)((X + Width - 1) / GameConstants.ONE_BLOCK_SIZE);
            int bottomRightY = (int)((Y + Height - 1) / GameConstants.ONE_BLOCK_SIZE);

            // Check if any corner is outside the map boundaries
            if (topLeftX < 0 || topLeftX >= Map.Cols ||
                topLeftY < 0 || topLeftY >= Map.Rows ||
                bottomRightX < 0 || bottomRightX >= Map.Cols ||
                bottomRightY < 0 || bottomRightY >= Map.Rows)
            {
                return true; // Collision with map boundary
            }

            // Check if any corner is touching a wall
            if (Map[topLeftY, topLeftX] == 1 ||
                Map[bottomRightY, topLeftX] == 1 ||
                Map[topLeftY, bottomRightX] == 1 ||
                Map[bottomRightY, bottomRightX] == 1)
            {
                return true; // Collision with wall
            }

            return false;
        }

        public virtual int GetMapX()
        {
            int mapX = (int)(X / GameConstants.ONE_BLOCK_SIZE);
            return Math.Clamp(mapX, 0, Map.Cols - 1);
        }

        public virtual int GetMapY()
        {
            int mapY = (int)(Y / GameConstants.ONE_BLOCK_SIZE);
            return Math.Clamp(mapY, 0, Map.Rows - 1);
        }

        protected virtual void MoveBackwards()
        {
            switch (Direction)
            {
                case GameConstants.DIRECTION_RIGHT:
                    X -= Speed;
                    break;
                case GameConstants.DIRECTION_UP:
                    Y += Speed;
                    break;
                case GameConstants.DIRECTION_LEFT:
                    X += Speed;
                    break;
                case GameConstants.DIRECTION_BOTTOM:
                    Y -= Speed;
                    break;
            }
        }

        protected virtual void MoveForwards()
        {
            double oldX = X;
            double oldY = Y;

            switch (Direction)
            {
                case GameConstants.DIRECTION_RIGHT:
                    X += Speed;
                    break;
                case GameConstants.DIRECTION_UP:
                    Y -= Speed;
                    break;
                case GameConstants.DIRECTION_LEFT:
                    X -= Speed;
                    break;
                case GameConstants.DIRECTION_BOTTOM:
                    Y += Speed;
                    break;
            }

            // Prevent moving outside map boundaries
            if (X < 0) X = 0;
            if (Y < 0) Y = 0;
            if (X + Width > Map.Cols * GameConstants.ONE_BLOCK_SIZE)
                X = Map.Cols * GameConstants.ONE_BLOCK_SIZE - Width;
            if (Y + Height > Map.Rows * GameConstants.ONE_BLOCK_SIZE)
                Y = Map.Rows * GameConstants.ONE_BLOCK_SIZE - Height;
        }
    }
}
