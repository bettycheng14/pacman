using System;
using SplashKitSDK;

namespace pacman
{
    public class ChasePacmanStrategy : IMovementStrategy
    {
        public Point2D GetNextTarget(Ghost ghost, Pacman pacman, int[,] map)
        {
            return new Point2D { X = pacman.X, Y = pacman.Y };
        }
    }
}