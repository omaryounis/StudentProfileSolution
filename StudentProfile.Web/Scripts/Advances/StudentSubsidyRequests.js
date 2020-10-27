(function () {
    app.controller("StudentSubsidyRequestController",
        ["$scope", "$http",
            function ($scope, $http) {

                $scope.Init = function () {

                    $scope.selectedItem = 1;
                    $scope.PopUpShow = false;
                    $scope.MDL_AdvanceValuePlaceHolder = "أدخل مبلغ الإعانة المطلوب";
                };
                $scope.Init();


                // MenuHandling
                const Menu = [
                    { key: 1, text: "طلب إعانة جديدة" },
                    { key: 2, text: "عرض طلبات الإعانات" }
                ];

                $scope.MenuOptions = {
                    dataSource: Menu,
                    itemTemplate: function (data) {
                        if (data.key === 1) {

                            return $("<div><i class='fa fa-plus-square-o'></i><div>" + data.text + "</div></div>");
                        } else {
                            return $("<div><i class='fa fa-id-card-o'></i><div>" + data.text + "</div></div>");
                        }
                    },
                    onItemClick: function (e) {
                        $scope.selectedItem = e.itemData.key;


                        if ($scope.selectedItem === 1) {

                            $scope.PopUpwidth = 500;
                            $scope.PopUpheight = 540;
                            $scope.PopupTitle = "إضافة طلب إعانة جديد";
                            $scope.PopupContent = "Controls";
                            $scope.PopUpShow = true;
                        }
                        else {
                            $scope.PopUpwidth = "90%";
                            $scope.PopUpheight = 500;
                            $scope.PopupTitle = "استعراض طلبات الإعانات";
                            $scope.PopupContent = "Grid";
                            $scope.PopUpShow = true;
                            $('body').css('overflow', 'hidden');
                        }

                    }
                };
                $scope.Advance = {
                    ValidationRules: {
                        AdvanceType: {
                            validationGroup: "addAdvance",
                            validationRules: [
                                {
                                    type: "required",
                                    message: "حقل اجبارى"
                                }
                            ]
                        },
                        AdvanceValue: {
                            validationGroup: "addAdvance",
                            validationRules: [
                                {
                                    type: "required",
                                    message: "حقل اجبارى"
                                }
                            ]
                        }
                    },
                    AdvanceType:
                    {
                        dataSource: new DevExpress.data.DataSource({
                            loadMode: "raw",
                            paginate: true,
                            cacheRawData: false,
                            key: "Value",
                            load: function () {
                                return $.getJSON("/Advances/GetAdvancesTypes?type=" + "S");
                            }
                        }),
                        bindingOptions: { value: "MDL_AdvanceTypeId" },
                        valueExpr: "Value",
                        displayExpr: "Text",
                        placeholder: "اختر نوع الإعانة",
                        showClearButton: true,
                        itemTemplate: function (data) {
                            return "<div title='" + data.Text + "' value='" + data.Value + "'>" + data.Text + "</div>";
                        },
                        onValueChanged: function (e) {

                            if ($scope.MDL_AdvanceValue > 0) {
                                $scope.MDL_AdvanceValue = '';
                            }

                            if (e.value !== '' && e.value !== null && e.value !== undefined) {
                                debugger;
                                $scope.MDL_AdvanceTypeId = e.value;
                                $scope.MDL_AdvanceMaxValue = e.component._dataSource._items.find(x => x.Value === e.value).MaxRequestValue;
                                $scope.MDL_AdvanceValuePlaceHolder = "الحد الأقصي المسموح به لقيمة الطلب" + " " + $scope.MDL_AdvanceMaxValue;
                            }
                            else {
                                $scope.MDL_AdvanceMaxValue = '';
                                $scope.MDL_AdvanceValuePlaceHolder = "أدخل مبلغ الإعانة المطلوب";
                            }
                        }
                    },
                    AdvanceValue:
                    {
                        bindingOptions: { value: "MDL_AdvanceValue", placeholder: "MDL_AdvanceValuePlaceHolder" },
                        showSpinButtons: true,
                        showClearButton: true,
                        rtlEnabled: true,
                        min: 1,
                        onValueChanged: function (e) {

                            if (e.value > $scope.MDL_AdvanceMaxValue) {
                                swal("تنبيه", $scope.MDL_AdvanceValuePlaceHolder, "warning");
                                $scope.MDL_AdvanceValue = '';
                            } else {
                                if ($scope.MDL_AdvanceValue > 0 && ($scope.MDL_AdvanceTypeId === '' || $scope.MDL_AdvanceTypeId === null || $scope.MDL_AdvanceTypeId === undefined)) {
                                    swal("تنبيه", "لابد من اختيار نوع الإعانة أولا", "warning");
                                } else {
                                    $scope.MDL_AdvanceValue = e.value;
                                }
                            }
                        }
                    },
                    AdvanceRequestNotes:
                    {
                        bindingOptions: { value: "MDL_AdvanceRequestNotes" },
                        placeholder: "يرجي كتابة رقم الحساب البنكي(IBAN) للطالب او الوكيل وصورة هوية الوكيل للأهميه",
                        rtlEnabled: true,
                        height: 120,
                        maxLength: 200,
                        onValueChanged: function (e) {
                            $scope.MDL_AdvanceRequestNotes = e.value;
                        }
                    },
                    SaveButton: {
                        text: "إرسال الطلب",
                        hint: "إضافة",
                        icon: "save",
                        type: "success",
                        validationGroup: "addAdvance",
                        useSubmitBehavior: true,
                        onClick: function (e) {
                            var validationGroup = DevExpress.validationEngine.getGroupConfig("addAdvance");
                            if (validationGroup) {
                                var result = validationGroup.validate();
                                if (result.isValid) {
                                    debugger;

                                    $http({
                                        method: "POST",
                                        url: "/Advances/SaveAdvanceRequestByStudent/",
                                        data: {
                                            advanceTypeId: $scope.MDL_AdvanceTypeId,
                                            advanceValue: $scope.MDL_AdvanceValue,
                                            advanceRequestNotes: $scope.MDL_AdvanceRequestNotes
                                        }
                                    }).then(function (data) {
                                        if (data.data === "") {

                                            $scope.ResetControls();
                                            validationGroup.reset();
                                            DataSourceAdvancesGrid.reload();

                                            swal("Done!", "تم الحفظ بنجاح", "success");

                                        } else {
                                            if (data.data !== "") { swal("حدث خطأ", data.data, "error"); }
                                        }
                                    });
                                }
                            }
                        }
                    }
                };

                var DataSourceAdvancesGrid = new DevExpress.data.DataSource({
                    paginate: true,
                    cacheRawData: false,
                    key: "AdvanceRequestId",
                    loadMode: "raw",
                    load: function () {
                        return $.getJSON("/Advances/DataSourceAdvanceRequestsGrid?type=" + "S", function (data) { debugger; });
                    }
                });

                $scope.AdvanceRequestsGrid = {
                    dataSource: DataSourceAdvancesGrid,
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
                    width: '100%',
                    columnChooser: {
                        enabled: true
                    },
                    "export": {
                        enabled: true,
                        fileName: "Advances"
                    },
                    onCellPrepared: function (e) {
                        if (e.rowType === "header" && e.column.command === "edit") {
                            e.column.width = 80;
                            e.column.alignment = "center";
                            e.cellElement.text(" حذف ");
                        }

                        if (e.rowType === "data" && e.column.command === "edit") {
                            $links = e.cellElement.find(".dx-link");
                            $links.text("");
                            if (e.data.ApprovedValue) {
                                debugger;
                                e.column.visible = false;
                            } else {
                                $links.filter(".dx-link-delete").addClass("btn btn-danger btn-sm fa fa-trash-o");
                            }
                        }

                        if (e.rowType === "data" && e.column.dataField === "Status") {
                            if (e.value === "تحت المراجعة") {
                                e.cellElement.css({ color: '#c7bb27', "font-weight": "bold" });

                            }
                            if (e.value === "معتمد") {
                                e.cellElement.css({ color: '#5cb85c', "font-weight": "bold" });

                            }
                            if (e.value === "مرفوض") {
                                e.cellElement.css({ color: '#d9534f', "font-weight": "bold" });
                            }
                        }

                    },
                    editing: {
                        allowUpdating: false,
                        allowAdding: false,
                        allowDeleting: true,
                        mode: "row",
                        texts: {
                            confirmDeleteMessage: "تأكيد حذف هذا الطلب ؟",
                            deleteRow: "",
                            editRow: "",
                            addRow: ""
                        },
                        useIcons: true
                    },
                    columns: [
                        {
                            dataField: "AdvanceRequestId",
                            caption: "رقم الإعانة",
                            alignment: "right"
                        },
                        {
                            dataField: "AdvanceName",
                            caption: "نوع الإعانة",
                            alignment: "right"  
                        },
                        {
                            dataField: "RequestedValue",
                            caption: "المبلغ المطلوب",
                            alignment: "right" 
                        },
                        {
                            dataField: "InsertionDate",
                            caption: "تاريخ الطلب",
                            alignment: "right",
                            dataType: "date" 
                        },
                        {
                            dataField: "RequestNotes",
                            caption: "ملاحظات الطلب",
                            alignment: "right" 
                        },
                        {
                            dataField: "Status",
                            caption: "حالة الإعتماد",
                            alignment: "right" 
                        },
                        {
                            dataField: "StatusNotes",
                            caption: "ملاحظات الإعتماد",
                            alignment: "right"
                        },
                        {
                            dataField: "ApprovedValue",
                            caption: "المبلغ المعتمد",
                            alignment: "right" 
                        },
                        {
                            dataField: "AdvanceIsPaid",
                            caption: "حالة الصرف",
                            alignment: "right" 
                        },
                        {
                            caption: "المرفقات",
                            cssClass: "text-center",
                            width: 100,
                            cellTemplate: function (container, options) {
                                $("<div />").dxButton({
                                    text: '',
                                    hint: "تحميل",
                                    type: 'success',
                                    icon: 'fa fa-download',
                                    useSubmitBehavior: false,
                                    onClick: function (e) {
                                        return window.open('/Advances/DownloadAdvanceAttachment?advanceRequestId=' + options.data.AdvanceRequestId, '_blank');
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
                        $scope.ActivitiesGridInstance = e.component;
                    },
                    onRowRemoving: function (e) {
                        e.cancel = true;
                        $http({
                            method: "Delete",
                            url: "/Advances/DeleteAdvanceRequest",
                            data: { advanceRequestId: e.data.AdvanceRequestId }
                        }).then(function (data) {
                            if (data.data !== "") {
                                return swal("خطأ", data.data, "error");
                            } else {
                                DataSourceAdvancesGrid.reload();
                                swal("Done!", "تم الحذف بنجاح", "success");
                            }
                        });
                    },
                    onContentReady: function (e) {
                        e.component.columnOption("command:edit", "width", 80);
                    }
                };

                $scope.PopupOptions = {
                    showTitle: true,
                    dragEnabled: false,
                    shadingColor: "rgba(0, 0, 0, 0.69)",
                    closeOnOutsideClick: false,
                    bindingOptions: {
                        visible: "PopUpShow",
                        contentTemplate: "PopupContent",
                        title: "PopupTitle",
                        width: "PopUpwidth",
                        height: "PopUpheight"
                    },
                    rtlEnabled: true,
                    onHiding: function () {
                        if ($scope.PopupContent === "Controls") {
                            $scope.ResetControls();
                        }
                        $('body').css('overflow', 'auto');
                    }
                };

                //upload
                $scope.btnUploadIcon = "upload";
                $scope.btnUploadText = "رفع المرفق";
                $scope.btnUploadOptions = {
                    bindingOptions: {
                        text: "btnUploadText",
                        icon: 'btnUploadIcon'
                    },
                    visible: true,
                    type: 'default',
                    useSubmitBehavior: false,
                    onClick: function (e) {
                        $scope.UploadPopUpShow = true;
                    }
                };

                $scope.UploadFilePopupOptions = {
                    showTitle: true,
                    dragEnabled: false,
                    shadingColor: "rgba(0, 0, 0, 0.69)",
                    closeOnOutsideClick: false,
                    bindingOptions: {
                        visible: "UploadPopUpShow"
                    },
                    contentTemplate: 'UploadFileContent',
                    title: "رفع مرفق الإعانة",
                    width: 500,
                    height: 500,
                    rtlEnabled: true,
                    onHiding: function () { }
                };


                // File Uploading ...
                $scope.multiple = false;
                $scope.accept = ".pdf";
                $scope.uploadMode = "useButtons";
                $scope.UploadingFilesvalue = [];


                $scope.FileUploadingOptions = {
                    name: "fileSent",
                    uploadUrl: "/Advances/UploadFiles",
                    allowCanceling: true,
                    rtlEnabled: true,
                    readyToUploadMessage: "استعداد للرفع",
                    selectButtonText: "تحميل صورة الحساب وصورة هوية الوكيل وصورة الفواتير",
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
                        //multiple: "multiple",
                        //accept: "accept",
                        value: "UploadingFilesvalue"
                        //uploadMode: "uploadMode"
                    },
                    onValueChanged: function (e) {
                        debugger;
                        if (e.value.length > 0) {
                            if (e.value[0].type === "application/pdf") {
                                $scope.MDL_UploadingFilesvalue = e.value;
                            } else {
                                $scope.MDL_UploadingFilesvalue = null;
                                $scope.FileUploadingOptionsInstance.reset();
                                return swal("تنبيه", "غير مسموح برفع هذا النوع من الملفات يسمح فقط بإمتداد" + ".pdf", "warning");
                            }
                        } else {
                            $scope.MDL_UploadingFilesvalue = e.value;
                        }
                    },
                    onInitialized: function (e) {
                        $scope.FileUploadingOptionsInstance = e.component;
                    },
                    onUploaded: function (e) {
                        debugger;
                        if (e.request.status === 200) {
                            $scope.UploadPopUpShow = false;
                            $scope.btnUploadText = "تم رفع المرفق بنجاح";
                            $scope.btnUploadIcon = "check";
                        }
                        if (e.request.status === 400) {
                            $scope.UploadPopUpShow = false;
                            return DevExpress.ui.notify({ message: "" }, "Error", 10000);
                            //$scope.btnUploadText = "تم رفع المرفق بنجاح";
                            //$scope.btnUploadIcon = "check";
                        }
                        //var xhttp = e.request;
                        //if (xhttp.status === 200) {
                        //    //$scope.ViolationForwordsFilesvalue = '';
                        //    //$scope.ViolationForwordsFilesvalue.reset();
                        //} else if (xhttp.status === 404) {
                        //    DevExpress.ui.notify({ message: "الملف المرفوع لايحتوي على بيانات" }, "Error", 10000);
                        //    $scope.ViolationForwordsFilesvalue = '';
                        //    $scope.ViolationForwordsFilesInstance.reset();

                        //} else {
                        //    DevExpress.ui.notify({ message: "حدث خطأ أثناء رفع الملف" }, "Error", 10000);
                        //    $scope.ViolationForwordsFilesvalue = '';
                        //    $scope.ViolationForwordsFilesInstance.reset();
                        //}

                    }
                };

                //Remove Uploaded File
                $scope.RemoveUploadingFile = function (hashkey) {
                    $scope.MDL_UploadingFilesvalue = '';
                    $scope.FileUploadingOptionsInstance.reset();
                    $http({
                        method: "POST",
                        url: "/Advances/UploadFiles"
                    });
                    $scope.btnUploadText = "رفع المرفق";
                    $scope.btnUploadIcon = "upload";
                };


                $scope.ResetControls = function () {

                    $scope.MDL_AdvanceValue = null;
                    $scope.MDL_AdvanceTypeId = null;
                    $scope.MDL_AdvanceRequestNotes = null;

                    $scope.btnUploadIcon = "upload";
                    $scope.btnUploadText = "رفع المرفق";

                    $scope.MDL_UploadingFilesvalue = null;
                    debugger;
                    if ($scope.FileUploadingOptionsInstance) {
                        $scope.FileUploadingOptionsInstance.reset();
                    }
                };
            }
        ]);
})();