using RunTime.Enums;
using RunTime.Signals;
using UnityEngine;
using UnityEngine.UI;

namespace RunTime.Keys
{
    public class AbilityUIEventSubscriber : MonoBehaviour
    {
        #region Self Variables

        #region Serialized Variables

        
        [SerializeField] private AbilityType abilityType;
        [SerializeField] private Button button;

        #endregion

        #region Private Variables

        #endregion

        #endregion


        private void OnEnable()
        {
            SubscribeEvents();
        }

        private void SubscribeEvents()
        {
            button.onClick.AddListener(OnButtonClicked);
            
        }

        private void OnButtonClicked()
        {
            switch (abilityType)
            {
                case AbilityType.FireCracker:
                    AbilitySignals.Instance.onAbilitySelected?.Invoke(abilityType);
                    GameSignals.Instance.onChangeGameState?.Invoke(GameState.Ability);
                    break;
                case AbilityType.FreezeTime:
                    AbilitySignals.Instance.onAbilitySelected?.Invoke(abilityType);
                    AbilitySignals.Instance.onStartAbility?.Invoke();
                    break;
                case AbilityType.Hammer:
                    AbilitySignals.Instance.onAbilitySelected?.Invoke(abilityType);
                    GameSignals.Instance.onChangeGameState?.Invoke(GameState.Ability);
                    break;
                case AbilityType.Vacuum:
                    AbilitySignals.Instance.onAbilitySelected?.Invoke(abilityType);
                    GameSignals.Instance.onChangeGameState?.Invoke(GameState.Ability);
                    break;
              
            }
        }

        private void UnSubscribeEvents()
        { 
            button.onClick.RemoveListener(OnButtonClicked);
        }

        private void OnDisable()
        {
            UnSubscribeEvents();
        }
    }
}