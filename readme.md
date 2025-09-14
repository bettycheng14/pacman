### Tutorial: Building a Data-Driven Pac-Man Game with Level Editor
This repository is about a Pac-Man game with a single static level building with C# and Splashkit. By adding a **Level Editor**, for users to create a customize map and to play on their own maps. This transforms the game from a static experience into an interactive, data-driven platform for creativity.

This tutorial demonstrates advanced object-oriented programming principles, including the **Strategy Pattern, Separation of Concerns, Encapsulation, and Data-Driven Design**. It also showcases iterative development, problem-solving, and polish in both gameplay and architecture.

Full Coding: https://github.com/bettycheng14/pacman

Demostration: https://deakin.au.panopto.com/Panopto/Pages/Viewer.aspx?id=d816bef4-56c9-4adc-90e6-b357013099d8

### 1.	Architectural Overview

The game follows a component-based architecture with clear separation of responsibilities.

Core Components involved in this feature:

* `PacmanGame`: Main controller on game states and flow
* `Map`: Handle Game grid data, rendering and dot consumption
* `Character`: Abstract base class for `Pacman` and `Ghost` entities.
* `LevelEditor`: *(New)* Provide Map editing functionality
* `IMovementStrategy`: Interface for ghost AI behaviours
This design makes the game flexible, testable, and easy to extend.

### 2. Add a Game State for Map editor for control Game Screen
To integrate the editor seamlessly, I extended the `GameState` enum:
```csharp
public enum GameState
{
    StartScreen,    // Main menu
    Playing,        // Gameplay
    GameOver,       // Win/Lose screen
    LevelEditing,   // NEW: Map editor
}
```
This ensures each screen has a dedicated workflow.

### 3. Data-Driven Map Design
#### Key Features:
The `Map` class stores level layouts in a 2D array and supports persistence.
```csharp
public class Map
{
    public int[,] _layout { get; private set; }
    public bool IsCustomMap { get; set; }
    
    public int this[int row, int col]  // Indexer for easy access
    {
        get => _layout[row, col];
        set => _layout[row, col] = value;
    }
}
```

#### Serialization Example:
This demonstrates how game data can be saved with StreamWriter, making levels customizable and shareable.
```csharp
public void SaveMap(string filename)
{
    using (StreamWriter writer = new StreamWriter(filename))
    {
        for (int i = 0; i < Rows; i++)
        {
            for (int j = 0; j < Cols; j++)
            {
                writer.Write(_map[i, j] + " ");  // Space-separated values
            }
            writer.WriteLine();  // New line for each row
        }
    }
}
```

### 4. Level Editor Implementation
#### Editor Features:
* Tile Painting: Place of walls, dots, and power-ups via mouse clicks
* Keyboard Controls: Number keys for tile selection, `S`/`L` for save/load, `P` for Play with current Map, `Ese` to Exit
* Real-time Preview: Immediate Visual feedback of changes on `Map`

```csharp
public void HandleInput()
{
    // [1] Tile Selection via Number Keys
    if (SplashKit.KeyTyped(KeyCode.Num1Key)) _selectedTileId = 1; // Wall
    if (SplashKit.KeyTyped(KeyCode.Num2Key)) _selectedTileId = 2; // Dot
    if (SplashKit.KeyTyped(KeyCode.Num3Key)) _selectedTileId = 4; // PowerUp
    if (SplashKit.KeyTyped(KeyCode.Num0Key)) _selectedTileId = 0; // Eraser
    
    // Tile placement by mouse click
    if (SplashKit.MouseDown(MouseButton.LeftButton))
    {
        Point2D mousePos = SplashKit.MousePosition();
        int col = (int)(mousePos.X / GameConstants.ONE_BLOCK_SIZE);
        int row = (int)(mousePos.Y / GameConstants.ONE_BLOCK_SIZE);
        
        if (row >= 0 && row < _map.Rows && col >= 0 && col < _map.Cols)
        {
            _map[row, col] = _selectedTileId;  // Modify the map
        }
    }
}
```
**Technical Challenge**: Mapping screen coordinates to grid cells required careful handling of integer division and bounds checking.


### 5. Ghost AI: Strategy Pattern Implementation
`IMovementStrategy` Interface:
```csharp
public interface IMovementStrategy
{
    Point2D GetNextTarget(Ghost ghost, Pacman pacman, int[,] map);
}
```

#### Concrete Strategies:
* ChasePacmanStrategy: Targets Pac-Man's position
* FleePacmanStrategy: Moves to opposite corner from Pac-Man
* RandomMovementStrategy: Cycles between map corners

#### Dynamic Behavior Switching:
This demonstrates runtime behavior switching without modifying core classes.
```csharp
public override void MoveProcess()
{
    // Dynamic strategy selection
    if (_pacman.CanEatGhosts)
        _movementStrategy = new FleePacmanStrategy();
    else if (IsInRange())
        _movementStrategy = new ChasePacmanStrategy();
    else
        _movementStrategy = new RandomMovementStrategy();

    Target = _movementStrategy.GetNextTarget(this, _pacman, Map._layout);
}
```

### 6. Pathfinding Algorithm
The ghosts use *Breadth-First Search (BFS)* to ensure shortest paths on the grid:
```csharp
public int CalculateNewDirection(int destX, int destY)
{
    int[,] mp = (int[,])Map._layout.Clone();
    Queue<PathNode> queue = new Queue<PathNode>();
    
    queue.Enqueue(new PathNode { X = GetMapX(), Y = GetMapY(), Moves = new List<int>() });

    while (queue.Count > 0)
    {
        PathNode popped = queue.Dequeue();
        
        if (popped.X == destX && popped.Y == destY)
            return popped.Moves.Count > 0 ? popped.Moves[0] : Direction;
        // ... process neighbors
    }
    return Direction;
}
```
**Algorithm Choice**: BFS was selected because it guarantees the shortest path on an unweighted grid.

### 7. Seamless Editor-Game Integration
Editor and gameplay interact via robust state management:
```csharp
case GameState.LevelEditing:
    _levelEditor.HandleInput();
    if (SplashKit.KeyTyped(KeyCode.EscapeKey))
        _gameState = GameState.StartScreen;
    if (SplashKit.KeyTyped(KeyCode.PKey))  // Playtest on current map
    {
        _map.IsCustomMap = true;
        _gameState = GameState.Playing;
        ResetGame(forceOriginalMap: false);  // Keep custom layout
    }
    break;
```
#### Reset Game:
```csharp
private void ResetGame(bool forceOriginalMap = false)
{
    _lives = 3;
    _score = 0;
    
    // Preserve custom maps during playtesting
    if (forceOriginalMap || !_map.IsCustomMap)
        _map.Reset();  // Reset to original
    
    CreateNewPacman();
    CreateGhosts();
}
```

### 8. Polish and User Experience
* Visual Feedback: Flashing power-ups, color-changing ghosts, and live tile previews
* Error Handling: Graceful failures on resource loading
* Player Guidance: On-screen hints for editor controls


### 9. Development Challenges and Solutions
* Encapsulation Issues: Solved by passing Map objects instead of raw arrays
* State Management Bugs: Fixed with parameterized reset logic (forceOriginalMap)
* Coordinate Conversion Errors: Eliminated with bounds checks and tested division logic

### 10. Key Programming Principles Demonstrated
* Single Responsibility Principle: Each class has a focused role
* Open/Closed Principle: Ghost behaviors extended without modifying core code
* Dependency Injection: Characters depend on Map through constructors, improving testability
* Encapsulation: Internal map details hidden behind a clean public API

## Conclusion
This project demonstrates how object-oriented principles can be applied in a game architecture. This can allows us to create a flexible, extensible, and user-centric game system.

### Key Takeaways:

* Strategy Pattern = adaptive AI behaviors
* Data-Driven Design = customizable and persistent maps
* State Management = seamless transitions across modes
* Serialization = sharable, community-driven content