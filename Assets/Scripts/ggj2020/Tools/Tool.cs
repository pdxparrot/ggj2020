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
            // -- parenting 2 rigidbodys is bad news, so remove it
            Rigidbody rigid = gameObject.GetComponentInParent<Rigidbody>();
            parent = rigid.gameObject;
            Destroy(rigid);
            parent.transform.SetParent(player.transform);
        }

        public void Drop()
        {
            HoldingPlayer = null;
            parent.transform.SetParent(null);
            // -- add a rigid body so we have nice physics
            parent.AddComponent<Rigidbody>();
            parent = null;
        }

        virtual public void UseTool()
        {
            print("Parent use tool called");
        }
    }
}
