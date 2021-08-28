$(document).ready(function () {

    //$("#Lang").change(function (e) {
    //    var languageID = $(this).val();
    //    var MessageID = $("#MessageID").val();
    //    $.ajax({
    //        url: "/Translation/GetTranslation/",
    //        data: { languageID: languageID, MessageID:MessageID },
    //        type: 'POST',           
    //        success: function (translation) {
    //            $("#DefaultMessage").val(translation);
    //        },
    //        error: function () {
    //        }
    //    });       
    //});

    $("#CustomMessage").prop('readonly', true);

    $("#UseCustom_bool").change(function () {

        if ($(this).is(":checked")) {
            $("#CustomMessage").removeAttr("readonly");
        }
        else {
            $("#CustomMessage").val('');
            $("#CustomMessage").prop('readonly', true);
        }
    });

    $("#btnSuggest").click(function (e) {
        var target = $("#target").val();       
        var Description = $("#DSCR").val();
        var newScript = document.createElement('script');
        alert(target);
        if (target != '' || target != undefined)
        {
            var source = 'https://www.googleapis.com/language/translate/v2?key=AIzaSyADL6QiY1-_pmXFRQNcAtytGAiGd2pBnmU&source=en&target=' + target + '&callback=translateText&q=' + Description;
            newScript.src = source;

            document.getElementsByTagName('head')[0].appendChild(newScript);
        }
        else {
            alert('Error in translation!');
        }
    });
});

function translateText(response) {    
    $("#DefaultMessage").val(response.data.translations[0].translatedText);
}