using System.Threading;

namespace WebDav
{
    /// <summary>
    /// Represents parameters for the UNLOCK WebDAV method.
    /// </summary>
    public class UnlockParameters
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UnlockParameters"/> class.
        /// </summary>
        public UnlockParameters()
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
