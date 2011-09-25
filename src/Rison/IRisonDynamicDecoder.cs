namespace Rison
{
    public interface IRisonDynamicDecoder
    {
        dynamic Decode(string risonString);
    }
}