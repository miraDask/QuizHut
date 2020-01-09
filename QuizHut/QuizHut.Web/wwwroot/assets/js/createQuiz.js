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

    if (addNameBtn) {

        addNameBtn.addEventListener("click", addName);
    }

    function addName(event) {
        event.stopPropagation()

        let nameInput = document.getElementById("nameInput");
        let input = document.getElementsByTagName("input")[0];
        let form = document.getElementById("quiz");

        let name = nameInput.value;
        input.value = name;

        document.getElementById("nameCard").style.display = "none";
        form.style.display = "block";
        //document.getElementById("addQuestion").addEventListener("click", addQuestion);
    }

    //function addQuestion(event) {
    //    event.stopPropagation();
    //    let questionText = document.getElementById("questionText").value;

    //    displayQuestion(questionText);
    //}


    function displayQuestion(text) {

    }
})();