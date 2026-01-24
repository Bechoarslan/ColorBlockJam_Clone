using RunTime.Enums;
using RunTime.Signals;
using UnityEngine;

namespace RunTime.Managers
{
    public class GameManager : MonoBehaviour
    {
        #region Self Variables

        #region Serialized Variables

        

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
            GameSignals.Instance.onChangeGameState += OnChangeGameState;
        }

        private void OnChangeGameState(GameState state)
        {
            switch (state)
            {
                case GameState.Game:
                    InputSignals.Instance.onInputStateChanged?.Invoke(GameState.Game);
                    break;
                case GameState.Ability:
                    InputSignals.Instance.onInputStateChanged?.Invoke(GameState.Ability);
                    break;
                
            }
        }

        private void UnSubscribeEvents()
        { 
            GameSignals.Instance.onChangeGameState -= OnChangeGameState;
        }

        private void OnDisable()
        {
            UnSubscribeEvents();
        }

    }
}