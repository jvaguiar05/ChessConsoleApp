# Console Chess Engine & Game

A fully compliant, 100% rule-accurate Chess game built entirely in C# as a lightweight .NET console application. The project features a responsive terminal presentation layer, live match statistics, and an intelligent, self-contained AI opponent utilizing Minimax search with Alpha-Beta pruning.

## 🚀 Features

### Core Game Engine
* **100% Rule Compliance:** Fully supports all tournament rules including En Passant, Castling, Pawn Promotion, Check/Checkmate alerts, and Stalemate validation.
* **Draw Conditions:** Built-in state-machine checking for the Fifty-Move Rule, Threefold Repetition (via position snapshots), and Insufficient Material layouts.

### Presentation Layer (`UI/`)
* **Dynamic Sidebar Dashboard:** Displays a live "Graveyard" tracking captured pieces using elegant Unicode symbols (`♛ ♞ ♟`).
* **Live Material Evaluation:** Keeps track of the active point differentials and prints a colored advantage alert (e.g., `+3 White Advantage`) updating in real-time.

### Artificial Intelligence Layer (`AI/`)
* **Minimax with Alpha-Beta Pruning:** A high-performance, self-contained decision tree that cuts off unpromising branches to calculate moves efficiently.
* **Piece-Square Tables (PSTs):** Makes the AI positionally aware. It actively fights for center control, coordinates pawn structures, develops knights away from the edges, and tucks its king away safely.
* **5 Difficulty Tiers (600 - 2500+ Elo):**
  * **Beginner (600 Elo):** 1-ply search with a 50% planned blunder rate.
  * **Intermediate (1200 Elo):** 3-ply search for steady, casual play.
  * **Advanced (1600 Elo):** 4-ply search searching for tactical forks.
  * **Master (2000 Elo):** 5-ply deep tactical calculating tree.
  * **Grandmaster (2500+ Elo):** 6-ply deep, highly punishing lookahead horizon.

---

## 🧪 Testing & Validation

### Unit Testing
The project includes a comprehensive suite of unit tests covering all critical components of the chess engine. The tests are organized into the following categories:

- **Board Tests:** Validate the integrity of the board representation, including piece placement, move generation, and check detection.
- **Draw Tests:** Ensure that draw conditions such as the Fifty-Move Rule, Threefold Repetition, and Insufficient Material are correctly identified.
- **Game State Tests:** Verify the correct functioning of the game state machine, including transitions between different game states and the detection of check/checkmate scenarios.
- **Move Tests:** Test the Move class and the parsing of algebraic notation to ensure accurate move representation.
- **Notation Tests:** Validate the InputParser's ability to correctly interpret algebraic notation and convert it into valid moves.
  
---

## 📂 Project Architecture

```text
ChessConsoleApp/
│
├── AI/                        # --- Artificial Intelligence Layer ---
│   ├── ChessAi.cs             # Minimax algorithm with Alpha-Beta pruning & Elo blending
│   └── BoardEvaluator.cs      # Evaluation engine using material weights & Piece-Square Tables
│
├── Core/                      # --- Environment & Orchestration ---
│   ├── Board.cs               # Manages the 2D piece array, indexing, and check detection
│   └── GameEngine.cs          # State-machine handling main game/menu loops and rules
│
├── Enums/                     # GameState, PieceColor, and AiDifficulty configurations
├── Models/                    # Position, Move records, and individual Piece entities
├── UI/                        # --- Presentation Layer ---
│   ├── ConsoleRenderer.cs     # Grid drawing, Unicode sidebar layout, and color evaluation
│   └── InputParser.cs         # Converts algebraic notation strings (e.g., "e2 e4")
└── Program.cs                 # Entry point for the console application

Tests/
│
├── BoardTests/               # Unit tests for Board class and move validation
├── DrawTests/                # Unit tests for draw conditions (Fifty-Move, Threefold Repetition, Insufficient Material)
├── GameStateTests/           # Unit tests for GameEngine state transitions and check/checkmate detection
├── MoveTests/                # Unit tests for Move class and algebraic notation parsing
├── NotationTests/            # Unit tests for InputParser and algebraic notation handling
└── TestSuite.cs              # Test runner for executing all unit tests
```

## 🛠️ Getting Started

### Prerequisites
- .NET 8.0 SDK or newer.

### Installation & Run
Clone this repository to your local computer:

```bash
git clone https://github.com/jvaguiar05/ConsoleChessApp.git
cd ConsoleChessApp
```

Build and run the application using the dotnet CLI:

```bash
dotnet run --project ChessConsoleApp
```

### Running the Test Suite
The core movement trajectories and edge-case draw scenarios are fully backed by an automated test suite. To verify everything is green:

```bash
dotnet run --project ChessConsoleApp --run-tests
```

## 🎮 How to Play

- **Main Menu:** Select between Pass-and-Play (2-Player Local) or Play vs Computer (AI).
- **Setup:** If playing the computer, select your opponent's skill tier (Beginner to Grandmaster) and pick your preferred color layout.
- **Entering Moves:** Enter moves using space-separated algebraic coordinates matching the board ranks, for example:
  - `e2 e4` (Moves a piece from e2 to e4)
  - `g1 f3` (Develops a Knight)
- **Resigning:** Type `quit` at any time during your turn to throw in the towel, display the conclusive winner, and return safely back to the Main Menu.

## 📜 License
This project is open-source and available under the MIT License.