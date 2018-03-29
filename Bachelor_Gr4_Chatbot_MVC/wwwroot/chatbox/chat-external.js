(function ($) {
    $(document).ready(function () {
        var $chatbox = $('.chatbox'),
            $chatboxTitle = $('.chatbox__title'),
            $chatboxTitleClose = $('.chatbox__title__close');

        $chatboxTitle.on('click', function () {
            $chatbox.toggleClass('chatbox--tray');
        });
        $chatboxTitleClose.on('click', function (e) {
            e.stopPropagation();
            $chatbox.addClass('chatbox--closed');
        });
        $chatbox.on('transitionend', function () {
            if ($chatbox.hasClass('chatbox--closed')) $chatbox.remove();

        });

        //$chatboxCredentials.on('submit', function(e) {
        //    e.preventDefault();
        //    $chatbox.removeClass('chatbox--empty');
        //});
    });
})(jQuery);

// Starts a SignalR connection with transport fallback - if the connection cannot be started using
// the webSockets transport the function will fallback to the serverSentEvents transport and
// if this does not work it will try longPolling. If the connection cannot be started using
// any of the available transports the function will return a rejected Promise.
function startConnection(url, configureConnection) {
    return function start(transport) {
        console.log(`Starting connection using ${signalR.TransportType[transport]} transport`)
        var connection = new signalR.HubConnection(url, { transport: transport });
        if (configureConnection && typeof configureConnection === 'function') {
            configureConnection(connection);
        }

        return connection.start()
            .then(function () {
                return connection;
            })
            .catch(function (error) {
                console.log(`Cannot start the connection use ${signalR.TransportType[transport]} transport. ${error.message}`);
                if (transport !== signalR.TransportType.LongPolling) {
                    return start(transport + 1);
                }

                return Promise.reject(error);
            });
    }(signalR.TransportType.WebSockets);
}

function displaySentMessage(message) {
    /*var str = "<div class='chatbox__body__message chatbox__body__message--left'>";
    str += "<img src='~/images/narvik_kommune_small.jpg' alt='Picture'>";
    str += "<p>" + message + "</p>";
    str += "</div>";*/
    // TODO: Html encode message.
    var encodedMsg = message;
    // Add the sent message to the page.
    var liElement = document.createElement('div');
    liElement.className += "chatbox__body__message";
    liElement.className += " chatbox__body__message--left";
    //liElement.innerHTML += '<img src="../images/narvik_kommune_small.jpg"/>';
    liElement.innerHTML += '<p>' + encodedMsg + '</p>';
    document.getElementById('chatbox__body').appendChild(liElement);
    document.getElementById('chatbox__body').scrollTop = document.getElementById('chatbox__body').scrollHeight;
}

function displayReceivedMessage(message) {
    // TODO: Html encode message.
    var encodedMsg = message;
    // Add the sent message to the page.
    var liElement = document.createElement('div');
    liElement.className += "chatbox__body__message";
    liElement.className += " chatbox__body__message--left";
    //liElement.innerHTML += '<img src="../images/narvik_kommune_small.jpg"/>';
    liElement.innerHTML += '<p>' + encodedMsg + '</p>';
    document.getElementById('chatbox__body').appendChild(liElement);
    document.getElementById('chatbox__body').scrollTop = document.getElementById('chatbox__body').scrollHeight;
}



document.addEventListener('DOMContentLoaded', function () {
    var messageInput = document.getElementById('message');
    var groupId = "";
    var talkWithChatBot = true;

    // Get the user name and store it to prepend to messages.
    var name = 'Guest';
    // Set initial focus to message input box.
    messageInput.focus();

    // Start the connection.
    startConnection('/chathub', function (connection) {
        // Create a function that the hub can call to broadcast messages.
        connection.on('broadcastMessage', function (message) {
            // TODO: Html encode display name and message.
            //var encodedName = name;
            var encodedMsg = message;
            // Add the message to the page.
            var liElement = document.createElement('div');
            liElement.className += "chatbox__body__message";
            liElement.className += " chatbox__body__message--left";
            liElement.innerHTML += '<img src="../images/narvik_kommune_small.jpg"/>';
            liElement.innerHTML += '<p>' + encodedMsg + '</p>';
            document.getElementById('chatbox__body').appendChild(liElement);
            document.getElementById('chatbox__body').scrollTop = document.getElementById('chatbox__body').scrollHeight;

        });


        /// SignalR Client methods called from hub:
        /*connection.on('send', function (message, from) {
            groupId = from;
            
            displayReceivedMessage(message);
            // TODO: Html encode message.
            var encodedMsg = message;
            // Add the sent message to the page.
            var liElement = document.createElement('div');
            liElement.className += "chatbox__body__message";
            liElement.className += " chatbox__body__message--left";
            liElement.innerHTML += '<img src="../images/narvik_kommune_small.jpg"/>';
            liElement.innerHTML += '<p>' + encodedMsg + '</p>';
            document.getElementById('chatbox__body').appendChild(liElement);
            document.getElementById('chatbox__body').scrollTop = document.getElementById('chatbox__body').scrollHeight;
            groupId = from;
        });*/

        connection.on('receiveMessage', function(groupFrom, message) {
            groupId = groupFrom;
            displayReceivedMessage(message);
        });

        connection.on('sendMessage', function (message) {
            displaySentMessage(message);
        });


        


        // TODO: for 
        connection.on('displayQueueNumber', function (queNumber) {
            displayReceivedMessage('Du er nå lagt i kø, en medarbeider vil svare deg så raskt som mulig. Din plass i køen er: ' + queNumber);
        });


    })
        .then(function (connection) {
            console.log('connection started'); // TODO:

            // Send message
            $("#sendmessage").click(function (event) {
                // TODO: Bytt ut kode her
                //connection.invoke('send', messageInput.value);
                if (talkWithChatBot) {
                    connection.invoke('sendToGroup', groupId, messageInput.value);
                    //connection.invoke('sendToGroup2', groupId, talkWithChatBot, messageInput.value);
                } else {
                    connection.invoke('sendToGroup', groupId, messageInput.value);
                }



                // Clear text box and reset focus for next comment.
                messageInput.value = '';
                messageInput.focus();
                event.preventDefault();
            });
            
            // Start chat by joining queue
            $("#chatbox_placeholder").on('click', "button[id='startChat']", function (event) {
                connection.invoke('joinQueue');
            });





        })
        .catch(error => {
            console.error(error.message);
        });
});

