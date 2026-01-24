using UnityEngine;

namespace RunTime.Interfaces.Ability
{
    public interface IAbility 
    {
        void OnEnterAbility();
        
        void OnExitAbility();
    }
}