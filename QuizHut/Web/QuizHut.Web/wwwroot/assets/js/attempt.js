//$(function () {
//    let attemptQuestionForm = document.getElementById("attemptQuestionForm");

//    if (attemptQuestionForm) {
//        let checkBoxes = $(":checkbox");
//        $(checkBoxes).toArray().forEach(cbox => {
//            $(cbox).val($(this).is(':checked'));
//            $(cbox).change(function () {
//                $(cbox).val($(this).is(':checked'));
//            });

//            $(cbox).mousedown(function () {
//                if (!$(this).is(':checked')) {
//                    $(this).trigger("change");
//                }
//            });
//        });
//    }
//})