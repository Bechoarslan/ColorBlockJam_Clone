using System.Threading;
using Cysharp.Threading.Tasks;
using RunTime.Signals;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

namespace RunTime.Managers
{
    public class TimerManager : MonoBehaviour
    {
        #region Self Variables

        #region Serialized Variables

        [SerializeField] private TextMeshProUGUI timerText;

        #endregion

        #region Private Variables

        private float _timer = 1f;
        private CancellationTokenSource _cts;

        private bool _isPaused;
        #endregion

        #endregion


        private void OnEnable()
        {
            SubscribeEvents();
        }

        private void SubscribeEvents()
        {
            
            GameSignals.Instance.onStartTimer += OnStartTimer;
            AbilitySignals.Instance.onTimeAbilityActivaed += OnTimeAbilityActivated;
        }

        private void OnTimeAbilityActivated(bool value)
        {
            _isPaused = value;
        }
        
        [Button]
        private void OnStartTimer()
        {
            _cts = new CancellationTokenSource();
            StartTimer(60,_cts.Token).Forget();
        }

        private async UniTaskVoid StartTimer(int totalSeconds, CancellationToken ctsToken)
        {
            int remaningTime = totalSeconds;
            while (remaningTime >= 0)
            {
                UpdateUI(remaningTime);
                if (remaningTime == 0)
                {
                    break;
                }

                while (_isPaused)
                {
                    await UniTask.Yield(cancellationToken: ctsToken);
                }
                await UniTask.Delay(1000, DelayType.UnscaledDeltaTime,cancellationToken: ctsToken);
                remaningTime--;
            }
            Debug.Log("Timer Finished");
        }

        private void UpdateUI(int seconds)
        {
            int minutes = seconds / 60;
            int secs = seconds % 60;
            
            timerText.text = $"{minutes:00}:{secs:00}";
        }

        private void UnSubscribeEvents()
        {
            GameSignals.Instance.onStartTimer -= OnStartTimer;
            AbilitySignals.Instance.onTimeAbilityActivaed -= OnTimeAbilityActivated;
        }

        private void OnDisable()
        {
            UnSubscribeEvents();
        }
    }
}