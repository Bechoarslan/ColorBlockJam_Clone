using System;
using RunTime.Controllers;
using UnityEngine;
using UnityEngine.InputSystem;

namespace RunTime.Managers
{
    public class InputManager : MonoBehaviour
    {
        #region Self Variables

        #region Serialized Variables

        [SerializeField] private InputController inputController;

        #endregion

        #region Private Variables

        private GameInput _gameInput;

        #endregion

        #endregion


        private void Awake()
        {
            _gameInput = new GameInput();
            var mainCamera = FindObjectOfType<Camera>();
            if (mainCamera != null)
            {
                inputController.GetReferences(mainCamera,_gameInput);
            }
        }

        private void OnEnable()
        {
            SubscribeEvents();
        }

        private void SubscribeEvents()
        {
            _gameInput.Enable();
            _gameInput.Input.Tap.performed += OnTapPerformed;
            _gameInput.Input.Tap.canceled += OnTapCanceled;
            
        }

        private void OnTapCanceled(InputAction.CallbackContext obj) => inputController.TapCancel(obj);
        

        private void OnTapPerformed(InputAction.CallbackContext obj) => inputController.Tapped(obj);
        

        private void UnSubscribeEvents()
        {
            _gameInput.Input.Tap.performed -= OnTapPerformed;
            _gameInput.Input.Tap.canceled -= OnTapCanceled;
            _gameInput.Disable();
        }

        private void OnDisable()
        {
            UnSubscribeEvents();
        }
    }
}