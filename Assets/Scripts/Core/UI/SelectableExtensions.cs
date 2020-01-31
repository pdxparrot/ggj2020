using UnityEngine.UI;

namespace pdxpartyparrot.Core.UI
{
    public static class SelectableExtensions
    {
        public static void Highlight(this Selectable selectable)
        {
            selectable.OnSelect(null);
        }
    }
}
