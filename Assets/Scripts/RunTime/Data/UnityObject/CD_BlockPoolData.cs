using System.Collections.Generic;
using RunTime.Data.ValueObjects;
using UnityEngine;

namespace RunTime.Data.UnityObject
{
    [CreateAssetMenu(fileName = "CD_BlockPoolData", menuName = "Clone/CD_BlockPoolData", order = 0)]
    public class CD_BlockPoolData : ScriptableObject
    {
        public List<BlockPoolData> BlockPools;
    }
}