using System.Collections.Generic;
using RunTime.Data.ValueObjects;
using UnityEngine;

namespace RunTime.Data.UnityObject
{
    [CreateAssetMenu(fileName = "CD_LevelData", menuName = "CD_LevelData", order = 0)]
    public class CD_LevelGridData : ScriptableObject
    {
        
        public List<LevelDataList> Levels;
        
        
    }
}