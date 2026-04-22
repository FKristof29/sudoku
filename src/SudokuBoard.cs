using System;

namespace SudokuApp
{
    public class SudokuBoard
    {
        private int[,] solution = new int[9, 9];
        private int[,] puzzle   = new int[9, 9];
        private Random rng      = new Random();

        public int[,] Puzzle   => (int[,]) puzzle.Clone();
        public int[,] Solution => (int[,]) solution.Clone();


        public void Generate(Difficulty difficulty)
        {
            int[,] board = new int[9, 9];
            FillBoard(board);
            solution = (int[,]) board.Clone();

            int cellsToRemove = difficulty switch
            {
                Difficulty.Easy   => 35,
                Difficulty.Medium => 45,
                Difficulty.Hard   => 55,
                _                 => 40
            };

            puzzle = (int[,]) board.Clone();
            RemoveCells(puzzle, cellsToRemove);
        }

        private bool FillBoard(int[,] board)
        {
            for (int row = 0; row < 9; row++)
            {
                for (int col = 0; col < 9; col++)
                {
                    if (board[row, col] == 0)
                    {
                        int[] nums = ShuffledNumbers();
                        foreach (int num in nums)
                        {
                            if (IsValid(board, row, col, num))
                            {
                                board[row, col] = num;
                                if (FillBoard(board)) return true;
                                board[row, col] = 0;
                            }
                        }
                        return false;
                    }
                }
            }
            return true;
        }

        private void RemoveCells(int[,] board, int count)
        {
            int removed = 0;
            while (removed < count)
            {
                int row = rng.Next(9);
                int col = rng.Next(9);
                if (board[row, col] != 0)
                {
                    board[row, col] = 0;
                    removed++;
                }
            }
        }

        private int[] ShuffledNumbers()
        {
            int[] nums = { 1, 2, 3, 4, 5, 6, 7, 8, 9 };
            for (int i = nums.Length - 1; i > 0; i--)
            {
                int j = rng.Next(i + 1);
                (nums[i], nums[j]) = (nums[j], nums[i]);
            }
            return nums;
        }


        public static bool IsValid(int[,] board, int row, int col, int num)
        {
            for (int c = 0; c < 9; c++)
                if (board[row, c] == num) return false;

            for (int r = 0; r < 9; r++)
                if (board[r, col] == num) return false;

            int boxRow = (row / 3) * 3;
            int boxCol = (col / 3) * 3;
            for (int r = boxRow; r < boxRow + 3; r++)
                for (int c = boxCol; c < boxCol + 3; c++)
                    if (board[r, c] == num) return false;

            return true;
        }

        public bool CheckSolution(int[,] userBoard)
        {
            for (int r = 0; r < 9; r++)
                for (int c = 0; c < 9; c++)
                    if (userBoard[r, c] != solution[r, c]) return false;
            return true;
        }

        public bool IsPuzzleCell(int row, int col)
        {
            return puzzle[row, col] == 0;
        }
    }

    public enum Difficulty { Easy, Medium, Hard }
}
