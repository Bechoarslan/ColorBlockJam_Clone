using System.Collections.Generic;
using DG.Tweening;
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
        [SerializeField] private ParticleSystem destroyParticleEffect;
        
        // GateController Variables
        private const float AnimDuration = 0.25f;
        private const float YMoveValue = -0.15f;
        private float _initPos;
        
        private bool _isMatched;
   
        private void Awake()
        {
            SetColorMesh();
        }

        private void Start()
        {
            _initPos = transform.localPosition.y;
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

        public void AbsorbItem(float duration)
        {
            transform.DOKill(true);

            float targetY = _initPos + YMoveValue;

            Sequence sequence = DOTween.Sequence();
            sequence.Append(transform.DOLocalMoveY(targetY, AnimDuration).SetEase(Ease.Linear));
            sequence.AppendInterval(duration);
            sequence.Append(transform.DOLocalMoveY(_initPos, AnimDuration).SetEase(Ease.Linear));
            sequence.Play();
        }

        private void OnTriggerEnter(Collider other)
        {
           
            if (other.CompareTag("Block"))
            {
                if (_isMatched) return;

                var parent = other.transform.parent;
                var otherBlock = parent.GetComponent<IBlock>();
                
                if (otherBlock == null) return;
                
             
                if (otherBlock.BlockColorType != blockColor) return;
                
            
                BoxCollider gateCollider = GetComponent<BoxCollider>();
                if (gateCollider != null)
                {
                  
                    Vector3 gateCenter = gateCollider.bounds.center;
                    Vector3 itemCenter = other.bounds.center;
                    
                    Vector3 direction = (gateCenter - itemCenter).normalized;
                    float maxDistance = Vector3.Distance(itemCenter, gateCenter) + 0.45f;
                    

                    if (Physics.Raycast(itemCenter, direction, out _, maxDistance))
                    {

                        _isMatched = true;
                        
                      
                        AbsorbItem(0.2f);
                        
           
                        if (destroyParticleEffect != null) 
                            destroyParticleEffect.Play();
                    }
                }
                else
                {
                   
                    _isMatched = true;
                    AbsorbItem(0.2f);
                }
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Block"))
            {
                _isMatched = false;

            }
        }
    }
}