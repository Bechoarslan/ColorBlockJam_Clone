using System;
using System.Collections.Generic;
using RunTime.Controllers;
using RunTime.Data.UnityObject;
using RunTime.Data.ValueObjects;
using RunTime.Signals;
using Sirenix.OdinInspector;
using UnityEngine;

namespace RunTime.Managers
{
    public class GridManager : MonoBehaviour
    {
        #region Self Variables

        #region Serialized Variables

        [SerializeField] private CD_LevelData levelData;
        [SerializeField] private Transform gridParent;
        [SerializeField] private BlockMoverController blockMover;
      

        #endregion

        #region Private Variables
        [SerializeField] private Dictionary<Vector2Int,bool> _gridDictionary = new Dictionary<Vector2Int, bool>();

        #endregion

        #endregion

        private void Awake()
        {
            GetData();
        }

        private void GetData()
        {
            var data = levelData.Levels[0].levels;
            foreach (var levelData in data)
            {
                _gridDictionary.Add(new Vector2Int((int)levelData.GridPosition.x,(int)levelData.GridPosition.y),levelData.IsOccupied);
            }
        }

        private void OnEnable()
        {
            SubscribeEvents();
        }

        private void SubscribeEvents()
        {
            InputSignals.Instance.onSendInputParams += blockMover.OnGetInputParams;
            InputSignals.Instance.onSendSelectedObject += blockMover.OnGetSelectedObject;
            InputSignals.Instance.onSelectedObjectReleased += blockMover.OnSelectedObjectReleased;
        }

        private void UnSubscribeEvents()
        {
            InputSignals.Instance.onSendInputParams -= blockMover.OnGetInputParams;
            InputSignals.Instance.onSendSelectedObject -= blockMover.OnGetSelectedObject;
            InputSignals.Instance.onSelectedObjectReleased -= blockMover.OnSelectedObjectReleased;
        }

        private void OnDisable()
        {
            UnSubscribeEvents();
        }
        

        public void ChangeOccupiedCell(Vector2Int gridPosition,bool occupied)
        {
            if (_gridDictionary.ContainsKey(gridPosition))
            {
                _gridDictionary[gridPosition] = occupied;
            }
        }

        [Button]
        public bool IsCellOccupied(Vector2 gridPos)
        {
             var newGridPos = new Vector2Int(Mathf.RoundToInt(gridPos.x),Mathf.RoundToInt( gridPos.y));
            if (_gridDictionary.TryGetValue(newGridPos, out var occupied))
            {
                return occupied;
            }
            return true;
          
            
        }


        
    }
}