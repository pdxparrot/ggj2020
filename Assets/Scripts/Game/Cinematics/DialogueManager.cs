using System.Collections.Generic;

using pdxpartyparrot.Core.Util;
using pdxpartyparrot.Game.Data;

using UnityEngine;

namespace pdxpartyparrot.Game.Cinematics
{
    public sealed class DialogueManager : SingletonBehavior<DialogueManager>
    {
        [SerializeField]
        private DialogueData _data;

        private readonly Dictionary<string, Dialogue> _dialoguePrefabs = new Dictionary<string, Dialogue>();

#region Unity Lifecycle
        private void Awake()
        {
            /*foreach(Dialogue dialoguePrefab in _data.DialoguePrefabs) {
                if(_dialoguePrefabs.ContainsKey(dialoguePrefab.Id)) {
                    Debug.LogWarning($"Overwriting dialogue {dialoguePrefab.Id}");
                }
                _dialoguePrefabs[dialoguePrefab.Id] = dialoguePrefab;
            }*/
        }
#endregion
    }
}
