using RunTime.Enums;
using UnityEngine;

namespace RunTime.Controllers
{
    public class BlockView : MonoBehaviour
    { 
        public BlockTypeEnums BlockType { get; private set; }
        public void SetType(BlockTypeEnums configBlockType)
        {
            BlockType = configBlockType;
        }
    }
}