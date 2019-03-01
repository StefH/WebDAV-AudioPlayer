namespace WebDav.AudioPlayer.Audio
{
    public class MediaDetails
    {
        public string Mode { get; set; }
        public int BitrateKbps { get; set; }

        public int SampleRate { get; set; }

        public int Channels { get; set; }

        public double DurationMs { get; set; }

        public bool IsLossless { get; set; }
    }
}
