using System;
using SplashKitSDK;

namespace pacman
{
    public class Ghost : Character
    {
        public int Range { get; set; }

        public int RandomTargetIndex { get; set; }
        public Point2D Target { get; set; }

        private DateTime _lastDirectionChangeTime;
        private readonly TimeSpan _directionChangeInterval = TimeSpan.FromSeconds(10);

        private Pacman _pacman;
        private Color _ghostColor;
        private IMovementStrategy _movementStrategy;

        public Ghost(
            double x,
            double y,
            double width,
            double height,
            double speed,
            int range,
            Pacman pacman,
            Color ghostColor,
            IMovementStrategy movementStrategy
        )
            : base(x, y, width, height, speed)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
            Speed = speed;
            Direction = GameConstants.DIRECTION_RIGHT;
            Range = range;
            _pacman = pacman;
            _ghostColor = ghostColor;
            _movementStrategy = movementStrategy;
            _lastDirectionChangeTime = DateTime.Now;
        }

        public bool IsInRange()
        {
            double xDistance = Math.Abs(_pacman.GetMapX() - GetMapX());
            double yDistance = Math.Abs(_pacman.GetMapY() - GetMapY());

            return Math.Sqrt(xDistance * xDistance + yDistance * yDistance) <= Range;
        }

        public void ChangeRandomDirection()
        {
            if (DateTime.Now - _lastDirectionChangeTime > _directionChangeInterval)
            {
                RandomTargetIndex = (RandomTargetIndex + 1) % 4;
                _lastDirectionChangeTime = DateTime.Now;
            }
        }

        public override void MoveProcess()
        {
            // Decide movement strategy dynamically
            if (_pacman.CanEatGhosts)
            {
                _movementStrategy = new FleePacmanStrategy();
            }
            else if (IsInRange())
            {
                _movementStrategy = new ChasePacmanStrategy();
            }
            else
            {
                _movementStrategy = new RandomMovementStrategy();
            }

            Target = _movementStrategy.GetNextTarget(this, _pacman, GameConstants.Map);

            ChangeDirectionIfPossible();
            MoveForwards();

            if (CheckCollisions())
            {
                MoveBackwards();
            }
        }

        private Point2D GetFleeTarget()
        {
            // Run to the opposite side of the map from Pacman
            double fleeX =
                (_pacman.X < GameConstants.ONE_BLOCK_SIZE * 10)
                    ? GameConstants.ONE_BLOCK_SIZE * 19
                    : GameConstants.ONE_BLOCK_SIZE * 1;

            double fleeY =
                (_pacman.Y < GameConstants.ONE_BLOCK_SIZE * 10)
                    ? GameConstants.ONE_BLOCK_SIZE * 19
                    : GameConstants.ONE_BLOCK_SIZE * 1;

            return new Point2D { X = fleeX, Y = fleeY };
        }

        public override bool CheckCollisions()
        {
            bool isCollided = false;
            if (
                GameConstants.Map[GetMapY(), GetMapX()] == 1
                || GameConstants.Map[(int)(Y / GameConstants.ONE_BLOCK_SIZE + 0.9999), GetMapX()]
                    == 1
                || GameConstants.Map[GetMapY(), (int)(X / GameConstants.ONE_BLOCK_SIZE + 0.9999)]
                    == 1
                || GameConstants.Map[
                    (int)(Y / GameConstants.ONE_BLOCK_SIZE + 0.9999),
                    (int)(X / GameConstants.ONE_BLOCK_SIZE + 0.9999)
                ] == 1
            )
            {
                isCollided = true;
            }
            return isCollided;
        }

        public void ChangeDirectionIfPossible()
        {
            int tempDirection = Direction;
            Direction = CalculateNewDirection(
                (int)(Target.X / GameConstants.ONE_BLOCK_SIZE),
                (int)(Target.Y / GameConstants.ONE_BLOCK_SIZE)
            );

            if (Direction == -1) // Invalid direction
            {
                Direction = tempDirection;
                return;
            }

            if (
                GetMapY() != GetMapYRightSide()
                && (
                    Direction == GameConstants.DIRECTION_LEFT
                    || Direction == GameConstants.DIRECTION_RIGHT
                )
            )
            {
                Direction = GameConstants.DIRECTION_UP;
            }

            if (GetMapX() != GetMapXRightSide() && Direction == GameConstants.DIRECTION_UP)
            {
                Direction = GameConstants.DIRECTION_LEFT;
            }

            MoveForwards();
            if (CheckCollisions())
            {
                MoveBackwards();
                Direction = tempDirection;
            }
            else
            {
                MoveBackwards();
            }
        }

        public int CalculateNewDirection(int destX, int destY)
        {
            int[,] mp = (int[,])GameConstants.Map.Clone();

            Queue<PathNode> queue = new Queue<PathNode>();
            queue.Enqueue(
                new PathNode
                {
                    X = GetMapX(),
                    Y = GetMapY(),
                    Moves = new List<int>(),
                }
            );

            while (queue.Count > 0)
            {
                PathNode popped = queue.Dequeue();

                if (popped.X == destX && popped.Y == destY)
                {
                    return popped.Moves.Count > 0 ? popped.Moves[0] : Direction;
                }
                else
                {
                    mp[popped.Y, popped.X] = 1;
                    List<PathNode> neighbors = AddNeighbors(popped, mp);

                    foreach (PathNode neighbor in neighbors)
                    {
                        queue.Enqueue(neighbor);
                    }
                }
            }

            return Direction; // Return current direction if no path found
        }

        private List<PathNode> AddNeighbors(PathNode popped, int[,] mp)
        {
            List<PathNode> neighbors = new List<PathNode>();
            int numOfRows = mp.GetLength(0);
            int numOfColumns = mp.GetLength(1);

            // Left
            if (popped.X - 1 >= 0 && mp[popped.Y, popped.X - 1] != 1)
            {
                List<int> tempMoves = new List<int>(popped.Moves);
                tempMoves.Add(GameConstants.DIRECTION_LEFT);
                neighbors.Add(
                    new PathNode
                    {
                        X = popped.X - 1,
                        Y = popped.Y,
                        Moves = tempMoves,
                    }
                );
            }

            // Right
            if (popped.X + 1 < numOfColumns && mp[popped.Y, popped.X + 1] != 1)
            {
                List<int> tempMoves = new List<int>(popped.Moves);
                tempMoves.Add(GameConstants.DIRECTION_RIGHT);
                neighbors.Add(
                    new PathNode
                    {
                        X = popped.X + 1,
                        Y = popped.Y,
                        Moves = tempMoves,
                    }
                );
            }

            // Up
            if (popped.Y - 1 >= 0 && mp[popped.Y - 1, popped.X] != 1)
            {
                List<int> tempMoves = new List<int>(popped.Moves);
                tempMoves.Add(GameConstants.DIRECTION_UP);
                neighbors.Add(
                    new PathNode
                    {
                        X = popped.X,
                        Y = popped.Y - 1,
                        Moves = tempMoves,
                    }
                );
            }

            // Down
            if (popped.Y + 1 < numOfRows && mp[popped.Y + 1, popped.X] != 1)
            {
                List<int> tempMoves = new List<int>(popped.Moves);
                tempMoves.Add(GameConstants.DIRECTION_BOTTOM);
                neighbors.Add(
                    new PathNode
                    {
                        X = popped.X,
                        Y = popped.Y + 1,
                        Moves = tempMoves,
                    }
                );
            }

            return neighbors;
        }

        public int GetMapXRightSide()
        {
            return (int)((X * 0.99 + GameConstants.ONE_BLOCK_SIZE) / GameConstants.ONE_BLOCK_SIZE);
        }

        public int GetMapYRightSide()
        {
            return (int)((Y * 0.99 + GameConstants.ONE_BLOCK_SIZE) / GameConstants.ONE_BLOCK_SIZE);
        }

        public void Draw()
        {
            Color ghostColor = _pacman.CanEatGhosts ? Color.Blue : _ghostColor;

            // Draw a colored circle for the ghost
            SplashKit.FillCircle(ghostColor, X + Width / 2, Y + Height / 2, Width / 2);

            // Draw eyes
            SplashKit.FillCircle(Color.White, X + Width / 3, Y + Height / 3, Width / 6);
            SplashKit.FillCircle(Color.White, X + 2 * Width / 3, Y + Height / 3, Width / 6);

            // Draw pupils (looking in direction of movement)
            double pupilOffsetX = 0;
            double pupilOffsetY = 0;

            switch (Direction)
            {
                case GameConstants.DIRECTION_RIGHT:
                    pupilOffsetX = Width / 10;
                    break;
                case GameConstants.DIRECTION_LEFT:
                    pupilOffsetX = -Width / 10;
                    break;
                case GameConstants.DIRECTION_UP:
                    pupilOffsetY = -Width / 10;
                    break;
                case GameConstants.DIRECTION_BOTTOM:
                    pupilOffsetY = Width / 10;
                    break;
            }

            SplashKit.FillCircle(
                Color.Black,
                X + Width / 3 + pupilOffsetX,
                Y + Height / 3 + pupilOffsetY,
                Width / 12
            );
            SplashKit.FillCircle(
                Color.Black,
                X + 2 * Width / 3 + pupilOffsetX,
                Y + Height / 3 + pupilOffsetY,
                Width / 12
            );

            // Draw range circle (for debugging)
            // SplashKit.DrawCircle(Color.Red, X + Width / 2, Y + Height / 2, Range * GameConstants.ONE_BLOCK_SIZE);
        }
    }

    public class PathNode
    {
        public int X { get; set; }
        public int Y { get; set; }
        public List<int> Moves { get; set; } = new List<int>();
    }
}
