using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Xml.Linq;

namespace WebDav.Client.Request
{
    public class SearchParameters
    {
        public SearchParameters()
        {
            Namespaces = new List<NamespaceAttr>();
            Headers = new List<KeyValuePair<string, string>>();
            SelectProperties = new List<XElement>();
            WhereProperties = new List<XElement>();
            CancellationToken = CancellationToken.None;
        }

        /// <summary>
        /// Root directory where to search from.
        /// </summary>
        public string SearchPath { get; set; }
       
        /// <summary>
        /// Keyword on what to search. For example: 'test%' (without quotes), results in LIKE test%.
        /// </summary>
        public string SearchKeyword { get; set; }
       
        /// <summary>
        /// Set the cancellation token.
        /// </summary>
        public CancellationToken CancellationToken { get; set; }

        /// <summary>
        /// Gets or sets the collection of xml namespaces of properties.
        /// </summary>
        public IReadOnlyCollection<NamespaceAttr> Namespaces { get; set; }

        /// <summary>
        /// Gets or sets the collection of the headers.
        /// </summary>
        public IReadOnlyCollection<KeyValuePair<string, string>> Headers { get; set; }

        /// <summary>
        /// Gets or sets the Select properties.
        /// </summary>
        public IReadOnlyCollection<XElement> SelectProperties { get; set; }

        /// <summary>
        /// Gets or sets the Where properties.
        /// </summary>
        public IReadOnlyCollection<XElement> WhereProperties { get; set; }
    }
}
