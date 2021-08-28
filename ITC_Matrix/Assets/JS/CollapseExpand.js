function SlidUp(id) {
    var parentDivs = $(id).parent().find('section.formBox-content');
    parentDivs.slideUp();

    $(id).attr('onclick', '').unbind('click');
    $(id).attr('class', 'accordian-icon close');
    $(id).attr('onclick', 'SlidDown(this);').bind('click');
}

function SlidDown(id) {
    var parentDivs = $(id).parent().find('section.formBox-content');
    parentDivs.slideDown();

    $(id).attr('onclick', '').unbind('click');
    $(id).attr('class', 'accordian-icon open');
    $(id).attr('onclick', 'SlidUp(this);').bind('click');
}