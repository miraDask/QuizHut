$(function () {
    const NAMES = {
        HOME: 'Home',
        GROUPS: 'Groups',
        QUIZZES: 'Quizzes',
        CATEGORIES: 'Categories',
        HASH: '#',
        SLASH: '/',
        MANAGE: 'Manage'
    }

    let readMoreBtns = Array.from(document.getElementsByTagName('a')).filter(x => x.classList.contains('read'));
    $(readMoreBtns).click(loadInfo);

    var navLinks = Array.from(document.getElementsByTagName('a')).filter(x => x.classList.contains('header'));
    if (navLinks) {
        let pageUrl = window.location.href;
        if (pageUrl.includes(NAMES.QUIZZES)) {
            activateNavLink(navLinks[2]);
        } else if (pageUrl.includes(NAMES.MANAGE)) {
            activateNavLink(navLinks[4]);
        } else if (pageUrl.includes(NAMES.CATEGORIES)) {
            activateNavLink(navLinks[1]);
        } else if (pageUrl.includes(NAMES.GROUPS)) {
            activateNavLink(navLinks[3]);
        } else {
            activateNavLink(navLinks[0]);
        }
    }

    function activateNavLink(element) {
        element.classList.add('active');
    }

    function loadInfo() {
        var paragraph = $(this).parent().prev().children().last();
        var btnText = $(this).text();
        if (btnText === 'Read More') {
            $(paragraph).show();
            $(this).text('Close');
        } else {
            $(paragraph).hide();
            $(this).text('Read More');
        }
    }
})