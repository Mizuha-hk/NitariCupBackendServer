using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using NitariCupBackend.Models;
using NitariCupBackend.Server.Data;
using NitariCupBackend.Library;
using Microsoft.CodeAnalysis.CSharp;

namespace NitariCupBackend.Server.EndPoints;

public static class TaskEndpoints
{
    public static void MapTaskSchemeEndpoints (this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/api/TaskScheme").WithTags(nameof(TaskScheme));

        group.MapGet("/Notification", async Task<Results<Ok<List<TaskScheme>>, NotFound>> (NitariCupBackendServerContext db) =>
        {
            var now = DateTime.Now;
            var tasks = await db.TaskScheme
                .OrderBy(model => model.startDate)
                .Where(model => model.isDone == false)
                .ToListAsync();        

            var response = tasks
                .Where(model => (model.startDate - now).Hours <= 2 && (model.startDate - now).Hours >= 0)
                .ToList();

            return response == null
                ? TypedResults.NotFound()
                : TypedResults.Ok(response);

        })
        .WithName("GetRecentTasks")
        .WithOpenApi();

        group.MapGet("/Id={id}", async Task<Results<Ok<TaskScheme>, NotFound>> (Guid id, NitariCupBackendServerContext db) =>
        {
            return await db.TaskScheme.AsNoTracking()
                .FirstOrDefaultAsync(model => model.Id == id)
                is TaskScheme model
                    ? TypedResults.Ok(model)
                    : TypedResults.NotFound();
        })
        .WithName("GetTaskSchemeById")
        .WithOpenApi();

        group.MapGet("/Done/Id={id}", async Task<Results<Ok<TaskResponseModel>, NotFound>> (Guid id, NitariCupBackendServerContext db) =>
        {
            var task = await db.TaskScheme.AsNoTracking()
                .FirstOrDefaultAsync(model => model.Id == id);
            if(task == null)
            {
                return TypedResults.NotFound();
            }
            var now = DateTime.Now;
            var score = (float)ScoreCalculater.ScoreCalc(task.startDate, task.limitDate, now);

            var affected = await db.TaskScheme
                .Where(model => model.Id == id)
                .ExecuteUpdateAsync(setters => setters
                    .SetProperty(model => model.isDone, true)
                    .SetProperty(model => model.DoneDate, now)
                    .SetProperty(model => model.score, score)
                );

            return affected == 1 ? TypedResults.Ok(new TaskResponseModel()
            {
                Id = task.Id,
                title = task.title,
                description = task.description,
                startDate = task.startDate,
                limitDate = task.limitDate,
                createdAt = task.createdAt,
                isDone = true,
                DoneDate = now,
                score = score
            }) 
            : TypedResults.NotFound();
        })
        .WithName("DoneTask")
        .WithOpenApi();

        group.MapGet("/NotDone/AT={AccessToken},index={index}", async Task<Results<Ok<List<TaskResponseModel>>, NotFound>> (string AccessToken, int index, NitariCupBackendServerContext db) =>
        {
            var profile = await LineAuth.GetProfile(AccessToken);
            Task.WaitAll();

            var tasks = await db.TaskScheme.AsNoTracking()
                .OrderBy(model => model.startDate)
                .Where(model => model.userId == profile.userId)
                .Where(model => model.isDone == false)
                .Skip(index * 10)
                .Take(10)
                .ToListAsync();

            var response = new List<TaskResponseModel>();

            foreach (var itme in tasks)
            {
                response.Add(new TaskResponseModel
                {
                    Id = itme.Id,
                    title = itme.title,
                    description = itme.description,
                    startDate = itme.startDate,
                    limitDate = itme.limitDate,
                    createdAt = itme.createdAt,
                    isDone = itme.isDone,
                    DoneDate = itme.DoneDate,
                    score = itme.score
                });
            }

            return response.Count() > 0
                ? TypedResults.Ok(response)
                : TypedResults.NotFound();
        })
        .WithName("GetUncompleted10Tasks")
        .WithOpenApi();

        group.MapGet("/IsDone/AT={AccessToken},index={index}", async Task<Results<Ok<List<TaskResponseModel>>, NotFound>> (string AccessToken, int index, NitariCupBackendServerContext db) =>
        {
            var profile = await LineAuth.GetProfile(AccessToken);
            Task.WaitAll();

            var tasks = await db.TaskScheme.AsNoTracking()
                .OrderBy(model => model.startDate)
                .Where(model => model.userId == profile.userId)
                .Where(model => model.isDone == true)
                .Skip(index * 10)
                .Take(10)
                .ToListAsync();

            var response = new List<TaskResponseModel>();

            foreach(var itme in tasks)
            {
                response.Add(new TaskResponseModel
                {
                    Id = itme.Id,
                    title = itme.title,
                    description = itme.description,
                    startDate = itme.startDate,
                    limitDate = itme.limitDate,
                    createdAt = itme.createdAt,
                    isDone = itme.isDone,
                    DoneDate = itme.DoneDate,
                    score = itme.score
                });
            }

            return response.Count() > 0
                ? TypedResults.Ok(response)
                : TypedResults.NotFound();
        })
        .WithName("GetCompleted10Tasks")
        .WithOpenApi();

        group.MapPut("/Id={id}", async Task<Results<Ok, NotFound>> (Guid id, TaskPutReqModel req, NitariCupBackendServerContext db) =>
        {
            var affected = await db.TaskScheme
                .Where(model => model.Id == id)
                .ExecuteUpdateAsync(setters => setters
                  .SetProperty(m => m.title, req.title)
                  .SetProperty(m => m.description, req.description)
                  .SetProperty(m => m.startDate, req.startDate)
                  .SetProperty(m => m.limitDate, req.limitDate)
                );

            return affected == 1 ? TypedResults.Ok() : TypedResults.NotFound();
        })
        .WithName("UpdateTask")
        .WithOpenApi();

        group.MapPost("/", async Task<Results<Ok, BadRequest<string>>> (TaskPostReqModel req, NitariCupBackendServerContext db) =>
        {
            var Id = Guid.NewGuid();
            var profile = await LineAuth.GetProfile(req.AccessToken);

            if(profile.displayName == "Unknown")
            {
                return TypedResults.BadRequest("AccessToken is invalid");
            }

            var taskScheme = new TaskScheme
            {
                Id = Id,
                userId = profile.userId,
                title = req.title,
                description = req.description,
                startDate = req.StartDate,
                limitDate = req.LimitDate,
                createdAt = req.CreatedAt,
                isDone = false,
            };

            db.TaskScheme.Add(taskScheme);
            await db.SaveChangesAsync();
            return TypedResults.Ok();
        })
        .WithName("CreateTask")
        .WithOpenApi();

        group.MapPost("/Sample", async Task<Results<Ok, BadRequest<string>>> (TaskPostReqModel req, NitariCupBackendServerContext db) =>
        {
            var Id = Guid.NewGuid();

            var taskScheme = new TaskScheme
            {
                Id = Id,
                userId = req.AccessToken,
                title = req.title,
                description = req.description,
                startDate = req.StartDate,
                limitDate = req.LimitDate,
                createdAt = req.CreatedAt,
                isDone = false,
            };

            db.TaskScheme.Add(taskScheme);
            await db.SaveChangesAsync();
            return TypedResults.Ok();
        })
        .WithName("sampleCreate")
        .WithOpenApi();

        group.MapDelete("/Id={id}", async Task<Results<Ok, NotFound>> (Guid id, NitariCupBackendServerContext db) =>
        {
            var affected = await db.TaskScheme
                .Where(model => model.Id == id)
                .ExecuteDeleteAsync();

            return affected == 1 ? TypedResults.Ok() : TypedResults.NotFound();
        })
        .WithName("DeleteTask")
        .WithOpenApi();
    }
}
