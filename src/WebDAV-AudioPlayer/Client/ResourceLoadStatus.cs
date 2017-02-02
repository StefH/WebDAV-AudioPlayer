
namespace WebDav.AudioPlayer.Client
{
    public enum ResourceLoadStatus
    {
        Unknown,
        StreamLoaded,
        StreamExisting,
        StreamFailedToLoad,
        IsCollection,
        FolderDownloaded,
        NoFilesFoundInFolder,
        OperationCanceled
    }
}