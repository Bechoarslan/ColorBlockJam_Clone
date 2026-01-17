using System.Collections.Generic;
using RunTime.Enums; // Senin enum namespace'in
using UnityEngine;

public static class BlockShapeDefinitions
{
    // Bloğun merkezine (0,0) göre diğer parçaların koordinatları
    public static List<Vector2Int> GetOffsets(BlockType type)
    {
        List<Vector2Int> offsets = new List<Vector2Int>();
        
        // Merkez her zaman doludur
        offsets.Add(new Vector2Int(0, 0)); 

        switch (type)
        {
            

            case BlockType.LBlock:
                // T Şekli: Merkez + Sol + Sağ + Aşağı (Veya yukarı, tasarıma bağlı)
                offsets.Add(new Vector2Int(0, 1)); 
                offsets.Add(new Vector2Int(0, 2));
                offsets.Add(new Vector2Int(1, 0));// Aşağı
                
                break;
            case BlockType.UBlock:
                offsets.Add(new Vector2Int(1, 0));
                offsets.Add(new Vector2Int(0,0));
                offsets.Add(new Vector2Int(0,1));
                offsets.Add(new Vector2Int(2,0));
                offsets.Add( new Vector2Int(2,1));
                offsets.Add( new Vector2Int(2,2));
                offsets.Add( new Vector2Int(0,2));
                
                break;
            case BlockType.SingleBlock:
                break;
            case BlockType.TwoLinearBlock:
                offsets.Add(new Vector2Int(0, 1));
                break;

            case BlockType.ThreeLinearBlock:
                offsets.Add(new Vector2Int(0, 1));
                offsets.Add(new Vector2Int(0, 2));
                // L Şekli: Merkez + Yukarı + Yukarı + Sağ (Örnek)
                
                break;
            
            // Diğer şekiller buraya eklenecek...
        }
        return offsets;
    }

    // ROTASYON HESABI (Çok Önemli)
    // Bir noktayı (x,y) 90 derece döndürmek
    public static Vector2Int RotateOffset(Vector2Int offset, float angle)
    {
        // Unity rotasyonları bazen 90.00001 gibi olabilir, yuvarlıyoruz
        int rotIndex = Mathf.RoundToInt(angle / 90f) % 4;
        if (rotIndex < 0) rotIndex += 4; // Negatif açı düzeltmesi

        int x = offset.x;
        int y = offset.y;

        // Her 90 derecede (x, y) -> (y, -x) olur
        for (int i = 0; i < rotIndex; i++)
        {
            int temp = x;
            x = y;
            y = -temp;
        }

        return new Vector2Int(x, y);
    }
}