using System.Collections.Generic;

using pdxpartyparrot.Core.Effects;
using pdxpartyparrot.Game.Interactables;
using pdxpartyparrot.ggj2020.Players;
using pdxpartyparrot.ggj2020.Actors;

using UnityEngine;

namespace pdxpartyparrot.ggj2020.Tools
{
    // TODO: make this an actor
    [RequireComponent(typeof(Rigidbody))]
    public abstract class Tool : MonoBehaviour, IInteractable
    {
        [SerializeField]
        private GameObject _model;

        [SerializeField]
        private EffectTrigger _useEffect;

        public bool CanInteract => true;

        protected Mechanic HoldingPlayer = null;
        protected RepairPoint closestPoint = null;
        protected Actors.RepairPoint.DamageType DType;
        protected RepairPoint oldClosestPoint = null;

        private Rigidbody _rigidbody;

#region Unity Lifecycle
        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();
        }
#endregion

        // -- TODO move this out of tool script
        public List<RepairPoint> FindRepairPoints()
        {
            List<RepairPoint> points = new List<RepairPoint>();

            GameObject[] allObjects = (GameObject[])FindObjectsOfType(typeof(GameObject));
            foreach (GameObject GO in allObjects)
            {
                RepairPoint point = GO.GetComponent<RepairPoint>();
                if (point != null)
                {
                    points.Add(point);
                }
            }
            return points;
        }

        // -- TODO move this out of tool script
        public RepairPoint FindClosestRepairPoint(List<RepairPoint> points, RepairPoint.DamageType type)
        {
            RepairPoint closestPoint = null;
            float closestDistance = 4.5f;
            float tempdist = 0;
            foreach (RepairPoint item in points)
            {
                Vector3 ToVector = gameObject.transform.position - item.gameObject.transform.position;
                float Distance = ToVector.magnitude;
                if (item.RepairPointDamageType == type)
                {
                    tempdist = Distance;
                }
                if (Distance < closestDistance && !item.IsRepaired && item.RepairPointDamageType == type)
                {
                    closestPoint = item;
                    closestDistance = Distance;
                }
            }

            return closestPoint;
        }

        public virtual void PlayerExitTrigger()
        {
        }

        public virtual void SetHeld(Mechanic player)
        {
            HoldingPlayer = player;

            _rigidbody.isKinematic = true;

            _model.SetActive(false);
            transform.SetParent(player.transform);

            SetAttachment();
        }

        public virtual void Drop()
        {
            RemoveAttachment();

            transform.SetParent(null);
            _model.SetActive(true);

            _rigidbody.isKinematic = false;

            HoldingPlayer.Owner.UIBubble.HideSprite();
            HoldingPlayer = null;
        }

        public virtual void TrackThumbStickAxis(Vector2 axis)
        {
        }

        public virtual void UseTool(Mechanic player)
        {
            _useEffect.Trigger();
        }

        public virtual void EndUseTool()
        {
            _useEffect.StopTrigger();
        }

        public virtual void SetAttachment()
        {
        }

        public virtual void RemoveAttachment()
        {
        }
    }
}
