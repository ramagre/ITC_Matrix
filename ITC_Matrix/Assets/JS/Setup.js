$(document).ready(function () {
   
    $("#btnSave").click(function () {
        ValidateForm();
    });
});

function ValidateForm() {
    $('input[type=text]').each(function () {
        var input = $(this);

        if (input != undefined) {
            var errorSpan = $(input).next("#errorSpan");

            if (errorSpan != undefined) {
                var error = $(errorSpan).find("span");

                if (error.val() != '') {
                    $(errorSpan).addClass("text-danger");
                }
                else {
                    $(errorSpan).removeClass("text-danger");
                }
            }
        }
    });
}