namespace WizardSoftTestTaskAPI.DTO.Catalog
{
    public class CatalogDTO
    {
        /// <summary>
        /// ID каталога
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// Наименование каталога
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// ID родительского каталога
        /// </summary>
        public long? ParentId { get; set; }

        public CatalogDTO(WizardSoftTestTaskAPI.Models.Catalog catalog)
        {
            Id = catalog.Id;
            Name = catalog.Name;
            ParentId = catalog.ParentId;
        }
    }
}