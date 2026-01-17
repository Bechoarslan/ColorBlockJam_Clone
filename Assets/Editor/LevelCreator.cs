using System;
using System.Collections.Generic;
using RunTime;
using RunTime.Data.UnityObject;
using RunTime.Data.ValueObjects;
using RunTime.Enums;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    public class LevelCreator : EditorWindow
    {
        #region Self Variables
        private CD_LevelData _currentLevelData;
        private CD_BlockData _blockAssets;
        private CD_ColorData _colorData;
        private Transform _levelParent;

        private BlockType _selectedBlockType;
        private BlockColorType _selectedBlockColorType;

        // Grid Settings
        private int _rows = 5;
        private int _columns = 5;
        private float _cellSize = 1.1f;
        private float _yHeight = 0f;
        private Quaternion _selectedRotation = Quaternion.identity;

        // Visuals
        private GUIStyle _gridButtonStyle;
        private Vector2 _scrollPosition;
        
        // Bu dictionary'yi CreateGrid'de dolduruyoruz ama Save sırasında dinamik güncellemek daha sağlıklı olabilir.
        private Dictionary<Vector2, LevelData> _levelDataDict = new Dictionary<Vector2, LevelData>();
        private Dictionary<Vector2,GameObject> _levelSpawnedCubes = new Dictionary<Vector2, GameObject>();
        
        // --- MAP DATA (Canlı Harita) ---
        // Hangi hücre dolu?
        private bool[,] _occupiedGrid;
        // Hangi hücrede hangi blok var (İsim yazdırmak için)
        private string[,] _occupiedGridNames;
        #endregion

        [MenuItem("Tools/Level Creator")]
        public static void ShowWindow()
        {
            GetWindow<LevelCreator>("Block Jam Editor");
        }

        private void OnGUI()
        {
            // Her frame'de sahneyi analiz et ve gridi güncelle
            CalculateOccupiedCells();

            if (_gridButtonStyle == null)
            {
                _gridButtonStyle = new GUIStyle(GUI.skin.button);
                _gridButtonStyle.margin = new RectOffset(2, 2, 2, 2);
            }

            GUILayout.BeginHorizontal();

            // --- SOL PANEL ---
            GUILayout.BeginVertical(GUILayout.Width(300));
            DrawSettingsPanel();
            GUILayout.EndVertical();

            // Araya çizgi
            GUILayout.Box("", GUILayout.Width(1), GUILayout.ExpandHeight(true));

            // --- SAĞ PANEL ---
            GUILayout.BeginVertical();
            DrawGridButtons();
            GUILayout.EndVertical();

            GUILayout.EndHorizontal();
        }

        // --- 1. SAHNE ANALİZİ (DÜZELTİLDİ) ---
        private void CalculateOccupiedCells()
        {
            // Dizileri sıfırla
            _occupiedGrid = new bool[_columns, _rows];
            _occupiedGridNames = new string[_columns, _rows];

            if (_levelParent == null) return;

            // Sahnedeki tüm objeleri gez
            foreach (Transform child in _levelParent)
            {
                // DÜZELTME 1: Zemin Grid küplerini görmezden gel
                if (child.name.StartsWith("GridCell")) continue;

                // Pivot noktasının grid koordinatı
                int pivotX = Mathf.RoundToInt(child.position.x / _cellSize);
                int pivotZ = Mathf.RoundToInt(child.position.z / _cellSize);
                
                float rotationY = child.eulerAngles.y;

                // DÜZELTME 2: Sahnedeki objenin şekli, menüde seçili olandan farklı olabilir.
                // Objenin isminden veya componentinden gerçek tipini bulmalıyız.
                // Obje ismi formatımız: "BlockType_X_Z" (Örn: LShape_2_3)
                BlockType objectBlockType = _selectedBlockType; // Fallback
                string[] nameParts = child.name.Split('_');
                
                if (nameParts.Length > 0)
                {
                    // İsmin ilk parçası Enum'a dönüşebiliyor mu?
                    if (Enum.TryParse(nameParts[0], out BlockType parsedType))
                    {
                        objectBlockType = parsedType;
                    }
                }

                // Bloğun şekil parçalarını al
                var offsets = BlockShapeDefinitions.GetOffsets(objectBlockType);

                // Her bir parçayı grid üzerine işle
                foreach (var offset in offsets)
                {
                    // Parçayı döndür
                    Vector2Int rotatedOffset = BlockShapeDefinitions.RotateOffset(offset, rotationY);
                    
                    int finalX = pivotX + rotatedOffset.x;
                    int finalZ = pivotZ + rotatedOffset.y;

                    // Grid sınırları kontrolü
                    if (finalX >= 0 && finalX < _columns && finalZ >= 0 && finalZ < _rows)
                    {
                        _occupiedGrid[finalX, finalZ] = true;
                        
                        // Dictionary kontrolü (Yoksa oluştur, varsa güncelle)
                        Vector2 key = new Vector2(finalX, finalZ);
                        if (!_levelDataDict.ContainsKey(key))
                        {
                            _levelDataDict[key] = new LevelData { Position = key };
                        }
                        _levelDataDict[key].IsOccupied = true;
                        
                        // Ekrana yazılacak harf (Örn: L, S, B)
                        _occupiedGridNames[finalX, finalZ] = objectBlockType.ToString().Substring(0, 1);
                    }
                }
            }
        }

        private void DrawSettingsPanel()
        {
            GUILayout.Label("CONFIGURATION", EditorStyles.boldLabel);
            _currentLevelData = (CD_LevelData)EditorGUILayout.ObjectField("Level Data", _currentLevelData, typeof(CD_LevelData), false);
            _blockAssets = (CD_BlockData)EditorGUILayout.ObjectField("Block Assets", _blockAssets, typeof(CD_BlockData), false);
            _levelParent = (Transform)EditorGUILayout.ObjectField("Level Parent", _levelParent, typeof(Transform), true);
            _colorData = (CD_ColorData)EditorGUILayout.ObjectField("Color Data", _colorData, typeof(CD_ColorData), true);

            GUILayout.Space(10);
            GUILayout.Label("GRID SIZE", EditorStyles.boldLabel);
            _rows = EditorGUILayout.IntField("Rows (Z)", _rows);
            _columns = EditorGUILayout.IntField("Columns (X)", _columns);
            _cellSize = EditorGUILayout.FloatField("Cell Size", _cellSize);
            _yHeight = EditorGUILayout.FloatField("Y Height", _yHeight);

            GUILayout.Space(10);
            GUILayout.Label("PALETTE", EditorStyles.boldLabel);
            _selectedBlockType = (BlockType)EditorGUILayout.EnumPopup("Type", _selectedBlockType);
            _selectedBlockColorType = (BlockColorType)EditorGUILayout.EnumPopup("Color", _selectedBlockColorType);
            
            GUILayout.Space(5);
            GUILayout.Label($"Rotation: {_selectedRotation.eulerAngles.y}°");
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("0°")) _selectedRotation = Quaternion.Euler(0, 0, 0);
            if (GUILayout.Button("90°")) _selectedRotation = Quaternion.Euler(0, 90, 0);
            if (GUILayout.Button("180°")) _selectedRotation = Quaternion.Euler(0, 180, 0);
            if (GUILayout.Button("270°")) _selectedRotation = Quaternion.Euler(0, 270, 0);
            GUILayout.EndHorizontal();

            GUILayout.Space(20);
            GUI.backgroundColor = Color.green;
            if (GUILayout.Button("SAVE LEVEL", GUILayout.Height(40))) SaveLevel();
            GUI.backgroundColor = Color.white;
            
            if (GUILayout.Button("CLEAR SCENE", GUILayout.Height(30))) ClearScene();
            if(GUILayout.Button("CREATE GRID (Helper)", GUILayout.Height(30))) CreateGrid();
        }

        private void CreateGrid()
        {
            if (_levelParent == null)
            {
                Debug.LogWarning("Level Parent is not assigned!");
                return;
            }

            // Sahneyi temizle (Eski grid hücrelerini silmek için)
            // NOT: Sadece GridCell olanları silmek daha güvenli olabilir ama
            // ClearScene tüm çocukları siliyor. Dikkatli kullanın.
            ClearScene();

            for (int z = 0; z < _rows; z++)
            {
                for (int x = 0; x < _columns; x++)
                {
                    Vector2 key = new Vector2(x, z);
                    if(!_levelDataDict.ContainsKey(key))
                        _levelDataDict.Add(key, new LevelData());
                    
                    var data = _levelDataDict[key];
                    data.Position = new Vector2(x, z);
                    

                    // Görsel Grid Küpü oluşturma
                    Vector3 worldPos = new Vector3(x * _cellSize, _yHeight - 0.1f, z * _cellSize); // Biraz aşağıda dursun
                    GameObject gridCell = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    _levelSpawnedCubes.Add(key, gridCell);
                    gridCell.transform.position = worldPos;
                    gridCell.transform.localScale = new Vector3(_cellSize * 1f, 0.1f, _cellSize * 1f);
                    gridCell.transform.parent = _levelParent;
                    
                    // ÖNEMLİ: İsimlendirme, filtrelemek için kullanılıyor
                    gridCell.name = $"GridCell_{x}_{z}";
                    
                    // Materyal rengi (Opsiyonel: gri zemin)
                    var renderer = gridCell.GetComponent<Renderer>();
                    if(renderer) renderer.material.color = new Color(0.2f, 0.2f, 0.2f, 0.5f);
                }
            }
        }

        private void DrawGridButtons()
        {
            GUILayout.Label("LEVEL PREVIEW", EditorStyles.boldLabel);
            _scrollPosition = GUILayout.BeginScrollView(_scrollPosition);

            // Z eksenini tersten çiziyoruz (Yukarıdan aşağıya görünüm için)
            for (int z = _rows - 1; z >= 0; z--)
            {
                GUILayout.BeginHorizontal();
                for (int x = 0; x < _columns; x++)
                {
                    Color btnColor = Color.gray;
                    string btnText = $"{x},{z}";

                    // --- CANLI GÖRSELLEŞTİRME ---
                    if (_occupiedGrid != null && _occupiedGrid[x, z])
                    {
                        btnColor = Color.green;
                        btnText = _occupiedGridNames[x, z]; // Bloğun tipi yazar (R, B, vs.)
                    }

                    GUI.backgroundColor = btnColor;

                    // TIKLAMA İŞLEMİ
                    if (GUILayout.Button(btnText, _gridButtonStyle, GUILayout.Width(50), GUILayout.Height(50)))
                    {
                        // Ctrl veya Sağ Tık -> SİL
                        if (Event.current.control || Event.current.button == 1)
                        {
                            RemoveBlockAtCoordinate(x, z);
                        }
                        // Sol Tık -> EKLE
                        else
                        {
                            SpawnBlockAt(x, z);
                        }
                    }
                }
                GUILayout.EndHorizontal();
            }

            GUI.backgroundColor = Color.white;
            GUILayout.EndScrollView();
        }

        // --- SPAWN LOGIC ---
        private void SpawnBlockAt(int gridX, int gridZ)
        {
            if (_levelParent == null || _blockAssets == null)
            {
                Debug.LogWarning("Level Parent or Block Assets missing!");
                return;
            }

            // Önce oradaki bloğu sil (Çakışma olmasın)
            RemoveBlockAtCoordinate(gridX, gridZ);

            Vector3 worldPos = new Vector3(gridX * _cellSize, _yHeight, gridZ * _cellSize);
            
            // Prefab güvenliği
            if ((int)_selectedBlockType >= _blockAssets.Blocks.Count)
            {
                Debug.LogError("Selected Block Type index is out of range!");
                return;
            }

            GameObject prefab = _blockAssets.Blocks[(int)_selectedBlockType].Prefab;
            
            // Renk güvenliği
            Material mat = null;
            if (_colorData != null && (int)_selectedBlockColorType < _colorData.ColorData.Count)
            {
                mat = _colorData.ColorData[(int)_selectedBlockColorType].Material;
            }

            if (prefab != null)
            {
                GameObject obj = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
                var cubePos = _levelSpawnedCubes[new Vector2(gridX, gridZ)].transform.position;
                cubePos.y += 0.5f;
                obj.transform.position = cubePos;
                obj.transform.rotation = _selectedRotation;
                obj.transform.parent = _levelDataDict[new Vector2(gridX, gridZ)].Position == new Vector2(gridX, gridZ) ? _levelParent : null;
                
                // İSİMLENDİRME: CalculateOccupiedCells buradaki isme bakıp tipini anlayacak
                obj.name = $"{_selectedBlockType}_{gridX}_{gridZ}";

                // Materyal atama
                if (mat != null)
                {
                    Renderer[] renderers = obj.GetComponentsInChildren<Renderer>();
                    foreach (var r in renderers) r.sharedMaterial = mat;
                }

                Undo.RegisterCreatedObjectUndo(obj, "Spawn Block");
            }
        }

        // --- REMOVE LOGIC (Polyomino Destekli) ---
        private void RemoveBlockAtCoordinate(int targetX, int targetZ)
        {
            if (_levelParent == null) return;

            // Tüm blokları tara.
            // Dizi bozulmasın diye tersten gitmek veya kopyalamak gerekebilir ama
            // DestroyImmediate ile anlık silindiği için break yapıp çıkacağız, sorun olmaz.
            foreach (Transform child in _levelParent)
            {
                // DÜZELTME 1: Zemin Grid küplerini silme!
                if (child.name.StartsWith("GridCell")) continue;

                int pivotX = Mathf.RoundToInt(child.position.x / _cellSize);
                int pivotZ = Mathf.RoundToInt(child.position.z / _cellSize);
                float rotY = child.eulerAngles.y;

                // DÜZELTME 2: Silinecek objenin kendi şeklini hesapla
                BlockType objectBlockType = _selectedBlockType; // Fallback
                string[] nameParts = child.name.Split('_');
                if (nameParts.Length > 0 && Enum.TryParse(nameParts[0], out BlockType parsedType))
                {
                    objectBlockType = parsedType;
                }

                var offsets = BlockShapeDefinitions.GetOffsets(objectBlockType);

                foreach (var offset in offsets)
                {
                    Vector2Int rotatedOffset = BlockShapeDefinitions.RotateOffset(offset, rotY);
                    int finalX = pivotX + rotatedOffset.x;
                    int finalZ = pivotZ + rotatedOffset.y;

                    if (finalX == targetX && finalZ == targetZ)
                    {
                        // Hedef bulundu! Ana objeyi sil.
                        Undo.DestroyObjectImmediate(child.gameObject);
                        return; // Bir blok silindi, döngüden çık
                    }
                }
            }
        }

        private void SaveLevel()
        {
            if (_currentLevelData == null || _levelParent == null)
            {
                Debug.LogError("Cannot save: Level Data or Parent is missing.");
                return;
            }
            
            _currentLevelData.Levels.Clear();
            
            // Dictionary'deki veriyi ScriptableObject listesine aktar
            // CalculateOccupiedCells zaten dictionary'i güncelliyor
            foreach (var levelData in _levelDataDict.Values)
            {
                // Sadece dolu olanları veya GridCell olanları kaydetmek isteyebilirsin
                // Şimdilik hepsini aktarıyoruz
                _currentLevelData.Levels.Add(levelData);
            }
        
            EditorUtility.SetDirty(_currentLevelData);
            AssetDatabase.SaveAssets();
            Debug.Log($"Level Saved with {_currentLevelData.Levels.Count} cells!");
        }

        private void ClearScene()
        {
            if (_levelParent == null) return;
            _levelDataDict.Clear();
            _levelSpawnedCubes.Clear();
            
            // Child sayısı değiştikçe indeks kaymasın diye tersten siliyoruz
            for (int i = _levelParent.childCount - 1; i >= 0; i--)
            {
                DestroyImmediate(_levelParent.GetChild(i).gameObject);
            }
        }
    }
}