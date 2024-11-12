using Newtonsoft.Json;
using System;
using WizardSoftTestTaskAPI.DTO.Catalog;
using WizardSoftTestTaskAPI.DTO.Paginations;
using WizardSoftTestTaskClient.Repositories;

CatalogRepository catalogRepository = new();
int currentPage = 1;
long? parentId = null;

while (true)
{
	var catalogsInfo = await catalogRepository.GetCatalogs(parentId, currentPage);
    PrintCatalogsInfo(catalogsInfo);

    string action = Console.ReadLine();
    string param = action.Substring(action.IndexOf(' ') + 1);

    switch (action?.Split(' ')[0])
	{
		case "n":
			currentPage = Math.Min(currentPage + 1, catalogsInfo.TotalPages);
            break;
        case "p":
            currentPage = Math.Max(currentPage - 1, 1);
            break;
        case "cd":
            JoinInCatalog(long.Parse(param), catalogsInfo.Data);
            break;
        case "..":
            await LeftFromCatalog();
            break;
        case "mk":
            await catalogRepository.CreateCatalog(parentId, param);
            break;
        case "rm":
            long removeCatalogId = long.Parse(param);
            if (catalogsInfo.Data.Any(c => c.Id == removeCatalogId))
            {
                await catalogRepository.DeleteCatalog(removeCatalogId);
            }
            break;
        case "upd":
            long updateCatalogId = long.Parse(param);
            if (catalogsInfo.Data.Any(c => c.Id == updateCatalogId))
            {
                await UpdateCatalog(updateCatalogId);
            }
            break;
        default:
			break;
	}

    Console.Clear();
}

void PrintCatalogsInfo(ResponsePaginationDTO<CatalogDTO> catalogsInfo)
{
    Console.WriteLine($"Текущий каталог - {(parentId == null ? "root" : parentId)}. Вложенные:");
    foreach (var catalog in catalogsInfo.Data)
    {
        Console.WriteLine($"[{catalog.Id} ID] {catalog.Name}");
    }
    Console.WriteLine($"\nСтраница: {catalogsInfo.PageNumber} из {catalogsInfo.TotalPages}\n");

    Console.WriteLine("Введите команду:");
    Console.WriteLine("n - следующая страница");
    Console.WriteLine("p - предыдущая страница");
    Console.WriteLine("cd [catalogId] - зайти в каталог");
    Console.WriteLine(".. - выйти из текущего каталога назад");
    Console.WriteLine("mk [catalogName] - создать каталог в текущем каталоге");
    Console.WriteLine("rm [catalogId] - удалить каталог");
    Console.WriteLine("upd [catalogId] - изменить каталог");
    Console.WriteLine();
}

void JoinInCatalog(long newCatalogId, IEnumerable<CatalogDTO> currentCatalogs)
{
    // Если каталог, в который мы хотим войти, находится в этом же каталоге
    if (currentCatalogs.Any(c => c.Id == newCatalogId))
    {
        parentId = newCatalogId;
        currentPage = 1;
    }
}

async Task LeftFromCatalog()
{
    if (parentId == null)
    {
        return;
    }

    // Получаем родительский каталог
    var parentCatalog = await catalogRepository.GetCatalogById((long)parentId!);

    // Если родительский каталог пропал или он в root`е
    // То переходим в root
    parentId = parentCatalog?.ParentId ?? null;
    currentPage = 1;
}

async Task UpdateCatalog(long catalogId)
{
    Console.Write("Введите новое имя (или нажмите Enter, чтобы пропустить): ");
    string newName = Console.ReadLine();

    Console.Write("Введите ID нового родительского объекта (или нажмите Enter, чтобы пропустить): ");
    string newParentIdStr = Console.ReadLine();

    long? newParentId = null;
    if (long.TryParse(newParentIdStr, out long tmp))
    {
        newParentId = tmp;
    }

    await catalogRepository.UpdateCatalog(catalogId, newName, newParentId);
}