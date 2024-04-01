using System.ComponentModel.DataAnnotations;

namespace GreenwichUniversityMagazine.Models
{
    public class Term
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

    }
}
