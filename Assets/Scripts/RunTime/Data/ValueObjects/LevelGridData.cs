
using System.Collections.Generic;
using RunTime.Enums;
using UnityEngine;

namespace RunTime.Data.ValueObjects
{
    [System.Serializable]
    public class LevelGridData
    {
        
        public bool IsOccupied;
        public Vector2 GridPosition;
        
    }

    [System.Serializable]
    public class BlockColorData
    {
        public List<GameObject> Block;
        public BlockColorType BlockColorType;
    }

    [System.Serializable]
    public class LevelDataList
    {
        public int Row;
        public int Column;
        public int LevelID;
        public List<LevelGridData> Grids = new List<LevelGridData>();
        public List<BlockColorData> BlockColors = new List<BlockColorData>();

    }
    
    
}