using System;
using RunTime.Enums;
using RunTime.Interfaces;
using Sirenix.OdinInspector;
using UnityEngine;

namespace RunTime.Controllers
{
    public class Block : MonoBehaviour, IBlock
    {
       
        [SerializeField] private BlockColorType blockColorType;
        [SerializeField] private BlockType blockType;
        [SerializeField] private int blockSize;

        public BlockColorType BlockColorType
        {
            get => blockColorType;
            set => blockColorType = value;
        }

        public BlockType BlockType
        {
            get => blockType;
            set => blockType = value;
        }

        public int BlockSize
        {
            get => blockSize;
            set => blockSize = value;
        }
        
        public void SetBlockSize(int newSize)
        {
            BlockSize = newSize;
        }
        
        public void SetColorType(BlockColorType colorType)
        {
            BlockColorType = colorType;
        }

        public void SetBlockType(BlockType blockType)
        {
            BlockType = blockType;
        }

        
        
    }
}
