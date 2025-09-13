using System;
using SplashKitSDK;

namespace pacman
{
    public class Map
    {
        private int[,] _layout;

        public Map()
        {
            _layout = (int[,])GameConstants.Map.Clone();
        }

        public int Rows => _layout.GetLength(0);
        public int Cols => _layout.GetLength(1);

        public int this[int row, int col]
        {
            get => _layout[row, col];
            set => _layout[row, col] = value;
        }

        public void Reset()
        {
            // Reset the map by GameConstants.Map
            for (int i = 0; i < Rows; i++)
            {
                for (int j = 0; j < Cols; j++)
                {
                    _layout[i, j] = GameConstants.Map[i, j];
                }
            }
        }

        // Consumes a dot at map cell (row, col).
        // Returns points awarded: 1 for regular dot, 10 for power-up, 0 for none.
        // Also sets the tile to 3 when consumed.
        public int ConsumeDotAt(int row, int col)
        {
            if (row < 0 || row >= Rows || col < 0 || col >= Cols) return 0;

            int tile = _layout[row, col];
            if (tile == 2)
            {
                _layout[row, col] = 3;
                return 1;
            }
            else if (tile == 4)
            {
                _layout[row, col] = 3;
                return 10;
            }

            return 0;
        }

        public void Draw()
        {
            DrawWalls();
            DrawDots();
        }

        private void DrawWalls()
        {
            for (int i = 0; i < Rows; i++)
            {
                for (int j = 0; j < Cols; j++)
                {
                    if (_layout[i, j] == 1)
                    {
                        SplashKit.FillRectangle(
                            GameConstants.WALL_COLOR,
                            j * GameConstants.ONE_BLOCK_SIZE,
                            i * GameConstants.ONE_BLOCK_SIZE,
                            GameConstants.ONE_BLOCK_SIZE,
                            GameConstants.ONE_BLOCK_SIZE
                        );

                        // inner details
                        if (j > 0 && _layout[i, j - 1] == 1)
                        {
                            SplashKit.FillRectangle(
                                GameConstants.WALL_INNER_COLOR,
                                j * GameConstants.ONE_BLOCK_SIZE,
                                i * GameConstants.ONE_BLOCK_SIZE + GameConstants.WALL_OFFSET,
                                GameConstants.WALL_SPACE_WIDTH + GameConstants.WALL_OFFSET,
                                GameConstants.WALL_SPACE_WIDTH
                            );
                        }

                        if (j < Cols - 1 && _layout[i, j + 1] == 1)
                        {
                            SplashKit.FillRectangle(
                                GameConstants.WALL_INNER_COLOR,
                                j * GameConstants.ONE_BLOCK_SIZE + GameConstants.WALL_OFFSET,
                                i * GameConstants.ONE_BLOCK_SIZE + GameConstants.WALL_OFFSET,
                                GameConstants.WALL_SPACE_WIDTH + GameConstants.WALL_OFFSET,
                                GameConstants.WALL_SPACE_WIDTH
                            );
                        }

                        if (i < Rows - 1 && _layout[i + 1, j] == 1)
                        {
                            SplashKit.FillRectangle(
                                GameConstants.WALL_INNER_COLOR,
                                j * GameConstants.ONE_BLOCK_SIZE + GameConstants.WALL_OFFSET,
                                i * GameConstants.ONE_BLOCK_SIZE + GameConstants.WALL_OFFSET,
                                GameConstants.WALL_SPACE_WIDTH,
                                GameConstants.WALL_SPACE_WIDTH + GameConstants.WALL_OFFSET
                            );
                        }

                        if (i > 0 && _layout[i - 1, j] == 1)
                        {
                            SplashKit.FillRectangle(
                                GameConstants.WALL_INNER_COLOR,
                                j * GameConstants.ONE_BLOCK_SIZE + GameConstants.WALL_OFFSET,
                                i * GameConstants.ONE_BLOCK_SIZE,
                                GameConstants.WALL_SPACE_WIDTH,
                                GameConstants.WALL_SPACE_WIDTH + GameConstants.WALL_OFFSET
                            );
                        }
                    }
                }
            }
        }

        private void DrawDots()
        {
            for (int i = 0; i < Rows; i++)
            {
                for (int j = 0; j < Cols; j++)
                {
                    if (_layout[i, j] == 2) // Regular food
                    {
                        Dot dot = new Dot(j, i, false);
                        dot.Draw();
                    }
                    else if (_layout[i, j] == 4) // Power-up
                    {
                        Dot dot = new Dot(j, i, true);
                        dot.Draw();
                    }
                }
            }
        }
    }
}
