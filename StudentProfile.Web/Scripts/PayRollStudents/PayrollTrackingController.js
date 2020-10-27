app.controller("PayrollTrackingCtrl", ["$scope", "$http", "PayrollTrackingSrvc", function ($scope, $http, PayrollTrackingSrvc) {
   
    $scope.PayrollDataList = [];

    $scope.PayrollSelectBox = {
        bindingOptions: {
            dataSource: "PayrollList",
            value: "PayrollID",
            items: "PayrollList"
        },
        placeholder: "--أختر--",
        noDataText: "لا يوجد بيانات",
        pagingEnabled: true, //Pagenation
        showClearButton: true,
        searchEnabled: true,
        displayExpr: "Text",
        valueExpr: "Value",
        disabled: false,
        onInitialized: function (e) {
            debugger;
            //Get Users
            PayrollTrackingSrvc.GetActivePayRolls().then(function (data) {
                $scope.PayrollList = data.data;
            });
        },
        onValueChanged: function (e) {
            debugger;
            $scope.PayrollDataList = [];
            GridDataSource.reload();
            $scope.PayrollID = e.value;
            if ($scope.PayrollID == null || $scope.PayrollID == '' || $scope.PayrollID == undefined) {
                return DevExpress.ui.notify({ message: "يجب إختيار المسير" }, "Error", 5000);
            }
            $scope.currentKey = $scope.PayrollID;
            return $http({
                method: "Get",
                url: "/PayRollStudents/GetPayRollStatge",
                params: {
                    payrollID: $scope.PayrollID
                }
            }).then(function (data) {
                if (data.data.length > 0) {
                    $scope.PayrollDataList = data.data;
                    GridDataSource.reload();
                    $('body').css('overflow', 'hidden');
                }
            });
        }
    };  

    var GridDataSource = new DevExpress.data.DataSource({
        paginate: true,
        cacheRawData: false,
        loadMode: "raw",
        load: function () {
            if ($scope.PayrollDataList.length > 0) {
                debugger;
                return $scope.PayrollDataList;
            } else {
                return [];
            }
        }
    });

    $scope.gridPayrollData = function () {
        return {
            dataSource: GridDataSource,
            keyExpr: "ID",
            sorting: {
                mode: "multiple"
            },
            paginate: true,
            columnAutoWidth: true,
            columnChooser: {
                enabled: true
            },
            wordWrapEnabled: true,
            showBorders: true,
            pager: {
                allowedPageSizes: "auto",
                infoText: "(صفحة  {0} من {1} ({2} عنصر",
                showInfo: true,
                showNavigationButtons: true,
                showPageSizeSelector: true,
                visible: true
            },
            headerFilter: {
                visible: false
            },
            showRowLines: true,
            groupPanel: {
                visible: false,
                emptyPanelText: "اسحب عمود هنا"
            },
            paging: {
                pageSize: 10
            },
            filterRow: {
                visible: true,
                operationDescriptions: {
                    between: "بين",
                    contains: "تحتوى على",
                    endsWith: "تنتهي بـ",
                    equal: "يساوى",
                    greaterThan: "اكبر من",
                    greaterThanOrEqual: "اكبر من او يساوى",
                    lessThan: "اصغر من",
                    lessThanOrEqual: "اصغر من او يساوى",
                    notContains: "لا تحتوى على",
                    notEqual: "لا يساوى",
                    startsWith: "تبدأ بـ"
                },
                resetOperationText: "الوضع الافتراضى"
            },
            noDataText: "لا يوجد بيانات",
            editing: {
                allowUpdating: false,
                allowAdding: false,
                allowDeleting: false,
                mode: "row",
                texts: {
                    confirmDeleteMessage: "",
                    deleteRow: "",
                    editRow: "",
                    addRow: ""
                },
                useIcons: true
            },
            allowColumnResizing: true,
            columnResizingMode: "widget",
            allowColumnReordering: true,
            showColumnLines: true,
            rowAlternationEnabled: false,
            columns: [
                {
                    caption: "المرحلة",
                    dataField: "PhaseName",
                    cssClass: "text-right"

                },
                {
                    caption: "تاريخ الإعتماد",
                    dataField: "InsertDate",
                    cssClass: "text-right"
                },
                {
                    caption: "الإعتماد بواسطة",
                    dataField: "UserName",
                    cssClass: "text-right"
                }],
            onRowPrepared: function (info) {
                if (info.rowType === 'data') {
                    if (info.data.UserName == null || info.data.UserName == '' && info.data.UserName == undefined) {
                        info.rowElement.attr('style', 'background-color: #F4CACA !important;');
                    } else {
                        info.rowElement.attr('style', 'background-color: #00ff80 !important;');
                    }
                }
            }
        };

    };
}
]);
