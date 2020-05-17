$(function () {
    var myForm = $('#form');

    $.data(myForm[0], 'validator').settings.ignore = "null";

    tinymce.init({
        selector: "textarea",
        setup: function (editor) {
            editor.on('keyUp', function () {
                tinyMCE.triggerSave();

                if (!$.isEmptyObject(myForm.validate().submitted))
                    myForm.validate().form();
            });
        },
        plugins: [
            "image paste table link code media"
        ],
    });
});