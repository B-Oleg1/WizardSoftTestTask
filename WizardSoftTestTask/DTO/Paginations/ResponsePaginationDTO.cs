namespace WizardSoftTestTaskAPI.DTO.Paginations
{
    /// <summary>
    /// Модель для пагинации данных, полученных из запроса
    /// </summary>
    public class ResponsePaginationDTO<T>
    {
        /// <summary>
        /// Количество элементов на странице
        /// </summary>
        public int PageSize { get; set; }

        /// <summary>
        /// Номер текущей страницы
        /// </summary>
        public int PageNumber { get; set; }

        /// <summary>
        /// Общее количество страниц, которое может вернуться из запроса
        /// </summary>
        public int TotalPages { get; set; }

        /// <summary>
        /// Коллекция данных на текущей странице
        /// </summary>
        public IEnumerable<T> Data { get; set; } = [];
    }
}