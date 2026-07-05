# ♟️ Console Chess Engine & Game

A fully playable chess game built in C# as a lightweight **.NET 10 console application**.

This project was created as both a functional chess game and an educational exercise for practicing modern C#, object-oriented design, chess rule implementation, game state management, console UI design, external engine integration, and custom testing infrastructure.

The game features a terminal-based presentation layer, local pass-and-play mode, player vs computer mode powered by **Stockfish**, live match information, special chess rules, draw condition validation, and a custom lightweight test runner.

---

## 🚀 Features

### ♟️ Core Game Logic

* **Standard Chess Rules:** Implements legal chess movement rules for all pieces.
* **Special Moves:** Supports castling, en passant, and pawn promotion.
* **Check Detection:** Detects when a king is currently in check.
* **Checkmate & Stalemate:** Evaluates whether the active player has any legal moves remaining.
* **Move History:** Tracks played moves and uses historical information for rule validation.
* **Turn Management:** Maintains the current player turn and prevents illegal out-of-turn moves.
* **Draw Conditions:** Supports Fifty-Move Rule, Threefold Repetition, and Insufficient Material detection.

### 🖥️ Console UI

* **Console Board Rendering:** Displays the current board state directly in the terminal.
* **Dynamic Sidebar Dashboard:** Shows captured pieces using Unicode chess symbols.
* **Live Material Evaluation:** Displays material advantage during the match.
* **Input Parsing:** Converts player commands like `e2 e4` into board positions.
* **Main Menu:** Supports local play, player vs computer, and application exit.
* **Resignation Support:** Allows the player to type `quit` to resign.

### 🤖 Stockfish Integration

Player vs computer mode is powered by **Stockfish**, connected through the **UCI** protocol.

The project does not use a custom Minimax AI anymore. Instead, the application:

1. Converts the current internal game state into FEN.
2. Sends that FEN position to Stockfish.
3. Requests a best move using UCI commands.
4. Parses the returned UCI move.
5. Executes the move through the project’s own move validation and execution pipeline.

This keeps the project responsible for its own game rules and board state while delegating computer move selection to a real chess engine.

### AI Difficulty Tiers

Difficulty is controlled by Stockfish strength configuration and move time.

| Difficulty   | Intended Feel              |
| ------------ | -------------------------- |
| Beginner     | Very easy / fast responses |
| Intermediate | Casual chess opponent      |
| Advanced     | Solid tactical opponent    |
| Master       | Hard but beatable          |
| Grandmaster  | Very hard Stockfish play   |

Exact strength may vary depending on the Stockfish version and local machine performance.

---

## 🧪 Testing & Validation

### Custom Unit Testing Infrastructure

This project uses a custom lightweight test runner instead of xUnit, NUnit, or MSTest.

The goal is educational: the testing infrastructure was built manually to better understand how test suites, assertions, result aggregation, and test execution work under the hood.

The test suite covers critical parts of the chess application, including:

* **Board Tests:** Validate board initialization, piece placement, board queries, and movement behavior.
* **Draw Tests:** Validate Fifty-Move Rule, Threefold Repetition, and Insufficient Material scenarios.
* **Engine Tests:** Validate FEN conversion and Stockfish-supporting engine utilities.
* **Game State Tests:** Validate checkmate, stalemate, and game state transitions.
* **Move Tests:** Validate move representation and movement-related behavior.
* **Notation Tests:** Validate input parsing and algebraic coordinate conversion.

To run the full test suite:

```bash
dotnet run --project ChessConsoleApp.csproj -- --run-tests
```

---

## 📂 Project Architecture

```text
ChessConsoleApp/
│
├── engines/
│   └── stockfish/
│       ├── .gitkeep
│       ├── README
│       └── stockfish.exe          # Local dependency, not committed to the repository
│
├── src/
│   │
│   ├── Core/                      # Core game orchestration and state management
│   │   ├── GameEngine.cs
│   │   ├── GameSession.cs
│   │   │
│   │   ├── Moves/                 # Move validation, execution, and promotion handling
│   │   │   ├── MoveValidator.cs
│   │   │   ├── MoveExecutor.cs
│   │   │   ├── MoveValidationResult.cs
│   │   │   ├── MoveExecutionResult.cs
│   │   │   └── PromotionPieceFactory.cs
│   │   │
│   │   └── State/                 # Game state evaluation and position tracking
│   │       ├── GameStateEvaluator.cs
│   │       └── PositionSnapshotService.cs
│   │
│   ├── Engine/                    # Stockfish integration and chess engine adapter layer
│   │   ├── FenConverter.cs
│   │   ├── StockfishOptions.cs
│   │   ├── StockfishUciClient.cs
│   │   └── StockfishChessAi.cs
│   │
│   ├── Enums/                     # GameState, PieceColor, AiDifficulty, and related enums
│   │
│   ├── Models/                    # Board, Position, Move, and chess piece models
│   │   └── Pieces/                # Individual chess piece entities and movement rules
│   │
│   ├── UI/                        # Console presentation and user input layer
│   │   ├── ConsoleRenderer.cs
│   │   ├── EndGameRenderer.cs
│   │   ├── GameMenu.cs
│   │   ├── InputParser.cs
│   │   └── PromotionPrompt.cs
│   │
│   └── Program.cs                 # Application entry point
│
├── Tests/
│   │
│   ├── BoardTests/                # Validate board initialization, piece placement, and movement behavior
│   ├── DrawTests/                 # Validate draw conditions: Fifty-Move Rule, Threefold Repetition, Insufficient Material
│   ├── EngineTests/               # Validate FEN conversion and Stockfish-supporting engine utilities
│   ├── GameStateTests/            # Validate checkmate, stalemate, and game state transitions
│   ├── MoveTests/                 # Validate move representation and movement-related behavior
│   ├── NotationTests/             # Validate input parsing and algebraic coordinate conversion
│   └── TestSuite.cs               # Aggregate test runner for all test categories
│
├── ChessConsoleApp.csproj
├── .gitignore
└── README.md
```

---

## 🛠️ Getting Started

### Prerequisites

* **.NET 10 SDK**
* **Stockfish executable**

This project intentionally targets **.NET 10** because one of its goals is to practice and get comfortable with newer versions of the .NET platform after working with previous versions such as .NET 6 and .NET 8.

Player vs computer mode requires a local Stockfish executable. The expected path is:

```text
engines/stockfish/stockfish.exe
```

The Stockfish executable itself is not committed to the repository.

To configure it:

1. Download a Stockfish build for your operating system.
2. Rename the executable to stockfish.exe.
3. Place it inside engines/stockfish/.

The final structure should look like this:

```text
engines/
└── stockfish/
    ├── .gitkeep
    ├── README
    └── stockfish.exe
```

---

## 📦 Installation & Run

Clone this repository to your local machine:

```bash
git clone https://github.com/jvaguiar05/ChessConsoleApp.git
cd ChessConsoleApp
```

Restore dependencies:

```bash
dotnet restore
```

Build the project:

```bash
dotnet build
```

Run the application:

```bash
dotnet run --project ChessConsoleApp.csproj
```

---

## 🧪 Running the Test Suite

The project includes a custom automated test suite that can be executed through the console application itself.

```bash
dotnet run --project ChessConsoleApp.csproj -- --run-tests
```

---

## 🎮 How to Play

### Main Menu

When the application starts, you can choose between:

* **Pass-and-Play:** Two-player local chess.
* **Play vs Computer:** Play against the built-in AI.
* **Exit Game:** Close the application.

### Entering Moves

Moves are entered using space-separated coordinate notation:

```text
e2 e4
g1 f3
e7 e5
```

The first coordinate is the source square, and the second coordinate is the destination square.

### Resigning

Type the following command during your turn to resign:

```text
quit
```

The game will declare the opponent as the winner and return safely to the main menu.

---

## 🧭 Current Development Focus

The project is still evolving. Current architectural priorities include:

* Keeping the Stockfish integration stable and isolated from core chess rules.
* Improving difficulty tuning so each AI level feels meaningfully different.
* Expanding UCI move parsing coverage for advanced edge cases.
* Keeping the console UI clean as the engine layer grows.
* Keeping project documentation synchronized with the actual code structure.
* Preserving the custom test infrastructure while improving the core design.

The goal is not only to make the chess game work, but to make the codebase easier to reason about, test, refactor, and extend.

---

## 🔮 Future Improvements

Potential future improvements include:

* Improving AI difficulty balancing.
* Adding stronger fallback/error messages when Stockfish is missing.
* Supporting configurable Stockfish paths.
* Expanding UCI move parsing coverage for advanced edge cases.
* Adding PGN-style move history.
* Adding save/load support.
* Adding undo/redo support.
* Optionally adding an xUnit test project later for comparison with the custom test runner.
* Improving packaging so Stockfish setup is clearer for end users.

---

## 📜 License

This project is open-source and available under the MIT License.
