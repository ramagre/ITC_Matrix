$(function () {
    $("#ShiftRight,#ShiftLeft").click(function (event) {
        var ID = $(event.target).attr("ID");
        var ChooseFrom = ID == "ShiftRight" ? "#ChooseLeft" : "#ChooseRight";
        var moveTo = ID == "ShiftRight" ? "#ChooseRight" : "#ChooseLeft";
        var SelectData = $(ChooseFrom + " :selected").toArray();
        $(moveTo).append(SelectData);
        SelectData.remove;
    });
});

$(document).ready(function () {
    $("#btnSave").click(function () {
        var selectedprods = new Array();
        $("#ChooseRight option").each(function () {
            selectedprods.push($(this).val());
        });
        $("#selectedProductString").val(selectedprods);
    });
});