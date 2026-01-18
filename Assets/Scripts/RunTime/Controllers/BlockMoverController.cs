using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using RunTime.Keys;
using RunTime.Managers;
using UnityEngine;

namespace RunTime.Controllers
{
    public class BlockMoverController : MonoBehaviour
    {
        #region Self Variables

        #region Serialized Variables

        [SerializeField] private float moveSpeed = 0.1f;
        [SerializeField] private GridManager gridManager;

        #endregion

        #region Private Variables

        private Transform _selectedObject;
        private InputParamKeys _inputParamKeys;
        private bool _isReadyToMove;
        
        #endregion

        #endregion

        public void OnGetInputParams(InputParamKeys inputParamKeys)
        {
            _inputParamKeys = inputParamKeys;
            _isReadyToMove = !gridManager.IsCellOccupied(_inputParamKeys.InputVector);
        }

        private void FixedUpdate()
        {
            if (_selectedObject == null || !_isReadyToMove) return;
            Vector3 targetPosition = new Vector3(_inputParamKeys.InputVector.x, _selectedObject.position.y,
                _inputParamKeys.InputVector.y);
            _selectedObject.position = Vector3.MoveTowards(_selectedObject.position, targetPosition,
                moveSpeed * Time.fixedDeltaTime);
           
        }


        public void OnGetSelectedObject(GameObject selectedObj)
        {
          
            _selectedObject = selectedObj.transform;
           
            

            
           
        }


        public void OnSelectedObjectReleased()
        {
            
            
           
        }
    }
}