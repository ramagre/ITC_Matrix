
/*For Editing Client Information */
$(document).ready(function () {
    $('.pink-btn').click(function () {
        var selectedprods = new Array();
        $('.departCode-box input:checked').each(function () {
            selectedprods.push($(this).attr('id'));          
        });
        $("#selectedAccCode").val(selectedprods);
    });
});
