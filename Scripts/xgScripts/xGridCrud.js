//////////////////////////// vars  section ////////////////////////////////
var actionCreate = "_Create";
var actionEdit = "_Edit";
var actionDelete = "_Delete";
var actionDeleteConfirmed = "_DeleteConfirmed";

var ajaxCallFinished = false;
var ajaxAction;
var ajaxTableName;
var ajaxId;
var ajaxPageNum;

//////////////////////////// get   section ////////////////////////////////
function CrudGet(action, tableName, id, pageNum /* for Delete only, otherwise null */) {
    SpinnerShow();

    var sURL = "/" + tableName + "/" + action + "/"
    if (action === actionEdit) {
        sURL = "/" + tableName + "/" + action + "/" + id;
    } else if (action === actionDelete) {
        sURL = "/" + tableName + "/" + action + "/?id=" + id + "&pageNum=" + pageNum;
    } else if (action === actionCreate) {
        sURL = "/" + tableName + "/" + action + "/";
    }

    $.ajax({
        url: sURL,
        type: "GET"
    })
    .done(function (result) {
        $("#dialogContent").html(result);
        DialogShow();
        if (action !== actionDelete) {
            InitializeDatePickers();
        }
    })
    .fail(function (jqXHR, textStatus) {
        alert(action + " Request failed: " + textStatus);
    })
    .always(function () {
        SpinnerHide();
    });
}

//////////////////////////// cancel section ///////////////////////////////
function CrudCancel(event) {
    event.preventDefault(); // need this because buttons firing twice
    DialogHide();
}

//////////////////////////// post  section ////////////////////////////////
function CrudPost(action, tableName, id /* for Insert, the id will be null */, pageNum /* for Delete only, otherwise null */, event) {
    event.preventDefault(); // need this because buttons firing twice
    SpinnerShow();

    var sURL = "/" + tableName + "/" + action + "/";
    var vars = "";
    if (action === actionCreate) {
        vars = $("#_insertForm").serialize();
    } else if (action === actionDeleteConfirmed) {
        vars = $("#_deleteForm").serialize();
    } else if (action === actionEdit) {
        vars = $("#_editForm").serialize();
    }
    ajaxCallFinished = false;
    ajaxTableName = tableName;
    ajaxId = null;
    ajaxPageNum = pageNum;
    ajaxAction = action;
    crudTimerStart();

    $.ajax({
        url: sURL,
        type: "POST",
        data: vars
    })
    .done(function (result) {
        if (result.success === "true") {
            var filters = GatherFilters(tableName);
            var sorts = GatherSorts(tableName);
            SpinnerHide();
            DialogHide();
            if (action === actionCreate) {
                ajaxId = result.id;
                ajaxCallFinished = true;
            } else if (action === actionDeleteConfirmed) {
                ajaxCallFinished = true;
            } else if (action === actionEdit) {
                ajaxId = result.id;
                ajaxCallFinished = true;
            }
        } else {
            SpinnerHide();
            alert(result.error);
        }
    })
    .fail(function (jqXHR, textStatus) {
        SpinnerHide();
        alert(action + " Request failed: " + textStatus);
    })
    .always(function () {
    });
}

//////////////////////////// timer section ////////////////////////////////
var crudTimerVar;
function crudTimerStart() {
    crudTimerVar = setInterval(function () { crudTimerStep() }, 1000);
}
function crudTimerStep() {
    if (ajaxCallFinished) {
        crudTimerStop();
        if (ajaxAction === actionCreate) {
            FilterSortPage(ajaxTableName, 1, "", "", ajaxId);
        } else if (ajaxAction === actionDeleteConfirmed) {
            FilterSortPage(ajaxTableName, ajaxPageNum, "", "", null);
        } else if (ajaxAction === actionEdit) {
            FilterSortPage(ajaxTableName, 1, "", "", ajaxId);
        }
    }
}
function crudTimerStop() {
    window.clearInterval(crudTimerVar);
}

//////////////////////////// antiforgery   ////////////////////////////////
function RequestVerificationToken(data) {
    var securityToken = $('[name=__RequestVerificationToken]').val();
    if (data.length > 0) {
        data += "&__RequestVerificationToken=" + encodeURIComponent(securityToken);
    }
    else {
        data = "__RequestVerificationToken=" + encodeURIComponent(securityToken);
    }
}

