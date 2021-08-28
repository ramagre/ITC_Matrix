(function($){
"use strict";
	$(document).ready(function(){
	
	//$('#menu li.active').addClass('open').children('ul').show();
		$('#menu li.has-sub>a').on('click', function(){
			$(this).removeAttr('href');
			var element = $(this).parent('li');
			if (element.hasClass('open')) {
				element.removeClass('open');
				element.find('li').removeClass('open');
				element.find('ul').slideUp(200);
			}
			else {
				element.addClass('open');
				element.children('ul').slideDown(200);
				element.siblings('li').children('ul').slideUp(200);
				element.siblings('li').removeClass('open');
				element.siblings('li').find('li').removeClass('open');
				element.siblings('li').find('ul').slideUp(200);
			}
		});
	
	});
	
	$(window).load(function(){			
		// floating navigation				
		var menuPosition = $('.leftPanel-wrapper').offset();
		$(window).scroll(function(){
			if($(window).scrollTop() > menuPosition.top){
				$('.leftPanel-wrapper').css('position','fixed').css('top','0');
			}else {
				$('.leftPanel-wrapper').css('position','absolute');
			}
		});
		
		//auto scroll
		$(".leftPanel-wrapper, .departCode-boxScroll").mCustomScrollbar({
			autoHideScrollbar:true,
			theme:"minimal"
		});
		
		//popupbox
		$('.open-modal-2').click(function(e) {
			e.preventDefault();
			$('.mb-modal').mbModal({
				animation:'slideFade',
				animationspeed: 600,
				overlayClose: false,
				escClose: false
			});
		});
	});
	
	
})(jQuery);
