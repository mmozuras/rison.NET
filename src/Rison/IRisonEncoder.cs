namespace Rison
{
    public interface IRisonEncoder
    {
        string Encode<T>(T obj);
    }
}