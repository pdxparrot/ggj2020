namespace pdxpartyparrot.Core.Loading
{
    public class LoadStatus
    {
        public float LoadPercent { get; }

        public string Status { get; }

        public LoadStatus(float loadPercent, string status)
        {
            LoadPercent = loadPercent;
            Status = status;
        }
    }
}
