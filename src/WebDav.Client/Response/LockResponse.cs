﻿
using System.Collections.Generic;
using System.Linq;

namespace WebDav
{
    /// <summary>
    /// Represents a response of the LOCK operation.
    /// </summary>
    public class LockResponse : WebDavResponse
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LockResponse"/> class.
        /// </summary>
        /// <param name="statusCode">The status code of the operation.</param>
        public LockResponse(int statusCode)
            : this(statusCode, null, new List<ActiveLock>())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LockResponse"/> class.
        /// </summary>
        /// <param name="statusCode">The status code of the response.</param>
        /// <param name="activeLocks">The active locks of the resource.</param>
        public LockResponse(int statusCode, IEnumerable<ActiveLock> activeLocks)
            : this(statusCode, null, activeLocks)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LockResponse"/> class.
        /// </summary>
        /// <param name="statusCode">The status code of the response.</param>
        /// <param name="description">The description of the response.</param>
        public LockResponse(int statusCode, string? description)
            : this(statusCode, description, new List<ActiveLock>())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LockResponse"/> class.
        /// </summary>
        /// <param name="statusCode">The status code of the response.</param>
        /// <param name="description">The description of the response.</param>
        /// <param name="activeLocks">The active locks of the resource.</param>
        public LockResponse(int statusCode, string? description, IEnumerable<ActiveLock> activeLocks)
            : base(statusCode, description)
        {
            Guard.NotNull(activeLocks, "activeLocks");
            ActiveLocks = activeLocks.ToList();
        }

        /// <summary>
        /// Gets a collection locks on this resource.
        /// </summary>
        public IReadOnlyCollection<ActiveLock> ActiveLocks { get; }

        public override string ToString()
        {
            return $"LOCK response - StatusCode: {StatusCode}, Description: {Description}";
        }
    }
}
