app.controller("CashCheckCtrl", ["$scope", '$rootScope', '$filter', 'CashCheckSrvc', function ($scope, $rootScope, $filter, CashCheckSrvc) {

    //Fields
    $scope.ChekId = 0;
    $scope.ChekNumber = null;
    $scope.chekIssueDateValue = null;
    $scope.chekValue = null;

    $scope.ExchangeOrderList = [];
    $scope.ExchangeOrderId = null;

    $scope.ExchangeOrderValue = null;
    $scope.RemainValue = null;


    $scope.AddExchangeOrderList = [];

    $scope.CashCheckList = [];



    //Controls
    $scope.txtChekNumber = {
        bindingOptions: {
            value: "ChekNumber"
        },
        placeholder: "رقم الشيك",
        onValueChanged: function (e) {
            $scope.ChekNumber = e.value;
        }
    };

    $scope.chekIssueDate = {
        bindingOptions: {
            value: "chekIssueDateValue"
        },
        type: "date",
        displayFormat: 'dd/MM/yyyy',

        onValueChanged: function (e) {
            $scope.chekIssueDateValue = e.value;

        }
    };

    $scope.txtchekValue = {
        bindingOptions: {
            value: "chekValue"
        },
        placeholder: "قيمة الشيك",
        onValueChanged: function (e) {
            $scope.chekValue = e.value;
        }
    };

    $scope.ExchangeOrderSelectBox = {
        bindingOptions: {
            dataSource: "ExchangeOrderList",
            value: "ExchangeOrderId",
            items: "ExchangeOrderList"
        },

        placeholder: "--أختر--",
        noDataText: "لا يوجد بيانات",
        paginate: true, //Pagenation
        displayExpr: "Text",
        valueExpr: "Value",
        showBorders: true,
        searchEnabled: true,

        onInitialized: function (e) {
            CashCheckSrvc.GetExchangeOrder().then(function (data) {
                $scope.ExchangeOrderList = data.data;
            });
        },
        onValueChanged: function (e) {
            debugger;
            $scope.ExchangeOrderId = e.value;
            var SumOutExchangeOrder = 0;

            if ($scope.ChekId === 0) {
                for (var i = 0; i < $scope.AddExchangeOrderList.length; i++) {
                    if ($scope.AddExchangeOrderList[i].ExchangeOrderId === e.value) {
                        SumOutExchangeOrder += Number.parseInt($scope.AddExchangeOrderList[i].Value);
                    }
                }


                CashCheckSrvc.GetExchangeOrderValueById($scope.ExchangeOrderId).then(function (data) {
                    $scope.RemainValue = Number.parseInt(data.data.TotalValue - SumOutExchangeOrder);
                });
            }
            else {
                debugger;
                for (var u = 0; u < $scope.AddExchangeOrderList.length; u++) {
                    if ($scope.AddExchangeOrderList[u].ExchangeOrderId === e.value) {
                        SumOutExchangeOrder += Number.parseInt($scope.AddExchangeOrderList[u].Value);
                    }
                }


                CashCheckSrvc.GetExchangeOrderValueByIdForEdit($scope.ExchangeOrderId, $scope.CurrentOrderId).then(function (data) {
                    $scope.RemainValue = Number.parseInt(data.data.TotalValue - SumOutExchangeOrder);
                });
            }

        }
    };

    $scope.txtExchangeOrderValue = {
        bindingOptions: {
            value: "ExchangeOrderValue"
        },
        placeholder: "قيمة امر الصرف",
        onValueChanged: function (e) {
            $scope.ExchangeOrderValue = e.value;

            var SumOutExchangeOrder = 0;


            debugger;

            if ($scope.ChekId === 0) {
                if (Number.parseInt($scope.ExchangeOrderValue) > Number.parseInt($scope.RemainValue)) {
                    DevExpress.ui.notify({ message: " قيمة امر الصرف المتبقية" + $scope.RemainValue + " وهي اقل من القيمة المدخلة ", type: 'error', displayTime: 3000, closeOnClick: true });
                    $scope.ExchangeOrderValue = null;
                    return false;
                }
                debugger;
                for (var i = 0; i < $scope.AddExchangeOrderList.length; i++) {
                    if ($scope.AddExchangeOrderList[i].ExchangeOrderId === $scope.ExchangeOrderId) {
                        SumOutExchangeOrder += Number.parseInt($scope.AddExchangeOrderList[i].Value);
                    }
                }
                CashCheckSrvc.GetExchangeOrderValueById($scope.ExchangeOrderId).then(function (data) {
                    $scope.RemainValue = (Number.parseInt(data.data.TotalValue) - SumOutExchangeOrder - e.value);
                });
            }
            else {

                for (var u = 0; u < $scope.AddExchangeOrderList.length; u++) {
                    if ($scope.AddExchangeOrderList[u].ExchangeOrderId === $scope.ExchangeOrderId) {
                        SumOutExchangeOrder += Number.parseInt($scope.AddExchangeOrderList[u].Value);
                    }
                }
                $scope.RemainValue = 0;
                CashCheckSrvc.GetExchangeOrderValueByIdForEdit($scope.ExchangeOrderId, $scope.CurrentOrderId).then(function (data) {

                    if (Number.parseInt($scope.ExchangeOrderValue) > Number.parseInt(data.data.TotalValue)) {
                        DevExpress.ui.notify({ message: " قيمة امر الصرف المتبقية" + $scope.RemainValue + " وهي اقل من القيمة المدخلة ", type: 'error', displayTime: 3000, closeOnClick: true });
                        $scope.ExchangeOrderValue = null;
                        return false;
                    }

                    $scope.RemainValue = (Number.parseInt(data.data.TotalValue) - SumOutExchangeOrder - e.value);
                });
            }
        }
    };

    $scope.txtRemainValue = {
        bindingOptions: {
            value: "RemainValue"
        },
        disabled: true,
        placeholder: "المتبقي",
        onValueChanged: function (e) {
            $scope.RemainValue = e.value;
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


    $scope.btnAdd = {
        icon: 'fa fa-plus',
        type: 'default',
        useSubmitBehavior: true
    };


    $scope.gridAddExchangeOrder = {
        bindingOptions: {
            dataSource: "AddExchangeOrderList"
        },

        noDataText: "لا يوجد بيانات",
        selection: {
            mode: "single"
        },
        showBorders: true,
        paging: {
            pageSize: 3
        },

        pager: {
            showPageSizeSelector: true,
            allowedPageSizes: [3],
            showInfo: true
        },
        columns: [
            {
                caption: "امر الصرف",
                dataField: "CachNumber",
                cssClass: "text-right"
            },
            {
                caption: "القيمة",
                dataField: "Value",
                cssClass: "text-right"
            },
            {
                caption: "تعديل",
                width: 100,
                cssClass: "text-center",
                cellTemplate: function (container, options) {
                    $("<div />").dxButton({
                        icon: "fa fa-pencil",
                        //text: "تعديل",
                        type: "warning",
                        hint: "تعديل",
                        elementAttr: { "class": "btn btn-warning btn-sm" },
                        onClick: function (e) {
                            if ($scope.ChekId !== 0) {
                                debugger;
                                CashCheckSrvc.GetExchangeOrderIsActive(options.data.ExchangeOrderId).then(function (data) {
                                    $scope.ExchangeOrderList = data.data;
                                });
                                debugger;
                                $scope.CurrentOrderId = Number.parseInt(options.data.ExchangeOrdersDetailId);
                                spliceCheckGrid(options.data);
                            }
                            else {
                                $scope.ExchangeOrderId = options.data.ExchangeOrderId.toString();
                                $scope.ExchangeOrderValue = options.data.Value;

                                spliceCheckGrid(options.data);
                            }
                        }
                    }).appendTo(container);
                }
            }
            //,{
            //    caption: "حذف",
            //    width: 100,
            //    cssClass: "text-center",
            //    cellTemplate: function (container, options) {
            //        $("<div />").dxButton({
            //            icon: "fa fa-trash-o",
            //            //text: "حذف",
            //            type: "danger",
            //            hint: "حذف",
            //            elementAttr: { "class": "btn btn-danger btn-sm" },
            //            onClick: function (e) {
            //                //CashCheckSrvc.GetExchangeOrderIsActive().then(function (data) {
            //                //    $scope.ExchangeOrderList = data.data;
            //                //});

            //                spliceCheckGrid(options.data);
            //            }
            //        }).appendTo(container);
            //    }
            //}
        ],

        onContentReady: function (e) {
            e.component.columnOption("command:edit", {
                visibleIndex: -1
            });
        },
        onSelectionChanged: function (info) {
            $scope.GridRowData = info.selectedRowKeys;
        },
        onInitialized: function (e) {

        }
    };



    //btn save
    $scope.btnSave = {
        text: 'حفظ',
        type: 'success',
        onClick: function (e) {
            if ($scope.ChekNumber === null || $scope.ChekNumber === "")
                return DevExpress.ui.notify({ message: " عفوا ادخل رقم الشيك", type: 'error', displayTime: 3000, closeOnClick: true });

            if ($scope.chekIssueDateValue === null || $scope.chekIssueDateValue === "")
                return DevExpress.ui.notify({ message: " عفوا ادخل تاريخ الشيك", type: 'error', displayTime: 3000, closeOnClick: true });

            if ($scope.AddExchangeOrderList.length === 0)
                return DevExpress.ui.notify({ message: " عفواادخل تفاصيل الشيك", type: 'error', displayTime: 3000, closeOnClick: true });


            var Checks = { ID: $scope.ChekId, CheckNumber: $scope.ChekNumber, CheckValue: $scope.chekValue, IssueDate: $scope.chekIssueDateValue };
            var ExchangeOrdersChecks = [];

            for (var i = 0; i < $scope.AddExchangeOrderList.length; i++) {
                ExchangeOrdersChecks.push({
                    ExchangeOrderID: $scope.AddExchangeOrderList[i].ExchangeOrderId,
                    Value: $scope.AddExchangeOrderList[i].Value
                });
            }

            CashCheckSrvc.SaveSaveCheks({ Checks, ExchangeOrdersChecks }).then(function (data) {
                debugger;
                if (data.data.status === 200) {
                    DevExpress.ui.notify({ message: data.data.Message, type: data.data.Type, displayTime: 3000, closeOnClick: true });
                    //Clear Data   
                    $scope.RefreshData();
                }
                if (data.data.status === 500) {
                    DevExpress.ui.notify({ message: data.data.Message, type: data.data.Type, displayTime: 3000, closeOnClick: true });
                }
            });
        }
    };


    /***************** List **********************/
    //dataGrid
    $scope.gridCashCheck = {
        keyExpr: "ID",

        bindingOptions: {
            dataSource: "CashCheckList"
        },
        noDataText: "لا يوجد بيانات",
        selection: {
            mode: "single"
        },
        showBorders: true,
        paging: {
            pageSize: 10
        },
        pager: {
            showPageSizeSelector: true,
            allowedPageSizes: [5, 10, 20],
            showInfo: true
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
                caption: "رقم الشيك",
                dataField: "CheckNumber",
                cssClass: "text-right"
            },
            {
                caption: "القيمة",
                dataField: "CheckValue",
                cssClass: "text-right"
            },
            {
                caption: "التاريخ",
                dataField: "InsertDate",
                cssClass: "text-right",
                type: "date"
            },
            {
                caption: "تاريخ الصرف",
                dataField: "IssueDate",
                cssClass: "text-right",
                type: "date"
            },

            {
                caption: "المسؤول",
                dataField: "InsertedBy",
                cssClass: "text-right"
            },
            {
                caption: "المستلم",
                dataField: "DeliveredTo",
                cssClass: "text-right"
            },

            {
                caption: "المسلم",
                dataField: "DeliveredBy",
                cssClass: "text-right"
            },
            {
                caption: "تاريخ التسليم",
                dataField: "DeliverDate",
                cssClass: "text-right",
                type: "date"
            },
            {
                caption: "تعديل",
                width: 100,
                cssClass: "text-center",
                cellTemplate: function (container, options) {
                    if (options.data.DeliveredTo !== null) {
                        $("<div />").dxButton({
                            icon: "fa fa-pencil",
                            //text: "تعديل",
                            type: "warning",
                            hint: "تعديل",
                            elementAttr: { "class": "btn btn-warning btn-sm" },
                            onClick: function (e) {

                                debugger;
                                $scope.ChekId = options.data.ID;
                                $scope.ChekNumber = options.data.CheckNumber;
                                $scope.chekIssueDateValue = options.data.FullInsertDate;
                                $scope.chekValue = options.data.CheckValue;

                                CashCheckSrvc.GetExchangeOrdersChecksByCheckId(options.data.ID).then(function (data) {
                                    debugger
                                    $scope.AddExchangeOrderList = [];
                                    var itemId = 0;
                                    for (var i = 0; i < data.data.length; i++) {
                                        $scope.AddExchangeOrderList.push({
                                            Id: (itemId + 1),
                                            ExchangeOrdersDetailId: data.data[i].ID,
                                            ExchangeOrderId: data.data[i].ExchangeOrderID,
                                            CachNumber: data.data[i].OrderNumber,
                                            Value: data.data[i].Value
                                        });
                                    }
                                });
                            }
                        }).appendTo(container);
                    }
                }
            },
            {
                caption: "حذف",
                width: 100,
                cssClass: "text-center",
                cellTemplate: function (container, options) {
                    if (options.data.DeliveredTo !== null) {
                        $("<div />").dxButton({
                            icon: "fa fa-trash-o",
                            //text: "حذف",
                            type: "danger",
                            hint: "حذف",
                            elementAttr: { "class": "btn btn-danger btn-sm" },
                            onClick: function (e) {
                                var result = DevExpress.ui.dialog.confirm("<i>هل انت متاكد؟</i>", "تاكيد الحذف");
                                result.done(function (dialogResult) {
                                    if (dialogResult) {
                                        CashCheckSrvc.DeleteCheck(options.data.ID).then(function (data) {
                                            if (data.data.status === 500) {
                                                DevExpress.ui.notify({ message: data.data.Message, type: data.data.Type, displayTime: 3000, closeOnClick: true });
                                            }
                                            if (data.data.status === 200) {
                                                DevExpress.ui.notify({ message: data.data.Message, type: data.data.Type, displayTime: 3000, closeOnClick: true });
                                                //Clear Data   
                                                $scope.RefreshData();
                                            }
                                        });
                                    }
                                });
                            }
                        }).appendTo(container);
                    }
                }
            }
        ],
        masterDetail: {
            enabled: true,
            template: "detail"
        },
        onContentReady: function (e) {
            e.component.columnOption("command:edit", {
                visibleIndex: -1
            });
        },
        onSelectionChanged: function (info) {
            $scope.GridRowData = info.selectedRowKeys;
        },
        onInitialized: function (e) {
            CashCheckSrvc.GetCashChecks().then(function (data) {
                $scope.CashCheckList = data.data;
            });
        }
    };

    $scope.gridExchangeOrdersChecks = function (key) {
        debugger;
        return {
            dataSource: new DevExpress.data.DataSource({
                paginate: true,
                cacheRawData: false,
                key: "CheckID",
                loadMode: "raw",
                load: function () {
                    debugger;
                    return $.getJSON("/ExchangeOrder/GetExchangeOrdersChecksByCheckId/" + key, function (data) { debugger; });
                }
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
                mode: "row"
            },
            allowColumnResizing: true,
            columnResizingMode: "widget",
            allowColumnReordering: true,
            showColumnLines: true,
            rowAlternationEnabled: true,
            columns: [
                {
                    caption: "امر الصرف",
                    dataField: "OrderNumber",
                    alignment: "center"
                },
                {
                    caption: "التاريخ",
                    dataField: "InsertDate",
                    alignment: "center",
                    type: "date"
                },
                {
                    caption: "القيمة",
                    dataField: "Value",
                    alignment: "center"
                }
            ]
        };
    };



    $scope.AddCashCheck = function () {
        debugger;
        if (Number.parseInt($scope.ExchangeOrderValue) > Number.parseInt($scope.chekValue)) {
            DevExpress.ui.notify({ message: "قيمة امر اصرف اكبر من قيمة الشيك", type: 'error', displayTime: 3000, closeOnClick: true });
            return false;
        }
        else {

            var SumOutExchangeOrder = 0;
            for (var i = 0; i < $scope.AddExchangeOrderList.length; i++) {
                if ($scope.AddExchangeOrderList[i].ExchangeOrderId === $scope.ExchangeOrderId) {
                    SumOutExchangeOrder += Number.parseInt($scope.AddExchangeOrderList[i].Value);
                }
            }
            CashCheckSrvc.GetExchangeOrderValueById($scope.ExchangeOrderId).then(function (data) {
                if (Number.parseInt($scope.ExchangeOrderValue) > Number.parseInt(data.data.TotalValue - SumOutExchangeOrder)) {
                    DevExpress.ui.notify({ message: " قيمة امر الصرف المتبقية " + $scope.RemainValue + " وهي اقل من القيمة المدخلة ", type: 'error', displayTime: 3000, closeOnClick: true });
                    return false;
                }
            });
        }

        var itemId = 0;
        $scope.AddExchangeOrderList.push({
            Id: (itemId + 1),
            ExchangeOrderId: $scope.ExchangeOrderId,
            CachNumber: $("#ExchangeOrderSelectBox").dxSelectBox("instance")._options.text,
            Value: $scope.ExchangeOrderValue
        });
        $scope.ExchangeOrderId = null;
        $scope.ExchangeOrderValue = null;
    };

    //Function
    function spliceCheckGrid(obj) {
        var index = $scope.AddExchangeOrderList.indexOf(obj);
        if (index > -1) {
            $scope.AddExchangeOrderList.splice(index, 1);
        }
    }

    $scope.RefreshData = function () {
        //Clear Data   
        $scope.ChekId = 0;
        $scope.CurrentOrderId = 0;
        $scope.ChekNumber = null;
        $scope.chekIssueDateValue = null;
        $scope.chekValue = null;
        $scope.ExchangeOrderId = null;
        $scope.ExchangeOrderValue = null;
        $scope.RemainValue = null;
        $scope.AddExchangeOrderList = [];
        //Referesh Grid
        CashCheckSrvc.GetCashChecks().then(function (data) {
            $scope.CashCheckList = data.data;
        });
        // Refresh DDL GetExchangeOrder
        CashCheckSrvc.GetExchangeOrder().then(function (data) {
            $scope.ExchangeOrderList = data.data;
        });
    }

}]);
