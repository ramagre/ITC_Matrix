/*
FileName : Common.js
FileFor : All Common Client Side Functions.
Created By : Sandip Katore
Create Date : 31-12-2015
Modified Date: 
*/
$(document).ready(function () {  
    //jQuery Function for preventing to enter special characters
    $("input[type=text],#COMMENT").keypress(function (key) {
        var keycode = (key.which) ? key.which : key.keyCode;
        if ((keycode == 60 || keycode == 62)) {
            return false;
        }
        else {
            return true;
        }
    });

    //function for validate the uploded files types
    $(".fileError").html("");
    $(".fileError").removeClass("text-danger");

    $('INPUT[type="file"]').on('change', function (e) {
        $(".fileError").hide();
        var ext = this.value.match(/\.(.+)$/)[1];
        switch (ext) {
            case 'jpg':
            case 'jpeg':
            case 'png':
            case 'gif':
            case 'jif':
                $('#uploadedImage').attr('disabled', false);
                break;
            default:
                $(".fileError").show();
                $(".fileError").html("Upload images of type png,jpeg,jpg.");
                $(".fileError").addClass("text-danger");
                e.preventDefault();             
        }
    });
});

