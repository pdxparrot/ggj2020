﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Cinemachine;

using pdxpartyparrot.Core;
using pdxpartyparrot.Core.Actors;
using pdxpartyparrot.Core.Collections;
using pdxpartyparrot.Core.Effects;
using pdxpartyparrot.Core.Tween;
using pdxpartyparrot.Core.Util;
using pdxpartyparrot.Core.World;
using pdxpartyparrot.ggj2020.Players;

using UnityEngine;
using UnityEngine.Serialization;

namespace pdxpartyparrot.ggj2020.Actors
{
    [RequireComponent(typeof(AudioSource))]
    [RequireComponent(typeof(CinemachineImpulseSource))]
    public sealed class RepairableRobot : Actor3D
    {
#region Events
        public event EventHandler<EventArgs> RepairedEvent;
#endregion

        public override bool IsLocalActor => true;

        [Space(10)]

        [SerializeField]
        private GameObject _repairPointContainer;

        [Space(10)]

#region Effects
        [Header("Effects")]

        [SerializeField]
        [FormerlySerializedAs("_enterRepairBayMoveEffectTrigger")]
        private EffectTrigger _enterRepairBayEffectTrigger;

        [SerializeField]
        private TweenMove _enterMoveTween;

        [SerializeField]
        private EffectTrigger _enterRepairBayAnimEffectTrigger;

        [SerializeField]
        private EffectTrigger _repairBayDockedEffect;

        [SerializeField]
        [FormerlySerializedAs("_exitRepairBayMoveEffectTrigger")]
        private EffectTrigger _exitRepairBayEffectTrigger;

        [SerializeField]
        private TweenMove _exitMoveTween;

        [SerializeField]
        private EffectTrigger _exitRepairBaySuccessEffectTrigger;

        [SerializeField]
        private EffectTrigger _exitRepairBayFailureEffectTrigger;

        [SerializeField]
        private EffectTrigger _chargingEffectTrigger;

        [SerializeField]
        private EffectTrigger _idleChargedEffectTrigger;

        [SerializeField]
        private EffectTrigger _idleUnchargedEffectTrigger;

        private EffectTrigger IdleEffect => GameManager.Instance.GameLevelHelper.ChargingStation.IsCharged ? _idleChargedEffectTrigger : _idleUnchargedEffectTrigger;
#endregion

        [Space(10)]

        [SerializeField]
        [ReadOnly]
        private int _currentDamagedParts;

        private bool CanIncreaseDamagedParts => _currentDamagedParts < _repairPoints.Count;

        [SerializeField]
        [ReadOnly]
        private int _currentDamageIncreaseChance;

        private readonly List<RepairPoint> _repairPoints = new List<RepairPoint>();
        //private readonly List<RepairPoint> _stackedRepairPoints = new List<RepairPoint>();

        private CinemachineImpulseSource _impulseSource;

        private Coroutine _impulseRoutine;

#region Unity Lifecycle
        protected override void Awake()
        {
            base.Awake();

            // TODO: this should come from an actor data object for this
            Rigidbody.isKinematic = true;
            Rigidbody.constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionZ;

            GetComponent<AudioSource>().spatialBlend = 0.0f;

            _impulseSource = GetComponent<CinemachineImpulseSource>();

            var repairPoints = _repairPointContainer.GetComponentsInChildren<RepairPoint>();
            foreach(RepairPoint repairPoint in repairPoints) {
                repairPoint.Initialize();
                repairPoint.RepairedEvent += RepairedEventHandler;

                // TODO: stacked damage goes in its own bucket
                _repairPoints.Add(repairPoint);
            }

            GameManager.Instance.StartUseChargingStationEvent += StartUseChargingStationEventHandler;
            GameManager.Instance.EndUseChargingStationEvent += EndUseChargingStationEventHandler;
        }

        protected override void OnDestroy()
        {
            if(GameManager.HasInstance) {
                GameManager.Instance.EndUseChargingStationEvent -= EndUseChargingStationEventHandler;
                GameManager.Instance.StartUseChargingStationEvent -= StartUseChargingStationEventHandler;
            }

            /*foreach(RepairPoint repairPoint in _stackedRepairPoints) {
                repairPoint.RepairedEvent -= RepairedEventHandler;
            }*/

            foreach(RepairPoint repairPoint in _repairPoints) {
                repairPoint.RepairedEvent -= RepairedEventHandler;
            }

            base.OnDestroy();
        }

        private void OnEnable()
        {
            _impulseRoutine = StartCoroutine(ImpulseRoutine());
        }

        private void OnDisable()
        {
            StopCoroutine(_impulseRoutine);
            _impulseRoutine = null;
        }
#endregion

        public bool IsRepaired()
        {
            foreach(RepairPoint repairPoint in _repairPoints) {
                if(!repairPoint.IsRepaired) {
                    return false;
                }
            }

            /*foreach(RepairPoint repairPoint in _stackedRepairPoints) {
                if(!repairPoint.IsRepaired) {
                    return false;
                }
            }*/

            return true;
        }

        public int GetDamagedCount()
        {
            int damagedCount = 0;

            foreach(RepairPoint repairPoint in _repairPoints) {
                if(!repairPoint.IsRepaired) {
                    damagedCount++;
                }
            }

            /*foreach(RepairPoint repairPoint in _stackedRepairPoints) {
                if(!repairPoint.IsRepaired) {
                    damagedCount++;
                }
            }*/

            return damagedCount;
        }

        public float GetRepairPercent()
        {
            return (_currentDamagedParts - GetDamagedCount()) / (float)_currentDamagedParts;
        }

        public void EnterRepairBay(Action onComplete)
        {
            Debug.Log($"Robot entering repair bay {transform.position}...");

            _enterMoveTween.From = transform.position;
            _enterMoveTween.To = Vector3.zero;

            _enterRepairBayEffectTrigger.Trigger(() => {
                _enterRepairBayEffectTrigger.KillTrigger();

                _repairBayDockedEffect.Trigger(() => {
                    IdleEffect.Trigger();
                    onComplete?.Invoke();
                });
            });

            _enterRepairBayAnimEffectTrigger.Trigger();
        }

        public void ExitRepairBay(bool success, Action onComplete)
        {
            Debug.Log("Robot exiting repair bay...");

            _exitMoveTween.From = Vector3.zero;
            _exitMoveTween.To = GameManager.Instance.GameLevelHelper.RepairableExit.position;

            _exitRepairBayEffectTrigger.Trigger(() => {
                _exitRepairBayEffectTrigger.KillTrigger();
                onComplete?.Invoke();
            });

            if(success) {
                _exitRepairBaySuccessEffectTrigger.Trigger();
            } else {
                _exitRepairBayFailureEffectTrigger.Trigger();
            }
        }

        private void InitDamage()
        {
            Debug.Log($"Damaging {_currentDamagedParts} parts");

            // TODO: move this allocation out of here
            List<RepairPoint> repairPoints = new List<RepairPoint>(_repairPoints);

            for(int i=0; i<_currentDamagedParts; ++i) {
                RepairPoint repairPoint = repairPoints.RemoveRandomEntry();
                repairPoint.Damage();
            }

            // TODO: handle stacked damage
        }

        private void ResetDamage()
        {
            Debug.Log("Resetting damage");

            foreach(RepairPoint repairPoint in _repairPoints) {
                repairPoint.ResetDamage();
            }

            /*foreach(RepairPoint repairPoint in _stackedRepairPoints) {
                repairPoint.ResetDamage();
            }*/
        }

        private IEnumerator ImpulseRoutine()
        {
            WaitForSeconds wait = new WaitForSeconds(GameManager.Instance.GameGameData.RepairableRobotData.ImpulseRate);
            while(true) {
                yield return wait;

                if(PartyParrotManager.Instance.IsPaused) {
                    continue;
                }

                if(!_enterRepairBayEffectTrigger.IsRunning && !_exitRepairBayEffectTrigger.IsRunning) {
                    continue;
                }

                if(EffectsManager.Instance.EnableViewerShake) {
                    _impulseSource.GenerateImpulse();
                }

                GameManager.Instance.RobotImpulse();
            }
        }

#region Events
        public override bool OnSpawn(SpawnPoint spawnpoint)
        {
            if(!base.OnSpawn(spawnpoint)) {
                return false;
            }

            _currentDamagedParts = GameManager.Instance.GameGameData.RepairableRobotData.InitialDamagedAreasPerPlayerCount.ElementAt(PlayerManager.Instance.PlayerCount - 1);
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
            if(CanIncreaseDamagedParts) {
                int chance = PartyParrotManager.Instance.Random.Next(100);
                if(chance <= _currentDamageIncreaseChance) {
                    Debug.Log($"Damage increased {chance} of {_currentDamageIncreaseChance}");
                    _currentDamagedParts += GameManager.Instance.GameGameData.RepairableRobotData.DamageAreaIncreasePerPlayerCount.ElementAt(PlayerManager.Instance.PlayerCount - 1);
                    if(_currentDamagedParts > _repairPoints.Count) {
                        _currentDamagedParts = _repairPoints.Count;
                    }

                    _currentDamageIncreaseChance = GameManager.Instance.GameGameData.RepairableRobotData.DamageAreaIncreaseBasePercent;
                } else {
                    Debug.Log($"No damage increase {chance} of {_currentDamageIncreaseChance}");
                    _currentDamageIncreaseChance += GameManager.Instance.GameGameData.RepairableRobotData.DamageAreaIncreaseRate;
                }
            }

            InitDamage();

            return true;
        }

        public override void OnDeSpawn()
        {
            ResetDamage();

            _exitRepairBayEffectTrigger.StopTrigger();
        }

        private void StartUseChargingStationEventHandler(object sender, EventArgs args)
        {
            _chargingEffectTrigger.Trigger();
        }

        private void EndUseChargingStationEventHandler(object sender, EventArgs args)
        {
            _chargingEffectTrigger.StopTrigger();

            IdleEffect.Trigger();
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
