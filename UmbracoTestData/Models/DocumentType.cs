using System;

namespace UmbracoTestData.Models
{
    internal class DocumentType
    {
        public DocumentType(Guid key, string name)
        {
            var templateName = name.ToCamelCase();
            Key = key;
            Name = name;
            Alias = name.ToCamelCase();
            AllowedTemplates = new[] { templateName };
            DefaultTemplate = templateName;
        }

        public Guid[] CompositeContentTypes { get; set; } = Array.Empty<Guid>();
        public bool IsContainer { get; set; }
        public bool AllowAsRoot { get; set; }
        public string[] AllowedTemplates { get; set; }
        public int[] AllowedContentTypes { get; set; } = Array.Empty<int>();
        public string Alias { get; set; }
        public string Description { get; set; }
        public string Thumbnail { get; set; } = "folder.png";
        public string Name { get; set; }
        public int Id { get; set; }
        public string Icon { get; set; } = "icon-document";
        public bool Trashed { get; set; }
        public Guid Key { get; set; }
        public int ParentId { get; set; } = -1;
        public string Path { get; set; }
        public bool AllowCultureVariant { get; set; }
        public bool IsElement { get; set; }
        public string DefaultTemplate { get; set; }
        public DocumentTypeGroup[] Groups { get; set; } = Array.Empty<DocumentTypeGroup>();
    }

    internal class DocumentTypeGroup
    {
        public int SortOrder { get; set; }
        public string Name { get; set; }= string.Empty;
        public DocumentTypeGroupProperty[] Properties { get; set; }= Array.Empty<DocumentTypeGroupProperty>();
    }

    internal class DocumentTypeGroupProperty
    {
        public string Alias { get; set; }
        public string Label { get; set; }
        public int SortOrder { get; set; }
        public int DataTypeId { get; set; }
        public bool AllowCultureVariant { get; set; }
        public DocumentTypeGroupPropertyValidation Validation { get; set; } = new DocumentTypeGroupPropertyValidation();
    }

    internal class DocumentTypeGroupPropertyValidation
    {
        public bool Mandatory { get; set; }
        public string Pattern { get; set; }
    }
}