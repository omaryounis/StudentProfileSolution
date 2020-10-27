(function () {
    app.controller("AdvanceDecisionTakingCtrl",
        ["$scope", "$http", "$timeout", "StudentAdvanceSrvc",
            function ($scope, $http, $timeout, StudentAdvanceSrvc) {

                $scope.studentExcludedDetalie = "";

                $scope.Init = function () {
                    $scope.StudentAdvancesApprovedGridInstance = '';
                    $scope.StudentAdvanceRequestGridInstance = '';
                    $scope.AdvanceRequestPhasesGridInstance = '';
                    $scope.StudentBasicDataGridInstance = '';
                    $scope.AdvanceRequestsGridInstance = '';
                };
                $scope.Init();

                /*********************************** Permissions *********************************/
                $scope.Permissions = {
                    View: false,
                    Check: false
                };

                $http({
                    method: "Get",
                    url: "/Advances/GetAdvanceDecisionTakingPermissions?screenId=82"
                }).then(function (data) {
                    $scope.Permissions.View = data.data.View ? data.data.View : false;
                    $scope.Permissions.Check = data.data.Check ? data.data.Check : false;
                });
                /*--------------------------------* Permissions *--------------------------------*/

                //==================================================
                // قائمة طلبات السلف الموجودة في الصفحة الرئيسية
                //==================================================
                var DataSourceAdvanceRequestsGrid = new DevExpress.data.DataSource({
                    paginate: true,
                    cacheRawData: false,
                    key: "AdvanceRequestId",
                    loadMode: "raw",
                    load: function () {
                        return $.getJSON("/Advances/GetAdvanceRequestsByUserId?type=A", function (data) {
                            debugger;
                        });
                    }
                });
                $scope.AdvanceRequestsGrid = {
                    dataSource: DataSourceAdvanceRequestsGrid,
                    keyExpr: "AdvanceRequestId",
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
                    allowColumnResizing: true,
                    columnResizingMode: "widget",
                    allowColumnReordering: true,
                    showColumnLines: true,
                    selection: { mode: "single" },
                    sorting: {
                        mode: "multiple"
                    },
                    rowAlternationEnabled: true,
                    hoverStateEnabled: true,
                    paging: {
                        pageSize: 5
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
                        fileName: "AdvanceRequests"
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
                            width: 120
                        },
                        {
                            dataField: "StudentAcademicData.STUDENT_ID",
                            caption: "رقم الطالب",
                            alignment: "right",
                            width: 120
                        },
                        {
                            dataField: "StudentAcademicData.NATIONALITY_DESC",
                            caption: "الجنسية",
                            alignment: "right",
                            width: 120
                        },
                        {
                            dataField: "AdvanceRequestId",
                            caption: "رقم السلفه",
                            alignment: "right",
                            width: 120
                        },
                        {
                            dataField: "AdvanceName",
                            caption: "نوع السلفة",
                            alignment: "right",
                            width: 140
                        }, {
                            dataField: "RequestedValue",
                            caption: "المبلغ المطلوب",
                            alignment: "right",
                            width: 140
                        },
                        {
                            dataField: "RequestedDate",
                            caption: "تاريخ الطلب",
                            alignment: "right",
                            //dataType: "date",
                            width: 120
                        },
                        {
                            dataField: "CreatedBy",
                            caption: "اسم المرسل",
                            alignment: "right",
                            //visible: false,
                            width: 120
                        },
                        {
                            dataField: "RequestNotes",
                            caption: "الملاحظات",
                            alignment: "right",
                            //visible: false,
                            width: 200
                        },
                        {
                            caption: "المرفقات",
                            cssClass: "text-center",
                            width: 80,
                            cellTemplate: function (container, options) {
                                $("<div />").dxButton({
                                    text: '',
                                    hint: "تحميل",
                                    type: 'success',
                                   // disabled: !$scope.Permissions.Attach,
                                    icon: 'fa fa-download',
                                    useSubmitBehavior: false,
                                    onClick: function (e) {
                                        return window.open('/Advances/DownloadAdvanceAttachment?advanceRequestId=' + options.data.AdvanceRequestId, '_blank');
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
                                    icon: "fa fa-address-card-o dark-text",//"fa fa-binoculars"
                                    width: 50,
                                    disabled: !$scope.Permissions.Check,
                                    hint: "معاينة",
                                    onClick: function (e) {

                                         $http({
                                            method: "Get",
                                            url: "/Advances/ExcludeAdvanceOrdersForStudentResons",
                                            params: {
                                                studentId: options.data.studentId,
                                                AdvanceId: options.data.AdvanceSettingId
                                            }
                                        }).then(function (data) {
                                          $scope.studentExcludedDetalie = options.data.IsExcluded.Item2;
                                        });

                                        $scope.CurrentAdvanceRequestId = options.data.AdvanceRequestId;
                                        debugger;
                                        return $http({
                                            method: "Get",
                                            url: "/Advances/StudentAdvancesDetailDataSource",
                                            params: {
                                                advanceRequestId: options.data.AdvanceRequestId
                                            }
                                        }).then(function (data) {
                                            debugger;
                                            $scope.StudentAdvanceDetailsArray = data.data;

                                            $scope.Refresh();

                                            $scope.AdvanceDetailsPopupVisible = true;
                                            $('body').css('overflow', 'hidden');
                                        });
                                    }
                                }).appendTo(container);
                            }
                        }
                    ],
                    onInitialized: function (e) {
                        $scope.AdvanceRequestsGridInstance = e.component;
                    },
                    onRowPrepared: function (e) {
                        debugger;
                        if (e.rowType === "data") {
                            if (e.data.IsExcluded.Item1 === true)
                            e.rowElement.addClass("red-ds");
                        }
                    }
         
                };


                //======================================================================
                // قائمة الخاصة ببيانات الطالب الأساسية الموجودة بداخل استعراض الطلب
                //======================================================================
                $scope.StudentAdvanceDetailsArray = '';
                var StudentBasicDataGridDataSource = new DevExpress.data.DataSource({
                    paginate: true,
                    cacheRawData: false,
                    loadMode: "raw",
                    key: "advanceRequestId",
                    load: function () {
                        debugger;
                        if ($scope.StudentAdvanceDetailsArray.length > 0) {
                            return $scope.StudentAdvanceDetailsArray;
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
                        scrolling: {
                            rtlEnabled: true,
                            useNative: true,
                            scrollByContent: true,
                            scrollByThumb: true,
                            showScrollbar: "onHover",
                            mode: "standard", // or "virtual"
                            direction: "both"
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
                var AdvanceRequestGridDataSource = new DevExpress.data.DataSource({
                    paginate: true,
                    cacheRawData: false,
                    loadMode: "raw",
                    key: "advanceRequestId",
                    load: function () {
                        debugger;

                        if ($scope.StudentAdvanceDetailsArray.length > 0) {
                            return $scope.StudentAdvanceDetailsArray;
                        } else {
                            return [];
                        }
                    }
                });
                $scope.StudentAdvanceRequestGrid = function () {
                    return {
                        dataSource: AdvanceRequestGridDataSource,
                        "export": {
                            enabled: true,
                            fileName: "StudentAdvanceRequest"
                        },
                        sorting: {
                            mode: "multiple"
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
                            $scope.StudentAdvanceRequestGridInstance = e.component;
                        },
                        onCellPrepared: (e) => {
                            if (e.rowType === 'data') {
                                debugger;
                                e.cellElement.css({ 'background-color': 'rgba(10, 77, 110, 0.03)', color: '#000000' });
                                e.cellElement.css({ 'font-weight': 'bold' });
                                //e.cellElement.css({ color: '#AAAAAA' });
                                //if (e.data.SaleAmount > 15000) {
                                //    if (e.column.dataField === 'OrderNumber') {
                                //        e.cellElement.css({ 'font-weight': 'bold' });
                                //    }
                                //    if (e.column.dataField === 'SaleAmount') {
                                //        e.cellElement.css({ 'background-color': '#FFBB00', color: '#000000' });
                                //    }
                                //}
                            }
                        },
                        columns: [
                            {
                                dataField: "advanceRequestId",
                                caption: "رقم السلفة",
                                alignment: "center",
                                width: 160
                            },
                            {
                                dataField: "studentAdvanceRequest.AdvanceName",
                                caption: "نوع السلفة",
                                alignment: "center",
                                width: 160
                            },
                            {
                                dataField: "studentAdvanceRequest.RequestedValue",
                                caption: "المبلغ المطلوب",
                                alignment: "center",
                                width: 160
                            },
                            {
                                dataField: "studentAdvanceRequest.RequestedDate",
                                caption: "تاريخ الطلب",
                                alignment: "center",
                                //dataType: "date",
                                width: 200
                            },
                            {
                                dataField: "studentAdvanceRequest.ApprovedValue",
                                caption: "المبلغ المعتمد",
                                alignment: "center",
                                width: 150
                            },
                            {
                                caption: "المرفقات",
                                cssClass: "text-center",
                                width: 80,
                                cellTemplate: function (container, options) {
                                    debugger;
                                    $("<div />").dxButton({
                                        text: '',
                                        hint: "تحميل",
                                        type: 'success',
                                        //disabled: !$scope.Permissions.Attach,
                                        icon: 'fa fa-download',
                                        useSubmitBehavior: false,
                                        onClick: function (e) {
                                            return window.open('/Advances/DownloadAdvanceAttachment?advanceRequestId=' + $scope.CurrentAdvanceRequestId, '_blank');
                                        }
                                    }).appendTo(container);
                                }
                            },{
                                dataField: "studentAdvanceRequest.RequestNotes",
                                caption: "الملاحظـات",
                                alignment: "center"
                            }
                        ]
                    };
                };


                //======================================================================================
                // القائمة الخاصة ببيانات السلف المصروفة لهذا الطالب و الموجودة بداخل استعراض الطلب
                //=======================================================================================
                var AdvancesApprovedGridDataSource = new DevExpress.data.DataSource({
                    paginate: true,
                    cacheRawData: false,
                    loadMode: "raw",
                    load: function () {
                        debugger;
                        if ($scope.StudentAdvanceDetailsArray[0]) {
                            return $scope.StudentAdvanceDetailsArray[0].advancesapprovedRequests;
                        } else {
                            return [];
                        }
                    }
                });
                $scope.StudentAdvancesApprovedGrid = function () {
                    return {
                        dataSource: AdvancesApprovedGridDataSource,
                        "export": {
                            enabled: true,
                            fileName: "AdvancesApproved"
                        },
                        columnChooser: {
                            enabled: true
                        },
                        columnAutoWidth: true,
                        showBorders: true,
                        sorting: {
                            mode: "multiple"
                        },
                        scrolling: {
                            rtlEnabled: true,
                            useNative: true,
                            scrollByContent: true,
                            scrollByThumb: true,
                            showScrollbar: "onHover",
                            mode: "virtual", // or "virtual"
                            direction: "both"
                        },
                        wordWrapEnabled: false,
                        paging: {
                            pageSize: 5
                        },
                        pager: {
                            allowedPageSizes: "auto",
                            infoText: "(صفحة  {0} من {1} ({2} عنصر",
                            showInfo: true,
                            showNavigationButtons: false,
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
                            $scope.StudentAdvancesApprovedGridInstance = e.component;
                        },
                        columns: [
                            {
                                caption: '#',
                                cssClass: "text-center",
                                cellTemplate: function (cellElement, cellInfo) {
                                    cellElement.text(cellInfo.row.rowIndex + 1);
                                },
                                width: 40
                            },
                            {
                                dataField: "AdvanceSettingName",
                                caption: "النوع",
                                alignment: "center"
                            },
                            {
                                dataField: "ApprovedValue",
                                caption: "المبلغ المعتمد",
                                alignment: "center"
                            }, {
                                dataField: "PaidAmount",
                                caption: "المبلغ المسدد",
                                alignment: "center"
                            },
                            {
                                dataField: "RemainAmount",
                                caption: "المتبقي",
                                alignment: "center"
                            },
                            {
                                dataField: "ApprovedDate",
                                caption: "تاريخ الإعتماد",
                                alignment: "center"
                            }
                        ]
                    };
                };


                //=============================================================================================
                // القائمة الخاصة ببيانات مراحل الإعتماد فيما يخص هذا الطلب و الموجودة بداخل استعراض الطلب
                //=============================================================================================
                var AdvanceRequestPhasesGridDataSource = new DevExpress.data.DataSource({
                    paginate: true,
                    cacheRawData: false,
                    loadMode: "raw",
                    load: function () {
                        if ($scope.StudentAdvanceDetailsArray[0]) {
                            return $scope.StudentAdvanceDetailsArray[0].advanceApprovedPhases;
                        } else {
                            return [];
                        }
                    }
                });
                $scope.AdvanceRequestPhasesGrid = function () {
                    return {
                        dataSource: AdvanceRequestPhasesGridDataSource,
                        "export": {
                            enabled: true,
                            fileName: "AdvanceRequestPhases"
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
                        scrolling: {
                            rtlEnabled: true,
                            useNative: true,
                            scrollByContent: true,
                            scrollByThumb: true,
                            showScrollbar: "onHover",
                            mode: "virtual", // or "virtual"
                            direction: "both"
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
                            $scope.AdvanceRequestPhasesGridInstance = e.component;
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
                            alignment: "center"
                        },
                        {
                            dataField: "RequestedValue",
                            caption: "المبلغ المطلوب",
                            alignment: "center"
                        },
                        {
                            dataField: "ApprovedValue",
                            caption: "المبلغ المعتمد",
                            alignment: "center"
                        },
                        {
                            dataField: "ResponseDate",
                            caption: "تاريخ الإعتماد",
                            alignment: "center"
                        },
                        {
                            dataField: "Name",
                            caption: "المسؤول",
                            alignment: "center",
                            width: 100
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
                            alignment: "center",
                            dataType: "date",
                            visible: false
                        }]
                    };
                };


                //==============================
                // الشاشة الخاصة بمعاينة الطلب
                //==============================
                $scope.AdvanceDetailsPopup = {
                    bindingOptions: {
                        visible: "AdvanceDetailsPopupVisible"
                    },
                    shadingColor: "rgba(0, 0, 0, 0.69)",
                    closeOnOutsideClick: false,
                    height: 650,
                    width: 1600,
                    showTitle: false,
                    title: "",
                    dragEnabled: false,
                    onHiding: function () {
                        debugger;
                        $scope.NB_ApprovedValue_Disabled = true;
                        $scope.StudentAdvanceDetailsArray = [];
                        $scope.CurrentAdvanceRequestId = '';

                        $scope.Refresh();

                        $('body').css('overflow', 'auto');
                        DataSourceAdvanceRequestsGrid.reload();
                        $scope.AdvanceRequestsGridInstance.refresh();
                    }
                };


                $scope.NB_RequestedValue = {
                    bindingOptions: {
                        value: "MDL_RequestedValue"
                    },
                    placeholder: "القيمة المطلوبة",
                    disabled: true,
                    rtlEnabled: true,
                    showSpinButtons: true,
                    showClearButton: true
                };

                $scope.NB_ApprovedValue_Disabled = true;
                $scope.NB_ApprovedValue = {
                    bindingOptions: {
                        //max: "MDL_RequestedValue",
                        value: "MDL_ApprovedValue",
                        disabled: "NB_ApprovedValue_Disabled"
                    },
                    // min: 1,
                    placeholder: "القيمة المعتمدة",
                    rtlEnabled: true,
                    showSpinButtons: true,
                    showClearButton: true,
                    onValueChanged: function (e) {
                        debugger;
                        $scope.MDL_ApprovedValue = e.value;
                        if (e.value > $scope.StudentAdvanceDetailsArray[0].studentAdvanceRequest.MaxRequestValue) {
                            swal("تنبيه", "لايمكن أن تكون القيمة المعتمدة أكبر من القيمة المطلوبة", "warning");
                            $scope.MDL_ApprovedValue = '';
                            return;
                        }
                        if (e.value <= 0 && $scope.MDL_ApprovedValue !== '') {
                            $scope.MDL_ApprovedValue = '';
                            swal("تنبيه", "لايمكن أن تكون القيمة المعتمدة أقل من أو تساوي صفر", "warning");
                            return;
                        }
                    }
                };

                $scope.AcceptButton = {
                    text: 'اعتماد الطلب',
                    type: "success",
                    width: "150",
                    icon: "fa fa-thumbs-up",
                    onClick: function (e) {

                        if ($scope.StudentAdvanceDetailsArray[0].nextApprovePhase.IsFinancialPhase === true) {
                            $scope.NB_ApprovedValue_Disabled = false;
                        }

                        $scope.MDL_RequestedValue = $scope.StudentAdvanceDetailsArray[0].studentAdvanceRequest.RequestedValue;

                        debugger;
                        let CurrentApprovedValue = $scope.StudentAdvanceDetailsArray[0].advanceApprovedPhases;
                        if (CurrentApprovedValue.length > 0) {
                           // $scope.MDL_ApprovedValue = Math.min(...CurrentApprovedValue.map(x => x.ApprovedValue));

                            var length = $scope.StudentAdvanceDetailsArray[0].advanceApprovedPhases.length;
                            var lastObject = $scope.StudentAdvanceDetailsArray[0].advanceApprovedPhases[length - 1];
                            $scope.MDL_ApprovedValue = lastObject.ApprovedValue;

                        } else {
                            debugger;
                            $scope.MDL_ApprovedValue = $scope.MDL_RequestedValue;
                        }
                        //here we put validation

                        $scope.type = "A";
                        $scope.ConfirmationPopupHeight = 550;
                        $scope.ConfirmationPopupContent = "acceptPopupContent";
                        $scope.ConfirmationPopupTitle = "توصيات اقرار طلب السلفة"; 
                        $scope.ConfirmationPopupShow = true;
                        $scope.ConfirmButtonType = "success";
                        $scope.ConfirmButtonText = "تأكيد الإعتماد";
                    }
                };
                $scope.RefuseButton = {
                    text: 'رفض الطلب',
                    type: "danger",
                    width: "150",
                    icon: "fa fa-thumbs-down",
                    onClick: function (e) {
                        debugger;
                        $scope.type = "R";
                        $scope.ConfirmationPopupHeight = 400;
                        $scope.ConfirmationPopupContent = "refusePopupContent";
                        $scope.ConfirmationPopupTitle = "توصيات رفض  طلب السلفة";

                        $scope.ConfirmationPopupShow = true;
                        $scope.ConfirmButtonType = "danger";
                        $scope.ConfirmButtonText = "تأكيد الرفض";
                    }
                };
                $scope.CloseButton = {
                    text: 'رجوع',
                    type: "default",
                    width: "150",
                    icon: "fa fa-arrow-left",
                    onClick: function (e) {
                        $scope.AdvanceDetailsPopupVisible = false;
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
                    width: 400,
                    rtlEnabled: true,
                    onHiding: function () {
                        $scope.recommendationsOfRefuse = '';
                        $scope.recommendationsOfAccept = '';
                    },
                    onShowing: function () {
                        $scope.recommendationsOfRefuse = "تم الرفض وفقا للأنظمة المعمول بها";
                        $scope.recommendationsOfAccept = "تم الاعتماد وفقا للأنظمة المعمول بها";
                    }
                };

                $scope.recommendationsOfRefuseTextArea = {
                    bindingOptions: {
                        value: "recommendationsOfRefuse"
                    },
                    height: 200,
                    maxLength: 200,
                    placeholder: "ادخل توصيات رفض السلفة ( حقل إلزامي ) ",
                    cssClass: "text-center"
                };

                $scope.recommendationsOfAcceptTextArea = {
                    bindingOptions: {
                        value: "recommendationsOfAccept"
                    },
                    height: 200,
                    maxLength: 200,
                    placeholder: "ادخل توصيات إقرار السلفة ( حقل إلزامي ) "
                };

                $scope.ConfirmButton = {
                    bindingOptions: {
                        text: 'ConfirmButtonText',
                        type: 'ConfirmButtonType'
                    },
                    validationGroup: "ConfirmationForm_VR",
                    useSubmitBehavior: true,
                    onClick: function (e) {

                        if ($scope.type === "R" && ($scope.recommendationsOfRefuse === "" || $scope.recommendationsOfRefuse === null || $scope.recommendationsOfRefuse === undefined)) {
                            DevExpress.ui.notify({
                                message: "عفوا ادخل توصيات رفض السلفة",
                                type: "error",
                                displayTime: 1000,
                                closeOnClick: true
                            });
                            return;
                        }

                        if ($scope.type === "A" && ($scope.recommendationsOfAccept === "" || $scope.recommendationsOfAccept === null || $scope.recommendationsOfAccept === undefined)) {
                            DevExpress.ui.notify({
                                message: "عفوا ادخل توصيات إقرار السلفة",
                                type: "error",
                                displayTime: 1000,
                                closeOnClick: true
                            });
                            return;
                        }


                        if ($scope.type === "A" &&
                            ($scope.MDL_ApprovedValue < 1 ||
                                $scope.MDL_ApprovedValue === "" ||
                                $scope.MDL_ApprovedValue === null ||
                                $scope.MDL_ApprovedValue === undefined)) {

                            DevExpress.ui.notify({
                                message: "عفوا أدخل قيمة السلفة المعتمدة بشكل صحيح",
                                type: "error",
                                displayTime: 1000,
                                closeOnClick: true
                            });
                            return;
                        }
                        debugger;
                        if ($scope.type === "A" && $scope.MDL_ApprovedValue > $scope.StudentAdvanceDetailsArray[0].studentAdvanceRequest.MaxRequestValue) {
                            DevExpress.ui.notify({
                                message: "لايمكن أن تكون القيمة المعتمدة أكبر من القيمة المطلوبة",
                                type: "error",
                                displayTime: 1000,
                                closeOnClick: true
                            });
                            return;
                        }
                        
                        if ($scope.type === "R") {
                            // لاحظ تم وضع القيمة بصفر عند الرفض
                            $scope.MDL_ApprovedValue = 0;
                            StudentAdvanceSrvc.ConfirmAdvanceRequest($scope.CurrentAdvanceRequestId, $scope.recommendationsOfRefuse, $scope.type, $scope.MDL_ApprovedValue)
                                .then(function (data) {
                                    if (data.data === "") {
                                        $scope.CurrentAdvanceRequestId = '';
                                        $scope.recommendationsOfRefuse = '';
                                        $scope.recommendationsOfAccept = '';

                                        swal("Done!", "تم رفض الطلب بنجاح", "success").then((value) => {
                                            $scope.StudentAdvanceDetailsArray = '';

                                            $scope.AdvanceDetailsPopupVisible = false;
                                            $scope.ConfirmationPopupShow = false;

                                            $scope.Refresh();
                                            return value;
                                        });
                                    } else {
                                        swal("حدث خطأ", data.data, "error");
                                    }
                                });
                        } else {
                           //here put validation of serverside
                            StudentAdvanceSrvc.CheckValidationOfAdvance($scope.CurrentAdvanceRequestId)
                                .then(function (data) {
                                    if (data.data === "") {
                                        StudentAdvanceSrvc.ConfirmAdvanceRequest($scope.CurrentAdvanceRequestId, $scope.recommendationsOfAccept, $scope.type, $scope.MDL_ApprovedValue)
                                            .then(function (data) {
                                                if (data.data === "") {
                                                    $scope.CurrentAdvanceRequestId = '';
                                                    $scope.recommendationsOfRefuse = '';
                                                    $scope.recommendationsOfAccept = '';

                                                    swal("Done!", "تم إعتماد الطلب بنجاح", "success").then((value) => {
                                                        $scope.StudentAdvanceDetailsArray = '';

                                                        $scope.AdvanceDetailsPopupVisible = false;
                                                        $scope.ConfirmationPopupShow = false;

                                                        $scope.Refresh();
                                                        return value;
                                                    });
                                                } else {
                                                    swal("حدث خطأ", data.data, "error");
                                                }
                                            });
                                    }
                                    else {
                                        //swal("حدث خطأ", data.data, "error");
                                        swal({
                                            title: "تنبيه",
                                            text: data.data + " " + " هل تريد الإعتماد ؟",
                                            icon: "warning",
                                            buttons: [
                                                'إلغاء',
                                                'موافقة'
                                            ],
                                            dangerMode: true,
                                        }).then(function (isConfirm) {

                                            if (isConfirm) {
                                                StudentAdvanceSrvc.ConfirmAdvanceRequest($scope.CurrentAdvanceRequestId, $scope.recommendationsOfAccept, $scope.type, $scope.MDL_ApprovedValue)
                                                    .then(function (data) {
                                                        if (data.data === "") {
                                                            $scope.CurrentAdvanceRequestId = '';
                                                            $scope.recommendationsOfRefuse = '';
                                                            $scope.recommendationsOfAccept = '';

                                                            swal("Done!", "تم إعتماد الطلب بنجاح", "success").then((value) => {
                                                                $scope.StudentAdvanceDetailsArray = '';

                                                                $scope.AdvanceDetailsPopupVisible = false;
                                                                $scope.ConfirmationPopupShow = false;

                                                                $scope.Refresh();
                                                                return value;
                                                            });
                                                        } else {
                                                            swal("حدث خطأ", data.data, "error");
                                                        }
                                                    });
                                            } else {

                                            }

                                        })
                                    }
                                })
                        }

                    }
                };

                $scope.Refresh = function () {

                    AdvanceRequestGridDataSource.reload();
                    AdvancesApprovedGridDataSource.reload();
                    StudentBasicDataGridDataSource.reload();
                    AdvanceRequestPhasesGridDataSource.reload();

                    //if ($scope.StudentBasicDataGridInstance) {
                    //    $scope.StudentBasicDataGridInstance.getDataSource().store()._array = $scope.StudentAdvanceDetailsArray;
                    //    $scope.StudentBasicDataGridInstance.getDataSource().reload();
                    //}

                    //if ($scope.StudentAdvanceRequestGridInstance) {
                    //    $scope.StudentAdvanceRequestGridInstance.getDataSource().store()._array = $scope.StudentAdvanceDetailsArray;
                    //    $scope.StudentAdvanceRequestGridInstance.getDataSource().reload();
                    //}

                    //if ($scope.AdvanceRequestPhasesGridInstance) {
                    //    $scope.AdvanceRequestPhasesGridInstance.getDataSource().store()._array = $scope.StudentAdvanceDetailsArray[0].advanceApprovedPhases;
                    //    $scope.AdvanceRequestPhasesGridInstance.getDataSource().reload();

                    //}

                    //if ($scope.StudentAdvancesApprovedGridInstance) {
                    //    $scope.StudentAdvancesApprovedGridInstance.getDataSource().store()._array = $scope.StudentAdvanceDetailsArray[0].advancesapprovedRequests;
                    //    $scope.StudentAdvancesApprovedGridInstance.getDataSource().reload();
                    //}
                };
            }
        ]);
})();