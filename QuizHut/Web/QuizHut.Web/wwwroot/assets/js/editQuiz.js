$(function () {
    let quizForm = $("#quiz");

    if (quizForm) {
        let deleteAnswerButtons = Array.from(document.getElementsByTagName("a")).filter(e => e.classList.contains("deleteAnswer"));
        let deleteQuestioButtons = Array.from(document.getElementsByTagName("a")).filter(e => e.classList.contains("deleteQuestion"));


        deleteAnswerButtons.forEach(b => b.addEventListener("click", deleteAnswer));
        deleteQuestioButtons.forEach(b => b.addEventListener("click", deleteQuestion));
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
            data: { "id" : questionId },
            async: false,
        });

        if (response.status == 200) {
            $("#" + questionId).remove();
        }
    }
})