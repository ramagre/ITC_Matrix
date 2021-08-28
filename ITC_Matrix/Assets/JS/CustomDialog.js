function OpenDialog(deleteLinkObj, message) {

    $("#dialog-confirm #spnDialogMessage").html(message);

    $('#dialog-confirm').dialog({
        width: 400,
        resizable: false,
        modal: true,
        buttons: {
            "Continue": function (event) {
                event.preventDefault();
               
                $.post(deleteLinkObj[0].href, function (data) {  //Post to action                          
                    
                    window.location.href = window.location.href; 
                });
               
                $(this).dialog("close");
            },
            "Cancel": function () {
                $(this).dialog("close");
            }
        }
    });
}