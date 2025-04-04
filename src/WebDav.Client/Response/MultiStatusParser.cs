﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace WebDav
{
    internal static class MultiStatusParser
    {
        private static readonly Regex StatusCodeRegex = new Regex(@".*(\d{3}).*");

        public static List<Propstat> GetPropstats(XElement xresponse)
        {
            return xresponse.LocalNameElements("propstat", StringComparison.OrdinalIgnoreCase)
                .Select(x =>
                    new Propstat
                    {
                        Element = x,
                        StatusCode = GetStatusCodeFromPropstat(x),
                        Description = GetDescriptionFromPropstat(x)
                    })
                .ToList();
        }

        public static List<WebDavPropertyStatus> GetPropertyStatuses(XElement xresponse)
        {
            var propstats = GetPropstats(xresponse);
            return GetPropertyStatuses(propstats);
        }

        public static List<WebDavPropertyStatus> GetPropertyStatuses(List<Propstat> propstats)
        {
            return propstats
                .SelectMany(x => x.Element?.LocalNameElements("prop", StringComparison.OrdinalIgnoreCase)
                    .SelectMany(p => p.Elements())
                    .Select(p => new { Prop = p, StatusCode = x.StatusCode, Description = x.Description }))
                .Select(x => new WebDavPropertyStatus(x.Prop.Name, x.StatusCode, x.Description))
                .ToList();
        }

        public static List<XElement> GetProperties(List<Propstat> propstats)
        {
            return propstats
                .Where(x => IsSuccessStatusCode(x.StatusCode))
                .SelectMany(x => x.Element?.LocalNameElements("prop", StringComparison.OrdinalIgnoreCase))
                .SelectMany(x => x.Elements())
                .ToList();
        }

        private static bool IsSuccessStatusCode(int statusCode)
        {
            return statusCode >= 200 && statusCode <= 299;
        }

        private static string? GetDescriptionFromPropstat(XElement propstat)
        {
            return
                propstat.LocalNameElement("responsedescription", StringComparison.OrdinalIgnoreCase).GetValueOrNull() ??
                propstat.LocalNameElement("status", StringComparison.OrdinalIgnoreCase).GetValueOrNull();
        }

        private static int GetStatusCodeFromPropstat(XElement propstat)
        {
            var statusRawValue = propstat.LocalNameElement("status", StringComparison.OrdinalIgnoreCase).GetValueOrNull();
            if (string.IsNullOrEmpty(statusRawValue))
                return default(int);

            var statusCodeGroup = StatusCodeRegex.Match(statusRawValue).Groups[1];
            if (!statusCodeGroup.Success)
                return default(int);

            int statusCode;
            if (!int.TryParse(statusCodeGroup.Value, out statusCode))
                return default(int);

            return statusCode;
        }

        internal class Propstat
        {
            public XElement? Element { get; set; }

            public int StatusCode { get; set; }

            public string? Description { get; set; }
        }
    }
}
