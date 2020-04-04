$(function () {
    let connection = null;

    setupConnection = () => {
        connection = new signalR.HubConnectionBuilder()
            .withUrl("/quizHub")
            .withAutomaticReconnect()
            .build();

        connection.start()
            .catch(err => console.error(err.toString()));

        connection.on("NewActiveEventMessage", () => {
            document.getElementById("events").innerHTML = "You have one active event waiting for you!";
     
        });
    }

    setupConnection();
});