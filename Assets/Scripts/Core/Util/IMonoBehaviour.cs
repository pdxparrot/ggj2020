using UnityEngine;

namespace pdxpartyparrot.Core.Util
{
    public interface IMonoBehaviour
    {
        GameObject gameObject { get; }

        Transform transform { get; }

        string tag { get; set; }

        string name { get; }

        bool isActiveAndEnabled { get; set; }

        bool enabled { get; set; }

        T GetComponent<T>() where T: Component;

        T GetComponentInChildren<T>() where T: Component;

        T[] GetComponents<T>() where T: Component;

        T[] GetComponentsInChildren<T>() where T: Component;

        T[] GetComponentsInChildren<T>(bool includeInactive) where T: Component;
    }
}
