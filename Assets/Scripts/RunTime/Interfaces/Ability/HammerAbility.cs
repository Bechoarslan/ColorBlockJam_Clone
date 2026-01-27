using RunTime.Controllers;
using RunTime.Enums;
using RunTime.Signals;
using UnityEngine;

namespace RunTime.Interfaces.Ability
{
    public class HammerAbility : IAbility
    {
        private AbilityManager Manager;
        public HammerAbility(AbilityManager manager)
        {
            Manager = manager;
        }
        public void OnEnterAbility()
        {
            var selectedObject = Manager.SelectedObject.parent;
            if (selectedObject is null) return;
            Debug.Log("Hammer Ability Used");
            var iblock = selectedObject.GetComponent<Block>();
            AbilitySignals.Instance.onRemoveObjectDestroyedByHammer?.Invoke(selectedObject.gameObject, iblock.BlockColorType);
            OnExitAbility();

        }

        public void OnExitAbility()
        {
            Manager.SelectedObject = null;
            GameSignals.Instance.onChangeGameState?.Invoke(GameState.Game);
        }
    }
}