namespace ToDo.API.Configurations
{
    public class ApplicationConfiguration
    {
        public string TargetUploadFolder { get; set; } = string.Empty;
        public string[] AllowedDocumentExtensions { get; set; } = Array.Empty<string>();
        public string[] AllowedImageExtensions { get; set; } = Array.Empty<string>();
        public long DocumentMaxBytes { get; set; }
    }
}
