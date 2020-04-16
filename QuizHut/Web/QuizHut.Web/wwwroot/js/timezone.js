$(function () {
    const timezone = Intl.DateTimeFormat().resolvedOptions().timeZone;
    $('#timezone').val(timezone);
})