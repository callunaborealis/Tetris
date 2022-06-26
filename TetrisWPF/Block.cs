using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TetrisWPF
{
    public abstract class Block
    {
        protected abstract Position[][] Tiles { get; }
        protected abstract Position StartOffset { get; }
        public abstract int Id { get; }
        private int rotationState;
        private Position offset;

        public Block()
        {
            offset = new(StartOffset.Row, StartOffset.Col);

        }

        public IEnumerable<Position> TilePositions()
        {
            foreach(Position p in Tiles[rotationState])
            {
                yield return new(p.Row + offset.Row, p.Col + offset.Col);
            }
        }

        public void RotateClockwise()
        {
            rotationState = (rotationState + 1) % Tiles.Length;
        }

        public void RotateAnticlockwise()
        {
            if (rotationState == 0)
            {
                rotationState = Tiles.Length - 1;
            } else
            {
                rotationState--;
            }
        }

        public void Move(int rows, int cols)
        {
            offset.Row += rows;
            offset.Col += cols;
        }

        public void Reset()
        {
            rotationState = 0;
            offset.Row = StartOffset.Row;
            offset.Col = StartOffset.Col;
        }
    }
}
