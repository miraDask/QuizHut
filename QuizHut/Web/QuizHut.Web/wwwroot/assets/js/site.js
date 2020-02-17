$(function () {
    const URLPART = {
        GROUPS: 'Groups',
        QUIZZES: 'Quizzes',
        CATEGORIES: 'Categories',
        MANAGE: 'Manage'
    }

    const ELEMENTS = {
        LINK : 'a',
        CLASSES : {
            READ : 'read',
            HEADER : 'header',
            ACTIVE : 'active'
        },
        BUTTONS : {
            READ_MORE : 'Read More',
            CLOSE : 'Close'
        }
    }

    let readMoreBtns = Array.from(document.getElementsByTagName(ELEMENTS.LINK)).filter(x => x.classList.contains(ELEMENTS.CLASSES.READ));
    $(readMoreBtns).click(loadInfo);

    var navLinks = Array.from(document.getElementsByTagName(ELEMENTS.LINK)).filter(x => x.classList.contains(ELEMENTS.CLASSES.HEADER));
    if (navLinks) {
        let pageUrl = window.location.href;
        if (pageUrl.includes(URLPART.QUIZZES)) {
            activateNavLink(navLinks[2]);
        } else if (pageUrl.includes(URLPART.MANAGE)) {
            activateNavLink(navLinks[4]);
        } else if (pageUrl.includes(URLPART.CATEGORIES)) {
            activateNavLink(navLinks[1]);
        } else if (pageUrl.includes(URLPART.GROUPS)) {
            activateNavLink(navLinks[3]);
        } else {
            activateNavLink(navLinks[0]);
        }
    }

    function activateNavLink(element) {
        element.classList.add(ELEMENTS.CLASSES.ACTIVE);
    }

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