
function FilterColumn(tableName, s, e) {
    FilterTable(tableName);
}

function FilterTextColumn(tableName, s, e) {
    /// Make sure to only run text box filters when user presses the enter key.
    if (e.keyCode !== 13) {
        return;
    }
    FilterTable(tableName);
}

function FilterTable(tableName) {
    /// Since this is used ONLY for filter selections by user, all selections are applied and grid is set to the first page.
    var filters = GatherFilters(tableName);
    var sorts = GatherSorts(tableName);
    SpinnerShow();
    FilterSortPage(tableName, 1, filters, sorts);
}

function GatherFilters(tableName) {
    /// gather filters from table form
    /// jQuery serialize returns a Url Encoded String... 
    /// example: "LAST_NAME=m^text&STATE=CA^select-one"
    var result = "";
    var filters = $("#" + tableName + "_Index > thead > tr.xgFilterRow > th").find("input, select, textarea").serialize();

    // Take the serialized data and strip tableName and '_Filter' to get just the column names.
    var fieldsAndValues = filters.split("&");
    for (i = 0; i < fieldsAndValues.length; i++) {
        var ss = fieldsAndValues[i].split("=");
        ss[0] = ss[0].substr(tableName.length + 1);
        ss[0] = ss[0].substring(0, ss[0].length - ("_Filter").length);
        fieldsAndValues[i] = ss[0] + "=" + ss[1];
    }

    // Now add the filter type to the serialized data.
    // jquery filter types are : text, select-one, radio, checkbox, textarea
    // we need this to properly apply filters on the server... Equals, Contains, StartsWith, etc.
    $("#" + tableName + "_Index > thead > tr.xgFilterRow > th").each(function () {
        var ctl = null;
        ctl = $(this).find("input, select, textarea");
        if (ctl === undefined || ctl === null) { return true; }
        var id = ctl.attr('name');
        if (id === undefined || id === null) { return true; }
        var columnName = id.substr(tableName.length + 1);
        columnName = columnName.substring(0, columnName.length - ("_Filter").length);
        var controlType = ctl.prop('type');
        for (i = 0; i < fieldsAndValues.length; i++) {
            if (fieldsAndValues[i].indexOf(columnName + "=") === 0) {
                fieldsAndValues[i] = fieldsAndValues[i] + "^" + controlType;
                break;
            }
        }
    });

    for (i = 0; i < fieldsAndValues.length; i++) {
        result = result + "&" + fieldsAndValues[i];
    }

    // strip extra leading ampersand
    if (result !== "") {
        result = result.substr(1);
    }

    return result;
}

