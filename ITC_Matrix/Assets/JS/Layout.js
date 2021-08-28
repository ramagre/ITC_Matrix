(function ($) {
    $(window).load(function () {
        // floating navigation
        var menuPosition = $('.leftPanel-wrapper').offset();
        $(window).scroll(function () {
            if ($(window).scrollTop() > menuPosition.top) {
                $('.leftPanel-wrapper').css('position', 'fixed').css('top', '0');
            } else {
                $('.leftPanel-wrapper').css('position', 'absolute');
            }
        });

        //auto scroll
        $(".leftPanel-wrapper").mCustomScrollbar({
            autoHideScrollbar: true,
            theme: "minimal"
        });
    });

})(jQuery);