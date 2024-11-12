using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WizardSoftTestTaskAPI.Models
{
    /// <summary>
    /// Структура таблицы каталогов
    /// </summary>
    public class Catalog
    {
        /// <summary>
        /// ID каталога
        /// </summary>
        [Key]
        public long Id { get; set; }

        /// <summary>
        /// Наименование каталога
        /// </summary>
        [Required]
        public required string Name { get; set; }

        /// <summary>
        /// Внешний ключ на родительский каталог
        /// </summary>
        public long? ParentId { get; set; }

        /// <summary>
        /// Объект родительского каталога
        /// </summary>
        [ForeignKey("ParentId")]
        public Catalog? Parent { get; set; }

        /// <summary>
        /// Дочерние каталоги
        /// </summary>
        public ICollection<Catalog> Children { get; set; } = [];
    }
}