using System;
using SplashKitSDK;

namespace pacman
{
    public class FleePacmanStrategy : IMovementStrategy
    {
        public Point2D GetNextTarget(Ghost ghost, Pacman pacman, int[,] map)
        {
            double fleeX = (pacman.X < GameConstants.ONE_BLOCK_SIZE * 10)
                ? GameConstants.ONE_BLOCK_SIZE * 19
                : GameConstants.ONE_BLOCK_SIZE * 1;

            double fleeY = (pacman.Y < GameConstants.ONE_BLOCK_SIZE * 10)
                ? GameConstants.ONE_BLOCK_SIZE * 19
                : GameConstants.ONE_BLOCK_SIZE * 1;

            return new Point2D { X = fleeX, Y = fleeY };
        }
    }
}
