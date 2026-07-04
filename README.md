# ♟️ Console Chess Engine & Game

A fully playable chess game built entirely in C# as a lightweight **.NET 10 console application**.

This project was created as both a functional chess game and an educational exercise for practicing modern C#, object-oriented design, game state management, chess rule implementation, AI search, and custom unit testing infrastructure.

The game features a terminal-based presentation layer, local pass-and-play mode, player vs computer mode, live match information, special chess rules, draw condition validation, and a self-contained AI opponent powered by Minimax search with Alpha-Beta pruning.

---

## 🚀 Features

### ♟️ Core Game Engine

* **Chess Rule Support:** Implements standard chess movement rules, including special moves and game-ending states.
* **Special Moves:** Supports Castling, En Passant, and Pawn Promotion.
* **Check Detection:** Detects when a king is currently in check.
* **Checkmate & Stalemate:** Evaluates whether the active player has any legal moves remaining.
* **Move History:** Tracks played moves and uses historical information for rule validation.
* **Turn Management:** Maintains the current player turn and prevents illegal out-of-turn moves.

### 🤝 Draw Conditions

The engine includes validation for multiple draw scenarios:

* **Fifty-Move Rule:** Detects when no pawn move or capture has occurred within the required move window.
* **Threefold Repetition:** Uses position snapshots to identify repeated board states.
* **Insufficient Material:** Detects common layouts where neither side has enough material to force checkmate.

### 🖥️ Presentation Layer (`src/UI/`)

* **Console Board Rendering:** Displays the current board state directly in the terminal.
* **Dynamic Sidebar Dashboard:** Shows captured pieces using Unicode chess symbols.
* **Live Material Evaluation:** Displays material advantage during the match.
* **Input Parsing:** Converts player commands like `e2 e4` into board positions.
* **Resignation Support:** Allows the player to type `quit` to resign and return to the main menu.

### 🤖 Artificial Intelligence Layer (`src/AI/`)

* **Minimax Search:** Evaluates future move sequences through a decision tree.
* **Alpha-Beta Pruning:** Skips branches that cannot improve the current result.
* **Piece-Square Tables:** Adds positional awareness to the AI evaluation.
* **Material Evaluation:** Scores board positions based on remaining pieces.
* **Difficulty Tiers:** Adjusts search depth and behavior depending on selected difficulty.

#### AI Difficulty Tiers

| Difficulty   | Search Depth | Behavior                                 |
| ------------ | -----------: | ---------------------------------------- |
| Beginner     |        1 ply | Basic play with a planned blunder chance |
| Intermediate |        3 ply | Casual tactical play                     |
| Advanced     |        4 ply | Stronger tactical search                 |
| Master       |        5 ply | Deeper calculation                       |
| Grandmaster  |        6 ply | Most demanding search mode               |

> The AI is intentionally implemented inside the project instead of relying on an external chess engine. This keeps the project useful for studying board evaluation, Minimax, Alpha-Beta pruning, and chess engine architecture.

---

## 🧪 Testing & Validation

### Custom Unit Testing Infrastructure

This project uses a custom lightweight test runner instead of xUnit, NUnit, or MSTest.

The goal is educational: the testing infrastructure was built manually to better understand how test suites, assertions, result aggregation, and test execution work under the hood.

The test suite covers critical parts of the chess engine, including:

* **Board Tests:** Validate board initialization, piece placement, board queries, and movement behavior.
* **Draw Tests:** Validate Fifty-Move Rule, Threefold Repetition, and Insufficient Material scenarios.
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
├── src/
│   │
│   ├── AI/                         # --- Artificial Intelligence Layer ---
│   │   ├── ChessAi.cs              # Minimax search, Alpha-Beta pruning, and difficulty behavior
│   │   └── BoardEvaluator.cs       # Material evaluation and Piece-Square Tables
│   │
│   ├── Core/                       # --- Core Game Logic ---
│   │   ├── Board.cs                # Board representation, indexing, movement, and board queries
│   │   └── GameEngine.cs           # Main game orchestration and state-machine flow
│   │
│   ├── Enums/                      # GameState, PieceColor, and AiDifficulty configurations
│   │
│   ├── Models/                     # Position, Move, and chess piece models
│   │   └── Pieces/                 # Individual piece entities and movement rules
│   │
│   ├── UI/                         # --- Presentation Layer ---
│   │   ├── ConsoleRenderer.cs      # Board drawing, Unicode sidebar, and material display
│   │   └── InputParser.cs          # Converts commands like "e2 e4" into board positions
│   │
│   └── Program.cs                  # Application entry point
│
├── Tests/
│   │
│   ├── BoardTests/                 # Unit tests for board behavior and move validation
│   ├── DrawTests/                  # Unit tests for draw conditions
│   ├── GameStateTests/             # Unit tests for checkmate, stalemate, and state transitions
│   ├── MoveTests/                  # Unit tests for move representation
│   ├── NotationTests/              # Unit tests for input parsing and algebraic coordinates
│   └── TestSuite.cs                # Central test runner for executing all test suites
│
├── ChessConsoleApp.csproj
└── README.md
```

---

## 🛠️ Getting Started

### Prerequisites

* **.NET 10 SDK**

This project intentionally targets **.NET 10** because one of its goals is to practice and get comfortable with newer versions of the .NET platform after working with previous versions such as .NET 6 and .NET 8.

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

* Reducing the responsibilities currently concentrated in `GameEngine`.
* Separating game orchestration from move validation and move execution.
* Making board simulation safer for AI search.
* Keeping project documentation synchronized with the actual code structure.
* Preserving the custom test infrastructure while improving the core design.

The goal is not only to make the chess game work, but to make the engine easier to reason about, test, refactor, and extend.

---

## 🔮 Future Improvements

Potential future improvements include:

* Extracting move validation into a dedicated component.
* Extracting move execution into a dedicated component.
* Introducing a dedicated game state model.
* Improving AI board simulation.
* Improving AI evaluation.
* Adding stronger search optimizations.
* Adding PGN-style move history.
* Adding save/load support.
* Optionally adding an xUnit test project later for comparison with the custom test runner.

---

## 📜 License

This project is open-source and available under the MIT License.
