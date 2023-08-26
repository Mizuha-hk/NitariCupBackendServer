using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using NitariCupBackend.Models;
using NitariCupBackend.Server.Data;
namespace NitariCupBackend.Server.EndPoints;

public static class TaskEndpoints
{
    public static void MapTaskSchemeEndpoints (this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/api/TaskScheme").WithTags(nameof(TaskScheme));

        group.MapGet("/", async (NitariCupBackendServerContext db) =>
        {
            return await db.TaskScheme.ToListAsync();
        })
        .WithName("GetAllTaskSchemes")
        .WithOpenApi();

        group.MapGet("/{id}", async Task<Results<Ok<TaskScheme>, NotFound>> (Guid id, NitariCupBackendServerContext db) =>
        {
            return await db.TaskScheme.AsNoTracking()
                .FirstOrDefaultAsync(model => model.Id == id)
                is TaskScheme model
                    ? TypedResults.Ok(model)
                    : TypedResults.NotFound();
        })
        .WithName("GetTaskSchemeById")
        .WithOpenApi();

        group.MapPut("/{id}", async Task<Results<Ok, NotFound>> (Guid id, TaskScheme taskScheme, NitariCupBackendServerContext db) =>
        {
            var affected = await db.TaskScheme
                .Where(model => model.Id == id)
                .ExecuteUpdateAsync(setters => setters
                  .SetProperty(m => m.Id, taskScheme.Id)
                  .SetProperty(m => m.userId, taskScheme.userId)
                  .SetProperty(m => m.title, taskScheme.title)
                  .SetProperty(m => m.description, taskScheme.description)
                  .SetProperty(m => m.startDate, taskScheme.startDate)
                  .SetProperty(m => m.limitDate, taskScheme.limitDate)
                  .SetProperty(m => m.createdAt, taskScheme.createdAt)
                  .SetProperty(m => m.isDone, taskScheme.isDone)
                  .SetProperty(m => m.DoneDate, taskScheme.DoneDate)
                  .SetProperty(m => m.score, taskScheme.score)
                );

            return affected == 1 ? TypedResults.Ok() : TypedResults.NotFound();
        })
        .WithName("UpdateTaskScheme")
        .WithOpenApi();

        group.MapPost("/", async (TaskScheme taskScheme, NitariCupBackendServerContext db) =>
        {
            db.TaskScheme.Add(taskScheme);
            await db.SaveChangesAsync();
            return TypedResults.Created($"/api/TaskScheme/{taskScheme.Id}",taskScheme);
        })
        .WithName("CreateTaskScheme")
        .WithOpenApi();

        group.MapDelete("/{id}", async Task<Results<Ok, NotFound>> (Guid id, NitariCupBackendServerContext db) =>
        {
            var affected = await db.TaskScheme
                .Where(model => model.Id == id)
                .ExecuteDeleteAsync();

            return affected == 1 ? TypedResults.Ok() : TypedResults.NotFound();
        })
        .WithName("DeleteTaskScheme")
        .WithOpenApi();
    }
}
