
var unsorted = "unsorted";
var ascending = "ascending";
var descending = "descending";
var asc = "asc";
var desc = "desc";
var upIcon = "fa-arrow-up";
var downIcon = "fa-arrow-down";

function SortColumn(columnName, link) {
    // lastname=asc&firstname=asc

    var sorts = $("#sorts").val();
    var $link = $(link);
    if (sorts.indexOf(columnName) === -1) {
        // column not a selected sort
        // add column to sorts as ascending
        sorts += columnName + "=asc";
    }
    else if ($link.html().indexOf(upIcon) !== -1) {
        // column is a selected sort and is ascending
        // change sort to descending
        sorts += columnName + "=desc";
    }
    else if ($link.html().indexOf(downIcon) !== -1) {
        // column is a selected sort and is descending
        // remove column from sorts
        var sortArr = sorts.split("&");
        var newSorts = null;
        for (var i = 0; i < sortArr.length; i++) {
            if (sortArr[i].indexOf(columnName) > -1) {
                sortArr.splice(i);
                sorts = sortArr.join("&");
                break;
            }
        }
    }
    var filters = $("#filters").val();
    FilterSortPage(tableName, 1, filters, sorts);
}

