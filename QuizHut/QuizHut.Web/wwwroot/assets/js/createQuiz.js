(function () {
    const TITLES = {
        QUESTION: "Add New Question",
        ANSWER: "Add New Answer"
    }

    const LABEL = {
        QUESTION: "Question",
        ANSWER: "Answer",
        NAME: "Name"
    };

    const BUTTONS_NAMES = {
        ADD_NAME: "Add Name",
        ADD_QUESTION: "Add Question",
        ADD_NEW_QUESTION: "Question / Finish",
        ADD_ANSWER: "Add Answer",
        FINISH: "Finish Quiz"
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
    let navbarsCount = 1;

    let addBtn = document.getElementById("add");
    let secondBtn = document.getElementById("cancel");
    let card = document.getElementById("nameCard");

    if (addBtn) {
        addBtn.addEventListener("click", addElementToQuiz);
    }

    function addElementToQuiz(event) {
        event.stopPropagation();
        let input = document.getElementById("nameInput");
        let quiz = document.getElementById("quiz");
        let text = input.value;

        if (addBtn.textContent === BUTTONS_NAMES.ADD_QUESTION) {
            addQuestion(quiz, text, input);

        } else if (addBtn.textContent === BUTTONS_NAMES.ADD_NAME) {
            addName(quiz, text, input);

        } else if (addBtn.textContent === BUTTONS_NAMES.ADD_ANSWER) {
            let checkBox = card.getElementsByTagName("input")[0];
            let isRightAnswer = checkBox.checked;

            addAnswer(quiz, text, input, isRightAnswer);
        }
    }

    function addName(quiz, text, input) {
        let newElement = displayQuizElement(quiz, LABEL.NAME, text);
        quiz.appendChild(newElement);
        quiz.style.display = "block";

        renderAddQuestionCard(input);
        window.scroll(0, findPos(newElement));
    }

    function addQuestion(quiz, text, input) {
        event.stopPropagation();
        let newElement = displayQuizElement(quiz, LABEL.QUESTION, text);
        quiz.appendChild(newElement);
        window.scroll(0, findPos(newElement));
        renderAddAnswerCard(input);
    }

    function addAnswer(quiz, text, input, isRightAnswer) {
        event.stopPropagation();
        let newElement = displayQuizElement(quiz, LABEL.ANSWER, text);

        if (isRightAnswer) {
            newElement.getElementsByTagName("input")[0].checked = true;
        }

        quiz.lastChild.appendChild(newElement);
        window.scroll(0, findPos(newElement));
        renderAddAnswerCard(input);
    }

    function displayQuizElement(quiz, labelText, text) {
        let template = document.getElementById("template");
        let element = template.cloneNode(true);
        let checkbox = element.getElementsByTagName("input")[0].parentNode.parentNode;
        let label = element.querySelector("h4");

        if (labelText === LABEL.NAME) {
            element.id = labelText.toLowerCase();
        }

        if (labelText === LABEL.ANSWER) {
            checkbox.style.display = "block";
            element.getElementsByTagName("nav")[0].style.display = "none";
        }

        if (labelText === LABEL.QUESTION) {
            let buttons = element.getElementsByTagName("a");
            let navBtn = element.querySelector("button");
            let navbarNavDropdown = element.querySelector("#navbarNavDropdown");
            let navId = navbarNavDropdown.id + navbarsCount;
            navbarNavDropdown.id = navId;

            navBtn.setAttribute("data-target", "#" + navId);
            navBtn.setAttribute("aria-controls", navId);

            navbarNavDropdown.classList.replace("invisible", "visible");
            navBtn.classList.replace("invisible", "visible");

            Array.from(buttons).forEach(b => b.classList.add(questionCount));

            buttons[0].addEventListener("click", addAnswerToCurrentQuestion);
            buttons[1].addEventListener("click", deleteCurrentQuestion);

            element.getElementsByTagName("nav")[0].style.display = "block";
            checkbox.style.display = "none";

            element.id = questionCount;
            questionCount++;
            navbarsCount++
        }

        label.textContent = labelText;
        let input = element.getElementsByTagName("input")[1];
        input.value = text;
        element.classList.replace("invisible", "visible");

        return element;
    }

    function renderAddQuestionCard(input) {
       
        secondBtn.addEventListener("click", finishQuiz);
        document.getElementsByClassName("card-text")[1].style.display = "none";

        let firstCardTextElement = document.getElementsByClassName("card-text")[0];
        let secondCardTextElement = document.getElementsByClassName("card-text")[1];

        if (firstCardTextElement.style.display === "none") {
            firstCardTextElement.style.display = "block";
            secondCardTextElement.style.display = "none";
        }

        addBtn.parentNode.classList.replace("mx-4", "mx-1");
        addBtn.textContent = BUTTONS_NAMES.ADD_QUESTION;
        secondBtn.textContent = BUTTONS_NAMES.FINISH;
        secondBtn.removeAttribute("href");
        document.getElementsByClassName("card-title")[0].textContent = TITLES.QUESTION;
        input.value = "";
    }

    function renderAddAnswerCard(input) {
        secondBtn.removeEventListener("click", finishQuiz);
        let cardTextForCheckBox = document.getElementsByClassName("card-text")[1];
        let checkBox = cardTextForCheckBox.getElementsByTagName("input")[0];

        addBtn.parentNode.classList.replace("mx-1", "mx-4");
        addBtn.textContent = BUTTONS_NAMES.ADD_ANSWER;
        secondBtn.textContent = BUTTONS_NAMES.ADD_NEW_QUESTION;
        document.getElementsByClassName("card-title")[0].textContent = TITLES.ANSWER;

        if (checkBox.checked) {
            checkBox.checked = false;
        }

        cardTextForCheckBox.style.display = "block";
        secondBtn.addEventListener("click", function (event) {
            event.stopPropagation();
            renderAddQuestionCard(input);
        });

        input.value = "";
    }

    function finishQuiz() {
        let continueBtn = document.getElementById("continue");
        let submitBtn = document.getElementById("submit");
        let quiz = document.getElementById("quiz");

        card.style.display = "none";
        quiz.appendChild(submitBtn);
        submitBtn.style.display = "block";
        continueBtn.style.display = "block";

        continueBtn.addEventListener("click", function () {
            continueBtn.style.display = "none";
            submitBtn.style.display = "none";
            card.style.display = "block";
        })

        //TODO: SUBMIT BTN 
    }

    function addAnswerToCurrentQuestion(event) {
        let currentQuestionNumber = event.target.classList[3];
        let question = document.getElementById(currentQuestionNumber);
        let newElement = displayQuizElement(null, LABEL.ANSWER, "");
        question.appendChild(newElement);

    }

    function deleteCurrentQuestion(event) {
        let currentQuestionNumber = event.target.classList[3];
        let question = document.getElementById(currentQuestionNumber);
        question.remove();
        renderAddQuestionCard(null);
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