
using System.Collections.Generic;
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
    public class LevelDataList
    {
        public int Row;
        public int Column;
        public int LevelID;
        public List<LevelGridData> levels;
    }
    
    
}