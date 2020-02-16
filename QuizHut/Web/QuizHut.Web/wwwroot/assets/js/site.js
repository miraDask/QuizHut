$(function () {
    var readMoreBtns = Array.from(document.getElementsByTagName('a')).filter(x => x.classList.contains('read'));
    $(readMoreBtns).click(loadInfo);

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