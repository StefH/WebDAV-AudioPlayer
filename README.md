# WebDAV-AudioPlayer
A simple AudioPlayer to play music from a WebDAV location.

The following codecs are supported:
* MP3
* WAVE
* FLAC
* AAC
* AC3
* WMA
* OGG-Vorbis

Used libraries:
* [CSCore](https://github.com/filoe/cscore) for playing audio files.
* [WebDAV-Client](https://github.com/StefH/WebDAV-Client) and [Portable-WebDAV-Library
](https://github.com/DecaTec/Portable-WebDAV-Library) for access the WebDAV location.
* [MediaInfo.DotNetWrapper](https://github.com/StefH/MediaInfo.DotNetWrapper) a C# .NET Wrapper for MediaInfo.Native and MediaInfo.dll
* [MediaInfo.Native](https://www.nuget.org/packages/MediaInfo.Native/) and [MediaInfo.dll](https://mediaarea.net/en/MediaInfo) are used to get some more details like bitrate from the audio files.


<br>
<br>
Screenshot:

![WebDAV-AudioPlayer](https://raw.githubusercontent.com/StefH/WebDAV-AudioPlayer/master/src/WebDAV-AudioPlayer/Resources/WebDAV-AudioPlayer.png "WebDAV-AudioPlayer")
