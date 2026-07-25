namespace WebDav.AudioPlayer.Audio;

public class MediaDetails
{
    public required string Mode { get; set; }

    public required string Album { get; set; }

    public required string Artist { get; set; }

    public required string Title { get; set; }

    public int BitrateKbps { get; set; }

    public int SampleRate { get; set; }

    public int Channels { get; set; }

    public double DurationMs { get; set; }

    public bool IsLossless { get; set; }
}
