using JetBrains.Annotations;

using pdxpartyparrot.Core.Effects.EffectTriggerComponents;
using pdxpartyparrot.Game.UI;

using UnityEngine;

namespace pdxpartyparrot.Game.Effects.EffectTriggerComponents
{
    public class FloatingTextEffectTriggerComponent : EffectTriggerComponent
    {
        [SerializeField]
        private string _poolName = "floating_text";

        [SerializeField]
        private string _text;

        public string Text
        {
            get => _text;
            set => _text = value;
        }

        [SerializeField]
        private Color _color;

        public Color Color
        {
            get => _color;
            set => _color = value;
        }

        [SerializeField]
        private FloatingTextQueue _queue;

        [SerializeField]
        [CanBeNull]
        private Transform _spawnLocation;

        public override bool WaitForComplete => false;

        public override void OnStart()
        {
            _queue.QueueFloatingText(_poolName, _text, _color, () => null == _spawnLocation ? transform.position : _spawnLocation.position);
        }

        public override void OnStop()
        {
            // TODO: handle this
        }
    }
}
