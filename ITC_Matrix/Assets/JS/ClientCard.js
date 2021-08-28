$(document).ready(function () {
    $("#Expiry_Date").datepicker(
    {
        changeMonth: true,
        changeYear: true,
        minDate: 'd',
        dateFormat: 'yy/mm/dd'
    });

    $('#btnSetPin').click(function () {
        $("#confirmpin").toggle();
        $("#pin").toggle();
        $("#pinbutton").toggle();
    });

    $($("#dialog-confirm").attr('aria-describedby')).dialog('destroy');

    $(function () {

        $('.lnkCancel').click(function () {            
            deleteLinkObj = $(this);
            var message = "Are you sure you want to cancel this record?";

            OpenDialog(deleteLinkObj, message);
            return false; // prevents the default behaviour
        });
    });
});
