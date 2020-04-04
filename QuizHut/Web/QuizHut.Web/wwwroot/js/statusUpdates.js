$(function () {
    const STATUS = {
        ACTIVE: {
            TEXT: 'Active',
            COLOR: '#0f990f'
        },
        ENDED: {
            TEXT: 'Ended',
            COLOR: '#d75277'
        },
        PENDING: {
            TEXT: 'Pending',
            COLOR1: '#f4f717',
            COLOR2: '#e2a00c'
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

        connection.on("NewEventStatusUpdate", (status, eventId) => {
            let text = '';
            let color = '';
            let statusElement = Array.from(document.getElementsByClassName('status')).filter(x => x.getAttribute("data") == eventId)[0];
            if (status == STATUS.ACTIVE.TEXT) {
                text = STATUS.ACTIVE.TEXT;
                color = STATUS.ACTIVE.COLOR;
            } else if (status == STATUS.ENDED.TEXT) {
                text = STATUS.ENDED.TEXT;
                color = STATUS.ENDED.COLOR;
            } else {
                text = STATUS.PENDING.TEXT;
                if (window.location.href.indexOf("Details") > -1) {
                    color = STATUS.PENDING.COLOR2;
                } else {
                    color = STATUS.PENDING.COLOR1;
                }
            }

            statusElement.innerHTML = text;
            statusElement.style.color = color;
        });

    }

    setupConnection();
});