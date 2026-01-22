using System;
using System.Collections.Generic;
using RunTime.Enums;
using RunTime.Interfaces;
using UnityEngine;

namespace RunTime.Controllers
{
    public class BlockTriggerController : MonoBehaviour
    {
        [SerializeField] private int blockSize = 2;
        
      

        private void Awake()
        {
            CreateGrid();
            
        }

        private void CreateGrid()
        {
          
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