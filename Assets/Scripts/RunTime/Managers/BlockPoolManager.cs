using System.Collections.Generic;
using RunTime.Controllers;
using RunTime.Data.UnityObject;
using RunTime.Data.ValueObjects;
using RunTime.Enums;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace RunTime.Managers
{
    public class BlockPoolManager
    {
        private readonly IObjectResolver _container;
        private readonly Transform _mainRoot;

        private readonly Dictionary<BlockTypeEnums, Transform> _typeRoots = new Dictionary<BlockTypeEnums, Transform>();
        private readonly Dictionary<BlockTypeEnums,BlockView> _prefabs = new Dictionary<BlockTypeEnums, BlockView>();
        private readonly Dictionary<BlockTypeEnums,Queue<BlockView>> _pool = new Dictionary<BlockTypeEnums, Queue<BlockView>>();
        
        public BlockPoolManager(IObjectResolver container,List<CD_BlockPoolData> blockPoolDataList)
        {
            var rootgo = new GameObject("BlockPoolRoot");
            _mainRoot = rootgo.transform;
             _container = container;
             foreach (var soData in blockPoolDataList)
             {
                 // Then iterate through the specific pools inside that SO
                 foreach (var config in soData.BlockPools)
                 {
                     Debug.Log("BlockPoolManager: Initializing pool for block type " + config.BlockType);
                     // Guard: Prevent duplicate keys if two SOs have the same BlockType
                     if (!_prefabs.ContainsKey(config.BlockType))
                     { 
                         _prefabs.Add(config.BlockType, config.BlockPrefab);
                         _pool.Add(config.BlockType, new Queue<BlockView>());
                        
                         // Create Sub-Folder (e.g. "Pool_Red")
                         var typeSubRoot = new GameObject($"Pool_{config.BlockType}");
                         typeSubRoot.transform.SetParent(_mainRoot); // Cleaner API than .parent = 
                         _typeRoots.Add(config.BlockType, typeSubRoot.transform);
                        
                         PreWarm(config.BlockType, config.InitialPoolSize);
                     }
                 }
             }
        }

        public BlockView Spawn(BlockTypeEnums blockType, Vector3 position, Quaternion rotation)
        {
            if (!_pool.ContainsKey(blockType))
            {
                Debug.LogError("BlockPoolManager: Spawn: No pool found for block type " + blockType);
                return null;
            }

            BlockView block;

            if (_pool[blockType].Count > 0)
            {
                block = _pool[blockType].Dequeue();
            }
            else
            {
                block = CreateNewInstances(blockType);
            }
            
            block.transform.position = position;
            block.transform.rotation = rotation;
            block.gameObject.SetActive(true);
            return block;
        }

        public void DeSpawn(BlockView block)
        {
            block.gameObject.SetActive(false);
            if (_typeRoots.ContainsKey(block.BlockType))
            {
                block.transform.SetParent(_typeRoots[block.BlockType]);
            }
            else
            {
                _mainRoot.transform.SetParent(_mainRoot);
            }
            if (_pool.ContainsKey(block.BlockType))
            {
                _pool[block.BlockType].Enqueue(block);
            }
        }
        private void PreWarm(BlockTypeEnums configBlockType, int configInitialPoolSize)
        {
            for (int i = 0; i < configInitialPoolSize; i++)
            {
                var block = CreateNewInstances(configBlockType);
                block.gameObject.SetActive(false);
                _pool[configBlockType].Enqueue(block);
            }
        }

        private BlockView CreateNewInstances(BlockTypeEnums configBlockType)
        {
            var prefab = _prefabs[configBlockType];
            var root = _typeRoots[configBlockType];
            var instance = _container.Instantiate(prefab,root.transform);
            instance.SetType(configBlockType);
            return instance;

        }
    }
}