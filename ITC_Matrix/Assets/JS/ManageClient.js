$(document).ready(function () {
    var myRules = new Object();
    var myMessages = new Object();

    $("#aspnet").validate({
        rules: myRules,
        messages: myMessages
    });


    $('input[type="text"]').focusout(function () {
        var txtfldid = $(this).attr('id');
        var txtfldval = $('#' + txtfldid).val();
        if (txtfldval != "") {
            if ($.isNumeric($('#' + txtfldid).val()) == false) {

                $('#lblError').html("Please enter numeric value in text field.");
                return false;
            }
            {
                $('#lblError').html("");
            }
        }
        else {
            $('#lblError').html("");
        }
    });


    $('input[type="text"]').keydown(function (event) {
        if (event.shiftKey == true)
        {
            event.preventDefault();
        }

        if ((event.keyCode >= 48 && event.keyCode <= 57) ||
            (event.keyCode >= 96 && event.keyCode <= 105) ||
            event.keyCode == 8 || event.keyCode == 9 || event.keyCode == 37 ||
            event.keyCode == 39 || event.keyCode == 46) {

        } else
        {
            event.preventDefault();
        }

    });

    $("#btnSave").click(function () {
        $('input[type="text"]').each(function () {
            var txtfldid = $(this).attr('id');
            var txtfldval = $('#' + txtfldid).val();
            var hdnfldid = "hdn" + txtfldid;           
            $('#' + hdnfldid).val(txtfldval);
        });
    });

});
