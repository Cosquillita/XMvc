////////////////////////////////////////////////////////////
var formTemplate =
    `
<div class="container body-content">
    <form action="/customers/Edit/{id}" method="post">
        <input name="__RequestVerificationToken" type="hidden" value="{token}">
        <div class="form-horizontal">
            <h4>customer</h4>
            <hr>
            <input data-val="true" data-val-number="The field id must be a number." data-val-required="The id field is required." id="id" name="id" type="hidden" value="1">

            {controls}

        </div>
        <br>
        <div class="form-group">
            <div class="col-md-offset-2 col-md-10">
                <input type="submit" value="Save" class="btn btn-default">
            </div>
        </div>
    </form>
    <div>
        <a href="/customers">Back to List</a>
    </div>
</div>
<script src="~/Scripts/jquery-3.3.1.js.download"></script>
<script src="~/Scripts/bootstrap.js.download"></script>
<script src="~/Scripts/jquery.validate.js.download"></script>
<script src="~/Scripts/jquery.validate.unobtrusive.js.download"></script>
`;
////////////////////////////////////////////////////////////
var editColumnTemplate =
    `
        <div class="form-group">
            <label class="control-label col-md-2" for="{columnName}">{columnLabel}</label>
            <div class="col-md-4">
                <input class="form-control text-box single-line" id="{columnName}" name="{columnName}" type="text" value="{columnValue}">
                <span class="field-validation-valid text-danger" data-valmsg-for="{columnName}" data-valmsg-replace="true"></span>
            </div>
        </div>
`;
////////////////////////////////////////////////////////////

var controls = null;
for (var i = 0; i < columnCount; i++) {
    var columnName = GetColumnName(i);
    var columnValue = GetColumnValue(i);
    controls += editColumnTemplate.replace("{columnName}", columnName).replace("columnValue", columnValue);
}
var formString = formTemplate.replace("{controls}", controls);

return formString;

function GetColumnCountOfTable(id) {
    var colCount = 0;
    $('table#' + id + ' > ' + 'tr:nth-child(1) td').each(function () {
        colCount++;
    });
    return colCount;
});

