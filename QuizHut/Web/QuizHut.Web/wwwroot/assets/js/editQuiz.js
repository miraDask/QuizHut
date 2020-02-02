$(function () {
    let quizForm = $("#quiz");

    if (quizForm) {
            let deleteQuestionButtons = Array.from(document.getElementsByTagName("a")).filter(e => e.classList.contains("deleteQuestion"));


        deleteQuestionButtons.forEach(b => b.addEventListener("click", deleteQuestion));
    }


    function deleteQuestion(e) {
        let classList = Array.from(e.target.classList);
        let questionId = classList[classList.length - 1];
        $("#" + questionId).remove();
        console.log(questionId)
    }

})