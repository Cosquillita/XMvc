@model IEnumerable<$nameSpace$.$modelName$>
@using $nameSpaceRoot$.Helpers;

    <link rel="stylesheet" href="~/Content/bootstrap.css">
    <link rel="stylesheet" href="~/Content/font-awesome.css">
    <script type="text/javascript" src="~/Scripts/jquery-1.12.4.js"></script>
    <script type="text/javascript" src="~/Scripts/bootstrap.js"></script>
	<script type="text/javascript" src="~/Scripts/xgScripts/xGridNew.js"></script>

    @{
        ViewBag.Title = "$modelNamePlural$";
    }

    <style>
        .table-striped > tbody > tr:nth-of-type(2n+1) {
            background-color: #DCDCDC;
        }

        .table-hover > tbody > tr:hover {
            background-color: #A9A9A9;
        }

        tr.sortRow > th {
            white-space: nowrap;
            background-color: #838383;
        }

            tr.sortRow > th > a > span {
                color: black;
            }

            tr.sortRow > th > a:hover > span {
                color: white;
            }

        tr.filterRow > th {
            white-space: nowrap;
            background-color: #838383;
        }
    </style>
    <h2>$modelNamePlural$</h2>

    <div style="height: 20px; width: 100%;"></div>
    <div id="container_$modelName$" style="overflow: scroll; max-height: 1000px; width: 95%; margin: 0 auto 0;">
        <input type="hidden" id="filters" name="filters" value="@ViewBag.filters" />
        <input type="hidden" id="sorts" name="sorts" value="@ViewBag.sorts" />
        <input type="hidden" id="pageNum" name="pageNum" value="@ViewBag.pageNum" />
        <input type="hidden" id="pageNum" name="pageNum" value="@ViewBag.pageSize" />
        <table id='table_$modelName$' class="table table-striped table-bordered table-hover table-responsive">
            <thead>
                <tr id="sortRow_$modelName$" class='sortRow'>
                    <th><button type="button" class="btn btn-primary" onclick="AddRecord('$modelName$');">Add</button></th>
                    {sortRowItems}
                </tr>
                <tr id="filterRow_$modelName$" class='filterRow'>
					<th><button type="button" class="btn btn-danger" onclick="ClearFiltersAndSorts('$modelName');">Clear</button></th>
                    {filterRowItems}
                </tr>
            </thead>
            <tbody style="overflow-y: scroll;">
                @foreach (var item in Model)
                {
                    <tr>
                        <td class="command" style="background-color: #838383"><button type="button" class="btn btn-warning" onclick="EditRecord('$modelNamePlural$', '@item.$pkName$');">Edit</button></td>
                        {dataRowItems}
                    </tr>
                }
            </tbody>
            <tfoot>
                <tr style="background-color: #A9A9A9">
                    <td colspan='$pagerColSpan$' id="pager" style="background-color: #838383">
                        @LinqPager.CreatePager("$modelNamePlural$", "List", ViewBag.pageNum, ViewBag.pageSize, ViewBag.rowCount, ViewBag.pageCount)
                    </td>
                </tr>
            </tfoot>
        </table>
    </div>

    <div id="modalDialog" class="modal" tabindex="-1" role="dialog" style="width: 1200px;">
        <div class="modal-dialog modal-dialog-centered" role="document" style="width: 100%;">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title">Edit</h5>
                    <button type="button" class="close" data-dismiss="modal" aria-label="Close">
                        <span aria-hidden="true">&times;</span>
                    </button>
                </div>
                <div class="modal-body">
                    <form>
                        <fieldset>
                            <legend>Edit</legend>
                        </fieldset>
                    </form>
                </div>
                <div class="modal-footer">
                    <button type="button" class="btn btn-primary" onclick="SaveRecord();" disabled="disabled">Save changes</button>
                    <button type="button" class="btn btn-secondary" data-dismiss="modal">Close</button>
                </div>
            </div>
        </div>
    </div>

    <script>
        $(document).ready(function () {
            AddSortIconsToGrid();
            AddFiltersToGrid();
        });
    </script>

