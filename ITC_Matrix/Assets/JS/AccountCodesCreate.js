/*  
    File Name : AcountCodesCreate.js
    File For :Javascript function for selecting ids of the checkbox 
    Created Date : 18-11-2015
    created by : Sandip Katore
    Modified Date : 4-12-2015
*/
$(document).ready(function () {
    $('.pink-btn').click(function () {
        var selectedprods = new Array();
        $('.departCode-box input:checked').each(function () {
            selectedprods.push($(this).attr('id'));
        });
        $("#selectedProfiles").val(selectedprods);
    });
});