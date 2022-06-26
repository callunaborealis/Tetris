using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TetrisWPF
{
    public class BlockQueue
    {
        private readonly Block[] blocks = new Block[]
        {
            new IShapedBlock(),
            new JShapedBlock(),
            new LShapedBlock(),
            new SquareBlock(),
            new TShapedBlock(),
            new ZShapedBlock()
        };

        private readonly Random random = new();

        public Block NextBlock { get; private set; }

        private Block RandomBlock()
        {
            return blocks[random.Next(blocks.Length)];
        }

        public BlockQueue()
        {
            NextBlock = RandomBlock();
        }

        public Block GetAndUpdate()
        {
            Block block = NextBlock;
            do
            {
                NextBlock = RandomBlock();
            }
            while (block.Id == NextBlock.Id);

            return block;
        }
    }
}
