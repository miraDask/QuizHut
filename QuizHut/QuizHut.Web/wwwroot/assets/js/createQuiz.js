(function () {

    const LABEL = {
        QUESTION: "Question",
        ANSWER: "Answer",
        NAME: "Name"
    };

    const BUTTONS_NAMES = {
        ADD_NAME : "Add Name",
        ADD_QUESTION: "Add Question",
        ADD_NEW_QUESTION: "Add  New Question",
        ADD_ANSWER : "Add Answer"
    }
    
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
    
    let addNameBtn = document.getElementById("add");
    //let addQuestionBtn = document.getElementById("addQuestion");
    //let addAnswerBtn = document.getElementById("addAnswer");
    //let addImageBtn = document.getElementById("addImage");
    //let addTextFieldBtn = document.getElementById("addTextField");

    if (addNameBtn) {
        addNameBtn.addEventListener("click", addElementToQuiz);
        //addQuestionBtn.addEventListener("click", addQuestion);
        //addAnswerBtn.addEventListener("click", addAnswer);
        //addImageBtn.addEventListener("click", addImage);
        //addTextFieldBtn.addEventListener("click", addTextField);
    }

    function addElementToQuiz(event) {
        event.stopPropagation();
            let input = document.getElementById("nameInput");
            let quiz = document.getElementById("quiz");
            let text = input.value;

        if (addNameBtn.textContent === BUTTONS_NAMES.ADD_QUESTION) {
            addQuestion(quiz, text, input);

        } else if (addNameBtn.textContent === BUTTONS_NAMES.ADD_NAME) {
            addName(quiz, text, input);

        } else if (addNameBtn.textContent === BUTTONS_NAMES.ADD_ANSWER) {
            addAnswer(quiz, text, input);
        }
    }

    function addName(quiz, text, input) {
        let newElement = displayQuizElement(quiz, LABEL.NAME, text);
        quiz.style.display = "block";
      
        renderAddQuestionCard(input);
        window.scroll(0, findPos(newElement));
    }

    function addQuestion(quiz, text, input) {
        event.stopPropagation();
        let newElement = displayQuizElement(quiz, LABEL.QUESTION, text);
        window.scroll(0, findPos(newElement));
        renderAddAnswerCard(input);
    }

    function addAnswer(quiz, text, input) {
        event.stopPropagation();
        let newElement = displayQuizElement(quiz, LABEL.ANSWER, text);
        window.scroll(0, findPos(newElement));
        renderAddAnswerCard(input); 

    }

    function displayQuizElement(quiz, labelText, text) {
        let template = document.getElementById("template");
        let element = template.cloneNode(true);
        let checkbox = element.getElementsByTagName("input")[0].parentNode.parentNode;
        let label = element.getElementsByTagName("h4")[0];

        if (labelText === LABEL.NAME) {
            element.id = labelText.toLowerCase();
        }

        if (labelText === LABEL.ANSWER) {
            checkbox.style.display = "block";
            label.style.display = "none";
            element.getElementsByTagName("div")[0].style.display = "block";
        }

        if (labelText === LABEL.QUESTION) {
            if (label.style.display === "none") {
                label.style.display = "block";
                element.getElementsByTagName("div")[0].style.display = "none";
            }

            label.parentNode.className = "mt-5";
            labelText = labelText + " " + questionCount;
            element.id = LABEL.QUESTION.toLowerCase() + questionCount;
            questionCount++;
        }

        element.getElementsByTagName("label")[0].textContent = labelText;
        quiz.appendChild(element);
        let input = element.getElementsByTagName("input")[1];
        input.value = text;
        element.classList.replace("invisible", "visible");

        return element;
    }

    function renderAddQuestionCard(input) {
        document.getElementsByClassName("card-text")[1].style.display = "none";

        let secondBtn = document.getElementById("cancel");
        let firstCardTextElement = document.getElementsByClassName("card-text")[0];
        let secondCardTextElement = document.getElementsByClassName("card-text")[1];

        if (firstCardTextElement.style.display === "none") {
            firstCardTextElement.style.display = "block";
            secondCardTextElement.style.display = "none";
        }

        addNameBtn.parentNode.classList.replace("mx-4", "mx-1");
        addNameBtn.textContent = BUTTONS_NAMES.ADD_QUESTION;
        //secondBtn.style.display = "none";
        secondBtn.textContent = "Finish Quiz";
        secondBtn.removeAttribute("href");
        document.getElementsByClassName("card-title")[0].textContent = "Add New Question";
        input.value = "";
    }

    function renderAddAnswerCard(input) {
        let secondBtn = document.getElementById("cancel");
        let checkBox = document.getElementsByClassName("card-text")[1];

        addNameBtn.parentNode.classList.replace("mx-1", "mx-4");
        addNameBtn.textContent = BUTTONS_NAMES.ADD_ANSWER;
        secondBtn.style.display = "block";
        secondBtn.textContent = BUTTONS_NAMES.ADD_NEW_QUESTION;
        document.getElementsByClassName("card-title")[0].textContent = "Add New Answer";
        // document.getElementsByClassName("card-text")[0].style.display = "none";
        if (checkBox.checked) {
            checkBox.checked = false;
        }
        checkBox.style.display = "block";
        secondBtn.addEventListener("click", function (event) {
            event.stopPropagation();

            renderAddQuestionCard(input);
        });
        input.value = "";
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

})();