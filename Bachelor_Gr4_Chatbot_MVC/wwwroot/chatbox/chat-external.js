(function ($) {
    $(document).ready(function () {
        var $chatbox = $('#chatbox'),
            $chatboxTitle = $('#chatbox__title'),
            $chatboxTitleClose = $('#chatbox__title__close'),
            $chatboxCredentials = $('#chatbox__credentials'),
            $chatlight = $('#chatbox__light');
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
        $chatboxCredentials.on('click', function (e) {
            e.preventDefault();
            $chatbox.removeClass('chatbox--empty');
            // This classchange should be done when the connection is established
            // so we can give a errormessage and keep the red light if the connection fails
            // Just placed here for now.
            $chatboxTitle.removeClass('chatbox_title_unconnected');
            $chatboxTitle.addClass('chatbox_title_connected');
            
        });
    });
})(jQuery);

function sessionId() {
    //https://blogs.msmvps.com/ricardoperes/2015/10/29/persisting-signalr-connections-across-page-reloads/
    var sessionId = window.sessionStorage.sessionId;

    if (!sessionId) {
        sessionId = window.sessionStorage.sessionId = Date.Now();
    }

    return sessionId;
}

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

function startConnectionWithProxy() {
    var chatHubProxy = $.connection.chatHub;
    //chatHubProxy.client.

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
    liElement.className += " chatbox__body__message--right";
    liElement.innerHTML += '<img src="../images/user.png"/>';
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
    liElement.innerHTML += '<img src="../images/narvik_kommune_small.jpg"/>';
    liElement.innerHTML += '<p>' + encodedMsg + '</p>';
    document.getElementById('chatbox__body').appendChild(liElement);
    document.getElementById('chatbox__body').scrollTop = document.getElementById('chatbox__body').scrollHeight;

    // dummy element
    var dummyEl = document.getElementById('message');
    // check for focus
    var isFocused = (document.activeElement === dummyEl);
    if (isFocused === false) {
        addBlink();
    }
    else {
        removeBlink();
    }
    
}
function addBlink() {
    var element = document.getElementById("chatbox__light");
    element.classList.add("chatbox__blink");
    element.innerHTML = '';
    var a = document.createElement('a');
    a.setAttribute('href', '#');
    a.innerHTML = 'Ny beskjed mottat';
    element.appendChild(a);
}

function removeBlink() {
    var element = document.getElementById("chatbox__light");
    element.classList.remove("chatbox__blink");
    element.innerHTML = '';
    var a = document.createElement('a');
    a.setAttribute('href', '#');
    a.innerHTML = 'Svarta Bjørn';
    element.appendChild(a);
}


function displayConversationEnded(startMessage) {
    var str = startMessage; 
    str += "Fikk du svar på det du lurte på? ";
    str += "<div class='btn-group' aria-label='Fikk du svar?'>";
    str += "<button type='button' class='btn btn-primary' id='resultYes'>Ja</button>";
    str += "<button type='button' class='btn btn-primary' id='resultNo'>Nei</button>";
    str += "</div>";


    str += "<button id='startChat' class='btn btn-success btn-block'> Start Chat</button>";
    str += "<button id='startChatBot' class='btn btn-success btn-block'>Start ChatBot</button>";


    displayReceivedMessage(str);
}

document.addEventListener('DOMContentLoaded', function () {
    var messageInput = document.getElementById('message');
    var groupId = "";
    var conversationId = null;
    var chatBotToken = "";
    var chatIsWithBot = false;
    var conversationIdForResult = null;

    // Set initial focus to message input box.
    messageInput.focus();

    function resetChatBotVariables() {
        chatIsWithBot = false;
        conversationId = null;
        chatBotToken = "";
    }


    //$("#connectToSignalR").click(function () {
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

        connection.on('receiveMessage', function (groupFrom, message) {
            //groupId = groupFrom;
            displayReceivedMessage(message);
        });

        connection.on('sendMessage', function (message) {
            displaySentMessage(message);
        });

        connection.on('setConversationId', function (id) {
            conversationId = id;
        });

        connection.on('setGroupId', function (id) {
            groupId = id;
        });

        connection.on('setChatBotToken', function (token) {
            chatBotToken = token;
            chatIsWithBot = true;
        });

        connection.on('errorMessage', function (message) {
            displayReceivedMessage(message);
        });

        connection.on('endBotConversation', function (message, id) {
            resetChatBotVariables();
            conversationIdForResult = id;
            displayConversationEnded(message);
        });

        connection.on('conversationEnded', function (message, id) {
            resetChatBotVariables();
            conversationIdForResult = id;
            groupId = "";
            displayConversationEnded(message);
        });

        connection.on('enableInputField', function (test) {
            messageInput.disabled = false;
        });




            // TODO: for 
            connection.on('displayPlaceInQueue', function (queNumber) {
                displayReceivedMessage('Du er nå lagt i kø, en medarbeider vil svare deg så raskt som mulig. Din plass i køen er: ' + queNumber);
            });


        })
            .then(function (connection) {
                console.log('connection started'); // TODO:

                function sendMessage() {
                    if (messageInput.value.length > 0) { // if there is any input
                        if (chatIsWithBot) {
                            connection.invoke('sendToChatBot', conversationId, chatBotToken, messageInput.value);
                        } else {
                            connection.invoke('sendToGroup', groupId, messageInput.value, conversationId);
                            //displaySentMessage(messageInput.value);
                        }
                        messageInput.value = '';
                        messageInput.focus();
                        event.preventDefault();
                    }
                }

                document.querySelector('#message').addEventListener('keypress', function (e) {
                    var key = e.which || e.keyCode;
                    if (key === 13) { // 13 is enter
                        sendMessage();
                    }
                });

                // Send message
                $("#sendmessage").click(function (event) {
                    sendMessage();
                });
                
                // Start chat by joining queue
                $("#chatbox_placeholder").on('click', "button[id='startChat']", function (event) {
                    if (chatIsWithBot) {
                        connection.invoke('endConversationWithChatBot', conversationId)
                        resetChatBotVariables();
                    }
                    connection.invoke('joinQueue');
                    messageInput.disabled = true;
                });

                // Start chat-bot
                $("#chatbox_placeholder").on('click', "button[id='startChatBot']", function (event) {
                    connection.invoke('startConversationWithChatBot');
                });

                // Store conversation result
                $("#chatbox_placeholder").on('click', "button[id='resultYes']", function (event) {
                    connection.invoke('registerConversationResult', conversationIdForResult, true);
                    displayReceivedMessage("Takk for din tilbakemelding!");
                });
                $("#chatbox_placeholder").on('click', "button[id='resultNo']", function (event) {
                    connection.invoke('registerConversationResult', conversationIdForResult, false);
                    displayReceivedMessage("Takk for din tilbakemelding!");
                });

            })
            .catch(error => {
                console.error(error.message);
            });
    //});

    
});

