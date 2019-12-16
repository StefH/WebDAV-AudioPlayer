let howl = null;
let soundId = null;
window.howl = {
    play: function (dotnetReference, options) {
        if (howl) {
            stop();
        }

        // ReSharper disable once UseOfImplicitGlobalInFunctionScope
        howl = new Howl({
            src: options.sources,
            html5: options.html5,
            onplay: async function (id) {
                const duration = Math.round(howl.duration());
                await dotnetReference.invokeMethodAsync('OnPlayCallback', id, duration);
            },
            onstop: async function (id) {
                await dotnetReference.invokeMethodAsync('OnStopCallback', id);
            },
            onend: async function (id) {
                await dotnetReference.invokeMethodAsync('OnEndCallback', id);
            },
            onloaderror: async function (id, error) {
                await dotnetReference.invokeMethodAsync('OnLoadErrorCallback', id, error);
            },
            onplayerror: async function (id, error) {
                await dotnetReference.invokeMethodAsync('OnPlayErrorCallback', id, error);
            }
        });

        soundId = howl.play();
        return soundId;
    },
    stop: function () {
        if (howl) {
            howl.stop();
            howl.unload();
        }

        soundId = null;
        howl = null;
    },
    pause: function (id) {
        if (howl) {
            if (howl.playing()) {
                if (id) {
                    howl.pause(id);
                } else {
                    howl.pause();
                }
            } else {
                howl.play(soundId);
            }
        }
    },
    seek: function (position) {
        if (howl) {
            howl.seek(position);
        }
    },
    getIsPlaying: function () {
        if (howl) {
            return howl.playing();
        }

        return false;
    },
    getCurrentTime: function () {
        if (howl && howl.playing()) {
            const seek = howl.seek();
            return Math.round(seek || 0);
        }

        return 0;
    },
    getTotalTime: function () {
        if (howl) {
            const duration = howl.duration();

            console.log(duration);

            return Math.round(duration || 0);
        }

        return 0;
    },
    getCodecs: function () {
        const codecs = [];
        for (let [key, value] of Object.entries(Howler._codecs)) {
            if (value) {
                codecs.push(key);
            }
        }

        return codecs.sort();
    },
    isCodecSupported: function (extension) {
        return extension ? Howler._codecs[extension.replace(/^x-/, '')] : false;
    }
};