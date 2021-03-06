﻿(function ($) {
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

$("#popup_chatbox_placeholder").load("chatbox.html");

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

function browserTabFlash() {
    var title = document.title;
    var newTitle = "Ny melding";
    var timeout = false;

    var blink = function () {
        document.title = (document.title == newTitle ? title : newTitle);

        // Stop blinking
        if (document.hasFocus()) {
            document.title = title;
            clearInterval(timeout);
        }
    };

    if (!timeout) {
        // Start blinking
        timeout = setInterval(blink, 400);
    };
}

function displayReceivedMessage(message) {
    /*var str = "<div class='chatbox__body__message chatbox__body__message--left'>";
    str += "<img src='~/images/narvik_kommune_small.jpg' alt='Picture'>";
    str += "<p>" + message + "</p>";
    str += "</div>";*/
    // TODO: Html encode message.
    var encodedMsg = message;
    var time = new Date().toLocaleTimeString();
    // Add the sent message to the page.
    var liElement = document.createElement('div');
    liElement.className += "chatbox__body__message";
    liElement.className += " chatbox__body__message--right";
    liElement.innerHTML += '<img src="../images/user.png"/>';
    liElement.innerHTML += '<p>' + encodedMsg + '<br />' + time + '</p>';
    document.getElementById('chatbox__body').appendChild(liElement);
    document.getElementById('chatbox__body').scrollTop = document.getElementById('chatbox__body').scrollHeight;

    browserTabFlash();
}






/*
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
*/

function displaySentMessage(message) {
    // TODO: Html encode message.
    var encodedMsg = message;
    var time = new Date().toLocaleTimeString();
    // Add the sent message to the page.
    var liElement = document.createElement('div');
    liElement.className += "chatbox__body__message";
    liElement.className += " chatbox__body__message--left";
    liElement.innerHTML += '<img src="../images/narvik_kommune_small.jpg"/>';
    liElement.innerHTML += '<p>' + encodedMsg + '<br />' + time + '</p>';
    document.getElementById('chatbox__body').appendChild(liElement);
    document.getElementById('chatbox__body').scrollTop = document.getElementById('chatbox__body').scrollHeight;
}

// ------------------ All queue buttons ------------------
function displayChatQueueButtons(queues) {
    var str = "";
    $.each(queues, function (index, queue) {
        str += "<button class='btn btn-primary'";
        str += "id = '" + queue.chatGroupId + "'>";
        str += queue.chatGroupName;
        str += "</button>";
        str += "<p>Tid i kø: " + queue.activeWaitTime + "<br />";
        str += "Gjennomsnittlig ventetid: " + queue.currentWaitTime + "<br />";
        str += "</p>";
       // str += "<p class='text-primary' id='waitTime'>Ventetid: " + queue.currentWaitTime + "</p>";
    });
    $("#queueContainer").html(str);
}

// Display queue
function displayChatQueues(queues) {
    var str = "";
    str += "<h5>Chat-grupper: </h5>";
    $.each(queues, function (index, queue) {
        //str += "<div class='container'>";

        str += "<p class='text-primary'>";
        str += queue.chatGroupName + "</p>";
        str += "<p>Tid i kø: " + queue.activeWaitTime + "<br />";
        str += "Gjennomsnittlig ventetid: " + queue.currentWaitTime + "<br />";
        str += "</p>";
        //str += "</div>";
        // str += "<p class='text-primary' id='waitTime'>Ventetid: " + queue.currentWaitTime + "</p>";
    });
    $("#queueContainer").html(str);
}


function CreateButton(id, text, buttonClass) {
    str = "<button class='" + buttonClass + "' ";
    str += "id = '" + id + "'>";
    str += text;
    str += "</button>";
    return str;
}

function displayQueueCounter(count) {
    var str = "Antall brukere i kø: " + count;
    $("#inQueue").html(str);
}


/*
function displayQueue(connections) {
    var str = "";
    $.each(connections, function (index, key) {
        str += "<tr>";
            str += "<td>" + (index + 1) + "</td>";
            str += "<td>" + key + "</td>"; // TODO: Endres
            str += "<td>test</td><td>test</td>"; // TODO: Endres
            str += "<td>"
            str += "Testknapp fjernet ";
            //str += "<button class='btn btn-default' name= 'joinChat' value= '" + key + "'> Åpne chat</button>";
            str += "</td>";
        str += "</tr>";
    });
    $("#queueList").html(str);
}
 
function addToQueue(connection) {
    var str = "";
    str += "<tr>";
    str += "<td>" + test + "</td>";
    str += "<td>" + key + "</td>"; // TODO: Endres
    str += "<td>test</td><td>test</td>"; // TODO: Endres
    str += "<td><button class='btn btn-default' name='joinGroup' value='" + key + "' >Åpne chat</button></td>";
    str += "</tr>";
    $("#queueList").append(str);
}*/


// -------------- List of all connections ------------------
/*
function updateConnectionList(connections) {
    var str = "";
    $.each(connections, function (index, key) {
        str += "<tr>";
            str += "<td>" + (index + 1) + "</td>";
            str += "<td>" + key + "</td>"; // TODO: Endres
            str += "<td>test</td><td>test</td>"; // TODO: Endres
            str += "<td>"
            str += "Testknapp fjernet ";
            //str += "<button class='btn btn-default' name= 'joinChat' value= '" + key + "'> Åpne chat</button>";
            str += "</td>";
        str += "</tr>";

    });
    $("#connectionList").html(str);
}
*/

// 
document.addEventListener('DOMContentLoaded', function () {
    var messageInput = document.getElementById('message');
    var groupId = "";
    var conversationId = "";
    var chatBoxBody = document.getElementById('chatbox__body');
    var loggedIn = false;
    var status = 1;
    // Set initial focus to message input box.
    messageInput.focus();

    // Start the connection.
    //startConnection('https://allanarnesen.com/chathub', function (connection) {
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
        connection.on('send', function (sender, msg) {
            //alert("I GOT HERE!");
            alert("message: " + msg + "--- sender:" + sender);
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

        });

        connection.on('setGroupId', function (id) {
            console.log("setGroupId", id);
            groupId = id;
        });

        connection.on('setConversationId', function (id) {
            console.log("setConversationId", id);
            conversationId = id;
        });

        connection.on('displayConnections', function (connections) {
            console.log("displayConnections", connections);
            updateConnectionList(connections);
        });

        connection.on('displayQueueCount', function (count) {
            console.log("displayQueueCount", count);
            displayQueueCounter(count);
        });

        connection.on('addToQueue', function (connection) {
            console.log("addToQueue", connection);
            addToQueue(connection);
        });

        connection.on('errorMessage', function (message) {
            console.log("errorMessage", message);
            displayReceivedMessage(message);
        });

        connection.on('receiveMessage', function (groupFrom, message) {
            console.log("receiveMessage", groupFrom, message);
            displayReceivedMessage(message);
        });

        connection.on('sendMessage', function (message) {
            console.log("sendMessage", message);
            displaySentMessage(message);
        });

        connection.on('conversationEnded', function (message, id) {
            console.log("conversationEnded", message, id);
            var groupId = "";
            var conversationId = "";
            $("#chatbox__body").html("Du er ikke påkoblet noen chat.");
        });

        connection.on('displayWaitTime', function (waitTime) {
            console.log("displayWaitTime", waitTime);
            $("#waitTime").html("Ventetid: " + waitTime);
        });

        connection.on('displayAllChatQueues', function (queues) {
            console.log("displayAllChatQueues", queues);
            displayChatQueueButtons(queues);
            //
            //displayChatQueues(queues);
        });


 

    })
        .then(function (connection) {
            console.log('connection started'); // TODO:
            connection.invoke('displayQueueCount');

            function sendMessage() {
                if (messageInput.value.length > 0) { // if there is any input
                    connection.invoke('sendToGroup', groupId, messageInput.value, conversationId);

                    // Clear text box and reset focus for next comment.
                    messageInput.value = '';
                    messageInput.focus();
                    event.preventDefault();
                }
            }

            // Send message
            document.querySelector('#message').addEventListener('keypress', function (e) {
                var key = e.which || e.keyCode;
                if (key === 13) { // 13 is enter
                    sendMessage();
                }
            });

            $("#sendmessage").click(function (event) {
                sendMessage();
            });


            // Join Chat-Group 
            /*
            $("#connectionList").on('click', "button[name='joinChat']", function (event) {
                groupId = $(this).val();
                connection.invoke('joinChat', groupId);
            });
            $("#queueList").on('click', "button[name='joinChat']", function (event) {
                groupId = $(this).val();
                connection.invoke('joinChat', groupId);
            });*/

            // Pick from queue
            $("#pickFromQueue").click(function (event) {
                $("#chatbox__body").html("");
                connection.invoke('pickFromQueue');
            });

            // End conversation
            $("#endConversation").click(function (event) {
                connection.invoke('endConversation', conversationId, groupId);
            });
            
            // Chat login
            $("#chatLogin").click(function (event) {
                loggedIn = true;
                connection.invoke('logIn')
            });

            // Queue, pick from specific queue
            $("#queueContainer").on('click', "button", function (event) {
                var queueId = this.id;
                //connection.invoke('pickFromQueue');
                connection.invoke('pickFromSpecificQueue', queueId);
            });
        })
        .catch(error => {
            console.error(error.message);
        });
});

