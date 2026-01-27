using System.Collections.Generic;
using RunTime.Controllers;
using RunTime.Enums;
using RunTime.Managers;
using RunTime.Signals;
using UnityEngine;

namespace RunTime.Interfaces.Ability
{
    public class FireCrackerAbility : IAbility
    {
        private AbilityManager Manager;
        private List<Transform> _children = new List<Transform>();
        private Dictionary<Vector2Int,Transform> _positionMap = new Dictionary<Vector2Int, Transform>();
        private HashSet<Transform> _visited = new HashSet<Transform>();
        private List<List<Transform>> _clusters = new List<List<Transform>>();
        

        public FireCrackerAbility(AbilityManager abilityManager)
        {
            Manager = abilityManager;
        }
        public void OnEnterAbility()
        {
            var selectedObject = Manager.SelectedObject;
            
            if (selectedObject == null) return;

            Transform parent = selectedObject.parent;
            
           
            foreach (Transform child in parent)
            {
                if (child != selectedObject) _children.Add(child);
            }

            AbilitySignals.Instance.onRemoveOccupiedGrid?.Invoke(new Vector2Int( Mathf.RoundToInt(selectedObject.position.x),
                Mathf.RoundToInt(selectedObject.position.z)), false);
            Object.Destroy(selectedObject.gameObject);

            if (_children.Count <= 1)
            {
                Debug.Log("Single Block Destroyed");
                return;
            }


            foreach (var c in _children)
            {
                Vector2Int gridPos = new Vector2Int(Mathf.RoundToInt(c.localPosition.x),
                    Mathf.RoundToInt(c.localPosition.z));
                _positionMap[gridPos] = c;
            }


           
            foreach (var start in _children)
            {
                if (_visited.Contains(start)) continue;

                List<Transform> cluster = new List<Transform>();
                Queue<Transform> queue = new Queue<Transform>();

                _visited.Add(start);
                queue.Enqueue(start);

                while (queue.Count > 0)
                {
                    var current = queue.Dequeue();
                    cluster.Add(current);

                    Vector2Int currentPos = new Vector2Int(Mathf.RoundToInt(current.localPosition.x),
                        Mathf.RoundToInt(current.localPosition.z));

                    var directions = GetDirections();
                    foreach (var dir in directions)
                    {
                        Vector2Int neighborPos = currentPos + dir;

                        if (_positionMap.TryGetValue(neighborPos, out var neighbor))
                        {
                            if (!_visited.Contains(neighbor))
                            {
                                _visited.Add(neighbor);
                                queue.Enqueue(neighbor);
                            }
                        }
                    }
                }

                _clusters.Add(cluster);
            }


            if (_clusters.Count <= 1)
            {
                parent.gameObject.transform.position = _clusters[0][0].position;
                Debug.Log("No clusters formed, single group remains.");
                return;
            }
            foreach (var cluster in _clusters)
            {
                GameObject newParent = new GameObject("BlockGroup");
                newParent.transform.eulerAngles = parent.eulerAngles;
                var component = newParent.AddComponent<Block>();
                component.SetBlockSize( cluster.Count);
                newParent.transform.position = cluster[0].position;




                
                foreach (var block in cluster)
                {
                 
                    block.SetParent(newParent.transform, true);
                 
                }
            }

            Object.Destroy(parent.gameObject);
            OnExitAbility();
        }


        private List<Vector2Int> GetDirections()
        {
            return new List<Vector2Int>() {  Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right  };
        }
        

        public void OnExitAbility()
        {
            GameSignals.Instance.onChangeGameState?.Invoke(GameState.Game);
            Manager.SelectedObject = null;
            _children.Clear();
            _positionMap.Clear();
            _visited.Clear();
            _clusters.Clear();
        }
    }
}