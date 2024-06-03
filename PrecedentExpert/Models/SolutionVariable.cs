using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace PrecedentExpert.Models

{
[Table("solution_variables")]
public class SolutionVariable : INotifyPropertyChanged
        {
            [Key]  
            [Column("solutionid")]          
            public int SolutionId { get; set; }
            [Column("name")]
            public string? Name { get; set; }
            
            [Column("object_id"), ForeignKey("Objects")] // Указываем, что это внешний ключ, связанный с таблицей Objects
            public int ObjectId { get; set; }
            public event PropertyChangedEventHandler? PropertyChanged;

        }
}