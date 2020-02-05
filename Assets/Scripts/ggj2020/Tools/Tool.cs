using System.Collections.Generic;

using pdxpartyparrot.Game.Interactables;
using pdxpartyparrot.ggj2020.Players;
using pdxpartyparrot.ggj2020.Actors;

using UnityEngine;

namespace pdxpartyparrot.ggj2020.Tools
{
    public class Tool : MonoBehaviour, IInteractable
    {
        public SpriteRenderer render = null;
        protected Mechanic HoldingPlayer = null;
        protected GameObject parent;
        protected RepairPoint closestPoint = null;
        protected Actors.RepairPoint.DamageType DType;
        protected RepairPoint oldClosestPoint = null;

        public bool CanInteract => true;

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

        void OnTriggerEnter(Collider collision) { 
            Mechanic mechanic = collision.gameObject.GetComponent<Mechanic>();
            if(null != mechanic) {
                mechanic.SetCollidedTool(this);
            }
        }

        void OnTriggerExit(Collider collision)
        {
            Mechanic mechanic = collision.gameObject.GetComponent<Mechanic>();
            if (null != mechanic)
            {
                mechanic.SetCollidedTool(null);
                PlayerExitTrigger();
            }
        }

        virtual public void PlayerExitTrigger() 
        {

        }

        virtual public void SetHeld(Mechanic player)
        {
            HoldingPlayer = player;
            Rigidbody rigid = gameObject.GetComponentInParent<Rigidbody>();
            rigid.isKinematic = true;
            parent = rigid.gameObject;
            parent.transform.SetParent(player.transform);
            render.enabled = false;
            SetAttachment();
        }

        virtual public void Drop()
        {
            RemoveAttachment();
            HoldingPlayer.Owner.UIBubble.HideSprite();
            HoldingPlayer = null;
            parent.GetComponent<Rigidbody>().isKinematic = false;
            parent.transform.SetParent(null);
            render.enabled = true;
            parent = null;
        }

        virtual public void TrackThumbStickAxis(Vector2 Axis)
        {

        }

        virtual public void UseTool(Mechanic player)
        {
            //print("Parent use tool called");
        }

        virtual public void EndUseTool()
        {
            //print("Parent end use tool called");
        }

        virtual public void SetAttachment()
        {

        }

        virtual public void RemoveAttachment()
        {

        }
    }
}
