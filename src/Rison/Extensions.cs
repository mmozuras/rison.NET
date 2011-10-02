namespace Rison
{
    internal static class Extensions
    {
         internal static bool IsNumber<T>(this T obj)
         {
            return obj is int || obj is long || obj is double ||
                   obj is decimal || obj is float ||
                   obj is byte || obj is short ||
                   obj is sbyte || obj is ushort ||
                   obj is uint || obj is ulong;
         }
    }
}