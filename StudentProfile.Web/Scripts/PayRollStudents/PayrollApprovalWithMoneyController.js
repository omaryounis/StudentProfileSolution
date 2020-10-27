app.controller("PayrollApprovalWithMoneyController", ["$scope", "$http", "PayrollApprovalWithMoneyService", function ($scope, $http, PayrollApprovalService) {
 
    $scope.ClearControls = function () {
        $scope.acceptPopupVisible = false;
        $scope.isRequiredNotes = false;
        $scope.isRequiredpayNo = false;
        $scope.currentKey = 0;
        $scope.recommendationNotes = '';
        $scope.payNo = '';
        $scope.dafPayNo = '';
        $scope.MDL_UploadingFilesvalue = '';
        //$scope.acceptPopup.hide();
    }
    $scope.ClearControls();
    var DataSourcePayrollsGrid = new DevExpress.data.DataSource({
        paginate: true,
        cacheRawData: true,
        key: "PayrollID",
        loadMode: "raw",
        load: function () {
            return $.getJSON("/PayRollStudents/GetPayRollsMoney", function (data) {
                debugger;
              //  $scope.payNo = data.data.PayNumber;
               // $scope.dafPayNo = data.data.DafPayNumber;
            });
        }
    });


    $scope.PayrollsGrid = {
        dataSource: DataSourcePayrollsGrid,
        keyExpr: "PayrollID",
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
                caption: 'رقم المسير',
                cssClass: "text-center",
                dataField: "PayrollNumber"
            },

            {
                dataField: "PayNumber",
                caption: "رقم أمر الصرف"

            },
            {
                dataField: "DafPayNumber",
                caption: "رقم أمر الدفع"
            },
            {
                dataField: "PayrollMoneyValue",
                caption: "قيمة المسير النقدي"
            },
            {
                dataField: "StudentNumber",
                caption: "عدد الطلاب"
            },
            {
                dataField: "LastStageDate",
                caption: "تاريخ أخر مرحلة إعتماد"
            }
            , {
                caption: "#",
                cssClass: "text-center",
                cellTemplate: function (container, options) {
                    $("<div />").dxButton({
                        text: 'التفاصيل',
                        icon: "fa fa-info",
                        width: 150,
                        type: "success",
                        hint: "التفاصيل",
                        onClick: function (e) {
                            debugger;
                            $scope.ResetControls();
                            $scope.currentKey = options.key;
                            return $http({
                                method: "Get",
                                url: "/PayRollStudents/GetPayRollsMoneyDetails",
                                params: {
                                    payrollID: options.data.PayrollID
                                }
                            }).then(function (data) {
                                debugger;
                                $scope.PayRollsMoneyDetails = data.data;
                                $scope.payNo = data.data[0].PayNumber;
                                $scope.dafPayNo = data.data[0].DafPayNumber;
                                //***************
                                $scope.PayRollsMoneyDetailsPopupVisible = true;
                                PayRollsMoneyDataGridDataSource.reload();
                                $('body').css('overflow', 'hidden');
                            });
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
        onInitialized: function (e) {
        }
    };
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

    $scope.PayRollsMoneyDetails = '';
    var PayRollsMoneyDataGridDataSource = new DevExpress.data.DataSource({
        paginate: true,
        cacheRawData: false,
        loadMode: "raw",
        key: "PayrollID",
        load: function () {
            if ($scope.PayRollsMoneyDetails.length > 0) {
                debugger;
                return $scope.PayRollsMoneyDetails;
            } else {
                return [];
            }
        }
    }); 

    $scope.PayRollsMoneyDataGrid = function () {
        return {
            dataSource: PayRollsMoneyDataGridDataSource,
            "export": {
                enabled: true,
                fileName: "تفاصيل المسير مع مسؤولي الصراف"
            },
            sorting: {
                mode: "multiple"
            },
            columnAutoWidth: true,
            columnChooser: {
                enabled: true
            },
            wordWrapEnabled: false,
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
                visible: false
            },
            showRowLines: true,
            groupPanel: {
                visible: false,
                emptyPanelText: "اسحب عمود هنا"
            },
            paging: {
                pageSize: 5
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
            rowAlternationEnabled: true,
            columns: [
                {
                    dataField: "Name",
                    caption: "مندوب الصرف",
                    alignment: "center",
                    width: 200
                },
                {
                    dataField: "PayrollNumber",
                    caption: "رقم المسير",
                    alignment: "center",
                    width: 130
                },
                {
                    dataField: "PayNumber",
                    caption: "رقم أمر الصرف",
                    alignment: "center",
                    width: 180
                }, {
                    dataField: "DafPayNumber",
                    caption: "رقم أمر الدفع",
                    alignment: "center",
                    width: 130
                },
                {
                    dataField: "StudentNumber",
                    caption: "عدد الطلاب",
                    alignment: "center",
                    width: 130
                },
                {
                    dataField: "PayrollMoneyValue",
                    caption: "الإجمالي",
                    alignment: "center",
                    width: 120
                },
                {
                    caption: "التفاصيل",
                    cssClass: "text-center",

                    cellTemplate: function (container, options) {
                        $("<div />").dxButton({
                            text: '',
                            icon: "fa fa-info",
                            width: 50,
                            type: "success",
                            hint: "التفاصيل",
                            onClick: function (e) {
                                debugger;
                                $scope.ResetControls();
                                $scope.currentKey = options.key;
                                $scope.PayRollsMoneyUserDetailsPopupVisible = true;
                                return $http({
                                    method: "Get",
                                    url: "/PayRollStudents/GetPayRollsMoneyUsersDetails",
                                    params: {
                                        payrollID: options.data.PayrollID,
                                        UserID: options.data.UserID
                                    }
                                }).then(function (data) {
                                    debugger;
                                    $scope.PayRollsMoneyUsersDetails = data.data;
                                    PayRollsMoneyUsersDataGridDataSource.reload();
                                    //***************
                                    $scope.PayRollsMoneyUserDetailsPopupVisible = true;
                                    $('body').css('overflow', 'hidden');
                                });
                            }
                        }).appendTo(container);
                    }
                }],
            summary: {
                totalItems: [{
                    column: "StudentNumber",
                    summaryType: "sum",
                    displayFormat: "الإجمالي: {0}",
                }, {
                    column: "PayrollMoneyValue",
                        summaryType: "sum",
                    displayFormat: "الإجمالي: {0}",
                }]
            }
        };
    };
     
    $scope.PayRollsMoneyUsersDetails = '';



    var PayRollsMoneyUsersDataGridDataSource = new DevExpress.data.DataSource({
        paginate: true,
        cacheRawData: false,
        loadMode: "raw",
        key: "PayrollID",
        load: function () {
            if ($scope.PayRollsMoneyUsersDetails.length > 0) {
                debugger;
                return $scope.PayRollsMoneyUsersDetails;
            } else {
                return [];
            }
        }
    });
    $scope.PayRollsMoneyUserDetailsGrid = function () {
        return {
            dataSource: PayRollsMoneyUsersDataGridDataSource,
            keyExpr: "ID",
            sorting: {
                mode: "multiple"
            },
            columnAutoWidth: true,
            columnChooser: {
                enabled: true
            },
            wordWrapEnabled: false,
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
                visible: false
            },
            showRowLines: true,
            groupPanel: {
                visible: false,
                emptyPanelText: "اسحب عمود هنا"
            },
            paging: {
                pageSize: 5
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
            rowAlternationEnabled: true,
            "export": {
                enabled: true,
                fileName: "مسؤول الصرافة وربطه بالكليات والمراحل العلمية للمسير"
            },
            columns: [
                {
                    caption: 'مندوب الصراف',
                    cssClass: "text-center",
                    dataField: "name",
                    groupIndex: 0
                },
                {
                    dataField: "PayrollNumber",
                    caption: "رقم المسير"
                },
                {
                    dataField: "faculty_name",
                    caption: "الكليه"
                },
                {
                    dataField: "DEGREE_DESC",
                    caption: "المرحله العلمية"
                },
                {
                    dataField: "PayrollMoneyValue",
                    caption: "قيمة المسير"
                },
                {
                    dataField: "StudentNumber",
                    caption: "عدد الطلاب"
                }
            ],
            summary: {
                totalItems: [{
                    column: "StudentNumber",
                    summaryType: "sum",
                    displayFormat: "الإجمالي: {0}",
                }, {
                        column: "PayrollMoneyValue",
                        summaryType: "sum",
                    displayFormat: "الإجمالي: {0}",
                }
                    // ...
                ]
            }
                //groupItems: [{
                //   column: "StudentNumber",
                //    summaryType: "count",
                //    displayFormat: "Total: {0}",
                //    showInGroupFooter: true,

                //},{
                //        column: "PayrollMoneyValue",
                //        summaryType: "sum",
                //        displayFormat: "Total: {0}",
                //        showInGroupFooter: true,

                //    }]
        };
    };

     
/*omar*/
     
    $scope.PayRollsMoneyUserDetailsPopup = {
        bindingOptions: {
            visible: "PayRollsMoneyUserDetailsPopupVisible"
        },
        shadingColor: "rgba(0, 0, 0, 0.69)",
        closeOnOutsideClick: true,
        height: 600,
        width: 1300,
        showTitle: false,
        title: "",
        dragEnabled: false,
        onHiding: function () {
            debugger;
        }
    };




    $scope.PayRollsMoneyDetailsPopup = {
        bindingOptions: {
            visible: "PayRollsMoneyDetailsPopupVisible"
        },
        shadingColor: "rgba(0, 0, 0, 0.69)",
        closeOnOutsideClick: true,
        height: 600,
        width: 1100,
        showTitle: false,
        title: "",
        dragEnabled: false,
        onHiding: function () {
            debugger;
            $scope.PayRollsMoneyDetails = [];
            $scope.currentKey = null;
            $scope.ResetControls();
            DataSourcePayrollsGrid.reload();
            $('body').css('overflow', 'auto');
        },
        onShown: function (options) {
            debugger;
            $scope.MDL_UploadingFilesvalue = '';
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
            onShowing: function () {
            }
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
        },
        textAreaPopup: {
            showTitle: true,
            dragEnabled: false,
            shadingColor: "rgba(0, 0, 0, 0.69)",
            closeOnOutsideClick: false,
            bindingOptions: {
                visible: "textAreaVisible"
            },
            title: "الملاحظات",
            width: 1200,
            height: 500,
            rtlEnabled: true
        }
        //,PayRollsMoneyUserDetailsPopup : {
        //    bindingOptions: {
        //        visible: "PayRollsMoneyUserDetailsPopupVisible"
        //    },
        //    shadingColor: "rgba(0, 0, 0, 0.69)",
        //    closeOnOutsideClick: true,
        //    height: 600,
        //    width: 1300,
        //    showTitle: false,
        //    title: "",
        //    dragEnabled: false,
        //    onHiding: function () {
        //        debugger;
        //    }
        //}
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
            $scope.recommendationNotes = '';
            $scope.textAreaVisible = true;
        }
    };


    $scope.saveButton = {
        text: "حفظ",
        type: "success",
        width: "150",
        onClick: function (e) {
            debugger;
            if (($scope.MDL_UploadingFilesvalue === '' || $scope.MDL_UploadingFilesvalue === null || $scope.MDL_UploadingFilesvalue === undefined)
                || ($scope.IsUploaded != true || $scope.IsUploaded === undefined || $scope.IsUploaded === null))
             {
                 return DevExpress.ui.notify({ message: " (لابد من رفع الملف المرفق)" }, "Error", 5000);
             }
            
            if ($scope.recommendationNotes == '' || $scope.recommendationNotes == undefined) {

                return DevExpress.ui.notify({ message: " (لابد من كتابة الملاحظات)" }, "Error", 5000);
            }
            PayrollApprovalService.PayrollApprovallAction($scope.currentKey, $scope.recommendationNotes, $scope.payNo, $scope.dafPayNo,true,false)
                .then(function (data) {
                    if (data.data === "") {
                        $scope.textAreaVisible = false;
                        $scope.PayRollsMoneyDetailsPopupVisible = false;
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
    $scope.filesPopup = {
        bindingOptions: {
            visible: "filesPopupVisible"
        },
        shadingColor: "rgba(0, 0, 0, 0.69)",
        closeOnOutsideClick: false,
        height: 500,
        showTitle: true,
        title: "تحميل المرفقات للمراحل السابقه",
        dragEnabled: false
    };
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
            debugger;
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
                $scope.IsUploaded = true;
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
