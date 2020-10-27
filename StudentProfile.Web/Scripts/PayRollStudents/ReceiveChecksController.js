app.controller("ReceiveChecksCtrl", ["$scope", "$http", 'ReceiveChecksSrvc', function ($scope, $http, ReceiveChecksSrvc) {

    //Filed
    $scope.CheckDataList = [];
    $scope.RevisionPassportUser = [];
    $scope.gridSelectedRowKeys = [];
    $scope.IsApproved = true;

    $scope.CurrentUniversityStudentId = null;

    $scope.PopUpDocumentsShow = false;

    $scope.PayrollList = [];
    $scope.PayrollID = null;
    $scope.validationRequired = {
        validationRules: [
            {
                type: "required",
                message: "الحقل مطلوب"
            }
        ]
    };

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
            //Get Users
            ReceiveChecksSrvc.GetPayrollWithNotReceiveChecksYet(e.value).then(function (data) {
                $scope.PayrollList = data.data;
            });
        },
        onValueChanged: function (e) {
            $scope.PayrollID = e.value;
            //$scope.DegreeList = '';
            ReceiveChecksSrvc.GetUsers($scope.PayrollID).then(function (data) {
                debugger;
                $scope.UserList = data.data;
            });

        }
    };
    $scope.UsersSelectBox = {
        bindingOptions: {
            value: "UserID",
            items: "UserList"
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
            if ($scope.PayrollID != null) {
                ReceiveChecksSrvc.GetUsers($scope.PayrollID).then(function (data) {
                    debugger;
                    $scope.UserList = data.data;
                });
            }
        },
        onClick: function (e) {
            debugger;
            if ($scope.PayrollID != null) {
                ReceiveChecksSrvc.GetUsers($scope.PayrollID).then(function (data) {
                    debugger;
                    $scope.UserList = data.data;
                });
            }
        },
        onClosed: function (e) {
            debugger;
            $scope.UserID = e.model.UserID;
        }
    }; 

    //Popup Documents
    

    $scope.btnSearch = {
        text: 'بحث',
        type: 'primary',
        onClick: function (e) {
            debugger;
            $scope.IsApproved = true;
            if ($scope.PayrollID == null || $scope.PayrollID == '' || $scope.PayrollID == undefined) {
                return DevExpress.ui.notify({ message: "يجب إختيار المسير" }, "Error", 5000);
            }
            if ($scope.UserID == null || $scope.UserID == '' || $scope.UserID == undefined) {
                return DevExpress.ui.notify({ message: "يجب إختيار مندوب الصرف" }, "Error", 5000);
            }
            ReceiveChecksSrvc.GetChecksData($scope.UserID, $scope.PayrollID).then(function (data) {
                $scope.CheckDataList = data.data;
                GridDataSource.reload();
            });
        }
    };


    //$scope.gridCheckData = function () {
    //    return {
    //        dataSource: GridDataSource,
    //        keyExpr: "ID",
    //        sorting: {
    //            mode: "multiple"
    //        },
    //        paginate: true,
    //        columnAutoWidth: true,
    //        columnChooser: {
    //            enabled: true
    //        },
    //        wordWrapEnabled: true,
    //        showBorders: true,
    //        pager: {
    //            allowedPageSizes: "auto",
    //            infoText: "(صفحة  {0} من {1} ({2} عنصر",
    //            showInfo: true,
    //            showNavigationButtons: true,
    //            showPageSizeSelector: true,
    //            visible: true
    //        },
    //        headerFilter: {
    //            visible: false
    //        },
    //        showRowLines: true,
    //        groupPanel: {
    //            visible: false,
    //            emptyPanelText: "اسحب عمود هنا"
    //        },
    //        paging: {
    //            pageSize: 5
    //        },
    //        filterRow: {
    //            visible: true,
    //            operationDescriptions: {
    //                between: "بين",
    //                contains: "تحتوى على",
    //                endsWith: "تنتهي بـ",
    //                equal: "يساوى",
    //                greaterThan: "اكبر من",
    //                greaterThanOrEqual: "اكبر من او يساوى",
    //                lessThan: "اصغر من",
    //                lessThanOrEqual: "اصغر من او يساوى",
    //                notContains: "لا تحتوى على",
    //                notEqual: "لا يساوى",
    //                startsWith: "تبدأ بـ"
    //            },
    //            resetOperationText: "الوضع الافتراضى"
    //        },
    //        noDataText: "لا يوجد بيانات",
    //        editing: {
    //            allowUpdating: false,
    //            allowAdding: false,
    //            allowDeleting: false,
    //            mode: "row",
    //            texts: {
    //                confirmDeleteMessage: "",
    //                deleteRow: "",
    //                editRow: "",
    //                addRow: ""
    //            },
    //            useIcons: true
    //        },
    //        allowColumnResizing: true,
    //        columnResizingMode: "widget",
    //        allowColumnReordering: true,
    //        showColumnLines: true,
    //        rowAlternationEnabled: false,
    //        "export": {
    //            enabled: true,
    //            fileName: "تفاصيل اصدار الشيكات"
    //        },
    //        columns: [
    //            {
    //                caption: "مندوب الصرف",
    //                dataField: "Name",
    //                cssClass: "text-right",
    //                groupIndex: 0

    //            },
    //            {
    //                caption: "رقم المسير",
    //                dataField: "PayrollNumber",
    //                cssClass: "text-right"
    //            },
    //            {
    //                caption: "رقم أمر الدفع",
    //                dataField: "DafPayNumber",
    //                cssClass: "text-right"
    //            },
    //            {
    //                caption: "رقم أمر الصرف",
    //                dataField: "PayNumber",
    //                cssClass: "text-right"
    //            },
    //            {
    //                caption: "رقم الشيك",
    //                dataField: "CheckNumber",
    //                cssClass: "text-right"
    //            },
    //            {
    //                caption: "قيمة الشيك",
    //                dataField: "checkvalue",
    //                cssClass: "text-right"
    //            },
    //            {
    //                caption: "تاريخ إصدار الشيك",
    //                dataField: "Exportdate",
    //                cssClass: "text-right"
    //            },
    //            {
    //                caption: "بيان الشيك ",
    //                dataField: "Description",
    //                cssClass: "text-right"
    //            },
    //            {
    //                caption: "تحميل مرفق الشيك",
    //                width: 200,
    //                cssClass: "text-center",
    //                cellTemplate: function (container, options) {
    //                    $("<div />").dxButton({
    //                        //icon: "fa fa-save",
    //                        icon: "fa fa-download",
    //                        type: "default",
    //                        hint: "تحميل مرفق الشيك ",
    //                        elementAttr: { "class": "btn btn-primary" },
    //                        onClick: function (e) {
    //                            return window.open('/PayrollStudents/DownloadCheckFile?ID=' + options.data.ID, '_blank');
    //                        }
    //                    }).appendTo(container);
    //                }
    //            }
    //        ],

    //        onContentReady: function (e) {
    //            e.component.columnOption("command:edit", {
    //                visibleIndex: -1
    //            });
    //        },
    //        onSelectionChanged: function (info) {
    //            debugger;
    //            $scope.gridSelectedRowKeys = [];
    //            for (var i = 0; i < info.selectedRowKeys.length; i++)
    //                $scope.gridSelectedRowKeys.push(info.selectedRowKeys[i].ID, info.selectedRowKeys[i].PayrollID);
    //        }

    //    };
    //};


    //dataGrid
    $scope.gridCheckData = function () {
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
                pageSize: 5
             },
            selection: {
            mode: "multiple"
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
            "export": {
                enabled: true,
                fileName: "الشيكات الغير مستلمة"
            },
            columns: [
                {
                    caption: "مندوب الصرف",
                    dataField: "Name",
                    cssClass: "text-right",
                    groupIndex: 0

                },
                {
                    caption: "رقم المسير",
                    dataField: "PayrollNumber",
                    cssClass: "text-right"
                },
                {
                    caption: "رقم أمر الدفع",
                    dataField: "DafPayNumber",
                    cssClass: "text-right"
                },
                {
                    caption: "رقم أمر الصرف",
                    dataField: "PayNumber",
                    cssClass: "text-right"
                },
                {
                    caption: "رقم الشيك",
                    dataField: "CheckNumber",
                    cssClass: "text-right"
                },
                {
                    caption: "قيمة الشيك",
                    dataField: "checkvalue",
                    cssClass: "text-right"
                },
                {
                    caption: "تاريخ إصدار الشيك",
                    dataField: "Exportdate",
                    cssClass: "text-right"
                },
                {
                    caption: "بيان الشيك ",
                    dataField: "Description",
                    cssClass: "text-right"
                },
                {
                    caption: "تحميل مرفق الشيك",
                    width: 200,
                    cssClass: "text-center",
                    cellTemplate: function (container, options) {
                        $("<div />").dxButton({
                            //icon: "fa fa-save",
                            icon: "fa fa-download",
                            type: "default",
                            hint: "تحميل مرفق الشيك ",
                            elementAttr: { "class": "btn btn-primary" },
                            onClick: function (e) {
                                return window.open('/PayrollStudents/DownloadCheckFile?ID=' + options.data.ID, '_blank');
                            }
                        }).appendTo(container);
                    }
                }
            ],

            onContentReady: function (e) {
                e.component.columnOption("command:edit", {
                    visibleIndex: -1
                });
            },
            onSelectionChanged: function (info) {
                debugger;
                $scope.gridSelectedRowKeys = [];
                for (var i = 0; i < info.selectedRowKeys.length; i++)
                    $scope.gridSelectedRowKeys.push(info.selectedRowKeys[i].ID);
            }
        }

    };

    $scope.btnApprovedAll = {
        text: 'تسليم الشيكات',
        type: 'success',
        onClick: function (e) {
            debugger;
            if ($scope.gridSelectedRowKeys.length === 0)
                return DevExpress.ui.notify({ message: 'اختر شيكات الإستلام', type: 'error', displayTime: 3000, closeOnClick: true });
            $scope.acceptPopupVisible = true;
        }
    };

    var GridDataSource = new DevExpress.data.DataSource({
        paginate: true,
        cacheRawData: false,
        loadMode: "raw",
        load: function () {
            if ($scope.CheckDataList.length > 0) {
                debugger;
                return $scope.CheckDataList;
            } else {
                return [];
            }
        }
    });

    $scope.acceptPopup = {
        bindingOptions: {
            visible: "acceptPopupVisible"
        },
        shadingColor: "rgba(0, 0, 0, 0.69)",
        closeOnOutsideClick: false,
        height: 200,
        width:700,
        showTitle: true,
        title: " إستلام الشيكات",
        dragEnabled: false,
        onHiding: function (options) {
            debugger;
            $scope.currentKey = null;
            $scope.ResetControls();
            GridDataSource.reload();
            // $scope.gridCheckData.reload();
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
        }
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
        uploadUrl: "/PayRollStudents/UploadSandFiles",
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
            url: "/PayRollStudents/UploadSandFiles"
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
    };

    $scope.acceptButton = {
        text: "تسليم الشيكات",
        type: "success",
        width: "150",
        onClick: function (e) {

            debugger;

            if ($scope.MDL_UploadingFilesvalue === '' || $scope.MDL_UploadingFilesvalue === null || $scope.MDL_UploadingFilesvalue === undefined) {
                return DevExpress.ui.notify({ message: " (لابد من رفع الملف المرفق)" }, "Error", 5000);
            }
            ReceiveChecksSrvc.UpdateChecksState($scope.gridSelectedRowKeys, $scope.PayrollID)
                .then(function (data) {
                    if (data.data === "") {
                        swal("تم!", "تم تسليم الشيك", "success").then((value) => {
                            $scope.ResetControls();
                            $scope.acceptPopupVisible.visible = false;
                            location.reload();
                            //ReceiveChecksSrvc.GetChecksData($scope.UserID, $scope.PayrollID).then(function (data) {
                            //    $scope.CheckDataList = data.data;
                            //    GridDataSource.reload();
                            //});
                            GridDataSource.reload();
                            return value;
                        });

                    } else {
                        swal("حدث خطأ", data.data, "error");
                    }
                });
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



}]);
