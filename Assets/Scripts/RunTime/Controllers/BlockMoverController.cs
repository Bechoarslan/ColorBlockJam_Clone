using System.Collections.Generic;
using RunTime.Interfaces;
using RunTime.Keys;
using RunTime.Managers;
using UnityEngine;

namespace RunTime.Controllers
{
    public class BlockMoverController : MonoBehaviour
    {
        [SerializeField] private GridManager gridManager;

        private Transform _selectedObject;
        private List<Vector2Int> _collisionOffsets = new List<Vector2Int>();

        public void OnGetSelectedObject(GameObject selectedObj)
        {
            _selectedObject = selectedObj.transform;
            _collisionOffsets.Clear();
            
            // Main object is always at (0,0) relative to itself

            // Register Group Shape from BlockVectorListKeys relative to main object
            var blockComp = selectedObj.GetComponent<IBlock>();
            if (blockComp != null)
            {
                var relativeOffsets = new BlockVectorListKeys().OnRegisterVectorList(blockComp.BlockType,_selectedObject.transform.eulerAngles.y);
                if(relativeOffsets != null)
                {
                    _collisionOffsets.AddRange(relativeOffsets);
                }
            }
            
        
            // Un-occupy current cells for the whole group
            Vector2Int currentGridPos = new Vector2Int(Mathf.RoundToInt(_selectedObject.position.x), Mathf.RoundToInt(_selectedObject.position.z));
            foreach (var offset in _collisionOffsets)
            {
                gridManager.ChangeOccupiedCell(currentGridPos + offset, false);
            }
        }

        public void OnGetInputParams(InputParamKeys inputParamKeys)
        {
            if (_selectedObject == null) return;
            
            Vector3 currentPos = _selectedObject.position;
            Vector3 targetInput = new Vector3(inputParamKeys.InputVector.x, currentPos.y, inputParamKeys.InputVector.y);

            // Calculate valid position for the PARENT object. 
            // The collision check inside creates the group virtually using offsets.
            float nextX = GetValidGroupPosition(currentPos.x, targetInput.x, currentPos.z, true);
           // float nextZ = GetValidGroupPosition(currentPos.z, targetInput.z, nextX, false);

            _selectedObject.position = new Vector3(nextX, currentPos.y, 0);
        }

        private float GetValidGroupPosition(float currentAnchorVal, float targetAnchorVal, float otherAxisAnchorVal, bool isXAxis)
        {
            int currentAnchorGridIdx = Mathf.RoundToInt(currentAnchorVal);
            Debug.Log("Selected Object X Value:" + currentAnchorGridIdx + " Target X Value:" + targetAnchorVal);
            float diff = targetAnchorVal - currentAnchorGridIdx;
            
           
            if (Mathf.Abs(diff) < 0.01f)
            {
                
                return targetAnchorVal;
            }
        
            int dir = (diff > 0) ? 1 : -1;
            
            // Check immediate neighbor blockage to prevent entering occupied cells
            int nextCellIndex = currentAnchorGridIdx + dir;
            if (IsGroupBlocked(nextCellIndex, otherAxisAnchorVal, isXAxis))
            {
                Debug.Log("Blocked at immediate neighbor cell:" + nextCellIndex);
                return currentAnchorGridIdx;
            }

            int targetAnchorGridIdx = Mathf.RoundToInt(targetAnchorVal);
            int steps = Mathf.Abs(targetAnchorGridIdx - currentAnchorGridIdx);
            
            int lastValidGridIdx = currentAnchorGridIdx;
            
            for (int i = 1; i <= steps; i++)
            {
                int checkGridIdx = currentAnchorGridIdx + (dir * i);

                if (IsGroupBlocked(checkGridIdx, otherAxisAnchorVal, isXAxis))
                {
                    return lastValidGridIdx; 
                }
                lastValidGridIdx = checkGridIdx;
            }
            
            return targetAnchorVal;
        }

        private bool IsGroupBlocked(float anchorMainAxisVal, float anchorOtherAxisVal, bool isXAxis)
        {
            int mainIdx = Mathf.RoundToInt(anchorMainAxisVal);
            int otherIdx = Mathf.RoundToInt(anchorOtherAxisVal); 

            // Check if ANY part of the group (defined by _collisionOffsets) would land on an occupied cell
            for (int i = 0; i < _collisionOffsets.Count; i++)
            {
                Vector2Int offset = _collisionOffsets[i];
                
                int checkX = isXAxis ? mainIdx + offset.x : otherIdx + offset.x; 
                int checkZ = isXAxis ? otherIdx + offset.y : mainIdx + offset.y; // offset.y corresponds to Z in world

                Vector2Int checkGridPos = new Vector2Int(checkX, checkZ);

                if (gridManager.IsCellOccupied(checkGridPos))
                {
                    return true;
                }
            }
            return false;
        }

        public void OnSelectedObjectReleased()
        {
            if (_selectedObject != null)
            {
                // Snap parent to nearest grid
                Vector2Int releasedGridPos = new Vector2Int(Mathf.RoundToInt(_selectedObject.position.x), Mathf.RoundToInt(_selectedObject.position.z));
                _selectedObject.position = new Vector3(releasedGridPos.x, _selectedObject.position.y, releasedGridPos.y);
                
                // Re-occupy all cells
                foreach (var offset in _collisionOffsets)
                {
                    gridManager.ChangeOccupiedCell(releasedGridPos + offset, true);
                }
                
                _selectedObject = null;
                _collisionOffsets.Clear();
            }
        }
    }
}