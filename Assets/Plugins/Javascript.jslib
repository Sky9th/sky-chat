mergeInto(LibraryManager.library, {
    jSLibWebSocketInit: function (url, port) {
        let socket;
        if (!window.WebSocket) {
            window.WebSocket = window.MozWebSocket;
        }
        if (window.WebSocket) {
            socket = new WebSocket(url + ":"+ port +"/websocket");
            socket.onopen = function (event) {
                console.log("client connected", event)
                if (window.myGameInstance) {
                    window.myGameInstance.SendMessage('NetworkController', 'wsConnectedCallback');
                }
            }
            socket.onmessage = function (event) {
                console.log("client receive msg", event)
                let data = event.data;
                window.myGameInstance.SendMessage('NetworkController', 'wsReceiveCallback', data);
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
        window.jSLibWebSocket = socket;
        return socket;
    },
    jSLibWebSocketSend: function (sendStr) {
        console.log("receive sendstr:", sendStr)
        console.log("send:",  UTF8ToString(sendStr))
        window.jSLibWebSocket.send(UTF8ToString(sendStr));
    },
    jSLibWebSocketClose: function (sendStr) {
        window.jSLibWebSocket.close();
    }
});