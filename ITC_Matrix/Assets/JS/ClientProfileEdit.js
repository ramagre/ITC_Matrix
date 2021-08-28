/*
    FileFor : Javascript function for hide and show days and month details based on radio button selection
    File Name : ClientProfileEdit.js
    Created Date : 25-11-2015
    Created By : Sandip katore
    Modified Date :
*/


$(document).ready(function () {


    var resetFrequency = $("input[name='ResetFrequency']:checked").val();

    //function for showing selected radio button checked at page load
    GetSelectedValue(resetFrequency);

    //function for onclicked showing and hiding controls
    $("input:radio[name=ResetFrequency]").click(function () {

        var resetFrequency = $(this).val();        
        GetSelectedValue(resetFrequency);
        
    });

});

//JQuery Function for showing the control based on radio button value at page load
function GetSelectedValue(resetFrequency) {

    if (resetFrequency == 0) {      
        $("#weekDays").hide();
        $("#resetMonth").hide();
    }

    else if (resetFrequency == 1) {       
        $("#weekDays").show();
        $("#resetMonth").hide();
    }

    else if (resetFrequency == 2) {       
        $("#weekDays").hide();
        $("#resetMonth").show();
    }

}
