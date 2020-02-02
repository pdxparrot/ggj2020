using System.Collections;
using System.Collections.Generic;
using pdxpartyparrot.ggj2020.Tools;
using UnityEngine;

namespace pdxpartyparrot.ggj2020.Players
{
    public class Mechanic : MonoBehaviour
    {
        private Tool held_tool = null;
        private Tool collided_tool = null;

        // Start is called before the first frame update
        void Start()
        {

        }

        bool HasTool()
        {
            return held_tool != null;
        }

        public void UseOrPickupTool()
        {
            if (HasTool())
            {
                held_tool.UseTool();
            } else if (held_tool == null)
            {
                held_tool = collided_tool;
                held_tool.SetHeld(this);
            }
        }

        public void DropTool()
        {
            held_tool = null;
        }

        public void SetCollidedTool(Tool new_tool)
        {
            collided_tool = new_tool;
        }

        Tool GetHeldTool()
        {
            return held_tool;
        }

        // Update is called once per frame
        void Update()
        {

        }

        void OnTriggerEnter(Collider collision)
        {
            collided_tool = collision.gameObject.GetComponent<Tool>();
        }
    }
}
