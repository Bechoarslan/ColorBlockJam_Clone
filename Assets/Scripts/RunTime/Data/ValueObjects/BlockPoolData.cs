using RunTime.Controllers;
using RunTime.Enums;
using UnityEngine;

namespace RunTime.Data.ValueObjects
{
    [System.Serializable]
    public class BlockPoolData
    {
        public BlockTypeEnums BlockType;
        public BlockView BlockPrefab;
        public int InitialPoolSize;
    }
}