using System.ComponentModel.DataAnnotations.Schema;

namespace Headlight.Models
{
    [Table("status")]
    public class Status
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public ICollection<Game> Games { get; } = new List<Game>();
    }
}
