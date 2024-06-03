using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace PrecedentExpert.Models
{

[Table("situation_variables")]
public class SituationVariable : INotifyPropertyChanged
        {
            [Key]
            [Column("situationid")] 
            public int Id { get; set; }
            [Column("name")]
            public string? Name { get; set; }
            [Column("object_id"), ForeignKey("Objects")] // Указываем, что это внешний ключ, связанный с таблицей Objects
            public int ObjectId { get; set; }

        public event PropertyChangedEventHandler? PropertyChanged;

        internal object FirstOrDefault(Func<object, bool> value)
        {
            throw new NotImplementedException();
        }
    }
}