using System;
using System.Collections.Generic;

using UnityEngine;

namespace pdxpartyparrot.Core.Util
{
    [Serializable]
    public abstract class ReorderableList<T>
    {
        [SerializeField]
        private List<T> _items = new List<T>();

        public IReadOnlyCollection<T> Items => _items;
    }

    [Serializable]
    public class ReorderableListString : ReorderableList<string>
    {
    }

    [Serializable]
    public class ReorderableListInt : ReorderableList<int>
    {
    }

    [Serializable]
    public class ReorderableListFloat : ReorderableList<float>
    {
    }

    [Serializable]
    public class ReorderableListGameObject : ReorderableList<GameObject>
    {
    }
}
