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

    $("#txtDOB").datepicker(
        {
            dateFormat: 'yy-mm-dd', 
            changeMonth: true,
            changeYear:true,
            maxDate: 'd',
            yearRange:"-100 : +@System.DateTime.Now.Year.ToString()"            
        }
        );

    $($("#dialog-confirm").attr('aria-describedby')).dialog('destroy');

    $(function () {
       
        $('.lnkDelete').click(function () {
            deleteLinkObj = $(this);        
            var message = "Are you sure you want to delete this record?";

            OpenDialog(deleteLinkObj, message);
            return false; // prevents the default behaviour
        });
    });        
});