// sort constants
var asc = "asc";
var desc = "desc";
// font-awesome sort icon names
var upIcon = "fa-arrow-up";
var downIcon = "fa-arrow-down";

var nbsp = "&nbsp;";

// & becomes ╬ ascii 206
// = becomes ═ ascii 205
var ampersandd = "╬";
var equalSignn = "═";

function AddSortIconsToGrid() {
    // sorts are URL-encoded string in hidden field
    // example: columnName=sortDirection
    var sorts = $("#sorts").val();
    if (sorts !== null && sorts !== "") {
        sortsArr = sorts.split(ampersandd);
        for (var i = 0; i < sortsArr.length; i++) {
            var oneSort = sortsArr[i].split(equalSignn);
            var columnName = oneSort[0];
            var sortDirection = oneSort[1];
            var sortIcon = null;
            sortIcon = sortDirection === asc ? upIcon : sortIcon;
            sortIcon = sortDirection === desc ? downIcon : sortIcon;
            $("tr.sortRow > th").each(function () {
                var html = $(this).html();
                if (html.indexOf(columnName) > -1) {
                    // parse the sortlink html on the font awesome icon
                    var bitz = html.split("<i");
                    // and add the <i back
                    bitz[1] = "<i" + bitz[1];
                    // then parse on the span element
                    var endBitz = bitz[1].split("<s");
                    // and add the <s back
                    endBitz[1] = "<s" + endBitz[1];
                    // and add in sortDirection
                    endBitz[0] = '<i class="fa ' + sortIcon + '"></i>';
                    // and add a space between the icon and column caption to make it more readable
                    var output = bitz[0] + endBitz[0] + nbsp + endBitz[1];
                    $(this).html(output);
                }
            });
        }
    }
}
function AddFiltersToGrid() {
    var filters = $("#filters").val();
    if (filters !== null && filters !== "") {
        filtersArr = filters.split(ampersandd);
        for (var i = 0; i < filtersArr.length; i++) {
            var oneFilter = filtersArr[i].split(equalSignn);
            var columnName = oneFilter[0];
            var filterValue = oneFilter[1];
            $("tr.filterRow > th").each(function () {
                var $html = $(this).html();
                if ($html.indexOf(columnName) > -1) {
                    $(this).find('input').first().val(filterValue);
                }
            });
        }
    }
}
function SaveRecord() {
    // to do : add save ajax logic
    $('#modalDialog').modal('hide');
    // clear the edit form
    $('#modalBody').html('');
}
function CancelEdit() {
    $('#modalDialog').modal('hide');
    // clear the edit form
    $('#modalBody').html('');
}
function EditRecord(tableName, id) {
    $.ajax({
        url: tableName + "/Edit",
        type: "GET",
        data: { 'id': id }
    })
    .done(function (result) {
        $("#modalBody").html(result);
        $('#modalDialog').modal('show');
    })
    .fail(function (jqXHR, textStatus) {
        alert("Edit Request failed: " + textStatus);
    })
    .always(function () {
    });
}
function SpinnerShow() {
    //var el = document.getElementById("progressOverlay");
    //el.style.visibility = "visible";
}
function SpinnerHide() {
    //var el = document.getElementById("progressOverlay");
    //el.style.visibility = "hidden";
}
function ClearFiltersAndSorts(tableName) {
    $("#filters").val("");
    $("#sorts").val("");
    var pageSize = $("#pageSize").val();
    FilterSortPage(tableName, null, null, 1, pageSize);
}
function PagerClick(tableName, pageNum, pageSize) {
    var filters = $("#filters").val();
    var sorts = $("#sorts").val();
    FilterSortPage(tableName, filters, sorts, pageNum, pageSize);
}
function FilterSortPage(tableName, filters, sorts, pageNum, pageSize) {
    // NULL HANDLING
    // clip the word null from going back as a string
    filters = filters === null ? filters = "" : filters;
    sorts = sorts === null ? sorts = "" : sorts;

    // NULL HANDLING
    if (pageNum === undefined || pageNum === null) {
        pageNum = 1;
    }
    if (pageSize === undefined || pageSize === null) {
        pageSize = 10;
    }

    // to do : make this better...
    // remove the weird ascii characters and use json probably.
    //var jsonData = {
    //    "filters": filters,
    //    "sorts": sorts,
    //    "pageNum": pageNum,
    //    "pageSize": pageSize
    //};
    //
    // modify so URL encoding isn't getting corrupted
    // & becomes ╬ ascii 206
    // = becomes ═ ascii 205

    //$.ajax({
    //    method: "GET",
    //    url: tableName + "/Index",
    //    data: { filters: filters, sorts: sorts, pageNum: pageNum, pageSize: pageSize }
    //});
    //    //.done(function (data) {
    //    //    alert('done');
    //    //})
    //    //.fail(function (jqXHR, textStatus, errorThrown) {
    //    //    alert("Fail: " + errorThrown);
    //    //})
    //    //.always(function () {
    //    //});

    var paramz = "?filters={ff}&sorts={ss}&pageNum={pn}&pageSize={ps}";
    paramz = paramz.replace("{ff}", filters).replace("{ss}", sorts).replace("{pn}", pageNum).replace("{ps}", pageSize);
    var getUrl = window.location;
    var baseUrl = getUrl.protocol + "//" + getUrl.host + "/" + getUrl.pathname.split('/')[1];
    window.location.href = baseUrl + paramz;
}

function SortColumn(tableName, columnName, link) {
    // lastname=asc&firstname=asc
    var sorts = $("#sorts").val();
    var $link = $(link);
    if (sorts.indexOf(columnName + equalSignn) === -1) {
        // column not a selected sort
        // add column to sorts as ascending
        sorts += columnName + equalSignn + asc;
    }
    else if ($link.html().indexOf(upIcon) > -1) {
        // column is a selected sort and is ascending
        // change sort to descending
        var sortArr = sorts.split(ampersandd);
        for (var i = 0; i < sortArr.length; i++) {
            if (sortArr[i].indexOf(columnName + equalSignn) === 0) {
                sortArr[i] = columnName + equalSignn + desc;
                sorts = sortArr.join(ampersandd);
                break;
            }
        }
    }
    else if ($link.html().indexOf(downIcon) > -1) {
        // column is a selected sort and is descending
        // remove column from sorts
        var sortArr2 = sorts.split(ampersandd);
        for (var j = 0; j < sortArr2.length; j++) {
            if (sortArr2[j].indexOf(columnName + equalSignn) === 0) {
                sortArr2.splice(j);
                sorts = sortArr2.join(ampersandd);
                break;
            }
        }
    }
    var filters = $("#filters").val();
    var pageNum = $("#pageNum").val();
    FilterSortPage(tableName, filters, sorts, pageNum, $("#pageSize").val());
}

function FilterColumn(tableName, columnName, input, e) {
    if (e.keyCode !== 13) { return; }
    var $input = $(input);
    var newFilter = columnName + equalSignn + $input.val();
    var filters = $("#filters").val();
    // if filter not in filters, add it
    if (filters.indexOf(columnName) === -1) {
        filters = filters + ampersandd + newFilter;
    } else if (filters.indexOf(columnName) > -1) {
        // filter already in filters
        // parse filters and remove existing and add new
        var filtersArr = filters.split(ampersandd);
        for (var i = 0; i < filtersArr.length; i++) {
            if (filtersArr[i].indexOf(columnName) > -1) {
                filtersArr.splice(i);
                filters = filtersArr.join(ampersandd);
                break;
            }
        }
        filters = filters + ampersandd + newFilter;
    }
    // if only one filter, clip leading ampersand
    if (filters.indexOf(ampersandd) === 0) {
        filters = filters.substring(1);
    }
    var sorts = $("#sorts").val();
    var pageNum = $("#pageNum").val();
    FilterSortPage(tableName, filters, sorts, pageNum, $("#pageSize").val());
}
