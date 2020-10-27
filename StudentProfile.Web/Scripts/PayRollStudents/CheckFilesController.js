app.controller("CheckFilesCtrl", ["$scope", "$http", 'CheckFilesSrvc', function ($scope, $http, CheckFilesSrvc) {

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
        displayExpr: "Text",
        valueExpr: "Value",
        paginate: true, //Pagenation
        selectAllMode: "page",
        showBorders: true,
        searchEnabled: true,
        showSelectionControls: true,
        maxDisplayedTags: 2,
        showMultiTagOnly: false,
        onInitialized: function (e) {
            //Get Users
            CheckFilesSrvc.GetPayrollWithNotReceiveChecksYet(e.value).then(function (data) {
                $scope.PayrollList = data.data;
            });
        },
        onValueChanged: function (e) {
            $scope.PayrollID = e.value.join(',');
            
            CheckFilesSrvc.GetUsers($scope.PayrollID).then(function (data) {
                debugger;
                if (data.data.length > 0) {
                    $scope.UserList = data.data;
                }
                else {
                    $scope.UserList = '';
                    $("#UsersSelectBox").dxTagBox("option", "dataSource", '');

                }
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
        displayExpr: "Text",
        valueExpr: "Value",
        paginate: true, //Pagenation
        selectAllMode: "page",
        showBorders: true,
        searchEnabled: true,
        showSelectionControls: true,
        maxDisplayedTags: 2,
        showMultiTagOnly: false,
        onValueChanged: function (e) {
            debugger;
            if (e.value != '' || e.value != undefined || e.value != null) {

                if (e.value.length > 1) {
                    $scope.UserID = e.value.join(',');
                } else {
                    $scope.UserID = e.value;
                }
            }
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
            CheckFilesSrvc.GetChecksData($scope.UserID, $scope.PayrollID).then(function (data) {
                $scope.CheckDataList = data.data;
                GridDataSource.reload();
            });
        }
    };


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
                    caption: "تنزيل مرفق الشيك",
                    width: 200,
                    cssClass: "text-center",
                    cellTemplate: function (container, options) {

                        if (options.data.Filepath != null) {

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
                },
                {
                    caption: "رفع مرفق الشيك",
                    width: 200,
                    cssClass: "text-center",
                    cellTemplate: function (container, options) {


                            $("<div />").dxButton({
                                //icon: "fa fa-save",
                                icon: "fa fa-upload",
                                type: "default",
                                hint: "رفع مرفق الشيك ",
                                elementAttr: { "class": "btn btn-primary" },
                                onClick: function (e) {
                                    $scope.MDL_UploadingFilesvalue = '';
                                    if ($scope.FileUploadingOptionsInstance) {
                                        $scope.FileUploadingOptionsInstance.reset();
                                    }
                                    $scope.ID = options.data.ID;
                                    $('#UploadFilePopupOptions').dxPopup('instance').option('visible', true);
                                }
                            }).appendTo(container);
                        }
                }
            ]
        }

    };

    //$scope.btnApprovedAll = {
    //    text: 'تسليم الشيكات',
    //    type: 'success',
    //    onClick: function (e) {
    //        debugger;
    //        if ($scope.gridSelectedRowKeys.length === 0)
    //            return DevExpress.ui.notify({ message: 'اختر شيكات الإستلام', type: 'error', displayTime: 3000, closeOnClick: true });
    //        $scope.acceptPopupVisible = true;
    //    }
    //};
    $scope.SaveUploadingFile = function (hashkey) {
        debugger;
        //$scope.MDL_UploadingFilesvalue1 = '';
        //$scope.FileUploadingOptionsInstance1.reset();
        $http({
            method: "POST",
            url: "/PayRollStudents/SaveCheckFiles",
            data: {
                "ID": $scope.ID
            }
        }).then(function (data) {
            debugger;
            if (data.data != "") {
                DevExpress.ui.notify(data.data, "error", 5000);
            } else {

                DevExpress.ui.notify("تم الحفظ بنجاح", "success", 5000);
                $('#UploadFilePopupOptions').dxPopup('instance').option('visible', false);
                CheckFilesSrvc.GetChecksData($scope.UserID, $scope.PayrollID).then(function (data) {
                    $scope.CheckDataList = data.data;
                    GridDataSource.reload();
                });
            }

        });
        //
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

    $scope.UploadFilePopupOptions = {
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
        height: 700,
        rtlEnabled: true,
        onShowing: function () {
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

            $('#UploadFilePopupOptions').dxPopup('instance').option('visible', true);
        }
    };

    //Upload Options...
    $scope.multiple = false;
    $scope.accept = ".jpg,.jpeg,.png,.tif,.gif";
    $scope.btnUploadIcon = "upload";
    $scope.UploadingFilesvalue = [];
    $scope.FileUploadingOptions = {
        name: "fileSent",
        uploadUrl: "/PayRollStudents/UploadCheckFiles",
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
        allowedFileExtensions: [".jpg", ".jpeg", ".tif", ".png", ".gif"],
        accept: ".jpg,.jpeg,.png,.tif,.gif",
        multiple: false,
        bindingOptions: {
            value: "UploadingFilesvalue"
        },
        onValueChanged: function (e) {
            debugger;
            if (e.value.length > 0) {
                if ((e.value[0].type === "image/jpeg")
                    || (e.value[0].type === "image/jpg")
                    || (e.value[0].type === "image/tif")
                    || (e.value[0].type === "image/png")
                    || (e.value[0].type === "image/gif")
                ) {
                    $scope.MDL_UploadingFilesvalue = e.value;
                } else {
                    $scope.MDL_UploadingFilesvalue = '';
                    $scope.FileUploadingOptionsInstance.reset();
                    return swal("تنبيه", "غير مسموح برفع هذا النوع من الملفات يسمح فقط بإمتداد" + e.value[0].type.toString(), "warning");

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

              //  $('#UploadFilePopupOptions').dxPopup('instance').option('visible', false);
                $scope.btnUploadText = "تم رفع المرفق بنجاح";
                $scope.btnUploadIcon = "check";

            }
            if (e.request.status === 400) {

                $('#UploadFilePopupOptions').dxPopup('instance').option('visible', false);
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
            url: "/PayRollStudents/UploadCheckFiles"
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




}]);
