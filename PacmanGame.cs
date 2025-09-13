using System;
using SplashKitSDK;

namespace pacman
{
    public enum GameState
    {
        StartScreen,
        Playing,
        GameOver,
    }

    public class PacmanGame
    {
        private Window _gameWindow;
        private GameState _gameState = GameState.StartScreen;
        private Map _map;
        private Pacman _pacman;
        private Ghost[] _ghosts;
        private int _lives = 3;
        private int _score = 0;
        private int _ghostCount = 4;

        private Bitmap _pacmanImage;
        private Bitmap _pacmanSpriteSheet;
        private AnimationScript _pacmanAnimationScript;
        private Animation _pacmanAnimation;
        private bool _animationsLoaded = false;
        private Music _backgroundMusic;
        private bool _musicPlaying = false;
        private DrawingOptions opt;
        private static Font _gameFont = SplashKit.LoadFont("MyFont", "Jersey10.ttf");

        // Ghost colors
        private Color[] _ghostColors =
        {
            Color.Red, // Blinky
            Color.Pink, // Pinky
            Color.Cyan, // Inky
            Color.Orange, // Clyde
        };

        // Start screen button
        private Rectangle _startButton;
        private bool _buttonHovered = false;

        // Mute button
        private Bitmap _soundOnIcon;
        private Bitmap _soundOffIcon;
        private bool _isMuted = false;
        private Rectangle _muteButton;
        private bool _muteButtonHovered = false;

        public PacmanGame(Window gameWindow)
        {
            try
            {
                _gameWindow = gameWindow;
                GameConstants.Initialize();

                // Load resources
                // Load background music
                _backgroundMusic = SplashKit.LoadMusic("background", "backgroundMusic.mp3");
                // Load pacman logo for start screen
                _pacmanImage = new Bitmap("pacman", "pacman.png");
                _gameFont = SplashKit.LoadFont("MyFont", "Jersey10.ttf");
                // Load for pacman eating animation
                _pacmanSpriteSheet = new Bitmap("PacmanSprite", "pacmanFrame.png");
                _pacmanSpriteSheet.SetCellDetails(20, 20, 7, 1, 7);
                _pacmanAnimationScript = new AnimationScript("PacmanAnimation", "pacman_anim.txt");
                _pacmanAnimation = _pacmanAnimationScript.CreateAnimation("Right");
                opt = SplashKit.OptionWithAnimation(_pacmanAnimation);

                // Initialize start button
                _startButton = new Rectangle();
                _startButton.Width = 180;
                _startButton.Height = 50;
                _startButton.X = (_gameWindow.Width - _startButton.Width) / 2;
                _startButton.Y = _gameWindow.Height / 2 + 80;

                // Initialize mute button (left bottom corner)
                _soundOnIcon = new Bitmap("sound_on", "sound.png");
                _soundOffIcon = new Bitmap("sound_off", "mute.png");
                _muteButton = new Rectangle();
                _muteButton.Width = _soundOnIcon.Width + 3;
                _muteButton.Height = _soundOnIcon.Height + 3;
                _muteButton.X = _muteButton.Width;
                _muteButton.Y = GameConstants.Map.GetLength(0) * GameConstants.ONE_BLOCK_SIZE;

                CreateNewPacman();
                CreateGhosts();
                // Init the Map
                _map = new Map();

                // Start playing music
                PlayBackgroundMusic();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                _animationsLoaded = false;
            }
        }

        private void CreateNewPacman()
        {
            _pacman = new Pacman(
                GameConstants.ONE_BLOCK_SIZE,
                GameConstants.ONE_BLOCK_SIZE,
                GameConstants.ONE_BLOCK_SIZE,
                GameConstants.ONE_BLOCK_SIZE,
                GameConstants.ONE_BLOCK_SIZE / 5
            );
        }

        private void CreateGhosts()
        {
            _ghosts = new Ghost[_ghostCount];
            for (int i = 0; i < _ghostCount; i++)
            {
                _ghosts[i] = new Ghost(
                    9 * GameConstants.ONE_BLOCK_SIZE + (i % 2) * GameConstants.ONE_BLOCK_SIZE,
                    10 * GameConstants.ONE_BLOCK_SIZE + (i / 2) * GameConstants.ONE_BLOCK_SIZE,
                    GameConstants.ONE_BLOCK_SIZE,
                    GameConstants.ONE_BLOCK_SIZE,
                    _pacman.Speed / 2,
                    6 + i,
                    _pacman,
                    _ghostColors[i],
                    new RandomMovementStrategy()
                );
            }
        }

        private void PlayBackgroundMusic()
        {
            try
            {
                if (_backgroundMusic != null && !_musicPlaying)
                {
                    SplashKit.PlayMusic(_backgroundMusic);
                    SplashKit.SetMusicVolume(0.5);
                    _musicPlaying = true;
                    Console.WriteLine("Background music started");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error playing music: {ex.Message}");
            }
        }

        private void StopBackgroundMusic()
        {
            if (_musicPlaying)
            {
                SplashKit.StopMusic();
                _musicPlaying = false;
                Console.WriteLine("Background music stopped");
            }
        }
        private void ToggleMute()
        {
            _isMuted = !_isMuted;

            if (_isMuted)
            {
                SplashKit.PauseMusic();
                Console.WriteLine("Music muted");
            }
            else
            {
                SplashKit.ResumeMusic();
                Console.WriteLine("Music unmuted");
            }
        }

        public void CleanUp()
        {
            StopBackgroundMusic();
            if (_backgroundMusic != null)
            {
                SplashKit.FreeMusic(_backgroundMusic);
            }
            if (_soundOnIcon != null)
            {
                SplashKit.FreeBitmap(_soundOnIcon);
            }
            if (_soundOffIcon != null)
            {
                SplashKit.FreeBitmap(_soundOffIcon);
            }
        }

        private void DrawRemainingLives()
        {
            SplashKit.DrawText(
                "Lives: ",
                Color.White,
                _gameFont,
                20,
                _gameWindow.Width - 110,
                GameConstants.Map.GetLength(0) * GameConstants.ONE_BLOCK_SIZE
            );

            for (int i = 0; i < _lives; i++)
            {
                Bitmap heart = SplashKit.LoadBitmap("heart", "heart.png");
                heart.Draw(
                    350 + i * GameConstants.ONE_BLOCK_SIZE,
                    GameConstants.Map.GetLength(0) * GameConstants.ONE_BLOCK_SIZE + 5
                );
            }
        }

        public void HandleInput()
        {
            switch (_gameState)
            {
                case GameState.StartScreen:
                    HandleStartScreenInput();
                    break;
                case GameState.Playing:
                    HandleGameInput();
                    break;
                case GameState.GameOver:
                    HandleGameOverInput();
                    break;
            }
        }

        private void HandleStartScreenInput()
        {
            // Check if mouse is over the button
            Point2D mousePos = SplashKit.MousePosition();
            _buttonHovered = SplashKit.PointInRectangle(mousePos, _startButton);

            // Check for button click
            if (_buttonHovered && SplashKit.MouseClicked(MouseButton.LeftButton))
            {
                StartGame();
            }

            // Check for mute button click
            _muteButtonHovered = SplashKit.PointInRectangle(mousePos, _muteButton);

            if (_muteButtonHovered && SplashKit.MouseClicked(MouseButton.LeftButton))
            {
                ToggleMute();
            }

            // Add music control keys
            if (SplashKit.KeyTyped(KeyCode.MKey))
            {
                ToggleMute();
            }
        }

        private void HandleGameInput()
        {
            if (SplashKit.KeyDown(KeyCode.LeftKey) || SplashKit.KeyDown(KeyCode.AKey))
            {
                _pacman.NextDirection = GameConstants.DIRECTION_LEFT;
            }
            else if (SplashKit.KeyDown(KeyCode.UpKey) || SplashKit.KeyDown(KeyCode.WKey))
            {
                _pacman.NextDirection = GameConstants.DIRECTION_UP;
            }
            else if (SplashKit.KeyDown(KeyCode.RightKey) || SplashKit.KeyDown(KeyCode.DKey))
            {
                _pacman.NextDirection = GameConstants.DIRECTION_RIGHT;
            }
            else if (SplashKit.KeyDown(KeyCode.DownKey) || SplashKit.KeyDown(KeyCode.SKey))
            {
                _pacman.NextDirection = GameConstants.DIRECTION_BOTTOM;
            }

            // Check for mute button click
            Point2D mousePos = SplashKit.MousePosition();
            _muteButtonHovered = SplashKit.PointInRectangle(mousePos, _muteButton);

            if (_muteButtonHovered && SplashKit.MouseClicked(MouseButton.LeftButton))
            {
                ToggleMute();
            }

            // Add music control keys
            if (SplashKit.KeyTyped(KeyCode.MKey))
            {
                ToggleMute();
            }

            // ESC key to return to start screen
            if (SplashKit.KeyTyped(KeyCode.EscapeKey))
            {
                _gameState = GameState.StartScreen;
            }
        }

        private void HandleGameOverInput()
        {
            // Check if mouse is over the button
            Point2D mousePos = SplashKit.MousePosition();
            _buttonHovered = SplashKit.PointInRectangle(mousePos, _startButton);

            // Check for button click
            if (_buttonHovered && SplashKit.MouseClicked(MouseButton.LeftButton))
            {
                ResetGame();
                _gameState = GameState.Playing;
            }
        }

        public void Update()
        {
            switch (_gameState)
            {
                case GameState.StartScreen:
                    if (!_musicPlaying)
                        PlayBackgroundMusic();
                    break;
                case GameState.Playing:
                    if (!_musicPlaying)
                        PlayBackgroundMusic();
                    break;
                case GameState.GameOver:
                    StopBackgroundMusic();
                    break;
            }
            if (_gameState == GameState.Playing && _pacman != null)
            {
                // Update power-up timer
                _pacman.UpdatePowerUp();
                if (_animationsLoaded && _pacmanAnimation != null)
                {
                    // Store old position for collision checking
                    double oldX = _pacman.X;
                    double oldY = _pacman.Y;

                    // Update animation (this will automatically apply vectors)
                    _pacmanAnimation.Update();

                    // Check for collisions after movement
                    if (_pacman.CheckCollisions())
                    {
                        // If collision, revert to old position
                        _pacman.X = oldX;
                        _pacman.Y = oldY;
                    }

                    _pacman.Eat(_map, ref _score);
                }
                else
                {
                    // Fallback to manual movement
                    _pacman.MoveProcess();
                    _pacman.Eat(_map, ref _score);
                }
                UpdateGhosts();

                // Check for ghost collisions - different behavior when powered up
                if (_pacman.CheckGhostCollision(_ghosts))
                {
                    if (_pacman.CanEatGhosts)
                    {
                        // Pacman eats the ghost!
                        EatGhost();
                    }
                    else
                    {
                        // Ghost eats Pacman
                        OnGhostCollision();
                    }
                }

                if (CheckWinCondition())
                {
                    _gameState = GameState.GameOver;
                }
            }
        }

        private void EatGhost()
        {
            // Find which ghost was actually collided with
            Ghost ghostToEat = null;
            foreach (var ghost in _ghosts)
            {
                if (ghost.GetMapX() == _pacman.GetMapX() && ghost.GetMapY() == _pacman.GetMapY())
                {
                    ghostToEat = ghost;
                    break;
                }
            }

            if (ghostToEat != null)
            {
                _score += 200; // Big points for eating ghost

                // Move only the eaten ghost back to start
                ghostToEat.X = 9 * GameConstants.ONE_BLOCK_SIZE;
                ghostToEat.Y = 10 * GameConstants.ONE_BLOCK_SIZE;
            }
        }

        private void DrawGame()
        {
            DrawMuteButton();

            _map.Draw();

            // Draw ghosts
            if (_ghosts != null)
            {
                foreach (Ghost ghost in _ghosts)
                {
                    ghost.Draw();
                }
            }

            // Draw Pacman with animation
            if (_pacman != null && _pacmanSpriteSheet != null)
            {
                // DrawingOptions pacmanOpts = SplashKit.OptionWithAnimation(_pacmanAnimation);
                _pacmanSpriteSheet.Draw(_pacman.X, _pacman.Y, opt);
                _pacmanAnimation.Update();
            }
            else if (_pacman != null)
            {
                _pacman.DrawSimple();
            }

            DrawScore();
            DrawRemainingLives();
        }

        private void UpdateGhosts()
        {
            if (_ghosts == null)
                return;

            foreach (Ghost ghost in _ghosts)
            {
                ghost.MoveProcess();
            }
        }

        private void OnGhostCollision()
        {
            _lives--;
            if (_lives > 0)
            {
                CreateNewPacman();
                CreateGhosts();
            }
            else
            {
                // Game over logic
                _gameState = GameState.GameOver;
            }
        }

        public void Draw()
        {
            _gameWindow.Clear(Color.Black);

            switch (_gameState)
            {
                case GameState.StartScreen:
                    DrawStartScreen();
                    break;
                case GameState.Playing:
                    DrawGame();
                    break;
                case GameState.GameOver:
                    DrawGameOverScreen();
                    break;
            }

            _gameWindow.Refresh(60);
        }

        private void DrawStartScreen()
        {
            // Draw title
            Font myFont = SplashKit.LoadFont("MyFont", "Jersey10.ttf");
            DrawMuteButton();
            try
            {
                Bitmap pacman = SplashKit.LoadBitmap("pacman menu", "pacman.png");
                // Pacman image
                SplashKit.DrawBitmap(
                    pacman,
                    _gameWindow.Width / 2 - pacman.Width / 2,
                    _gameWindow.Height / 3 - pacman.Height / 2
                );
            }
            catch
            {
                // If image fails to load, draw a yellow circle instead
                SplashKit.FillCircle(
                    Color.Yellow,
                    _gameWindow.Width / 2,
                    _gameWindow.Height / 3,
                    50
                );
            }

            // Title
            SplashKit.DrawText(
                "PAC-MAN",
                Color.Yellow,
                myFont,
                48,
                _gameWindow.Width / 2 - 70,
                _gameWindow.Height / 3 + 100
            );

            // Start button
            // Draw play again button
            Color buttonColor = _buttonHovered ? Color.AliceBlue : Color.DarkBlue;
            SplashKit.FillRectangle(buttonColor, _startButton);
            SplashKit.DrawRectangle(Color.White, _startButton);

            SplashKit.DrawText(
                "Game Start",
                Color.White,
                myFont,
                35,
                _startButton.X + 20,
                _startButton.Y + 10
            );
        }

        private void DrawGameOverScreen()
        {
            if (_gameFont == null)
                return;

            // Draw game over message
            if (_lives > 0)
            {
                SplashKit.DrawText(
                    "YOU WIN!",
                    Color.Green,
                    _gameFont,
                    40,
                    _gameWindow.Width / 2 - 60,
                    100
                );
            }
            else
            {
                SplashKit.DrawText(
                    "GAME OVER",
                    Color.Red,
                    _gameFont,
                    40,
                    _gameWindow.Width / 2 - 80,
                    100
                );
            }

            // Draw final score
            SplashKit.DrawText(
                $"Final Score: {_score}",
                Color.White,
                _gameFont,
                30,
                _gameWindow.Width / 2 - 100,
                200
            );

            // Draw play again button
            Color buttonColor = _buttonHovered ? Color.Green : Color.DarkGreen;
            SplashKit.FillRectangle(buttonColor, _startButton);
            SplashKit.DrawRectangle(Color.White, _startButton);

            SplashKit.DrawText(
                "PLAY AGAIN",
                Color.White,
                _gameFont,
                35,
                _startButton.X + 25,
                _startButton.Y + 10
            );
        }

        private void StartGame()
        {
            ResetGame();
            _gameState = GameState.Playing;
        }

        private void ResetGame()
        {
            _lives = 3;
            _score = 0;

            // Reset map (convert 3s back to 2s)
            for (int i = 0; i < GameConstants.Map.GetLength(0); i++)
            {
                for (int j = 0; j < GameConstants.Map.GetLength(1); j++)
                {
                    if (GameConstants.Map[i, j] == 3)
                    {
                        GameConstants.Map[i, j] = 2;
                    }
                }
            }
            _map.Reset();
            CreateNewPacman();
            CreateGhosts();
        }

        private bool CheckWinCondition()
        {
            // Check if all food is eaten
            for (int i = 0; i < GameConstants.Map.GetLength(0); i++)
            {
                for (int j = 0; j < GameConstants.Map.GetLength(1); j++)
                {
                    if (GameConstants.Map[i, j] == 2) // If there's still food
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        private void DrawMuteButton()
        {
            // Draw button background
            if (_muteButtonHovered)
            {
                SplashKit.FillRectangle(Color.LightGray, _muteButton);
                SplashKit.DrawRectangle(Color.DarkGray, _muteButton);
            }

            // Calculate icon position (centered in button)
            double iconX = _muteButton.X + (_muteButton.Width - _soundOnIcon.Width) / 2;
            double iconY = _muteButton.Y + (_muteButton.Height - _soundOnIcon.Height) / 2;

            // Draw appropriate icon based on mute state
            if (_isMuted)
            {
                if (_soundOffIcon != null)
                {
                    _soundOffIcon.Draw(iconX, iconY);
                }
            }
            else
            {
                if (_soundOnIcon != null)
                {
                    _soundOnIcon.Draw(iconX, iconY);
                }
            }
        }

        private void DrawScore()
        {
            if (_gameFont == null)
                return;

            SplashKit.DrawText(
                "Score: " + _score,
                Color.White,
                _gameFont,
                20,
                50,
                GameConstants.Map.GetLength(0) * GameConstants.ONE_BLOCK_SIZE
            );
        }
    }
}
