namespace WebDav
{
    /// <summary>
    /// Specifies a type of PROPFIND request.
    /// AllProperties: 'allprop' + 'include'.
    /// NamedProperties: 'prop'.
    /// </summary>
    public enum PropfindRequestType
    {
        AllProperties,
        NamedProperties,

        /// <summary>
        /// Don't pass a body
        /// </summary>
        AllPropertiesImplied,
    }
}
