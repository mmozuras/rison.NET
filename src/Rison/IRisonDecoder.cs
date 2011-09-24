namespace Rison
{
    public interface IRisonDecoder
    {
        T Decode<T>(string risonString);
    }
}