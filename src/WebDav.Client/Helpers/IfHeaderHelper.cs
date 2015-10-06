namespace WebDav
{
    internal static class IfHeaderHelper
    {
        public static string GetHeaderValue(string lockToken)
        {
            return string.Format("(<{0}>)", lockToken);
        }
    }
}
