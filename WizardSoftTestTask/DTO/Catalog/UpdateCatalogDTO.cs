using System.ComponentModel;

namespace WizardSoftTestTaskAPI.DTO.Catalog
{
    /// <summary>
    /// Модель для изменения данных о каталоге
    /// </summary>
    public class UpdateCatalogDTO
    {
        /// <summary>
        /// Наименование каталога
        /// </summary>
        [DefaultValue(null)]
        public string? Name { get; set; } = null;

        /// <summary>
        /// Родительский элемент нового каталога.
        /// <c>Пустое значение</c> - если у каталога нет родительского элемента
        /// </summary>
        public long? ParentId { get; set; } = null;
    }
}