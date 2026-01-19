using RunTime.Keys;
using RunTime.Managers;
using UnityEngine;

namespace RunTime.Controllers
{
    public class BlockMoverController : MonoBehaviour
    {
        [SerializeField] private GridManager gridManager;

        private Transform _selectedObject;
   
        public void OnGetSelectedObject(GameObject selectedObj)
        {
            _selectedObject = selectedObj.transform;
         
        }

        public void OnGetInputParams(InputParamKeys inputParamKeys)
        {
            if (_selectedObject == null) return;

            Vector3 currentPos = _selectedObject.position;
            Vector3 targetInput = new Vector3(inputParamKeys.InputVector.x, currentPos.y, inputParamKeys.InputVector.y);

            // Resolve X Axis Movement
            float nextX = GetValidEndPosition(currentPos.x, targetInput.x, currentPos.z, true);

            // Resolve Z Axis Movement (using the new X position for accurate column checking)
            float nextZ = GetValidEndPosition(currentPos.z, targetInput.z, nextX, false);

            _selectedObject.position = new Vector3(nextX, currentPos.y, nextZ);
        }

        private float GetValidEndPosition(float currentVal, float targetVal, float otherAxisVal, bool isXAxis)
        {
            int currentGridIdx = Mathf.RoundToInt(currentVal);
            int otherGridIdx = Mathf.RoundToInt(otherAxisVal);
            
            float diff = targetVal - currentGridIdx;
            
            // If very close to center and no input change, stay.
            if (Mathf.Abs(diff) < 0.01f) return targetVal;

            int dir = (diff > 0) ? 1 : -1;
            
            // Check immediate neighbor blockage
            Vector2Int checkPos = isXAxis 
                ? new Vector2Int(currentGridIdx + dir, otherGridIdx) 
                : new Vector2Int(otherGridIdx, currentGridIdx + dir);
                
            if (gridManager.IsCellOccupied(checkPos))
            {
                // If moving towards blocked neighbor, clamp at the current cell center.
                // This creates a hard stop "snap" effect against the occupied block.
                if (dir == 1 && targetVal > currentGridIdx) return currentGridIdx;
                if (dir == -1 && targetVal < currentGridIdx) return currentGridIdx;
            }
            
            // Handle multi-step fast movement (skipping cells)
            int targetGridIdx = Mathf.RoundToInt(targetVal);
            int steps = Mathf.Abs(targetGridIdx - currentGridIdx);
            
            int checkGridIdx = currentGridIdx;
            for (int i = 0; i < steps; i++)
            {
                checkGridIdx += dir;
                Vector2Int pos = isXAxis
                    ? new Vector2Int(checkGridIdx, otherGridIdx)
                    : new Vector2Int(otherGridIdx, checkGridIdx);

                if (gridManager.IsCellOccupied(pos))
                {
                    // Blocked at checkGridIdx. The last valid pos was checkGridIdx - dir.
                    return (checkGridIdx - dir);
                }
            }
            
            return targetVal;
        }

        public void OnSelectedObjectReleased()
        {
            _selectedObject = null;
        }
    }
}