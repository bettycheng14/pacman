using System;
using SplashKitSDK;

namespace pacman
{
    public class RandomMovementStrategy : IMovementStrategy
    {
        private int _currentTargetIndex = 0;

        public Point2D GetNextTarget(Ghost ghost, Pacman pacman, int[,] map)
        {
            _currentTargetIndex = (_currentTargetIndex + 1) % GameConstants.RandomTargetsForGhosts.Length;
            return GameConstants.RandomTargetsForGhosts[_currentTargetIndex];
        }
    }
}
