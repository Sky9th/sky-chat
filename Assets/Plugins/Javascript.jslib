mergeInto(LibraryManager.library, {
    jSLibWebSocketInit: function () {
        let socket;
        if (!window.WebSocket) {
            window.WebSocket = window.MozWebSocket;
        }
        if (window.WebSocket) {
            socket = new WebSocket("ws://127.0.0.1:6667/websocket");
            socket.onopen = function (event) {
                console.log("client connected", event)
                if (window.myGameInstance) {
                    window.myGameInstance.SendMessage('NetworkController', 'wsConnectedCallback');
                }
            }
            socket.onmessage = function (event) {
                console.log("client receive msg", event)
                let data = event.data;
                let obj = JSON.parse(data);
            }
            socket.onclose = function (event) {
                console.log("client close", event)
            }
            socket.onerror = function (event) {
                console.log("client trigger error", event)
            }
        } else {
            alert("您的浏览器不支持WebSocket协议！");
        }
        this.socket = socket;
        return socket;
    },
    jSLibWebSocketSend: function (sendStr) {
        console.log("receive sendstr:", sendStr)
        console.log("send:",  UTF8ToString(sendStr))
        this.socket.send(UTF8ToString(sendStr));
    }
});