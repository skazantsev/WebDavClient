﻿using System;
using System.Collections.Generic;

namespace WebDav
{
    /// <summary>
    /// Represents a WebDAV resource.
    /// </summary>
    public class WebDavResource
    {
        /// <summary>
        /// Prevents a default instance of the <see cref="WebDavResource"/> class from being created.
        /// </summary>
        private WebDavResource()
        {
            Properties = new List<WebDavProperty>();
            PropertyStatuses = new List<WebDavPropertyStatus>();
        }

        /// <summary>
        /// Gets a collection locks on this resource.
        /// </summary>
        public IReadOnlyCollection<ActiveLock>? ActiveLocks { get; private set; }

        /// <summary>
        /// Gets the content language of this resource.
        /// </summary>
        public string? ContentLanguage { get; private set; }

        /// <summary>
        /// Gets the content length of this resource.
        /// </summary>
        public long? ContentLength { get; private set; }

        /// <summary>
        /// Gets the content type of this resource.
        /// </summary>
        public string? ContentType { get; private set; }

        /// <summary>
        /// Gets the creation date of this resource.
        /// </summary>
        public DateTime? CreationDate { get; private set; }

        /// <summary>
        /// Gets the display name of this resource.
        /// </summary>
        public string? DisplayName { get; private set; }

        /// <summary>
        /// Gets the ETag of this resource.
        /// </summary>
        public string? ETag { get; private set; }

        /// <summary>
        /// Gets the URI of this resource.
        /// </summary>
        public string? Uri { get; private set; }

        /// <summary>
        /// Gets a value indicating whether this resource is a collection.
        /// </summary>
        /// <value>
        /// <c>true</c> if this resource is a collection; otherwise, <c>false</c>. The default value is <c>false</c>.
        /// </value>
        public bool IsCollection { get; private set; }

        /// <summary>
        /// Gets a value indicating whether this resource is hidden.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this resource is hidden; otherwise, <c>false</c>. The default value is <c>false</c>.
        /// </value>
        public bool IsHidden { get; private set; }

        /// <summary>
        /// Gets the last modified date of this resource.
        /// </summary>
        public DateTime? LastModifiedDate { get; private set; }

        /// <summary>
        /// Gets the collection of properties of this resource.
        /// </summary>
        public IReadOnlyCollection<WebDavProperty> Properties { get; private set; }

        /// <summary>
        /// Gets the collection of property statuses of this resource.
        /// </summary>
        public IReadOnlyCollection<WebDavPropertyStatus> PropertyStatuses { get; private set; }

        /// <summary>
        /// Represents a builder of the <see cref="WebDavResource"/> class.
        /// </summary>
        public class Builder
        {
            private IReadOnlyCollection<ActiveLock>? _activeLocks;
            private string? _contentLanguage;
            private long? _contentLength;
            private string? _contentType;
            private DateTime? _creationDate;
            private string? _displayName;
            private string? _eTag;
            private string? _uri;
            private bool _isCollection;
            private bool _isHidden;
            private DateTime? _lastModifiedDate;
            private IReadOnlyCollection<WebDavProperty>? _properties;
            private IReadOnlyCollection<WebDavPropertyStatus>? _propertyStatuses;

            /// <summary>
            /// Sets the ActiveLocks parameter of an instance of the <see cref="WebDavResource"/> class.
            /// </summary>
            public Builder WithActiveLocks(IReadOnlyCollection<ActiveLock> activeLocks)
            {
                _activeLocks = activeLocks;
                return this;
            }

            /// <summary>
            /// Sets the ContentLanguage parameter of an instance of the <see cref="WebDavResource"/> class.
            /// </summary>
            public Builder WithContentLanguage(string? contentLanguage)
            {
                _contentLanguage = contentLanguage;
                return this;
            }

            /// <summary>
            /// Sets the ContentLength parameter of an instance of the <see cref="WebDavResource"/> class.
            /// </summary>
            public Builder WithContentLength(long? contentLength)
            {
                _contentLength = contentLength;
                return this;
            }

            /// <summary>
            /// Sets the ContentType parameter of an instance of the <see cref="WebDavResource"/> class.
            /// </summary>
            public Builder WithContentType(string? contentType)
            {
                _contentType = contentType;
                return this;
            }

            /// <summary>
            /// Sets the CreationDate parameter of an instance of the <see cref="WebDavResource"/> class.
            /// </summary>
            public Builder WithCreationDate(DateTime? creationDate)
            {
                _creationDate = creationDate;
                return this;
            }

            /// <summary>
            /// Sets the DisplayName parameter of an instance of the <see cref="WebDavResource"/> class.
            /// </summary>
            public Builder WithDisplayName(string? displayName)
            {
                _displayName = displayName;
                return this;
            }

            /// <summary>
            /// Sets the ETag parameter of an instance of the <see cref="WebDavResource"/> class.
            /// </summary>
            public Builder WithETag(string? eTag)
            {
                _eTag = eTag;
                return this;
            }

            /// <summary>
            /// Sets the Uri parameter of an instance of the <see cref="WebDavResource"/> class.
            /// </summary>
            public Builder WithUri(string? uri)
            {
                _uri = uri;
                return this;
            }

            /// <summary>
            /// Sets the IsCollection parameter of an instance of the <see cref="WebDavResource"/> class to <c>true</c>.
            /// </summary>
            public Builder IsCollection()
            {
                _isCollection = true;
                return this;
            }

            /// <summary>
            /// Sets the IsCollection parameter of an instance of the <see cref="WebDavResource"/> class to <c>false</c>.
            /// </summary>
            public Builder IsNotCollection()
            {
                _isCollection = false;
                return this;
            }

            /// <summary>
            /// Sets the IsHidden parameter of an instance of the <see cref="WebDavResource"/> class to <c>true</c>.
            /// </summary>
            public Builder IsHidden()
            {
                _isHidden = true;
                return this;
            }

            /// <summary>
            /// Sets the IsHidden parameter of an instance of the <see cref="WebDavResource"/> class to <c>false</c>.
            /// </summary>
            public Builder IsNotHidden()
            {
                _isHidden = false;
                return this;
            }

            /// <summary>
            /// Sets the LastModifiedDate parameter of an instance of the <see cref="WebDavResource"/> class.
            /// </summary>
            public Builder WithLastModifiedDate(DateTime? lastModifiedDate)
            {
                _lastModifiedDate = lastModifiedDate;
                return this;
            }

            /// <summary>
            /// Sets the Properties parameter of an instance of the <see cref="WebDavResource"/> class.
            /// </summary>
            public Builder WithProperties(IReadOnlyCollection<WebDavProperty> properties)
            {
                Guard.NotNull(properties, "properties");
                _properties = properties;
                return this;
            }

            /// <summary>
            /// Sets the PropertyStatuses parameter of an instance of the <see cref="WebDavResource"/> class.
            /// </summary>
            public Builder WithPropertyStatuses(IReadOnlyCollection<WebDavPropertyStatus> propertyStatuses)
            {
                Guard.NotNull(propertyStatuses, "propertyStatuses");
                _propertyStatuses = propertyStatuses;
                return this;
            }

            /// <summary>
            /// Builds a new instance of the <see cref="WebDavResource"/> class.
            /// </summary>
            /// <returns>A new instance of the <see cref="WebDavResource"/> class.</returns>
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
                    Uri = _uri,
                    IsCollection = _isCollection,
                    IsHidden = _isHidden,
                    LastModifiedDate = _lastModifiedDate,
                    Properties = _properties!,
                    PropertyStatuses = _propertyStatuses!
                };
            }
        }
    }
}
