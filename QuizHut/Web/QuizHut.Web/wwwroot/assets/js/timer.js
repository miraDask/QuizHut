$(document).ready(function () {
    var startBtn = document.getElementById('start');
    var counter = 1;

    if (startBtn) {
        var mins = document.getElementById("minutes").value;
        var questionCount = parseInt(document.getElementsByTagName("form")[0].id);
        var nextBtns = Array.from(document.getElementsByTagName('a')).filter(x => x.id.includes('next'));
        $(nextBtns).click(loadNextQuestion);
        $(startBtn).click(startQuiz)
    }

    function startQuiz() {
        $('#clockdiv').show();
        $('#pagging').show();
        $('#submit').show();
        $('#details').hide();
        startTimer();
        showQuestion(counter);
    }

    function loadNextQuestion(e) {
        e.preventDefault();
        $(`#${counter}`).hide();
        if (counter == questionCount) {
            $(`#${counter}`).show();
        } else {
            $(`#${counter + 1}`).show();
        }

        if (counter < questionCount) {
            counter++;
        }
        let prevBtn = $(`#${counter} #prev`)[0];
        $(prevBtn).click(loadPreviousQuestion);

    }

    function loadPreviousQuestion(e) {
        e.preventDefault();
        $(`#${counter}`).hide();
        if (counter == 1) {
            $(`#${counter}`).show();
        } else {
            $(`#${counter - 1}`).show();
        }

        if (counter > 1) {
            counter--;
        }
    }

    function showQuestion(counter) {
        $(`#${counter}`).show();
    }

    function startTimer() {
        let now = new Date($.now());
        let endTime = getEndDate(now, mins);
        initializeClock('clockdiv', endTime);

        function getTimeRemaining(endtime) {
            var t = Date.parse(endtime) - Date.parse(new Date());
            var seconds = Math.floor((t / 1000) % 60);
            var minutes = Math.floor((t / 1000 / 60) % 60);

            return {
                'total': t,
                'minutes': minutes,
                'seconds': seconds
            };
        }

        function initializeClock(id, endtime) {
            var clock = document.getElementById(id);
            var minutesSpan = clock.querySelector('.minutes');
            var secondsSpan = clock.querySelector('.seconds');

            function updateClock() {
                var t = getTimeRemaining(endtime);

                minutesSpan.innerHTML = ('0' + t.minutes).slice(-2);
                secondsSpan.innerHTML = ('0' + t.seconds).slice(-2);

                if (t.total <= 0) {
                    //todo redirect to ...
                    clearInterval(timeinterval);
                }
            }

            updateClock();
            var timeinterval = setInterval(updateClock, 1000);
        }

        function getEndDate(dt, minutes) {
            return new Date(dt.getTime() + minutes * 60000).toString();
        }
    }
})