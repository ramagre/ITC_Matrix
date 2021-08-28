$(document).ready(function () {       
    $('#colorSelectorSiteBackground').ColorPicker({
        color: '#171A29',

        onShow: function (colpkr) {
            $(colpkr).fadeIn(500);
            return false;
        },
        onHide: function (colpkr) {
            $(colpkr).fadeOut(500);
            return false;
        },
        onChange: function (hsb, hex, rgb) {
            $('#colorSelectorSiteBackground div').css('backgroundColor', '#' + hex);
            $("#lblSiteBackgroundColor").text('#' + hex);
            $("#SiteBackgroundColor").val('#' + hex);
        }
    });

    $('#colorSelectorHeaderBackgroundColor').ColorPicker({
        color: '#171A29',

        onShow: function (colpkr) {
            $(colpkr).fadeIn(500);
            return false;
        },
        onHide: function (colpkr) {
            $(colpkr).fadeOut(500);
            return false;
        },
        onChange: function (hsb, hex, rgb) {
            $('#colorSelectorHeaderBackgroundColor div').css('backgroundColor', '#' + hex);
            $("#lblHeaderBackgroundColor").text('#' + hex);
            $("#HeaderBackgroundColor").val('#' + hex);
        }
    });

    $('#colorSelectorMenuBackgroundColor').ColorPicker({
        color: '#171A29',

        onShow: function (colpkr) {
            $(colpkr).fadeIn(500);
            return false;
        },
        onHide: function (colpkr) {
            $(colpkr).fadeOut(500);
            return false;
        },
        onChange: function (hsb, hex, rgb) {
            $('#colorSelectorMenuBackgroundColor div').css('backgroundColor', '#' + hex);
            $("#lblMenuBackgroundColor").text('#' + hex);
            $("#MenuBackgroundColor").val('#' + hex);
        }
    });

    $('#colorSelectorSubmenuBackgroundColor').ColorPicker({
        color: '#171A29',

        onShow: function (colpkr) {
            $(colpkr).fadeIn(500);
            return false;
        },
        onHide: function (colpkr) {
            $(colpkr).fadeOut(500);
            return false;
        },
        onChange: function (hsb, hex, rgb) {
            $('#colorSelectorSubmenuBackgroundColor div').css('backgroundColor', '#' + hex);
            $("#lblSubmenuBackgroundColor").text('#' + hex);
            $("#SubmenuBackgroundColor").val('#' + hex);
        }
    });

    $('#colorSelectorFormControlFontColor').ColorPicker({
        color: '#171A29',

        onShow: function (colpkr) {
            $(colpkr).fadeIn(500);
            return false;
        },
        onHide: function (colpkr) {
            $(colpkr).fadeOut(500);
            return false;
        },
        onChange: function (hsb, hex, rgb) {
            $('#colorSelectorFormControlFontColor div').css('backgroundColor', '#' + hex);
            $("#lblFormControlFontColor").text('#' + hex);
            $("#FormControlFontColor").val('#' + hex);
        }
    });

    $('#colorSelectorFormControlBordercolor').ColorPicker({
        color: '#171A29',

        onShow: function (colpkr) {
            $(colpkr).fadeIn(500);
            return false;
        },
        onHide: function (colpkr) {
            $(colpkr).fadeOut(500);
            return false;
        },
        onChange: function (hsb, hex, rgb) {
            $('#colorSelectorFormControlBordercolor div').css('backgroundColor', '#' + hex);
            $("#lblFormControlBorderColor").text('#' + hex);
            $("#FormControlBorderColor").val('#' + hex);
        }
    });

    $('#colorSelectorGridHeaderBackgroundcolor').ColorPicker({
        color: '#171A29',

        onShow: function (colpkr) {
            $(colpkr).fadeIn(500);
            return false;
        },
        onHide: function (colpkr) {
            $(colpkr).fadeOut(500);
            return false;
        },
        onChange: function (hsb, hex, rgb) {
            $('#colorSelectorGridHeaderBackgroundcolor div').css('backgroundColor', '#' + hex);
            $("#lblGridHeaderBackgroundColor").text('#' + hex);
            $("#GridHeaderBackgroundColor").val('#' + hex);
        }
    });

    $('#colorSelectorGridFooterBackgroundcolor').ColorPicker({
        color: '#171A29',

        onShow: function (colpkr) {
            $(colpkr).fadeIn(500);
            return false;
        },
        onHide: function (colpkr) {
            $(colpkr).fadeOut(500);
            return false;
        },
        onChange: function (hsb, hex, rgb) {
            $('#colorSelectorGridFooterBackgroundcolor div').css('backgroundColor', '#' + hex);
            $("#lblGridFooterBackgroundColor").text('#' + hex);
            $("#GridFooterBackgroundColor").val('#' + hex);
        }
    });

    $('#colorSelectorGridRowBackgroundcolor').ColorPicker({
        color: '#171A29',

        onShow: function (colpkr) {
            $(colpkr).fadeIn(500);
            return false;
        },
        onHide: function (colpkr) {
            $(colpkr).fadeOut(500);
            return false;
        },
        onChange: function (hsb, hex, rgb) {
            $('#colorSelectorGridRowBackgroundcolor div').css('backgroundColor', '#' + hex);
            $("#lblGridRowBackgroundColor").text('#' + hex);
            $("#GridRowBackgroundColor").val('#' + hex);
        }
    });

    $('#colorSelectorGridRowAlternateBackgroundcolor').ColorPicker({
        color: '#171A29',

        onShow: function (colpkr) {
            $(colpkr).fadeIn(500);
            return false;
        },
        onHide: function (colpkr) {
            $(colpkr).fadeOut(500);
            return false;
        },
        onChange: function (hsb, hex, rgb) {
            $('#colorSelectorGridRowAlternateBackgroundcolor div').css('backgroundColor', '#' + hex);
            $("#lblGridRowAlternateBackgroundColor").text('#' + hex);
            $("#GridRowAlternateBackgroundColor").val('#' + hex);
        }
    });

    $('#colorSelectorHeaderFontStyleFontColor').ColorPicker({
        color: '#171A29',

        onShow: function (colpkr) {
            $(colpkr).fadeIn(500);
            return false;
        },
        onHide: function (colpkr) {
            $(colpkr).fadeOut(500);
            return false;
        },
        onChange: function (hsb, hex, rgb) {
            $('#colorSelectorHeaderFontStyleFontColor div').css('backgroundColor', '#' + hex);
            $("#lblHeaderFontStyleFontColor").text('#' + hex);
            $("#HeaderFontStyleFontColor").val('#' + hex);
        }
    });

    $('#colorSelectorMenuFontStyleFontColor').ColorPicker({
        color: '#c4c4c4',

        onShow: function (colpkr) {
            $(colpkr).fadeIn(500);
            return false;
        },
        onHide: function (colpkr) {
            $(colpkr).fadeOut(500);
            return false;
        },
        onChange: function (hsb, hex, rgb) {
            $('#colorSelectorMenuFontStyleFontColor div').css('backgroundColor', '#' + hex);
            $("#lblMenuFontStyleFontColor").text('#' + hex);
            $("#MenuFontStyleFontColor").val('#' + hex);
        }
    });

    $('#colorSelectorSubmenuFontStyleFontColor').ColorPicker({
        color: '#c4c4c4',

        onShow: function (colpkr) {
            $(colpkr).fadeIn(500);
            return false;
        },
        onHide: function (colpkr) {
            $(colpkr).fadeOut(500);
            return false;
        },
        onChange: function (hsb, hex, rgb) {
            $('#colorSelectorSubmenuFontStyleFontColor div').css('backgroundColor', '#' + hex);
            $("#lblSubmenuFontStyleFontColor").text('#' + hex);
            $("#SubmenuFontStyleFontColor").val('#' + hex);
        }
    });

    $('#colorSelectorFormHeaderFontStyleFontColor').ColorPicker({
        color: '#c4c4c4',

        onShow: function (colpkr) {
            $(colpkr).fadeIn(500);
            return false;
        },
        onHide: function (colpkr) {
            $(colpkr).fadeOut(500);
            return false;
        },
        onChange: function (hsb, hex, rgb) {
            $('#colorSelectorFormHeaderFontStyleFontColor div').css('backgroundColor', '#' + hex);
            $("#lblFormHeaderFontStyleFontColor").text('#' + hex);
            $("#FormHeaderFontStyleFontColor").val('#' + hex);
        }
    });

    $('#colorSelectorFormLableFontStyleFontColor').ColorPicker({
        color: '#c4c4c4',

        onShow: function (colpkr) {
            $(colpkr).fadeIn(500);
            return false;
        },
        onHide: function (colpkr) {
            $(colpkr).fadeOut(500);
            return false;
        },
        onChange: function (hsb, hex, rgb) {
            $('#colorSelectorFormLableFontStyleFontColor div').css('backgroundColor', '#' + hex);
            $("#lblFormLableFontStyleFontColor").text('#' + hex);
            $("#FormLableFontStyleFontColor").val('#' + hex);
        }
    });

    $('#colorSelectorFormControlFontStyleFontColor').ColorPicker({
        color: '#c4c4c4',

        onShow: function (colpkr) {
            $(colpkr).fadeIn(500);
            return false;
        },
        onHide: function (colpkr) {
            $(colpkr).fadeOut(500);
            return false;
        },
        onChange: function (hsb, hex, rgb) {
            $('#colorSelectorFormControlFontStyleFontColor div').css('backgroundColor', '#' + hex);
            $("#lblFormControlFontStyleFontColor").text('#' + hex);
            $("#FormControlFontStyleFontColor").val('#' + hex);
        }
    });

    $('#colorSelectorClientTileBackgroundColor').ColorPicker({
        color: '#c4c4c4',

        onShow: function (colpkr) {
            $(colpkr).fadeIn(500);
            return false;
        },
        onHide: function (colpkr) {
            $(colpkr).fadeOut(500);
            return false;
        },
        onChange: function (hsb, hex, rgb) {
            $('#colorSelectorClientTileBackgroundColor div').css('backgroundColor', '#' + hex);
            $("#lblClientTileBackgroundColor").text('#' + hex);
            $("#ClientTileBackgroundColor").val('#' + hex);
        }
    });

    $('#colorSelectorAccountsTileBackgroundColor').ColorPicker({
        color: '#c4c4c4',

        onShow: function (colpkr) {
            $(colpkr).fadeIn(500);
            return false;
        },
        onHide: function (colpkr) {
            $(colpkr).fadeOut(500);
            return false;
        },
        onChange: function (hsb, hex, rgb) {
            $('#colorSelectorAccountsTileBackgroundColor div').css('backgroundColor', '#' + hex);
            $("#lblAccountsTileBackgroundColor").text('#' + hex);
            $("#AccountsTileBackgroundColor").val('#' + hex);
        }
    });

    $('#colorSelectorPrivilegesTileBackgroundColor').ColorPicker({
        color: '#c4c4c4',

        onShow: function (colpkr) {
            $(colpkr).fadeIn(500);
            return false;
        },
        onHide: function (colpkr) {
            $(colpkr).fadeOut(500);
            return false;
        },
        onChange: function (hsb, hex, rgb) {
            $('#colorSelectorPrivilegesTileBackgroundColor div').css('backgroundColor', '#' + hex);
            $("#lblPrivilegesTileBackgroundColor").text('#' + hex);
            $("#PrivilegesTileBackgroundColor").val('#' + hex);
        }
    });

    $('#colorSelectorDevicesTileBackgroundColor').ColorPicker({
        color: '#c4c4c4',

        onShow: function (colpkr) {
            $(colpkr).fadeIn(500);
            return false;
        },
        onHide: function (colpkr) {
            $(colpkr).fadeOut(500);
            return false;
        },
        onChange: function (hsb, hex, rgb) {
            $('#colorSelectorDevicesTileBackgroundColor div').css('backgroundColor', '#' + hex);
            $("#lblDevicesTileBackgroundColor").text('#' + hex);
            $("#DevicesTileBackgroundColor").val('#' + hex);
        }
    });

    $('#colorSelectorReportsTileBackgroundColor').ColorPicker({
        color: '#c4c4c4',

        onShow: function (colpkr) {
            $(colpkr).fadeIn(500);
            return false;
        },
        onHide: function (colpkr) {
            $(colpkr).fadeOut(500);
            return false;
        },
        onChange: function (hsb, hex, rgb) {
            $('#colorSelectorReportsTileBackgroundColor div').css('backgroundColor', '#' + hex);
            $("#lblReportsTileBackgroundColor").text('#' + hex);
            $("#ReportsTileBackgroundColor").val('#' + hex);
        }
    });

    $('#colorSelectorRolesTileBackgroundColor').ColorPicker({
        color: '#c4c4c4',

        onShow: function (colpkr) {
            $(colpkr).fadeIn(500);
            return false;
        },
        onHide: function (colpkr) {
            $(colpkr).fadeOut(500);
            return false;
        },
        onChange: function (hsb, hex, rgb) {
            $('#colorSelectorRolesTileBackgroundColor div').css('backgroundColor', '#' + hex);
            $("#lblRolesTileBackgroundColor").text('#' + hex);
            $("#RolesTileBackgroundColor").val('#' + hex);
        }
    });

    $('#colorSelectorAdministrationTileBackgroundColor').ColorPicker({
        color: '#c4c4c4',

        onShow: function (colpkr) {
            $(colpkr).fadeIn(500);
            return false;
        },
        onHide: function (colpkr) {
            $(colpkr).fadeOut(500);
            return false;
        },
        onChange: function (hsb, hex, rgb) {
            $('#colorSelectorAdministrationTileBackgroundColor div').css('backgroundColor', '#' + hex);
            $("#lblAdministrationTileBackgroundColor").text('#' + hex);
            $("#AdministrationTileBackgroundColor").val('#' + hex);
        }
    });

    $('#colorSelectorToolsTileBackgroundColor').ColorPicker({
        color: '#c4c4c4',

        onShow: function (colpkr) {
            $(colpkr).fadeIn(500);
            return false;
        },
        onHide: function (colpkr) {
            $(colpkr).fadeOut(500);
            return false;
        },
        onChange: function (hsb, hex, rgb) {
            $('#colorSelectorToolsTileBackgroundColor div').css('backgroundColor', '#' + hex);
            $("#lblToolnTileBackgroundColor").text('#' + hex);
            $("#ToolsTileBackgroundColor").val('#' + hex);
        }
    });

    $('#colorSelectorAppearanceTileBackgroundColor').ColorPicker({
        color: '#c4c4c4',

        onShow: function (colpkr) {
            $(colpkr).fadeIn(500);
            return false;
        },
        onHide: function (colpkr) {
            $(colpkr).fadeOut(500);
            return false;
        },
        onChange: function (hsb, hex, rgb) {
            $('#colorSelectorAppearanceTileBackgroundColor div').css('backgroundColor', '#' + hex);
            $("#lblAppearanceTileBackgroundColor").text('#' + hex);
            $("#AppearanceTileBackgroundColor").val('#' + hex);
        }
    });
});

function ChooseImage(id, e, para, hidden) {
    $("#" + para).html(e.target.files[0].name);

    $("#" + hidden).val(e.target.files[0].name);
}

function RemoveColorAndBackground(div, label, color, para, image)
{
    $("#" + div).css("background-color", '');
    $("#" + label).text('');
    $("#" + color).val('');

    $("#" + para).html('File name will be come here');
    $("#" + image).val('');

    pClientTileBackgroundImage
}