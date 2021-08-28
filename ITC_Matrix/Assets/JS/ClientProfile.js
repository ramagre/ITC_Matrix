/*
    FileFor : Javascript function for hide and show days and month details based on radio button selection
    File Name : ClientProfile.js
    Created Date : 24-11-2015
    Created By : Sandip katore
    Modified Date :
*/

$(document).ready(function () {    

    $("#weekDays,#resetMonth").each(function () {
        $(this).hide();
    });

    $("#Weekly").click(function () {
        $("#weekDays").show();
        $("#resetMonth").hide();
    });

    $("#Monthly").click(function () {
        $("#resetMonth").show();
        $("#weekDays").hide();
    });

    $("#Daily").click(function () {
        $("#resetMonth").hide();
        $("#weekDays").hide();
    });
});