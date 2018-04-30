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

const RECEIVED_MESSAGE_POSISTION = " chatbox__body__message--left";
const SENT_MESSAGE_POSITION = " chatbox__body__message--right";
const NARVIK_KOMMUNE_IMAGE = "<img src='../images/narvik_kommune_small.jpg'/>";
const USER_IMAGE = "<img src='../images/user.png'/>";

// Get a random user id
// Source: https://stackoverflow.com/questions/105034/create-guid-uuid-in-javascript
function getRandomUserId() {
    return ([1e7] + -1e3 + -4e3 + -8e3 + -1e11).replace(/[018]/g, c =>
        (c ^ crypto.getRandomValues(new Uint8Array(1))[0] & 15 >> c / 4).toString(16)
    )
}

// Set cookie used in signalr to map all users active connections. 
function setSignalRCookie(value) {
    var cookieName = "SignalRCookie";

    // Set cookie if it does not exist
    if (!(document.cookie.indexOf(cookieName + "=") >= 0)) {
        var cookieValue = value;
        var path = "/";
        var expires = ""; // Cookie expire when browser is closed

        document.cookie = cookieName + "=" + cookieValue + ";expires=" + expires + ";path=" + path + ";";

        console.log("Cookie set: " + document.cookie);
    } else {
        console.log("SignalRCookie exists: " + document.cookie);
    }
}


function displaySentMessage(message) {
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

}

function displayReceivedMessage2(message) {
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

    browserTabFlash();

    // dummy element
    var dummyEl = document.getElementById('message');
    // check for focus
    var isFocused = (document.activeElement === dummyEl);
    if (isFocused === false) {
        //addBlink();
        
    }
    else {
        removeBlink();
    }
    
}

function displayConversation(conversation) {
    var str = "";
    $.each(conversation.messages, function (index, message) {
        str += "<div class='container'>";
        str += "<button class='btn btn-primary'";
        str += "id = '" + queue.chatGroupId + "'>";
        str += queue.chatGroupName;
        str += "</button>";
        str += "<p class='text-primary'>Antall brukere i kø: " + queue.count + "</p>";
        str += "</div>";
       // str += "<p class='text-primary' id='waitTime'>Ventetid: " + queue.currentWaitTime + "</p>";
    });
    $("#chatbox__body").html(str);
}

function displayMessage(message, position, image) {
    var time = new Date().toLocaleTimeString();
    //var time = message.DateTime;

    var str = "<div class='chatbox__body__message" + position + "'>";
    str += image;
    //str += "<p>" + message.content + "<br />" + time + "</p>";
    str += "<p>" + message + "<br />" + time + "</p>";
    str += "</div>";

    $("#chatbox__body").append(str);
    document.getElementById('chatbox__body').scrollTop = document.getElementById('chatbox__body').scrollHeight;

    browserTabFlash();
    // dummy element
    var dummyEl = document.getElementById('message');
    // check for focus
    var isFocused = (document.activeElement === dummyEl);
    if (isFocused === false) {
        //addBlink();
    }
    else {
        removeBlink();
    }
}

function displayReceivedMessage(message) {
    // TODO: Html encode message.
    var encodedMsg = message;
    var time = new Date().toLocaleTimeString();
    // Add the received message to the page.
    var str = "";
    str += "<div class='chatbox__body__message chatbox__body__message--left'>";
    str += "<img src='../images/narvik_kommune_small.jpg'/>";
    str += "<p>" + encodedMsg + "<br />" + time + "</p>";
    str += "</div>";

    $("#chatbox__body").append(str);
    document.getElementById('chatbox__body').scrollTop = document.getElementById('chatbox__body').scrollHeight;

    browserTabFlash();
    // dummy element
    var dummyEl = document.getElementById('message');
    // check for focus
    var isFocused = (document.activeElement === dummyEl);
    if (isFocused === false) {
        //addBlink();
    }
    else {
        removeBlink();
    }
}


function browserTabFlash() {
    var title = document.title;
    var newTitle = "Ny melding";
    var timeout = false;

    var blink = function () {
        document.title = (document.title == newTitle ? title : newTitle);

        // Stop blinking
        if (document.hasFocus())
        {
            document.title = title;
            clearInterval(timeout);
        }
    };

    if (!timeout) {
        // Start blinking
        timeout = setInterval(blink, 400);
    };
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

//$(function () {
document.addEventListener('DOMContentLoaded', function () {
    var messageInput = document.getElementById('message');
    var groupId = "";
    var conversationId = null;
    var chatIsWithBot = true;
    var conversationIdForResult = null;
    var messageIsChatGroup = false;

    function resetChatBotVariables(chatWithBot) {
        chatIsWithBot = chatWithBot;
        conversationId = null;
    }

    setSignalRCookie(getRandomUserId());
    //var connection = new signalR.HubConnection("https://allanarnesen.com/chathub");

    // Start connection
    var connection = new signalR.HubConnection("chathub");

    /// SignalR Client functions that can be called from the hub
    connection.on('send', function (message, from) {
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
        console.log("connection on: send");
    });

    connection.on("returnConnectionId", function (connectionId) {
        console.log("returnConnectionId invoked");
        setSignalRCookie(connectionId);
        console.log("connection on: returnConnectionId");
    });

    connection.on('receiveMessage', function (groupFrom, message) {
        //groupId = groupFrom;
        displayReceivedMessage(message);
        console.log("connection on: receiveMessage: " + message);
        //displayMessage(message, RECEIVED_MESSAGE_POSITION, NARVIK_KOMMUNE_IMAGE);
    });

    connection.on('sendMessage', function (message) {
        displaySentMessage(message);
        //displayMessage(message, SENT_MESSAGE_POSITION, USER_IMAGE);
        console.log("connection on: sendMessage: " + message);
    });

    connection.on('setConversationId', function (id) {
        conversationId = id;
        console.log("connection on: setConversationId");
    });

    connection.on('setGroupId', function (id) {
        groupId = id;
        console.log("connection on: setGroupId");
    });

    connection.on('setMessageIsChatGroup', function (value) {
        messageIsChatGroup = value;
        console.log("connection on: setMessageIsChatGroup: ", value);
    });

    connection.on('errorMessage', function (message) {
        displayReceivedMessage(message);
    });

    connection.on('endBotConversation', function (id) {
        resetChatBotVariables(false);
        conversationIdForResult = id;
    });

    connection.on('conversationEnded', function (message, id) {
        resetChatBotVariables();
        conversationIdForResult = id;
        groupId = "";
        displayConversationEnded(message);
        console.log("connection on: conversationEnded");
    });

    connection.on('enableInputField', function (test) {
        messageInput.disabled = false;
        console.log("connection on: enableInputField");
    });

    connection.on('displayConversation', function (conversation) {
        console.log("connection on: displayConversation");
        //$("#chatbox__body").html("test");
        //displayReceivedMessage("test");

        //displayConversation(conversation);
    });

    // TODO: Testcode, should be deleted
    connection.on('alert', function (message) {
        console.log("connection on: alert");
        alert(message);
    });

    // TODO: for 
    connection.on('displayPlaceInQueue', function (message) {
        displayReceivedMessage(message);
        console.log("connection on: displayPlaceInQueue");
    });

    // Transport fallback
    connection.start()
        .then(() => {
            console.log("Connection started");
            connection.invoke('startConversationWithChatBot');

            // Functions used to invoke methods in hub
            function sendMessage() {
                if (messageInput.value.length > 0) { // if there is any input
                    if (messageIsChatGroup) {
                        connection.invoke('selectChatGroup', conversationId, messageInput.value);
                    }
                    else if (chatIsWithBot) {
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



            // FØLGENDE SKAL SLETTES
            // Start chat by joining queue
            /*$("#chatbox_placeholder").on('click', "button[id='startChat']", function (event) {
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
            });*/
            
        })
        
        .catch(error => { console.log(error.message); });

});





