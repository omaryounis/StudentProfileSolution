(function () {
    app.controller("StudentAdvanceRequestsController", ["$scope", "$http", function ($scope, $http) {

        $scope.selectedItem = 2;
        $scope.disabledSaveButton = true;
        $scope.disabledFileUploadButton = true;
        $scope.AdvanceRequestsGridInstance = '';
        $scope.MDL_AdvanceValuePlaceHolder = "أدخل مبلغ السلفة المطلوب";

        // MenuHandling
        const Menu = [

            { key: 1, text: "طلب سلفة جديدة" },
            { key: 2, text: "عرض طلبات السلف" }
        ];
        $scope.PopUpShow = false;

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
                    $http({
                        method: "get",
                        url: "/Advances/GetAdvancesTotalRemainForStudent"
                    }).then(function (data) {
                        if (data.data.TotalRemain === 0) {
                            $scope.PopUpwidth = "70%";
                            $scope.PopUpheight = 520;
                            $scope.PopupTitle = "إضافة طلب سلفة جديد";
                            $scope.PopupContent = "Controls";
                            $scope.PopUpShow = true;
                        } else {
                            return swal("تنبيه", "لايمكن تقديم طلب سلفة جديد حتي يتم سداد الرصيد المتبقي من السلف السابقة البالغ قيمتها" + " " + data.data.TotalRemain + " " + "ريال", "warning");
                        }
                    });
                }
                else {
                    $scope.PopUpwidth = "80%";
                    $scope.PopUpheight = 540;
                    $scope.PopupTitle = "استعراض طلبات السلف";
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
                },
                AdvanceRequestNotes: {
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
                        return $.getJSON("/Advances/GetAdvancesTypes?type=" + "A");
                    }
                }),
                bindingOptions: { value: "MDL_AdvanceTypeId" },
                valueExpr: "Value",
                displayExpr: "Text",
                placeholder: "اختر",
                showClearButton: true,
                itemTemplate: function (data) {
                    return "<div title='" + data.Text + "' value='" + data.Value + "'>" + data.Text + "</div>";
                },
                onValueChanged: function (e) {
                    debugger;
                    if ($scope.MDL_AdvanceValue > 0) {
                        $scope.MDL_AdvanceValue = '';
                    }

                    if (e.value !== '' && e.value !== null && e.value !== undefined) {
                        if (e.component._dataSource._items.find(x => x.Value === e.value).IsConditional) {

                            $scope.disabledSaveButton = true;
                            $scope.disabledFileUploadButton = true;
                            $http({
                                method: "GET",
                                url: "/Advances/IsDueAdvance/",
                                params: {
                                    studentId: null
                                }
                            }).then(function (data) {

                                if (data.data !== "") {
                                    $scope.MDL_AdvanceTypeId = '';
                                    $scope.MDL_AdvanceMaxValue = '';
                                    $scope.MDL_AdvanceValuePlaceHolder = "أدخل مبلغ السلفة المطلوب";

                                    return swal("تنبيه", data.data, "warning");
                                } else {
                                    $scope.disabledSaveButton = false;
                                    $scope.disabledFileUploadButton = false;

                                    $scope.MDL_AdvanceTypeId = e.value;
                                    $scope.MDL_AdvanceMaxValue = e.component._dataSource._items.find(x => x.Value === e.value).MaxRequestValue;
                                    $scope.MDL_AdvanceValuePlaceHolder = "الحد الأقصي المسموح به لقيمة الطلب" + " " + $scope.MDL_AdvanceMaxValue;
                                }
                            });
                        } else {
                            $scope.disabledSaveButton = false;

                            $scope.disabledFileUploadButton = false;
                            $scope.MDL_AdvanceTypeId = e.value;
                            $scope.MDL_AdvanceMaxValue = e.component._dataSource._items.find(x => x.Value === e.value).MaxRequestValue;
                            $scope.MDL_AdvanceValuePlaceHolder = "الحد الأقصي المسموح به لقيمة الطلب" + " " + $scope.MDL_AdvanceMaxValue;
                        }
                    } else {
                        $scope.MDL_AdvanceMaxValue = '';
                        $scope.MDL_AdvanceValuePlaceHolder = "أدخل مبلغ السلفة المطلوب";
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
                    debugger;
                    if (e.value > $scope.MDL_AdvanceMaxValue) {
                        swal("تنبيه", $scope.MDL_AdvanceValuePlaceHolder, "warning");
                        $scope.MDL_AdvanceValue = '';
                    } else {
                        if ($scope.MDL_AdvanceValue > 0 && ($scope.MDL_AdvanceTypeId === '' || $scope.MDL_AdvanceTypeId === null || $scope.MDL_AdvanceTypeId === undefined)) {
                            swal("تنبيه", "لابد من اختيار نوع السلفة أولا", "warning");
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
                bindingOptions: { disabled: "disabledSaveButton" },
                text: "إرسال الطلب",
                hint: "إضافة",
                //icon: "add",
                type: "success",
                validationGroup: "addAdvance",
                useSubmitBehavior: true,
                onClick: function (e) {
                    var validationGroup = DevExpress.validationEngine.getGroupConfig("addAdvance");
                    if (validationGroup) {
                        var result = validationGroup.validate();
                        if (result.isValid) {
                            debugger;
                            if ($scope.MDL_AdvanceRequestNotes === '' || $scope.MDL_AdvanceRequestNotes === null || $scope.MDL_AdvanceRequestNotes === undefined) {
                                return DevExpress.ui.notify({
                                    message: "حقل الملاحظات إلزامي",
                                    type: "error",
                                    displayTime: 3000,
                                    closeOnClick: true
                                });
                            }

                    
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

                                    $scope.MDL_AdvanceValue = null;
                                    $scope.MDL_AdvanceTypeId = null;
                                    $scope.disabledSaveButton = true;

                                    $scope.disabledFileUploadButton = true;
                                    $scope.MDL_AdvanceRequestNotes = null;
                                    $scope.btnUploadIcon1 = "upload";
                                    $scope.btnUploadText1 = "رفع المرفق";

                                    $scope.MDL_UploadingFilesvalue1 = null;
                                    debugger;
                                    if ($scope.FileUploadingOptionsInstance1) {
                                        $scope.FileUploadingOptionsInstance1.reset();
                                    }

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
                return $.getJSON("/Advances/DataSourceAdvanceRequestsGrid?type=" + "A", function (data) { });
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
            showBorders: true,
            width: "100%",
            searchPanel: {
                visible: true,
                placeholder: "بحث",
                width: 300
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
                    caption: "رقم السلفة",
                    alignment: "right",
                    width: 120
                },
                {
                    dataField: "AdvanceName",
                    caption: "نوع السلفة",
                    alignment: "right",
                    width: 140
                },
                {
                    dataField: "RequestedValue",
                    caption: "المبلغ المطلوب",
                    alignment: "right",
                    width: 130
                },
                {
                    dataField: "InsertionDate",
                    caption: "تاريخ الطلب",
                    alignment: "right",
                    dataType: "date",
                    width: 120
                },
                {
                    dataField: "RequestNotes",
                    caption: "ملاحظات الطلب",
                    alignment: "right",
                    visible: true,
                    width: 150
                },
                {
                    dataField: "Status",
                    caption: "حالة الإعتماد",
                    alignment: "right",
                    width: 120
                },
                {
                    dataField: "StatusNotes",
                    caption: "ملاحظات الإعتماد",
                    alignment: "right",
                    visible: true,
                    width: 150
                },
                {
                    dataField: "ApprovedValue",
                    caption: "المبلغ المعتمد",
                    alignment: "right",
                    width: 130
                },
                {
                    dataField: "AdvanceIsPaid",
                    caption: "حالة الصرف",
                    alignment: "right",
                    width: 110
                }
            ],
            onContentReady: function (e) {
                e.component.columnOption("command:edit", "width", 80);
            },
            wordWrapEnabled: true,
            allowColumnResizing: true,
            columnResizingMode: "widget",
            allowColumnReordering: true,
            showColumnLines: true,
            rowAlternationEnabled: true,
            onInitialized: function (e) {
                $scope.AdvanceRequestsGridInstance = e.component;
            },
            onRowRemoving: function (e) {
                e.cancel = true;
                $http({
                    method: "Delete",
                    url: "/Advances/DeleteAdvanceRequest",
                    data: { advanceRequestId: e.data.AdvanceRequestId }
                }).then(function (data) {
                    if (data.data !== "") {
                        return DevExpress.ui.notify({ message: data.data }, "Error", 10000);
                    } else {
                        DataSourceAdvancesGrid.reload();
                        swal("Done!", "تم الحذف بنجاح", "success");
                    }
                });
            },
            masterDetail: {
                enabled: true,
                template: "RemainAmountDetailContent"
            }
        };

        $scope.GetRemainAmountDetailGrid = function (key) {
            return {
                dataSource: new DevExpress.data.DataSource({
                    paginate: true,
                    cacheRawData: false,
                    key: "AdvanceRequestId",
                    loadMode: "raw",
                    load: function () {
                        return $.getJSON("/Advances/GetAdvanceRemainAmount?advanceRequestId=" + key, function (data) { debugger; });
                    }
                }),
                key: "AdvanceRequestId",
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
                        dataField: "AdvanceRequestValue",
                        caption: "الملبغ المطلوب",
                        alignment: "center"
                    },
                    {
                        dataField: "AdvanceApprovedValue",
                        caption: "المبلغ المعتمد",
                        alignment: "center"
                    }, {
                        dataField: "PaidAmount",
                        caption: "المبلغ المسدد",
                        alignment: "center"
                    }, {
                        dataField: "RemainAmount",
                        caption: "المبلغ المتبقي",
                        alignment: "center"
                    }
                ],
                masterDetail: {
                    enabled: true,
                    template: "PaidAmountDetailGridContent"
                }
            };
        };

        $scope.GetPaidAmountDetailGrid = function (key) {
            return {
                dataSource: new DevExpress.data.DataSource({
                    paginate: true,
                    cacheRawData: false,
                    key: "AdvanceRequestId",
                    loadMode: "raw",
                    load: function () {
                        return $.getJSON("/Advances/GetAdvancePaidAmount?advanceRequestId=" + key, function (data) {});
                    }
                }),
                key: "AdvanceRequestId",
                columnAutoWidth: true,
                showBorders: true,
                sorting: {
                    mode: "multiple"
                },
                wordWrapEnabled: false,
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
                        dataField: "PaidAmount",
                        caption: "القيمة المسددة",
                        alignment: "center"
                    },
                    {
                        dataField: "PayRollNumber",
                        caption: "رقم المسير",
                        alignment: "center"
                    }
                ],
                summary: {
                    totalItems: [{
                        column: "PaidAmount",
                        summaryType: "sum",
                        displayFormat: "المجموع :{0}"
                    }]
                }
            };
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
                $('body').css('overflow', 'auto');
            }
        };



        /********************************************************************* Upload advances file ***********************************************************************/
        $scope.btnUploadIcon1 = "upload";
        $scope.btnUploadText1 = "رفع المرفق";
        $scope.btnUploadOptions1 = {
            bindingOptions: {
                text: "btnUploadText1",
                icon: 'btnUploadIcon1',
                disabled: "disabledFileUploadButton"
            },
            visible: true,
            type: 'default',
            useSubmitBehavior: false,
            onClick: function (e) {
                $scope.UploadPopUpShow1 = true;
            }
        };
        //=====================
        // File Uploading Popup
        //=====================
        $scope.UploadFilePopupOptions1 = {
            showTitle: true,
            dragEnabled: false,
            shadingColor: "rgba(0, 0, 0, 0.69)",
            closeOnOutsideClick: false,
            bindingOptions: {
                visible: "UploadPopUpShow1"
            },
            contentTemplate: 'UploadFileContent1',
            title: "رفع مرفق السلفه",
            width: 500,
            height: 500,
            rtlEnabled: true,
            onHiding: function () { }
        };



        //===============//===============//===============//===============//===============
        // File Uploading
        //===============//===============//===============//===============//===============
        $scope.multiple = false;
        $scope.accept = ".pdf";
        $scope.uploadMode = "useButtons";
        $scope.UploadingFilesvalue1 = [];

        $scope.FileUploadingOptions1 = {
            name: "fileSent",
            uploadUrl: "/Advances/UploadFiles",
            allowCanceling: true,
            rtlEnabled: true,
            readyToUploadMessage: "استعداد للرفع",
            selectButtonText: "تحميل صورة الحساب وصورة هوية الوكيل",
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
                value: "UploadingFilesvalue1"
            },
            onValueChanged: function (e) {
                debugger;
                if (e.value.length > 0) {
                    if (e.value[0].type === "application/pdf") {
                        $scope.MDL_UploadingFilesvalue1 = e.value;
                    } else {
                        $scope.MDL_UploadingFilesvalue1 = null;
                        $scope.FileUploadingOptionsInstance1.reset();
                        return swal("تنبيه", "غير مسموح برفع هذا النوع من الملفات يسمح فقط بإمتداد" + ".pdf", "warning");
                    }
                } else {
                    $scope.MDL_UploadingFilesvalue1 = e.value;
                }
            },
            onInitialized: function (e) {
                $scope.FileUploadingOptionsInstance1 = e.component;
            },
            onUploaded: function (e) {
                debugger;
                if (e.request.status === 200) {
                    $scope.UploadPopUpShow1 = false;
                    $scope.btnUploadText1 = "تم رفع المرفق بنجاح";
                    $scope.btnUploadIcon1 = "check";
                }
                if (e.request.status === 400) {
                    $scope.UploadPopUpShow1 = false;
                    return DevExpress.ui.notify({ message: "" }, "Error", 10000);
                }

            }
        };



        //=====================
        // Remove Uploaded File
        //=====================
        $scope.RemoveUploadingFile1 = function (hashkey) {
            $scope.MDL_UploadingFilesvalue1 = '';
            $scope.FileUploadingOptionsInstance1.reset();
            $http({
                method: "POST",
                url: "/Advances/UploadFiles"
            });
            $scope.btnUploadText1 = "رفع المرفق";
            $scope.btnUploadIcon1 = "upload";
        };



    }]);

})();