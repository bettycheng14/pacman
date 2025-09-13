using System;
using SplashKitSDK;

namespace pacman
{
    public class Dot
    {
        private int _row;
        private int _col;
        private bool _isPowerUp;

        public Dot(int col, int row, bool isPowerUp)
        {
            _col = col;
            _row = row;
            _isPowerUp = isPowerUp;
        }

        public void Draw()
        {
            if (_isPowerUp)
            {
                double size = GameConstants.ONE_BLOCK_SIZE / 2;
                double offset = (GameConstants.ONE_BLOCK_SIZE - size) / 2;

                if ((DateTime.Now.Millisecond / 250) % 2 == 0) // toggles 4 times per sec
                {
                    SplashKit.FillCircle(
                        Color.CornflowerBlue,
                        _col * GameConstants.ONE_BLOCK_SIZE + offset + size / 2,
                        _row * GameConstants.ONE_BLOCK_SIZE + offset + size / 2,
                        size / 2
                    );
                }
            }
            else
            {
                SplashKit.FillRectangle(
                    GameConstants.FOOD_COLOR,
                    _col * GameConstants.ONE_BLOCK_SIZE + GameConstants.ONE_BLOCK_SIZE / 3,
                    _row * GameConstants.ONE_BLOCK_SIZE + GameConstants.ONE_BLOCK_SIZE / 3,
                    GameConstants.ONE_BLOCK_SIZE / 3,
                    GameConstants.ONE_BLOCK_SIZE / 3
                );
            }
        }
    }
}
