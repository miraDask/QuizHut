$(function () {
    const URL_PARTS = {
        STUDENTS: "StudentsAll",
        GROUPS: "GroupsAll",
        RESULTS: "ResultsAll",
        QUIZZES: "QuizzesAll",
        EVENTS: "EventsAll",
        MAIN: "Dashboard"
    }

    const timezone = Intl.DateTimeFormat().resolvedOptions().timeZone;
    $('#timezone').val(timezone);

    var url = window.location.href;
    var navs = $('a[class*=" nav-to-change"]');

    [...navs].forEach(x => {
        $(x).removeClass("active");
    });

    if (url.indexOf(URL_PARTS.RESULTS) >= 0) {
        $('#results').addClass('active');
    } else if (url.indexOf(URL_PARTS.EVENTS) >= 0) {
        $('#events').addClass('active');
    } else if (url.indexOf(URL_PARTS.QUIZZES) >= 0) {
        $('#quizzes').addClass('active');
    } else if (url.indexOf(URL_PARTS.GROUPS) >= 0) {
        $('#groups').addClass('active');
    } else if (url.indexOf(URL_PARTS.STUDENTS) >= 0) {
        $('#students').addClass('active');
    } else if (url.indexOf(URL_PARTS.MAIN) >= 0) {
        $('#main').addClass('active');
    }
})