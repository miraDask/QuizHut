(function () {

    const LABEL = {
        QUESTION: "Question",
        ANSWER: "Answer",
        NAME: "Name"
    };
    
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
    let addAnswerBtn = document.getElementById("addAnswer");
    let addImageBtn = document.getElementById("addImage");
    let addTextFieldBtn = document.getElementById("addTextField");

    if (addNameBtn) {

        addNameBtn.addEventListener("click", addName);
        addQuestionBtn.addEventListener("click", addQuestion);
        addAnswerBtn.addEventListener("click", addAnswer);
        addImageBtn.addEventListener("click", addImage);
        addTextFieldBtn.addEventListener("click", addTextField);
    }

    function addName(event) {
        event.stopPropagation()

        let nameInput = document.getElementById("nameInput");
        let quiz = document.getElementById("quiz");
        let text = nameInput.value;

        let newElement = displayQuizElement(quiz, LABEL.NAME, text);
        //document.getElementById("nameCard").style.display = "none";
        quiz.style.display = "block";
        let buttons = Array.from(document.getElementsByTagName("button"));

        buttons.filter(b => b.classList[0] === "btn")
            .map(b => b.classList.replace("btn-outline-primary", "btn-primary"));

        window.scroll(0, findPos(newElement));

    }

    function findPos(obj) {
        var curtop = 0;
        if (obj.offsetParent) {
            do {
                curtop += obj.offsetTop;
            } while (obj = obj.offsetParent);
            return [curtop];
        }
    }

    function addQuestion() {
        event.stopPropagation()
        let newElement = displayQuizElement(quiz, LABEL.QUESTION, "");
        window.scroll(0, findPos(newElement));
    }

    function addAnswer() {
        event.stopPropagation()
        let newElement = displayQuizElement(quiz, LABEL.ANSWER, "");
        window.scroll(0, findPos(newElement));
    }

    function displayQuizElement(quiz, labelText, text) {
        let template = document.getElementById("template");
        let element = template.cloneNode(true);
        let checkbox = element.getElementsByTagName("input")[0].parentNode.parentNode;

        if (labelText === LABEL.NAME) {
            element.id = labelText.toLowerCase();
        }

        if (labelText === LABEL.ANSWER) {
            checkbox.style.display = "block";
            element.getElementsByTagName("h4")[0].style.display = "none";
        }

        if (labelText === LABEL.QUESTION) {
            labelText = labelText + " " + questionCount;
            element.id = labelText.toLowerCase() + questionCount;
            questionCount++;
        }

        element.getElementsByTagName("label")[0].textContent = labelText;
        quiz.appendChild(element);

        let input = element.getElementsByTagName("input")[1];
        input.value = text;
        element.style.display = "block";

        return element;
    }

})();