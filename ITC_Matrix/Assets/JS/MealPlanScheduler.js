// Javascript function for toggle the class

$(document).ready(function () {
   

    $('input[type="text"]').keydown(function (event) {
        if (event.shiftKey == true) {
            event.preventDefault();
        }

        if ((event.keyCode >= 48 && event.keyCode <= 57) ||
            (event.keyCode >= 96 && event.keyCode <= 105) ||
            event.keyCode == 8 || event.keyCode == 9 || event.keyCode == 37 ||
            event.keyCode == 39 || event.keyCode == 46) {

        } else {
            event.preventDefault();
        }

    });

    //toggle class between checked and unchecked
    $('i').click(function () {
        var x = $(this).attr('class');
        if (x == 'fa fa-times') {
            $(this).removeClass('fa fa-times')
            $(this).toggleClass('fa fa-check');
        }
        else {
            $(this).removeClass('fa fa-check');
            $(this).toggleClass('fa fa-times');
        }
    });

    $("i").click(function () {
        var X = "hdn" + $(this).attr('id');
        var y = $(this).attr('class');
        
        if (y == "fa fa-times") {
            $('#' + X).val("0");
        }
        if (y == "fa fa-check") {
            $('#' + X).val("1");
        }
    });


    $("#totalamt").click(function () {
        $(":checkbox").each(function () {
            var chkfldid = $(this).attr('id');
            var chkfldval = $('#' + chkfldid).is(":checked");
            var hdnfldid = "hdn" + $(this).attr('id');
            //alert(chkfldval);
            if (chkfldval == true) {
                $('#' + hdnfldid).val("1");
            }
        });
    });

   

});