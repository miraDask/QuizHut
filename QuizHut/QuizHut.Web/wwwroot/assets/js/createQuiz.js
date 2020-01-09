(function () {
    
    //prevents Enter key to submit form:
    window.addEventListener('keydown', function (e) {
        if (e.keyIdentifier == 'U+000A' || e.keyIdentifier == 'Enter' || e.keyCode == 13) {
            if (e.target.nodeName == 'INPUT' && e.target.type == 'text') {
                e.preventDefault();
                return false;
            }
        }
    }, true);

    let questionCount = 1;
    
    let addNameBtn = document.getElementById("addName");
    let addQuestionBtn = document.getElementById("addQuestion");

    if (addNameBtn) {

        addNameBtn.addEventListener("click", addName);
        addQuestionBtn.addEventListener("click", addQuestion)
    }

    function addName(event) {
        event.stopPropagation()

        let nameInput = document.getElementById("nameInput");
        let quiz = document.getElementById("quiz");
        let text = nameInput.value;

        displayQuizElement(quiz, "Name", text);
        document.getElementById("nameCard").style.display = "none";
        quiz.style.display = "block";
        let buttons = Array.from(document.getElementsByTagName("button"));

        buttons.filter(b => b.classList[0] === "btn")
               .map(b => b.classList.replace("btn-outline-primary", "btn-primary"));
    }

    function addQuestion() {
        event.stopPropagation()
        displayQuizElement(quiz, "Question " + questionCount++, "");
    }

    function displayQuizElement(quiz, labelText, text) {
        let template = document.getElementById("template");
        let element = template.cloneNode(true);
        element.getElementsByTagName("label")[0].textContent = labelText;
        quiz.appendChild(element);

        let input = element.getElementsByTagName("input")[0];
        input.value = text;
        element.style.display = "block";
    }

})();