using System.Net;
using System.Net.Http.Json;
using AutomationExecution.API.Dtos;
using AutomationExecution.API.Models;
using Microsoft.AspNetCore.Mvc.Testing;

namespace AutomationExecution.API.Tests;

public class ExecutionsApiTests
    : IClassFixture<WebApplicationFactory<Program>>
{
    private const string ExecutionsEndpoint = "/api/Executions";
    private readonly HttpClient _client;

    public ExecutionsApiTests(WebApplicationFactory<Program> factory)
    {
        // Creates a client connected to the API running in memory.
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetAll_ReturnsOkAndContainsExecutions()
    {
        // Arrange: create data owned by this test.
        var createdExecution = await CreateExecutionAsync();

        try
        {
            // Act
            var response = await _client.GetAsync(ExecutionsEndpoint);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var executions =
                await response.Content.ReadFromJsonAsync<List<TestExecution>>();

            Assert.NotNull(executions);
            Assert.Contains(
                executions,
                execution => execution.Id == createdExecution.Id);
        }
        finally
        {
            // Cleanup runs even when an assertion fails.
            await DeleteExecutionAsync(createdExecution.Id);
        }
    }

    [Fact]
    public async Task GetById_ExistingExecution_ReturnsExecution()
    {
        // Arrange
        var createdExecution = await CreateExecutionAsync();

        try
        {
            // Act
            var response = await _client.GetAsync(
                $"{ExecutionsEndpoint}/{createdExecution.Id}");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var returnedExecution =
                await response.Content.ReadFromJsonAsync<TestExecution>();

            Assert.NotNull(returnedExecution);
            Assert.Equal(createdExecution.Id, returnedExecution.Id);
            Assert.Equal(
                createdExecution.TestSuite,
                returnedExecution.TestSuite);
        }
        finally
        {
            await DeleteExecutionAsync(createdExecution.Id);
        }
    }

    [Fact]
    public async Task GetById_UnknownExecution_ReturnsNotFound()
    {
        // Act
        var response = await _client.GetAsync(
            $"{ExecutionsEndpoint}/{int.MaxValue}");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task CreateExecution_ValidRequest_ReturnsCreatedExecution()
    {
        // Arrange
        var request = CreateValidRequest();

        // Act
        var response = await _client.PostAsJsonAsync(
            ExecutionsEndpoint,
            request);

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var createdExecution =
            await response.Content.ReadFromJsonAsync<TestExecution>();

        Assert.NotNull(createdExecution);

        try
        {
            Assert.True(createdExecution.Id > 0);
            Assert.Equal(request.TestSuite, createdExecution.TestSuite);
            Assert.Equal(request.Environment, createdExecution.Environment);
            Assert.Equal(request.Status, createdExecution.Status);

            // POST should identify the URL of the created resource.
            Assert.NotNull(response.Headers.Location);
            Assert.EndsWith(
                $"{ExecutionsEndpoint}/{createdExecution.Id}",
                response.Headers.Location.ToString());
        }
        finally
        {
            await DeleteExecutionAsync(createdExecution.Id);
        }
    }

    [Fact]
    public async Task CreateExecution_InvalidRequest_ReturnsBadRequest()
    {
        // Arrange: required string properties are empty.
        var invalidRequest = new CreateTestExecutionRequest
        {
            TestSuite = "",
            Environment = "",
            Status = ""
        };

        // Act
        var response = await _client.PostAsJsonAsync(
            ExecutionsEndpoint,
            invalidRequest);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task UpdateExecution_ExistingExecution_ReturnsUpdatedExecution()
    {
        // Arrange: create an isolated record to update.
        var createdExecution = await CreateExecutionAsync();

        try
        {
            var request = new UpdateTestExecutionRequest
            {
                TestSuite = "Updated Test Suite",
                Environment = "TEST",
                Status = "Completed",
                StartedAt = createdExecution.StartedAt,
                CompletedAt = DateTime.UtcNow
            };

            // Act
            var response = await _client.PutAsJsonAsync(
                $"{ExecutionsEndpoint}/{createdExecution.Id}",
                request);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var updatedExecution =
                await response.Content.ReadFromJsonAsync<TestExecution>();

            Assert.NotNull(updatedExecution);
            Assert.Equal(createdExecution.Id, updatedExecution.Id);
            Assert.Equal(request.TestSuite, updatedExecution.TestSuite);
            Assert.Equal(request.Environment, updatedExecution.Environment);
            Assert.Equal(request.Status, updatedExecution.Status);
            Assert.Equal(request.StartedAt, updatedExecution.StartedAt);
            Assert.Equal(request.CompletedAt, updatedExecution.CompletedAt);
        }
        finally
        {
            await DeleteExecutionAsync(createdExecution.Id);
        }
    }

    [Fact]
    public async Task UpdateExecution_UnknownExecution_ReturnsNotFound()
    {
        // Arrange
        var request = new UpdateTestExecutionRequest
        {
            TestSuite = "Unknown Test Suite",
            Environment = "TEST",
            Status = "Completed",
            StartedAt = DateTime.UtcNow,
            CompletedAt = DateTime.UtcNow
        };

        // Act
        var response = await _client.PutAsJsonAsync(
            $"{ExecutionsEndpoint}/{int.MaxValue}",
            request);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task DeleteExecution_ExistingExecution_ReturnsNoContent()
    {
        // Arrange
        var createdExecution = await CreateExecutionAsync();

        // Act
        var deleteResponse = await _client.DeleteAsync(
            $"{ExecutionsEndpoint}/{createdExecution.Id}");

        // Assert
        Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);

        // Confirm that the deleted resource can no longer be retrieved.
        var getResponse = await _client.GetAsync(
            $"{ExecutionsEndpoint}/{createdExecution.Id}");

        Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);
    }

    [Fact]
    public async Task DeleteExecution_UnknownExecution_ReturnsNotFound()
    {
        // Act
        var response = await _client.DeleteAsync(
            $"{ExecutionsEndpoint}/{int.MaxValue}");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    private static CreateTestExecutionRequest CreateValidRequest()
    {
        // A unique name prevents tests from depending on shared names.
        return new CreateTestExecutionRequest
        {
            TestSuite = $"Integration Test {Guid.NewGuid():N}",
            Environment = "TEST",
            Status = "Queued",
            StartedAt = DateTime.UtcNow,
            CompletedAt = null
        };
    }

    private async Task<TestExecution> CreateExecutionAsync()
    {
        // Creates isolated setup data for tests that need an existing record.
        var response = await _client.PostAsJsonAsync(
            ExecutionsEndpoint,
            CreateValidRequest());

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var execution =
            await response.Content.ReadFromJsonAsync<TestExecution>();

        Assert.NotNull(execution);

        return execution;
    }

    private async Task DeleteExecutionAsync(int id)
    {
        // Removes setup data created by the test.
        await _client.DeleteAsync($"{ExecutionsEndpoint}/{id}");
    }
}