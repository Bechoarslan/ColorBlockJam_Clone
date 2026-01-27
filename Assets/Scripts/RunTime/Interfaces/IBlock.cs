using RunTime.Enums;

namespace RunTime.Interfaces
{
    public interface IBlock
    {
        public BlockType BlockType { get; set; }
        
        public BlockColorType BlockColorType { get; set; }
        
        public int BlockSize { get; set; }


       
        
    }
}