namespace WizardSoftTestTaskAPI.DTO.Errors
{
    /// <summary>
    /// Модель для возвращение ошибок в читаемом формате
    /// </summary>
    public class ResponseErrorDTO
    {
        /// <summary>
        /// Описание ошибки
        /// </summary>
        public string Error { get; set; } = string.Empty;
    }
}