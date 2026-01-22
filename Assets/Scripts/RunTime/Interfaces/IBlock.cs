using RunTime.Enums;

namespace RunTime.Interfaces
{
    public interface IBlock
    {
        public BlockType BlockType { get; }
        
        public int BlockSize { get; }
        

        
    }
}