using System.Collections.Generic;
using RunTime.Managers;
using Sirenix.OdinInspector;
using Unity.VisualScripting;
using UnityEngine;

namespace RunTime.Controllers
{
    public class AbilityController : MonoBehaviour
    {
        #region Self Variables

        #region Serialized Variables

       

        #endregion

        #region Private Variables

        
        #endregion

        #endregion

        [Button("Use Ability")]
        private void UsedAbility(Transform selectedObject)
        {
            if (selectedObject == null) return;

            Transform parent = selectedObject.parent;
            
            List<Transform> children = new List<Transform>();
            foreach (Transform child in parent)
            {
                if (child != selectedObject) children.Add(child);
            }

          
            var gridManager = FindAnyObjectByType<GridManager>();
            gridManager.ChangeOccupiedCell( new Vector2Int(Mathf.RoundToInt(selectedObject.position.x),
                Mathf.RoundToInt(selectedObject.position.z)), false);
            Destroy(selectedObject.gameObject);

            if (children.Count <= 1)
            {
                Debug.Log("Single Block Destroyed");
                return;
            }

           
            Dictionary<Vector2Int, Transform> posMap = new Dictionary<Vector2Int, Transform>();
            foreach (var c in children)
            {
                Vector2Int gridPos = new Vector2Int(Mathf.RoundToInt(c.localPosition.x),
                    Mathf.RoundToInt(c.localPosition.z));
                posMap[gridPos] = c;
            }

            HashSet<Transform> visited = new HashSet<Transform>();
            List<List<Transform>> clusters = new List<List<Transform>>();

           
            foreach (var start in children)
            {
                if (visited.Contains(start)) continue;

                List<Transform> cluster = new List<Transform>();
                Queue<Transform> queue = new Queue<Transform>();

                visited.Add(start);
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

                        if (posMap.TryGetValue(neighborPos, out var neighbor))
                        {
                            if (!visited.Contains(neighbor))
                            {
                                visited.Add(neighbor);
                                queue.Enqueue(neighbor);
                            }
                        }
                    }
                }

                clusters.Add(cluster);
            }


            if (clusters.Count <= 1)
            {
                parent.gameObject.transform.position = clusters[0][0].position;
                Debug.Log("No clusters formed, single group remains.");
                return;
            }
            foreach (var cluster in clusters)
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

            Destroy(parent.gameObject);
        }


        private List<Vector2Int> GetDirections()
        {
            return new List<Vector2Int>() {  Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right  };
        }
    }
}