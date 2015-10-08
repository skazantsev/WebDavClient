using System.Threading;

namespace WebDav
{
    /// <summary>
    /// Represents parameters for the GET WebDAV method.
    /// </summary>
    public class GetFileParameters
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GetFileParameters"/> class.
        /// </summary>
        public GetFileParameters()
        {
            CancellationToken = CancellationToken.None;
        }

        /// <summary>
        /// Gets or sets the cancellation token.
        /// </summary>
        public CancellationToken CancellationToken { get; set; }
    }
}
