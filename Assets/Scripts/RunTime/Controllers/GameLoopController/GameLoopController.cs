using System.Collections.Generic;
using RunTime.Data.UnityObject;
using RunTime.Data.ValueObjects;
using RunTime.Managers;
using VContainer.Unity;

namespace RunTime.Controllers.GameLoopController
{
    public class GameLoopController : IStartable
    {
        private readonly GridManager _gridManager;
        private readonly List<CD_BlockPoolData> _blockPoolDataList;


        public GameLoopController(GridManager gridManager, List<CD_BlockPoolData> blockPoolDataList)
        {
            _blockPoolDataList = blockPoolDataList;
            _gridManager = gridManager;
        }
        public void Start()
        {
            _gridManager.GenerateBlock(_blockPoolDataList);
        }
    }
}