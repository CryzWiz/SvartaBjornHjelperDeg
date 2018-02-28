let connection = new signalR.HubConnection('/chat');

connection.on('send', data => {
    console.log(data);
});

connection.start()
    .then(() => connection.invoke('send', 'Hello'));

var connection = new HubConnectionBuilder()
    .WithUrl("http://localhost:5000/")
    .WithConsoleLogger()
    .Build();

connection.On<string>("Send", data => {
    Console.WriteLine($"Received: {data}");
});

await connection.StartAsync();

await connection.InvokeAsync("Send", "Hello");