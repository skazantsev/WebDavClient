using System.Collections.Generic;

namespace WebDav.Response
{
    internal static class DefaultResourceProperties
    {
        public static Dictionary<string, ResourcePropertyType> Get()
        {
            return new Dictionary<string, ResourcePropertyType>
            {
                {"creationdate", ResourcePropertyType.DateTime},
                {"displayname", ResourcePropertyType.String},
                {"getcontentlanguage", ResourcePropertyType.String},
                {"getcontentlength", ResourcePropertyType.Integer},
                {"getcontenttype", ResourcePropertyType.String},
                {"getetag", ResourcePropertyType.String},
                {"getlastmodified", ResourcePropertyType.DateTime},
                {"resourcetype", ResourcePropertyType.ResourceType},
                {"iscollection", ResourcePropertyType.Integer},
                {"ishidden", ResourcePropertyType.Integer}
            };
        }
    }
}
