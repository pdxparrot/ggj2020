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
            collision.gameObject.GetComponent<Mechanic>().SetCollidedTool(this);
        }

        public void SetHeld(Mechanic player)
        {
            HoldingPlayer = player;
            
        }

        public void Drop()
        {
            HoldingPlayer = null;
        }

        virtual public void UseTool()
        {
            print("Parent use tool called");
        }
    }
}
