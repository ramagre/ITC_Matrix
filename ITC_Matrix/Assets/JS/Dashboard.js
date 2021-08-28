//var flag = true;
//$(document).ready(function () {      
//    $("body").click(function (event) {
//        flag = true;

//        var className = event.target.className;

//        if (!className.contains('dashInner-box'))
//        {
//            ClearAllFlips();
//        }           
//    });
//});

function ShowMenu(obj) {
    flag = false;

    ClearAllFlips();

    var menu = $(obj).find("section.dropMenus");

    if (menu != undefined) {
        $(menu).show();
    }
}

function ClearAllFlips() {
    // clear all other flips
    $(".mainTable-right").each(function () {
        var dropMenus = $(this).find("section.dropMenus");
        $(dropMenus).hide();
    });
}