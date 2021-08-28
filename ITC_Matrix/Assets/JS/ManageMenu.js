$(document).ready(function () {
    $(".accordian-title").find('input[type=checkbox][name=main]').each(function () {

        var main = $(this).attr('name');

        if(main==='main')
        {
            var childDiv = $(main).parent().find('ul');
            var childCheckboxes = $(childDiv).find('input[type=checkbox]');
            var lengthChildCheckboxes = $(childCheckboxes).length;
            var lengthChildSelected = $(childDiv).find('input[type=checkbox]').filter(":checked").length;
            
            //alert("all --- " + lengthChildCheckboxes + "   select    " + lengthChildSelected);
            if (lengthChildSelected != undefined && lengthChildCheckboxes != undefined) {
                if (parseInt(lengthChildCheckboxes) === parseInt(lengthChildSelected)) {
                    $(main).prop('checked', true);
                }
            }
        }      
    });
});

function SlidUp(id)
{    
    var parentDivs = $(id).parent().find('ul');            
    parentDivs.slideUp();

    $(id).attr('onclick', '').unbind('click');
    $(id).attr('class', 'accordian-icon close');
    $(id).attr('onclick', 'SlidDown(this);').bind('click');
}

function SlidDown(id) {
    var parentDivs = $(id).parent().find('ul');
    parentDivs.slideDown();

    $(id).attr('onclick', '').unbind('click');
    $(id).attr('class', 'accordian-icon open');
    $(id).attr('onclick', 'SlidUp(this);').bind('click');
}

function CheckMainMenu(id)
{
    var subMenuSection = $(id).parent().find('ul');
    var section = $(id).prev();
    var chkSubMenus = $(subMenuSection).find('input[type="checkbox"]');

    if (id.checked == false) {
        subMenuSection.slideUp();
       
        $(section).attr('onclick', '').unbind('click');
        $(section).attr('class', 'accordian-icon close');
        $(section).attr('onclick', 'SlidDown(this);').bind('click');

        if (chkSubMenus != undefined) {
            if (chkSubMenus.length > 0) {
                for (var i = 0; i < chkSubMenus.length; i++) {
                    chkSubMenus[i].checked = false;
                }
            }
        }
    }
   else if (id.checked == true) {
        subMenuSection.slideDown();
       
        $(section).attr('onclick', '').unbind('click');
        $(section).attr('class', 'accordian-icon open');
        $(section).attr('onclick', 'SlidUp(this);').bind('click');

        if (chkSubMenus != undefined) {
            if (chkSubMenus.length > 0) {
                for (var i = 0; i < chkSubMenus.length; i++) {
                    chkSubMenus[i].checked = true;
                }
            }
        }
    }
}

function CheckSubMenu(id, chkMainMenu) {

    var subMenuSection = $(id).closest('ul');    
    var chkSubMenus = $(subMenuSection).find('input[type="checkbox"]');
    var section = $(subMenuSection).parent().find('section');
    
    if (id.checked == false) {
        if (chkSubMenus != undefined) {
            if (chkSubMenus.length > 0) {
                if ((chkSubMenus.filter(':checked').length == 0)) {
                    subMenuSection.slideUp();

                    $(section).attr('class', 'accordian-icon close');
                    chkMainMenu.checked = false;
                }
            }
        }
    }
    else if (id.checked == true) {
        if (chkMainMenu.checked == false) {
            chkMainMenu.checked = true;
        }
    }
}
