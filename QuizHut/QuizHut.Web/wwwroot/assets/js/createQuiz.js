(function () {

    let count = 1;
    let addNameBtn = document.getElementById("addName");
    
    if (addNameBtn) {

        addNameBtn.addEventListener("click", addName);
    }

    function addName(event) {
        event.stopPropagation()

        let nameElement = document.getElementById("quizName");
        let infoElement = document.getElementById("info");
        let input = document.getElementsByTagName("input")[0];

        let name = input.value;
        nameElement.textContent = name;
        infoElement.style.display = "none";

        document.getElementById("name").style.display = "none";
        document.getElementById("question").style.display = "block";
        document.getElementById("addQuestion").addEventListener("click", addQuestion);
    }

    function addQuestion(event) {
        event.stopPropagation();
        let questionText = document.getElementById("questionText").value;

        displayQuestion(questionText);
    }


    function displayQuestion(text) {

    }
})();