using System;
using RunTime.Keys;
using RunTime.Signals;
using UnityEngine;
using UnityEngine.InputSystem;

namespace RunTime.Controllers
{
    public class InputController : MonoBehaviour
    {
        #region Self Variables

        #region Serialized Variables
        
        

        #endregion

        #region Private Variables

        private GameInput _gameInput;
        [SerializeField]private Camera _mainCamera;
        private bool _isTapped;
        private InputParamKeys _inputParamKeys;
        private Plane _worldPlane = new Plane(Vector3.up, Vector3.zero);
        private Transform _selectedObject;
        private Vector3 _offSet;
       
        #endregion

        #endregion

        public void GetReferences(Camera mainCamera, GameInput gameInput)
        {
            _mainCamera = mainCamera;
            _gameInput = gameInput;
            _inputParamKeys = new InputParamKeys();
        }

        public void Tapped(InputAction.CallbackContext ctx)
        {
            
            var input = _gameInput.Input.ScreenPosition.ReadValue<Vector2>();
            Ray ray = _mainCamera.ScreenPointToRay(input);
            
            if (Physics.Raycast(ray, out RaycastHit hit , 100f))
            {
                if (!hit.collider.gameObject.CompareTag("Block")) return;
                _worldPlane = new Plane(Vector3.up, new Vector3( 0,hit.point.y,0 ));
                _selectedObject = hit.collider.transform;
                InputSignals.Instance.onSendSelectedObject?.Invoke(hit.collider.gameObject);
                _worldPlane = new  Plane(Vector3.up, new Vector3( 0,hit.point.y,0));
                _isTapped = true;

            }

            if (_worldPlane.Raycast(ray, out float hitDistance))
            {
                _offSet = _selectedObject.position - ray.GetPoint(hitDistance);
            }
            
            
        }

       

        public void TapCancel(InputAction.CallbackContext ctx)
        {
            _isTapped  = false;
            _inputParamKeys.InputVector = new Vector2Int(0, 0);
            InputSignals.Instance.onSendInputParams?.Invoke(_inputParamKeys);
            InputSignals.Instance.onSelectedObjectReleased?.Invoke();
        }

        

        
        private void Update()
        {
            if (!_isTapped) return;
           var input = _gameInput.Input.ScreenPosition.ReadValue<Vector2>();
           var ray = _mainCamera.ScreenPointToRay(input);
           if (_worldPlane.Raycast(ray, out var hit))
           {
               
               var hitPoint = ray.GetPoint(hit) + _offSet;
               
               var newInputVector = new Vector2Int(Mathf.RoundToInt(hitPoint.x), Mathf.RoundToInt(hitPoint.z));
               Debug.Log(newInputVector);
               _inputParamKeys.InputVector = newInputVector;
                InputSignals.Instance.onSendInputParams?.Invoke(_inputParamKeys);
             
           }

        }
        
    }
}