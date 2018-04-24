document.addEventListener('DOMContentLoaded', function () {
    // Start connection
    var connection = new signalR.HubConnection("chathub");

    connection.on("numberOfChatWorkersConnected", function (count) {
        $("#chatWorkersConnected").html(count);
    });

    connection.on("numberOfClientsConnected", function (count) {
        $("#chatUsersConnected").html(count);
    });

    connection.on("numberInQueue", function (count) {
        $("#numberInQueue").html(count);
        $("#numberInQueue2").html(count);
    });

    connection.start()
        .then(() => {
            console.log("Connection started");
        })
        .catch(error => { console.log(error.message); });
});