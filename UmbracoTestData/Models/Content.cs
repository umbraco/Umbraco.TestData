using System;
// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable MemberCanBePrivate.Global

namespace UmbracoTestData.Models
{
    internal class Content
    {
        public int Id { get; set; }
        public string ContentTypeAlias { get; set; }
        public int ParentId { get; set; }
        public string Action { get; set; }
        public string TemplateAlias { get; set; }
        public DateTimeOffset? ExpireDate { get; set; }
        public DateTimeOffset? ReleaseDate { get; set; }
        public ContentVariant[] Variants { get; set; } = Array.Empty<ContentVariant>();
    }

    internal class ContentVariant
    {
        public string Name { get; set; }
        public string Culture { get; set; } = "en-US";
        public bool Publish { get; set; }
        public bool Save { get; set; } = true;
        public DateTimeOffset? ReleaseDate { get; set; }
        public DateTimeOffset? ExpireDate { get; set; }
        public ContentVariantProperty[] Properties { get; set; } = Array.Empty<ContentVariantProperty>();
    }

    internal class ContentVariantProperty
    {
        public int Id { get; set; }
        public string Alias { get; set; }
        public object Value { get; set; }
    }
}