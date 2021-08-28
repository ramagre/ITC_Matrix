$(document).ready(function () {

    var deviceTypeID = $("#DeviceType").val();
    ShowHideControls(deviceTypeID);


    $("#ddlPriceLine1").on('change', function () {
        if (this.value == "1") {
            $("#lblprice1").html("$0.10");
        }
        else if (this.value == "2") {
            $("#lblprice1").html("$0.20");
        }
        else {
            $("#lblprice1").html("");
        }
    });
    $("#ddlPriceLine2").on('change', function () {
        if (this.value == "1") {
            $("#lblprice2").html("$0.10");
        }
        else if (this.value == "2") {
            $("#lblprice2").html("$0.20");
        }
        else {
            $("#lblprice2").html("");
        }
    });
    $("#ddlPriceLine3").on('change', function () {
        if (this.value == "1") {
            $("#lblprice3").html("$0.10");
        }
        else if (this.value == "2") {
            $("#lblprice3").html("$0.20");
        }
        else {
            $("#lblprice3").html("");
        }
    });
    $("#ddlPriceLine4").on('change', function () {
        if (this.value == "1") {
            $("#lblprice4").html("$0.10");
        }
        else if (this.value == "2") {
            $("#lblprice4").html("$0.20");
        }
        else {
            $("#lblprice4").html("");
        }
    });

    $(".pink-btn").click(function () {
        var selectedprods = new Array();
        var selectedtime = new Array();

        $("#lstTimeRight option").each(function () {
            selectedtime.push($(this).val());
        });
        $("#selectedTime").val(selectedtime);

        $('#divact input:checked').each(function () {
            selectedprods.push($(this).attr('id'));
        });
        $("#selectedProductString").val(selectedprods);
    });

});

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

$(function () {
    $("#ShiftTimeRight,#ShiftTimeLeft").click(function (event) {
        var ID = $(event.target).attr("ID");
        var ChooseFrom = ID == "ShiftTimeRight" ? "#lstTimeLeft" : "#lstTimeRight";
        var moveTo = ID == "ShiftTimeRight" ? "#lstTimeRight" : "#lstTimeLeft";
        var SelectData = $(ChooseFrom + " :selected").toArray();
        $(moveTo).append(SelectData);
        SelectData.remove;
    });
});

$(function () {
    $('#DeviceType').change(function () {
        var deviceTypeID = $(this).val();
        $("#lblError").removeClass("error-msg");

        if (deviceTypeID > 0 || deviceTypeID != "") {
            ShowHideControls(deviceTypeID);
        }
        else {
            $('#lblError').html("Please select model.");
            $("#lblError").addClass("error-msg");

            // hide all control section
            $('#secExtendedPassback').hide();
            $('#secOffline').hide();

            $('#secPriceLine').hide();
            $('#secModem').hide();
            $('#secPrintQueues').hide();

            return false;
        }
    });
});

function ShowHideControls(deviceTypeID) {
    $.ajax({
        url: "/Registers/GetDeviceClass/",
        type: 'POST',
        data: { id: deviceTypeID },
        success: function (deviceClass) {
            $('#lblError').html("");

            if (deviceClass == 0) {
                $('#lblError').html("Error occurred.");
            }
            else if (deviceClass == 1) {
                // show hide control section
                HideDiscount();

                $('#secExtendedPassback').show();
                $('#secOffline').show();
                $('#secPriceLine').hide();
                $('#secModem').hide();
                $('#secPrintQueues').hide();
            }
            else if (deviceClass == 2) {
                // show hide control section 
                HideDiscount();

                $('#secExtendedPassback').hide();
                $('#secOffline').hide();
                $('#secPriceLine').show();
                $('#secModem').hide();
                $('#secPrintQueues').hide();
            }
            else if (deviceClass == 3) {
                // show hide control section
                HideDiscount();

                $('#secExtendedPassback').show();
                $('#secOffline').hide();
                $('#secPriceLine').hide();
                $('#secModem').hide();
                $('#secPrintQueues').hide();
            }
            else if (deviceClass == 4) {
                // show hide control section
                ShowDiscount();

                $('#secExtendedPassback').show();
                $('#secOffline').hide();
                $('#secPriceLine').hide();
                $('#secModem').hide();
                $('#secPrintQueues').hide();
            }
            else if (deviceClass == 5) {
                // show hide control section
                HideDiscount();

                $('#secExtendedPassback').hide();
                $('#secOffline').hide();
                $('#secPriceLine').hide();
                $('#secModem').hide();
                $('#secPrintQueues').hide();
            }
            else if (deviceClass == 6) {
                // show hide control section
                HideDiscount();

                $('#secExtendedPassback').hide();
                $('#secOffline').hide();
                $('#secPriceLine').hide();
                $('#secModem').hide();
                $('#secPrintQueues').show();
            }
            else if (deviceClass == 7) {
                // show hide control section
                HideDiscount();

                $('#secExtendedPassback').hide();
                $('#secOffline').hide();
                $('#secPriceLine').show();
                $('#secModem').hide();
                $('#secPrintQueues').show();
            }
            else if (deviceClass == 8) {
                // show hide control section
                HideDiscount();

                $('#secExtendedPassback').hide();
                $('#secOffline').show();
                $('#secPriceLine').hide();
                $('#secModem').show();
                $('#secPrintQueues').hide();
            }
        },
        error: function () {
        }
    });
    return false;
}

function HideDiscount() {
    $('.departCode-row').each(function () {
        var secAccountDiscount = $(this).find('section#secAccountDiscount');
        var secDiscountLabel = $(this).find('section#secDiscountLabel');
        $(secAccountDiscount).hide();

        if (secDiscountLabel != undefined) {
            $(secDiscountLabel).hide();
        }
    });        // show discount %
}

function ShowDiscount() {
    $('.departCode-row').each(function () {
        var secAccountDiscount = $(this).find('section#secAccountDiscount');
        var secDiscountLabel = $(this).find('section#secDiscountLabel');
        $(secAccountDiscount).show();

        if (secDiscountLabel != undefined) {
            $(secDiscountLabel).show();
        }
    });        // show discount %      
}