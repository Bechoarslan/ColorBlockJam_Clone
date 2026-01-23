using System;
using RunTime.Enums;
using RunTime.Interfaces;
using Sirenix.OdinInspector;
using UnityEngine;

namespace RunTime.Controllers
{
    public class Block : MonoBehaviour, IBlock
    {
        [SerializeField] private  BlockType type;
        [SerializeField] private int size;
        public BlockType BlockType => type;
        public int BlockSize => size;
        
        
        public void SetBlockSize(int newSize)
        {
            size = newSize;
        }
    }
}
