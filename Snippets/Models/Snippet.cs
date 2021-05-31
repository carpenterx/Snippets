namespace Snippets.Models
{
    public class Snippet
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string Prerequisites { get; set; }
        public string Code { get; set; }
        public int UseCount { get; set; }
    }
}
