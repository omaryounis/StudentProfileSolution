app.controller("ExchangeOrderController", ["$scope", "$http", "ExchangeOrderSrvc", function ($scope, $http, ExchangeOrderSrvc) {
    $scope.Payrolls = [];
    $scope.Payroll = [];
    $scope.SelectedPayrollId = null;
    $scope.ExchangeOrder = { OrderNumber: "" };
    $scope.btn_Savetext = "حفظ";
    $scope.ExchangeOrderID = null;
    $scope.onClickeditButton = function (e) {
        $scope.btn_Savetext = "تعديل"; 
        $scope.ExchangeOrderID = e.value;
        ExchangeOrderSrvc.GetAllPayRolls($scope.ExchangeOrderID ).then(function (data) {
            $scope.Payrolls = data.data;

        });
        ExchangeOrderSrvc.GetEditExChangeOrder(e.value).then(function (data) {
            $scope.ExchangeOrder.OrderNumber = data.data;

        });
        ExchangeOrderSrvc.GetPayrollNumbers(e.value).then(function (data) {

            $scope.SelectedPayrollId = data.data;
        });


    };


    $scope.onClickActiveAndNotActive = function (e) {

        ExchangeOrderSrvc.ActiveAndNotActive(e.value).then(function (data) {
            DevExpress.ui.notify(data.data, "success", 5000);
            ExchangeOrderSrvc.GetAllPayRollExchaneOrders().then(function (data) {
                $scope.Payroll = data.data;
            });
        });
    }

    $scope.saveChanges = function () {      
        ExchangeOrderSrvc.addNewPayRollExchaneOrders({
            ExchangeOrder: $scope.ExchangeOrder,
            payllosId: $scope.SelectedPayrollId,
            ExchangeOrderID: $scope.ExchangeOrderID
        }).then(function (data) {

            if (data.data !== "") {
                DevExpress.ui.notify(data.data, "error", 5000);
                return false;
            }
            else {

                if ($scope.btn_Savetext=="حفظ")
                    DevExpress.ui.notify("تم الحفظ ", "success", 5000);

                else {
                        DevExpress.ui.notify("تم التعديل", "success", 5000);
                        $scope.ExchangeOrder.OrderNumber = "";
                        $scope.btn_Savetext = "حفظ";
                        $scope.ExchangeOrderID = null;
                        $scope.SelectedPayrollId = null;                                
                }
                   
                ExchangeOrderSrvc.GetAllPayRollExchaneOrders().then(function (data) {
                    $scope.Payroll = data.data;
                });
                
                ExchangeOrderSrvc.GetAllPayRolls($scope.ExchangeOrderID).then(function (data) {
                    $scope.Payrolls = data.data;
                    
                });
            }
        });
    }
   
    ExchangeOrderSrvc.GetAllPayRolls($scope.ExchangeOrderID).then(function (data) {
        $scope.Payrolls = data.data;

    });


    ExchangeOrderSrvc.GetAllPayRollExchaneOrders().then(function (data) {
        $scope.Payroll = data.data;
    });
  
    $scope.textBox = {
        ExchangeOrder:
        {
            placeholder: "ادخل امر صرف",
            bindingOptions: {
                value: "ExchangeOrder.OrderNumber"
            },
            onValueChanged: function (e) {
                $scope.OrderNumbeRvalue = e.value;

            }
        }
    };


    //validation 
    $scope.validationRequired = {
        validationRules: [
            {
                type: "required",
                message: "الحقل مطلوب"
            }
        ]
    };

    $scope.submitButtonOptions = {
        bindingOptions: {
            text: "btn_Savetext",
        },

        type: 'success',
        useSubmitBehavior: true
    };

    $scope.PayrollOptions = {
        bindingOptions: {
            items: "Payrolls",
            value: "SelectedPayrollId"          
        },

        placeholder: "--أختر--",
        noDataText: "لا يوجد بيانات",
        paginate: true, //Pagenation
        displayExpr: "PayrollNumber",
        valueExpr: "ID",
        showBorders: true,
        searchEnabled: true,
        showSelectionControls: true,
        maxDisplayedTags: 2,
        showMultiTagOnly: false,

        onInitialized: function (e) {

        },
        onValueChanged: function (e) {
            $scope.SelectedPayrollId = e.value;
            if (e.value == 0)
                $scope.SelectedPayrollId = null;
        }
    };




    $scope.PayRollExchaneOrdersGrid = {
        bindingOptions: {
            dataSource: "Payroll",
            value: "SelectPayroll",

        },
        showBorders: true,
        paging: {
            pageSize: 10
        },

        pager: {
            allowedPageSizes: [5, 10, 20]
        },
        "export": {
            enabled: true,
            fileName: "قائمة اوامر الصرف"
        },
        searchPanel: {
            visible: true,
            placeholder: "بحث",
            width: 300
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
        headerFilter: {
            visible: true,
            allowSearch: true
        },
        showRowLines: false,
        groupPanel: {
            visible: true,
            emptyPanelText: "اسحب عمود هنا"
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

        rowAlternationEnabled: true,
        allowColumnReordering: true,
        allowColumnResizing: true,
        columnAutoWidth: true,
        columnChooser: {
            enabled: false
        },

        reshapeOnPush: true,
        keyExpr: "ID",
        onValueChanged: function (e) {
        },
        columns: [
            {
                dataField: "ID",
                visible: false
            },
            {
                dataField: "OrderNumber",
                caption: "امر الصرف",
                alignment: "center",
            },
            {
                dataField: "Payrolls",
                caption: "ارقام المسيرات",
                alignment: "center"

            },
            {
                dataField: "IsTotallyInChecks",
                caption: "تم الصرف",
                alignment: "center"

            },
            {
                dataField: "ID",
                alignment: "center",
                caption: "تعديل",
                cssClass: "text-center ",
                width: 220,
                cellTemplate: function (cellElement, cellInfo) {
                        var editButton = $("<button>")
                            .addClass(" fa fa-pencil btn btn-warning btn-sm")
                        .on("click", $.proxy($scope.onClickeditButton, this, cellInfo));
                    cellElement.append(editButton);
                      
                }
            }

            ,
            {
                dataField: "ID",
                alignment: "center",
                caption: "تنشيط/ايقاف",
                cssClass: "text-center ",
                width: 220,
                cellTemplate: function (cellElement, cellInfo) {
                    if (cellInfo.data.IsActive === true) {
                        var deleteButton = $("<button>")
                            .addClass("fa fa-lock btn btn-danger btn-sm")
                            .on("click", $.proxy($scope.onClickActiveAndNotActive, this, cellInfo));
                        cellElement.append(deleteButton);
                    }
                    else {
      
                        var deleteButton = $("<button>")
                            .addClass("fa fa-unlock-alt btn btn-success btn-sm")
                            .on("click", $.proxy($scope.onClickActiveAndNotActive, this, cellInfo));
                        cellElement.append(deleteButton);
                    }


                }
            }
        ],
        rtlEnabled: true,
        editing: {
            mode: "row"
        }
    };
}]);
