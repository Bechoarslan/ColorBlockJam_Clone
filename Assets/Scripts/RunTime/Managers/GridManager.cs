using System;
using System.Collections.Generic;
using RunTime.Controllers;
using RunTime.Data.UnityObject;
using RunTime.Data.ValueObjects;
using RunTime.Enums;
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
        [ShowInInspector]
        [SerializeField] private Dictionary<BlockColorType,List<GameObject>> _blockDictionary = new Dictionary<BlockColorType, List<GameObject>>();

        #endregion

        #endregion

        private void Awake()
        {
            GetData();
        }

        private void GetData()
        {
            var data = levelData.Levels[0];
            foreach (var levelData in data.Grids)
            {
                _gridDictionary.Add(new Vector2Int((int)levelData.GridPosition.x,(int)levelData.GridPosition.y),levelData.IsOccupied);
            }

            foreach (var blockData in data.BlockColors)
            {
                _blockDictionary.Add(blockData.BlockColorType,blockData.Block);
            }
            
            
       
        }

        private void OnEnable()
        {
            SubscribeEvents();
        }

        private void SubscribeEvents()
        {
            AbilitySignals.Instance.onRemoveOccupiedGrid += ChangeOccupiedCell;
            InputSignals.Instance.onSendInputParams += blockMover.OnGetInputParams;
            InputSignals.Instance.onSendSelectedObject += blockMover.OnGetSelectedObject;
            InputSignals.Instance.onSelectedObjectReleased += blockMover.OnSelectedObjectReleased;
        }

        private void UnSubscribeEvents()
        {
            AbilitySignals.Instance.onRemoveOccupiedGrid -= ChangeOccupiedCell;
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