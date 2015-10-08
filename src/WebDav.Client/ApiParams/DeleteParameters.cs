using System.Threading;

namespace WebDav
{
    /// <summary>
    /// Represents parameters for the DELETE WebDAV method.
    /// </summary>
    public class DeleteParameters
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DeleteParameters"/> class.
        /// </summary>
        public DeleteParameters()
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
