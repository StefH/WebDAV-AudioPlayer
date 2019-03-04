using ATL;
using CSCore;
using CSCore.Codecs.OPUS;
using CSCore.SoundOut;
using System;
using System.IO;
using System.Threading.Tasks;

namespace ConsoleAppPlayer
{
    class Program
    {
        static void Main(string[] args)
        {
            RunAsync().GetAwaiter().GetResult();
        }

        private static async Task RunAsync()
        {
            ISoundOut soundOut;
            if (WasapiOut.IsSupportedOnCurrentPlatform)
            {
                soundOut = new WasapiOut();
            }
            else
            {
                soundOut = new DirectSoundOut();
            }

            var stream = File.OpenRead(@"c:\temp\Abandoned _ Chill Mix.opus");
            var track = new Track(stream, ".opus");

            var waveSource = new OpusSource(stream, (int)track.SampleRate, 2);

            Console.WriteLine("len = {0} {1}", waveSource.Length, waveSource.GetLength());

            soundOut.Initialize(waveSource);
            soundOut.Play();

            //while (waveSource.Position < waveSource.Length)
            //{
            //    await Task.Delay(TimeSpan.FromSeconds(1));
            //    Console.WriteLine("GetPosition = {0} - {1}", waveSource.GetPosition(), DateTime.Now.TimeOfDay);
            //}

            await Task.Delay(TimeSpan.FromSeconds(3));

            Console.WriteLine("GetPosition = {0}", waveSource.GetPosition());

            soundOut.Pause();

            double x = 0.994;
            long pos = Convert.ToInt64(x * track.SampleRate * track.DurationMs / 1000);
            Console.WriteLine("pos = {0}", pos);

            waveSource.SetPosition(TimeSpan.FromMinutes(58));

            soundOut.Play();
            while (waveSource.Position < waveSource.Length)
            {
                await Task.Delay(TimeSpan.FromSeconds(1));
                Console.WriteLine("GetPosition = {0} {1}", waveSource.GetPosition(), waveSource.Position);
            }

            Console.WriteLine("*** GetPosition = {0} {1}", waveSource.GetPosition(), waveSource.Position);
        }
    }
}