# Assignment 9 - Feedback and Status

## Status

All required tasks for Assignment 9 have been completed:

### 1. Solution Setup
- The Presentation layer folder has been created under `src/Presentation`
- A Web API project (`ViaEventAssociation.Presentation.WebApi`) has been created with proper project references
- `Program.cs` has been cleaned up: weather forecast code removed, `AddControllers()` and `MapControllers()` added
- All extension methods for DI registration are in place across projects

### 2. Object Mapper (Core/Tools/ObjectMapper)
- `IObjectMapper` interface with generic `Map<TOutput>` method
- `IMappingConfig<TInput, TOutput>` interface for custom mapping rules
- `ObjectMapper` implementation with JSON default fallback and custom config support via DI
- `ObjectMapperServiceExtensions` for automatic discovery and registration of mapping configs
- Unit tests covering: default mapping, specific mapping config, complex object graphs, and collection mapping

### 3. REPR Fluent Base Class
- `ApiEndpoint` class in `Endpoints/Common/ApiEndpoint.cs`
- Supports `WithRequest<TRequest>` and `WithoutRequest` patterns
- Supports `AndResult<T>`, `AndResults<T1, T2>`, and `AndResults<T1, T2, T3>` return types
- `EndpointBase` abstract class with `[ApiController, Route("api")]` attributes

### 4. Update Write Context
- `DmContext` already uses `DbContextOptions<DmContext>` (generic type argument) to support two registered DbContexts

### 5. Web API Endpoints (Command Side)
All command endpoints implemented:
- `CreateEventEndpoint` (POST)
- `UpdateEventTitleEndpoint` (PATCH)
- `UpdateEventDescriptionEndpoint` (PATCH)
- `SetMaxNumberOfGuestsEndpoint` (PATCH)
- `MakeEventPublicEndpoint` (PATCH)
- `MakeEventPrivateEndpoint` (PATCH)
- `UpdateEventTimeIntervalEndpoint` (PATCH)

### 6. Web API Query Endpoints
All query endpoints implemented with object mapper integration:
- `BrowseUpcomingEventsEndpoint` (GET) with mapping configs
- `ViewSingleEventEndpoint` (GET) with mapping configs
- `EventsCalendarOverviewEndpoint` (GET) with mapping configs
- `EventsEditingOverviewEndpoint` (GET) with mapping configs

### 7. Testing

#### Unit Tests (Endpoint behavior)
Tests per endpoint covering: success, failure input, business logic failure (NotFound), and exception scenarios:
- `CreateEventEndpointTests` (3 tests)
- `UpdateEventTitleEndpointTests` (3 tests)
- `UpdateEventDescriptionEndpointTests` (4 tests)
- `SetMaxNumberOfGuestsEndpointTests` (4 tests)
- `MakeEventPublicEndpointTests` (4 tests)
- `MakeEventPrivateEndpointTests` (4 tests)
- `UpdateEventTimeIntervalEndpointTests` (4 tests)
- `BrowseUpcomingEventsEndpointTests` (3 tests)
- `ViewSingleEventEndpointTests` (3 tests)
- `EventsCalendarOverviewEndpointTests` (3 tests)
- `EventsEditingOverviewEndpointTests` (3 tests)

#### Unit Tests (Object Mapper)
- `ObjectMapperTests` (4 tests: default mapping, specific config, complex graphs, collection mapping)

#### Integration Tests
- Full end-to-end tests using `WebApplicationFactory<Program>` with SQLite in-memory database
- Tests cover: success paths, missing input, validation failures, not found scenarios, and invalid GUID handling

## Feedback on the Project

Overall, the assignment was well-structured and clearly communicated the expectations for the presentation layer.

### Positive aspects
- The REPR pattern is a clean alternative to traditional controllers, making endpoints focused and testable
- The fluent base class (`ApiEndpoint.WithRequest<T>.AndResults<...>`) makes the intent of each endpoint immediately clear
- The object mapper abstraction with JSON fallback is practical and easy to test

### Suggestions
- It would be helpful to have a more detailed example of how the swagger page grouping should work with the REPR pattern
- The relationship between the DmContext and QueryContext could be explained earlier in the assignments, since the generic `DbContextOptions<T>` requirement only becomes apparent when both are registered
