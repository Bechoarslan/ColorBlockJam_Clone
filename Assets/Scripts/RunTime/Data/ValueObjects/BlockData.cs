using RunTime.Enums;
using UnityEngine;

namespace RunTime.Data.ValueObjects
{
    [System.Serializable]
    public class BlockData
    {
        public BlockType Type;
        public GameObject Prefab;
    }
}