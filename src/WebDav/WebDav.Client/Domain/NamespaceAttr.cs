namespace WebDav
{
    public class NamespaceAttr
    {
        public NamespaceAttr(string defaultNamespace)
        {
            Namespace = defaultNamespace;
        }

        public NamespaceAttr(string prefix, string @namespace)
        {
            Prefix = prefix;
            Namespace = @namespace;
        }

        public string Prefix { get; private set; }

        public string Namespace { get; private set; }
    }
}
