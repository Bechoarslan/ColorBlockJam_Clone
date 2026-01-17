using System.Collections.Generic;
using RunTime.Data.ValueObjects;
using UnityEngine;

namespace RunTime.Data.UnityObject
{
    [CreateAssetMenu(fileName = "CD_ColorData", menuName = "Clone/CD_ColorData", order = 0)]
    public class CD_ColorData : ScriptableObject
    {
        public List<ColorData> ColorData;
    }
}