$(document).ready(function () {

    $($("#dialog-confirm").attr('aria-describedby')).dialog('destroy');
   

    $(function () {

        $('.lnkDelete').click(function () {
            deleteLinkObj = $(this);
            var message = "Are you sure you want to delete this record?";

            OpenDialog(deleteLinkObj, message);
            return false; // prevents the default behaviour
        });
    });
    $(function () {

        $('.lnkReactive').click(function () {
            deleteLinkObj = $(this);
            var message = "Are you sure you want to Reactive this Client?";

            OpenDialog(deleteLinkObj, message);
            return false; // prevents the default behaviour
        });
    });
});

$(document).ready(function () {
    $('[data-toggle="tooltip"]').tooltip({
		using: function( position, feedback ) {
          $( this ).css( position );
          $( "<div>" )
            .addClass( "arrow" )
            .addClass( feedback.vertical )
            .addClass( feedback.horizontal )
            .appendTo( this );
        }
	});

});




