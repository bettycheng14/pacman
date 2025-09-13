using SplashKitSDK;

namespace pacman
{
    public static class GameConstants
    {
        public const int DIRECTION_RIGHT = 4;
        public const int DIRECTION_UP = 3;
        public const int DIRECTION_LEFT = 2;
        public const int DIRECTION_BOTTOM = 1;

        public const int FPS = 30;
        public const int ONE_BLOCK_SIZE = 20;
        public const int WALL_SPACE_WIDTH = ONE_BLOCK_SIZE / 2;
        public const int WALL_OFFSET = (ONE_BLOCK_SIZE - WALL_SPACE_WIDTH) / 2;

        public static readonly Color WALL_INNER_COLOR = Color.Black;
        public static readonly Color WALL_COLOR = Color.RGBColor(0.2f, 0.18f, 0.79f); // #342DCA
        public static readonly Color FOOD_COLOR = Color.RGBColor(0.996f, 0.722f, 0.592f); // #FEB897

        // Ghost image locations (assuming a sprite sheet)
        public static readonly Point2D[] GhostImageLocations =
        {
            new Point2D { X = 0, Y = 0 },
            new Point2D { X = 176, Y = 0 },
            new Point2D { X = 0, Y = 121 },
            new Point2D { X = 176, Y = 121 },
        };

        // Random targets for ghosts
        public static Point2D[] RandomTargetsForGhosts = new Point2D[4];

        // Game map
        public static int[,] Map =
        {
            { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 },
            { 1, 2, 2, 2, 2, 2, 2, 2, 2, 2, 1, 2, 2, 2, 2, 2, 2, 2, 2, 2, 1 },
            { 1, 2, 1, 1, 1, 2, 1, 1, 1, 2, 1, 2, 1, 1, 1, 2, 1, 1, 1, 2, 1 },
            { 1, 4, 1, 1, 1, 2, 1, 1, 1, 2, 1, 2, 1, 1, 1, 2, 1, 1, 1, 2, 1 },
            { 1, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 4, 2, 1 },
            { 1, 2, 1, 1, 1, 2, 1, 2, 1, 1, 1, 1, 1, 2, 1, 2, 1, 1, 1, 2, 1 },
            { 1, 2, 2, 2, 2, 2, 1, 2, 2, 2, 1, 2, 2, 2, 1, 2, 2, 2, 2, 2, 1 },
            { 1, 1, 1, 1, 1, 2, 1, 1, 1, 2, 1, 2, 1, 1, 1, 2, 1, 1, 1, 1, 1 },
            { 1, 0, 0, 0, 1, 2, 1, 2, 2, 2, 2, 2, 2, 2, 1, 2, 1, 0, 0, 0, 1 },
            { 1, 1, 1, 1, 1, 4, 1, 2, 1, 1, 2, 1, 1, 2, 1, 2, 1, 1, 1, 1, 1 },
            { 1, 2, 2, 2, 2, 2, 2, 2, 1, 2, 2, 2, 1, 2, 2, 2, 2, 2, 2, 4, 1 },
            { 1, 1, 1, 1, 1, 2, 1, 2, 1, 2, 2, 2, 1, 2, 1, 2, 1, 1, 1, 1, 1 },
            { 1, 0, 0, 0, 1, 2, 1, 2, 1, 1, 1, 1, 1, 2, 1, 2, 1, 0, 0, 0, 1 },
            { 1, 0, 0, 0, 1, 2, 1, 2, 2, 2, 2, 2, 2, 2, 1, 2, 1, 0, 0, 0, 1 },
            { 1, 1, 1, 1, 1, 2, 2, 2, 1, 1, 1, 1, 1, 2, 2, 2, 1, 1, 1, 1, 1 },
            { 1, 2, 2, 2, 2, 2, 2, 2, 2, 2, 1, 2, 2, 2, 2, 2, 2, 2, 2, 2, 1 },
            { 1, 2, 1, 1, 1, 2, 1, 1, 1, 2, 1, 2, 1, 1, 1, 2, 1, 1, 1, 2, 1 },
            { 1, 2, 2, 2, 1, 2, 2, 2, 2, 2, 1, 2, 2, 2, 2, 2, 1, 2, 2, 2, 1 },
            { 1, 1, 2, 2, 1, 2, 1, 2, 1, 1, 1, 1, 1, 2, 1, 2, 1, 2, 2, 1, 1 },
            { 1, 2, 2, 2, 2, 2, 1, 2, 2, 2, 1, 2, 2, 2, 1, 2, 2, 2, 4, 2, 1 },
            { 1, 2, 1, 1, 1, 1, 1, 1, 1, 2, 1, 2, 1, 1, 1, 1, 1, 1, 1, 2, 1 },
            { 1, 2, 4, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 1 },
            { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 },
        };

        public static void Initialize()
        {
            // Initialize random targets for ghosts
            RandomTargetsForGhosts[0] = new Point2D
            {
                X = 1 * ONE_BLOCK_SIZE,
                Y = 1 * ONE_BLOCK_SIZE,
            };
            RandomTargetsForGhosts[1] = new Point2D
            {
                X = 1 * ONE_BLOCK_SIZE,
                Y = (Map.GetLength(0) - 2) * ONE_BLOCK_SIZE,
            };
            RandomTargetsForGhosts[2] = new Point2D
            {
                X = (Map.GetLength(1) - 2) * ONE_BLOCK_SIZE,
                Y = 1 * ONE_BLOCK_SIZE,
            };
            RandomTargetsForGhosts[3] = new Point2D
            {
                X = (Map.GetLength(1) - 2) * ONE_BLOCK_SIZE,
                Y = (Map.GetLength(0) - 2) * ONE_BLOCK_SIZE,
            };
        }
    }
}
