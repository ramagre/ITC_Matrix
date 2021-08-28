$(document).ready(function () {

    //JQuery function for selecting the selected checkbox id
    $('.pink-btn').click(function () {
        var selectedprods = new Array();
        $('#permissionTask input:checked').each(function () {
            selectedprods.push($(this).attr('id'));
        });
        $("#selectedChechboxid").val(selectedprods);


        // code for selecting the selected device id's.
        var selectedprods = new Array();
        $("#ChooseRight option").each(function () {
            selectedprods.push($(this).val());
        });
        $("#selectedDeviceString").val(selectedprods);

        // code for selecting selected profile id's.
        var selectedprods = new Array();
        $("#ChooseProfileRight option").each(function () {
            selectedprods.push($(this).val());
        });
        $("#selectedProfileString").val(selectedprods);

        // code for selecting selected pay method id's.
        var selectedprods = new Array();
        $("#ChoosePayMethodRight option").each(function () {
            selectedprods.push($(this).val());
        });
        $("#selectedPayMethodString").val(selectedprods);

    });



    //jQuery function for switching the selectlist item from one list to other list for Device list.
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

    //jQuery function for switching the selectlist item from one list to other list for Profile list.
    $(function () {
        $("#ShiftProfileRight,#ShiftProfileLeft").click(function (event) {
            var ID = $(event.target).attr("ID");
            var ChooseFrom = ID == "ShiftProfileRight" ? "#ChooseProfileLeft" : "#ChooseProfileRight";
            var moveTo = ID == "ShiftProfileRight" ? "#ChooseProfileRight" : "#ChooseProfileLeft";
            var SelectData = $(ChooseFrom + " :selected").toArray();
            $(moveTo).append(SelectData);
            SelectData.remove;
        });
    });

    //jQuery function for switching the selectlist item from one list to other list for paymethod list.
    $(function () {
        $("#ShiftPayMethodRight,#ShiftPayMethodLeft").click(function (event) {
            var ID = $(event.target).attr("ID");
            var ChooseFrom = ID == "ShiftPayMethodRight" ? "#ChoosePayMethodLeft" : "#ChoosePayMethodRight";
            var moveTo = ID == "ShiftPayMethodRight" ? "#ChoosePayMethodRight" : "#ChoosePayMethodLeft";
            var SelectData = $(ChooseFrom + " :selected").toArray();
            $(moveTo).append(SelectData);
            SelectData.remove;
        });
    });
});