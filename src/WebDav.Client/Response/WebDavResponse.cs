﻿namespace WebDav
{
    /// <summary>
    /// Represents a response of a WebDAV operation.
    /// </summary>
    public class WebDavResponse
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WebDavResponse"/> class.
        /// </summary>
        /// <param name="statusCode">The status code of the operation.</param>
        public WebDavResponse(int statusCode)
        {
            StatusCode = statusCode;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WebDavResponse"/> class.
        /// </summary>
        /// <param name="statusCode">The status code of the response.</param>
        /// <param name="description">The description of the response.</param>
        public WebDavResponse(int statusCode, string? description)
        {
            StatusCode = statusCode;
            Description = description;
        }

        /// <summary>
        /// Gets the status code of the response.
        /// </summary>
        public int StatusCode { get; }

        /// <summary>
        /// Gets the description of the response.
        /// </summary>
        public string? Description { get; }

        /// <summary>
        /// Gets a value indicating whether the operation was successful.
        /// </summary>
        /// <value>
        /// <c>true</c> if the operation was successful; otherwise, <c>false</c>.
        /// </value>
        public virtual bool IsSuccessful => StatusCode >= 200 && StatusCode <= 299;

        public override string ToString()
        {
            return $"WebDAV response - StatusCode: {StatusCode}, Description: {Description}";
        }
    }
}
