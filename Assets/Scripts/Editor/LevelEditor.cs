using System;
using System.Collections.Generic;
using RunTime.Data.UnityObject;
using RunTime.Enums;
using RunTime.Keys;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    public class LevelEditor  : EditorWindow
    {
        #region Self Variables

        #region Data Variables

        private CD_LevelGridData _levelData;
        private CD_BlockData _blockData;
        private CD_ColorData _colorData;

        #endregion

        #region Property Variables

        private BlockType _selectedBlockType;
        private BlockColorType _selectedBlockColorType;
        private float _currentRotationY;
        private int _row;
        private int _column;
        private int _level;
        private Transform _levelParent;
        
        
        private Dictionary<Vector2Int,bool> _activeCells = new Dictionary<Vector2Int,bool>();
        private Dictionary<List<Vector2Int>, GameObject> _blocks = new Dictionary<List<Vector2Int>, GameObject>();
        #endregion

        #endregion

        [MenuItem("Tools/Level Editor")]
        public static void Open()
        {
            GetWindow<LevelEditor>("Level Editor");
        }

        private  void OnGUI()
        {
            GUILayout.BeginHorizontal();
            
            GUILayout.BeginVertical(GUILayout.Width(300));
            
            GUILayout.Space(20);
           
            DrawObjectField();
            
            GUILayout.EndVertical();
            
            
            GUILayout.Box("", GUILayout.Width(1), GUILayout.ExpandHeight(true));
            
            
            GUILayout.BeginVertical();
            
            GUILayout.Space(20);
            GUILayout.Label("Grid Settings", EditorStyles.boldLabel, GUILayout.Width(100));
            CreateVisualGrid();
            GUILayout.EndVertical();
            
            
            
            GUILayout.EndHorizontal();
            
         
     
        }

        private void CreateVisualGrid()
        {
            
            for (int x = _row - 1; x >= 0; x--)
            {
                
                GUILayout.BeginHorizontal();

                for (int z = _column - 1; z >= 0; z--)
                {
                    Vector2Int cell = new Vector2Int(x, z);
                    _activeCells.TryAdd(cell, false);
                
                    bool isActive = _activeCells[cell];

                    Color oldColor = GUI.backgroundColor;
                    GUI.backgroundColor = isActive ? Color.green : Color.gray;

                    if (GUILayout.Button($"({x},{z})", GUILayout.Width(50), GUILayout.Height(50)))
                    {
                        
                        if (Event.current.button == 0)
                        {
                            // 1️⃣ Seçili block'un offset'lerini al
                            var blockVectorListKeys = new BlockVectorListKeys();
                            var blockVectors =
                                blockVectorListKeys.OnRegisterVectorList(
                                    _selectedBlockType,
                                    _currentRotationY
                                );

                            // 2️⃣ Önce TÜM hücreleri kontrol et (validation)
                            bool canPlace = true;
                            List<Vector2Int> targetCells = new List<Vector2Int>();
                            targetCells.Add(cell);

                            foreach (var vector in blockVectors)
                            {
                                
                                Vector2Int targetCell =
                                    new Vector2Int(cell.x + vector.x, cell.y + vector.y);
                                
                                
                                // Grid dışında mı?
                                if (!_activeCells.ContainsKey(targetCell))
                                {
                                    canPlace = false;
                                    break;
                                }

                                // Hücre dolu mu?
                                if (_activeCells[targetCell])
                                {
                                    canPlace = false;
                                    break;
                                }

                                targetCells.Add(targetCell);
                            }

                            // 3️⃣ Eğer yerleştirilemezse → HATA VER, ÇIKIŞ
                            if (!canPlace)
                            {
                                
                                return;
                            }
                            
                            // 4️⃣ TÜM hücreler uygunsa → TEK SEFERDE YERLEŞTİR
                            foreach (var targetCell in targetCells)
                            {
                                
                                _activeCells[targetCell] = true; // yeşile boya
                            }

                            // 5️⃣ Block'u tek sefer ekle (çok önemli)
                            var obj = (GameObject)PrefabUtility.InstantiatePrefab(_blockData.Blocks[(int)_selectedBlockType].Prefab);
                            obj.transform.position = new Vector3(x, 1, z);
                            obj.transform.rotation = Quaternion.Euler(0, _currentRotationY, 0);
                     
                            _blocks.Add(targetCells, obj);
                        }

                        else if (Event.current.button == 1)
                        {
                            foreach (var list in _blocks.Keys)
                            {
                                if (list.Contains(cell))
                                {
                                    // Bulunan block'u sil
                                    foreach (var targetCell in list)
                                    {
                                        _activeCells[targetCell] = false; // griye boya
                                    }
                                    DestroyImmediate(_blocks[list]);
                                    _blocks.Remove(list);
                                    
                                    break;
                                }
                            }
                        }

                    }

                    GUI.backgroundColor = oldColor;
                }
                
                GUILayout.EndHorizontal();
            }
        }


        private void DrawObjectField()
        {
            DrawDescriptionText("Data Settings");
            DrawBox(5);
            EditorGUIUtility.labelWidth = 80f;
            _levelData = (CD_LevelGridData)EditorGUILayout.ObjectField("Level Data",_levelData, typeof(CD_LevelGridData), true, GUILayout.Width(200));
            _blockData = (CD_BlockData)EditorGUILayout.ObjectField("Block Data",_blockData, typeof(CD_BlockData), true, GUILayout.Width(200));
            _colorData = (CD_ColorData)EditorGUILayout.ObjectField("Color Data",_colorData, typeof(CD_ColorData), true, GUILayout.Width(200));
            DrawBox(5);
            DrawDescriptionText("Block Settings");
            GUILayout.Box("", GUILayout.ExpandWidth(true), GUILayout.Height(5));
            _selectedBlockType = (BlockType)EditorGUILayout.EnumPopup("Block Type", _selectedBlockType, GUILayout.Width(200));
            _selectedBlockColorType = (BlockColorType)EditorGUILayout.EnumPopup("Block Color, Type", _selectedBlockColorType, GUILayout.Width(200));
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
            _row = EditorGUILayout.IntField("Row", _row, GUILayout.Width(200));
            _column = EditorGUILayout.IntField("Column", _column, GUILayout.Width(200));
            DrawBox(5);
            DrawDescriptionText("Level Settings");
            _level = EditorGUILayout.IntField("Level", _level, GUILayout.Width(200));
            _levelParent = (Transform)EditorGUILayout.ObjectField("Level Parent", _levelParent, typeof(Transform), true, GUILayout.Width(200));
            DrawBox(5);
            DrawDescriptionText("Create Level Grid");
            DrawBox(2);
            
            GUILayout.BeginHorizontal();

            if (GUILayout.Button("Create", GUILayout.ExpandWidth(true), GUILayout.Height(30)))
            {
                CreateLevelGrid();}
            if(GUILayout.Button("Clear", GUILayout.ExpandWidth(true), GUILayout.Height(30))){}
            
            GUILayout.EndHorizontal();
            DrawBox(5);
            DrawDescriptionText("Save & Load Level Data");
            GUILayout.BeginHorizontal();
            if(GUILayout.Button("Save", GUILayout.ExpandWidth(true), GUILayout.Height(30))){}
            
            if(GUILayout.Button("Load ", GUILayout.ExpandWidth(true), GUILayout.Height(30))){}
           
            GUILayout.EndHorizontal();
          
            DrawRotatedPreview( _blockData.Blocks[(int)_selectedBlockType].Prefab, _currentRotationY);
        }

        private void DrawRotationButton(float angle, string label)
        {
            // 1. Mevcut rengi hafızaya al (Böylece diğer butonlar etkilenmez)
            Color defaultColor = GUI.backgroundColor;

            // 2. Eğer _currentRotationY bu butondaki açıya eşitse rengi YEŞİL yap
            // Float karşılaştırmalarında == yerine Mathf.Approximately kullanmak daha güvenlidir
            if (Mathf.Approximately(_currentRotationY, angle))
            {
                GUI.backgroundColor = Color.green; // Seçili renk
            }
            else
            {
                GUI.backgroundColor = defaultColor; // Seçili değilse standart renk
            }

            // 3. Butonu çiz
            if (GUILayout.Button(label, GUILayout.ExpandWidth(true), GUILayout.Width(80)))
            {
                _currentRotationY = angle;
            }

            // 4. Rengi eski haline döndür (Sıradaki butonlar için)
            GUI.backgroundColor = defaultColor;
        }

        private void CreateLevelGrid()
        {
           for(int x = _row - 1; x >= 0; x--) 
           {
               for(int z = _column -1; z >= 0 ; z--)
               {
                   Vector3 pos = new Vector3(Mathf.RoundToInt(x),0,Mathf.RoundToInt(z));
                   GameObject cellObj = GameObject.CreatePrimitive(PrimitiveType.Cube);
                   cellObj.transform.position = pos;
                   if(_levelParent != null)
                   {
                       cellObj.transform.parent = _levelParent;
                   }
               }
           }
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
        
        private PreviewRenderUtility _previewUtility;

// Hafıza sızıntısı olmasın diye pencere kapanınca temizle
        private void OnDisable()
        {
            if (_previewUtility != null)
            {
                _previewUtility.Cleanup();
                _previewUtility = null;
            }
        }
        
        private void DrawRotatedPreview(GameObject prefab, float rotationY)
{
    if (prefab == null) return;

    // 1. Preview Utility yoksa oluştur
    if (_previewUtility == null)
    {
        _previewUtility = new PreviewRenderUtility();
        
        // Arka plan rengi
        _previewUtility.camera.backgroundColor = new Color(0.2f, 0.2f, 0.2f, 1f);
        _previewUtility.camera.clearFlags = CameraClearFlags.SolidColor;

        // --- BU KISMI DEĞİŞTİRİYORUZ ---

        // 1. Modu Orthographic yap (Perspektifi kapatır, teknik çizim gibi düz gösterir)
        _previewUtility.camera.orthographic = true;
        
        // 2. Zoom ayarı (Bu sayıyı artırırsan uzaklaşır, azaltırsan yakınlaşır)
        _previewUtility.camera.orthographicSize = 2.0f; 

        // 3. Pozisyon: Tam merkezde (0,0) ama YUKARIDA (Y=10)
        _previewUtility.camera.transform.position = new Vector3(0, 10, 0);

        // 4. Rotasyon: X ekseninde 90 derece eğ (Tam aşağı bak)
        // Y eksenini 0 yapıyoruz ki "Yukarı" yönü Dünya'nın "İleri" (Z) yönü olsun.
        _previewUtility.camera.transform.rotation = Quaternion.Euler(90,90, 0);

        // 5. Kesme düzlemleri (Kameranın ne kadar yakını/uzağı göreceği)
        _previewUtility.camera.nearClipPlane = 0.1f;
        _previewUtility.camera.farClipPlane = 20f;
    }
    // ...

    // 2. Çizim alanını al (Inspector'da 200x200 kare yer ayır)
    Rect rect = EditorGUILayout.GetControlRect(false, 200);

    // 3. Render İşlemini Başlat
    _previewUtility.BeginPreview(rect, GUIStyle.none);

    // 4. Bloğun içindeki tüm parçaları (Mesh) bul ve çiz
    // Senin kodun "Line Block" olduğu için child objeleri taramamız lazım.
    MeshFilter[] filters = prefab.GetComponentsInChildren<MeshFilter>();
    
    // Dönme işlemini burada yapıyoruz!
    Quaternion rot = Quaternion.Euler(0, rotationY, 0);

    foreach (var filter in filters)
    {
        Mesh mesh = filter.sharedMesh;
        
        // Materyali al (Yoksa standart pembe materyal olmasın diye default ata)
        Material mat = filter.GetComponent<Renderer>()?.sharedMaterial;
        if (mat == null) mat = new Material(Shader.Find("Standard"));

        // Objenin yerel pozisyonunu, bizim döndürdüğümüz ana rotasyonla çarp
        // Bu matematik: Child'ın yerini, ana objenin dönüşüne göre ayarlar.
        Vector3 finalPos = rot * filter.transform.localPosition;
        
        // Çiz (Mesh, Pozisyon, Rotasyon, Materyal, Layer)
        _previewUtility.DrawMesh(mesh, Matrix4x4.TRS(finalPos, rot, Vector3.one), mat, 0);
    }

    // 5. Kamerayı render et ve sonucu al
    _previewUtility.camera.Render();
    Texture resultTexture = _previewUtility.EndPreview();

    // 6. Sonucu ekrana bas
    GUI.DrawTexture(rect, resultTexture, ScaleMode.ScaleToFit);
}
    }
}