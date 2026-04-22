# Sudoku

A Sudoku game built with C# and Windows Forms. Generate puzzles, solve them, check your answers.

## What It Does

- **Random puzzles every time** – No two games are the same
- **Three difficulty levels** – Easy, Medium, Hard (just means more empty squares)
- **Check your work** – Wrong cells light up red so you know what's broken
- **Stuck? See the answer** – Show solution when you need it
- **Simple grid** – 9×9 board with clear 3×3 box lines

## Getting It Running

### What You Need
- .NET 6.0 SDK (Windows)
- Visual Studio 2022 or VS Code with the C# extension

### Start the Game
```bash
dotnet run
```

### Build a Standalone Version
```bash
dotnet build
dotnet publish -c Release -r win-x64 --self-contained
```

The executable will be in `bin/Release/net6.0/win-x64/publish/`.

## Files

- **SudokuBoard.cs** – Generates puzzles, validates solutions, checks if you won
- **MainForm.cs** – The UI and all the button clicks
- **Sudoku.csproj** – Project setup

## How It Actually Works

The app fills a complete valid board using backtracking, then removes cells based on your difficulty pick. When you hit "Check," it compares what you entered against the solved board.

Validation is straightforward: no duplicates in any row, column, or 3×3 box.

## Built With

- C# 10
- .NET 6 / WinForms
- Backtracking for puzzle generation
