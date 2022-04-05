using backend.Requests;
using backend.Stores;

namespace backend.Endpoints;

public static partial class Endpoints
{
    public static async Task<IResult> NewHunt(NewHunt hunt, HuntStore huntStore, GameStore gameStore)
    {
        if (hunt.Game is null || hunt.Type is null)
        {
            return Results.BadRequest("Hunt must have a game and type");
        }

        var gameIdTask = gameStore.GetId(hunt.Game);
        var typeIdTask = huntStore.GetHuntTypeId(hunt.Type);

        return await typeIdTask is null
            ? Results.BadRequest("Type does not exist.")
            : await gameIdTask is null
                ? Results.BadRequest("Game does not exist.")
                : Results.Ok(await huntStore.CreateHunt((await gameIdTask).Value, (await typeIdTask).Value, hunt.Target));
    }

}
