app.controller("StudentAdvanceRequestsForAdminController",
    ["$scope", "$http", "StudentAdvanceForAdminSrvc",
        function ($scope, $http, StudentAdvanceForAdminSrvc) {

            $scope.studentsList = [];
            $scope.studentID = null;

            $scope.selectedItem = 1;

            // MenuHandling
            const Menu = [
                { key: 1, text: "إضافة سلفة جديدة" }//,
                //{ key: 2, text: "عرض طلبات السلف" }
            ];
            $scope.PopUpShow = false;

            $scope.MenuOptions = {
                dataSource: Menu,
                itemTemplate: function (data) {
                    if (data.key === 1) {
                        return $("<div><i class='fa fa-plus-square-o'></i><div>" + data.text + "</div></div>");
                    }
                    //else {
                    //    return $("<div><i class='fa fa-id-card-o'></i><div>" + data.text + "</div></div>");
                    //}
                },
                onItemClick: function (e) {
                    debugger;
                    $scope.selectedItem = e.itemData.key;

                    if ($scope.selectedItem === 1) {
                        $scope.PopUpwidth = 1000;
                        $scope.PopUpheight = 450;
                        $scope.PopupTitle = "إضافة سلفة جديدة";
                        $scope.PopupContent = "Controls";
                    }
                    //else {
                    //    $scope.PopUpwidth = 1240;
                    //    $scope.PopUpheight = 500;
                    //    $scope.PopupTitle = "استعراض طلبات السلف";
                    //    $scope.PopupContent = "Grid";
                    //}
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
                width: 1240,
                height: 500,
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
                        cacheRawData: true,
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
                    rtlEnabled: true,
                    min: 1,
                    onValueChanged: function (e) {
                        $scope.MDL_AdvanceValue = e.value;
                    }
                },
                AdvanceRequestNotes:
                {
                    bindingOptions: { value: "MDL_AdvanceRequestNotes" },
                    placeholder: "ملاحظات طلب السلفة",
                    rtlEnabled: true,
                    height: 120,
                    onValueChanged: function (e) {
                        $scope.MDL_AdvanceRequestNotes = e.value;
                    }
                },
                SaveButton: {
                    //bindingOptions: { visible: "!isUpdate" },
                    text: "إرسال الطلب",
                    hint: "إضافة",
                    //icon: "add",
                    type: "success",
                    validationGroup: "addAdvance",
                    useSubmitBehavior: true,
                    onClick: function (e) {
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

            //var DataSourceAdvancesGrid = new DevExpress.data.DataSource({
            //    paginate: true,
            //    cacheRawData: false,
            //    key: "AdvanceRequestId",
            //    loadMode: "raw",
            //    load: function () {
            //        return $.getJSON("/Advances/DataSourceAdvanceRequestsGrid?type=" + "A", function (data) { debugger; });
            //    }
            //});
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
            //            caption: "نوع السلفة",
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
            //            width: 300
            //        },
            //        {
            //            dataField: "ApprovedValue",
            //            caption: "المبلغ المعتمد",
            //            alignment: "center",
            //            width: 150
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
            //    },
            //    masterDetail: {
            //        enabled: true,
            //        template: "RemainAmountDetailContent"
            //    }
            //};

            //$scope.GetRemainAmountDetailGrid = function (key) {
            //    return {
            //        dataSource: new DevExpress.data.DataSource({
            //            paginate: true,
            //            cacheRawData: false,
            //            key: "AdvanceRequestId",
            //            loadMode: "raw",
            //            load: function () {
            //                return $.getJSON("/Advances/GetAdvanceRemainAmount?advanceRequestId=" + key, function (data) { debugger; });
            //            }
            //        }),
            //        sorting: {
            //            mode: "multiple"
            //        },
            //        wordWrapEnabled: false,
            //        showBorders: true,
            //        paging: {
            //            pageSize: 5
            //        },
            //        pager: {
            //            allowedPageSizes: "auto",
            //            infoText: "(صفحة  {0} من {1} ({2} عنصر",
            //            showInfo: true,
            //            showNavigationButtons: true,
            //            showPageSizeSelector: true,
            //            visible: "auto"
            //        },
            //        headerFilter: {
            //            visible: false
            //        },
            //        showRowLines: true,
            //        groupPanel: {
            //            visible: false,
            //            emptyPanelText: "اسحب عمود هنا"
            //        },
            //        noDataText: "لايوجد بيانات",
            //        columnAutoWidth: true,
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
            //        rowAlternationEnabled: true,
            //        columns: [
            //            {
            //                dataField: "AdvanceRequestValue",
            //                caption: "الملبغ المطلوب",
            //                alignment: "center"
            //            },
            //            {
            //                dataField: "AdvanceApprovedValue",
            //                caption: "المبلغ المعتمد",
            //                alignment: "center"
            //            }, {
            //                dataField: "PaidAmount",
            //                caption: "المبلغ المسدد",
            //                alignment: "center"
            //            }, {
            //                dataField: "RemainAmount",
            //                caption: "المبلغ المتبقي",
            //                alignment: "center"
            //            }
            //        ],
            //        masterDetail: {
            //            enabled: true,
            //            template: "PaidAmountDetailGridContent"
            //        }
            //    };
            //};

            //$scope.GetPaidAmountDetailGrid = function (key) {
            //    return {
            //        dataSource: new DevExpress.data.DataSource({
            //            paginate: true,
            //            cacheRawData: false,
            //            key: "AdvanceRequestId",
            //            loadMode: "raw",
            //            load: function () {
            //                return $.getJSON("/Advances/GetAdvancePaidAmount?advanceRequestId=" + key, function (data) { debugger; });
            //            }
            //        }),
            //        columnAutoWidth: true,
            //        showBorders: true,
            //        sorting: {
            //            mode: "multiple"
            //        },
            //        wordWrapEnabled: false,
            //        paging: {
            //            pageSize: 5
            //        },
            //        pager: {
            //            allowedPageSizes: "auto",
            //            infoText: "(صفحة  {0} من {1} ({2} عنصر",
            //            showInfo: true,
            //            showNavigationButtons: true,
            //            showPageSizeSelector: true,
            //            visible: "auto"
            //        },
            //        headerFilter: {
            //            visible: false
            //        },
            //        showRowLines: true,
            //        groupPanel: {
            //            visible: false,
            //            emptyPanelText: "اسحب عمود هنا"
            //        },
            //        noDataText: "لايوجد بيانات",
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
            //        rowAlternationEnabled: true,
            //        columns: [

            //            {
            //                dataField: "PaidAmount",
            //                caption: "القيمة المسددة",
            //                alignment: "center"
            //            },
            //            {
            //                dataField: "PayRollNumber",
            //                caption: "رقم المسير",
            //                alignment: "center"
            //            }
            //        ],
            //        summary: {
            //            totalItems: [{
            //                column: "PaidAmount",
            //                summaryType: "sum"
            //            }]
            //        }
            //    };
            //};

        }]);