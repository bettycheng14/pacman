using System;
using SplashKitSDK;

namespace pacman
{
    public class Pacman : Character
    {
        public int NextDirection { get; set; }
        public bool CanEatGhosts { get; private set; }
        private DateTime _powerUpEndTime;

        public Pacman(double x, double y, double width, double height, double speed)
            : base(x, y, width, height, speed)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
            Speed = speed;
            Direction = GameConstants.DIRECTION_RIGHT;
            NextDirection = GameConstants.DIRECTION_RIGHT;
        }

        public override void MoveProcess()
        {
            ChangeDirectionIfPossible();
            MoveForwards();
            if (CheckCollisions())
            {
                MoveBackwards();
            }
        }

        public void Eat(Map map, ref int score)
        {
            int row = GetMapY();
            int col = GetMapX();

            int points = map.ConsumeDotAt(row, col);
            if (points > 0)
            {
                score += points;
                if (points == 10) ActivatePowerUp();
            }
        }

        public void ActivatePowerUp()
        {
            CanEatGhosts = true;
            _powerUpEndTime = DateTime.Now.AddSeconds(10); // 10 seconds of power
        }

        // Add this method to check if power-up expired
        public void UpdatePowerUp()
        {
            if (CanEatGhosts && DateTime.Now > _powerUpEndTime)
            {
                CanEatGhosts = false;
            }
        }

        public override bool CheckCollisions()
        {
            bool isCollided = false;
            if (
                GameConstants.Map[GetMapY(), GetMapX()] == 1
                || GameConstants.Map[(int)(Y / GameConstants.ONE_BLOCK_SIZE + 0.9999), GetMapX()]
                    == 1
                || GameConstants.Map[GetMapY(), (int)(X / GameConstants.ONE_BLOCK_SIZE + 0.9999)]
                    == 1
                || GameConstants.Map[
                    (int)(Y / GameConstants.ONE_BLOCK_SIZE + 0.9999),
                    (int)(X / GameConstants.ONE_BLOCK_SIZE + 0.9999)
                ] == 1
            )
            {
                isCollided = true;
            }
            return isCollided;
        }

        public bool CheckGhostCollision(Ghost[] ghosts)
        {
            foreach (Ghost ghost in ghosts)
            {
                if (ghost.GetMapX() == GetMapX() && ghost.GetMapY() == GetMapY())
                {
                    return true;
                }
            }
            return false;
        }

        public void ChangeDirectionIfPossible()
        {
            if (Direction == NextDirection)
                return;

            int tempDirection = Direction;
            Direction = NextDirection;
            MoveForwards();

            if (CheckCollisions())
            {
                MoveBackwards();
                Direction = tempDirection;
            }
            else
            {
                MoveBackwards();
            }
        }

        public int GetMapXRightSide()
        {
            return (int)((X * 0.99 + GameConstants.ONE_BLOCK_SIZE) / GameConstants.ONE_BLOCK_SIZE);
        }

        public int GetMapYRightSide()
        {
            return (int)((Y * 0.99 + GameConstants.ONE_BLOCK_SIZE) / GameConstants.ONE_BLOCK_SIZE);
        }

        public void DrawSimple()
        {
            double centerX = X + Width / 2;
            double centerY = Y + Height / 2;

            SplashKit.FillCircle(Color.Yellow, centerX, centerY, Width / 2);
        }
    }
}
