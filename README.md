# WebDAV-AudioPlayer
A simple AudioPlayer to play audio files from a WebDAV location.

The following codecs are supported:
* MP3
* WAV
* FLAC
* AAC
* AC3
* WMA
* OGG-Vorbis
* OPUS

## Blazor UI screenshot

![Blazor-WebDAV-AudioPlayer](https://raw.githubusercontent.com/StefH/WebDAV-AudioPlayer/master/resources/blazor-webdav-audioplayer.png "Blazor-WebDAV-AudioPlayer")

## Windows UI screenshot

![WebDAV-AudioPlayer](https://raw.githubusercontent.com/StefH/WebDAV-AudioPlayer/master/resources/WebDAV-AudioPlayer.png "WebDAV-AudioPlayer")

Used libraries:
* [CSCore](https://github.com/filoe/cscore) for playing audio files.
* [CSCore.Ogg](https://github.com/StefH/WebDAV-AudioPlayer) for playing OGG-Vorbis audio files.
* [CSCore.Opus](https://github.com/StefH/WebDAV-AudioPlayer) for playing Opus audio files.
* [WebDAV-Client](https://github.com/StefH/WebDAV-Client) for accessing the WebDAV location.
* [Audio Tools Library (ATL) for .NET](https://github.com/Zeugma440/atldotnet) the C# port of the original ATL.
* [ByteSize](https://github.com/omar/ByteSize) A utility class that makes byte size representation easy.

Used code:
* [concentus](https://github.com/lostromb/concentus) Pure Portable C# and Java implementations of the Opus audio codec
* [concentus.oggfile](https://github.com/lostromb/concentus.oggfile) Implementing support for reading/writing .opus audio files using Concentus
* [ByteQueue](https://github.com/Kelindar/circular-buffer/blob/master/Source/ByteQueue.cs) An efficient implementation of a resizeable circular byte buffer in C#
* [CSCore with NVorbis](https://github.com/filoe/cscore/blob/master/Samples/NVorbisIntegration/Program.cs) NVorbisIntegration sample
