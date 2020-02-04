$(function () {
    let quizForm = $("#quiz");

    if (quizForm) {
        let deleteAnswerButtons = Array.from(document.getElementsByTagName("a")).filter(e => e.classList.contains("deleteAnswer"));
        let addAnswerButtons = Array.from(document.getElementsByTagName("a")).filter(e => e.classList.contains("addAnswer"));
        let deleteQuestioButtons = Array.from(document.getElementsByTagName("a")).filter(e => e.classList.contains("deleteQuestion"));

        addAnswerButtons.forEach(b => b.addEventListener("click", addNewAnswer));
        deleteAnswerButtons.forEach(b => b.addEventListener("click", deleteAnswer));
        deleteQuestioButtons.forEach(b => b.addEventListener("click", deleteQuestion));
    }

    function addNewAnswer(e) {
        let classList = Array.from(e.target.classList);
        let questionId = classList[classList.length - 1];
        let question = $("#" + questionId);
        let newAnswerId = generateGuid();

        let response = $.ajax({
            url: "/Answer/AddNewAnswerAjaxCall",
            type: "POST",
            dataType: "application/json",
            data: { "questionId": questionId, "answerId": newAnswerId },
            async: false,
        });

        if (response.status == 200) {
            let newAnswer = $($.parseHTML(response.responseText));
            console.log(newAnswer)
            let deleteAnswerBtn = $(newAnswer).find("a")[0];
            deleteAnswerBtn.addEventListener("click", deleteAnswer);
            $(question).append(newAnswer);
        }
    }

    function deleteAnswer(e) {
        let classList = Array.from(e.target.classList);
        let answerId = classList[classList.length - 1];

        var response = $.ajax({
            url: "/Answer/RemoveAnswer",
            type: "POST",
            dataType: "application/json",
            data: { "id": answerId },
            async: false,
        });

        if (response.status == 200) {
            $("#" + answerId).remove();
        }
    }

    function deleteQuestion(e) {
        let classList = Array.from(e.target.classList);
        let questionId = classList[classList.length - 1];

        var response = $.ajax({
            url: "/Question/RemoveQuestion",
            type: "POST",
            dataType: "application/json",
            data: { "id": questionId },
            async: false,
        });

        if (response.status == 200) {
            $("#" + questionId).remove();
        }
    }

    function generateGuid() {
        return (randimiser() + randimiser() + "-" + randimiser() + "-4" + randimiser().substr(0, 3) + "-" + randimiser() + "-" + randimiser() + randimiser() + randimiser()).toLowerCase();
    }
    function randimiser() {
        return (((1 + Math.random()) * 0x10000) | 0).toString(16).substring(1);
    }
})