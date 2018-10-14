namespace Mega.Services.ZadolbaliClient
{
    public interface IZadolbaliClientFactory
    {
        ZadolbaliClient Create(string proxy, int seed);
    }
}
