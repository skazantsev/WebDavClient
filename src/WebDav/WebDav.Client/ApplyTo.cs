namespace WebDav
{
    public static class ApplyTo
    {
        public enum Propfind
        {
            ResourceOnly,
            ResourceAndChildren,
            ResourceAndAncestors
        }

        public enum Copy
        {
            ResourceOnly,
            ResourceAndAncestors
        }

        public enum Lock
        {
            ResourceOnly,
            ResourceAndAncestors
        }
    }
}
