using System.Collections.Generic;
using RunTime.Controllers;
using RunTime.Enums;
using UnityEngine;

namespace RunTime.Systems
{
    public static class BlockRegistry
    {
        public static readonly Dictionary<BlockColorType, List<GameObject>> _blocks = new Dictionary<BlockColorType, List<GameObject>>();

        public static void RegisterBlock(BlockColorType blockType, GameObject block)
        {
            if (_blocks.TryGetValue(blockType, out var blockList))
            {
                blockList.Add(block);
            }
            else
            {
                var newList = new List<GameObject> { block };
                _blocks.Add(blockType, newList);
            }
           
        }
        
        public static Dictionary<BlockColorType, List<GameObject>> GetRegistry() =>  _blocks;
        
        public static void ClearRegistry()
        {
            _blocks.Clear();
        }
    }
}