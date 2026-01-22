using System;
using System.Collections.Generic;
using RunTime.Data.UnityObject;
using RunTime.Enums;
using RunTime.Keys;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    public class LevelEditor : EditorWindow
    {
        #region Self Variables

        #region Data Variables

        public CD_LevelGridData LevelData { get; set; }
        public CD_BlockData BlockData { get; set; }
        public CD_ColorData ColorData { get; set; }

        #endregion

        #region Property Variables

        public BlockType SelectedBlockType { get; set; }
        public BlockColorType SelectedColorType { get; set; }
        public float CurrentRotationY { get; set; }
        public int Row { get; set; }
        public int Column { get; set; }
        public int LevelID { get; set; }
        
        public GameObject Obstacle { get; set; }
        public GameObject ObstacleCorner { get; set; }
        
        public GameObject Ground { get; set; }
        public Transform LevelParent { get; set; }


        public Dictionary<Vector2Int, bool> ActiveCellDic { get; set; } = new Dictionary<Vector2Int, bool>();

        public Dictionary<List<Vector2Int>, GameObject> BlockDic { get; set; } =
            new Dictionary<List<Vector2Int>, GameObject>();
        
        public List<GameObject> GroundBlockList { get; set; } = new List<GameObject>();

        private VisualGridDrawer _visualGridDrawer;
        private PrefabInspector _prefabInspector;
        private CreateLevelGrid _createLevelGrid;

        private Vector2 _scrollView;
        public PreviewRenderUtility _previewUtility { get; set; }
        #endregion

        #endregion

        [MenuItem("Tools/Level Editor")]
        public static void Open()
        {
            GetWindow<LevelEditor>("Level Editor");
        }

        private void OnEnable()
        {
            _visualGridDrawer = new VisualGridDrawer(this);
            _prefabInspector = new PrefabInspector(this);
            _createLevelGrid = new CreateLevelGrid(this);
            
        }
        private void OnDisable()
        {
            if (_previewUtility != null)
            {
                _previewUtility.Cleanup();
                _previewUtility = null;
            }
        }

        private void OnGUI()
        {
            GUILayout.BeginHorizontal();

            GUILayout.BeginVertical(GUILayout.Width(300));

            GUILayout.Space(20);

            DrawObjectField();

            GUILayout.EndVertical();


            GUILayout.Box("", GUILayout.Width(1), GUILayout.ExpandHeight(true));


            GUILayout.BeginVertical();

            GUILayout.Space(20);
            GUILayout.Label("Grid Visual", EditorStyles.boldLabel, GUILayout.Width(100));
            _scrollView = GUILayout.BeginScrollView(_scrollView, false, true, GUILayout.ExpandHeight(true),
                GUILayout.Height(400));
            _visualGridDrawer.DrawGridWithOutline();
            GUILayout.EndScrollView();
            if (GUILayout.Button("Clear Grid", GUILayout.ExpandWidth(true), GUILayout.Height(30)))
            {
                ActiveCellDic.Clear();
                foreach (var block in BlockDic.Values)
                {
                    DestroyImmediate(block);
                }

                BlockDic.Clear();

            }

            GUILayout.EndVertical();



            GUILayout.EndHorizontal();



        }



        private void DrawObjectField()
        {
            DrawDescriptionText("Data Settings");
            DrawBox(5);
            EditorGUIUtility.labelWidth = 100;
            LevelData = (CD_LevelGridData)EditorGUILayout.ObjectField("Level Data", LevelData, typeof(CD_LevelGridData),
                true, GUILayout.Width(200));
            BlockData = (CD_BlockData)EditorGUILayout.ObjectField("Block Data", BlockData, typeof(CD_BlockData), true,
                GUILayout.Width(200));
            ColorData = (CD_ColorData)EditorGUILayout.ObjectField("Color Data", ColorData, typeof(CD_ColorData), true,
                GUILayout.Width(200));
            DrawBox(5);
            DrawDescriptionText("Block Settings");
            GUILayout.Box("", GUILayout.ExpandWidth(true), GUILayout.Height(5));
            SelectedBlockType =
                (BlockType)EditorGUILayout.EnumPopup("Block Type", SelectedBlockType, GUILayout.Width(200));
            SelectedColorType =
                (BlockColorType)EditorGUILayout.EnumPopup("Block Color", SelectedColorType, GUILayout.Width(200));
            Obstacle = (GameObject)EditorGUILayout.ObjectField("Obstacle", Obstacle, typeof(GameObject), true,
                GUILayout.Width(200));
            ObstacleCorner = (GameObject)EditorGUILayout.ObjectField("Obstacle Corner", ObstacleCorner, typeof(GameObject), true,
                GUILayout.Width(200));
            Ground = (GameObject)EditorGUILayout.ObjectField("Ground", Ground, typeof(GameObject), true,
                GUILayout.Width(200));
            
            DrawBox(5);
            DrawDescriptionText("Block Rotation");
            EditorGUILayout.BeginHorizontal();
            {
                DrawRotationButton(0f, "0°");
                DrawRotationButton(90f, "90°");
                DrawRotationButton(180f, "180°");
                DrawRotationButton(270f, "270°");
            }
            EditorGUILayout.EndHorizontal();
            DrawBox(5);


            DrawDescriptionText("Grid Settings");
            Row = EditorGUILayout.IntField("Row", Row, GUILayout.Width(200));
            Column = EditorGUILayout.IntField("Column", Column, GUILayout.Width(200));
            DrawBox(5);
            DrawDescriptionText("Level Settings");
            LevelID = EditorGUILayout.IntField("Level", LevelID, GUILayout.Width(200));
            LevelParent = (Transform)EditorGUILayout.ObjectField("Level Parent", LevelParent, typeof(Transform), true,
                GUILayout.Width(200));
            DrawBox(5);
            DrawDescriptionText("Create Level Grid");
            DrawBox(2);

            GUILayout.BeginHorizontal();

            if (GUILayout.Button("Create", GUILayout.ExpandWidth(true), GUILayout.Height(30)))
            {
                _createLevelGrid.Create();
            }

            if (GUILayout.Button("Clear", GUILayout.ExpandWidth(true), GUILayout.Height(30)))
            {
                _createLevelGrid.Clear();
            }

            GUILayout.EndHorizontal();
            DrawBox(5);
            DrawDescriptionText("Save & Load Level Data");
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Save", GUILayout.ExpandWidth(true), GUILayout.Height(30)))
            {
            }

            if (GUILayout.Button("Load ", GUILayout.ExpandWidth(true), GUILayout.Height(30)))
            {
            }

            GUILayout.EndHorizontal();

            _prefabInspector.DrawRotatedPreview(BlockData.Blocks[(int)SelectedBlockType].Prefab, CurrentRotationY);
        }

        
        private void DrawRotationButton(float angle, string label)
        {
           
            Color defaultColor = GUI.backgroundColor;

          
            if (Mathf.Approximately(CurrentRotationY, angle))
            {
                GUI.backgroundColor = Color.green; 
            }
            else
            {
                GUI.backgroundColor = defaultColor; 
            }
            
            if (GUILayout.Button(label, GUILayout.ExpandWidth(true), GUILayout.Width(80)))
            {
                CurrentRotationY = angle;
            }
          
            GUI.backgroundColor = defaultColor;
        }

        
        private void DrawBox(float height)
        {

            GUILayout.Box("", GUILayout.ExpandWidth(true), GUILayout.Height(height));

        }

        private void DrawDescriptionText(string text)
        {
            GUIStyle style = new GUIStyle(EditorStyles.boldLabel);
            style.alignment = TextAnchor.MiddleCenter;
            GUILayout.Label(text, style, GUILayout.ExpandWidth(true));
        }


        public float CornerAngle(Vector2Int cornerCell)
        {
            if(cornerCell.x == -1 && cornerCell.y == -1)
                return 0f; // Bottom-left corner
            if(cornerCell.x == -1 && cornerCell.y == Column)
                return 90f; // Top-left corner
            if(cornerCell.x == Row && cornerCell.y == -1)
                return 270f; // Bottom-right corner
            if(cornerCell.x == Row && cornerCell.y == Column)
                return 180f; // Top-right corner
            throw new ArgumentException("The provided cell is not a corner cell.");
        }

        public bool IsCornerCell(Vector2Int cell)
        {
            return (cell.x == -1 && cell.y == -1) ||
                   (cell.x == -1 && cell.y == Column) ||
                   (cell.x == Row && cell.y == -1) ||
                   (cell.x == Row  && cell.y == Column);
        }

        public bool IsOutlineCell(int x, int z, int totalRows, int totalCols)
        {
            return x == 0 || z == 0 ||
                   x == totalRows - 1 || z == totalCols - 1;
        }
    }
}