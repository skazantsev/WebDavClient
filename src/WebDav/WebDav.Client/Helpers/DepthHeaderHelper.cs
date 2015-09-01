using System.ComponentModel;

namespace WebDav.Helpers
{
    internal static class DepthHeaderHelper
    {
        public static string GetValueForPropfind(ApplyTo.Propfind applyTo)
        {
            switch (applyTo)
            {
                case ApplyTo.Propfind.CollectionOnly:
                    return "0";
                case ApplyTo.Propfind.CollectionAndChildren:
                    return "1";
                case ApplyTo.Propfind.CollectionAndAncestors:
                    return "infinity";
                default:
                    throw new InvalidEnumArgumentException("applyTo", (int)applyTo, typeof(ApplyTo.Propfind));
            }
        }

        public static string GetValueForCopy(ApplyTo.Copy applyTo)
        {
            switch (applyTo)
            {
                case ApplyTo.Copy.CollectionOnly:
                    return "0";
                case ApplyTo.Copy.CollectionAndAncestors:
                    return "infinity";
                default:
                    throw new InvalidEnumArgumentException("applyTo", (int)applyTo, typeof(ApplyTo.Copy));
            }
        }
    }
}
