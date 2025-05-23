﻿using System;
using System.Collections.Generic;
using System.Threading;
using System.Xml.Linq;

namespace WebDav
{
    public class SearchParameters
    {
        public SearchParameters()
        {
            Scope = "/";
            Namespaces = new List<NamespaceAttr>();
            SelectProperties = new List<XName>();
            Headers = new List<KeyValuePair<string, string>>();
            CancellationToken = CancellationToken.None;
        }

        /// <summary>
        /// Gets or sets a directory to search in.
        /// </summary>
        public string Scope { get; set; }

        /// <summary>
        /// Gets or sets the search property in the DAV:like element.
        /// </summary>
        public XName? SearchProperty { get; set; }

        /// <summary>
        /// Gets or sets the search keyword to be used in the DAV:like element.
        /// Example: 'test%' (without quotes), results in 'LIKE test%' search.
        /// </summary>
        public string? SearchKeyword { get; set; }

        /// <summary>
        /// Gets or sets the collection of xml namespaces of properties.
        /// </summary>
        public IReadOnlyCollection<NamespaceAttr> Namespaces { get; set; }

        /// <summary>
        /// Gets or sets the Select properties.
        /// </summary>
        public IReadOnlyCollection<XName> SelectProperties { get; set; }

        /// <summary>
        /// Gets or sets the collection of the headers.
        /// </summary>
        public IReadOnlyCollection<KeyValuePair<string, string>> Headers { get; set; }

        /// <summary>
        /// Gets or sets the cancellation token.
        /// </summary>
        public CancellationToken CancellationToken { get; set; }

        public void AssertParametersAreValid()
        {
            if (string.IsNullOrEmpty(Scope))
                throw new ArgumentException("Scope is required.", nameof(Scope));

            if (SearchProperty == null)
                throw new ArgumentException("Search property is required.", nameof(SearchProperty));

            if (string.IsNullOrEmpty(SearchKeyword))
                throw new ArgumentException("Search keyword is required.", nameof(SearchKeyword));
        }
    }
}
