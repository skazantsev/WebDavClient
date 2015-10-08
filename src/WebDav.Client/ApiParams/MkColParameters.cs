using System.Threading;

namespace WebDav
{
    /// <summary>
    /// Represents parameters for the MKCOL WebDAV method.
    /// </summary>
    public class MkColParameters
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MkColParameters"/> class.
        /// </summary>
        public MkColParameters()
        {
            CancellationToken = CancellationToken.None;
        }

        /// <summary>
        /// Gets or sets the resource lock token.
        /// </summary>
        public string LockToken { get; set; }

        /// <summary>
        /// Gets or sets the cancellation token.
        /// </summary>
        public CancellationToken CancellationToken { get; set; }
    }
}
