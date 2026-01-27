using RunTime.Enums;
using RunTime.Keys;
using RunTime.Signals;
using UnityEngine;
using UnityEngine.InputSystem;

namespace RunTime.Controllers
{
    public class InputController : MonoBehaviour
    {
        [SerializeField] private Camera mainCamera;

        private GameInput _gameInput;
        private bool _isTapped;
        private Transform _selectedObject;
        private Plane _worldPlane;
        private Vector3 _offset;
        private InputParamKeys _inputParamKeys;
        private GameState _gameState;

        public void GetReferences(Camera cam, GameInput input)
        {
            mainCamera = cam;
            _gameInput = input;
            _inputParamKeys = new InputParamKeys();
        }

        public void Tapped(InputAction.CallbackContext ctx)
        {
           GameTap();
         
        }

        private void GameTap()
        {
            if (_isTapped) return;
            Vector2 screenPos = _gameInput.Input.ScreenPosition.ReadValue<Vector2>();
            Ray ray = mainCamera.ScreenPointToRay(screenPos);

            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                if (!hit.collider.CompareTag("Block")) return;

                _selectedObject = hit.transform;
                _worldPlane = new Plane(Vector3.up, _selectedObject.position);

                if (_worldPlane.Raycast(ray, out float enter))
                {
                    Vector3 hitPoint = ray.GetPoint(enter);
                    _offset = _selectedObject.position - hitPoint;
                }

                switch (_gameState)
                {
                    case GameState.Game:
                        InputSignals.Instance.onSendSelectedObject?.Invoke(hit.collider.gameObject);
                        _isTapped = true;
                        break;
                    case GameState.Ability:
                        Debug.Log("Ability Object Selected");
                        AbilitySignals.Instance.onAbilitySelectedObject?.Invoke(hit.collider.gameObject.transform);
                        AbilitySignals.Instance.onStartAbility?.Invoke();
                        break;
                    case GameState.Pause:
                        _isTapped  = false;
                        _selectedObject = null;
                        InputSignals.Instance.onSelectedObjectReleased?.Invoke();
                        break;
                }
               
            }
        }

        public void TapCancel(InputAction.CallbackContext ctx)
        {
            switch (_gameState)
            {
                case GameState.Game:
                    _isTapped = false;
                    _selectedObject = null;
                    InputSignals.Instance.onSelectedObjectReleased?.Invoke();
                    break;
                case GameState.Ability:
                    _isTapped = false;
                    break;
            }
            
        }

        private void Update()
        {
            if (!_isTapped || _selectedObject == null) return;

            Ray ray = mainCamera.ScreenPointToRay(
                _gameInput.Input.ScreenPosition.ReadValue<Vector2>()
            );

            if (_worldPlane.Raycast(ray, out float enter))
            {
                Vector3 hitPoint = ray.GetPoint(enter);
                Vector3 intendedPos = hitPoint + _offset;

                _inputParamKeys.InputVector = new Vector2(intendedPos.x, intendedPos.z);
                InputSignals.Instance.onSendInputParams?.Invoke(_inputParamKeys);
            }
        }

        public void OnInputStateChanged(GameState state) => _gameState = state;
        
    }
}
