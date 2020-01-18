(function () {
    const TITLES = {
        QUESTION: "Add New Question",
        ANSWER: "Add New Answer",
        DATE: "Add Activation Date",
        DURATION: "Duration"
    }

    const LABEL = {
        QUESTION: "Question",
        ANSWER: "Answer",
        NAME: "Name",
        DURATION: "Duration",
        ACTIVATION_DATE: "Actvation Date"
    };

    const BUTTONS_NAMES = {
        ADD_NAME: "Add Name",
        ADD_QUESTION: "Add Question",
        ADD_NEW_QUESTION: "Question / Finish",
        ADD_ANSWER: "Add Answer",
        FINISH: "Finish Quiz",
        ADD_DATE: "Add Date",
        ADD_DURATION: "Add Duration",
        NO: "No"
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
    let cardTitle = document.getElementsByClassName("card-title")[0];
    let continueBtn = document.getElementById("continue");
    let submitBtn = document.getElementById("activation");
    let firstCardTextElement = document.getElementsByClassName("card-text")[0];
    let secondCardTextElement = document.getElementsByClassName("card-text")[1];
    let input = document.getElementById("nameInput");
    let infoDiv = document.getElementById("info");

    if (addBtn) {
        addBtn.addEventListener("click", addElementToQuiz);
    }

    function addElementToQuiz(event) {
        event.stopPropagation();
        let quiz = document.getElementById("quiz");
        let text = input.value;

        if (addBtn.textContent === BUTTONS_NAMES.ADD_QUESTION) {
            addQuestion(quiz, text);

        } else if (addBtn.textContent === BUTTONS_NAMES.ADD_NAME) {
            addName(quiz, text);

        } else if (addBtn.textContent === BUTTONS_NAMES.ADD_ANSWER) {
            let checkBox = card.getElementsByTagName("input")[0];
            let isRightAnswer = checkBox.checked;

            addAnswer(quiz, text, isRightAnswer);
        } else if (addBtn.textContent === BUTTONS_NAMES.ADD_DATE) {
            addActivationDate(text);
        } else if (addBtn.textContent === BUTTONS_NAMES.ADD_DURATION) {
            addDuration(text);
        }
    }

    function addName(quiz, text) {
        let newElement = displayQuizElement(LABEL.NAME, text);
        quiz.appendChild(newElement);
        showElement(quiz);

        renderAddQuestionCard();
        window.scroll(0, findPos(newElement));
    }

    function addQuestion(quiz, text) {
        event.stopPropagation();
        let newElement = displayQuizElement(LABEL.QUESTION, text);
        newElement.getElementsByTagName("input")[1].type = "text";
        quiz.appendChild(newElement);
        window.scroll(0, findPos(newElement));
        renderAddAnswerCard();
    }

    function addAnswer(quiz, text, isRightAnswer) {
        event.stopPropagation();
        let newElement = displayQuizElement(LABEL.ANSWER, text);
        let checkbox = newElement.querySelector("input[type='checkbox']");

        if (isRightAnswer) {
            checkbox.setAttribute("checked", "");
        } else {
            checkbox.removeAttribute("checked");
        }

        quiz.lastChild.appendChild(newElement);
        window.scroll(0, findPos(newElement));
        renderAddAnswerCard();
    }

    function addActivationDate(text) {
        let newElement = displayQuizElement(LABEL.ACTIVATION_DATE, text);

        showElement(infoDiv);
        infoDiv.appendChild(newElement);
        newElement.getElementsByTagName("input")[1].type = "date";
        renderQuizDurationCard();
        window.scroll(0, findPos(card));
    }

    function addDuration(text) {
        let newElement = displayQuizElement(LABEL.DURATION, text);

        newElement.getElementsByTagName("input")[1].type = "text";
        infoDiv.appendChild(newElement);
        window.scroll(0, findPos(newElement));
        finishQuiz();
    }

    function displayQuizElement(labelText, text) {
        let template = document.getElementById("template");
        let element = template.cloneNode(true);
        let checkbox = element.getElementsByTagName("input")[0].parentNode.parentNode;
        let label = element.querySelector("h4");
        let nav = element.getElementsByTagName("nav")[0];

        if (labelText === LABEL.NAME) {
            element.id = labelText.toLowerCase();

        } else if (labelText === LABEL.ANSWER) {
            showElement(checkbox);
            hideElement(nav);

        } else if (labelText === LABEL.QUESTION) {
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

            showElement(nav);
            hideElement(checkbox);

            element.id = questionCount;
            questionCount++;
            navbarsCount++

        } else if (labelText === LABEL.ACTIVATION_DATE) {
            showElement(nav);
            hideElement(checkbox);

        } else if (labelText === LABEL.DURATION) {
            showElement(nav);
            hideElement(checkbox);
        }

        label.textContent = labelText;
        let input = element.getElementsByTagName("input")[1];
        input.value = text;
        element.classList.replace("invisible", "visible");

        return element;
    }

    function renderAddQuestionCard() {
        if (infoDiv.textContent === "") {
            secondBtn.addEventListener("click", renderActivationQuestionCard);

        } else {
            secondBtn.addEventListener("click", finishQuiz);
        }

        if (secondBtn.style.display === "none") {
            showElement(secondBtn);
        }

        hideElement(firstCardTextElement);

        if (firstCardTextElement.style.display === "none") {
            showElement(firstCardTextElement);
            hideElement(secondCardTextElement);
        }

        if (input.type === "date") {
            input.type = "text";
        }

        addBtn.parentNode.classList.replace("mx-4", "mx-1");
        addBtn.textContent = BUTTONS_NAMES.ADD_QUESTION;
        secondBtn.textContent = BUTTONS_NAMES.FINISH;
        secondBtn.removeAttribute("href");
        cardTitle.textContent = TITLES.QUESTION;
        input.value = "";
    }

    function renderAddAnswerCard() {

        let checkBox = secondCardTextElement.getElementsByTagName("input")[0];
        secondBtn.removeEventListener("click", renderActivationQuestionCard);
        secondBtn.removeEventListener("click", finishQuiz);

        if (secondBtn.style.display === "none") {
            showElement(secondBtn);
        }

        addBtn.parentNode.classList.replace("mx-1", "mx-4");
        addBtn.textContent = BUTTONS_NAMES.ADD_ANSWER;
        secondBtn.textContent = BUTTONS_NAMES.ADD_NEW_QUESTION;
        cardTitle.textContent = TITLES.ANSWER;

        if (checkBox.checked) {
            checkBox.checked = false;
        }

        showElement(secondCardTextElement)
        secondBtn.addEventListener("click", renderAddQuestionCard);

        input.value = "";
    }

    function renderActivationQuestionCard() {
        hideElement(submitBtn);
        hideElement(continueBtn);
        hideElement(secondBtn);

        if (firstCardTextElement.style.display === "none") {
            showElement(firstCardTextElement);
            hideElement(secondCardTextElement);
        }

        secondBtn.removeEventListener("click", renderActivationQuestionCard);
        secondBtn.textContent = BUTTONS_NAMES.NO;
        addBtn.textContent = BUTTONS_NAMES.ADD_DATE;
        cardTitle.textContent = TITLES.DATE;
        input.type = "date";
        showElement(card);
    }

    function renderQuizDurationCard() {
        addBtn.textContent = BUTTONS_NAMES.ADD_DURATION;
        cardTitle.textContent = TITLES.DURATION;
        input.type = "text";
    }

    function finishQuiz() {

        hideElement(card);
        showElement(submitBtn)
        showElement(continueBtn);

        continueBtn.addEventListener("click", continueQuiz)
    }

    function continueQuiz() {
        hideElement(submitBtn);
        hideElement(continueBtn);
        renderAddQuestionCard();
        showElement(card);
        window.scroll(0, findPos(card));
    }

    function addAnswerToCurrentQuestion(event) {
        let currentQuestionNumber = event.target.classList[3];
        let question = document.getElementById(currentQuestionNumber);
        let newElement = displayQuizElement(LABEL.ANSWER, "");
        question.appendChild(newElement);

    }

    function deleteCurrentQuestion(event) {
        let currentQuestionNumber = event.target.classList[3];
        let question = document.getElementById(currentQuestionNumber);
        question.remove();
        renderAddQuestionCard(null);
    }

    function hideElement(element) {
        element.style.display = "none";
    }

    function showElement(element) {
        element.style.display = "block";
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