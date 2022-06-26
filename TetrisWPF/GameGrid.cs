using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TetrisWPF
{
    internal class GameGrid
    {
        private readonly int[,] grid;
        public int Rows { get; }
        public int Cols { get; }

        // indexing on the GameGrid
        public int this[int row, int col]
        {
            get => grid[row, col];
            set => grid[row, col] = value;
        }

        public GameGrid(int rows, int cols)
        {
            Rows = rows;
            Cols = cols;
            grid = new int[rows, cols];
        }

        public bool IsCellWithinGameGrid(int row, int col)
        {
            return row >= 0 && row < Rows && col >= 0 && col < Cols;
        }

        public bool IsCellEmpty(int row, int col)
        {
            return IsCellWithinGameGrid(row, col) && grid[row, col] == 0;
        }

        public bool IsRowFull(int row)
        {
            for (int col = 0; col < Cols; col++)
            {
                if (grid[row, col] == 0)
                {
                    return false;
                }
            }
            return true;
        }

        public bool IsRowEmpty(int row)
        {
            for (int col = 0; col < Cols; col++)
            {
                if (grid[row, col] != 0)
                {
                    return false;
                }
            }
            return true;
        }

        private void ClearRow(int row)
        {
            for (int col = 0; col < Cols; col++)
            {
                grid[row, col] = 0;
            }
        }

        private void MoveRowDown(int row, int numberOfRows)
        {
            for (int col = 0; col < Cols; col++)
            {
                grid[row + numberOfRows, col] = grid[row, col];
                grid[row, col] = 0;
            }
        }

        public int ClearFullRows()
        {
            int cleared = 0;
            for (int row = Rows - 1; row >= 0; row--)
            {
                if(IsRowFull(row))
                {
                    ClearRow(row);
                    cleared++;
                }
                else if (cleared > 0)
                {
                    MoveRowDown(row, cleared);
                }
            }
            return cleared;
        }
    }
}
