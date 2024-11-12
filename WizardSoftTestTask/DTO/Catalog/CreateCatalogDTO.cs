using System.ComponentModel.DataAnnotations;

namespace WizardSoftTestTaskAPI.DTO.Catalog
{
    /// <summary>
    /// Модель с данными для создания нового каталога
    /// </summary>
    public class CreateCatalogDTO
    {
        /// <summary>
        /// Наименование каталога.
        /// Длина названия должна быть от 1 до 1000 символов включительно
        /// </summary>
        [Required]
        [Length(1, 1000)]
        public required string Name { get; set; }

        /// <summary>
        /// Родительский элемент нового каталога.
        /// <c>Пустое значение</c> - если у каталога нет родительского элемента
        /// </summary>
        public long? ParentId { get; set; } = null;
    }
}