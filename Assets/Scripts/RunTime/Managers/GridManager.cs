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

        [SerializeField] private CD_LevelGridData levelGridData;
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
            var data = levelGridData.Levels[0].levels;
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
        private void CreateGrid()
        {
            if (levelGridData.Levels[0].levels.Count > 0)
            {
                levelGridData.Levels[0].levels.Clear();
            }
            var data = levelGridData.Levels[0];
            var column = data.Column;
            var row = data.Row;
            for (int z = row - 1 ; z >= 0 ; z--) 
            {
                for (int x = 0; x < column; x++)
                {
                    var cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                   
                    cube.transform.parent = gridParent;
                    cube.transform.localPosition = new Vector3(x ,0, z);
                    var getRandomBool = Convert.ToBoolean(UnityEngine.Random.Range(0, 2));
                    var newData = new LevelGridData
                    {
                        GridPosition = new Vector2(x, z),
                        IsOccupied = getRandomBool
                    };
                    _gridDictionary.Add(new Vector2Int(x,z), getRandomBool);
                    if (getRandomBool)
                    {
                        var newCube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                        newCube.transform.position = new Vector3(x, 0.5f, z);
                    }
                    data.levels.Add(newData);
                }
            }

        }

        public void DeleteGridPos(Vector2Int gridPosition)
        {
            if (_gridDictionary.ContainsKey(gridPosition))
            {
                _gridDictionary[gridPosition] = false;
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