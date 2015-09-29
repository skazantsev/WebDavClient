using System;
using System.Collections.Generic;

namespace WebDav
{
    public class WebDavResource
    {
        private WebDavResource()
        {
            Properties = new List<WebDavProperty>();
            PropertyStatuses = new List<WebDavPropertyStatus>();
        }

        public IReadOnlyCollection<ActiveLock> ActiveLocks { get; private set; }

        public string ContentLanguage { get; private set; }

        public int? ContentLength { get; private set; }

        public string ContentType { get; private set; }

        public DateTime? CreationDate { get; private set; }

        public string DisplayName { get; private set; }

        public string ETag { get; private set; }

        public string Href { get; private set; }

        public bool IsCollection { get; private set; }

        public bool IsHidden { get; private set; }

        public DateTime? LastModifiedDate { get; private set; }

        public IReadOnlyCollection<WebDavProperty> Properties { get; private set; }

        public IReadOnlyCollection<WebDavPropertyStatus> PropertyStatuses { get; private set; }

        public class Builder
        {
            private IReadOnlyCollection<ActiveLock> _activeLocks;
            private string _contentLanguage;
            private int? _contentLength;
            private string _contentType;
            private DateTime? _creationDate;
            private string _displayName;
            private string _eTag;
            private string _href;
            private bool _isCollection;
            private bool _isHidden;
            private DateTime? _lastModifiedDate;
            private IReadOnlyCollection<WebDavProperty> _properties;
            private IReadOnlyCollection<WebDavPropertyStatus> _propertyStatuses;

            public Builder WithActiveLocks(IReadOnlyCollection<ActiveLock> activeLocks)
            {
                _activeLocks = activeLocks;
                return this;
            }

            public Builder WithContentLanguage(string contentLanguage)
            {
                _contentLanguage = contentLanguage;
                return this;
            }

            public Builder WithContentLength(int? contentLength)
            {
                _contentLength = contentLength;
                return this;
            }

            public Builder WithContentType(string contentType)
            {
                _contentType = contentType;
                return this;
            }

            public Builder WithCreationDate(DateTime? creationDate)
            {
                _creationDate = creationDate;
                return this;
            }

            public Builder WithDisplayName(string displayName)
            {
                _displayName = displayName;
                return this;
            }

            public Builder WithETag(string eTag)
            {
                _eTag = eTag;
                return this;
            }

            public Builder WithHref(string href)
            {
                _href = href;
                return this;
            }

            public Builder IsCollection()
            {
                _isCollection = true;
                return this;
            }

            public Builder IsNotCollection()
            {
                _isCollection = false;
                return this;
            }

            public Builder IsHidden()
            {
                _isHidden = true;
                return this;
            }

            public Builder IsNotHidden()
            {
                _isHidden = false;
                return this;
            }

            public Builder WithLastModifiedDate(DateTime? lastModifiedDate)
            {
                _lastModifiedDate = lastModifiedDate;
                return this;
            }

            public Builder WithProperties(IReadOnlyCollection<WebDavProperty> properties)
            {
                Guard.NotNull(properties, "properties");
                _properties = properties;
                return this;
            }

            public Builder WithPropertyStatuses(IReadOnlyCollection<WebDavPropertyStatus> propertyStatuses)
            {
                Guard.NotNull(propertyStatuses, "propertyStatuses");
                _propertyStatuses = propertyStatuses;
                return this;
            }

            public WebDavResource Build()
            {
                return new WebDavResource
                {
                    ActiveLocks = _activeLocks,
                    ContentLanguage = _contentLanguage,
                    ContentLength = _contentLength,
                    ContentType = _contentType,
                    CreationDate = _creationDate,
                    DisplayName = _displayName,
                    ETag = _eTag,
                    Href = _href,
                    IsCollection = _isCollection,
                    IsHidden = _isHidden,
                    LastModifiedDate = _lastModifiedDate,
                    Properties = _properties,
                    PropertyStatuses = _propertyStatuses
                };
            }
        }
    }
}
