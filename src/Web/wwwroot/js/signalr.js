const connection = new signalR.HubConnectionBuilder()
    .withUrl("/progressHub")
    .build();

connection.on("ReceiveProgress", function (progress) {
    document.getElementById("progress").style.width = progress + "%";
});

connection.start().catch(function (err) {
    console.error(err.toString());
});