$(document).ready(function () {
    $('#input').keyup(function () {
        if ($(this).val() == '') {
            $('#search').prop('disabled', true);
        } else {
            $('#search').prop('disabled', false);
        }
    });

    $('#select').change(function () {
        if ($('#input').val()) {
            $('#search').prop('disabled', false);
        }

    });
});