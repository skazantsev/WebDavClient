using System;

namespace WebDav
{
    public class ActiveLock
    {
        private ActiveLock()
        {
        }

        public ApplyTo.Lock ApplyTo { get; private set; }

        public LockScope? LockScope { get; private set; }

        public string LockToken { get; private set; }

        public LockOwner Owner { get; private set; }

        public string ResourceUri { get; private set; }

        public TimeSpan? Timeout { get; private set; }

        public class Builder
        {
            private ApplyTo.Lock _applyTo;
            private LockScope? _lockScope;
            private string _lockToken;
            private LockOwner _owner;
            private string _resourceUri;
            private TimeSpan? _timeout;

            public Builder WithApplyTo(ApplyTo.Lock applyTo)
            {
                _applyTo = applyTo;
                return this;
            }

            public Builder WithLockScope(LockScope? lockScope)
            {
                _lockScope = lockScope;
                return this;
            }

            public Builder WithLockToken(string lockToken)
            {
                _lockToken = lockToken;
                return this;
            }

            public Builder WithOwner(LockOwner owner)
            {
                _owner = owner;
                return this;
            }

            public Builder WithResourceUri(string resourceUri)
            {
                _resourceUri = resourceUri;
                return this;
            }

            public Builder WithTimeout(TimeSpan? timeout)
            {
                _timeout = timeout;
                return this;
            }

            public ActiveLock Build()
            {
                return new ActiveLock
                {
                    ApplyTo = _applyTo,
                    LockScope = _lockScope,
                    LockToken = _lockToken,
                    Owner = _owner,
                    ResourceUri = _resourceUri,
                    Timeout = _timeout
                };
            }
        }
    }
}
