﻿using System;
using System.Collections.Generic;
using System.Linq;

using pdxpartyparrot.Core;
using pdxpartyparrot.Core.Actors;
using pdxpartyparrot.Core.Collections;
using pdxpartyparrot.Core.Effects;
using pdxpartyparrot.Core.Tween;
using pdxpartyparrot.Core.Util;
using pdxpartyparrot.Core.World;
using pdxpartyparrot.ggj2020.Players;

using UnityEngine;

namespace pdxpartyparrot.ggj2020.Actors
{
    public class RepairableRobot : Actor3D
    {
#region Events
        public event EventHandler<EventArgs> RepairedEvent;
#endregion

        public override bool IsLocalActor => true;

        [Space(10)]

        [SerializeField]
        private RepairPoint[] _repairPoints;

        // TODO: this should be split into a factor per-player
        [SerializeField]
        [ReadOnly]
        private float _chargeLevel;

        public float ChargeLevel => _chargeLevel;

        [Space(10)]

        [Header("Effects")]

        [SerializeField]
        private EffectTrigger _enterRepairBayEffectTrigger;

        [SerializeField]
        private TweenMove _enterMoveTween;

        [SerializeField]
        private EffectTrigger _exitRepairBayEffectTrigger;

        [SerializeField]
        private TweenMove _exitMoveTween;

        [SerializeField]
        [ReadOnly]
        private int _currentDamagedParts;

        [SerializeField]
        [ReadOnly]
        private int _currentDamageIncreaseChance;

#region Unity Lifecycle
        protected override void Awake()
        {
            base.Awake();

            foreach(RepairPoint repairPoint in _repairPoints) {
                repairPoint.ResetDamage();
                repairPoint.RepairedEvent += RepairedEventHandler;
            }
        }

        protected override void OnDestroy()
        {
            foreach(RepairPoint repairPoint in _repairPoints) {
                repairPoint.RepairedEvent -= RepairedEventHandler;
            }

            base.OnDestroy();
        }
#endregion

        public bool IsRepaired()
        {
            foreach(RepairPoint repairPoint in _repairPoints) {
                if(!repairPoint.IsRepaired) {
                    return false;
                }
            }
            return true;
        }

        public float GetRepairPercent()
        {
            int damageCount = 0;
            foreach(RepairPoint repairPoint in _repairPoints) {
                if(!repairPoint.IsRepaired) {
                    damageCount++;
                }
            }

            return (_currentDamagedParts - damageCount) / (float)_currentDamagedParts;
        }

        public void EnterRepairBay(Action onComplete)
        {
            Debug.Log($"Robot entering repair bay {transform.position}...");

            _enterMoveTween.From = transform.position;
            _enterMoveTween.To = Vector3.zero;

            _enterRepairBayEffectTrigger.Trigger(() => {
                onComplete?.Invoke();
            });
        }

        public void ExitRepairBay(Action onComplete)
        {
            Debug.Log("Robot exiting repair bay...");

            _exitMoveTween.From = Vector3.zero;
            _exitMoveTween.To = GameManager.Instance.GameLevelHelper.RepairableExit.position;

            _exitRepairBayEffectTrigger.Trigger(() => {
                onComplete?.Invoke();
            });
        }

        private void InitDamage()
        {
            // TODO: move this allocation out of here
            List<RepairPoint> repairPoints = new List<RepairPoint>(_repairPoints);
            for(int i=0; i<_currentDamagedParts; ++i) {
                RepairPoint repairPoint = repairPoints.RemoveRandomEntry();
                repairPoint.Damage();
            }
        }

        private void ResetDamage()
        {
            foreach(RepairPoint repairPoint in _repairPoints) {
                repairPoint.ResetDamage();
            }
        }

#region Events
        public override bool OnSpawn(SpawnPoint spawnpoint)
        {
            if(!base.OnSpawn(spawnpoint)) {
                return false;
            }

            _currentDamagedParts = GameManager.Instance.GameGameData.RepairableRobotData.InitialDamagedAreasPerPlayerCount.ElementAt(PlayerManager.Instance.Players.Count);
            _currentDamageIncreaseChance = GameManager.Instance.GameGameData.RepairableRobotData.DamageAreaIncreaseBasePercent;

            InitDamage();

            return true;
        }

        public override bool OnReSpawn(SpawnPoint spawnpoint)
        {
            if(!base.OnReSpawn(spawnpoint)) {
                return false;
            }

            // see if we get a damage increase
            int chance = PartyParrotManager.Instance.Random.Next(100);
            if(chance <= _currentDamageIncreaseChance) {
                Debug.Log($"Damage increased {chance} of {_currentDamageIncreaseChance}");
                _currentDamagedParts += GameManager.Instance.GameGameData.RepairableRobotData.DamageAreaIncreasePerPlayerCount.ElementAt(PlayerManager.Instance.Players.Count);
                _currentDamageIncreaseChance = GameManager.Instance.GameGameData.RepairableRobotData.DamageAreaIncreaseBasePercent;
            } else {
                Debug.Log($"No damage increase {chance} of {_currentDamageIncreaseChance}");
                _currentDamageIncreaseChance += GameManager.Instance.GameGameData.RepairableRobotData.DamageAreaIncreaseRate;
            }

            InitDamage();

            return true;
        }

        public override void OnDeSpawn()
        {
            ResetDamage();

            _enterRepairBayEffectTrigger.StopTrigger();
            _exitRepairBayEffectTrigger.StopTrigger();
        }

        private void RepairedEventHandler(object sender, EventArgs args)
        {
            if(IsRepaired()) {
                RepairedEvent?.Invoke(this, EventArgs.Empty);
            }
        }
#endregion
    }
}
