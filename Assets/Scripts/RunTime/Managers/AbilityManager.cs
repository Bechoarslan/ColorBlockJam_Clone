using System;
using System.Collections.Generic;
using RunTime.Enums;
using RunTime.Interfaces.Ability;
using RunTime.Managers;
using RunTime.Signals;
using Sirenix.OdinInspector;
using Unity.VisualScripting;
using UnityEngine;

namespace RunTime.Controllers
{
    public class AbilityManager : MonoBehaviour
    {
        #region Self Variables

        #region Public Variables

        public Transform SelectedObject;
        public IAbility CurrentAbility;

        #endregion
        #region Serialized Variables

       

        #endregion

        #region Private Variables

        private AbilityType _abilityType = AbilityType.None;
        private FireCrackerAbility _fireCrackerAbility;
        
        #endregion

        #endregion

        private void Awake()
        {
            SetAbilities();
        }

        private void SetAbilities()
        {
            _fireCrackerAbility = new FireCrackerAbility(this);
        }

        private void OnEnable()
        {
            SubscribeEvents();
        }

        private void SubscribeEvents()
        {
            AbilitySignals.Instance.onAbilitySelected += OnSwitchAbility;
            AbilitySignals.Instance.onAbilitySelectedObject += (selectedObj) => { SelectedObject = selectedObj; };
            AbilitySignals.Instance.onStartAbility += StartAbility;
        }

        private void UnSubscribeEvents()
        {
            AbilitySignals.Instance.onAbilitySelected -= OnSwitchAbility;
            AbilitySignals.Instance.onAbilitySelectedObject -= (selectedObj) => { SelectedObject = selectedObj; };
            AbilitySignals.Instance.onStartAbility -= StartAbility;
        
        }

        private void StartAbility()
        {
            Debug.Log("Ability Started");
            if (CurrentAbility is null) return;
            CurrentAbility.OnEnterAbility();
        }

        private void OnDisable()
        {
            UnSubscribeEvents();
        }
       
        private void OnSwitchAbility(AbilityType abilityType)
        {
            
            _abilityType = abilityType;
            switch (_abilityType)
            {
                case AbilityType.None:
                    CurrentAbility = null;
                    break;
                case AbilityType.FireCracker:
                    CurrentAbility = _fireCrackerAbility;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            
        }
    }
}