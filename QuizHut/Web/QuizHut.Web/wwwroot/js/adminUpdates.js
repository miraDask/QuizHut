$(function () {
    const MESSAGES = {
        EVENTS: {
            NEW: 'just created new Event:',
            STARTED: 'event just started!',
            ENDED: 'event just ended!'
        },
        QUIZ: {
            NEW: 'just created new Quiz:'
        }
    };

    let connection = null;

    setupConnection = () => {
        connection = new signalR.HubConnectionBuilder()
            .withUrl('/quizHub')
            .withAutomaticReconnect()
            .build();

        connection.start()
            .catch(err => console.error(err.toString()));

        connection.on("NewEventUpdate", (creatorName, eventName) => {
            let li = document.createElement('li');
            li.innerHTML = `${creatorName} ${MESSAGES.EVENTS.NEW} ${eventName}!<hr>`;
            document.getElementById("updates").prepend(li);
        });

        connection.on("NewQuizUpdate", (creatorName, quizName) => {
            let li = document.createElement('li');
            li.innerHTML = `${creatorName} ${MESSAGES.QUIZ.NEW} ${quizName}!<hr>`;
            document.getElementById("updates").prepend(li);
        });

        connection.on("ActiveEventUpdate", (eventName) => {
            let li = document.createElement('li');
            li.innerHTML = `${eventName} ${MESSAGES.EVENTS.STARTED}<hr>`;
            document.getElementById("updates").prepend(li);
        });

        connection.on("EndedEventUpdate", (eventName) => {
            let li = document.createElement('li');
            li.innerHTML = `${eventName} ${MESSAGES.EVENTS.ENDED}<hr>`;
            document.getElementById("updates").prepend(li);
        });
    }

    setupConnection();
});