namespace SearchService.Api.Clients;

public record SearchDocument<TDocument>(TDocument Document, double? Score);

public record SearchResult<TDocument>(long Total, IReadOnlyCollection<SearchDocument<TDocument>> Documents);
