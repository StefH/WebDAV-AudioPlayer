window.exampleJsFunctions = {
    convertArray: function (win1251Array) {
        var win1251decoder = new TextDecoder('windows-1251');
        var bytes = new Uint8Array(win1251Array);
        var decodedArray = win1251decoder.decode(bytes);
        console.log(decodedArray);
        return decodedArray;
    },
    showPrompt: function (text) {
        return prompt(text, 'Type your name here');
    },
    displayWelcome: function (welcomeMessage) {
        document.getElementById('welcome').innerText = welcomeMessage;
    },
    returnArrayAsyncJs: function () {
        DotNet.invokeMethodAsync('BlazorSample', 'ReturnArrayAsync')
            .then(data => {
                data.push(4);
                console.log(data);
            });
    },
    sayHello: function (dotnetHelper) {
        return dotnetHelper.invokeMethodAsync('SayHello')
            .then(r => console.log(r));
    }
};

//let howl;
//const playlist = [];
//window.howl = {
//    create: function (sources) {
//        sources.forEach((src) => {

//        });
//    },
//    play: function (dotnetReference, src) {
//        if (howl) {
//            howl.unload();
//        }

//        howl = new Howl({
//            // preload: false,
//            pool: 3,
//            src: [src],
//            onplay: async function () {
//                const durationInSeconds = Math.round(howl.duration());
//                console.log(durationInSeconds);

//                try {
//                    await dotnetReference.invokeMethodAsync('OnPlay', durationInSeconds);
//                }
//                catch (e) {
//                    console.log('e = ' + e);
//                }

//                console.log('sent');
//            },
//            onstop: async function () {
//                await dotnetReference.invokeMethodAsync('OnPlay', 0);
//            }
//        });
//        return howl.play();
//    },
//    stop: function () {
//        howl.stop();
//    },
//    getCurrentTime: function () {
//        if (howl && howl.playing()) {
//            const seek = howl.seek();
//            return Math.round(seek || 0);
//        }
//        return 0;
//    }
//};