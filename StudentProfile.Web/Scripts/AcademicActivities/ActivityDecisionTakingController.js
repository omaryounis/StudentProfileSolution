(function () {
    app.controller("ActivityDecisionTakingCtrl",
        ["$scope", "$http", "$timeout", "AcademicActivitiesSrvc",
            function ($scope, $http, $timeout, AcademicActivitiesSrvc) {

                /*********************************** Permissions *********************************/
                $scope.Permissions = {
                    EditActivityRequest: false
                };
                $http({
                    method: "get",
                    url: "/AcademicActivities/GetActivityDecisionTakingPermissions?screenId=73"
                }).then(function (data) {
                    debugger;
                    $scope.Permissions.EditActivityRequest = data.data.EditActivityRequest;
                });
                /*--------------------------------* Permissions *--------------------------------*/


                $scope.Init = function () {
                    $scope.StudentActivitiesApprovedGridInstance = '';
                    $scope.StudentActivityRequestGridInstance = '';
                    $scope.ActivityRequestPhasesGridInstance = '';
                    $scope.StudentBasicDataGridInstance = '';
                    $scope.ActivityRequestsGridInstance = '';
                };
                $scope.Init();


                //======================================================
                // قائمة طلبات المشاركات الموجودة في الصفحة الرئيسية
                //======================================================
                var DataSourceActivityRequestsGrid = new DevExpress.data.DataSource({
                    paginate: true,
                    cacheRawData: false,
                    key: "ActivityRequestId",
                    loadMode: "raw",
                    load: function () {
                        return $.getJSON("/AcademicActivities/GetActivityRequestsByUserId", function (data) { });
                    }
                });
                $scope.ActivityRequestsGrid = {
                    dataSource: DataSourceActivityRequestsGrid,
                    keyExpr: "ActivityRequestId",
                    bindingOptions: {
                        rtlEnabled: true
                    },
                    wordWrapEnabled: true,
                    showBorders: true,
                    searchPanel: {
                        width: 300,
                        visible: true,
                        placeholder: "بحث"
                    },
                    pager: {
                        allowedPageSizes: true, //"auto",
                        infoText: "( عدد الطلبات المرسلة ( {2} طلب",
                        showInfo: true,
                        showNavigationButtons: false,
                        showPageSizeSelector: true,
                        visible: true //"auto"
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
                        fileName: "ActivityRequests"
                    },
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
                    columns: [
                        {
                            caption: '#',
                            cssClass: "text-center",
                            cellTemplate: function (cellElement, cellInfo) {
                                cellElement.text(cellInfo.row.rowIndex + 1);
                            },
                            width: 30
                        },
                        {
                            dataField: "StudentAcademicData.STUDENT_NAME",
                            caption: "اسم الطالـب",
                            alignment: "right"
                        },
                        {
                            dataField: "StudentAcademicData.NATIONAL_ID",
                            caption: "رقم الهوية",
                            alignment: "right",
                            width: 110
                        },
                        {
                            dataField: "StudentAcademicData.STUDENT_ID",
                            caption: "رقم الطالب",
                            alignment: "right",
                            width: 110
                        },
                        {
                            dataField: "StudentAcademicData.NATIONALITY_DESC",
                            caption: "الجنسية",
                            alignment: "right",
                            width: 100
                        },
                        {
                            dataField: "ActivityLocation",
                            caption: "مكان المشاركة",
                            alignment: "right",
                            width: 140
                        },
                        {
                            dataField: "ActivityName",
                            caption: "اسم المشاركة",
                            alignment: "right"
                            //width: 160
                        },
                        {
                            dataField: "RequestedDate",
                            caption: "تاريخ الطلب",
                            alignment: "right", 
                            width: 110
                        },
                        {
                            //caption: "المرفق",
                            cssClass: "text-center",
                            width: 45,
                            cellTemplate: function (container, options) {
                                $("<div />").dxButton({
                                    text: '',
                                    hint: "تحميل",
                                    type: 'default',
                                    icon: 'fa fa-download',
                                    useSubmitBehavior: false,
                                    onClick: function (e) {
                                        return window.open('/AcademicActivities/DownloadActivityArchivedAttachment?activityArchivedId=' + options.data.ActivityRequestId, '_blank');
                                    }
                                }).appendTo(container);
                            }
                        },
                        {
                            width: 100,
                            caption: "معاينة الطلب",
                            cssClass: "text-center dark-text",
                            cellTemplate: function (container, options) {
                                $("<div />").dxButton({
                                    text: '',
                                    icon: "fa fa-address-card-o dark-text",
                                    width: 50,
                                    hint: "معاينة",
                                    onClick: function (e) {
                                        $scope.CurrentActivityRequestId = options.data.ActivityRequestId;

                                        return $http({
                                            method: "Get",
                                            url: "/AcademicActivities/StudentActivityDetailsDataSource",
                                            params: {
                                                ActivityRequestId: options.data.ActivityRequestId
                                            }
                                        }).then(function (data) {
                                            debugger;
                                            $scope.StudentActivityDetailsArray = data.data;
                                            //***************
                                            $scope.Refresh();
                                            //***************
                                            $scope.ActivityDetailsPopupVisible = true;
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
                    onInitialized: function (e) {
                        $scope.ActivityRequestsGridInstance = e.component;
                    },
                    selection: { mode: "single" },
                    sorting: {
                        mode: "multiple" // or "multiple"
                    },
                    rowAlternationEnabled: true,
                    hoverStateEnabled: true,
                    masterDetail: {
                        enabled: false,
                        template: ""
                    },
                    paging: {
                        pageSize: 5
                    }
                };


                //======================================================================
                // قائمة الخاصة ببيانات الطالب الأساسية الموجودة بداخل استعراض الطلب
                //======================================================================

                $scope.StudentActivityDetailsArray = '';
                var StudentBasicDataGridDataSource = new DevExpress.data.DataSource({
                    paginate: true,
                    cacheRawData: false,
                    loadMode: "raw",
                    key: "ActivityRequestId",
                    load: function () {
                        if ($scope.StudentActivityDetailsArray.length > 0) {
                            return $scope.StudentActivityDetailsArray;
                        } else {
                            return [];
                        }
                    }
                });
                $scope.StudentBasicDataGrid = function () {
                    return {
                        dataSource: StudentBasicDataGridDataSource,
                        "export": {
                            enabled: true,
                            fileName: "StudentBasicData"
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
                        paging: {
                            pageSize: 1
                        },
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
                        onInitialized: function (e) {
                            debugger;
                            $scope.StudentBasicDataGridInstance = e.component;
                        },
                        columns: [{
                            dataField: "studentAcademicData.StudentName",
                            caption: "إسم الطالب",
                            alignment: "center"
                        },
                        {
                            dataField: "studentAcademicData.FACULTY_NAME",
                            caption: "الكلية",
                            alignment: "center",
                            width: 180
                        }, {
                            dataField: "studentAcademicData.DEGREE_DESC",
                            caption: "الدرجة العلمية",
                            alignment: "center",
                            width: 130
                        }, {
                            dataField: "studentAcademicData.LEVEL_DESC",
                            caption: "المستوي",
                            alignment: "center",
                            width: 120
                        }, {
                            dataField: "studentAcademicData.STUDY_DESC",
                            caption: "نوع الدراسة",
                            alignment: "center",
                            width: 120
                        },
                        {
                            dataField: "studentAcademicData.STATUS_DESC",
                            caption: "الحالة",
                            alignment: "center",
                            width: 110
                        },
                        {
                            dataField: "studentAcademicData.ACADEMIC_AVG",
                            caption: "GPA",
                            alignment: "center",
                            width: 100
                        }]
                    };
                };


                //======================================================================
                // القائمة الخاصة ببيانات الطلب المرسل الموجودة بداخل استعراض الطلب
                //======================================================================
                var ActivityRequestGridDataSource = new DevExpress.data.DataSource({
                    paginate: true,
                    cacheRawData: false,
                    loadMode: "raw",
                    key: "ActivityRequestId",
                    load: function () {
                        if ($scope.StudentActivityDetailsArray.length > 0) {
                            debugger;
                            return $scope.StudentActivityDetailsArray;
                        } else {
                            return [];
                        }
                    }
                });
                $scope.StudentActivityRequestGrid = function () {
                    return {
                        dataSource: ActivityRequestGridDataSource,
                        "export": {
                            enabled: true,
                            fileName: "StudentActivityRequest"
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
                        paging: {
                            pageSize: 1
                        },
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
                        onCellPrepared: (e) => {
                            if (e.rowType === 'data') {
                                e.cellElement.css({ 'background-color': 'rgba(10, 77, 110, 0.03)', color: '#000000' });
                                e.cellElement.css({ 'font-weight': 'bold' });
                            }
                        },
                        onInitialized: function (e) {
                            $scope.StudentActivityRequestGridInstance = e.component;
                        },
                        columns: [
                            {
                                dataField: "studentActivityRequest.Name",
                                caption: "اسم المشاركة",
                                alignment: "center",
                                //width: 150
                            },
                            {
                                dataField: "studentActivityRequest.Location",
                                caption: "مكان المشاركة",
                                alignment: "center",
                                //width: 140
                            },
                            {
                                dataField: "studentActivityRequest.Type",
                                caption: "نوع المشاركة",
                                alignment: "center",
                                width: 120
                            },
                            {
                                dataField: "studentActivityRequest.Degree",
                                caption: "التقييم",
                                alignment: "center",
                                width: 95
                            },
                            {
                                dataField: "studentActivityRequest.Ratio",
                                caption: "النسبة",
                                alignment: "center",
                                width: 95
                            },
                            {
                                dataField: "studentActivityRequest.StartDate",
                                caption: "تاريخ البداية",
                                alignment: "center",
                                width: 125
                                //width: 175
                            },
                            {
                                dataField: "studentActivityRequest.EndDate",
                                caption: "تاريخ النهاية",
                                alignment: "center",
                                width: 125
                                //alignment: "center"
                            },
                            {
                                dataField: "studentActivityRequest.Duration",
                                caption: "عدد الساعات",
                                alignment: "center",
                                width: 100
                                //datatype: "date"
                                //alignment: "center"
                            },
                            {
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
                                            return window.open('/AcademicActivities/DownloadActivityArchivedAttachment?activityArchivedId=' + options.data.ActivityRequestId, '_blank');
                                        }
                                    }).appendTo(container);
                                }
                            },
                            {
                                caption: "",
                                cssClass: "text-center",
                                width: 50,
                                cellTemplate: function (container, options) {
                                    $("<div />").dxButton({
                                        text: '',
                                        hint: "تعديل",
                                        type: 'success',
                                        icon: 'fa fa-pencil',
                                        bindingOptions: {
                                            visible: "Permissions.EditActivityRequest"
                                        },
                                        useSubmitBehavior: false,
                                        onClick: function (e) {
                                            //جزئية تعديل بيانات المشاركة من عند اللي بيعتمد

                                            AcademicActivitiesSrvc.GetActivityRequestById($scope.CurrentActivityRequestId).then(function (data) {
                                                if (data.data === "") { swal("حدث خطأ", "لا يمكن التعديل نظرا لإرتباطه بسجلات أخري", "error"); }
                                                else {
                                                    debugger;
                                                    $scope.EditingActivityPopupShow = true;
                                                    $scope.MDL_ActivityDegreeToBeEditted = data.data.Degree;
                                                    $scope.MDL_ActivityDurationToBeEditted = data.data.Duration;
                                                    $scope.MDL_ActivityLocationToBeEditted = data.data.Location;
                                                    $scope.MDL_ActivityTypeToBeEditted = data.data.Type;
                                                    $scope.MDL_ActivityEndDateToBeEditted = data.data.EndDate;
                                                    $scope.MDL_ActivityStartDateToBeEditted = data.data.StartDate;
                                                    $scope.MDL_ActivityNameToBeEditted = data.data.Name;
                                                    $scope.MDL_ActivityRatioToBeEditted = data.data.Ratio;
                                                }

                                            });
                                        }
                                    }).appendTo(container);
                                }
                            }
                        ]
                    };
                };



                //===========================================================================================
                // القائمة الخاصة ببيانات المشاركات المعتمدة لهذا الطالب و الموجودة بداخل استعراض الطلب
                //===========================================================================================

                var ActivitiesApprovedGridDataSource = new DevExpress.data.DataSource({
                    paginate: true,
                    cacheRawData: false,
                    loadMode: "raw",
                    load: function () {
                        debugger;

                        if ($scope.StudentActivityDetailsArray[0]) {
                            debugger;
                            return $scope.StudentActivityDetailsArray[0].ActivitysApprovedRequests;
                        } else {
                            return [];
                        }
                    }
                });
                $scope.StudentActivitiesApprovedGrid = function () {
                    return {
                        dataSource: ActivitiesApprovedGridDataSource,
                        "export": {
                            enabled: true,
                            fileName: "ActivitiesApproved"
                        },
                        columnChooser: {
                            enabled: true
                        },
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
                        onInitialized: function (e) {
                            $scope.StudentActivitiesApprovedGridInstance = e.component;
                        },
                        columns: [
                            {
                                caption: '#',
                                width: 30,
                                alignment: "center",
                                cellTemplate: function (cellElement, cellInfo) {
                                    cellElement.text(cellInfo.row.rowIndex + 1);
                                }
                            },
                            {
                                dataField: "Name",
                                caption: "المشاركة",
                                alignment: "right"
                            },
                            {
                                dataField: "Duration",
                                caption: "عدد الساعات",
                                alignment: "right",
                                visible: false
                            },
                            {
                                dataField: "Location",
                                caption: "المكان",
                                alignment: "right"
                            },
                            {
                                dataField: "Type",
                                caption: "نوع المشاركة",
                                alignment: "right",
                                visible: false
                            },
                            {
                                dataField: "Degree",
                                caption: "التقييم",
                                alignment: "right",
                                visible: false
                            },
                            {
                                dataField: "Ratio",
                                caption: "النسبة",
                                alignment: "right",
                                visible: false
                            },
                            {
                                dataField: "StartDate",
                                caption: "تاريخ البداية",
                                alignment: "right",
                                dataType: "date",
                                width: 100
                            },
                            {
                                dataField: "EndDate",
                                caption: "تاريخ النهاية",
                                alignment: "right",
                                dataType: "date",
                                width: 100
                            },
                            {
                                dataField: "RefusalReason",
                                caption: "الملاحظات",
                                alignment: "right",
                                visible: false
                            },
                            {
                                //caption: "المرفقات",
                                cssClass: "text-center",
                                width: 45,
                                cellTemplate: function (container, options) {
                                    $("<div />").dxButton({
                                        text: '',
                                        hint: "تحميل",
                                        type: 'default',
                                        //icon: 'fa fa-download',
                                        useSubmitBehavior: false,
                                        onClick: function (e) {
                                            if (options.data.Status === 'Archived') {
                                                return window.open('/AcademicActivities/DownloadActivityArchivedAttachment?activityArchivedId=' + options.data.AcademicActivitiesId, '_blank');
                                            } else {
                                                return window.open('/AcademicActivities/DownloadActivityMasterAttachment?activityRequestMasterId=' + options.data.AcademicActivitiesId, '_blank');
                                            }
                                        }
                                    }).appendTo(container);
                                }
                            }
                        ]
                    };
                };


                //=============================================================================================
                // القائمة الخاصة ببيانات مراحل الإعتماد فيما يخص هذا الطلب و الموجودة بداخل استعراض الطلب
                //=============================================================================================

                var ActivityRequestPhasesGridDataSource = new DevExpress.data.DataSource({
                    paginate: true,
                    cacheRawData: false,
                    loadMode: "raw",
                    load: function () {
                        if ($scope.StudentActivityDetailsArray[0]) {
                            return $scope.StudentActivityDetailsArray[0].ActivityApprovedPhases;
                        } else {
                            return [];
                        }
                    }
                });
                $scope.ActivityRequestPhasesGrid = function () {
                    return {

                        dataSource: ActivityRequestPhasesGridDataSource,
                        "export": {
                            enabled: true,
                            fileName: "ActivityRequestPhases"
                        },
                        columnChooser: {
                            enabled: true
                        },
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
                        onInitialized: function (e) {
                            $scope.ActivityRequestPhasesGridInstance = e.component;
                        },
                        columns:
                        [{
                            caption: '#',
                            cssClass: "text-center",
                            cellTemplate: function (cellElement, cellInfo) {
                                cellElement.text(cellInfo.row.rowIndex + 1);
                            },
                            width: 30
                        },
                        {
                            dataField: "PhaseName",
                            caption: "المرحلة",
                            alignment: "right"
                        },
                        {
                            dataField: "Name",
                            caption: "القائم بالاعتماد",
                            alignment: "right",
                            //width: 120
                        },
                        {
                            dataField: "Reason",
                            caption: "ملاحظات الإعتماد",
                            alignment: "right",
                            visible: false
                        },
                        {
                            dataField: "ResponseDate",
                            caption: "تاريخ الإعتماد",
                            alignment: "right",
                            //dataType: "date",
                            visible: true
                        }]
                    };
                };


                //==============================
                // الشاشة الخاصة بمعاينة الطلب
                //==============================
                $scope.ActivityDetailsPopup = {
                    bindingOptions: {
                        visible: "ActivityDetailsPopupVisible"
                    },
                    shadingColor: "rgba(0, 0, 0, 0.69)",
                    closeOnOutsideClick: false,
                    height: 600,
                    width: 1300,
                    showTitle: false,
                    title: "",
                    dragEnabled: false,
                    onHiding: function () {
                        debugger;
                        $scope.StudentActivityDetailsArray = [];
                        $scope.CurrentActivityRequestId = '';

                        $scope.Refresh();

                        $('body').css('overflow', 'auto');
                        DataSourceActivityRequestsGrid.reload();
                        $scope.ActivityRequestsGridInstance.refresh();
                    }
                };

                $scope.AcceptButton = {
                    text: 'اعتماد',
                    type: "success",
                    icon: "fa fa-thumbs-up",
                    width: "150",
                    onClick: function (e) {
                        debugger;

                        $scope.type = "A";
                        $scope.ConfirmationPopupHeight = 400;
                        $scope.ConfirmationPopupContent = "acceptPopupContent";
                        $scope.ConfirmationPopupTitle = "توصيات اقرار طلب المشاركة";

                        $scope.ConfirmationPopupShow = true;
                        $scope.ConfirmButtonType = "success";
                        $scope.ConfirmButtonText = "تأكيد الإعتماد";
                    }
                };
                $scope.RefuseButton = {
                    text: 'رفض',
                    type: "danger",
                    width: "150",
                    icon: "fa fa-thumbs-down",
                    onClick: function (e) {

                        $scope.type = "R";
                        $scope.ConfirmationPopupHeight = 400;
                        $scope.ConfirmationPopupContent = "refusePopupContent";
                        $scope.ConfirmationPopupTitle = "توصيات رفض  طلب المشاركة";

                        $scope.ConfirmationPopupShow = true;
                        $scope.ConfirmButtonType = "danger";
                        $scope.ConfirmButtonText = "تأكيد الرفض";
                    }
                };

                $scope.CloseButton = {
                    text: 'رجوع',
                    //type: "default",
                    elementAttr: { "class": "btn-secondary" },
                    width: "150",
                    icon: "fa fa-arrow-left",
                    onClick: function (e) {
                        $scope.ActivityDetailsPopupVisible = false;
                    }
                };


                //========================================
                // الشاشة الخاصة بتأكيد القبول أو الرفض 
                //========================================
                $scope.ConfirmationPopup = {
                    showTitle: true,
                    dragEnabled: false,
                    shadingColor: "rgba(0, 0, 0, 0.69)",
                    closeOnOutsideClick: false,
                    bindingOptions: {

                        title: "ConfirmationPopupTitle",
                        visible: "ConfirmationPopupShow",
                        height: "ConfirmationPopupHeight",
                        contentTemplate: 'ConfirmationPopupContent'
                    },
                    width: 420,

                    rtlEnabled: true,
                    onHiding: function () {
                        $scope.recommendationsOfRefuse = '';
                        $scope.recommendationsOfAccept = '';
                    }
                };

                $scope.recommendationsOfRefuseTextArea = {
                    bindingOptions: {
                        value: "recommendationsOfRefuse"
                    },
                    height: 200,
                    maxLength: 200,
                    placeholder: "ادخل توصيات رفض المشاركة ( حقل إلزامي ) ",
                    cssClass: "text-center"
                };
                $scope.recommendationsOfAcceptTextArea = {
                    bindingOptions: {
                        value: "recommendationsOfAccept"
                    },
                    height: 200,
                    maxLength: 200,
                    placeholder: "ادخل توصيات إقرار المشاركة ( حقل إلزامي ) "
                };

                $scope.ConfirmButton = {
                    bindingOptions: {
                        text: 'ConfirmButtonText',
                        type: 'ConfirmButtonType'
                    },
                    visible: true,
                    validationGroup: "ConfirmationForm_VR",
                    useSubmitBehavior: true,
                    onClick: function (e) {

                        if ($scope.type === "R" && ($scope.recommendationsOfRefuse === "" || $scope.recommendationsOfRefuse === null || $scope.recommendationsOfRefuse === undefined)) {
                            DevExpress.ui.notify({
                                message: "عفوا ادخل توصيات رفض المشاركة",
                                type: "error",
                                displayTime: 1000,
                                closeOnClick: true
                            });
                            return;
                        }

                        if ($scope.type === "A" && ($scope.recommendationsOfAccept === "" || $scope.recommendationsOfAccept === null || $scope.recommendationsOfAccept === undefined)) {
                            DevExpress.ui.notify({
                                message: "عفوا ادخل توصيات إقرار المشاركة",
                                type: "error",
                                displayTime: 1000,
                                closeOnClick: true
                            });
                            return;
                        }

                        if ($scope.type === "R") {
                            AcademicActivitiesSrvc.ConfirmActivityRequest($scope.CurrentActivityRequestId, $scope.recommendationsOfRefuse, $scope.type)
                                .then(function (data) {
                                    if (data.data === "") {
                                        $scope.CurrentActivityRequestId = '';
                                        $scope.recommendationsOfRefuse = '';
                                        $scope.recommendationsOfAccept = '';

                                        swal("Done!", "تم رفض الطلب بنجاح", "success").then((value) => {
                                            $scope.StudentActivityDetailsArray = '';

                                            $scope.ActivityDetailsPopupVisible = false;
                                            $scope.ConfirmationPopupShow = false;

                                            $scope.Refresh();

                                            return value;
                                        });
                                    } else {
                                        swal("حدث خطأ", data.data, "error");
                                    }
                                });
                        } else {
                            AcademicActivitiesSrvc.ConfirmActivityRequest($scope.CurrentActivityRequestId, $scope.recommendationsOfAccept, $scope.type)
                                .then(function (data) {
                                    if (data.data === "") {
                                        $scope.CurrentActivityRequestId = '';
                                        $scope.recommendationsOfRefuse = '';
                                        $scope.recommendationsOfAccept = '';

                                        swal("Done!", "تم إعتماد الطلب بنجاح", "success").then((value) => {
                                            $scope.StudentActivityDetailsArray = '';

                                            $scope.ActivityDetailsPopupVisible = false;
                                            $scope.ConfirmationPopupShow = false;

                                            $scope.Refresh();
                                            return value;
                                        });
                                    } else {
                                        swal("حدث خطأ", data.data, "error");
                                    }
                                });
                        }

                    }
                };

                $scope.Refresh = function () {
                    ActivityRequestGridDataSource.reload();
                    ActivitiesApprovedGridDataSource.reload();
                    StudentBasicDataGridDataSource.reload();
                    ActivityRequestPhasesGridDataSource.reload();
                };


                //=======================================
                //الشاشة الخاصة بتعديل بيانات المشاركة
                //=======================================
                var now = new Date();
                $scope.EditingActivity = {
                    PopupOptions: {
                        showTitle: true,
                        dragEnabled: false,
                        shadingColor: "rgba(0, 0, 0, 0.69)",
                        closeOnOutsideClick: false,
                        bindingOptions: {
                            visible: "EditingActivityPopupShow"
                        },
                        contentTemplate: 'EditingActivityPopupContent',
                        title: "تعديل بيانات المشاركة",
                        width: 840,
                        height: 540,
                        rtlEnabled: true,
                        onHiding: function () {
                            debugger;
                            $scope.ResetControls();
                            ActivityRequestGridDataSource.reload();
                        }
                    },
                    VR_Required: {
                        validationRules: [{ type: "required", message: "حقل إلزامي" }],
                        validationGroup: "ActivityForm_vg"
                    },
                    TB_ActivityName: {
                        bindingOptions: {
                            value: "MDL_ActivityNameToBeEditted"
                        },
                        onValueChanged: function (e) {
                            $scope.MDL_ActivityNameToBeEditted = e.value;
                        },
                        placeholder: "",
                        rtlEnabled: true
                    },
                    TB_ActivityDuration: {
                        bindingOptions: {
                            value: "MDL_ActivityDurationToBeEditted"
                        },
                        onValueChanged: function (e) {
                            $scope.MDL_ActivityDurationToBeEditted = e.value;
                        },
                        placeholder: "",
                        rtlEnabled: true
                    },
                    TB_ActivityLocation: {
                        bindingOptions: {
                            value: "MDL_ActivityLocationToBeEditted"
                        },
                        onValueChanged: function (e) {
                            $scope.MDL_ActivityLocationToBeEditted = e.value;
                        },
                        placeholder: "",
                        rtlEnabled: true
                    },
                    TB_ActivityType: {
                        bindingOptions: {
                            value: "MDL_ActivityTypeToBeEditted"
                        },
                        onValueChanged: function (e) {
                            $scope.MDL_ActivityTypeToBeEditted = e.value;
                        },
                        placeholder: "",
                        rtlEnabled: true
                    },
                    TB_ActivityDegree: {
                        bindingOptions: {
                            value: "MDL_ActivityDegreeToBeEditted"
                        },
                        onValueChanged: function (e) {
                            $scope.MDL_ActivityDegreeToBeEditted = e.value;
                        },
                        placeholder: "",
                        rtlEnabled: true
                    },
                    NB_ActivityRatio: {
                        bindingOptions: {
                            value: "MDL_ActivityRatioToBeEditted"
                        },
                        onValueChanged: function (e) {
                            $scope.MDL_ActivityRatioToBeEditted = e.value;
                        },
                        placeholder: "",
                        max: 100,
                        rtlEnabled: true,
                        showSpinButtons: true,
                        showClearButton: true
                    },
                    DB_ActivityStartDate: {
                        bindingOptions: {
                            value: "MDL_ActivityStartDateToBeEditted"
                        },
                        max: now,
                        type: "date",
                        displayFormat: "dd/MM/yyyy",
                        onValueChanged: function (e) {
                            $scope.MDL_ActivityStartDateToBeEditted = e.value;
                            if (new Date($scope.MDL_ActivityStartDateToBeEditted) > new Date($scope.MDL_ActivityEndDateToBeEditted)) {
                                $scope.MDL_ActivityEndDateToBeEditted = '';
                            }
                        }
                    },
                    DB_ActivityEndDate: {
                        bindingOptions: {
                            value: "MDL_ActivityEndDateToBeEditted",
                            min: "MDL_ActivityStartDateToBeEditted"
                        },
                        max: now,
                        type: "date",
                        displayFormat: "dd/MM/yyyy",
                        onValueChanged: function (e) {
                            $scope.MDL_ActivityEndDateToBeEditted = e.value;
                        }
                    },
                    btnSaveOptions: {
                        text: 'تعديل',
                        visible: true,
                        type: 'success',
                        validationGroup: "ActivityForm_vg",
                        useSubmitBehavior: true,
                        onClick: function (e) {
                            var validationGroup = DevExpress.validationEngine.getGroupConfig("ActivityForm_vg");
                            if (validationGroup) {
                                var result = validationGroup.validate();
                                if (result.isValid) {

                                    if (new Date($scope.MDL_ActivityEndDate) < new Date($scope.MDL_ActivityStartDate)) {
                                        return DevExpress.ui.notify({ message: "لا يمكن أن يكون تاريخ بداية المشاركة بعد تاريخ نهاية المشاركة" }, "Error", 10000);
                                    }
                                    debugger;
                                    AcademicActivitiesSrvc.EditActivityRequest($scope.CurrentActivityRequestId,
                                        $scope.MDL_ActivityDegreeToBeEditted,
                                        $scope.MDL_ActivityDurationToBeEditted,
                                        $scope.MDL_ActivityLocationToBeEditted,
                                        $scope.MDL_ActivityTypeToBeEditted,
                                        $scope.MDL_ActivityStartDateToBeEditted,
                                        $scope.MDL_ActivityEndDateToBeEditted,
                                        $scope.MDL_ActivityNameToBeEditted,
                                        $scope.MDL_ActivityRatioToBeEditted).then(function (data) {
                                            if (data.data !== "") { swal("حدث خطأ", data.data, "error"); }
                                            else {
                                                $scope.StudentActivityDetailsArray = [];

                                                $http({
                                                    method: "Get",
                                                    url: "/AcademicActivities/StudentActivityDetailsDataSource",
                                                    params: {
                                                        ActivityRequestId: $scope.CurrentActivityRequestId
                                                    }
                                                }).then(function (data) {
                                                    $scope.StudentActivityDetailsArray = data.data;
                                                    //***************
                                                    $scope.Refresh();
                                                    //***************
                                                    $scope.EditingActivityPopupShow = false;
                                                    swal("Done!", "تم الحفظ بنجاح", "success");
                                                });

                                            }
                                        });
                                }
                            }
                        }
                    }
                };
                $scope.ResetControls = function () {
                    debugger;
                    $scope.MDL_ActivityTypeToBeEditted = '';
                    $scope.MDL_ActivityNameToBeEditted = '';
                    $scope.MDL_ActivityRatioToBeEditted = '';
                    $scope.MDL_ActivityDegreeToBeEditted = '';
                    $scope.MDL_ActivityEndDateToBeEditted = '';
                    $scope.MDL_ActivityDurationToBeEditted = '';
                    $scope.MDL_ActivityLocationToBeEditted = '';
                    $scope.MDL_ActivityStartDateToBeEditted = '';
                };
            }
        ]);
})();
