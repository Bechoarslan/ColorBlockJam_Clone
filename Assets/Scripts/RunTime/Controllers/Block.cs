using RunTime.Enums;
using RunTime.Interfaces;
using Sirenix.OdinInspector;
using UnityEngine;

namespace RunTime.Controllers
{
    public class Block : MonoBehaviour, IBlock
    {
        public BlockType Type;
        public BlockType BlockType => Type;
        
        
    }
}
