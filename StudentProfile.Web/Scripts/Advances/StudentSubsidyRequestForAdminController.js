    app.controller("StudentSubsidyRequestForAdminController", ["$scope", "$http", "StudentAdvanceForAdminSrvc", function ($scope, $http, StudentAdvanceForAdminSrvc) {

        $scope.studentsList = [];
        $scope.studentID = null;

        $scope.selectedItem = 1;

        // MenuHandling
        const Menu = [
            { key: 1, text: "طلب إعانة جديدة" }//,
            //{ key: 2, text: "عرض طلبات الإعانات" }
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
                    $scope.PopUpwidth = 500;
                    $scope.PopUpheight = 500;
                    $scope.PopupTitle = "إضافة طلب إعانة جديد";
                    $scope.PopupContent = "Controls";
                }
                else {
                    $scope.PopUpwidth = 1240;
                    $scope.PopUpheight = 500;
                    $scope.PopupTitle = "استعراض طلبات الإعانات";
                    $scope.PopupContent = "Grid";
                }
                $scope.PopUpShow = true;
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
            }
        };



        $scope.Advance = {

            StudentsSelectBox: {
                bindingOptions: {
                    dataSource: "studentsList",
                    value: "studentID",
                    items: "studentsList"
                },
                onInitialized: function (e) {
                    StudentAdvanceForAdminSrvc.GetStudents().then(function (data) {
                        $scope.studentsList = data.data;
                    });
                },

                placeholder: "--أختر--",
                noDataText: "لا يوجد بيانات",
                pagingEnabled: true, //Pagenation
                showClearButton: true,
                searchEnabled: true,
                displayExpr: "Text",
                valueExpr: "Value",
                searchExpr: ['Text', 'Value', 'NationalID']
            },

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
                    $scope.MDL_AdvanceValue = '';
                    $scope.MDL_AdvanceMaxValue = '';

                    if (e.value !== '' && e.value !== null && e.value !== undefined) {
                        $scope.MDL_AdvanceTypeId = e.value;
                        $scope.MDL_AdvanceMaxValue = e.component._dataSource._items.find(x => x.Value === e.value).MaxRequestValue;
                    }
                }
            },
            AdvanceValue:
            {
                bindingOptions: { value: "MDL_AdvanceValue", max: "MDL_AdvanceMaxValue" },
                showSpinButtons: true,
                showClearButton: true,
                placeholder: "أدخل مبلغ الإعانة المطلوب",
                rtlEnabled: true,
                min: 1,
                onValueChanged: function (e) {
                    $scope.MDL_AdvanceValue = e.value;
                }
            },
            AdvanceRequestNotes:
            {
                bindingOptions: { value: "MDL_AdvanceRequestNotes" },
                placeholder: "أدخل ملاحظات على طلب الإعانة الذي سيتم إرساله",
                rtlEnabled: true,
                height: 120,
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

                            //Save Advance Request ByAdmin
                            StudentAdvanceForAdminSrvc.SaveAdvanceRequestByAdmin($scope.MDL_AdvanceTypeId, $scope.MDL_AdvanceValue, $scope.MDL_AdvanceRequestNotes, $scope.studentID)
                                .then(function (data) {
                                    if (data.data.status === 200) {
                                        DevExpress.ui.notify({ message: data.data.Message, type: data.data.Type, displayTime: 3000, closeOnClick: true });
                                        //Clear Data 
                                        $scope.MDL_AdvanceTypeId = null;
                                        $scope.MDL_AdvanceValue = null;
                                        $scope.MDL_AdvanceRequestNotes = null;
                                        $scope.studentID = null;
                                    }
                                    if (data.data.status === 500) {
                                        DevExpress.ui.notify({ message: data.data.Message, type: data.data.Type, displayTime: 3000, closeOnClick: true });
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
            selectButtonText: "تحميل نموذج الشهادة",
            labelText: "",
            uploadButtonText: "رفع",
            uploadedMessage: "تم الرفع",
            invalidFileExtensionMessage: "نوع الملف غير مسموح",
            uploadFailedMessage: "خطأ أثناء الرفع",
            allowedFileExtensions: [".pdf"],
            bindingOptions: {
                multiple: "multiple",
                accept: "accept",
                value: "UploadingFilesvalue",
                uploadMode: "uploadMode"
            },
            onValueChanged: function (e) {
                $scope.MDL_UploadingFilesvalue = e.value;
            },
            onInitialized: function (e) {
                $scope.FileUploadingOptionsInstance = e.component;
            },
            onUploaded: function (e) {
                if (e.request.status === 200) {
                    $scope.UploadPopUpShow = false;
                    $scope.btnUploadText = "تم رفع المرفق بنجاح";
                    $scope.btnUploadIcon = "check";
                }
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


        //$scope.AdvanceRequestsGrid = {
        //    dataSource: DataSourceAdvancesGrid,
        //    bindingOptions: {
        //        rtlEnabled: true
        //    },
        //    sorting: {
        //        mode: "multiple"
        //    },
        //    wordWrapEnabled: true,
        //    showBorders: true,
        //    searchPanel: {
        //        visible: true,
        //        placeholder: "بحث",
        //        width: 300
        //    },
        //    paging: {
        //        pageSize: 10
        //    },
        //    pager: {
        //        allowedPageSizes: "auto",
        //        infoText: "(صفحة  {0} من {1} ({2} عنصر",
        //        showInfo: true,
        //        showNavigationButtons: true,
        //        showPageSizeSelector: true,
        //        visible: "auto"
        //    },
        //    filterRow: {
        //        visible: true,
        //        operationDescriptions: {
        //            between: "بين",
        //            contains: "تحتوى على",
        //            endsWith: "تنتهي بـ",
        //            equal: "يساوى",
        //            greaterThan: "اكبر من",
        //            greaterThanOrEqual: "اكبر من او يساوى",
        //            lessThan: "اصغر من",
        //            lessThanOrEqual: "اصغر من او يساوى",
        //            notContains: "لا تحتوى على",
        //            notEqual: "لا يساوى",
        //            startsWith: "تبدأ بـ"
        //        },
        //        resetOperationText: "الوضع الافتراضى"
        //    },
        //    headerFilter: {
        //        visible: true
        //    },
        //    showRowLines: true,
        //    groupPanel: {
        //        visible: false,
        //        emptyPanelText: "اسحب عمود هنا"
        //    },
        //    noDataText: "لايوجد بيانات",
        //    columnAutoWidth: true,
        //    width: '100%',
        //    columnChooser: {
        //        enabled: true
        //    },
        //    "export": {
        //        enabled: true,
        //        fileName: "Advances"
        //    },
        //    onCellPrepared: function (e) {
        //        if (e.rowType === "header" && e.column.command === "edit") {
        //            e.column.width = 80;
        //            e.column.alignment = "center";
        //            e.cellElement.text(" حذف ");
        //        }

        //        if (e.rowType === "data" && e.column.command === "edit") {
        //            $links = e.cellElement.find(".dx-link");
        //            $links.text("");

        //            $links.filter(".dx-link-delete").addClass("btn btn-danger btn-sm fa fa-trash-o");
        //        }
        //    },
        //    editing: {
        //        allowUpdating: false,
        //        allowAdding: false,
        //        allowDeleting: true,
        //        mode: "row",
        //        texts: {
        //            confirmDeleteMessage: "تأكيد حذف هذا الطلب ؟",
        //            deleteRow: "",
        //            editRow: "",
        //            addRow: ""
        //        },
        //        useIcons: true
        //    },
        //    columns: [

        //        {
        //            dataField: "AdvanceName",
        //            caption: "نوع الإعانة",
        //            alignment: "center",
        //            width: 150
        //        },
        //        {
        //            dataField: "RequestedValue",
        //            caption: "المبلغ المطلوب",
        //            alignment: "center",
        //            width: 150
        //        },
        //        {
        //            dataField: "InsertionDate",
        //            caption: "تاريخ الطلب",
        //            alignment: "center",
        //            dataType: "date",
        //            width: 150
        //        },
        //        {
        //            dataField: "Status",
        //            caption: "حالة الإعتماد",
        //            alignment: "center",
        //            width: 150
        //        },
        //        {
        //            dataField: "StatusNotes",
        //            caption: "ملاحظات الإعتماد",
        //            alignment: "center",
        //            visible: true,
        //            width: 220
        //        },
        //        {
        //            dataField: "ApprovedValue",
        //            caption: "المبلغ المعتمد",
        //            alignment: "center",
        //            width: 150
        //        },
        //        {
        //            caption: "المرفقات",
        //            cssClass: "text-center",
        //            width: 80,
        //            cellTemplate: function (container, options) {
        //                $("<div />").dxButton({
        //                    text: '',
        //                    hint: "تحميل",
        //                    type: 'success',
        //                    icon: 'fa fa-download',
        //                    useSubmitBehavior: false,
        //                    onClick: function (e) {
        //                        return window.open('/Advances/DownloadAdvanceAttachment?advanceRequestId=' + options.data.AdvanceRequestId, '_blank');
        //                    }
        //                }).appendTo(container);
        //            }
        //        }
        //    ],
        //    allowColumnResizing: true,
        //    columnResizingMode: "widget",
        //    allowColumnReordering: true,
        //    showColumnLines: true,
        //    rowAlternationEnabled: true,
        //    onInitialized: function (e) {
        //        $scope.ActivitiesGridInstance = e.component;
        //    },
        //    onRowRemoving: function (e) {
        //        e.cancel = true;
        //        //$http({
        //        //    method: "Delete",
        //        //    url: "/AcademicActivities/DeleteAcademicActivity",
        //        //    data: { AcademicActivityId: e.data.AcademicActivitiesId }
        //        //}).then(function (data) {
        //        //    if (data.data !== "") {
        //        //        return DevExpress.ui.notify({ message: data.data }, "Error", 10000);
        //        //    } else {
        //        //        DataSourceActivitiesGrid.reload();
        //        //        swal("Done!", "تم الحذف بنجاح", "success");
        //        //    }
        //        //});
        //    }
        //};
    }]);
