/*
FileName : Defaults.js
FileFor : Managing the show and hide the controls.
Created By : Sandip Katore
Created Date : 29-jan-2016
Modified Date : 
Modified By : 
*/

$(document).ready(function () {   
    $('#tbl tr#ytd_month').hide();
    $('#selectedMonth,#Month,#themes').hide();
    // debug button show hide.
    $("#btndebugSave").click(function () {
        var ddldebug = $("#ddldebug").val();
        $("#value").val(ddldebug);
    });

    $("#btndebugCancel").click(function () {
        $("#dddlstebug").removeClass("customeFeild selectTag");
        $("#lbldebug").removeClass("displayNone");
        $("#ddldebug").addClass("displayNone");
        $("#btndebugSave").addClass("displayNone");
        $("#btndebugCancel").addClass("displayNone");
    });

    // for deepdebug show hide controls.
    $("#btndeepdebugSave").click(function () {
        var ddldeepdebug = $("#ddldeepdebug").val();
        $("#value").val(ddldeepdebug);
    });

    $("#btndeepdebugCancel").click(function () {
        $("#ddllstdeepdebug").removeClass("customeFeild selectTag");
        $("#lbldeepdebug").removeClass("displayNone");
        $("#ddldeepdebug").addClass("displayNone");
        $("#btndeepdebugSave").addClass("displayNone");
        $("#btndeepdebugCancel").addClass("displayNone");
    });

    // for  htmldocbaseurl show hide controls.
    $("#btndocbaseurlSave").click(function () {
        var btndocbaseurlSave = $("#txthtmldocbaseurl").val();
        $("#value").val(btndocbaseurlSave);
    });

    $("#btndocbaseurlCancel").click(function () {
        $("#ddllstldapUse").removeClass("customeFeild selectTag");
        $("#lblhtmldocbaseurl").removeClass("displayNone");
        $("#txthtmldocbaseurl").addClass("displayNone");
        $("#btndocbaseurlSave").addClass("displayNone");
        $("#btndocbaseurlCancel").addClass("displayNone");
    });

    // for  htmldocpath show hide controls
    $("#btnhtmldocpathSave").click(function () {
        var txthtmldocpath = $("#txthtmldocpath").val();
        $("#value").val(txthtmldocpath);
    });

    $("#btnhtmldocpathCancel").click(function () {
        $("#lblhtmldocpath").removeClass("displayNone");
        $("#txthtmldocpath").addClass("displayNone");
        $("#btnhtmldocpathSave").addClass("displayNone");
        $("#btnhtmldocpathCancel").addClass("displayNone");
    });


    // for  htmldocpdfstore show hide controls
    $("#btnhtmldocpdfstoreSave").click(function () {
        var txthtmldocpath = $("#txthtmldocpdfstore").val();
        $("#value").val(txthtmldocpath);
    });

    $("#btnhtmldocpdfstoreCancel").click(function () {
        $("#lblhtmldocpdfstore").removeClass("displayNone");
        $("#txthtmldocpdfstore").addClass("displayNone");
        $("#btnhtmldocpdfstoreSave").addClass("displayNone");
        $("#btnhtmldocpdfstoreCancel").addClass("displayNone");
    });

    // for  htmldocpdfstore show hide controls
    $("#btnhtmldocsiterootstoreSave").click(function () {
        var txthtmldocpathstore = $("#txthtmldocsiterootstore").val();
        $("#value").val(txthtmldocpathstore);
    });

    $("#btnhtmldocsiterootstoreCancel").click(function () {
        $("#lblhtmldocsiterootstore").removeClass("displayNone");
        $("#txthtmldocsiterootstore").addClass("displayNone");
        $("#btnhtmldocsiterootstoreSave").addClass("displayNone");
        $("#btnhtmldocsiterootstoreCancel").addClass("displayNone");
    });

    // for  ldapHost  show hide controls
    $("#btnldapHostSave").click(function () {
        var txtldapHost = $("#txtldapHost").val();
        $("#value").val(txtldapHost);
    });

    $("#btnldapHostCancel").click(function () {
        $("#lblldapHost").removeClass("displayNone");
        $("#txtldapHost").addClass("displayNone");
        $("#btnldapHostSave").addClass("displayNone");
        $("#btnldapHostCancel").addClass("displayNone");
    });

    // for  ldapPort  show hide controls
    $("#btnldapPortSave").click(function () {
        var txtldapPort = $("#txtldapPort").val();
        $("#value").val(txtldapPort);
    });

    $("#btnldapPortCancel").click(function () {
        $("#lblldapPort").removeClass("displayNone");
        $("#txtldapPort").addClass("displayNone");
        $("#btnldapPortSave").addClass("displayNone");
        $("#btnldapPortCancel").addClass("displayNone");
    });

    // for  ldapUse  show hide controls
    $("#btnldapUseSave").click(function () {
        var ddlldapUse = $("#ddlldapUse").val();
        $("#value").val(ddlldapUse);
    });

    $("#btnldapUseCancel").click(function () {
        $("#ddllstldapUse").removeClass("customeFeild selectTag");
        $("#lblldapUse").removeClass("displayNone");
        $("#ddlldapUse").addClass("displayNone");
        $("#btnldapUseSave").addClass("displayNone");
        $("#btnldapUseCancel").addClass("displayNone");
    });

    // for  matrix  verrsion   show hide controls
    $("#btnversionSave").click(function () {
        var txtversion = $("#txtversion").val();
        $("#value").val(txtversion);
    });

    $("#btnversionCancel").click(function () {
        $("#lblversion").removeClass("displayNone");
        $("#txtversion").addClass("displayNone");
        $("#btnversionSave").addClass("displayNone");
        $("#btnversionCancel").addClass("displayNone");
    });

    // for  matrix key show hide controls
    $("#btnmatrixKeySave").click(function () {
        var txtmatrixKey = $("#txtmatrixKey").val();
        $("#value").val(txtmatrixKey);
    });

    $("#btnmatrixKeyCancel").click(function () {
        $("#lblmatrixKey").removeClass("displayNone");
        $("#txtmatrixKey").addClass("displayNone");
        $("#btnmatrixKeySave").addClass("displayNone");
        $("#btnmatrixKeyCancel").addClass("displayNone");
    });

    // for  maxRows show hide controls
    $("#btnmaxRowsSave").click(function () {
        var txtmaxRows = $("#txtmaxRows").val();
        $("#value").val(txtmaxRows);
    });

    $("#btnmaxRowsCancel").click(function () {
        $("#lblmaxRows").removeClass("displayNone");
        $("#txtmaxRows").addClass("displayNone");
        $("#btnmaxRowsSave").addClass("displayNone");
        $("#btnmaxRowsCancel").addClass("displayNone");
    });



    // for  sessionIdle  show hide controls
    $("#btnsessionIdleSave").click(function () {
        var txtsessionIdle = $("#txtsessionIdle").val();
        $("#value").val(txtsessionIdle);
    });

    $("#btnsessionIdleCancel").click(function () {
        $("#lblsessionIdle").removeClass("displayNone");
        $("#txtsessionIdle").addClass("displayNone");
        $("#btnsessionIdleSave").addClass("displayNone");
        $("#btnsessionIdleCancel").addClass("displayNone");
    });

    // for  Master user
    $("#btnmasterUserSave").click(function () {
        var txtUser = $("#txtmasterUser").val();
        $("#value").val(txtUser);
    });

    $("#btnmasterUserCancel").click(function () {
        $("#lblmasterUser").removeClass("displayNone");
        $("#txtmasterUser").addClass("displayNone");
        $("#btnmasterUserSave").addClass("displayNone");
        $("#btnmasterUserCancel").addClass("displayNone");
    });

    // for  Session Name  show hide controls
    $("#btnsessionNameSave").click(function () {
        var txtsessionName = $("#txtsessionName").val();
        $("#value").val(txtsessionName);
    });

    $("#btnsessionNameCancel").click(function () {
        $("#lblsessionName").removeClass("displayNone");
        $("#txtsessionName").addClass("displayNone");
        $("#btnsessionNameSave").addClass("displayNone");
        $("#btnsessionNameCancel").addClass("displayNone");
    });

    // for  timer  show hide controls
    $("#btntimerSave").click(function () {
        var ddlltimer = $("#ddlltimer").val();
        $("#value").val(ddlltimer);
    });

    $("#btntimerCancel").click(function () {
        $("#ddlslttimer").removeClass("customeFeild selectTag");
        $("#lbltimer").removeClass("displayNone");
        $("#ddlltimer").addClass("displayNone");
        $("#btntimerSave").addClass("displayNone");
        $("#btntimerCancel").addClass("displayNone");
    });

    // for  Global menu  show hide controls
    $("#btnglobalMenusSave").click(function () {
        var txtglobalMenus = $("#txtglobalMenus").val();
        $("#value").val(txtglobalMenus);
    });

    $("#btnglobalMenusCancel").click(function () {
        $("#txtglobalReadOnly").removeClass("displayNone");
        $("#txtglobalMenus").addClass("displayNone");
        $("#btnglobalMenusSave").addClass("displayNone");
        $("#btnglobalMenusCancel").addClass("displayNone");
    });
    
    // for  Themes  show hide controls

    $('#btnthemesSave').click(function () {
        var selectedValue = $("#themes :selected").val();
        var selectDefault = $("#themes :selected").text();

        if (selectDefault == "Default Theme") {         
            $('#value').val('0');
        }
        else {
            $('#value').val(selectedValue);
        }
       
    });

    $("#btnthemesCancel").click(function () {
        $("#themes").hide();
        $("#ddlsltthemes").removeClass("customeFeild selectTag");
        $("#lblthemes").removeClass("displayNone");
        $("#themes").addClass("displayNone");
        $("#btnthemesSave").addClass("displayNone");
        $("#btnthemesCancel").addClass("displayNone");
    });

    // for  Default Language  show hide controls
    $('#btnlanguageSave').click(function () {
        var selectedValue = $("#ddlLanguages :selected").text();
        $('#value').val(selectedValue);
    });

    $("#btnlanguageCancel").click(function () {
        $("#ddlsltLanguages").removeClass("customeFeild selectTag");
        $("#lblLanguages").removeClass("displayNone");
        $("#ddlLanguages").addClass("displayNone");
        $("#btnlanguageSave").addClass("displayNone");
        $("#btnlanguageCancel").addClass("displayNone");
    });

    // firstDayOfWeek
    $('#btnweekDaysSave').click(function () {
        var selectedDay = $("#ddlweek :selected").val();
        $('#value').val(selectedDay);
    });

    $("#btnweekDaysCancel").click(function () {
        $("#ddllstWeekDays").removeClass("customeFeild selectTag");       
        $("#weekDays").removeClass("displayNone");
        //$("#lblLanguages").removeClass("displayNone");
        $("#ddlweek").addClass("displayNone");
        $("#btnweekDaysSave").addClass("displayNone");
        $("#btnweekDaysCancel").addClass("displayNone");
    });

    // For Masrter Role
    $('#btnmasterRoleSave').click(function () {
        var txtmasterRole = $("#txtmasterRole").val();
        $('#value').val(txtmasterRole);
    });

    $("#btnmasterRoleCancel").click(function () {
        $("#lblmasterRole").removeClass("displayNone");
        $("#txtmasterRole").addClass("displayNone");
        $("#btnmasterRoleSave").addClass("displayNone");
        $("#btnmasterRoleCancel").addClass("displayNone");
    });

    // Show Report Logo

    $('#btnreportLogoSave').click(function () {
        var selectedValue = $("#ddlReportLogo :selected").text();
        $('#value').val(selectedValue);
    });

    $("#btnreportLogoCancel").click(function () {
        $("#dddlstReportLogo").removeClass("customeFeild selectTag");
        $("#lblreportLogo").removeClass("displayNone");
        $("#ddlReportLogo").addClass("displayNone");
        $("#btnreportLogoSave").addClass("displayNone");
        $("#btnreportLogoCancel").addClass("displayNone");
    });

    // Report Form
    $('#btnreportsFromSave').click(function () {
        var selectedValue = $("txtreportsFrom").val();
        $('#value').val(selectedValue);
    });

    $("#btnreportsFromCancel").click(function () {
        $("#lblreportsFrom").removeClass("displayNone");
        $("#txtreportsFrom").addClass("displayNone");
        $("#btnreportsFromSave").addClass("displayNone");
        $("#btnreportsFromCancel").addClass("displayNone");
    });

    $('#btnlblyearstartSave').click(function () {
        var selectDate = $("#ddllstMonth :selected").text();
        var selectMonthName = $("#ddlMontName :selected").val();

        $('#value').val(selectDate);
        $('#monthName').val(selectMonthName);
    });

    $("#btnlblyearstartCancel").click(function () {
        $('#selectedMonth,#Month').hide();
        $("#ddllstMonth").removeClass("customeFeild selectTag");
        $("#ddllstMonthName").removeClass("customeFeild selectTag");

        $("#lblyearstart").removeClass("displayNone");
        $("#Month").addClass("displayNone");
        $("#ddlMontName").addClass("displayNone");
        $("#btnlblyearstartSave").addClass("displayNone").addClass("pink-btn");
        $("#btnlblyearstartCancel").addClass("displayNone").addClass("blue-btn");
    });

    //finding the table row id on click  edit_icon class
    $('tr .edit_icon').click(function () {
        var trid = $(this).closest('tr').attr('id'); // table row ID          
        $("#name").val(trid);

        if (trid == "debug") {
            $("#lbldebug").addClass("displayNone");
            $("#dddlstebug").addClass("customeFeild selectTag");
            $("#ddldebug").removeClass("displayNone");
            $("#btndebugSave").removeClass("displayNone").addClass("pink-btn");
            $("#btndebugCancel").removeClass("displayNone").addClass("blue-btn");
        }

        else if (trid == "deepdebug") {
            $("#lbldeepdebug").addClass("displayNone");
            $("#ddllstdeepdebug").addClass("customeFeild selectTag");
            $("#ddldeepdebug").removeClass("displayNone");
            $("#btndeepdebugSave").removeClass("displayNone").addClass("pink-btn");
            $("#btndeepdebugCancel").removeClass("displayNone").addClass("blue-btn");
        }

        else if (trid == "globalMenus") {
            $("#txtglobalReadOnly").addClass("displayNone");
            $("#txtglobalMenus").removeClass("displayNone");
            $("#btnglobalMenusSave").removeClass("displayNone").addClass("pink-btn");
            $("#btnglobalMenusCancel").removeClass("displayNone").addClass("blue-btn");
        }

        else if (trid == "htmldocbaseurl") {
            $("#lblhtmldocbaseurl").addClass("displayNone");
            $("#txthtmldocbaseurl").removeClass("displayNone");
            $("#btndocbaseurlSave").removeClass("displayNone").addClass("pink-btn");
            $("#btndocbaseurlCancel").removeClass("displayNone").addClass("blue-btn");
        }

        else if (trid == "htmldocpath") {
            $("#lblhtmldocpath").addClass("displayNone");
            $("#txthtmldocpath").removeClass("displayNone");
            $("#btnhtmldocpathSave").removeClass("displayNone").addClass("pink-btn");
            $("#btnhtmldocpathCancel").removeClass("displayNone").addClass("blue-btn");
        }

        else if (trid == "htmldocpdfstore") {
            $("#lblhtmldocpdfstore").addClass("displayNone");
            $("#txthtmldocpdfstore").removeClass("displayNone");
            $("#btnhtmldocpdfstoreSave").removeClass("displayNone").addClass("pink-btn");
            $("#btnhtmldocpdfstoreCancel").removeClass("displayNone").addClass("blue-btn");
        }

        else if (trid == "htmldocsiterootstore") {
            $("#lblhtmldocsiterootstore").addClass("displayNone");
            $("#txthtmldocsiterootstore").removeClass("displayNone");
            $("#btnhtmldocsiterootstoreSave").removeClass("displayNone").addClass("pink-btn");
            $("#btnhtmldocsiterootstoreCancel").removeClass("displayNone").addClass("blue-btn");
        }

            //--------------
        else if (trid == "langdefault") {
            $("#ddlsltLanguages").addClass("customeFeild selectTag");
            $("#lblLanguages").addClass("displayNone");
            $("#ddlLanguages").removeClass("displayNone");
            $("#btnlanguageSave").removeClass("displayNone").addClass("pink-btn");
            $("#btnlanguageCancel").removeClass("displayNone").addClass("blue-btn");
        }

            //else if (trid == "languages") {
            //    $("#lbldeepdebug").addClass("displayNone");
            //    $("#ddldeepdebug").removeClass("displayNone");
            //    $("#btndeepdebugSave").removeClass("displayNone");
            //    $("#btndeepdebugCancel").removeClass("displayNone");
            //}

        else if (trid == "ldapHost") {
            $("#lblldapHost").addClass("displayNone");
            $("#txtldapHost").removeClass("displayNone");
            $("#btnldapHostSave").removeClass("displayNone").addClass("pink-btn");
            $("#btnldapHostCancel").removeClass("displayNone").addClass("blue-btn");
        }

        else if (trid == "ldapPort") {
            $("#lblldapPort").addClass("displayNone");
            $("#txtldapPort").removeClass("displayNone");
            $("#btnldapPortSave").removeClass("displayNone").addClass("pink-btn");
            $("#btnldapPortCancel").removeClass("displayNone").addClass("blue-btn");
        }

        else if (trid == "ldapUse") {
            $("#ddllstldapUse").addClass("customeFeild selectTag");
            $("#lblldapUse").addClass("displayNone");
            $("#ddlldapUse").removeClass("displayNone");
            $("#btnldapUseSave").removeClass("displayNone").addClass("pink-btn");
            $("#btnldapUseCancel").removeClass("displayNone").addClass("blue-btn");
        }

        else if (trid == "matrixdb_version") {
            $("#lblversion").addClass("displayNone");
            $("#txtversion").removeClass("displayNone");
            $("#btnversionSave").removeClass("displayNone").addClass("pink-btn");
            $("#btnversionCancel").removeClass("displayNone").addClass("blue-btn");
        }

        else if (trid == "matrixKey") {
            $("#lblmatrixKey").addClass("displayNone");
            $("#txtmatrixKey").removeClass("displayNone");
            $("#btnmatrixKeySave").removeClass("displayNone").addClass("pink-btn");
            $("#btnmatrixKeyCancel").removeClass("displayNone").addClass("blue-btn");
        }

        else if (trid == "maxRows") {
            $("#lblmaxRows").addClass("displayNone");
            $("#txtmaxRows").removeClass("displayNone");
            $("#btnmaxRowsSave").removeClass("displayNone").addClass("pink-btn");
            $("#btnmaxRowsCancel").removeClass("displayNone").addClass("blue-btn");
        }

        else if (trid == "sessionIdle") {
            $("#lblsessionIdle").addClass("displayNone");
            $("#txtsessionIdle").removeClass("displayNone");
            $("#btnsessionIdleSave").removeClass("displayNone").addClass("pink-btn");
            $("#btnsessionIdleCancel").removeClass("displayNone").addClass("blue-btn");
        }

        else if (trid == "sessionName") {
            $("#lblsessionName").addClass("displayNone");
            $("#txtsessionName").removeClass("displayNone");
            $("#btnsessionNameSave").removeClass("displayNone").addClass("pink-btn");
            $("#btnsessionNameCancel").removeClass("displayNone").addClass("blue-btn");
        }

     

        else if (trid == "timer") {

            $("#ddlslttimer").addClass("customeFeild selectTag");
            $("#lbltimer").addClass("displayNone");
            $("#ddlltimer").removeClass("displayNone");
            $("#btntimerSave").removeClass("displayNone").addClass("pink-btn");
            $("#btntimerCancel").removeClass("displayNone").addClass("blue-btn");
        }

        else if (trid == "skincss") {
            $("#themes").show();
            $("#ddlsltthemes").addClass("customeFeild selectTag");
            $("#lblthemes").addClass("displayNone");
            $("#themes").removeClass("displayNone");
            $("#btnthemesSave").removeClass("displayNone").addClass("pink-btn");
            $("#btnthemesCancel").removeClass("displayNone").addClass("blue-btn");
        }
        else if (trid == "firstDayOfWeek") {
            $("#ddllstWeekDays").addClass("customeFeild selectTag");
            $("#weekDays").addClass("displayNone");
            $("#ddlweek").removeClass("displayNone");
            $("#btnweekDaysSave").removeClass("displayNone").addClass("pink-btn");
            $("#btnweekDaysCancel").removeClass("displayNone").addClass("blue-btn");
        }

        else if (trid == "Show Report Logo") {
            $("#dddlstReportLogo").addClass("customeFeild selectTag");
            $("#lblreportLogo").addClass("displayNone");
            $("#ddlReportLogo").removeClass("displayNone");
            $("#btnreportLogoSave").removeClass("displayNone").addClass("pink-btn");
            $("#btnreportLogoCancel").removeClass("displayNone").addClass("blue-btn");
        }

        else if (trid == "masterRole") {
            $("#lblmasterRole").addClass("displayNone");
            $("#txtmasterRole").removeClass("displayNone");
            $("#btnmasterRoleSave").removeClass("displayNone").addClass("pink-btn");
            $("#btnmasterRoleCancel").removeClass("displayNone").addClass("blue-btn");
        }

        else if (trid == "masterUser") {
            $("#lblmasterUser").addClass("displayNone");
            $("#txtmasterUser").removeClass("displayNone");
            $("#btnmasterUserSave").removeClass("displayNone").addClass("pink-btn");
            $("#btnmasterUserCancel").removeClass("displayNone").addClass("blue-btn");
        }

        else if (trid == "reportsFrom") {
            $("#lblreportsFrom").addClass("displayNone");
            $("#txtreportsFrom").removeClass("displayNone");
            $("#btnreportsFromSave").removeClass("displayNone").addClass("pink-btn");
            $("#btnreportsFromCancel").removeClass("displayNone").addClass("blue-btn");
        }

        else if (trid == "ytd_day") {
            $('#selectedMonth,#Month').show();
            $("#ddllstMonth").addClass("customeFeild selectTag");
            $("#ddllstMonthName").addClass("customeFeild selectTag");

            $("#lblyearstart").addClass("displayNone");
            $("#Month").removeClass("displayNone");
            $("#ddlMontName").removeClass("displayNone");
            $("#btnlblyearstartSave").removeClass("displayNone").addClass("pink-btn");
            $("#btnlblyearstartCancel").removeClass("displayNone").addClass("blue-btn");
        }

    });
});
