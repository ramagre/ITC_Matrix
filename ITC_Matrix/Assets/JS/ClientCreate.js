/*  
    File Name : ClientCreate.js
    File For :
    Created Date : 18-11-2015
    created by : Sandip Katore
    Modified Date : 4-12-2015
*/

$(document).ready(function () {
    $(".fileError").html("");

    var selectedprods = new Array();
    $('.departCode-box input:checked').each(function () {
        selectedprods.push($(this).attr('id'));
    });

    

    //$("#ID_NO").change(function () {
    //    $.ajax({
    //        url: "/Clients/CheckClientID/",
    //        type: 'POST',
    //        data: { ID_NO: $('#ID_NO').val() },
    //        success: function (result) {

    //            if (result == false) {
    //                $('#lblError').html("Please Check ClientID length");
    //            }

    //            else {
    //                $('#lblError').html("");
    //            }
    //        },
    //        error: function () {
    //        }
    //    });
    //    return false;
    //});

});
