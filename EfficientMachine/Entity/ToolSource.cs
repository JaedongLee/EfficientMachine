namespace EfficientMachine.Entity
{
    public class ToolSource
    
    {
        public int Id { get; set; }
        public string Type { get; set; }
        public string URLForRelease { get; set; }
        public int ToolId { get; set; }
        public string Homepage { get; set; }
        public string ReleaseAssetRegex { get; set; }
    }
}