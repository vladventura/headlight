using System.ComponentModel.DataAnnotations.Schema;

namespace Headlight.Models
{
    [Table("game")]
    public class Game
    {
        public int Id { get; set; }
        public int StatusId { get; set; }
        public int PlatformId { get; set; }
        public Status Status { get; set; } = null;
        public Platform Platform { get; set; } = null;
        public string Name { get; set; } = string.Empty;
        public DateTime? FinishedDateTime { get; set; }
    }
}
