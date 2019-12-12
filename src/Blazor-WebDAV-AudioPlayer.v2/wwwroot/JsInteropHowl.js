﻿let howl = null;
//let soundId = null;
window.howl = {
    play: function (dotnetReference, src, format) {
        if (howl) {
            stop();
        }

        howl = new Howl({
            // preload: false,
            // pool: 3,
            src: [src],
            format: [format],
            onplay: async function () {
                try {
                    const duration = Math.round(howl.duration());
                    await dotnetReference.invokeMethodAsync('OnPlayCallback', duration);
                }
                catch (e) {
                    console.log('Error = ' + e);
                }
            },
            onstop: async function () {
                await dotnetReference.invokeMethodAsync('OnStopCallback');
            }
        });

        return howl.play();
    },
    stop: function () {
        if (howl) {
            howl.stop();
            howl.unload();
        }

        howl = null;
    },
    pause: function () {
        if (howl) {
            howl.pause();
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
    }
};