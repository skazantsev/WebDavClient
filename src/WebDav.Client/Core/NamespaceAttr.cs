﻿namespace WebDav
{
    /// <summary>
    /// Represents an xml namespace attribute.
    /// </summary>
    public class NamespaceAttr
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NamespaceAttr"/> class.
        /// </summary>
        /// <param name="defaultNamespace">The default namespace.</param>
        public NamespaceAttr(string defaultNamespace)
        {
            Namespace = defaultNamespace;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NamespaceAttr"/> class.
        /// </summary>
        /// <param name="prefix">The prefix of the namespace</param>
        /// <param name="namespace">The namespace in form of URI specified at an xmlns attribute</param>
        public NamespaceAttr(string prefix, string @namespace)
        {
            Prefix = prefix;
            Namespace = @namespace;
        }

        /// <summary>
        /// Gets the prefix of this namespace.
        /// </summary>
        public string? Prefix { get; }

        /// <summary>
        /// Gets the namespace in form of URI specified at an xmlns attribute as follows. xmlns:prefix="URI".
        /// </summary>
        public string Namespace { get; }
    }
}
