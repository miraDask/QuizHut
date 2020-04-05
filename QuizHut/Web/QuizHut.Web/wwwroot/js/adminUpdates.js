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
            let date = new Date();
            let hours = addZero(date.getHours());
            let minutes = addZero(date.getMinutes());

            let li = document.createElement('li');
            li.innerHTML = `<i class="fas fa-check text-warning mr-3"></i>${creatorName} ${MESSAGES.EVENTS.NEW} ${eventName}!<span class="ml-2 font-weight-bold">${hours}:${minutes}</span><hr>`;
            document.getElementById("updates").prepend(li);
        });

        connection.on("NewQuizUpdate", (creatorName, quizName) => {
            let date = new Date();
            let hours = addZero(date.getHours());
            let minutes = addZero(date.getMinutes());

            let li = document.createElement('li');
            li.innerHTML = `<i class="fas fa-check text-info mr-3"></i>${creatorName} ${MESSAGES.QUIZ.NEW} ${quizName}!<span class="ml-2 font-weight-bold">${hours}:${minutes}</span><hr>`;
            document.getElementById("updates").prepend(li);
        });

        connection.on("ActiveEventUpdate", (eventName) => {
            let date = new Date();
            let hours = addZero(date.getHours());
            let minutes = addZero(date.getMinutes());

            let li = document.createElement('li');
            li.innerHTML = `<i class="fas fa-check text-success mr-3"></i>${eventName} ${MESSAGES.EVENTS.STARTED}<span class="ml-2 font-weight-bold">${hours}:${minutes}</span><hr>`;
            document.getElementById("updates").prepend(li);
        });

        connection.on("EndedEventUpdate", (eventName) => {
            let date = new Date();
            let hours = addZero(date.getHours());
            let minutes = addZero(date.getMinutes());

            let li = document.createElement('li');
            li.innerHTML = `<i class="fas fa-check text-danger mr-3"></i>${eventName} ${MESSAGES.EVENTS.ENDED}<span class="ml-2 font-weight-bold">${hours}:${minutes}</span><hr>`;
            document.getElementById("updates").prepend(li);
        });
    }

    setupConnection();

    function addZero(i) {
        if (i < 10) {
            i = "0" + i;
        }
        return i;
    }
});