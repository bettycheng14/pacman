using System;
using System.Linq;
using SplashKitSDK;

namespace pacman
{
    public class LevelEditor
    {
        private Map _map;
        private int _selectedTileId;

        // Tile options: 0:Empty, 1:Wall, 2:Dot, 3:Empty (eaten dot), 4:Power-Up
        private readonly int[] _tileOptions = { 0, 1, 2, 4 };
        private string[] _tileNames = { "Erase", "Wall", "Dot", "Power-Up" };
        private Font _gameFont; // Remove static and initialization

        public LevelEditor(Map map)
        {
            _map = map;
            _selectedTileId = 1;
            try
            {
                _gameFont = SplashKit.LoadFont("MyFont", "Jersey10.ttf");
            }
            catch
            {
                _gameFont = SplashKit.FontNamed("Default");
            }
        }

        public void HandleInput()
        {
            // [1] Tile Selection via Number Keys
            if (SplashKit.KeyTyped(KeyCode.Num1Key)) _selectedTileId = 1;
            if (SplashKit.KeyTyped(KeyCode.Num2Key)) _selectedTileId = 2;
            if (SplashKit.KeyTyped(KeyCode.Num3Key)) _selectedTileId = 4;
            if (SplashKit.KeyTyped(KeyCode.Num0Key)) _selectedTileId = 0;

            // [2] Place Tile on Mouse Click
            if (SplashKit.MouseDown(MouseButton.LeftButton))
            {
                Point2D mousePos = SplashKit.MousePosition();
                int col = (int)(mousePos.X / GameConstants.ONE_BLOCK_SIZE);
                int row = (int)(mousePos.Y / GameConstants.ONE_BLOCK_SIZE);

                if (row >= 0 && row < _map.Rows && col >= 0 && col < _map.Cols)
                {
                    _map[row, col] = _selectedTileId;
                }
            }

            // [3] Save & Load with Keyboard Shortcuts
            if (SplashKit.KeyTyped(KeyCode.SKey)) SaveMap("custom_level.txt");
            if (SplashKit.KeyTyped(KeyCode.LKey)) LoadMap("custom_level.txt");
        }

        public void Draw()
        {
            // Draw the map as usual
            _map.Draw();
            Font fontToUse = _gameFont ?? SplashKit.FontNamed("Default");

            // Draw the current selected tile info
            SplashKit.DrawText($"Selected: {_tileNames[_tileOptions.ToList().IndexOf(_selectedTileId)]} ({_selectedTileId})", Color.White, fontToUse, 20, 10, SplashKit.CurrentWindowHeight() - 85);
            SplashKit.DrawText("Keys: 0=Erase, 1=Wall, 2=Dot, 3=Power-Up", Color.White, fontToUse, 20, 10, SplashKit.CurrentWindowHeight() - 70);
            SplashKit.DrawText("S = Save Map, L = Load Map", Color.White, fontToUse, 20, 10, SplashKit.CurrentWindowHeight() - 55);
            SplashKit.DrawText("P = Play On This Level", Color.White, fontToUse, 20, 10, SplashKit.CurrentWindowHeight() - 40);
            SplashKit.DrawText("ESC = Exit Editor", Color.White, fontToUse, 20, 10, SplashKit.CurrentWindowHeight() - 25);
        }

        public void SaveMap(string filename)
        {
            try
            {
                using (StreamWriter writer = new StreamWriter(filename))
                {
                    for (int i = 0; i < _map.Rows; i++)
                    {
                        for (int j = 0; j < _map.Cols; j++)
                        {
                            writer.Write(_map[i, j] + " ");
                        }
                        writer.WriteLine();
                    }
                }
                Console.WriteLine($"Map saved to {filename}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving map: {ex.Message}");
            }
        }

        public void LoadMap(string filename)
        {
            try
            {
                if (!File.Exists(filename))
                {
                    Console.WriteLine("File not found.");
                    return;
                }

                string[] lines = File.ReadAllLines(filename);
                int[,] loadedLayout = new int[_map.Rows, _map.Cols]; // Create a new array

                for (int i = 0; i < _map.Rows && i < lines.Length; i++)
                {
                    string[] values = lines[i].Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    for (int j = 0; j < _map.Cols && j < values.Length; j++)
                    {
                        if (int.TryParse(values[j], out int tileValue))
                        {
                            loadedLayout[i, j] = tileValue; // Load into the TEMPORARY array
                        }
                    }
                }

                // CRITICAL FIX: Pass the fully loaded array to the Map class.
                // This ONE call handles the assignment and sets the IsCustomMap flag.
                _map.LoadCustomLayout(loadedLayout);

                Console.WriteLine($"Map loaded from {filename}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading map: {ex.Message}");
            }
        }
    }
}
