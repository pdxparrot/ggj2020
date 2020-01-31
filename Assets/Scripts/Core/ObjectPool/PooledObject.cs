using System;

using pdxpartyparrot.Core.Util;

using UnityEngine;

namespace pdxpartyparrot.Core.ObjectPool
{
    public sealed class PooledObject : MonoBehaviour
    {
#region Events
        public event EventHandler<EventArgs> RecycleEvent;
#endregion

        [SerializeField]
        [ReadOnly]
        private string _tag;

        public string Tag
        {
            get => _tag;
            set => _tag = value;
        }

        public void Recycle()
        {
            ObjectPoolManager.Instance.Recycle(this);

            RecycleEvent?.Invoke(this, EventArgs.Empty);
        }
    }
}
