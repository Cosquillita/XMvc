///////////////////// main function //////////////////////
// filter, sort, page - this is the main function for client side selection
///////////////////// pager function //////////////////////
function FilterSortPage(tableName, filters, sorts, pageNum, pageSize) {
    /// This function must be called after each change on the _Index page.
    /// Changes : filter selection, sort selection, page click, edit, insert, delete.
    SpinnerShow();
    var sURL = "/" + tableName + "/List";
    if (pageNum === null) {
        pageNum = 1;
    }
    var vars = { "filters": filters, "sorts": sorts, "pageNum": pageNum, "pageSize": pageSize };
    $.ajax({
        url: sURL,
        type: "GET",
        data: vars
    })
    .done(function (result) {
        $("#" + tableName + "_Container").html(result);
        SetupGrid();
    })
    .fail(function () {
        alert('fail FilterSortPage() function');
    })
    .always(function () {
        SpinnerHide();
    });
}

///////////////////// pager function //////////////////////
function PagerClick(tableName, pageNum, pageSize) {
    var filters = null;
    var sorts = null;
    //var filters = GatherFilters(tableName);
    //var sorts = GatherSorts(tableName);
    FilterSortPage(tableName, filters, sorts, pageNum, pageSize);
}

function SetupGrid() {
    /// the basic setup grid function
    if ($('table.xg').length === 0 && $("input[class*='xgEditDate']").length === 0) {
        return;
    }
    HighlightHoveredRow();
    HighlightAlternatingRows();
    InitializeDatePickers()
    AddSelectedFilterToSelect();
}

function AddSelectedFilterToSelect() {
    // set the selected filter for a drop down list
    // the selected filter is stored in the data-filter property or attribute by the Razor page.
    // this gets that value and assigns it properly to the select element.
    $("select[class*='xgFilterSelect']").each(function (index, value) {
        var $select = $(this);
        if ($select.attr("data-filter") !== undefined && $select.attr("data-filter") !== null && $select.attr("data-filter") !== "") {
            var s = $select.attr("data-filter");
            $select.val(s);
        }
    });
}

function HighlightHoveredRow() {
    /// self-explanatory from jQuery Recipes
    /// try-catch required when no data in table
    try {
        $('table.xg tbody tr').hover(
            function () {
                $(this).find('td').addClass('xgHover');
            },
            function () {
                $(this).find('td').removeClass('xgHover');
            }
        );
    }
    catch (err) {
    }
}

function HighlightAlternatingRows() {
    /// self-explanatory from jQuery Recipes
    /// try-catch required when no data in table
    try {
        $('table.xg tbody tr:odd').addClass('xgAltRow');
    }
    catch (err) {
    }
}

function InitializeDatePickers() {
    /// setup jquery datepickers
    /// causes unknown error ???
    try {
        $("input[class*='xgEditDate']").each(function (index, value) {
            $(this).datepicker({ dateFormat: "mm-dd-yy" });
        });
        $("input[class*='xgFilterDate']").each(function (index, value) {
            $(this).datepicker({ dateFormat: "mm-dd-yy" });
        });
        $("input[type='datetime']").each(function (index, value) {
            $(this).datepicker({ dateFormat: "mm-dd-yy" });
        });
    }
    catch (err) {
        alert("Unknown jQuery error : InitializeDatePickers() " + err);
    }
}

function ReplaceAllEx(find, replace, str) {
    /// javascript replace all for a string
    return str.replace(new RegExp(find, 'g'), replace);
}

function LinkKeyPressed(tableName, s, e) {
    ///	<summary>
    ///		Enable keyboard navigation of grids.
    ///     section 508 advanced grid navigation
    ///     from DevExpress support
    /// to use this, all grid cells must be links and their id's assigned as in the example below.
    /// example of cell id: xg_[tableName]_cell_[row number]_[column number]
    ///	</summary>
    ///	<param name="tableName" type="String">
    ///	</param>
    ///	<param name="s" type="HtmlElement">
    ///	</param>
    ///	<param name="e" type="HtmlEvent">
    ///	</param>

    var idBase = "xg__" + tableName + '_cell_';
    if (s.id.indexOf(idBase) === -1) {
        return;
    }
    idBase = "xg__" + tableName + '_cell_';
    var tmp = s.id.replace(idBase, '');
    var nameParams = tmp.split('_');
    var rowIndex = parseInt(nameParams[0], 10);
    var colIndex = parseInt(nameParams[1], 10);

    switch (e.keyCode) {
        case 38: //Up arrow
            rowIndex--; break;
        case 40: //Down arrow
            rowIndex++; break;
        case 37: //Left arrow
            colIndex--; break;
        case 39: //Right arrow
            colIndex++; break;
        default: break;
    }
    var nextLink = document.getElementById(idBase + rowIndex + "_" + colIndex);
    if (nextLink !== null) { nextLink.focus(); }
}

///////////////////// spinner functions /////////////////////

function SpinnerShow() {
    var el = document.getElementById("progressOverlay");
    el.style.visibility = "visible";
}
function SpinnerHide() {
    var el = document.getElementById("progressOverlay");
    el.style.visibility = "hidden";
}

///////////////////// dialog functions /////////////////////

function DialogShow() {
    var el = document.getElementById("dialogOverlay");
    el.style.visibility = "visible";
}
function DialogHide() {
    var el = document.getElementById("dialogOverlay");
    el.style.visibility = "hidden";
    $("#dialogContent").html("");
}
function DialogHidden() {
    var result = false;
    var el = document.getElementById("dialogOverlay");
    if (el.style.visibility === "hidden") {
        result = true;
    }
    return result;
}

///////////////////// clear grid filter and sort selections //////////////////////
function ClearFiltersAndSorts(tableName) {
    FilterSortPage(tableName, 1);
}

