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

        protected Character(double x, double y, double width, double height, double speed)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
            Speed = speed;
        }

        public abstract void MoveProcess();

        public virtual bool CheckCollisions()
        {
            return GameConstants.Map[GetMapY(), GetMapX()] == 1
                || GameConstants.Map[(int)(Y / GameConstants.ONE_BLOCK_SIZE + 0.9999), GetMapX()]
                    == 1
                || GameConstants.Map[GetMapY(), (int)(X / GameConstants.ONE_BLOCK_SIZE + 0.9999)]
                    == 1
                || GameConstants.Map[
                    (int)(Y / GameConstants.ONE_BLOCK_SIZE + 0.9999),
                    (int)(X / GameConstants.ONE_BLOCK_SIZE + 0.9999)
                ] == 1;
        }

        public virtual int GetMapX()
        {
            return (int)(X / GameConstants.ONE_BLOCK_SIZE);
        }

        public virtual int GetMapY()
        {
            return (int)(Y / GameConstants.ONE_BLOCK_SIZE);
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
        }
    }
}
