using RunTime.Enums;
using RunTime.Interfaces;
using UnityEngine;

namespace RunTime.Controllers
{
    public class Block : MonoBehaviour, IBlock
    {
        public BlockType Type;
        public BlockType BlockType => Type;
    }
}
