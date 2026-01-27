using RunTime.Controllers;
using RunTime.Enums;
using RunTime.Signals;

namespace RunTime.Interfaces.Ability
{
    public class VacuumAbility : IAbility
    {
        private AbilityManager Manager;
        
        public VacuumAbility(AbilityManager abilityManager)
        {
            Manager = abilityManager;
        }
        public void OnEnterAbility()
        {
            var selectedObject = Manager.SelectedObject.parent;
            if (selectedObject is null) return;
            var iblock = selectedObject.GetComponent<IBlock>();
            AbilitySignals.Instance.onRemoveObjectDestroyedByVacuum?.Invoke(iblock.BlockColorType);
            OnExitAbility();
        }

        public void OnExitAbility()
        {
            GameSignals.Instance.onChangeGameState?.Invoke(GameState.Game);
            Manager.SelectedObject = null;
        }
    }
}