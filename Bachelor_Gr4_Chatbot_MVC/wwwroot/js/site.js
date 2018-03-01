// Referanser: 
// https://docs.microsoft.com/en-us/aspnet/signalr/overview/guide-to-the-api/hubs-api-guide-javascript-client

function testWithProxy() {
    var chatHubProxy = $.connection.chatHub;
    chatHubProxy.client.Send = function (name, message) {
        console.log(name + ' ' + message);
    }

    $.connection.hub.start().done(function () {
        $('#testProxy').click(function () {
            var msg = $('#messageProxy').val('');
            chatHubProxy.server.Send('testNavn', msg);
            msg.focus();
        });
    });
}