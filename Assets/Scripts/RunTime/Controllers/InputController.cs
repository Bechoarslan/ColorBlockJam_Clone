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
        private Camera _mainCamera;
        private bool _isTapped;
        private Plane _groundPlane = new Plane(Vector3.up, Vector3.zero);
        #endregion

        #endregion

        public void Tapped(InputAction.CallbackContext ctx)
        {
            
        }

       

        public void TapCancel(InputAction.CallbackContext ctx)
        {
            _isTapped  = false;
        }

        

        public void GetReferences(Camera mainCamera, GameInput gameInput)
        {
            _mainCamera = mainCamera;
            _gameInput = gameInput;
        }
    }
}