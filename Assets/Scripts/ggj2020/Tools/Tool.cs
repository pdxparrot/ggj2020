using System.Collections;
using System.Collections.Generic;
using pdxpartyparrot.ggj2020.Players;
using UnityEngine;

namespace pdxpartyparrot.ggj2020.Tools
{
    public class Tool : MonoBehaviour
    {
        private Mechanic HoldingPlayer = null;
        private GameObject parent;
        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

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
            }
        }

        virtual public void SetHeld(Mechanic player)
        {
            HoldingPlayer = player;
            Rigidbody rigid = gameObject.GetComponentInParent<Rigidbody>();
            rigid.isKinematic = true;
            parent = rigid.gameObject;
            parent.transform.SetParent(player.transform);
        }

        public void Drop()
        {
            HoldingPlayer = null;
            parent.GetComponent<Rigidbody>().isKinematic = false;
            parent.transform.SetParent(null);
            parent = null;
        }

        virtual public void UseTool()
        {
            //print("Parent use tool called");
        }

        virtual public void EndUseTool()
        {
            //print("Parent end use tool called");
        }
    }
}
