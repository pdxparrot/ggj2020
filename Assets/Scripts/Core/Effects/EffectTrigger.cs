using System;
using System.Collections;
using System.Collections.Generic;

using JetBrains.Annotations;

using pdxpartyparrot.Core.Collections;
using pdxpartyparrot.Core.Effects.EffectTriggerComponents;
using pdxpartyparrot.Core.Util;

using UnityEngine;

namespace pdxpartyparrot.Core.Effects
{
    public class EffectTrigger : MonoBehaviour
    {
        [Serializable]
        public class ReorderableList : ReorderableList<EffectTrigger>
        {
        }

        [SerializeField]
        [ReorderableList]
        private ReorderableList _triggerOnComplete = new ReorderableList();

        [SerializeField]
        [ReorderableList]
        private EffectTriggerComponent.ReorderableList _components = new EffectTriggerComponent.ReorderableList();

        [SerializeField]
        [ReadOnly]
        private bool _isRunning;

        public bool IsRunning => _isRunning;

        [SerializeField]
        [ReadOnly]
        private bool _complete;

        [SerializeReference]
        [ReadOnly]
        private /*readonly*/ Dictionary<string, object> _context = new Dictionary<string, object>();

        private Coroutine _effectWaiter;

#region Unity Lifecycle
        private void Awake()
        {
            RunOnComponents(c => c.Initialize(this));
        }

        private void OnDisable()
        {
            KillTrigger();
        }

        private void Update()
        {
            float dt = UnityEngine.Time.deltaTime;

            RunOnComponents(c => {
                if(!c.IsDone) {
                    c.OnUpdate(dt);
                }
            });
        }
#endregion

#region Components
        [CanBeNull]
        public T GetEffectTriggerComponent<T>() where T: EffectTriggerComponent
        {
            foreach(var component in _components.Items) {
                T tc = component as T;
                if(tc != null) {
                    return tc;
                }
            }
            return null;
        }

        public void GetEffectTriggerComponents<T>(ICollection<T> components) where T: EffectTriggerComponent
        {
            components.Clear();
            foreach(var component in _components.Items) {
                T tc = component as T;
                if(tc != null) {
                    components.Add(tc);
                }
            }
        }

        private void RunOnComponents(Action<EffectTriggerComponent> f)
        {
            foreach(var component in _components.Items) {
                f(component);
            }
        }
#endregion

#region Context
        public int GetInt(string name)
        {
            return Convert.ToInt32(_context.GetOrDefault(name));
        }

        public void SetInt(string name, int value)
        {
            _context[name] = value;
        }

        public float GetFloat(string name)
        {
            return Convert.ToSingle(_context.GetOrDefault(name));
        }

        public void SetFloat(string name, float value)
        {
            _context[name] = value;
        }

        public string GetString(string name)
        {
            return _context.GetOrDefault(name)?.ToString() ?? string.Empty;
        }

        public void SetString(string name, string value)
        {
            _context[name] = value;
        }
#endregion

        public void Trigger(Action callback=null)
        {
            KillTrigger();

            _isRunning = true;

            if(EffectsManager.Instance.EnableDebug) {
                Debug.Log($"Trigger {_components.Items.Count} more effects on {name}");
            }

            RunOnComponents(c => {
                if(c.IsDone) {
                    c.OnStart();
                }
            });

            _effectWaiter = StartCoroutine(EffectWaiter(callback));
        }

        private void TriggerWithContext(Dictionary<string, object> context, Action callback=null)
        {
            foreach(var kvp in context) {
                _context[kvp.Key] = kvp.Value;
            }
            Trigger(callback);
        }

        // forcefully stops the trigger early
        public void StopTrigger()
        {
            _complete = true;
        }

        // forcefully kills the trigger early
        public void KillTrigger()
        {
            if(null != _effectWaiter) {
                StopCoroutine(_effectWaiter);
                _effectWaiter = null;
            }

            RunOnComponents(c => {
                if(!c.IsDone) {
                    c.OnStop();
                }
            });

            foreach(var onCompleteEffect in _triggerOnComplete.Items) {
                onCompleteEffect.KillTrigger();
            }

            _isRunning = false;
            _complete = false;
        }

        public void ResetTrigger()
        {
            RunOnComponents(c => c.OnReset());
        }

        private IEnumerator EffectWaiter(Action callback)
        {
            WaitForSeconds wait = new WaitForSeconds(0.05f);

            // wait for components (if we should)
            while(true) {
                if(_complete) {
                    RunOnComponents(c => {
                        if(!c.IsDone) {
                            c.OnStop();
                        }
                    });

                    break;
                }

                bool done = true;
                foreach(EffectTriggerComponent component in _components.Items) {
                    if(!component.WaitForComplete || component.IsDone) {
                        continue;
                    }

                    done = false;
                    break;
                }

                if(done) {
                    break;
                }

                yield return wait;
            }

            // invoke our callback
            // (don't wait for further effects)
            _effectWaiter = null;
            callback?.Invoke();

            if(EffectsManager.Instance.EnableDebug) {
                Debug.Log($"Trigger {_triggerOnComplete.Items.Count} more effects from {name}");
            }

            // trigger further effects
            foreach(var onCompleteEffect in _triggerOnComplete.Items) {
                onCompleteEffect.TriggerWithContext(_context);

                if(_complete) {
                    onCompleteEffect.StopTrigger();
                }
            }

            // wait for those effects before we call ourself not running
            while(true) {
                bool done = true;
                foreach(EffectTrigger onCompleteEffect in _triggerOnComplete.Items) {
                    if(!onCompleteEffect.IsRunning) {
                        continue;
                    }

                    done = false;
                    break;
                }

                if(done) {
                    break;
                }

                yield return wait;
            }

            _isRunning = false;
        }
    }
}
