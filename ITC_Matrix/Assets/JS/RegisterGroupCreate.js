/*
   FileFor : Managing the RegisterGroup Model
   FileName : RegisterGroupCreate.js
   Created Date : 16-12-2015
   Created By : Sandip Katore
   Modified Date :17-12-2015
*/


$(document).ready(function () {

    //checked / Unchecked  all checkboxes on click checked selectAll chechbox
    $("#SelectAllDevice").on("click", function () {    
        if ($(this).is(':checked')) {
            $(this).closest("#DeviceCR").find(":checkbox").prop('checked', true);

        } else {
            $(this).closest("#DeviceCR").find(":checkbox").prop('checked', false);
        }
    });

    //checked / Unchecked  all checkboxes on click checked selectAll chechbox

    $("#SelectAllDepartment").on("click", function () {
        if ($(this).is(':checked')) {
            $(this).closest("#departmentCR").find(":checkbox").prop('checked', true);

        } else {
            $(this).closest("#departmentCR").find(":checkbox").prop('checked', false);
        }
    });

    // selecting all selected checkboxes ids
    $('.pink-btn').click(function () {
        var selectedDeviceCR = new Array();
        $('#DeviceCR input:checked').each(function () {
            selectedDeviceCR.push($(this).attr('id'));
        });
        $("#selectedDeviceCR").val(selectedDeviceCR);

        var selecteddepartmentCR = new Array();
        $('#departmentCR input:checked').each(function () {
            selecteddepartmentCR.push($(this).attr('id'));
        });
        $("#selecteddepartmentCR").val(selecteddepartmentCR);

    });

    DisplaySelectAllChecked();

    
});

function DisplaySelectAllChecked()
{
    var allDepartmentCheckboxes = $("#SelectAllDepartment").closest("#departmentCR").find(":checkbox").length;
    var allDepartmentSelectedCheckboxes = $("#SelectAllDepartment").closest("#departmentCR").find(":checkbox").filter(":checked").length;

    if (allDepartmentCheckboxes == parseInt(allDepartmentSelectedCheckboxes + 1)) {
        $("#SelectAllDepartment").prop('checked', true);
    }

    var allDeviceCheckboxes = $("#SelectAllDevice").closest("#DeviceCR").find(":checkbox").length;
    var allDeviceSelectedCheckboxes = $("#SelectAllDevice").closest("#DeviceCR").find(":checkbox").filter(":checked").length;

    if (allDeviceCheckboxes == parseInt(allDeviceSelectedCheckboxes + 1)) {
        $("#SelectAllDevice").prop('checked', true);
    }
}