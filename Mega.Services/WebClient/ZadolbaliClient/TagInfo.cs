namespace Mega.WebClient.ZadolbaliClient
{
    public class TagInfo
    {
        public readonly string TagKey;

        public readonly string Name;

        public TagInfo(string tagKey, string name)
        {
            this.TagKey = tagKey;
            this.Name = name;
        }
    }
}
