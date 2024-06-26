﻿namespace DiscordRepair.Api.MigrationMaster.Models;

internal enum ResponseTypes
{
    MissingPermissions,
    Ratelimit,
    Banned,
    InvalidAuthToken,
    TooManyGuilds,
    TooManyRequests,
    GenericError,
    GenericErrorRetryAttempt,
    NewJsonError,
    Success
}
