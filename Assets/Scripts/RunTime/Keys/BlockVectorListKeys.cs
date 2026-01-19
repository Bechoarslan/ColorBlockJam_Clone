using System.Collections.Generic;
using RunTime.Enums;
using UnityEngine;


namespace RunTime.Keys
{
    public class BlockVectorListKeys
    {
        public List<Vector2Int> OnRegisterVectorList(BlockType blockType, float rotationY)
        {
            var vectorList = new List<Vector2Int>();
            switch (blockType)
            {
                case BlockType.SingleBlock:
                    vectorList.Add(new Vector2Int(0, 0));
                    break;
                case BlockType.TwoLinearBlock:
                    vectorList.Add(new Vector2Int(0, 0));
                    vectorList.Add(new Vector2Int(0,1));
                    break;
                case BlockType.ThreeLinearBlock:
                    vectorList.Add(new Vector2Int(0, 0));
                    vectorList.Add(new Vector2Int(0,1));
                    vectorList.Add(new Vector2Int(0,2));
                    break;
                case BlockType.LBlock:
                    vectorList.Add(new Vector2Int(0, 0));
                    vectorList.Add(new Vector2Int(0,1));
                    vectorList.Add(new Vector2Int(1,0));
                    break;
                case BlockType.UBlock:
                    vectorList.Add(new Vector2Int(0, 0));
                    vectorList.Add(new Vector2Int(0,1));
                    vectorList.Add(new Vector2Int(-1,1));
                    vectorList.Add(new Vector2Int(0,-1));
                    vectorList.Add(new Vector2Int(-1,-1));
                    break;
                
            }
            
            return CheckRotation(vectorList, rotationY);
        }

        public List<Vector2Int> CheckRotation(List<Vector2Int> vectorList,float rotation)
        {
            int angle = Mathf.RoundToInt(rotation / 90f) * 90;
            angle = (angle % 360 + 360) % 360;
            for(int i=0; i< vectorList.Count; i++)
            {
                var vec = vectorList[i];
                // Skip (0,0) rotation calculation as it remains (0,0)
                if(vec == Vector2Int.zero) continue;

                Vector2Int rotatedVec = vec;
                switch (angle)
                {
                    case 90: rotatedVec = new Vector2Int(vec.y, -vec.x); break;
                    case 180: rotatedVec = new Vector2Int(-vec.x, -vec.y); break;
                    case 270: rotatedVec = new Vector2Int(-vec.y, vec.x); break;
                }
                vectorList[i] = rotatedVec; 
            }
            return vectorList;
        }
    }
}