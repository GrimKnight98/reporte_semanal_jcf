using System.ComponentModel.DataAnnotations;
namespace DemoMvc.Models
{
    public class Instructor
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }
        [Required]
        public string Position { get; set; }
        [Required]
        public string Department { get; set; }
        [Required]
        public string Status { get; set; }
        //new fields 
        public string PersonNumber { get; set; }

        public string? SignPath { get; set; }

        //end new fields
        public int CreatedBy { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        // Relación uno-a-muchos con Report
        public ICollection<Report>? Reports { get; set; } = new List<Report>();
            
    }
}