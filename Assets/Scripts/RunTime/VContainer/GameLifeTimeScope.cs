using System.Collections;
using System.Collections.Generic;
using RunTime.Controllers.GameLoopController;
using RunTime.Data.UnityObject;
using RunTime.Data.ValueObjects;
using RunTime.Managers;
using UnityEngine;
using VContainer;
using VContainer.Unity;

public class GameLifetimeScope : LifetimeScope
{
   [SerializeField] private Camera mainCamera;
   [SerializeField] private List<CD_BlockPoolData> blockPoolDataList;
   protected override void Configure(IContainerBuilder builder)
   {
      builder.Register<GridManager>(Lifetime.Singleton);
      builder.Register<BlockPoolManager>(Lifetime.Singleton).WithParameter(blockPoolDataList);

      builder.RegisterInstance(blockPoolDataList);
      builder.RegisterComponent(mainCamera);
      
      builder.RegisterEntryPoint<GameLoopController>();

     
    
   }
    
}
