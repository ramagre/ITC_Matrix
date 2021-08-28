$(document).ready(function () {

    $("#EXPIRY_DATE").datepicker(
        {
            changeMonth: true,
            changeYear: true,
            minDate: 'd'
        });
    $("#START_DATE").datepicker(
   {
       changeMonth: true,
       changeYear: true,
       minDate: 'd'
   });

    $("#secDeviceType").css("display", "none");
    $("#secDeviceGroup").css("display", "block");

    //checked / Unchecked  for profile
    $("#chkselectall").on("click", function () {
        if ($(this).is(':checked')) {
            $(this).closest("#secProfile").find(":checkbox").prop('checked', true);

        } else {
            $(this).closest("#secProfile").find(":checkbox").prop('checked', false);
        }
    });

    //toggle group and type
    $("#dgroupsTwo").on("click", function () {
        $(this).attr('class', 'tr first active');

        DisplayGroup(this);

        var dtypesTwo = $(this).parent().find('li[id$=dtypesTwo]');

        if (dtypesTwo != undefined) {
            $(dtypesTwo).attr('class', 'tr last');
        }
    });

    //toggle group and type
    $("#dtypesTwo").on("click", function () {
        $(this).attr('class', 'tr last active');

        DisplayType(this);

        var dgroupsTwo = $(this).parent().find('li[id$=dgroupsTwo]');

        if (dgroupsTwo != undefined) {
            $(dgroupsTwo).attr('class', 'tr first');
        }
    });

    //checked / Unchecked  for device type
    $("#DeviceTypeAll").on("click", function () {
        if ($(this).is(':checked')) {
            $(this).closest("#secDeviceType").find(":checkbox").prop('checked', true);

        } else {
            $(this).closest("#secDeviceType").find(":checkbox").prop('checked', false);
        }
    });

    //checked / Unchecked  for device group
    $("#DeviceGroupAll").on("click", function () {
        if ($(this).is(':checked')) {
            $(this).closest("#secDeviceGroup").find(":checkbox").prop('checked', true);

        } else {
            $(this).closest("#secDeviceGroup").find(":checkbox").prop('checked', false);
        }
    });

});

function DisplayGroup(obj) {
    var secDeviceType = $(obj).parent().parent().parent().find('section[id$=secDeviceType]');
    var secDeviceGroup = $(obj).parent().parent().parent().find('section[id$=secDeviceGroup]');

    if (secDeviceType != undefined && secDeviceGroup != undefined) {
        $(secDeviceType).css("display", "none");
        $(secDeviceGroup).css("display", "block");
    }
}

function DisplayType(obj) {
    var secDeviceType = $(obj).parent().parent().parent().find('section[id$=secDeviceType]');
    var secDeviceGroup = $(obj).parent().parent().parent().find('section[id$=secDeviceGroup]');

    if (secDeviceType != undefined && secDeviceGroup != undefined) {
        $(secDeviceType).css("display", "block");
        $(secDeviceGroup).css("display", "none");
    }
}

function CheckDevices(obj) {
    if ($(obj).is(':checked')) {
        $(obj).closest("#secModel").find(":checkbox").prop('checked', true);

    } else {
        $(obj).closest("#secModel").find(":checkbox").prop('checked', false);
    }
}

function CheckGroup(obj) {
    if ($(obj).is(':checked')) {
        $(obj).closest("#secGroup").find(":checkbox").prop('checked', true);

    } else {
        $(obj).closest("#secGroup").find(":checkbox").prop('checked', false);
    }
}