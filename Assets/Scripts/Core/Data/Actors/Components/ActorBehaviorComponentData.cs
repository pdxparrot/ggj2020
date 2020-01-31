using System;

using UnityEngine;

namespace pdxpartyparrot.Core.Data.Actors.Components
{
    [Serializable]
    public abstract class ActorBehaviorComponentData : ScriptableObject
    {
        [SerializeField]
        private LayerMask _actorLayer;

        public LayerMask ActorLayer => _actorLayer;

#region Physics
        [Header("Actor Physics")]

        [SerializeField]
        [Tooltip("Mass in Kg. Overrides Rigidbody mass setting")]
        private float _mass = 45.0f;

        public float Mass => _mass;

        [SerializeField]
        [Tooltip("Drag coefficient. Overrides Rigidbody drag setting")]
        private float _drag = 0.0f;

        public float Drag => _drag;

        [SerializeField]
        [Tooltip("Angular drag coefficient. Overrides Ridigbody angular drag setting")]
        private float _angularDrag = 0.0f;

        public float AngularDrag => _angularDrag;

        [SerializeField]
        [Tooltip("Is this actor kinematic. Overrides Rigidbody kinematic setting")]
        private bool _isKinematic = false;

        public bool IsKinematic => _isKinematic;
#endregion

        [Space(10)]

#region Animation
        [Header("Actor Animations")]

        [SerializeField]
        [Tooltip("Whether or not to animate the model object directly if it's set")]
        private bool _animateModel;

        public bool AnimateModel => _animateModel;
#endregion
    }
}
