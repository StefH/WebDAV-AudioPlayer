window.exampleJsFunctions = {
    convertArray: function(win1251Array) {
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