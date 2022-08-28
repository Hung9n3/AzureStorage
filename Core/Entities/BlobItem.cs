namespace AzureStorage.Core.Entities
{
    public class BlobItem
    {
        public int Id { get; set; }
        public string Name { get; set; } = String.Empty;
        public string Url { get; set; } = String.Empty;
        public int Container { get; set; }
        public string Tag { get; set; } = String.Empty ;
    }
}
