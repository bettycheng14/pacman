using System;
using SplashKitSDK;

namespace pacman
{
    public interface IMovementStrategy
    {
        // Returns the next target point or direction for the character.
        Point2D GetNextTarget(Ghost ghost, Pacman pacman, int[,] map);
    }
}
