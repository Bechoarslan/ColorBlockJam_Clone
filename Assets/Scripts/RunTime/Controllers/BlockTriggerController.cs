using System;
using System.Collections.Generic;
using RunTime.Data.UnityObject;
using RunTime.Enums;
using RunTime.Interfaces;
using UnityEngine;

namespace RunTime.Controllers
{
    public class BlockTriggerController : MonoBehaviour
    {
        [SerializeField] private int blockSize = 2;
        public BlockColorType blockColor;
        [SerializeField] private List<GameObject> blocks;
        [SerializeField] private CD_ColorData colorData;
      

        
        private void Awake()
        {
            SetColorMesh();
            
        }

        private void SetColorMesh()
        {
            var material = colorData.ColorData[(int)blockColor].Material;
            foreach (var block in blocks)
            {
                var meshRenderer = block.GetComponent<MeshRenderer>();
                if (meshRenderer != null)
                {
                    meshRenderer.sharedMaterial = material;
                }
            }
        }


        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Block"))
            {
                Debug.Log("Trigger Entered by: " + other.name);
                var iBlock = other.GetComponent<IBlock>();
                if (iBlock is null) return;
                if (iBlock.BlockSize > blockSize) return;

                bool isMatch = true;
                if (other.transform.forward != transform.forward)
                {
                    Debug.Log("Block orientation does not match the trigger orientation.");
                }
                else
                {
                    Debug.Log("Block orientation matches the trigger orientation.");
                }





            }
        }

    

        
    }
}