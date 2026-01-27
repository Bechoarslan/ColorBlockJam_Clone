using System.Collections.Generic;
using RunTime.Controllers;
using RunTime.Data.UnityObject;
using RunTime.Data.ValueObjects;
using RunTime.Enums;
using RunTime.Interfaces;
using RunTime.Keys;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

namespace Editor
{
    public class VisualGridDrawer
    {
        private LevelEditor Editor;
        public VisualGridDrawer(LevelEditor levelEditor)
        {
            Editor = levelEditor;
            
        }

        
        
        public void DrawGridWithOutline()
        {
            int totalRows = Editor.Row + 2;
            int totalCols = Editor.Column + 2;
          

            for (int x = totalRows - 1; x >= 0; x--)
            {
                GUILayout.BeginHorizontal();

                for (int z = totalCols - 1; z >= 0; z--)
                {
                    bool isOutline = Editor.IsOutlineCell( x, z, totalRows, totalCols);
                 
                      

                    Color oldColor = GUI.backgroundColor;

                    if (isOutline)
                    {
                        Vector2Int outlineCell = new Vector2Int(x - 1 , z - 1);
                        Editor.ActiveCellDic.TryAdd(outlineCell, false);
                        var isActiveCell = Editor.ActiveCellDic[outlineCell];
                        var buttonText = $"{outlineCell.x} , {outlineCell.y}";
                        GUI.backgroundColor = isActiveCell ? Color.red : Color.black;


                        if (GUILayout.Button(buttonText, GUILayout.Width(50), GUILayout.Height(50)))
                        {
                            if (Event.current.button == 0)
                            {
                                AddObstacle(outlineCell);
                            }
                            else if (Event.current.button == 1)
                            {
                                RemoveObstacle(outlineCell);
                            }
                        };
                    }
                    else
                    {
                      
                        Vector2Int gridCell = new Vector2Int(x - 1, z - 1);

                        Editor.ActiveCellDic.TryAdd(gridCell, false);
                        bool isActive = Editor.ActiveCellDic[gridCell];

                        GUI.backgroundColor = isActive ? Color.green : Color.gray;

                        if (GUILayout.Button(
                                $"({gridCell.x},{gridCell.y})",
                                GUILayout.Width(50),
                                GUILayout.Height(50)))
                        {
                            if (Event.current.button == 0)
                                AddBlock(gridCell);
                            else if (Event.current.button == 1)
                                RemoveCell(gridCell);
                        }
                    }

                    GUI.backgroundColor = oldColor;
                }

                GUILayout.EndHorizontal();
            }
        }

        private void RemoveObstacle(Vector2Int cell)
        {
            foreach (var value in Editor.BlockDic.Keys)
            {
                if (value.Contains(cell))
                {
                    UnityEngine.Object.DestroyImmediate(Editor.BlockDic[value]);
                    Editor.BlockDic.Remove(value);
                    Editor.ActiveCellDic[cell] = false;
                    break;
                }
            }
        }

        private void AddObstacle(Vector2Int cell)
        {
            
            if (Editor.ActiveCellDic[cell])
            {
                return;
            }
            var obj = (GameObject)PrefabUtility.InstantiatePrefab(Editor.Obstacle,Editor.LevelMeshes.transform);
            obj.transform.position = new Vector3(cell.x,0.5f, cell.y);
            Editor.ActiveCellDic[cell] = true;
            var cellList = new List<Vector2Int> { cell };
            Editor.BlockDic.Add(cellList, obj);
        }


        private void AddBlock(Vector2Int cell)
        {
            if (Editor.LevelBlocks is null)
            {
                Editor.LevelBlocks = new GameObject("LevelBlocks");
                Editor.LevelBlocks.transform.SetParent(Editor.LevelTransform.transform);
            }
            var blockVectorListKeys = new BlockVectorListKeys();
                            var blockVectors =
                                blockVectorListKeys.OnRegisterVectorList(
                                    Editor.SelectedBlockType,
                                    Editor.CurrentRotationY
                                );
            
            bool canPlace = true;
            List<Vector2Int> targetCells = new List<Vector2Int>();
            targetCells.Add(cell);

            foreach (var vector in blockVectors)
            {
                                    
                Vector2Int targetCell =
                    new Vector2Int(cell.x + vector.x, cell.y + vector.y);
                                    
                                    
           
                if (!Editor.ActiveCellDic.ContainsKey(targetCell))
                {
                    canPlace = false;
                    break;
                }
                
                if (Editor.ActiveCellDic[targetCell])
                {
                    canPlace = false;
                    break;
                }

                targetCells.Add(targetCell);
            }

         
            if (!canPlace)
            {
                                    
                return;
            }
                                
   
            foreach (var targetCell in targetCells)
            {
                                    
                Editor.ActiveCellDic[targetCell] = true; 
            }
            
            var obj = (GameObject)PrefabUtility.InstantiatePrefab(Editor.BlockData.Blocks[(int)Editor.SelectedBlockType].Prefab,Editor.LevelTransform.transform);
            for (int i = obj.transform.childCount - 1; i >= 0; i--)
            {
                var child = obj.transform.GetChild(i);
                var renderer = child.GetComponent<MeshRenderer>();
                renderer.sharedMaterial =
                    Editor.ColorData.ColorData[(int)Editor.SelectedColorType].Material;
                EditorUtility.SetDirty(renderer);
                
            }

            var block = obj.GetComponent<Block>();
           block.SetBlockSize( obj.transform.childCount);
              block.SetBlockType(Editor.SelectedBlockType);
            block.SetColorType(Editor.SelectedColorType);
            
            obj.transform.position = new Vector3(cell.x,1, cell.y);
            obj.transform.rotation = Quaternion.Euler(0, Editor.CurrentRotationY, 0);
         
            
            Editor.BlockDic.Add(targetCells, obj);
            EditorUtility.SetDirty(block);
            EditorUtility.SetDirty(obj);
        }

        private void RemoveCell(Vector2Int cell)
        {
            foreach (var list in Editor.BlockDic.Keys)
            {
                if (list.Contains(cell))
                {
                                    
                    foreach (var targetCell in list)
                    {
                        Editor.ActiveCellDic[targetCell] = false; // griye boya
                    }
                    
                    
                    UnityEngine.Object.DestroyImmediate(Editor.BlockDic[list]);
                    Editor.BlockDic.Remove(list);
                   
                   
                            
                    break;
                }
            }
            
            
        }
        
    }
}