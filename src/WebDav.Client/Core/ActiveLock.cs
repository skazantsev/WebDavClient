﻿using System;

namespace WebDav
{
    /// <summary>
    /// Represents an active lock taken on a WebDAV resource.
    /// </summary>
    public class ActiveLock
    {
        private ActiveLock()
        {
        }

        /// <summary>
        /// Gets a value indicating whether the lock is to be applied only to the resource or the resource and all its members.
        /// It corresponds to the WebDAV Depth header.
        /// </summary>
        public ApplyTo.Lock? ApplyTo { get; private set; }

        /// <summary>
        /// Gets a value indicating whether this lock is an exclusive lock or a shared lock.
        /// </summary>
        public LockScope? LockScope { get; private set; }

        /// <summary>
        /// Gets the resource lock token.
        /// </summary>
        public string? LockToken { get; private set; }

        /// <summary>
        /// Gets the owner of this lock.
        /// </summary>
        public LockOwner? Owner { get; private set; }

        /// <summary>
        /// Gets the root URL of the lock, which is the URL through which the resource was addressed in the LOCK request.
        /// </summary>
        public string? LockRoot { get; private set; }

        /// <summary>
        /// Gets the duration of this lock.
        /// </summary>
        public TimeSpan? Timeout { get; private set; }

        /// <summary>
        /// Represents a builder of the <see cref="ActiveLock"/> class.
        /// </summary>
        public class Builder
        {
            private ApplyTo.Lock? _applyTo;
            private LockScope? _lockScope;
            private string? _lockToken;
            private LockOwner? _owner;
            private string? _lockRoot;
            private TimeSpan? _timeout;

            /// <summary>
            /// Sets the ApplyTo parameter of an instance of the <see cref="ActiveLock"/> class.
            /// </summary>
            public Builder WithApplyTo(ApplyTo.Lock? applyTo)
            {
                _applyTo = applyTo;
                return this;
            }

            /// <summary>
            /// Sets the LockTo parameter of an instance of the <see cref="ActiveLock"/> class.
            /// </summary>
            public Builder WithLockScope(LockScope? lockScope)
            {
                _lockScope = lockScope;
                return this;
            }

            /// <summary>
            /// Sets the LockToken parameter of an instance of the <see cref="ActiveLock"/> class.
            /// </summary>
            public Builder WithLockToken(string? lockToken)
            {
                _lockToken = lockToken;
                return this;
            }

            /// <summary>
            /// Sets the Owner parameter of an instance of the <see cref="ActiveLock"/> class.
            /// </summary>
            public Builder WithOwner(LockOwner? owner)
            {
                _owner = owner;
                return this;
            }

            /// <summary>
            /// Sets the LockRoot parameter of an instance of the <see cref="ActiveLock"/> class.
            /// </summary>
            public Builder WithLockRoot(string? lockRoot)
            {
                _lockRoot = lockRoot;
                return this;
            }

            /// <summary>
            /// Sets the Timeout parameter of an instance of the <see cref="ActiveLock"/> class.
            /// </summary>
            public Builder WithTimeout(TimeSpan? timeout)
            {
                _timeout = timeout;
                return this;
            }

            /// <summary>
            /// Builds a new instance of the <see cref="ActiveLock"/> class.
            /// </summary>
            /// <returns>A new instance of the <see cref="ActiveLock"/> class.</returns>
            public ActiveLock Build()
            {
                return new ActiveLock
                {
                    ApplyTo = _applyTo,
                    LockScope = _lockScope,
                    LockToken = _lockToken,
                    Owner = _owner,
                    LockRoot = _lockRoot,
                    Timeout = _timeout
                };
            }
        }
    }
}
