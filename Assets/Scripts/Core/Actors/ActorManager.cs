using System;
using System.Collections;
using System.Collections.Generic;

using pdxpartyparrot.Core.Collections;
using pdxpartyparrot.Core.DebugMenu;
using pdxpartyparrot.Core.Time;
using pdxpartyparrot.Core.Util;

using UnityEngine;

namespace pdxpartyparrot.Core.Actors
{
    public sealed class ActorManager : SingletonBehavior<ActorManager>
    {
        private readonly Dictionary<Type, HashSet<Actor>> _actors = new Dictionary<Type, HashSet<Actor>>();

        [SerializeField]
        private float _actorThinkRateMs = 10.0f;

        private float ActorThinkRateSeconds => _actorThinkRateMs * TimeManager.MilliSecondsToSeconds;

#region Debug
        [SerializeField]
        private bool _enableDebug;

        public bool EnableDebug => _enableDebug;
#endregion

#region Unity Lifecycle
        private void Awake()
        {
            InitDebugMenu();

            StartCoroutine(ThinkRoutine());
        }
#endregion

        public void Register<T>(T actor) where T: Actor
        {
            Type actorType = actor.GetType();
            if(_enableDebug) {
                Debug.Log($"Registering actor {actor.Id} of type {actorType}");
            }

            HashSet<Actor> actors = _actors.GetOrAdd(actorType);
            actors.Add(actor);
        }

        public void Unregister<T>(T actor) where T: Actor
        {
            Type actorType = actor.GetType();
            if(_enableDebug) {
                Debug.Log($"Unregistering actor {actor.Id} of type {actorType}");
            }

            if(_actors.TryGetValue(actorType, out var actors)) {
                actors.Remove(actor);
            }
        }

        public int ActorCount<TV>() where TV: Actor
        {
            return _actors.TryGetValue(typeof(TV), out var actors) ? actors.Count : 0;
        }

        public IReadOnlyCollection<Actor> GetActors<T>() where T: Actor
        {
            return _actors.GetOrDefault(typeof(T));
        }

        private IEnumerator ThinkRoutine()
        {
            float lastThinkTime = UnityEngine.Time.time;

            WaitForSeconds wait = new WaitForSeconds(ActorThinkRateSeconds);
            while(true) {
                float now = UnityEngine.Time.time;

                if(PartyParrotManager.Instance.IsPaused) {
                    lastThinkTime = now;
                    yield return wait;

                    continue;
                }

                foreach(var kvp in _actors) {
                    foreach(Actor actor in kvp.Value) {
                        actor.Think(now - lastThinkTime);
                    }
                }

                lastThinkTime = now;
                yield return wait;
            }
        }

        private void InitDebugMenu()
        {
            DebugMenuNode debugMenuNode = DebugMenuManager.Instance.AddNode(() => "Core.ActorManager");
            debugMenuNode.RenderContentsAction = () => {
                _enableDebug = GUILayout.Toggle(_enableDebug, "Enable Debug");

                foreach(var kvp in _actors) {
                    GUILayout.BeginVertical($"{kvp.Key}", GUI.skin.box);
                        foreach(Actor actor in kvp.Value) {
                            GUILayout.Label($"{actor.Id} {(null != actor.Movement ? actor.Movement.Position : actor.transform.position)}");
                        }
                    GUILayout.EndVertical();
                }
            };
        }
    }
}
