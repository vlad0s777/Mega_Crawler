namespace Mega.Services.UriRequest
{
    public class UriRequest
    {
        public readonly int Attempt;

        public readonly int Depth;

        public readonly string Id;

        public UriRequest(string id, int attempt = 0, int depth = 1)
        {
            this.Id = id;
            this.Attempt = attempt;
            this.Depth = depth;
        }
    }
}