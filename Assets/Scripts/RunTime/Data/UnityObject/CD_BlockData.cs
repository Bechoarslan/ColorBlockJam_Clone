using System.Collections.Generic;
using RunTime.Data.ValueObjects;

namespace RunTime.Data.UnityObject
{
    [UnityEngine.CreateAssetMenu(fileName = "CD_BlockData", menuName = "Clone/CD_BlockData", order = 0)]
    public class CD_BlockData : UnityEngine.ScriptableObject
    {
        public List<BlockData> Blocks;
    }
}