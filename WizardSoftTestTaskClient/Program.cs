using Newtonsoft.Json;
using WizardSoftTestTaskAPI.DTO.Catalog;
using WizardSoftTestTaskAPI.DTO.Paginations;

const string serverAddress = "http://localhost:5000/api/v1/";
var client = new HttpClient();

int currentPage = 1;
long? parentId = null;

while (true)
{
	var catalogsReseponse = await GetCatalogs(parentId, currentPage);

    Console.WriteLine($"Текущий каталог - {(parentId == null ? "root" : parentId)}. Вложенные:");
	foreach (var catalog in catalogsReseponse.Data)
	{
		Console.WriteLine($"[{catalog.Id} ID] {catalog.Name}");
	}
    Console.WriteLine($"\nСтраница: {catalogsReseponse.PageNumber} из {catalogsReseponse.TotalPages}\n");

    Console.WriteLine("Введите команду:");
    Console.WriteLine("n - следующая страница");
    Console.WriteLine("p - предыдущая страница");
    Console.WriteLine("cd [catalogId] - зайти в каталог");
    Console.WriteLine(".. - выйти из текущего каталога назад");
    Console.WriteLine("mk [catalogName] - создать каталог в текущем каталоге");
    Console.WriteLine("rm [catalogId] - удалить каталог");

    string action = Console.ReadLine();
	switch (action?.Split(' ')[0])
	{
		case "n":
			currentPage = Math.Min(currentPage + 1, catalogsReseponse.TotalPages);
            break;
        case "p":
            currentPage = Math.Max(currentPage - 1, 1);
            break;
        case "cd":
            long newParentId = long.Parse(action.Split(' ')[1]);
            if (catalogsReseponse.Data.Any(c => c.Id == newParentId))
            {
                parentId = newParentId;
                currentPage = 1;
            }
            break;
        case "..":
            if (parentId == null)
            {
                break;
            }

            var parentCatalog = await GetCatalogById((long)parentId!);
            parentId = parentCatalog?.ParentId ?? null;
            currentPage = 1;
            break;
        case "mk":
            await AddNewCatalog(parentId, action.Substring(action.IndexOf(' ') + 1));
            break;
        case "rm":
            long removeCatalogId = long.Parse(action.Split(' ')[1]);
            if (catalogsReseponse.Data.Any(c => c.Id == removeCatalogId))
            {
                await DeleteCatalog(removeCatalogId);
            }
            break;
        default:
			break;
	}

    Console.Clear();
}

async Task<ResponsePaginationDTO<CatalogDTO>?> GetCatalogs(long? parentCatalogId = null, int pageNumber = 1)
{
	string parentCatalogIdQuery = string.Empty;
	if (parentCatalogId != null)
	{
		parentCatalogIdQuery = $"&parentId={parentCatalogId}";
    }

	try
	{
        using var request = new HttpRequestMessage(HttpMethod.Get, $"{serverAddress}Catalogs?PageNumber={pageNumber}{parentCatalogIdQuery}");
        using var response = await client.SendAsync(request);

        response.EnsureSuccessStatusCode();

		var responseBody = await response.Content.ReadAsStringAsync();
		var responseObject = JsonConvert.DeserializeObject<ResponsePaginationDTO<CatalogDTO>>(responseBody);

		return responseObject;
    }
	catch (Exception ex)
	{
		Console.WriteLine(ex);
	}

    return null;
}

async Task<CatalogDTO?> GetCatalogById(long catalogId)
{
    try
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, $"{serverAddress}Catalogs/{catalogId}");
        using var response = await client.SendAsync(request);

        response.EnsureSuccessStatusCode();

        var responseBody = await response.Content.ReadAsStringAsync();
        var responseObject = JsonConvert.DeserializeObject<CatalogDTO>(responseBody);

        return responseObject;
    }
    catch (Exception ex)
    {
        Console.WriteLine(ex);
    }

    return null;
}

async Task<CatalogDTO?> AddNewCatalog(long? parentId, string name)
{
    var parentIdStr = parentId == null ? "null" : parentId.ToString();
    try
    {
        using var request = new HttpRequestMessage(HttpMethod.Post, $"{serverAddress}Catalogs");
        var content = new StringContent($"{{\r\n  \"name\": \"{name}\",\r\n  \"parentId\": {parentIdStr}\r\n}}", null, "application/json");
        request.Content = content;
        using var response = await client.SendAsync(request);

        response.EnsureSuccessStatusCode();

        var responseBody = await response.Content.ReadAsStringAsync();
        var responseObject = JsonConvert.DeserializeObject<CatalogDTO>(responseBody);

        return responseObject;
    }
    catch (Exception ex)
    {
        Console.WriteLine(ex);
    }

    return null;
}

async Task<bool> DeleteCatalog(long catalogId)
{
    try
    {
        using var request = new HttpRequestMessage(HttpMethod.Delete, $"{serverAddress}Catalogs/{catalogId}");
        using var response = await client.SendAsync(request);

        response.EnsureSuccessStatusCode();

        return true;
    }
    catch (Exception ex)
    {
        Console.WriteLine(ex);
    }

    return false;
}