using System;
using SplashKitSDK;

namespace pacman
{
    public class Program
    {
        public static void Main()
        {
            Window gameWindow = new Window("Pacman", 420, 500);
            PacmanGame game = new PacmanGame(gameWindow);

            try
            {
                while (!gameWindow.CloseRequested)
                {
                    SplashKit.ProcessEvents();
                    game.HandleInput();
                    game.Update();
                    game.Draw();
                }
            }
            finally
            {
                // Ensure music is cleaned up even if game crashes
                game.CleanUp();
                gameWindow.Close();
            }
        }
    }
}
