app.controller("CheckDeliveryCtrl", ["$scope", '$rootScope', '$filter', 'CheckDeliveryService', function ($scope, $rootScope, $filter, CheckDeliveryService )
{
    $scope.Users = [];
    $scope.UserID = null;
    $scope.CheckIds = [];

    var DataSourceCheckstagbox = function (deliveredTo) {
       return new DevExpress.data.DataSource({
            paginate: true,
            cacheRawData: false,
            key: "ID",
            loadMode: "raw",
            load: function () {
                return $.getJSON("/ExchangeOrder/GetChecks?deliveredTo=" + deliveredTo, function (data) {
                    //ActivityRequestsArray = data;
                });
            }
        });
    }
    //var DataSourceUserselectbox = new DevExpress.data.DataSource({
    //    paginate: true,
    //    cacheRawData: false,
    //    key: "ID",
    //    loadMode: "raw",
    //    load: function () {
    //        return $.getJSON("/ExchangeOrder/GetUsers", function (data) {
    //            debugger;
    //            //ActivityRequestsArray = data;
    //        });
    //    }
    //}); 
    var DataSourceRepUsers = new DevExpress.data.DataSource({
        paginate: true,
        cacheRawData: false,
        key: "ID",
        loadMode: "raw",
        load: function () {
            return $.getJSON("/ExchangeOrder/RepUsersGrid", function (data) {
                debugger;
                //ActivityRequestsArray = data;
            });
        }
    }); 
    $scope.Checkstagbox = {
        showSelectionControls: true,
        dataSource: DataSourceCheckstagbox(null),
        bindingOptions: {
            //items: "Payrolls",
            value: "CheckIds"
        },
        displayExpr: "CheckNumber",
        valueExpr: "ID",

        onValueChanged: function (e) {
            $scope.CheckIds = e.value;
        }
    }  
    $scope.Userselectbox = {
        bindingOptions: {
            items: "Users",
            value: "UserID"
        },
        onInitialized: function (e) {
            CheckDeliveryService.GetUsers().then(function (data) {
                $scope.Users = data.data;
            });
            
        },
        //showSelectionControls: true,
        //dataSource: DataSourceUserselectbox,

        displayExpr: "Name",
        valueExpr: "ID",
        placeholder: "--أختر--",
        noDataText: "لا يوجد بيانات",
        paginate: true, //Pagenation
        showClearButton: true,
        searchEnabled: true,
        disabled: false,
        onValueChanged: function (e) {
        }
    }
    $scope.RepUsersGrid = {
        dataSource: DataSourceRepUsers,
        keyExpr: "ID",
        bindingOptions: {
            rtlEnabled: true
        },
        sorting: {
            mode: "multiple"
        },
        wordWrapEnabled: false,
        showBorders: true,
        searchPanel: {
            width: 1300,
            visible: true,
            placeholder: "بحث"
        },
        paging: {
            pageSize: 5
        },
        pager: {
            allowedPageSizes: "auto",
            infoText: "(صفحة  {0} من {1} ({2} عنصر",
            showInfo: true,
            showNavigationButtons: true,
            showPageSizeSelector: true,
            visible: "auto"
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
            visible: true
        },
        showRowLines: true,
        groupPanel: {
            visible: false,
            emptyPanelText: "اسحب عمود هنا"
        },
        noDataText: "لايوجد بيانات",
        columnAutoWidth: true,
        columnChooser: {
            enabled: true
        },
        "export": {
            enabled: true,
            fileName: "DeliveredChecks"
        },
        columns: [
           
            {
                dataField: "UserName",
                caption: "اسم الموظف"
            },
            
            {
                dataField: "TotalAssignedValue",
                caption: "القيمة الكلية المسندة"
            },

            {
                caption: "#",
                width: 100,
                cssClass: "text-center",
                cellTemplate: function (container, options) {
                    $("<div />").dxButton({
                        //icon: "fa fa-save",
                        text: "",
                        type: "info",
                        icon:"edit",
                        hint: "تعديل",
                        elementAttr: { "class": "btn btn-primary" },
                        onClick: function (e) {
                            debugger;
                            CheckDeliveryService.GetCurrentChecksIDs(options.key).then(function (data) {
                                debugger;
                                DataSourceCheckstagbox(options.key).load();
                                $scope.UserID = options.key;
                                $scope.CheckIds = data.data;
                            });
                        }
                    }).appendTo(container);

                }
            }],
        allowColumnResizing: true,
        columnResizingMode: "widget",
        allowColumnReordering: true,
        showColumnLines: true,
        rowAlternationEnabled: true,

        masterDetail: {
        enabled: true,
            template: "DeliveredChecksToUser"
    }
    };
    $scope.getDeliveredChecksByUserId = function (key) {
        return {
            dataSource: new DevExpress.data.DataSource({
                paginate: true,
                cacheRawData: true,
                key: "ID",
                loadMode: "raw",
                load: function () {
                    return $.getJSON("/ExchangeOrder/GetDeliveredChecksByUserID?userID=" + key, function (data) {
                        //ActivityRequestsArray = data;
                    });
                },
                filter: ["DeliveredTo", "=", key]
            }),
            sorting: {
                mode: "multiple"
            },
            wordWrapEnabled: false,
            showBorders: true,
            paging: {
                pageSize: 5
            },
            pager: {
                allowedPageSizes: "auto",
                infoText: "(صفحة  {0} من {1} ({2} عنصر",
                showInfo: true,
                showNavigationButtons: true,
                showPageSizeSelector: true,
                visible: "auto"
            },
            headerFilter: {
                visible: false
            },
            showRowLines: true,
            groupPanel: {
                visible: false,
                emptyPanelText: "اسحب عمود هنا"
            },
            noDataText: "لايوجد بيانات",
            columnAutoWidth: true,
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
            rowAlternationEnabled: true,
            columns: [
                {
                    caption: 'رقم الشيك',
                    cssClass: "text-center",
                    dataField: "CheckNumber"
                },
                {
                    dataField: "CheckValue",
                    caption: "قيمة الشيك"
                },
                {
                    dataField: "DeliverDate",
                    caption: "تاريخ الاستلام"

                },
                {
                    dataField: "AssignedFrom",
                    caption: "مسند من"
                }
            ]
        };
    };
    //btn save
    $scope.btnSave = {
        text: 'حفظ',
        type: 'success',
        useSubmitBehavior: true
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
    $scope.SaveCashCheck = function (event) {
        CheckDeliveryService.AssignCheckToUser($scope.CheckIds, $scope.UserID).then(function (data) {
            if (data.data == "") {
                debugger;
                DevExpress.ui.notify({ message: "تم الاسناد للمندوب بنجاح", type: "success", displayTime: 3000, closeOnClick: true });
                $scope.CheckIds = [];
                $scope.UserID = null;
                DataSourceCheckstagbox(null).load();
                DataSourceRepUsers.reload();
                return;
            }
            DevExpress.ui.notify({ message: "حدث خطأ أثناء الحفظ", type: "error", displayTime: 3000, closeOnClick: true });
        })
    }
}]);