app.controller("PayrollApprovalController", ["$scope", "$http", "PayrollApprovalService", function ($scope, $http, PayrollApprovalService) {
 
    $scope.ClearControls = function () {
        $scope.acceptPopupVisible = false;
        $scope.isRequiredNotes = false;
        $scope.isRequiredpayNo = false;
        $scope.currentKey = 0;
        $scope.recommendationNotes = '';
        $scope.payNo = '';
        $scope.dafPayNo = '';
        $scope.dafPayNo2 = '';
        $scope.MDL_UploadingFilesvalue = '';
        //$scope.acceptPopup.hide();
    }
    $scope.ClearControls();
    var DataSourcePayrollsGrid = new DevExpress.data.DataSource({
        paginate: true,
        cacheRawData: true,
        key: "ID",
        loadMode: "raw",
        load: function () {
            debugger;
            return $.getJSON("/PayRollStudents/GetPayRolls", function (data) {

            });
        }
    });

    var DataSourcePayrollFilesGrid = new DevExpress.data.DataSource({
       
        paginate: true,
        cacheRawData: true,
        key: "ID",
        loadMode: "raw",
        load: function () { 
            if ($scope.FilesDetailsArray.length > 0) {
                debugger;
                return $scope.FilesDetailsArray;
            } else {
                return [];
            }
        }
    });


    var DataSourcePayrollBankGrid = new DevExpress.data.DataSource({
        paginate: true,
        cacheRawData: true,
        loadMode: "raw",
        load: function () {
            debugger;
            if ($scope.BankDetailsArray.length > 0) {
                return $scope.BankDetailsArray;
            } else {
                return [];
            }
        }
    });

    var DataSourcePayrollMoneyGrid = new DevExpress.data.DataSource({
        paginate: true,
        cacheRawData: true,
        loadMode: "raw",
        load: function () {
            debugger;
            if ($scope.MoneyDetailsArray.length > 0) {
                return $scope.MoneyDetailsArray;
            } else {
                return [];
            }
        }
    });

    $scope.MoneyGrid = function () {
        return {
            dataSource: DataSourcePayrollMoneyGrid,
            columnChooser: {
                enabled: true
            },
            "export": {
                enabled: true,
                fileName: "تفاصيل المسير النقديه",
                customizeExcelCell: function (e) {
                    debugger;
                    if (e.gridCell.rowType === "totalFooter" || e.gridCell.rowType === "data") {
                        e.horizontalAlignment = 'center'; //e.gridCell.column.alignment;
                        e.verticalAlignment = 'center';
                    }
                    if (e.gridCell.rowType === "totalFooter") {
                        e.backgroundColor = '#FFBB00';
                        e.font.color = '#000000';
                    }
                }
            }, sorting: {
                mode: "multiple"
            },
            columnAutoWidth: true,
            wordWrapEnabled: true,
            showBorders: true,
            pager: {
                allowedPageSizes: "auto",
                infoText: "(صفحة  {0} من {1} ({2} عنصر",
                showInfo: true,
                showNavigationButtons: true,
                showPageSizeSelector: true,
                visible: false
            },
            headerFilter: {
                visible: true
            },
            showRowLines: true,
            groupPanel: {
                visible: false,
                emptyPanelText: "اسحب عمود هنا"
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
            rowAlternationEnabled: true,
            selection: { mode: "single" }, 
            customizeExportData: function (columns, rows) {
                var arr = $scope.bankDataAfterJson;
                debugger;
                columns.forEach(function (column) {
                    column.width = 150;
                });
                //rows.forEach(function (row) {
                //    if (row.rowType = "totalFooter") {

                //    }
                //});
            },
            summary: {
                totalItems: [{}]
            }, onRowPrepared: function (info) {
                if (info.rowType === "data") {
                    var lengthRecs = info.columns.length;
                    var items = info.component.option("summary.totalItems");
                    if (!info.component.__summaryIsNotAdded) {
                        for (i = 0; i < lengthRecs; i++) {
                            var name = info.columns[i].dataField;
                            if (name != 'الكلية') {
                                items.push({
                                    column: name,
                                    summaryType: "sum",
                                    displayFormat: "{0}"
                                });
                                info.component.option("summary.totalItems", items);
                            } else {
                                items.push({
                                    column: name,
                                    displayFormat: "الإجمالي"
                                });
                                info.component.option("summary.totalItems", items);
                            }
                        }
                        info.component.__summaryIsNotAdded = true;
                    }
                }
                if (info.rowType == 'totalFooter') {
                    info.rowElement.attr('style', 'background-color: #F4CACA !important;');
                }
            }

            , onContentReady: function (e) {
                debugger;
                var dataGridInstance = $("#MoneyGrid").dxDataGrid("instance");
                var columnCount = dataGridInstance.columnCount();
                for (i = 0; i < columnCount; i++) {
                    $("#MoneyGrid").dxDataGrid("instance").columnOption(i, "width", 100);
                }

            }
        }
    }
    $scope.BankGrid = function () {
        return {
            dataSource: DataSourcePayrollBankGrid,
            columnChooser: {
                enabled: true
            },
            "export": {
                enabled: true,
                fileName: "تفاصيل المسير البنكيه",
                customizeExcelCell: function (e) {
                    debugger;
                    if (e.gridCell.rowType === "totalFooter" || e.gridCell.rowType === "data") {
                        e.horizontalAlignment = 'center'; //e.gridCell.column.alignment;
                        e.verticalAlignment = 'center';
                    }
                    if (e.gridCell.rowType === "totalFooter") {
                        e.backgroundColor = '#FFBB00';
                        e.font.color = '#000000';
                    }
                }  
            },
            sorting: {
                mode: "multiple"
            },
            columnAutoWidth: true,
            wordWrapEnabled: true,
            showBorders: true,
            pager: {
                allowedPageSizes: "auto",
                infoText: "(صفحة  {0} من {1} ({2} عنصر",
                showInfo: true,
                showNavigationButtons: true,
                showPageSizeSelector: true,
                visible: false
            },
            headerFilter: {
                visible: true
            },
            showRowLines: true,
            groupPanel: {
                visible: false,
                emptyPanelText: "اسحب عمود هنا"
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
            rowAlternationEnabled: true,
            selection: { mode: "single" },
            customizeExportData: function (columns, rows) {
                var arr = $scope.bankDataAfterJson;
                debugger;
                columns.forEach(function (column) {
                    column.width = 150;
                });
                //rows.forEach(function (row) {
                //    if (row.rowType = "totalFooter") {

                //    }
                //});
            },
            summary: {
                totalItems: [{}]
            }, onRowPrepared: function (info) {
                if (info.rowType === "data") {
                    var lengthRecs = info.columns.length;
                    var items = info.component.option("summary.totalItems");
                    if (!info.component.__summaryIsNotAdded) {
                        for (i = 0; i < lengthRecs; i++) {
                            var name = info.columns[i].dataField;
                            if (name != 'الكلية') {
                                items.push({
                                    column: name,
                                    summaryType: "sum",
                                    displayFormat: "{0}"
                                });
                                info.component.option("summary.totalItems", items);
                            } else {
                                items.push({
                                    column: name,
                                    displayFormat: "الإجمالي"
                                });
                                info.component.option("summary.totalItems", items);
                            }
                        }
                        info.component.__summaryIsNotAdded = true;
                    }
                }
                if (info.rowType == 'totalFooter') {
                    info.rowElement.attr('style', 'background-color: #F4CACA !important;');
                }
            } 
            , onContentReady: function (e) {
                debugger;
                 var dataGridInstance = $("#BankGrid").dxDataGrid("instance"); 
                var columnCount = dataGridInstance.columnCount();
                for (i = 0; i < columnCount; i++) {
                    $("#BankGrid").dxDataGrid("instance").columnOption(i, "width", 100);
                }
                
            }
        }
    } 

    $scope.PayrollsFilesGrid = function () {
        return {
            dataSource: DataSourcePayrollFilesGrid,
            keyExpr: "ID",
            bindingOptions: {
                rtlEnabled: true
            },
            sorting: {
                mode: "multiple"
            },
            wordWrapEnabled: false,
            showBorders: false,
            searchPanel: {
                width: 900,
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
            scrolling: {
                rtlEnabled: true,
                useNative: true,
                scrollByContent: true,
                scrollByThumb: true,
                showScrollbar: "onHover",
                mode: "standard", // or "virtual"
                direction: "both"
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
                fileName: "Payrolls"
            },
            columns: [
            {
                caption: 'المرحلة',
                cssClass: "text-center",
                dataField: "PhaseName"
            },
            {
                dataField: "InsertDate",
                caption: "تاريخ المرحلة"
            },
            {
                caption: "#",
                cssClass: "text-center",
                cellTemplate: function (container, options) {
                    $("<div />").dxButton({
                        text: 'تنزيل الملف',
                        icon: "fa fa-download",
                        width: 150,
                        type: "default",
                        hint: 'تنزيل الملف',
                        onClick: function (e) {
                            debugger;
                            return window.open('/PayrollStudents/DownloadFile?PayrollApprovalId=' + options.data.ID, '_blank');


                        }
                    }).appendTo(container);
                }
            }
        ]
        };
    };



    $scope.PayrollsGrid = {
        dataSource: DataSourcePayrollsGrid,
        keyExpr: "ID",
        bindingOptions: {
            rtlEnabled: true
        },
        sorting: {
            mode: "multiple"
        },
        wordWrapEnabled: false,
        showBorders: false,
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
            fileName: "Payrolls"
        },
        columns: [
            {
                caption: 'Is Financial ',
                cssClass: "text-center",
                dataField: "IsFinancialApprove",
                visible: false
            },
            {
                caption: 'رقم المسير',
                cssClass: "text-center",
                dataField: "PayrollNumber"
            },
            {
                caption: 'نوع المسير',
                cssClass: "text-center",
                dataField: "MinistrVal"
            },
            {
                dataField: "InsertDate",
                caption: "تاريخ الإصدار"

            },
            {
                dataField: "IssueDate",
                caption: "تاريخ المسير"
            },
            {
                dataField: "TotalAmount",
                caption: "إجمالي المبلغ"
            },
            {
                dataField: "AdvancesValues",
                caption: "إجمالي أقساط السلف"
            },
            {
                dataField: "RealValues",
                caption: "صافي المبلغ"
            },
            {
                dataField: "StudentsCount",
                caption: "عدد الطلاب"
            },
            {
                caption: "#",
                cssClass: "text-center",
                cellTemplate: function (container, options) {
                    $("<div />").dxButton({
                        text: 'اعتماد/رد',
                        icon: "fa fa-check",
                        width: 150,
                        type: "success",
                        hint: "اقرار",
                        //elementAttr: { "class": "btn btn-success btn-sm" },
                        onClick: function (e) {
                            debugger;
                            $scope.MDL_UploadingFilesvalue = '';
                            if (options.data.IssuingExchangeOrder == true) {
                                $scope.payNoTextVisible = true;
                            } else {
                                $scope.payNoTextVisible = false;
                            }
                            if (options.data.IssuingPaymentOrder == true) {
                                $scope.dafPayNoTextVisible = true;
                                $scope.dafPayNo2TextVisible = true;
                            } else {
                                $scope.dafPayNoTextVisible = false;
                                $scope.dafPayNo2TextVisible = false;
                            }
                            $scope.currentKey = options.key;
                            $scope.payNo = options.data.PayNo;
                            $scope.dafPayNo = options.data.DafPayNo;
                            $scope.dafPayNo2 = options.data.DafPayNo2;
                            $scope.acceptPopupVisible = true;
                            PayrollApprovalService.GetPayrollApprovalHistory($scope.currentKey)
                                .then(function (data) {
                                    if (data.data != "") {
                                        $scope.phase_name = data.data.PhaseName;
                                        $scope.recommendationNotes = data.data.Notes;
                                    } else {
                                        swal("حدث خطأ", data.data, "error");
                                    }
                                });
                        }
                    }).appendTo(container);
                }
            },
            //{
            //    caption: "#",
            //    cssClass: "text-center",
            //    cellTemplate: function (container, options) {
            //        $("<div />").dxButton({
            //            text: 'إستعراض المرفقات',
            //            icon: "fa fa-download",
            //            width: 150,
            //            type: "default",
            //            hint: 'إستعراض المرفقات',
            //            onClick: function (e) { 
            //                $scope.FilesDetailsArray = '';
            //                debugger;
            //                $scope.currentKey = options.key; 
            //                $scope.payrollID = options.key;
            //            /*Omar*/
            //                return $http({
            //                    method: "Get",
            //                    url: "/PayRollStudents/GetPayRollFiles",
            //                    params: {
            //                        payrollID: $scope.payrollID
            //                    }
            //                }).then(function (data) {
            //                    debugger;
            //                    $scope.FilesDetailsArray = data.data;
            //                    DataSourcePayrollFilesGrid.reload(); 
            //                    $scope.filesPopupVisible = true;
            //                    $('body').css('overflow', 'hidden');
            //                });
            //            }
            //        }).appendTo(container);
            //    }
            //},
            //{
            //    caption: "#",
            //    cssClass: "text-center",
            //    cellTemplate: function (container, options) {
            //        $("<div />").dxButton({
            //            text: 'إستعراض تفاصيل المسير البنكيه',
            //            icon: "fa fa-info",
            //            width: 150,
            //            type: "default",
            //            hint: 'إستعراض تفاصيل المسير البنكيه',
            //            onClick: function (e) {
            //                $scope.BankDetailsArray = '';
            //                debugger;
            //                $scope.currentKey = options.key;
            //                $scope.payrollID = options.key;
            //                var d = new $.Deferred();
            //                $http({
            //                    method: "Get",
            //                    url: "/PayRollStudents/GetPayRollsDetails",
            //                    params: {
            //                        payrollID: $scope.payrollID
            //                    }
            //                }).then(function (data) {
            //                    debugger;
            //                    //data1 = JSON.parse(data.data);
            //                    //$scope.BankDetailsArray = data1;
            //                    //DataSourcePayrollBankGrid.reload();
            //                    //d.resolve(data.data, { totalCount: data.data.length });
            //                    //$scope.bankPopupVisible = true;
            //                    //$('body').css('overflow', 'hidden');
            //                    if (data.data == "" || data.data == undefined || data.data == null) {
            //                        return d.promise();
            //                    } else {
            //                        dataAfterJson = JSON.parse(data.data);
            //                        $scope.BankDetailsArray = dataAfterJson;
            //                        DataSourcePayrollBankGrid.reload();
            //                        d.resolve(dataAfterJson, { totalCount: dataAfterJson.length });
            //                        $scope.bankPopupVisible = true;
            //                        $('body').css('overflow', 'hidden');
            //                        return d.promise();
            //                    }
            //                });
            //                return d.promise();
            //            }
            //        }).appendTo(container);
            //    }
            //},
            //{
            //    caption: "#",
            //    cssClass: "text-center",
            //    cellTemplate: function (container, options) {
            //        $("<div />").dxButton({
            //            text: 'إستعراض تفاصيل المسير النقديه',
            //            icon: "fa fa-info",
            //            width: 150,
            //            type: "default",
            //            hint: 'إستعراض تفاصيل المسير النقديه',
            //            onClick: function (e) {
            //                $scope.MoneyDetailsArray = '';
            //                debugger;
            //                $scope.currentKey = options.key;
            //                $scope.payrollID = options.key;
            //                var d = new $.Deferred();
            //                $http({
            //                    method: "Get",
            //                    url: "/PayRollStudents/GetPayRollsMonetaryDetails",
            //                    params: {
            //                        payrollID: $scope.payrollID
            //                    }
            //                }).then(function (data) {
            //                    debugger;
            //                    if (data.data == "" || data.data == undefined || data.data == null) {
            //                        return d.promise();
            //                    } else {
            //                        dataAfterJson = JSON.parse(data.data);
            //                        $scope.MoneyDetailsArray = dataAfterJson;
            //                        DataSourcePayrollMoneyGrid.reload();
            //                        d.resolve(dataAfterJson, { totalCount: dataAfterJson.length });
            //                        $scope.moneyPopupVisible = true;
            //                        $('body').css('overflow', 'hidden');
            //                        return d.promise();
            //                    }
            //                });
            //            }
            //        }).appendTo(container);
            //    }
            //}


        ],
        allowColumnResizing: true,
        columnResizingMode: "widget",
        allowColumnReordering: true,
        showColumnLines: true,
        rowAlternationEnabled: true,
        onInitialized: function (e) { 
        }
    };
     
   
    // Buttons
    $scope.btnBank = {
        text: "بنكي",
        type: "default",
        width: "150",
        onClick: function (options) {
                $scope.BankDetailsArray = '';
                debugger;
                //$scope.currentKey = options.key;
            $scope.payrollID = $scope.currentKey;
                var d = new $.Deferred();
                $http({
                    method: "Get",
                    url: "/PayRollStudents/GetPayRollsDetails",
                    params: {
                        payrollID: $scope.payrollID
                    }
                }).then(function (data) {
                    debugger;
                    if (data.data == "" || data.data == undefined || data.data == null) {
                        return d.promise();
                    } else {
                        debugger;
                        dataAfterJson = JSON.parse(data.data);
                        $scope.BankDetailsArray = dataAfterJson;
                        $scope.bankDataAfterJson = dataAfterJson;
                        DataSourcePayrollBankGrid.reload();
                        d.resolve(dataAfterJson, { totalCount: dataAfterJson.length });
                        $scope.bankPopupVisible = true; 
                        $('body').css('overflow', 'hidden');
                        return d.promise();
                    }
                });
                return d.promise();
            }
    };
    $scope.btnMoney = {
        text: "نقدي",
        type: "default",
        width: "150",
        onClick: function (e) {
            debugger;
            $scope.MoneyDetailsArray = '';
            debugger;
            //$scope.currentKey = options.key;
            $scope.payrollID = $scope.currentKey;
            var d = new $.Deferred();
            $http({
                method: "Get",
                url: "/PayRollStudents/GetPayRollsMonetaryDetails",
                params: {
                    payrollID: $scope.payrollID
                }
            }).then(function (data) {
                debugger;
                if (data.data == "" || data.data == undefined || data.data == null) {
                    return d.promise();
                } else {
                    dataAfterJson = JSON.parse(data.data);
                    $scope.MoneyDetailsArray = dataAfterJson;
                    DataSourcePayrollMoneyGrid.reload();
                    d.resolve(dataAfterJson, { totalCount: dataAfterJson.length });
                    $scope.moneyPopupVisible = true;
                    $('body').css('overflow', 'hidden');
                    return d.promise();
                }
            });
        }
    };


    $scope.btnShowFiles = {
        text: "إستعراض المرفقات",
        type: "default",
        width: "150",
        onClick: function (e) { 
            $scope.FilesDetailsArray = '';
            debugger;
            //$scope.currentKey = options.key; 
            $scope.payrollID = $scope.currentKey;
        /*Omar*/
            return $http({
                method: "Get",
                url: "/PayRollStudents/GetPayRollFiles",
                params: {
                    payrollID: $scope.payrollID
                }
            }).then(function (data) {
                debugger;
                $scope.FilesDetailsArray = data.data;
                DataSourcePayrollFilesGrid.reload(); 
                $scope.filesPopupVisible = true;
                $('body').css('overflow', 'hidden');
            });
        }
    };
    //
    $scope.acceptButton = {
        text: "اعتماد",
        type: "success",
        width: "150",
        onClick: function (e) {
            
            debugger;
           
            if ($scope.MDL_UploadingFilesvalue === '' || $scope.MDL_UploadingFilesvalue === null || $scope.MDL_UploadingFilesvalue === undefined) {
                return DevExpress.ui.notify({ message: " (لابد من رفع الملف المرفق)" }, "Error", 5000);
            }
            if ($scope.payNoTextVisible == true) {

                if ($scope.payNo == '' || $scope.payNo == undefined) {

                    return DevExpress.ui.notify({ message: " (لابد من كتابة أمر الصرف)" }, "Error", 5000);
                }
            }

            if ($scope.dafPayNoTextVisible == true) {
                if ($scope.dafPayNo == '' || $scope.dafPayNo == undefined) {

                    return DevExpress.ui.notify({ message: " (لابد من كتابة أمر الدفع)" }, "Error", 5000);
                }
            }
            if ($scope.dafPayNo2TextVisible == true) {
                if ($scope.dafPayNo2 == '' || $scope.dafPayNo2 == undefined) {

                    return DevExpress.ui.notify({ message: " (لابد من كتابة أمر الدفع(2))" }, "Error", 5000);
                }
            }
            PayrollApprovalService.PayrollApprovallAction($scope.currentKey, $scope.recommendationNotes, $scope.payNo, $scope.dafPayNo, $scope.dafPayNo2 , true)
                .then(function (data) {
                    if (data.data === "") {
                        DataSourcePayrollsGrid.reload();
                        swal("تم!", "تم الاعتماد بنجاح", "success").then((value) => {
                            $scope.ResetControls();
                            return value;
                        });

                    } else {
                        swal("حدث خطأ", data.data, "error");
                    }
                });
        }
    };
    $scope.refuseButton = {
        text: "رد",
        type: "danger",
        width: "150",
        useSubmitBehavior: true,
        onClick: function (e) {
            debugger;
            if ($scope.recommendationNotes == '' || $scope.recommendationNotes == undefined) {

                return DevExpress.ui.notify({ message: " (لابد من كتابة ملاحظات الرفض)" }, "Error", 5000);
            }
            PayrollApprovalService.PayrollApprovallAction($scope.currentKey, $scope.recommendationNotes, "", "", false)
                .then(function (data) {
                    if (data.data === "") {
                        DataSourcePayrollsGrid.reload();
                        swal("تم!", "تم  رد الاعتماد بنجاح", "success").then((value) => {
                            $scope.ResetControls();
                            return value;
                        });

                    } else {
                        swal("حدث خطأ", data.data, "error");
                    }
                });
        }
    };
    // Popup ...
    $scope.acceptPopup = {
        bindingOptions: {
            visible: "acceptPopupVisible"
        },
        shadingColor: "rgba(0, 0, 0, 0.69)",
        closeOnOutsideClick: false,
        //height: 500,
        width: 1200,
        showTitle: true,
        title: "اعتماد / رد لإعادة الاحتساب",
        dragEnabled: false,
        onHiding: function (options) {
            debugger;
            $scope.currentKey = null;
            $scope.ResetControls();
            //options.cancel = true;
            //$scope.listOptions($scope.currentKey);
        },
        onShown: function (options) {
            debugger;
            $scope.MDL_UploadingFilesvalue = '';
            //$scope.currentKey = options.key;
            //$scope.FileUploadingOptionsInstance.reset();
        },
        UploadFilePopupOptions: {
            showTitle: true,
            dragEnabled: false,
            shadingColor: "rgba(0, 0, 0, 0.69)",
            closeOnOutsideClick: false,
            bindingOptions: {
                visible: "UploadPopUpShow"
            },
            contentTemplate: 'UploadFileContent',
            title: "رفع الملف",
            width: 800,
            height: 500,
            rtlEnabled: true,
            //onHiding: function () {
            //    $scope.MDL_UploadingFilesvalue = '';
            //},
            onShowing: function () {
                //debugger;
                //$scope.MDL_UploadingFilesvalue = '';
            }
        },
        bankPopup: {
            showTitle: true,
            dragEnabled: false,
            shadingColor: "rgba(0, 0, 0, 0.69)",
            closeOnOutsideClick: true,
            bindingOptions: {
                visible: "bankPopupVisible"
            },
            title: "بنكي",
            width: 1200,
            height: 900,
            rtlEnabled: true
        },
        moneyPopup: {
            showTitle: true,
            dragEnabled: false,
            shadingColor: "rgba(0, 0, 0, 0.69)",
            closeOnOutsideClick: true,
            bindingOptions: {
                visible: "moneyPopupVisible"
            },
            title: "نقدي",
            width: 1200,
            height: 900,
            rtlEnabled: true
        },
        filesPopup: {
            showTitle: true,
            dragEnabled: false,
            shadingColor: "rgba(0, 0, 0, 0.69)",
            closeOnOutsideClick: false,
            bindingOptions: {
                visible: "filesPopupVisible"
            },
            height: 500,
            title: "تحميل المرفقات للمراحل السابقه"
        }
    };
    $scope.recommendationsTextArea = {
        bindingOptions: {
            required: "isRequiredNotes",
            value: "recommendationNotes"
        },
        inputAttr: {"rows":10},
        AutoResizeEnabled: true,
        placeholder:"ملاحظات"
    };
    $scope.payNoText = {
        bindingOptions: {
            required: "isRequiredpayNo",
            value: "payNo",
            visible: "payNoTextVisible"
        }, 
        AutoResizeEnabled: true,
        placeholder: "رقم أمر الصرف"
    };
    $scope.dafPayNoText = {
        bindingOptions: {
            value: "dafPayNo",
            visible: "dafPayNoTextVisible"
        },
        AutoResizeEnabled: true,
        placeholder: "رقم أمر الدفع"
    };
    $scope.dafPayNo2Text = {
        bindingOptions: {
            value: "dafPayNo2",
            visible: "dafPayNo2TextVisible"
        },
        AutoResizeEnabled: true,
        placeholder: "رقم أمر الدفع (2)"
    };
    //$scope.filesPopup = {
    //    bindingOptions: {
    //        visible: "filesPopupVisible"
    //    },
    //    shadingColor: "rgba(0, 0, 0, 0.69)",
    //    closeOnOutsideClick: false,
    //    height: 500,
    //    showTitle: true,
    //    title: "تحميل المرفقات للمراحل السابقه",
    //    dragEnabled: false
    //};
    //==========================================================
    // الجزء الخاص برفع المرفقات والتابعة لشاشة إضافة مشاركة
    //==========================================================
    $scope.uploadMode = "useButtons";
    $scope.btnUploadText = "رفع المرفق";
    $scope.btnUploadOptions = {
        bindingOptions: {
            text: 'btnUploadText',
            icon: 'btnUploadIcon'
        },
        visible: true,
        type: 'default',
        useSubmitBehavior: false,
        onClick: function (e) {
            
            $scope.MDL_UploadingFilesvalue = '';
            $scope.UploadingFilesvalue = [];  
            $scope.UploadPopUpShow = true;
        }
    };

    //Upload Options...
    $scope.multiple = false;
    $scope.accept = ".pdf";
    $scope.btnUploadIcon = "upload";
    $scope.UploadingFilesvalue = [];
    $scope.FileUploadingOptions = {
        name: "fileSent",
        uploadUrl: "/PayRollStudents/UploadFiles",
        allowCanceling: true,
        rtlEnabled: true,
        readyToUploadMessage: "استعداد للرفع",
        selectButtonText: "رفع المرفق",
        labelText: "",
        uploadMode: "useButtons",
        uploadButtonText: "رفع",
        uploadedMessage: "تم الرفع",
        invalidFileExtensionMessage: "نوع الملف غير مسموح",
        uploadFailedMessage: "نوع الملف غير مسموح",
        allowedFileExtensions: [".pdf"],
        accept: ".pdf",
        multiple: false,
        bindingOptions: {
            value: "UploadingFilesvalue"
        },
        onValueChanged: function (e) {
            debugger;
            if (e.value.length > 0) {
                if (e.value[0].type === "application/pdf") {
                    $scope.MDL_UploadingFilesvalue = e.value;
                } else {
                    $scope.MDL_UploadingFilesvalue = '';
                    $scope.FileUploadingOptionsInstance.reset();
                    return swal("تنبيه", "غير مسموح برفع هذا النوع من الملفات يسمح فقط بإمتداد" + ".pdf", "warning");
                }
            } else {
                $scope.MDL_UploadingFilesvalue = e.value;
            }
        },
        onInitialized: function (e) {
            debugger;
            //$scope.MDL_UploadingFilesvalue = '';
            $scope.FileUploadingOptionsInstance = e.component;
        },
        onUploaded: function (e) {
            debugger;
            if (e.request.status === 200) {
                debugger;
                $scope.UploadPopUpShow = false;
                $scope.btnUploadText = "تم رفع المرفق بنجاح";
                $scope.btnUploadIcon = "check";
              
            }
            if (e.request.status === 400) {
                $scope.UploadPopUpShow = false;
                return DevExpress.ui.notify({ message: "غير مسموح برفع هذا الملف" }, "Error", 10000);
            }

        }
    };

    //Remove Uploaded File...
    $scope.RemoveUploadingFile = function (hashkey) {
        $scope.MDL_UploadingFilesvalue = '';
        $scope.FileUploadingOptionsInstance.reset();
        $http({
            method: "POST",
            url: "/PayRollStudents/UploadFiles"
        });
        $scope.btnUploadText = "رفع المرفق";
        $scope.btnUploadIcon = "upload";
    };
    $scope.ResetUploadingControls = function () {

        $scope.MDL_UploadingFilesvalue = '';
        if ($scope.FileUploadingOptionsInstance) {
            $scope.FileUploadingOptionsInstance.reset();
        }
    };

    $scope.validationRequired = {
        validationRules: [
            {
                type: "required",
                message: "الحقل مطلوب"
            }
        ]
    };

    $scope.ResetControls = function () { 
        $scope.MDL_UploadingFilesvalue = '';
        $scope.btnUploadIcon = "upload";
        $scope.btnUploadText = "رفع المرفق";
        $scope.acceptPopupVisible = false;
        $scope.isRequiredNotes = false;
        $scope.isRequiredpayNo = false;
        $scope.currentKey = 0;
        $scope.FilesDetailsArray = '';
       // $scope.recommendationNotes = '';
        $scope.MDL_UploadingFilesvalue = '';
        $scope.payrollID = null;
    };



}
 ]);
