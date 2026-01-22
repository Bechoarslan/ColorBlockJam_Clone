using System;
using System.Collections;
using UnityEngine;

namespace RunTime.Controllers
{
    public class BlockTriggerController : MonoBehaviour
    {
        private Vector2Int _triggerPosition;
        private Coroutine _destroyCoroutine;
        private void Awake()
        {
            _triggerPosition = new Vector2Int(Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.z));
        }

        private void OnTriggerStay(Collider other)
        {
            if (other.CompareTag("Block"))
            {
                var blockGrid = new Vector2Int(Mathf.RoundToInt(other.transform.position.x), Mathf.RoundToInt(other.transform.position.z));
                if(Vector2.Distance(blockGrid, _triggerPosition) < 1.1f && _destroyCoroutine is null)
                {
                    _destroyCoroutine = StartCoroutine(BlockDestroyCoroutine(other.gameObject));
                    Debug.Log("Block is within the trigger area at position: " + _triggerPosition);
                }
            }
        }

        private IEnumerator BlockDestroyCoroutine(GameObject otherGameObject)
        {
            yield return new WaitForSeconds(2f); // Wait for 2 seconds before destroying
            Destroy(otherGameObject);
            Debug.Log("Block destroyed at position: " + _triggerPosition);
            _destroyCoroutine = null;
        }
    }
}