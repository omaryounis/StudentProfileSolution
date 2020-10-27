app.controller("StudentsCashCtrl", ["$scope", '$rootScope', '$filter', 'StudentsCashService', function ($scope, $rootScope, $filter, StudentsCashService) {

    function ClearControls() {
        $scope.SelectedExchangeOrders = [];
        $scope.SelectedCheckID = null;
        $scope.SelectedPayrolls = [];
       
        $scope.PayrollsDataSource = [];
        $scope.ExchangeOrdersDataSource = [];
        $scope.selectedStudents = [];
    }
    $scope.CheckId = 0;
    ClearControls();
    var DataSourceCheckstagbox = new DevExpress.data.DataSource({
        paginate: true,
        cacheRawData: false,
        key: "ID",
        loadMode: "raw",
        load: function () {
            return $.getJSON("/ExchangeOrder/GetCurrentUserChecks", function (data) {
                //ActivityRequestsArray = data;
            });
        }
    });
    var DataSourceStudentsGrid = new DevExpress.data.DataSource({
        paginate: true,
        cacheRawData: false,
        key: ["PayrollID", "StudentID"],
        loadMode: "raw",
        load: function () {
            return $.getJSON("/ExchangeOrder/GetStudentPayrollsByPayrollID?payrolls=" + $scope.SelectedPayrolls, function (data) {
                //ActivityRequestsArray = data;
            });
        }
    });
    $scope.Checkselectbox = {
        dataSource: DataSourceCheckstagbox,
        bindingOptions: {
            //items: "Payrolls",
            value: "SelectedCheckID"
        },
        displayExpr: "CheckNumber",
        valueExpr: "ID",
        placeholder: "--أختر--",
        noDataText: "لا يوجد بيانات",
        paginate: true, //Pagenation
        showClearButton: true,
        searchEnabled: true,
        disabled: false,

        onValueChanged: function (e) {
            debugger;
           
            StudentsCashService.GetExchangeOrdersByCheckID(e.value).then(function (data) {
                $scope.ExchangeOrdersDataSource = data.data;
            });
        }
    }
    $scope.exchangeOrderstagbox = {
        showSelectionControls: true,
        bindingOptions: {
            items: "ExchangeOrdersDataSource",
            value: "SelectedExchangeOrders"
        },
        displayExpr: "OrderNumber",
        valueExpr: "ID",
        placeholder: "--أختر--",
        noDataText: "لا يوجد بيانات",
        onValueChanged: function (e) {
            debugger;
            $scope.SelectedExchangeOrders = e.value;
            StudentsCashService.GetPayrollsByExchangeOrders($scope.SelectedExchangeOrders).then(function (data) {
                $scope.PayrollsDataSource = data.data;
            });
        }
    };
    $scope.payrollstagbox = {
        showSelectionControls: true,
        bindingOptions: {
            items: "PayrollsDataSource",
            value: "SelectedPayrolls"
        },
        displayExpr: "PayrollNumber",
        valueExpr: "ID",
        placeholder: "--أختر--",
        noDataText: "لا يوجد بيانات",
        onValueChanged: function (e) {
            $scope.SelectedPayrolls = e.value;
        }
    };

    $scope.StudentsCashGrid = {
        dataSource: DataSourceStudentsGrid,
        keyExpr: "PayrollID;StudentID",
        bindingOptions: {
            rtlEnabled: true,

            //selectAllMode: "allPages",
            value: "selectedStudents"
        },
        selection: {
            mode: "multiple",
            showCheckBoxesMode: "always",

            deferred: true
        },
        showBorders: true,
        paging: {
            pageSize: 10
        },
        pager: {
            //showPageSizeSelector: true,
            allowedPageSizes: [5, 10, 20],
            showInfo: true
        },
        "export": {
            enabled: false
        },
        loadPanel: {
            enabled: true
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
            scrolling: {
                rtlEnabled: true,
                useNative: true,
                scrollByContent: true,
                scrollByThumb: true,
                showScrollbar: "onHover",
                mode: "standard", // or "virtual"
                direction: "both"
            },
            resetOperationText: "الوضع الافتراضى"
        },
        rowAlternationEnabled: true,
        allowColumnReordering: true,
        allowColumnResizing: true,
        columnAutoWidth: true,
        columnChooser: {
            enabled: false
        },
        columns: [

            {
                dataField: "StudentName",
                caption: "اسم الطالب"
            },

            {
                dataField: "PayrollNumber",
                caption: "رقم المسير"
            },
            {
                dataField: "Value",
                caption: "قيمة المكافأة"
            },
            {
                caption: "#",
                width: 100,
                cssClass: "text-center",
                cellTemplate: function (container, options) {
                    $("<div />").dxButton({
                        //icon: "fa fa-save",
                        text: "",
                        icon: "check",
                        type: "success",
                        hint: "تسليم",
                        elementAttr: { "class": "btn btn-cash" },
                        onClick: function (e) {
                            debugger;
                            //CheckDeliveryService.GetCurrentChecksIDs(options.key).then(function (data) {
                            //    debugger;
                            //    DataSourceCheckstagbox(options.key).load();
                            //    $scope.UserID = options.key;
                            //    $scope.CheckIds = data.data;
                            //});
                        }
                    }).appendTo(container);

                }
            }],
        allowColumnResizing: true,
        columnResizingMode: "widget",
        allowColumnReordering: true,
        showColumnLines: true,
        rowAlternationEnabled: true,
        onToolbarPreparing: function (e) {
            var dataGrid = e.component;
            e.toolbarOptions.items.unshift({
                location: "before",
                template: function () {
                    return $("<div />").dxButton({
                        //icon: "fa fa-save",
                        text: "تسليم المكافأة للجميع",
                        icon: "check",
                        type: "success",
                        hint: "تسليم",
                        elementAttr: { "class": "btn btn-cash" },
                        onClick: function (e) {
                            debugger;
                            //CheckDeliveryService.GetCurrentChecksIDs(options.key).then(function (data) {
                            //    debugger;
                            //    DataSourceCheckstagbox(options.key).load();
                            //    $scope.UserID = options.key;
                            //    $scope.CheckIds = data.data;
                            //});
                        }
                    })
                }
            })
        },
        onSelectionChanged: function (selectedItems) {
            debugger;
            $scope.gridSelectedRowsData = [];

            selectedItems.component.getSelectedRowKeys().done(function (keys) {
                debugger;
                $scope.selectedStudents = keys;
            })

        }
    };
    $scope.SearchForStudentsRewards = function () {
        debugger;
        DataSourceStudentsGrid.reload();
        $scope.CheckId = $scope.SelectedCheckID;
        ClearControls();
    }
    $scope.PayforStudents = function () {
        StudentsCashService.PayRewardtoStudents($scope.CheckId,$scope.selectedStudents).then(function (data) {
            DevExpress.ui.notify({ message: data.data.Message, type: data.data.Type, displayTime: 3000, closeOnClick: true });
        });
    }
    $scope.btnSearch = {
        text: '',
        icon:'search',
        type: 'success',
        useSubmitBehavior: true
    };
    $scope.validationRequired = {
        validationRules: [
            {
                type: "required",
                message: "الحقل مطلوب"
            }
        ]
    };
}]);