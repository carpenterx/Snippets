using YamlDotNet.Core;
using YamlDotNet.Serialization;

namespace Snippets.Models
{
    public class Snippet
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public int Used { get; set; }
        [YamlMember(ScalarStyle = ScalarStyle.Literal)]
        public string Prerequisites { get; set; }
        [YamlMember(ScalarStyle = ScalarStyle.Literal)]
        public string Code { get; set; }
    }
}
