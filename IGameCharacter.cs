using System;
using SplashKitSDK;

namespace pacman
{
    public interface IGameCharacter
    {
        double X { get; set; }
        double Y { get; set; }
        double Width { get; set; }
        double Height { get; set; }
        int Direction { get; set; }

        void MoveProcess();
        bool CheckCollisions();
        int GetMapX();
        int GetMapY();
    }
}
