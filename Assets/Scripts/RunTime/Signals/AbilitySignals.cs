using System;
using RunTime.Enums;
using RunTime.Extensions.RunTime.Utilities;
using UnityEngine;

namespace RunTime.Signals
{
    public class AbilitySignals : MonoSingleton<AbilitySignals>
    {
        public Action<Transform> onAbilitySelectedObject;

        public Action onStartAbility;
        
        public Action<AbilityType> onAbilitySelected;

        public Action<GameObject,BlockColorType> onRemoveObjectDestroyedByHammer;
        
        public Action<BlockColorType> onRemoveObjectDestroyedByVacuum;

        public Action<Vector2Int,bool> onRemoveOccupiedGrid;


        public Action<bool> onTimeAbilityActivaed;
    }
}