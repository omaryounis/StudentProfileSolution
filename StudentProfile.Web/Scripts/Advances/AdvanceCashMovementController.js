(function () {

    app.controller("advanceCashMovement", function ($scope, $http,Service) {

        // start: datasource of grid
        var DataSourceAdvanceCash = new DevExpress.data.DataSource({
            paginate: true,
            cacheRawData: false,
            key: "ID",
            loadMode: "raw",
            load: function () {
                return $.getJSON("/Advances/AdvanceCashMovementsData", function (data) { debugger; });
            }
        });// end:datasource grid

        // start : data source of printing data 
        var DataSourcePrintedR = new DevExpress.data.DataSource({

            load: function () {
                return $.getJSON("/Advances/PaidDocPrintDataSource", function (data) { debugger; });
            }
         
        });// end : data source of printing data
        
        // start : grid
        $scope.AdvanceCashMovementGrid = {
            dataSource: DataSourceAdvanceCash,
            bindingOptions: {
                rtlEnabled: true
            },
            sorting: {
                mode: "multiple"
            },
            wordWrapEnabled: true,
            showBorders: true,
            searchPanel: {
                visible: true,
                placeholder: "بحث",
                width: 300
            },
            paging: {
                pageSize: 10
            },
            pager: {
                allowedPageSizes: "auto",
                infoText: "( صفحة  {0} من {1} ( {2} عنصر",
                showInfo: true,
                showNavigationButtons: true,
                showPageSizeSelector: true,
                visible: true
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
            width: '100%',
            columnChooser: {
                enabled: true
            },
            columns: [
                {
                    dataField: "AdvanceRequestID",
                    caption: "رقم الطلب",
                    alignment: "right",
                    width: 120
                    //groupIndex: 0
                },
                {
                    dataField: "AdvanceSettingName",
                    caption: "نوع الطلب",
                    alignment: "right",
                    width: 120
                    //groupIndex: 0
                },
                {
                    dataField: "DocName",
                    caption: "نوع السند",
                    alignment: "right",
                    width: 120
                    //groupIndex: 0
                },
                {
                    dataField: "DocNumber",
                    caption: "رقم الإيصال",
                    alignment: "right",
                    width: 110
                }, 
                {
                    dataField: "StudentName",
                    caption: " اسم الطالب",
                    alignment: "right",
                    width: 240
                },
                {
                    dataField: "TotalValue",
                    caption: "المبلغ الكلي",
                    alignment: "right",
                    width: 130
                },
                {
                    dataField: "DocHeader",
                    caption: "المحتوي",
                    alignment: "right"
                },
                {
                    dataField: "InsertionDate",
                    caption: "التاريخ ",
                    alignment: "right",
                    dataType: "date",
                    width: 120
                },
                {
                    dataField: "DocNotes",
                    caption: "بيان السند",
                    alignment: "right",
                    //width: 170
                },
                {
                    caption: "",
                    width: 80,
                    cssClass: "text-center",
                    cellTemplate: function (container, options) {
                        $("<div />").dxButton({
                            text: "",
                            type: "success",
                            hint: "طباعة",
                            icon:"print", 

                            onClick: function (e) {
                                debugger;
                                //$http({
                                //    method: "GET",
                                //    url: "/advances/PaidDocPrintDataSource",
                                //    data: {
                                //        DocMasterIds: options.data.ID
                                //    }
                                //}).then(function (data) {
                                   
                                //});
                                if (options.data.DocType === "P")
                                {
                                    window.location.href = "/Advances/PrintDocAdvance" + "?DocMasterIds=" + options.data.ID;
                                }
                                else if (options.data.DocType === "R")
                                {
                                    window.location.href = "/Advances/ReceiveDocAdvance" + "?DocMasterIds=" + options.data.ID;
                                }
                               
                            }
                        }).appendTo(container);
                    }
                }

            ],
            allowColumnResizing: true,
            columnResizingMode: "widget",
            allowColumnReordering: true,
            showColumnLines: true,
            rowAlternationEnabled: true,
        };
        // end : grid


        Service.getReceiptData()

    });// end controller

    //Service
    app.service("Service", ["$http", function ($http) {
        this.getReceiptData = function (id) {
            return $http({
                method: 'Get',
                url: '/Advances/PaidDocPrintDataSource?DocMasterIds=' + id
            });
        };// end function

    }]);// End service


})();





