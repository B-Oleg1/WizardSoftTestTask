using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WizardSoftTestTaskAPI.Data;
using WizardSoftTestTaskAPI.DTO.Catalog;
using WizardSoftTestTaskAPI.DTO.Errors;
using WizardSoftTestTaskAPI.DTO.Paginations;
using WizardSoftTestTaskAPI.Models;

namespace WizardSoftTestTaskAPI.Controllers
{
    /// <summary>
    /// Управление каталогами через CRUD операции
    /// </summary>
    [Route("api/v1/[controller]")]
    [ApiController]
    public class CatalogsController : ControllerBase
    {
        private readonly CatalogsDbContext _catalogsDbContext;

        public CatalogsController(CatalogsDbContext catalogsDbContext)
        {
            _catalogsDbContext = catalogsDbContext;
        }

        /// <summary>
        /// Получение списка каталогов
        /// </summary>
        /// <remarks>
        /// Возвращает список каталогов, которые являются дочерними у каталога с ID, указанным в <c>parentCatalogId</c>
        /// </remarks>
        /// <param name="parentId">ID родительского каталога. Пустое значение - если у каталога не должно быть родительского элемента</param>
        /// <param name="requestPaginationDTO">Пагинация данных, получаемых из запроса</param>
        /// <response code="200">Успешное получение списка каталогов</response>
        [HttpGet]
        [ProducesResponseType(typeof(ResponsePaginationDTO<CatalogDTO>), StatusCodes.Status200OK, "application/json")]
        public async Task<IActionResult> GetCatalogs([FromQuery] long? parentId = null,
                                                     [FromQuery] RequestPaginationDTO? requestPaginationDTO = null)
        {
            IQueryable<Catalog> queryableCatalogs = _catalogsDbContext.Catalogs.Where(c => c.ParentId == parentId);
            long totalCountElements = await queryableCatalogs.LongCountAsync();

            requestPaginationDTO ??= new();
            List<CatalogDTO> catalogsDTO = await queryableCatalogs.Skip((requestPaginationDTO.PageNumber - 1) * requestPaginationDTO.PageSize)
                                                                  .Take(requestPaginationDTO.PageSize)
                                                                  .OrderBy(c => c.Id)
                                                                  .Select(c => new CatalogDTO(c)).ToListAsync();

            ResponsePaginationDTO<CatalogDTO> responsePaginationDTO = new()
            {
                PageSize = requestPaginationDTO.PageSize,
                PageNumber = requestPaginationDTO.PageNumber,
                TotalPages = (int)Math.Ceiling((double)totalCountElements / requestPaginationDTO.PageSize),
                Data = catalogsDTO
            };

            return Ok(responsePaginationDTO);
        }

        /// <summary>
        /// Получение каталога по ID
        /// </summary>
        /// <param name="catalogId">ID каталога, который необходимо получить</param>
        /// <response code="200">Успешное получение каталога</response>
        /// <response code="404">Каталог не найден</response>
        [HttpGet]
        [Route("{catalogId}")]
        [ProducesResponseType(typeof(CatalogDTO), StatusCodes.Status200OK, "application/json")]
        [ProducesResponseType(typeof(ResponseErrorDTO), StatusCodes.Status404NotFound, "application/json")]
        public async Task<IActionResult> GetCatalogById(long catalogId)
        {
            CatalogDTO? catalogDTO = await _catalogsDbContext.Catalogs
                                                             .Where(c => c.Id == catalogId)
                                                             .Select(c => new CatalogDTO(c))
                                                             .SingleOrDefaultAsync();

            if (catalogDTO == null)
            {
                return NotFound(new ResponseErrorDTO()
                {
                    Error = "Не удалось найти каталог по указанному ID"
                });
            }

            return Ok(catalogDTO);
        }

        /// <summary>
        /// Добавление нового каталога
        /// </summary>
        /// <param name="createCatalogDTO">Данные нового каталога</param>
        /// <response code="201">Успешное добавление нового каталога</response>
        /// <response code="404">Родительский каталог не найден</response>
        [HttpPost]
        [ProducesResponseType(typeof(CatalogDTO), StatusCodes.Status201Created, "application/json")]
        [ProducesResponseType(typeof(ResponseErrorDTO), StatusCodes.Status404NotFound, "application/json")]
        public async Task<IActionResult> CreateCatalog([FromBody] CreateCatalogDTO createCatalogDTO)
        {
            long? parentCatalogId = null;

            // Если указан родительский каталог
            if (createCatalogDTO.ParentId != null)
            {
                // Проверяем на существование родительского каталога
                var parentCatalog = await _catalogsDbContext.Catalogs.FindAsync(createCatalogDTO.ParentId);
                if (parentCatalog == null)
                {
                    return NotFound(new ResponseErrorDTO()
                    {
                        Error = "Не удалось найти родительский каталог."
                    });
                }

                parentCatalogId = parentCatalog.Id;
            }

            // Формируем экземляр нового каталога
            Catalog newCatalog = new()
            {
                Name = createCatalogDTO.Name,
                ParentId = parentCatalogId,
            };

            await _catalogsDbContext.Catalogs.AddAsync(newCatalog);
            await _catalogsDbContext.SaveChangesAsync();

            CatalogDTO catalogDTO = new(newCatalog);

            return CreatedAtAction(nameof(GetCatalogById), new { catalogId = newCatalog.Id }, catalogDTO);
        }

        /// <summary>
        /// Изменение параметров у каталога
        /// </summary>
        /// <param name="catalogId">ID каталога, который необходимо изменить</param>
        /// <param name="updateCatalogDTO">Данные, которые нужно поменять у каталога</param>
        /// <response code="200">Успешное изменение каталога</response>
        /// <response code="404">Каталог не найден</response>
        [HttpPut]
        [Route("{catalogId}")]
        [ProducesResponseType(typeof(CatalogDTO), StatusCodes.Status200OK, "application/json")]
        [ProducesResponseType(typeof(ResponseErrorDTO), StatusCodes.Status404NotFound, "application/json")]
        public async Task<IActionResult> UpdateCatalog(long catalogId, [FromBody] UpdateCatalogDTO updateCatalogDTO)
        {
            Catalog? catalog = await _catalogsDbContext.Catalogs.FindAsync(catalogId);
            if (catalog == null)
            {
                return NotFound(new ResponseErrorDTO()
                {
                     Error = "Не удалось найти каталог по указанному ID"
                });
            }

            // Меняем название каталога
            if (!string.IsNullOrEmpty(updateCatalogDTO.Name))
            {
                catalog.Name = updateCatalogDTO.Name;
            }

            // Меняем родителя у текущего каталога
            if (updateCatalogDTO.ParentId != null)
            {
                if (await _catalogsDbContext.Catalogs.FindAsync(updateCatalogDTO.ParentId) == null)
                {
                    return NotFound(new ResponseErrorDTO()
                    {
                        Error = "Не удалось найти родительский каталог."
                    });
                }

                catalog.ParentId = updateCatalogDTO.ParentId;
            }

            await _catalogsDbContext.SaveChangesAsync();

            CatalogDTO catalogDTO = new(catalog);

            return Ok(catalogDTO);
        }

        /// <summary>
        /// Удаление каталога
        /// </summary>
        /// <param name="catalogId">ID каталога, который необходимо удалить</param>
        /// <response code="200">Успешное удаление каталога</response>
        /// <response code="404">Каталог не найден</response>
        [HttpDelete]
        [Route("{catalogId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseErrorDTO), StatusCodes.Status404NotFound, "application/json")]
        public async Task<IActionResult> DeleteCatalog(long catalogId)
        {
            Catalog? catalog = await _catalogsDbContext.Catalogs.FindAsync(catalogId);
            if (catalog == null)
            {
                return NotFound(new ResponseErrorDTO()
                {
                    Error = "Не удалось найти каталог по указанному ID"
                });
            }

            _catalogsDbContext.Catalogs.Remove(catalog);
            await _catalogsDbContext.SaveChangesAsync();

            return Ok();
        }
    }
}