using System.Collections;
using System.Collections.Generic;
using pdxpartyparrot.ggj2020.Players;
using pdxpartyparrot.ggj2020.Actors;
using UnityEngine;

namespace pdxpartyparrot.ggj2020.Tools
{
    public class Tool : MonoBehaviour
    {
        protected Mechanic HoldingPlayer = null;
        protected GameObject parent;
        protected RepairPoint closestPoint = null;
        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            
        }

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
        public RepairPoint FindClosestRepairPoint(List<RepairPoint> points)
        {
            RepairPoint closestPoint = null;
            float closestDistance = 9999999999;
            foreach (RepairPoint item in points)
            {
                Vector3 ToVector = gameObject.transform.position - item.gameObject.transform.position;
                float Distance = ToVector.magnitude;
                if (Distance < closestDistance)
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
        }

        virtual public void Drop()
        {
            HoldingPlayer = null;
            parent.GetComponent<Rigidbody>().isKinematic = false;
            parent.transform.SetParent(null);
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
    }
}
