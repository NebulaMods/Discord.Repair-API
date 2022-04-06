namespace RestoreCord.Records.Discord;

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
