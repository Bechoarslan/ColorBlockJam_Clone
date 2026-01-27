using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;


namespace Editor
{
    public class CreateLevelGrid
    {
        private LevelEditor Editor;

        public CreateLevelGrid(LevelEditor editor)
        {
            Editor = editor;
        }
        
        
        public void Create()
        {
            Editor.LevelTransform = new GameObject("LevelParent");
            Editor.LevelMeshes = new GameObject("LevelMeshes");
            Editor.LevelMeshes.transform.SetParent( Editor.LevelTransform.transform);
            
            var totalColumn = Editor.Column + 2;
            var totalRow = Editor.Row + 2;
            for (int x = totalRow- 1; x >= 0; x--)
            {
                for (int z = totalColumn - 1; z >= 0; z--)
                {
                    bool isOutline = Editor.IsOutlineCell( x, z, totalRow, totalColumn);
                    if (isOutline)
                    {
                        Vector2Int outlineCell = new Vector2Int(x - 1, z - 1);
                        var cellList = new List<Vector2Int> { outlineCell };
                        var obstacle = Editor.IsCornerCell( outlineCell) ?
                            PrefabUtility.InstantiatePrefab(Editor.ObstacleCorner, Editor.LevelMeshes.transform) as GameObject :
                            PrefabUtility.InstantiatePrefab(Editor.Obstacle, Editor.LevelMeshes.transform) as GameObject;
                        Editor.ActiveCellDic[outlineCell] = true;
                        Editor.BlockDic.Add(cellList,obstacle);
                        if (obstacle != null)
                        {
                            obstacle.transform.position = new Vector3(outlineCell.x, 0.75f, outlineCell.y);
                            if (Editor.IsCornerCell(outlineCell))
                            {
                                obstacle.transform.position = new Vector3(outlineCell.x, 0.5f, outlineCell.y);
                                obstacle.transform.rotation = Quaternion.Euler(0, Editor.CornerAngle(outlineCell), 0);
                            }
                        }
                    }
                    else
                    {
                        Vector2Int innerCell = new Vector2Int(x - 1, z - 1);
                        var ground = PrefabUtility.InstantiatePrefab(Editor.Ground, Editor.LevelMeshes.transform) as GameObject;
                        Editor.GroundBlockList.Add(ground);
                        if (ground != null)
                        {
                            ground.transform.position = new Vector3(innerCell.x, 0, innerCell.y);
                        }
                    }
                }
            }
        }


        public void Clear()
        {
           
            var totalColumn = Editor.Column + 2;
            var totalRow = Editor.Row + 2;
            for (int x = totalRow- 1; x >= 0; x--)
            {
                for (int z = totalColumn - 1; z >= 0; z--)
                {
                    bool isOutline = Editor.IsOutlineCell( x, z, totalRow, totalColumn);
                    if (isOutline)
                    {  
                        Vector2Int outlineCell = new Vector2Int(x - 1, z - 1);
                  
                        foreach (var value in Editor.BlockDic.Keys)
                        {
                            if (value.Contains(outlineCell))
                            {
                                UnityEngine.Object.DestroyImmediate(Editor.BlockDic[value]);
                                Editor.BlockDic.Remove(value);
                                Editor.ActiveCellDic[outlineCell] = false;
                                break;
                            }
                        }
                        
                       
                    }
                    
                    
                }
            }

            Object.DestroyImmediate(Editor.LevelTransform);
            
        }
    }
}