using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using CapacitiesCommandPaletteExtension.Parsing;
using CapacitiesCommandPaletteExtension.Settings;

namespace CapacitiesCommandPaletteExtension.Capacities;

internal sealed class CapacitiesClient
{
    private const string ApiVersionHeaderName = "X-Capacities-Api-Version";
    private const string ApiVersionHeaderValue = "0.1.0";
    private static readonly TimeSpan StructureCacheLifetime = TimeSpan.FromMinutes(10);
    private static readonly HashSet<string> DisallowedSearchStructureIds = new(StringComparer.OrdinalIgnoreCase)
    {
        "RootQuery",
        "MediaImage",
        "RootDailyNote",
        "RootAIChat",
        "RootTask",
        "MediaWebResource",
        "RootSimpleTable",
        "RootTag",
        "MediaPDF",
    };
    private static string _cachedStructureToken = string.Empty;
    private static DateTimeOffset _cachedStructureExpiry = DateTimeOffset.MinValue;
    private static IReadOnlyList<CapacitiesStructureMatch> _cachedStructures = [];
    private static IReadOnlyList<string> _cachedStructureIds = [];
    private static readonly Dictionary<string, CapacitiesObjectDetails> CachedObjectDetails = new(StringComparer.Ordinal);
    private static DateTimeOffset _objectDetailsRateLimitedUntil = DateTimeOffset.MinValue;
    private const int MaxObjectDetailsFetchPerSearch = 5;

    private static readonly Uri BaseAddress = new("https://api.capacities.io/");
    private static readonly HttpClient HttpClient = CreateHttpClient();

    public static CapacitiesWriteResult AppendToObject(string apiToken, string targetObjectId, CapCommandInput input)
    {
        var sanitizedApiToken = SanitizeApiToken(apiToken);
        var sanitizedTargetObjectId = targetObjectId.Trim();

        if (string.IsNullOrWhiteSpace(sanitizedApiToken))
        {
            return CapacitiesWriteResult.Failure("Capacities API token is empty after removing hidden characters. Paste the token again.");
        }

        if (ContainsNonAscii(sanitizedApiToken))
        {
            return CapacitiesWriteResult.Failure(
                "Capacities API token contains non-ASCII characters. Re-enter the token using plain keyboard characters only.");
        }

        if (string.IsNullOrWhiteSpace(sanitizedTargetObjectId))
        {
            return CapacitiesWriteResult.Failure("Choose a Capacities target object first.");
        }

        using var request = new HttpRequestMessage(HttpMethod.Post, "blocks/append")
        {
            Content = CreateRequestContent(sanitizedTargetObjectId, input),
        };

        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", sanitizedApiToken);

        try
        {
            using var response = HttpClient.Send(request);
            var responseBody = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();

            if (response.IsSuccessStatusCode)
            {
                return CapacitiesWriteResult.Success("Added to Capacities.");
            }

            return CapacitiesWriteResult.Failure(MapErrorMessage(response.StatusCode, responseBody));
        }
        catch (HttpRequestException ex)
        {
            return CapacitiesWriteResult.Failure($"Could not reach Capacities. {SummarizeException(ex)}");
        }
        catch (TaskCanceledException ex)
        {
            return CapacitiesWriteResult.Failure($"The request to Capacities timed out. {SummarizeException(ex)}");
        }
        catch (Exception ex)
        {
            return CapacitiesWriteResult.Failure($"Unexpected Capacities error. {SummarizeException(ex)}");
        }
    }

    public static CapacitiesObjectSearchResult SearchObjects(string apiToken, string query, int limit = 10)
    {
        var sanitizedToken = SanitizeApiToken(apiToken);
        if (string.IsNullOrWhiteSpace(sanitizedToken))
        {
            return CapacitiesObjectSearchResult.Failure("Add your Capacities API token first.");
        }

        if (ContainsNonAscii(sanitizedToken))
        {
            return CapacitiesObjectSearchResult.Failure("Capacities API token contains non-ASCII characters.");
        }

        if (string.IsNullOrWhiteSpace(query))
        {
            return CapacitiesObjectSearchResult.Success([]);
        }

        var trimmedQuery = query.Trim();
        var structures = GetSearchableStructureIds(sanitizedToken);
        var structureIds = string.IsNullOrWhiteSpace(structures.Error)
            ? structures.StructureIds
            : [];

        // Prefer structure-scoped search, but don't block searching if structures call is transiently unavailable.
        var primary = SendObjectSearchRequest(sanitizedToken, trimmedQuery, limit, structureIds);
        if (primary.Succeeded)
        {
            var parsedObjects = ParseObjects(primary.ResponseBody);
            var enrichedObjects = EnrichObjectsWithDetails(sanitizedToken, parsedObjects);
            return CapacitiesObjectSearchResult.Success(enrichedObjects);
        }

        if (IsTransientServerError(primary.StatusCode))
        {
            if (structureIds.Count > 0)
            {
                var fallback = SendObjectSearchRequest(sanitizedToken, trimmedQuery, limit, []);
                if (fallback.Succeeded)
                {
                    var parsedFallbackObjects = ParseObjects(fallback.ResponseBody);
                    var enrichedFallbackObjects = EnrichObjectsWithDetails(sanitizedToken, parsedFallbackObjects);
                    return CapacitiesObjectSearchResult.Success(enrichedFallbackObjects);
                }

                if (!string.IsNullOrWhiteSpace(fallback.Error))
                {
                    return CapacitiesObjectSearchResult.Failure(fallback.Error);
                }
            }
            else
            {
                // Retry once on transient backend failures.
                var retry = SendObjectSearchRequest(sanitizedToken, trimmedQuery, limit, structureIds);
                if (retry.Succeeded)
                {
                    var parsedRetryObjects = ParseObjects(retry.ResponseBody);
                    var enrichedRetryObjects = EnrichObjectsWithDetails(sanitizedToken, parsedRetryObjects);
                    return CapacitiesObjectSearchResult.Success(enrichedRetryObjects);
                }

                if (!string.IsNullOrWhiteSpace(retry.Error))
                {
                    return CapacitiesObjectSearchResult.Failure(retry.Error);
                }
            }
        }

        if (!string.IsNullOrWhiteSpace(primary.Error))
        {
            return CapacitiesObjectSearchResult.Failure(primary.Error);
        }

        return CapacitiesObjectSearchResult.Failure("Could not search objects. No additional details were returned.");
    }

    public static CapacitiesStructureSearchResult SearchStructures(string apiToken, string query, int limit = 20)
    {
        var sanitizedToken = SanitizeApiToken(apiToken);
        if (string.IsNullOrWhiteSpace(sanitizedToken))
        {
            return CapacitiesStructureSearchResult.Failure("Add your Capacities API token first.");
        }

        if (ContainsNonAscii(sanitizedToken))
        {
            return CapacitiesStructureSearchResult.Failure("Capacities API token contains non-ASCII characters.");
        }

        var structures = GetSearchableStructures(sanitizedToken);
        if (!string.IsNullOrWhiteSpace(structures.Error))
        {
            return CapacitiesStructureSearchResult.Failure(structures.Error);
        }

        if (string.IsNullOrWhiteSpace(query))
        {
            var top = new List<CapacitiesStructureMatch>();
            foreach (var structure in structures.Structures)
            {
                top.Add(structure);
                if (top.Count >= limit)
                {
                    break;
                }
            }

            return CapacitiesStructureSearchResult.Success(top);
        }

        var queryTrimmed = query.Trim();
        var filtered = new List<CapacitiesStructureMatch>();
        foreach (var structure in structures.Structures)
        {
            if (structure.Title.Contains(queryTrimmed, StringComparison.OrdinalIgnoreCase) ||
                structure.Id.Contains(queryTrimmed, StringComparison.OrdinalIgnoreCase))
            {
                filtered.Add(structure);
            }

            if (filtered.Count >= limit)
            {
                break;
            }
        }

        return CapacitiesStructureSearchResult.Success(filtered);
    }

    public static CapacitiesCreateObjectResult CreateObject(string apiToken, string structureId, string objectName, string objectContent)
    {
        var sanitizedToken = SanitizeApiToken(apiToken);
        if (string.IsNullOrWhiteSpace(sanitizedToken))
        {
            return CapacitiesCreateObjectResult.Failure("Add your Capacities API token first.");
        }

        if (ContainsNonAscii(sanitizedToken))
        {
            return CapacitiesCreateObjectResult.Failure("Capacities API token contains non-ASCII characters.");
        }

        if (string.IsNullOrWhiteSpace(structureId))
        {
            return CapacitiesCreateObjectResult.Failure("Choose a structure before creating an object.");
        }

        if (string.IsNullOrWhiteSpace(objectName))
        {
            return CapacitiesCreateObjectResult.Failure("Object name cannot be empty.");
        }

        using var request = new HttpRequestMessage(HttpMethod.Post, "object/markdown")
        {
            Content = CreateObjectRequestContent(structureId.Trim(), objectName.Trim(), objectContent),
        };
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", sanitizedToken);

        try
        {
            using var response = HttpClient.Send(request);
            var responseBody = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();

            if (!response.IsSuccessStatusCode)
            {
                return CapacitiesCreateObjectResult.Failure(MapErrorMessage(response.StatusCode, responseBody));
            }

            var createdObjectId = ParseObjectId(responseBody);
            if (string.IsNullOrWhiteSpace(createdObjectId))
            {
                return CapacitiesCreateObjectResult.Failure("Capacities created the object but returned no object ID.");
            }

            return CapacitiesCreateObjectResult.Success(createdObjectId, objectName.Trim());
        }
        catch (Exception ex)
        {
            return CapacitiesCreateObjectResult.Failure($"Could not create object. {SummarizeException(ex)}");
        }
    }

    public static CapacitiesSpaceResult GetSpaceId(string apiToken)
    {
        var sanitizedToken = SanitizeApiToken(apiToken);
        if (string.IsNullOrWhiteSpace(sanitizedToken))
        {
            return CapacitiesSpaceResult.Failure("Add your Capacities API token first.");
        }

        using var request = new HttpRequestMessage(HttpMethod.Get, "space");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", sanitizedToken);

        try
        {
            using var response = HttpClient.Send(request);
            var responseBody = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();

            if (!response.IsSuccessStatusCode)
            {
                return CapacitiesSpaceResult.Failure(MapErrorMessage(response.StatusCode, responseBody));
            }

            var spaceId = ParseSpaceId(responseBody);
            if (string.IsNullOrWhiteSpace(spaceId))
            {
                return CapacitiesSpaceResult.Failure("Capacities returned no space ID.");
            }

            return CapacitiesSpaceResult.Success(spaceId);
        }
        catch (Exception ex)
        {
            return CapacitiesSpaceResult.Failure($"Could not load space information. {SummarizeException(ex)}");
        }
    }

    public static string BuildObjectWebUrl(string spaceId, string objectId)
    {
        return $"https://app.capacities.io/{Uri.EscapeDataString(spaceId)}/{Uri.EscapeDataString(objectId)}";
    }

    private static HttpClient CreateHttpClient()
    {
        var httpClient = new HttpClient
        {
            BaseAddress = BaseAddress,
            Timeout = TimeSpan.FromSeconds(20),
        };

        httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        httpClient.DefaultRequestHeaders.Add(ApiVersionHeaderName, ApiVersionHeaderValue);
        return httpClient;
    }

    private static StringContent CreateRequestContent(string targetObjectId, CapCommandInput input)
    {
        using var stream = new MemoryStream();
        using (var writer = new Utf8JsonWriter(stream))
        {
            writer.WriteStartObject();
            writer.WriteString("id", targetObjectId);

            writer.WriteString("markdown", CapacitiesContentComposer.ComposeMarkdown(input));

            writer.WriteStartObject("position");
            writer.WriteString("type", "end");
            writer.WriteEndObject();
            writer.WriteEndObject();
        }

        return new StringContent(Encoding.UTF8.GetString(stream.ToArray()), Encoding.UTF8, "application/json");
    }

    private static StringContent CreateSearchRequestContent(string query, int limit, IReadOnlyList<string> structureIds)
    {
        using var stream = new MemoryStream();
        using (var writer = new Utf8JsonWriter(stream))
        {
            writer.WriteStartObject();
            writer.WriteString("query", query);
            if (structureIds.Count > 0)
            {
                writer.WriteStartArray("structureIds");
                foreach (var structureId in structureIds)
                {
                    writer.WriteStringValue(structureId);
                }

                writer.WriteEndArray();
            }

            writer.WriteNumber("limit", limit);
            writer.WriteEndObject();
        }

        return new StringContent(Encoding.UTF8.GetString(stream.ToArray()), Encoding.UTF8, "application/json");
    }

    private static StringContent CreateObjectRequestContent(string structureId, string objectName, string objectContent)
    {
        var markdown = ComposeCreateObjectMarkdown(objectName, objectContent);

        using var stream = new MemoryStream();
        using (var writer = new Utf8JsonWriter(stream))
        {
            writer.WriteStartObject();
            writer.WriteString("structureId", structureId);
            writer.WriteString("markdown", markdown);
            writer.WriteEndObject();
        }

        return new StringContent(Encoding.UTF8.GetString(stream.ToArray()), Encoding.UTF8, "application/json");
    }

    private static string ComposeCreateObjectMarkdown(string objectName, string objectContent)
    {
        var content = objectContent.Trim();
        if (string.IsNullOrWhiteSpace(content))
        {
            return $"# {objectName}";
        }

        return $"# {objectName}\n\n{content}";
    }

    private static CapacitiesStructureFilterResult GetSearchableStructureIds(string sanitizedToken)
    {
        var structureResult = GetSearchableStructures(sanitizedToken);
        if (!string.IsNullOrWhiteSpace(structureResult.Error))
        {
            return CapacitiesStructureFilterResult.Failure(structureResult.Error);
        }

        return CapacitiesStructureFilterResult.Success(_cachedStructureIds);
    }

    private static CapacitiesStructureSearchResult GetSearchableStructures(string sanitizedToken)
    {
        if (string.Equals(_cachedStructureToken, sanitizedToken, StringComparison.Ordinal) &&
            DateTimeOffset.UtcNow < _cachedStructureExpiry &&
            _cachedStructures.Count > 0)
        {
            return CapacitiesStructureSearchResult.Success(_cachedStructures);
        }

        using var request = new HttpRequestMessage(HttpMethod.Get, "space/structures");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", sanitizedToken);

        try
        {
            using var response = HttpClient.Send(request);
            var responseBody = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();

            if (!response.IsSuccessStatusCode)
            {
                return CapacitiesStructureSearchResult.Failure(MapErrorMessage(response.StatusCode, responseBody));
            }

            var structures = ParseSearchableStructures(responseBody);
            _cachedStructureToken = sanitizedToken;
            _cachedStructures = structures;

            var ids = new List<string>(structures.Count);
            foreach (var structure in structures)
            {
                ids.Add(structure.Id);
            }

            _cachedStructureIds = ids;
            _cachedStructureExpiry = DateTimeOffset.UtcNow.Add(StructureCacheLifetime);
            return CapacitiesStructureSearchResult.Success(structures);
        }
        catch (Exception ex)
        {
            return CapacitiesStructureSearchResult.Failure($"Could not load structures. {SummarizeException(ex)}");
        }
    }

    private static string GetDescriptionFromPropertyDefinitions(JsonElement structure)
    {
        // Currently the API does not provide structure descriptions.
        // When they add this field to the propertyDefinitions or elsewhere,
        // we can implement extraction logic here. For now, return empty to show
        // blank subtitles until the API is updated.
        return string.Empty;
    }

    private static IReadOnlyList<CapacitiesStructureMatch> ParseSearchableStructures(string responseBody)
    {
        var structuresList = new List<CapacitiesStructureMatch>();
        using var document = JsonDocument.Parse(responseBody);

        if (!document.RootElement.TryGetProperty("structures", out var structures) || structures.ValueKind != JsonValueKind.Array)
        {
            return structuresList;
        }

        foreach (var structure in structures.EnumerateArray())
        {
            var id = TryGetString(structure, "id");
            var title = TryGetString(structure, "title");
            var description = GetDescriptionFromPropertyDefinitions(structure);

            if (string.IsNullOrWhiteSpace(id))
            {
                continue;
            }

            if (DisallowedSearchStructureIds.Contains(id))
            {
                continue;
            }

            var normalizedTitle = string.IsNullOrWhiteSpace(title) ? id : title;
            if (string.Equals(id, "RootPage", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(normalizedTitle, "RootPage", StringComparison.OrdinalIgnoreCase))
            {
                normalizedTitle = "Page";
            }

            structuresList.Add(new CapacitiesStructureMatch(
                id,
                normalizedTitle,
                description ?? string.Empty));
        }

        return structuresList;
    }

    private static IReadOnlyList<CapacitiesObjectMatch> EnrichObjectsWithDetails(string apiToken, IReadOnlyList<CapacitiesObjectMatch> parsedObjects)
    {
        var enriched = new List<CapacitiesObjectMatch>(parsedObjects.Count);
        var fetchBudget = MaxObjectDetailsFetchPerSearch;

        foreach (var obj in parsedObjects)
        {
            var shouldFetch = NeedsObjectDetails(obj) &&
                              fetchBudget > 0 &&
                              !CachedObjectDetails.ContainsKey(obj.Id);

            CapacitiesObjectDetails details;
            if (shouldFetch)
            {
                fetchBudget--;
                details = GetObjectDetails(apiToken, obj.Id);
            }
            else
            {
                details = CachedObjectDetails.TryGetValue(obj.Id, out var cached)
                    ? cached
                    : CapacitiesObjectDetails.Empty;
            }

            var description = string.IsNullOrWhiteSpace(details.Description) ? obj.Description : details.Description;
            var structureId = string.IsNullOrWhiteSpace(details.StructureId) ? obj.StructureId : details.StructureId;
            var structureTitle = string.IsNullOrWhiteSpace(details.StructureTitle) ? obj.StructureTitle : details.StructureTitle;

            enriched.Add(obj with
            {
                Description = description,
                StructureId = structureId,
                StructureTitle = structureTitle,
            });
        }

        return enriched;
    }

    private static bool NeedsObjectDetails(CapacitiesObjectMatch obj)
    {
        return string.IsNullOrWhiteSpace(obj.Description) ||
               string.IsNullOrWhiteSpace(obj.StructureId) ||
               string.IsNullOrWhiteSpace(obj.StructureTitle);
    }

    private static CapacitiesObjectDetails GetObjectDetails(string apiToken, string objectId)
    {
        if (DateTimeOffset.UtcNow < _objectDetailsRateLimitedUntil)
        {
            return CapacitiesObjectDetails.Empty;
        }

        if (CachedObjectDetails.TryGetValue(objectId, out var cached))
        {
            return cached;
        }

        using var request = new HttpRequestMessage(HttpMethod.Get, $"object?id={Uri.EscapeDataString(objectId)}");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", apiToken);

        try
        {
            using var response = HttpClient.Send(request);
            var responseBody = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();

            if (response.StatusCode == (HttpStatusCode)429)
            {
                _objectDetailsRateLimitedUntil = ResolveRetryAfter(response.Headers, TimeSpan.FromSeconds(30));
                return CapacitiesObjectDetails.Empty;
            }

            if (!response.IsSuccessStatusCode)
            {
                var empty = CapacitiesObjectDetails.Empty;
                CachedObjectDetails[objectId] = empty;
                return empty;
            }

            var details = ParseObjectDetails(responseBody);
            CachedObjectDetails[objectId] = details;
            return details;
        }
        catch
        {
            var empty = CapacitiesObjectDetails.Empty;
            CachedObjectDetails[objectId] = empty;
            return empty;
        }
    }

    private static DateTimeOffset ResolveRetryAfter(HttpResponseHeaders headers, TimeSpan fallback)
    {
        if (headers.RetryAfter?.Delta is TimeSpan delta && delta > TimeSpan.Zero)
        {
            return DateTimeOffset.UtcNow.Add(delta);
        }

        if (headers.RetryAfter?.Date is DateTimeOffset date && date > DateTimeOffset.UtcNow)
        {
            return date;
        }

        return DateTimeOffset.UtcNow.Add(fallback);
    }

    private static CapacitiesObjectDetails ParseObjectDetails(string responseBody)
    {
        using var document = JsonDocument.Parse(responseBody);
        var root = document.RootElement;

        if (root.TryGetProperty("object", out var objectElement) && objectElement.ValueKind == JsonValueKind.Object)
        {
            root = objectElement;
        }

        var description = ParseDescription(root);
        var structureId = TryGetString(root, "structureId") ?? string.Empty;
        var structureTitle = TryGetString(root, "structureTitle") ?? string.Empty;

        if (string.IsNullOrWhiteSpace(structureTitle) &&
            root.TryGetProperty("structure", out var structure) &&
            structure.ValueKind == JsonValueKind.Object)
        {
            structureTitle = TryGetString(structure, "title") ?? string.Empty;
        }

        structureTitle = NormalizeStructureTitle(structureId, structureTitle);

        return new CapacitiesObjectDetails(description, structureId, structureTitle);
    }

    private static string ParseDescription(JsonElement root)
    {
        var description = TryGetString(root, "description");
        if (!string.IsNullOrWhiteSpace(description))
        {
            return description.Trim();
        }

        if (root.TryGetProperty("properties", out var properties) && properties.ValueKind == JsonValueKind.Object)
        {
            foreach (var property in properties.EnumerateObject())
            {
                if (!string.Equals(property.Name, "description", StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                var value = property.Value;
                description = TryGetString(value, "value");
                if (!string.IsNullOrWhiteSpace(description))
                {
                    return description.Trim();
                }

                if (value.TryGetProperty("text", out var textValue) && textValue.ValueKind == JsonValueKind.Object)
                {
                    description = TryGetString(textValue, "value");
                    if (!string.IsNullOrWhiteSpace(description))
                    {
                        return description.Trim();
                    }
                }
            }
        }

        return string.Empty;
    }

    private static string ParseObjectId(string responseBody)
    {
        using var document = JsonDocument.Parse(responseBody);

        var id = TryGetString(document.RootElement, "id");
        if (!string.IsNullOrWhiteSpace(id))
        {
            return id;
        }

        if (document.RootElement.TryGetProperty("object", out var objectElement) && objectElement.ValueKind == JsonValueKind.Object)
        {
            id = TryGetString(objectElement, "id");
            if (!string.IsNullOrWhiteSpace(id))
            {
                return id;
            }
        }

        return string.Empty;
    }

    private static string ParseSpaceId(string responseBody)
    {
        using var document = JsonDocument.Parse(responseBody);

        var id = TryGetString(document.RootElement, "id");
        if (!string.IsNullOrWhiteSpace(id))
        {
            return id;
        }

        id = TryGetString(document.RootElement, "spaceId");
        if (!string.IsNullOrWhiteSpace(id))
        {
            return id;
        }

        if (document.RootElement.TryGetProperty("space", out var spaceElement) && spaceElement.ValueKind == JsonValueKind.Object)
        {
            id = TryGetString(spaceElement, "id");
            if (!string.IsNullOrWhiteSpace(id))
            {
                return id;
            }

            id = TryGetString(spaceElement, "spaceId");
            if (!string.IsNullOrWhiteSpace(id))
            {
                return id;
            }
        }

        return string.Empty;
    }

    private static IReadOnlyList<CapacitiesObjectMatch> ParseObjects(string responseBody)
    {
        var matches = new List<CapacitiesObjectMatch>();
        using var document = JsonDocument.Parse(responseBody);

        if (TryCollectFromArrayProperty(document.RootElement, "objects", matches) ||
            TryCollectFromArrayProperty(document.RootElement, "results", matches) ||
            TryCollectFromArrayProperty(document.RootElement, "content", matches))
        {
            return matches;
        }

        if (document.RootElement.ValueKind == JsonValueKind.Array)
        {
            foreach (var item in document.RootElement.EnumerateArray())
            {
                TryAddMatch(item, matches);
            }
        }

        return matches;
    }

    private static bool TryCollectFromArrayProperty(JsonElement root, string propertyName, List<CapacitiesObjectMatch> matches)
    {
        if (!root.TryGetProperty(propertyName, out var array) || array.ValueKind != JsonValueKind.Array)
        {
            return false;
        }

        foreach (var item in array.EnumerateArray())
        {
            TryAddMatch(item, matches);
        }

        return true;
    }

    private static void TryAddMatch(JsonElement item, List<CapacitiesObjectMatch> matches)
    {
        var id = TryGetString(item, "id");
        if (string.IsNullOrWhiteSpace(id))
        {
            return;
        }

        var title = TryGetString(item, "title");
        if (string.IsNullOrWhiteSpace(title))
        {
            title = TryGetNestedTitle(item);
        }

        if (string.IsNullOrWhiteSpace(title))
        {
            title = "Untitled object";
        }

        var description = ParseDescription(item);
        var structureId = TryGetString(item, "structureId") ?? string.Empty;
        var structureTitle = TryGetString(item, "structureTitle") ?? string.Empty;

        if (string.IsNullOrWhiteSpace(structureTitle) &&
            item.TryGetProperty("structure", out var structure) &&
            structure.ValueKind == JsonValueKind.Object)
        {
            structureTitle = TryGetString(structure, "title") ?? string.Empty;
        }

        structureTitle = NormalizeStructureTitle(structureId, structureTitle);

        matches.Add(new CapacitiesObjectMatch(id, title, description, structureId, structureTitle));
    }

    private static string NormalizeStructureTitle(string structureId, string structureTitle)
    {
        if (string.Equals(structureId, "RootPage", StringComparison.OrdinalIgnoreCase) ||
            string.Equals(structureTitle, "RootPage", StringComparison.OrdinalIgnoreCase))
        {
            return "Page";
        }

        return structureTitle;
    }

    private static ObjectSearchHttpResult SendObjectSearchRequest(
        string sanitizedToken,
        string query,
        int limit,
        IReadOnlyList<string> structureIds)
    {
        using var request = new HttpRequestMessage(HttpMethod.Post, "objects/search")
        {
            Content = CreateSearchRequestContent(query, limit, structureIds),
        };
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", sanitizedToken);

        try
        {
            using var response = HttpClient.Send(request);
            var responseBody = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();

            if (!response.IsSuccessStatusCode)
            {
                return ObjectSearchHttpResult.Failure(response.StatusCode, MapErrorMessage(response.StatusCode, responseBody));
            }

            return ObjectSearchHttpResult.Success(responseBody);
        }
        catch (Exception ex)
        {
            return ObjectSearchHttpResult.Failure(null, $"Could not search objects. {SummarizeException(ex)}");
        }
    }

    private static bool IsTransientServerError(HttpStatusCode? statusCode)
    {
        if (statusCode is null)
        {
            return false;
        }

        var status = (int)statusCode.Value;
        return status >= 500 && status <= 599;
    }

    private static string? TryGetNestedTitle(JsonElement item)
    {
        if (!item.TryGetProperty("properties", out var properties) || properties.ValueKind != JsonValueKind.Object)
        {
            return null;
        }

        foreach (var property in properties.EnumerateObject())
        {
            if (!string.Equals(property.Name, "title", StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            var titleProperty = property.Value;
            if (!titleProperty.TryGetProperty("title", out var titleContent) || titleContent.ValueKind != JsonValueKind.Object)
            {
                continue;
            }

            return TryGetString(titleContent, "value");
        }

        return null;
    }

    private static string? TryGetString(JsonElement element, string propertyName)
    {
        if (!element.TryGetProperty(propertyName, out var value) || value.ValueKind != JsonValueKind.String)
        {
            return null;
        }

        return value.GetString();
    }

    private static string MapErrorMessage(HttpStatusCode statusCode, string responseBody)
    {
        var details = ExtractDetails(responseBody);

        return statusCode switch
        {
            HttpStatusCode.BadRequest => $"Capacities rejected the request. {details}",
            HttpStatusCode.Unauthorized or HttpStatusCode.Forbidden => "Capacities rejected the API token. Check the token in extension settings.",
            HttpStatusCode.NotFound => "Capacities could not find the target object. Check the configured object ID.",
            HttpStatusCode.UnprocessableEntity => $"Capacities could not process the content. {details}",
            _ => $"Capacities returned {(int)statusCode} {statusCode}. {details}",
        };
    }

    private static string ExtractDetails(string responseBody)
    {
        if (string.IsNullOrWhiteSpace(responseBody))
        {
            return "No additional details were returned.";
        }

        var singleLine = responseBody.Replace('\r', ' ').Replace('\n', ' ').Trim();
        return singleLine.Length <= 240 ? singleLine : singleLine[..240] + "...";
    }

    private static string SummarizeException(Exception ex)
    {
        var message = ex.Message.Trim();
        var innerMessage = ex.InnerException?.Message?.Trim();

        if (!string.IsNullOrWhiteSpace(innerMessage) && !string.Equals(message, innerMessage, StringComparison.Ordinal))
        {
            return $"{message} Inner: {innerMessage}";
        }

        return string.IsNullOrWhiteSpace(message) ? ex.GetType().Name : message;
    }

    private static bool ContainsNonAscii(string value)
    {
        foreach (var c in value)
        {
            if (c > 127)
            {
                return true;
            }
        }

        return false;
    }

    private static string SanitizeApiToken(string value)
    {
        var normalized = value.Normalize(NormalizationForm.FormKC);
        var builder = new StringBuilder(normalized.Length);

        foreach (var c in normalized)
        {
            if (char.IsWhiteSpace(c) || char.IsControl(c) || char.GetUnicodeCategory(c) == UnicodeCategory.Format)
            {
                continue;
            }

            builder.Append(c);
        }

        return builder.ToString();
    }

    private sealed record ObjectSearchHttpResult(bool Succeeded, string ResponseBody, HttpStatusCode? StatusCode, string Error)
    {
        public static ObjectSearchHttpResult Success(string responseBody) => new(true, responseBody, null, string.Empty);

        public static ObjectSearchHttpResult Failure(HttpStatusCode? statusCode, string error) =>
            new(false, string.Empty, statusCode, error);
    }
}

internal sealed record CapacitiesWriteResult(bool Succeeded, string Message)
{
    public static CapacitiesWriteResult Success(string message) => new(true, message);

    public static CapacitiesWriteResult Failure(string message) => new(false, message);
}

internal sealed record CapacitiesObjectMatch(
    string Id,
    string Title,
    string Description,
    string StructureId,
    string StructureTitle);

internal sealed record CapacitiesObjectSearchResult(IReadOnlyList<CapacitiesObjectMatch> Objects, string? Error)
{
    public static CapacitiesObjectSearchResult Success(IReadOnlyList<CapacitiesObjectMatch> objects) => new(objects, null);

    public static CapacitiesObjectSearchResult Failure(string error) => new([], error);
}

internal sealed record CapacitiesStructureFilterResult(IReadOnlyList<string> StructureIds, string? Error)
{
    public static CapacitiesStructureFilterResult Success(IReadOnlyList<string> structureIds) => new(structureIds, null);

    public static CapacitiesStructureFilterResult Failure(string error) => new([], error);
}

internal sealed record CapacitiesStructureMatch(string Id, string Title, string Description);

internal sealed record CapacitiesObjectDetails(string Description, string StructureId, string StructureTitle)
{
    public static CapacitiesObjectDetails Empty => new(string.Empty, string.Empty, string.Empty);
}

internal sealed record CapacitiesStructureSearchResult(IReadOnlyList<CapacitiesStructureMatch> Structures, string? Error)
{
    public static CapacitiesStructureSearchResult Success(IReadOnlyList<CapacitiesStructureMatch> structures) => new(structures, null);

    public static CapacitiesStructureSearchResult Failure(string error) => new([], error);
}

internal sealed record CapacitiesCreateObjectResult(bool Succeeded, string ObjectId, string ObjectTitle, string Message)
{
    public static CapacitiesCreateObjectResult Success(string objectId, string objectTitle)
        => new(true, objectId, objectTitle, "Object created.");

    public static CapacitiesCreateObjectResult Failure(string message)
        => new(false, string.Empty, string.Empty, message);
}

internal sealed record CapacitiesSpaceResult(bool Succeeded, string SpaceId, string Message)
{
    public static CapacitiesSpaceResult Success(string spaceId) => new(true, spaceId, string.Empty);

    public static CapacitiesSpaceResult Failure(string message) => new(false, string.Empty, message);
}