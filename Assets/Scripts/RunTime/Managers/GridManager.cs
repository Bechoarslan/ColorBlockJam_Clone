using System;
using System.Collections.Generic;
using RunTime.Controllers;
using RunTime.Data.UnityObject;
using RunTime.Data.ValueObjects;
using RunTime.Enums;
using RunTime.Interfaces;
using RunTime.Signals;
using RunTime.Systems;
using Sirenix.OdinInspector;
using Unity.VisualScripting;
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

            
            
            
            
       
        }

        private void Start()
        {
           _blockDictionary.AddRange(BlockRegistry.GetRegistry());
           Debug.Log(_blockDictionary.Count);
        }

        private void OnEnable()
        {
            SubscribeEvents();
        }

        private void SubscribeEvents()
        {
            AbilitySignals.Instance.onRemoveObjectDestroyedByVacuum += OnRemoveObjectDestroyedByHammer;
            AbilitySignals.Instance.onRemoveObjectDestroyedByHammer += OnRemoveObjectDestroyedByHammer;
            AbilitySignals.Instance.onRemoveOccupiedGrid += ChangeOccupiedCell;
            InputSignals.Instance.onSendInputParams += blockMover.OnGetInputParams;
            InputSignals.Instance.onSendSelectedObject += blockMover.OnGetSelectedObject;
            InputSignals.Instance.onSelectedObjectReleased += blockMover.OnSelectedObjectReleased;
        }

        private void OnRemoveObjectDestroyedByHammer(BlockColorType type)
        {
            if(_blockDictionary.TryGetValue(type ,out List<GameObject> blockList))
            {
                foreach (var block in blockList)
                {
                    for (int i = block.transform.childCount; i > 0; i--)
                    {
                        var child = block.transform.GetChild(i - 1).gameObject;
                        ChangeOccupiedCell(new Vector2Int(Mathf.RoundToInt(child.transform.position.x),
                            Mathf.RoundToInt(child.transform.position.z)), false);
                    }
                    Destroy(block);
                }
                blockList.Clear();
            }
        }

        private void OnRemoveObjectDestroyedByHammer(GameObject block,BlockColorType type)
        {
           
            if (!_blockDictionary.TryGetValue(type, out List<GameObject> blockList)) return;
            foreach (var obj in blockList)
            {
                Debug.Log(obj.name);
            }
            if (!blockList.Contains(block)) return;
          
            for (int i = block.transform.childCount; i > 0; i--)
            { 
               var child = block.transform.GetChild(i-1).gameObject;
               ChangeOccupiedCell( new Vector2Int(Mathf.RoundToInt(child.transform.position.x),Mathf.RoundToInt(child.transform.position.z)),false);
            }
            blockList.Remove(block);
            Destroy(block);
        }

        

        private void UnSubscribeEvents()
        {
            AbilitySignals.Instance.onRemoveObjectDestroyedByVacuum -= OnRemoveObjectDestroyedByHammer;
            AbilitySignals.Instance.onRemoveObjectDestroyedByHammer -= OnRemoveObjectDestroyedByHammer;
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