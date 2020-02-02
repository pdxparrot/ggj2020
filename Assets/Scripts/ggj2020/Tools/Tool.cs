using System.Collections;
using System.Collections.Generic;
using pdxpartyparrot.ggj2020.Players;
using UnityEngine;

namespace pdxpartyparrot.ggj2020.Tools
{
    public class Tool : MonoBehaviour
    {
        private Mechanic HoldingPlayer = null;
        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        void OnTriggerEnter(Collider collision)
        {            
            Mechanic mechanic = collision.gameObject.GetComponent<Mechanic>();
            if(null != mechanic) {
                mechanic.SetCollidedTool(this);
            }
        }

        public void SetHeld(Mechanic player)
        {
            HoldingPlayer = player;
            gameObject.transform.parent = player.gameObject.transform;
        }

        public void Drop()
        {
            HoldingPlayer = null;
            gameObject.transform.parent = null;
        }

        virtual public void UseTool()
        {
            print("Parent use tool called");
        }
    }
}
