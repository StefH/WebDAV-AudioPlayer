
namespace WebDav.AudioPlayer.Client
{
    public enum ResourceLoadStatus
    {
        Unknown,
        Ok,
        StreamExisting,
        StreamFailedToLoad,
        IsCollection,
        NoResourcesFound,
        OperationCanceled,
        Unauthorized
    }
}