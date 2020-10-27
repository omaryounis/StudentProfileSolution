(function () {
    app.controller("SearchActivityMenusCtrl",
        ["$scope", "$http",
            function ($scope, $http) {

                // Start Permissions Scope...
                $scope.Permissions = {
                    Edit: false,
                    Post: false,
                    Delete: false
                };
                $http({
                    method: "get",
                    url: "/AcademicActivities/GetSearchActivityMenusPermissions?screenId=100"
                }).then(function (data) {
                    
                    $scope.Permissions.Edit = data.data.Edit;
                    $scope.Permissions.Post = data.data.Post;
                    $scope.Permissions.Delete = data.data.Delete;
                });
                // End Permissions Scope...
                
                $scope.ActivityMainGridInstance = '';

                var DataSourceActivityMainGrid = new DevExpress.data.DataSource({
                    paginate: true,
                    cacheRawData: false,
                    key: "ID",
                    loadMode: "raw",
                    load: function () {
                        return $.getJSON("/AcademicActivities/GetActivityMenusDataSource", function (data) {});
                    }
                });

                $scope.ActivityMainGrid = {
                    dataSource: DataSourceActivityMainGrid,
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
                        width: 230
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
                        visible: true,
                        emptyPanelText: ""
                    },
                    noDataText: "لايوجد بيانات",
                    columnAutoWidth: true,
                    columnChooser: {
                        enabled: true
                    },
                    "export": {
                        enabled: true,
                        fileName: "Activities"
                    },
                    onCellPrepared: function (e) {
                        if (e.rowType === "header" && e.column.command === "edit") {
                            e.column.width = 100;
                        }

                        if (e.rowType === "data" && e.column.command === "edit") {
                            $links = e.cellElement.find(".dx-link");
                            $links.text("");
                            if (e.data.StatusCode === 1) {
                                e.column.visible = false;
                            } else {
                                $links.filter(".dx-link-edit").addClass("btn btn-dark-orange btn-sm fa fa-pencil");
                                $links.filter(".dx-link-delete").addClass("btn btn-danger btn-sm fa fa-trash-o");
                            }
                        }

                        if (e.rowType === "data" && e.column.dataField === "ApprovedStatus") {
                            if (e.value === "تحت المراجعة") {
                                e.cellElement.css({ color: '#c7bb27', "font-weight": "bold" });

                            }
                            else if (e.value === "معتمدة") {
                                e.cellElement.css({ color: '#5cb85c', "font-weight": "bold" });

                            }
                            else if (e.value === "مرفوضة") {
                                e.cellElement.css({ color: '#d9534f', "font-weight": "bold" });
                            }
                            else {
                                e.cellElement.css({ "font-weight": "bold" });
                            }
                        }

                    },
                    editing: {
                        allowUpdating: false,
                        allowAdding: false,
                        allowDeleting: false,
                        mode: "row",
                        texts: {
                            confirmDeleteMessage: "تأكيد حذف  قائمة المشاركة ؟",
                            deleteRow: "",
                            editRow: "",
                            addRow: ""
                        },
                        useIcons: true
                    },
                    columns: [
                        {
                            dataField: "ActivityLocation",
                            caption: "مكان المشاركة",
                            alignment: "right",
                            //width: 160
                        },
                        {
                            dataField: "ActivityName",
                            caption: "اسم المشاركة",
                            alignment: "right",
                            //width: 160
                        },
                        {
                            dataField: "ActivityNoFormated",
                            caption: "رقم المشاركة",
                            alignment: "right",
                            width: 125
                        },
                        {
                            dataField: "InsertionDate",
                            caption: "تاريخ الإنشاء",
                            alignment: "right",
                            dataType: "date",
                            width: 125
                        },
                        {
                            dataField: "StudentsCount",
                            caption: "عدد الطلاب",
                            alignment: "right",
                            width: 110
                        },
                        {
                            dataField: "ApprovedStatus",
                            caption: "حالة الإعتماد",
                            alignment: "right",
                            width: 140
                        },
                        {
                            dataField: "StatusNotes",
                            caption: "ملاحظات الإعتماد",
                            alignment: "right",
                            visible: false
                        },
                        {
                            //caption: "المرفقات",
                            cssClass: "text-center",
                            width: 50,
                            cellTemplate: function (container, options) {
                                $("<div />").dxButton({
                                    text: '',
                                    hint: "تحميل",
                                    type: 'default',
                                    icon: 'fa fa-download',
                                    useSubmitBehavior: false,
                                    onClick: function (e) {
                                        return window.open('/AcademicActivities/DownloadActivityMasterAttachment?activityRequestMasterId=' + options.data.ID, '_blank');
                                    }
                                }).appendTo(container);
                            }
                        },
                        {
                            caption: "",
                            cssClass: "text-center",
                            width: 45,
                            dataField: "IsPosted",
                            cellTemplate: function (container, options) {
                                $("<div />").dxButton({
                                    text: '',
                                    hint: "تعديل",
                                    type: 'success',
                                    icon: 'fa fa-pencil',
                                    visible: !options.value,
                                    bindingOptions: {
                                        // "Permissions.",
                                    },
                                    useSubmitBehavior: false,
                                    onClick: function (e) {
                                        return window.open("/AcademicActivities/UpdateActivityMenu/?paramMasterId=" + options.data.ID, "_self");
                                    }
                                }).appendTo(container);
                            }
                        },
                        {
                            caption: "",
                            cssClass: "text-center",
                            width: 45,
                            dataField: "IsPosted",
                            cellTemplate: function (container, options) {
                                $("<div />").dxButton({
                                    text: '',
                                    hint: "حذف",
                                    type: 'danger',
                                    icon: 'fa fa-trash-o',
                                    visible: !options.value,
                                    bindingOptions: {
                                        // "Permissions.Edit",
                                    },
                                    useSubmitBehavior: false,
                                    onClick: function (e) {

                                        var result = DevExpress.ui.dialog.confirm("هل تريد حقاً حذف هذا العنصر؟!", "تأكيد الحذف");
                                        result.done(function (dialogResult) {
                                            if (dialogResult === true) {
                                                $http({
                                                    method: "Delete",
                                                    url: "/AcademicActivities/DeleteActivityMenu",
                                                    data: { paramActivityMasterId: options.data.ID }
                                                }).then(function (data) {
                                                    if (data.data !== "") {
                                                        return swal("حدث خطأ", data.data, "error");
                                                    } else {
                                                        DataSourceActivityMainGrid.reload();
                                                        return swal("Done!", "تم الحذف بنجاح", "success");
                                                    }
                                                });
                                            }
                                        });
                                    }
                                }).appendTo(container);
                            }
                        },
                        {
                            width: 140,
                            caption: "",
                            cssClass: "text-center dark-text",
                            dataField: "IsPosted",
                            cellTemplate: function (container, options) {
                                debugger;
                                $("<div />").dxButton({
                                    text: 'توجيه للاعتماد',
                                    icon: "chevrondoubleleft",
                                    elementAttr: { "class": "btn-dark" },
                                    hint: "ترحيل للاعتماد",
                                    visible: !options.value,
                                    bindingOptions: {
                                        // "Permissions.",
                                    },
                                    //icon: "fa fa-address-card-o dark-text",//"fa fa-binoculars"
                                    width: 140,
                                    onClick: function (e) {

                                        var result = DevExpress.ui.dialog.confirm("هل تريد حقاً توجيه هذه القائمة للاعتماد؟", "تأكيد التوجيه");
                                        result.done(function (dialogResult) {
                                            if (dialogResult === true) {
                                                $http({
                                                    method: "POST",
                                                    url: "/AcademicActivities/PostActivityMenu/",
                                                    data: {
                                                        paramActivityMasterId: options.data.ID
                                                    }
                                                }).then(function (data) {
                                                    if (data.data === "") {
                                                        swal("Done!", "تم توجيه القائمة للاعتماد بنجاح", "success");
                                                        $scope.ActivityMainGridInstance.refresh();
                                                    } else {
                                                        swal("حدث خطأ", data.data, "error");
                                                    }
                                                });
                                            }
                                        });
                                    }
                                }).appendTo(container);
                            }
                        }
                    ],
                    wordWrapEnabled: true,
                    allowColumnResizing: true,
                    columnResizingMode: "widget",
                    allowColumnReordering: true,
                    showColumnLines: true,
                    rowAlternationEnabled: true,
                    onInitialized: function (e) {
                        $scope.ActivityMainGridInstance = e.component;
                    },
                    masterDetail: {
                        enabled: true,
                        template: "ActivityRequestMasterContent"
                    }
                };

                $scope.GetActivityRequestMaster = function (key) {
                    return {
                        dataSource: new DevExpress.data.DataSource({
                            paginate: true,
                            cacheRawData: false,
                            key: "ID",
                            //loadMode: "raw",
                            load: function () {
                                debugger;
                                return $.getJSON(`/AcademicActivities/GetActivityMasterById?paramActivityMasterId=${key}`, function (data) { });
                            }
                        }),
                        sorting: {
                            mode: "multiple"
                        },
                        wordWrapEnabled: false,
                        showBorders: true,
                        paging: {
                            pageSize: 1
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
                                dataField: "HoursNo",
                                caption: "عدد الساعات المعتمدة",
                                alignment: "center"
                            },
                            {
                                dataField: "ActivityTypeName",
                                caption: "نوع المشاركة",
                                alignment: "center"
                            },
                            {
                                dataField: "StartDate",
                                caption: "تاريخ بداية المشاركة",
                                dataType: "date",
                                alignment: "center"
                            },
                            {
                                dataField: "EndDate",
                                caption: "تاريخ نهاية المشاركة",
                                dataType: "date",
                                alignment: "center"
                            }
                        ],
                        masterDetail: {
                            enabled: true,
                            template: "ActivityRequestDetailsGridContent"
                        }
                    };
                };

                $scope.ActivityRequestDetailsGrid = function (key) {
                    return {
                        dataSource: new DevExpress.data.DataSource({
                            paginate: true,
                            cacheRawData: false,
                            key: "RequestDetailsId",
                            loadMode: "raw",
                            load: function () {
                                debugger;
                                return $.getJSON(`/AcademicActivities/GetActivityRequestDetails?paramActivityMasterId=${key}`, function (data) { });
                            }
                        }),
                        columnAutoWidth: true,
                        showBorders: true,
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
                        sorting: {
                            mode: "multiple"
                        },
                        wordWrapEnabled: true,
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
                            visible: true
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
                                dataField: "StudentName",
                                caption: "اسم الطالب",
                                alignment: "right"
                            },
                            {
                                dataField: "NationalId",
                                caption: "رقم الهوية",
                                alignment: "right"
                            },
                            {
                                dataField: "Student_Id",
                                caption: "الرقم الجامعي",
                                alignment: "right"
                            },
                            {
                                dataField: "EvaluationName",
                                caption: "التقييم",
                                alignment: "right"
                            }
                        ]
                    };
                };

            }
        ]);

})();


//var result = DevExpress.ui.dialog.confirm("هل تريد حقاً حذف هذا العنصر؟!", "تأكيد الحذف");
//result.done(function (dialogResult) {
//    if (dialogResult === true) {
//        $.ajax({
//            url: "/JournalEntry/DeleteJournal",
//            data: { GJHID: gjhid }
//        }).done(function (data) {
//            if (data === "") {
//                DevExpress.ui.notify({
//                    message: "تم الحذف بنجاح",
//                    type: "success",
//                    displayTime: 3000,
//                    closeOnClick: true
//                });
//                grd_Juornals.PerformCallback();
//            } else {
//                DevExpress.ui.notify({
//                    message: data,
//                    type: "error",
//                    displayTime: 3000,
//                    closeOnClick: true
//                });
//            }
//        });
//    }
//});