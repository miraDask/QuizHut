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
            let li = document.createElement('li');
            li.innerHTML = `<i class="fas fa-check text-success mr-3"></i>One of your events just started!
                             <a href="/Students/StudentActiveEventsAll" class="btn btn-outline-success btn-icon btn-sm">
                             <i class="fas fa-arrow-right text-success"></i>
                             </a>
                            <hr>`;
            document.getElementById("updates").prepend(li);
        });

        connection.on("NewPendingEventMessage", () => {
            let li = document.createElement('li');
            li.innerHTML = `<i class="fas fa-check text-warning mr-3"></i>You have one new pending event!
                             <a href="/Students/StudentPendingEventsAll" class="btn btn-outline-warning btn-icon btn-sm">
                             <i class="fas fa-arrow-right text-warning"></i>
                             </a>
                            <hr>`;
            document.getElementById("updates").prepend(li);
        });

        connection.on("NewEndedEventMessage", () => {
            let li = document.createElement('li');
            li.innerHTML = `<i class="fas fa-check text-danger mr-3"></i>One of your events just ended!
                             <a href="/Students/StudentEndedEventsAll" class="btn btn-outline-danger btn-icon btn-sm">
                             <i class="fas fa-arrow-right text-danger"></i>
                             </a>
                            <hr>`;
            document.getElementById("updates").prepend(li);
        });
    }

    setupConnection();
});
