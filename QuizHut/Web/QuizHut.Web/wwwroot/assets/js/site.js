$(function () {

    const ELEMENTS = {
        LINK : 'a',
        CLASSES : {
            READ : 'read',
        },
        BUTTONS : {
            READ_MORE : 'Read More',
            CLOSE : 'Close'
        }
    }

    let readMoreBtns = Array.from(document.getElementsByTagName(ELEMENTS.LINK)).filter(x => x.classList.contains(ELEMENTS.CLASSES.READ));
    $(readMoreBtns).click(loadInfo);

    function loadInfo() {
        var paragraph = $(this).parent().prev().children().last();
        var btnText = $(this).text();
        if (btnText === ELEMENTS.BUTTONS.READ_MORE) {
            $(paragraph).show();
            $(this).text(ELEMENTS.BUTTONS.CLOSE);
        } else {
            $(paragraph).hide();
            $(this).text(ELEMENTS.BUTTONS.READ_MORE);
        }
    }
})