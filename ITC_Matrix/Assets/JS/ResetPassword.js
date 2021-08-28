
$(document).ready(function () {
     var myRules = new Object();
     myRules["password"] = { required: true };
     myRules["ConfirmPassword"] = { required: true, equalTo: "#password" };

    var myMessages = new Object();
    myMessages["password"] = {
        required: " Please enter password.",
        minlength: " Password must be at least 8 characters long."
    };
    myMessages["ConfirmPassword"] = {
        required: " Please enter password.</span>",
        minlength: " Password must be at least 8 characters long.",
        equalTo: " Please enter the same password as above"
    };

    $("#aspnet").validate({
        rules: myRules,
        messages: myMessages
    });

    $("#password").blur(function () { $("#password").valid(); });
    $("#ConfirmPassword").blur(function () { $("#ConfirmPassword").valid(); });

    

});

function validateHTMLInput(key) {
    var keycode = (key.which) ? key.which : key.keyCode

    if ((keycode == 60 || keycode == 62 || keycode == 13)) {
        return false;
    }
    else {
        return true;
    }
}