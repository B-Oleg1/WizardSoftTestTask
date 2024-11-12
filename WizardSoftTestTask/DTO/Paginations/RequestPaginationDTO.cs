using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace WizardSoftTestTaskAPI.DTO.Paginations
{
    /// <summary>
    /// Модель для пагинации данных, полученных из запроса
    /// </summary>
    public class RequestPaginationDTO
    {
        /// <summary>
        /// Количество данных на одной странице. Минимальный размер - 1, максимальный - 100
        /// </summary>
        [DefaultValue(30)]
        [Range(1, 100)]
        public int PageSize { get; set; } = 30;

        /// <summary>
        /// Номер страницы, с которой нужно получить данные. Нумерация начинается с 1
        /// </summary>
        [DefaultValue(1)]
        [Range(1, int.MaxValue)]
        public int PageNumber { get; set; } = 1;
    }
}