# WebDAV-AudioPlayer
A simple AudioPlayer to play audio files from a WebDAV location.

The following codecs are supported:
* MP3
* WAVE
* FLAC
* AAC
* AC3
* WMA
* OGG-Vorbis
* OPUS

Used code & libraries:
* [CSCore](https://github.com/filoe/cscore) for playing audio files.
* [WebDAV-Client](https://github.com/StefH/WebDAV-Client) for accessing the WebDAV location.
* [Audio Tools Library (ATL) for .NET](https://github.com/Zeugma440/atldotnet) the C# port of the original ATL.
* [concentus](https://github.com/lostromb/concentus) Pure Portable C# and Java implementations of the Opus audio codec
* [concentus.oggfile](https://github.com/lostromb/concentus.oggfile) Implementing support for reading/writing .opus audio files using Concentus
* [ByteQueue](https://github.com/Kelindar/circular-buffer/blob/master/Source/ByteQueue.cs) An efficient implementation of a resizeable circular byte buffer in C#

<br>
<br>
Screenshot:

![WebDAV-AudioPlayer](https://raw.githubusercontent.com/StefH/WebDAV-AudioPlayer/master/src/WebDAV-AudioPlayer/Resources/WebDAV-AudioPlayer.png "WebDAV-AudioPlayer")
