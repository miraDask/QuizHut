(function () {

    
    let addNameBtn = document.getElementById("addName");
    
    if (addNameBtn) {
        

        addNameBtn.addEventListener("click", addName);
    }

    function addName(event) {
        event.stopPropagation()

        let input = document.getElementsByTagName("input")[0];

        let name = input.value;

        console.log(name);
    }
})();