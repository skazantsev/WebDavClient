namespace WebDav
{
    public static class ApplyTo
    {
        public enum Propfind
        {
            CollectionOnly,
            CollectionAndChildren,
            CollectionAndAncestors
        }

        public enum Copy
        {
            CollectionOnly,
            CollectionAndAncestors
        }
    }
}
