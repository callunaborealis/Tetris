using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TetrisWPF
{
    public class GameState
    {
        private Block currentBlock;
        public Block CurrentBlock
        {
            get => currentBlock;
            private set
            {
                currentBlock = value;
                currentBlock.Reset();

                for (int i = 0; i < 2; i++)
                {
                    currentBlock.Move(1, 0);
                    if (!BlockFits())
                    {
                        // Ensure block is fully visible on spawn
                        currentBlock.Move(-1, 0);
                    }    
                }
            }
        }
        public GameGrid GameGrid { get; }
        public BlockQueue BlockQueue { get; }

        public bool GameOver { get; private set; }

        public int Score { get; private set; }

        public Block HeldBlock { get; private set; }
        public bool CanHold { get; private set; }

        public GameState()
        {
            GameGrid = new(22, 10);
            BlockQueue = new();
            currentBlock = BlockQueue.GetAndUpdate();
            CanHold = true;
        }

        private bool BlockFits()
        {
            foreach (Position position in CurrentBlock.TilePositions())
            {
                if (!GameGrid.IsCellEmpty(position.Row, position.Col))
                {
                    return false;
                }
            }
            return true;
        }

        public void HoldBlock()
        {
            if (!CanHold)
            {
                return;
            }

            if (HeldBlock == null)
            {
                HeldBlock = currentBlock;
                CurrentBlock = BlockQueue.GetAndUpdate();
            } else
            {
                (HeldBlock, CurrentBlock) = (CurrentBlock, HeldBlock);
            }

            CanHold = false;

    
        }

        public void RotateBlockClockwise()
        {
            CurrentBlock.RotateClockwise();

            if (!BlockFits())
            {
                CurrentBlock.RotateAnticlockwise();
            }
        }
        public void RotateBlockAnticlockwise()
        {
            CurrentBlock.RotateAnticlockwise();

            if (!BlockFits())
            {
                CurrentBlock.RotateClockwise();
            }
        }

        public void MoveBlockLeft()
        {
            CurrentBlock.Move(0, -1);

            if (!BlockFits())
            {
                CurrentBlock.Move(0, 1);
            }
        }

        public void MoveBlockRight()
        {
            CurrentBlock.Move(0, 1);

            if (!BlockFits())
            {
                CurrentBlock.Move(0, -1);
            }
        }

        private bool IsGameOver()
        {
            // Lose game if the first two rows which are hidden have blocks sticking in them
            return !(GameGrid.IsRowEmpty(0) && GameGrid.IsRowEmpty(1));
        }

        private void PlaceBlock()
        {
            foreach (Position position in CurrentBlock.TilePositions())
            {
                GameGrid[position.Row, position.Col] = CurrentBlock.Id;
            }

            Score += GameGrid.ClearFullRows();

            if (IsGameOver())
            {
                GameOver = true;
            }
            else
            {
                CurrentBlock = BlockQueue.GetAndUpdate();
                CanHold = true;
            }
        }

        public void MoveBlockDown()
        {
            CurrentBlock.Move(1, 0);

            if (!BlockFits())
            {
                CurrentBlock.Move(-1, 0);
                PlaceBlock();
            }
        }

        private int GetTotalDropHeight(Position position)
        {
            int dropHeight = 0;

            while (GameGrid.IsCellEmpty(position.Row + dropHeight + 1, position.Col))
            {
                dropHeight += 1;
            }

            return dropHeight;
        }

        public int GetDropHeightOfCurrentBlock()
        {
            int dropHeight = GameGrid.Rows;
            foreach (Position position in CurrentBlock.TilePositions())
            {
                dropHeight = Math.Min(dropHeight, GetTotalDropHeight(position));
            }
            return dropHeight;
        }

        public void FastForwardCurrentBlockDrop()
        {
            CurrentBlock.Move(GetDropHeightOfCurrentBlock(), 0);
            PlaceBlock();
        }

    }
}
