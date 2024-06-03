using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PrecedentExpert.Models
{
    [Table("precedents")]
    public class Precedent
    {
        [Key]
        [Column("id")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Column("solutions")]
        public int[] SolutionParams { get; set; }
        
        [Column("situation_params")]
        public int[] SituationParams { get; set; }

        [Column("object_id")]
        public int ObjectId { get; set; }
    }
}
