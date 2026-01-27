using System.Collections;
using RunTime.Controllers;
using RunTime.Signals;
using UnityEngine;

namespace RunTime.Interfaces.Ability
{
    public class FreezeTimerAbility : IAbility
    {

        private AbilityManager Manager;
        private Coroutine _timerCoroutine;
        private float _freezeDuration = 5f;
        
        public FreezeTimerAbility(AbilityManager manager)
        {
            Manager = manager;
        }
        public void OnEnterAbility()
        {
            _timerCoroutine = Manager.StartCoroutine(StartFreezeTimeCoroutine());
        }

        private IEnumerator StartFreezeTimeCoroutine()
        {
            AbilitySignals.Instance.onTimeAbilityActivaed?.Invoke(true);
            yield return new WaitForSeconds(_freezeDuration);
            AbilitySignals.Instance.onTimeAbilityActivaed?.Invoke(false);
            OnExitAbility();
        }

        public void OnExitAbility()
        {
            if (_timerCoroutine != null)
            {
                Manager.StopCoroutine(_timerCoroutine);
                _timerCoroutine = null;
            }
        }
    }
}