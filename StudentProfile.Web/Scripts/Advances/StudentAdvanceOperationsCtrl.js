(function () {
    app.controller("StudentAdvanceOperationsCtrl",
        ["$scope", "StudentAdvanceSrvc", "$http",
            function ($scope, StudentAdvanceSrvc, $http) {
                $scope.studentExcludedDetalie = "";

                $scope.PopupStudentExcludedResonShow = false;

                $scope.Init = function () {
                    $scope.StudentAdvancesApprovedGridInstance = '';
                    $scope.StudentAdvancePremiumsGridInstance = '';
                    $scope.AdvanceRequestPhasesGridInstance = '';
                    $scope.StudentBasicDataGridInstance = '';
                    $scope.AdvanceRequestsGridInstance = '';
                    $scope.StudentAdvanceDetailsArray = '';
                    $scope.StudentBasicDataArray = '';
                    $scope.SubsidiesGridInstance = '';
                    $scope.AdvancesGridInstance = '';
                };
                $scope.Init();

                /*********************************** Permissions *********************************/
                $scope.Permissions = {
                    View: false,
                    AdvenceRequest: false,
                    SubsidyRequest: false,
                    CheckAdvence: false,
                    CheckSubsidy: false
                };

                $http({
                    method: "Get",
                    url: "/Advances/GetStudentAdvanceOperationsPermissions?screenId=84"
                }).then(function (data) {
                    debugger;
                    $scope.Permissions.View = data.data.View;
                    $scope.Permissions.AdvenceRequest = data.data.AdvenceRequest ? data.data.AdvenceRequest : false;
                    $scope.Permissions.SubsidyRequest = data.data.SubsidyRequest ? data.data.SubsidyRequest : false;
                    $scope.Permissions.CheckAdvence = data.data.CheckAdvence ? data.data.CheckAdvence : false;
                    $scope.Permissions.CheckSubsidy = data.data.CheckSubsidy ? data.data.CheckSubsidy : false;

                    //$scope.SubsidiesGridInstance.refresh();
                });

                /*--------------------------------* Permissions *--------------------------------*/


                //===================
                // DDL Student Search
                //===================
                let DDL_StudentsDataSource = new DevExpress.data.DataSource({
                    paginate: true,
                    cacheRawData: true,
                    key: "STUDENT_ID",
                    loadMode: "raw",
                    load: function () {
                        debugger;
                        return $.getJSON("/Advances/GetAllStudents", function (data) {
                        });
                    }
                });
                DDL_StudentsDataSource.load();

                $scope.DDL_Students = {
                    dataSource: DDL_StudentsDataSource,
                    bindingOptions: {
                        value: "MDL_StudentId"
                    },
                    placeholder: 'بحث ...',
                    noDataText: "لا يوجد بيانات",
                    displayExpr: "STUDENT_NAME",
                    valueExpr: "STUDENT_ID",
                    showBorders: true,
                    searchEnabled: true,
                    searchExpr: ['STUDENT_ID', 'NATIONAL_ID', 'STUDENT_NAME'],
                    showClearButton: true,
                    rtlEnabled: true,
                    onValueChanged: function (e) {
                        debugger;
                        $scope.MDL_StudentId = e.value;
                        if (e.value !== null && e.value !== '' && e.value !== undefined) {

                            // * Filling Personal Controls...
                            StudentAdvanceSrvc.GetStudentDataByID(e.value).then(function (data) {
                                debugger;
                                $scope.StudentBasicDataArray = data.data;

                                $scope.MDL_LevelName = data.data[0].StudentBasicData.LevelName;
                                $scope.MDL_DegreeName = data.data[0].StudentBasicData.DegreeName;
                                $scope.MDL_NationalId = data.data[0].StudentBasicData.National_Id;
                                $scope.MDL_StatusName = data.data[0].StudentBasicData.StatusName;
                                $scope.MDL_StudentNo = data.data[0].StudentBasicData.Student_Id;
                                $scope.MDL_FacultiyName = data.data[0].StudentBasicData.FacultiyName;
                                $scope.MDL_StudyTypeName = data.data[0].StudentBasicData.StudyTypeName;
                                $scope.MDL_NationalityName = data.data[0].StudentBasicData.NationalityName;
                                $scope.MDL_StudentAverageDegree = data.data[0].StudentAverageDegree;
                            });
                        } else {
                            $scope.MDL_StudentNo = null;
                            $scope.MDL_LevelName = null;
                            $scope.MDL_DegreeName = null;
                            $scope.MDL_NationalId = null;
                            $scope.MDL_StatusName = null;
                            $scope.MDL_FacultiyName = null;
                            $scope.MDL_StudyTypeName = null;
                            $scope.MDL_NationalityName = null;
                            $scope.MDL_StudentAverageDegree = null;
                        }
                        debugger;
                        SubsidiesGriddataSource.reload();
                        AdvancesGriddataSource.reload();
                    }
                };



                //======================
                // StudentBasicData Menu
                //======================
                $scope.Menu_StudentBasicData = {
                    TB_Facultiy: {
                        bindingOptions: {
                            value: "MDL_FacultiyName"
                        },
                        rtlEnabled: true,
                        readOnly: true
                    },
                    TB_Nationality: {
                        bindingOptions: {
                            value: "MDL_NationalityName"
                        },
                        rtlEnabled: true,
                        readOnly: true
                    },
                    TB_LevelName: {
                        bindingOptions: {
                            value: "MDL_LevelName"
                        },
                        rtlEnabled: true,
                        readOnly: true
                    },
                    TB_StudentId: {
                        bindingOptions: {
                            value: "MDL_StudentNo"
                        },
                        rtlEnabled: true,
                        readOnly: true
                    },
                    TB_DegreeName: {
                        bindingOptions: {
                            value: "MDL_DegreeName"
                        },
                        rtlEnabled: true,
                        readOnly: true
                    },
                    TB_StudyTypeName: {
                        bindingOptions: {
                            value: "MDL_StudyTypeName"
                        },
                        rtlEnabled: true,
                        readOnly: true
                    },
                    TB_NationalId: {
                        bindingOptions: {
                            value: "MDL_NationalId"
                        },
                        rtlEnabled: true,
                        readOnly: true
                    },
                    TB_StatusName: {
                        bindingOptions: {
                            value: "MDL_StatusName"
                        },
                        rtlEnabled: true,
                        readOnly: true
                    },
                    TB_StudentAverageDegree: {
                        bindingOptions: {
                            value: "MDL_StudentAverageDegree"
                        },
                        rtlEnabled: true,
                        readOnly: true
                    }
                };



                //====================================
                // قائمة الإعانات في الصفحة الرئيسية
                //====================================
                let SubsidiesGriddataSource = new DevExpress.data.DataSource({
                    paginate: true,
                    cacheRawData: false,
                    key: "AdvanceRequestId",
                    loadMode: "raw",
                    load: function () {
                        if ($scope.MDL_StudentId) {
                            return $.getJSON("/Advances/AdvancesDataSource?studentId=" + $scope.MDL_StudentId + "&type=S", function (data) { });
                        }
                        return [];
                    }
                });
                $scope.SubsidiesGrid = {

                    dataSource: SubsidiesGriddataSource,
                    "export": {
                        enabled: true,
                        fileName: "StudentSubsidies"
                    },
                    sorting: {
                        mode: "multiple"
                    },
                    keyExpr: "AdvanceRequestId",
                    columnAutoWidth: true,
                    columnChooser: {
                        enabled: true
                    },
                    wordWrapEnabled: false,
                    showBorders: true,
                    paging: {
                        pageSize: 5
                    },
                    pager: {
                        //allowedPageSizes: "auto",
                        infoText: "(صفحة  {0} من {1} ({2} عنصر",
                        showInfo: true,
                        showNavigationButtons: true,
                        showPageSizeSelector: true,
                        allowedPageSizes: [5,10],
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
                    onToolbarPreparing: function (e) {
                        e.toolbarOptions.items.unshift(
                            {
                                location: "after",
                                widget: "dxButton",
                                options: {
                                    text: "طلب إعانة",
                                    //type:"success",
                                    icon: "plus",
                                    hint: "إضافة طلب إعانة جديد",
                                    onClick: function (event) {
                                        debugger;
                                        if (!$scope.Permissions.SubsidyRequest) {
                                            return swal("تنبيه", "عفوا ليس لديك صلاحية علي هذه العملية", "warning");
                                        }

                                        if ($scope.MDL_StudentId === '' || $scope.MDL_StudentId === undefined || $scope.MDL_StudentId === null) {
                                            return swal("تنبيه", "برجاء اختيار الطالب أولا", "warning");
                                        }

                                        $scope.PopUpwidth = 500;
                                        $scope.PopUpheight = 450;
                                        $scope.PopupTitle = "إضافة طلب إعانة جديد";
                                        $scope.PopupContent = "AddNewSubsidyContent";
                                        $scope.AddNewPopUpShow = true;
                                    }
                                }
                            });
                    },
                    onSelectionChanged: function (selectedItems) {
                        // $scope.selectedEmployee = selectedItems.selectedRowsData[0];
                    },
                    onCellPrepared: (e) => {
                        if (e.rowType === 'data') {
                            if (e.column.dataField === "ApprovedStatus") {
                                debugger;
                                //if (e.value === "تحت المراجعة") {
                                //    e.cellElement.css({ 'background-color': '#0e70a040', color: '#000000', "font-weight": "bold" });

                                //}
                                //if (e.value === "معتمد") {
                                //    e.cellElement.css({ 'background-color': 'rgba(139, 195, 74, 0.4)', color: '#000000', "font-weight": "bold" });

                                //}
                                //if (e.value === "تم الرفض") {
                                //    e.cellElement.css({ 'background-color': 'rgba(244, 67, 54, 0.4)', color: '#000000', "font-weight": "bold" });
                                //}
                                debugger;
                                if (e.value === "تحت المراجعة") {
                                    e.cellElement.css({ color: '#c7bb27', "font-weight": "bold" });

                                }
                                if (e.value === "معتمد") {
                                    e.cellElement.css({ color: '#5cb85c', "font-weight": "bold" });

                                }
                                if (e.value === "مرفوض") {
                                    e.cellElement.css({ color: '#d9534f', "font-weight": "bold" });
                                }

                                //e.cellElement.css({ "font-style": "italic" });
                            }
                            //if (e.data.OrderDate < new Date(2014, 2, 3)) {
                            //    e.cellElement.css({ color: "#AAAAAA" });
                            //}
                            //if (e.data.SaleAmount > 15000) {
                            //    if (e.column.dataField === "OrderNumber") {
                            //        e.cellElement.css({ "font-weight": "bold" });
                            //    }
                            //    if (e.column.dataField === "SaleAmount") {
                            //        e.cellElement.css({ "background-color": "#FFBB00", "color": "#000" });
                            //    }
                            //}
                            //debugger;
                            ////e.cellElement.css({ color: '#AAAAAA' });
                            //e.cellElement.css({ 'background-color': 'rgba(10, 77, 110, 0.03)', color: '#000000' });
                            //e.cellElement.css({ 'font-weight': 'bold' });
                        }
                    },
                    onInitialized: function (e) {
                        debugger;
                        $scope.SubsidiesGridInstance = e.component;
                    },
                    columns: [
                        //{
                        //    caption: '#',
                        //    cssClass: "text-center",
                        //    cellTemplate: function (cellElement, cellInfo) {
                        //        cellElement.text(cellInfo.row.rowIndex + 1);
                        //    },
                        //    width: 30
                        //},
                        {
                            dataField: "AdvanceSettingName",
                            caption: "النوع",
                            alignment: "center",
                            //width: 160
                        },
                        {
                            dataField: "RequestedValue",
                            caption: "المبلغ المطلوب",
                            alignment: "center",
                            width: 110
                        },
                        {
                            dataField: "InsertionDate",
                            caption: "تاريخ الطلب",
                            alignment: "center",
                            //dataType: "date",
                            //width: 150
                        },
                        {
                            dataField: "ApprovedStatus",
                            caption: "حالة الاعتماد",
                            alignment: "center",
                            //width: 175
                        },
                        {
                            dataField: "ApprovedValue",
                            caption: "المبلغ المعتمد",
                            alignment: "center",
                            width: 110
                        },
                        {
                            dataField: "AdvanceIsPaid",
                            caption: "حالة الصرف",
                            alignment: "center"
                            //width: 175
                        },
                        {

                            //width: 110,
                            caption: "التفاصيل",
                            cssClass: "text-center dark-text",
                            cellTemplate: function (container, options) {
                                $("<div />").dxButton({
                                    text: '',
                                    type: 'default',
                                    icon: "fa fa-address-card-o dark-text",//"fa fa-binoculars"
                                    width: 60,
                                    disabled: !$scope.Permissions.CheckSubsidy,
                                    hint: "معاينة",
                                    onClick: function (e) {
                                        if ($scope.Permissions.CheckSubsidy) {       

                                            $scope._advanceRequestId = options.data.AdvanceRequestId;
                                            debugger;
                                            return $http({
                                                method: "Get",
                                                url: "/Advances/StudentAdvanceDetailDataSource",
                                                params: {
                                                    advanceRequestId: options.data.AdvanceRequestId
                                                }
                                            }).then(function (data) {
                                                debugger;
                                                $scope.StudentAdvanceDetailsArray = data.data;

                                                $scope.ContentTemplateForDetailsPopup = "SubsidyDetailsPopupContent";
                                                $scope.AdvanceDetailsPopupVisible = true;

                                                $('body').css('overflow', 'hidden');
                                            });
                                        }
                                        else {
                                            return swal("تنبيه", "عفوا ليس لديك صلاحية علي هذه العملية", "warning");

                                        }

                                    }
                                }).appendTo(container);
                            }
                        }, {
                            caption: "حذف",
                            //width: 60,
                            cssClass: "text-center",
                            cellTemplate: function (container, options) {
                                if (options.data.IsApprovedPhase === false) {
                                    $("<div />").dxButton({
                                        icon: "fa fa-trash-o",
                                        //text: "حذف",
                                        type: "danger",
                                        hint: "حذف",
                                        width: 60,
                                        elementAttr: { "class": "btn btn-danger btn-sm" },
                                        onClick: function (e) {
                                            var result = DevExpress.ui.dialog.confirm("<i>هل انت متاكد؟</i>", "تاكيد الحذف");
                                            result.done(function (dialogResult) {
                                                if (dialogResult) {
                                                    StudentAdvanceSrvc.DeleteAdvanceRequests(options.data.AdvanceRequestId).then(function (data) {
                                                        if (data.data.status === 500) {
                                                            DevExpress.ui.notify({ message: data.data.Message, type: data.data.Type, displayTime: 3000, closeOnClick: true });
                                                        }
                                                        if (data.data.status === 200) {
                                                            DevExpress.ui.notify({ message: data.data.Message, type: data.data.Type, displayTime: 3000, closeOnClick: true });
                                                            //Referesh Grid
                                                            AdvancesGriddataSource.reload();
                                                            $scope.AdvancesGridInstance.refresh();
                                                            SubsidiesGriddataSource.reload();
                                                            $scope.SubsidiesGridInstance.refresh();
                                                        }
                                                    });
                                                }
                                            });
                                        }
                                    }).appendTo(container);
                                }
                            }
                        }
                    ]
                };



                //==================================
                // قائمة السلف في الصفحة الرئيسية
                //==================================
                let AdvancesGriddataSource = new DevExpress.data.DataSource({
                    paginate: true,
                    cacheRawData: false,
                    key: "AdvanceRequestId",
                    loadMode: "raw",
                    load: function () {
                        if ($scope.MDL_StudentId) {
                            return $.getJSON("/Advances/AdvancesDataSource?studentId=" + $scope.MDL_StudentId + "&type=A", function (data) { debugger; });
                        }
                        return [];
                    }
                });
                $scope.AdvancesGrid = {

                    dataSource: AdvancesGriddataSource,
                    "export": {
                        enabled: true,
                        fileName: "StudentAdvances"
                    },
                    sorting: {
                        mode: "multiple"
                    },
                    keyExpr: "AdvanceRequestId",
                    columnAutoWidth: true,
                    columnChooser: {
                        enabled: true
                    },
                    wordWrapEnabled: false,
                    showBorders: true,
                    paging: {
                        pageSize: 5
                    },
                    pager: {
                        //allowedPageSizes: "auto",
                        infoText: "(صفحة  {0} من {1} ({2} عنصر",
                        showInfo: true,
                        showNavigationButtons: true,
                        showPageSizeSelector: true,
                        allowedPageSizes: [5, 10],
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
                    onToolbarPreparing: function (e) {
                        e.toolbarOptions.items.unshift(
                            {
                                location: "after",
                                widget: "dxButton",
                                options: {
                                    //type:"success",
                                    text: "طلب سلفة",
                                    icon: "plus",
                                    hint: "إضافة طلب سلفة جديدة",
                                    onClick: function (event) {
                                        debugger;

                                        if (!$scope.Permissions.AdvenceRequest) {
                                            return swal("تنبيه", "عفوا ليس لديك صلاحية علي هذه العملية", "warning");
                                        }

                                        if ($scope.MDL_StudentId === null || $scope.MDL_StudentId === '' || $scope.MDL_StudentId === undefined) {
                                            return swal("تنبيه", "برجاء اختيار الطالب أولا", "warning");
                                        }

                                        $scope.PopUpwidth = 1000;
                                        $scope.PopUpheight = 450;
                                        $scope.PopupTitle = "إضافة طلب سلفة جديد";
                                        $scope.PopupContent = "AddNewAdvanceContent";
                                        $scope.AddNewPopUpShow = true;
                                    }
                                }
                            });
                    },
                    onSelectionChanged: function (selectedItems) {
                        // $scope.selectedEmployee = selectedItems.selectedRowsData[0];
                    },
                    onInitialized: function (e) {
                        $scope.AdvancesGridInstance = e.component;
                    },
                    onCellPrepared: (e) => {
                        if (e.rowType === 'data') {
                            debugger;
                            if (e.column.dataField === "ApprovedStatus") {
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
                        }
                    },
                    columns: [
                        //{
                        //    caption: '#',
                        //    cssClass: "text-center",
                        //    cellTemplate: function (cellElement, cellInfo) {
                        //        cellElement.text(cellInfo.row.rowIndex + 1);
                        //    },
                        //    width: 30
                        //},
                        {
                            dataField: "AdvanceSettingName",
                            caption: "النوع",
                            alignment: "center",
                            //width: 160
                        },
                        {
                            dataField: "RequestedValue",
                            caption: "المبلغ المطلوب",
                            alignment: "center",
                            width: 110
                        },
                        {
                            dataField: "InsertionDate",
                            caption: "تاريخ الطلب",
                            alignment: "center",
                            //dataType: "date",
                            //width: 150
                        },
                        {
                            dataField: "ApprovedStatus",
                            caption: "حالة الاعتماد",
                            alignment: "center",
                            //width: 175
                        },
                        {
                            dataField: "ApprovedValue",
                            caption: "المبلغ المعتمد",
                            alignment: "center",
                            width: 110
                        },
                        {
                            dataField: "AdvanceIsPaid",
                            caption: "حالة الصرف",
                            alignment: "center"
                            //width: 175
                        },
                        {
                            //width: 110,
                            caption: "التفاصيل",
                            cssClass: "text-center dark-text",
                            cellTemplate: function (container, options) {
                                $("<div />").dxButton({
                                    text: '',
                                    type: 'default',
                                    icon: "fa fa-address-card-o dark-text",//"fa fa-binoculars"
                                    width: 60,
                                    disabled: !$scope.Permissions.CheckAdvence,
                                    hint: "معاينة",
                                    onClick: function (e) {
                                      if ($scope.Permissions.CheckAdvence) {
                                            debugger;
                                            $http({
                                                method: "Get",
                                                url: "/Advances/ExcludeAdvanceOrdersForStudentResons",
                                                params: {
                                                    studentId: $scope.MDL_StudentId,
                                                    AdvanceId: options.data.AdvanceSettingId
                                                }
                                            }).then(function (data) {
                                                debugger;
                                                $scope.studentExcludedDetalie = data.data.Item2;
                                            });

                                            $scope._advanceRequestId = options.data.AdvanceRequestId;
                                            debugger;
                                            return $http({
                                                method: "Get",
                                                url: "/Advances/StudentAdvanceDetailDataSource",
                                                params: {
                                                    advanceRequestId: options.data.AdvanceRequestId
                                                }
                                            }).then(function (data) {
                                                debugger;
                                                $scope.StudentAdvanceDetailsArray = data.data;

                                                $scope.ContentTemplateForDetailsPopup = "AdvanceDetailsPopupContent";
                                                $scope.AdvanceDetailsPopupVisible = true;
                                                $('body').css('overflow', 'hidden');

                                                $scope.Refresh();
                                            });
                                        }
                                        else {
                                            return swal("تنبيه", "عفوا ليس لديك صلاحية علي هذه العملية", "warning");

                                        }

                                    }
                                }).appendTo(container);
                            }
                        },{                                                
                            caption: "حذف",
                            //width: 60,
                            cssClass: "text-center",
                            cellTemplate: function (container, options) {
                                if (options.data.IsApprovedPhase === false) {
                                    $("<div />").dxButton({
                                        icon: "fa fa-trash-o",
                                        //text: "حذف",
                                        width: 60,
                                        type: "danger",
                                        hint: "حذف",
                                        elementAttr: { "class": "btn btn-danger btn-sm" },
                                        onClick: function (e) {
                                            var result = DevExpress.ui.dialog.confirm("<i>هل انت متاكد؟</i>", "تاكيد الحذف");
                                            result.done(function (dialogResult) {
                                                if (dialogResult) {
                                                    StudentAdvanceSrvc.DeleteAdvanceRequests(options.data.AdvanceRequestId).then(function (data) {
                                                        if (data.data.status === 500) {
                                                            DevExpress.ui.notify({ message: data.data.Message, type: data.data.Type, displayTime: 3000, closeOnClick: true });
                                                        }
                                                        if (data.data.status === 200) {
                                                            DevExpress.ui.notify({ message: data.data.Message, type: data.data.Type, displayTime: 3000, closeOnClick: true });
                                                            //Referesh Grid
                                                            AdvancesGriddataSource.reload();
                                                            $scope.AdvancesGridInstance.refresh();
                                                            SubsidiesGriddataSource.reload();
                                                            $scope.SubsidiesGridInstance.refresh();

                                                        }
                                                    });
                                                }
                                            });
                                        }
                                    }).appendTo(container);
                                }
                            }
                        }
                    ]
                };



                //==================================
                // الخاصة بعرض تفاصيل كل طلب Popup
                //==================================
                $scope.AdvanceDetailsPopup = {
                    bindingOptions: {
                        visible: "AdvanceDetailsPopupVisible",
                        contentTemplate: "ContentTemplateForDetailsPopup"
                    },
                    shadingColor: "rgba(0, 0, 0, 0.69)",
                    closeOnOutsideClick: false,
                    height: 700,
                    width: 1600,
                    showTitle: false,
                    title: "",
                    dragEnabled: true,
                    onHiding: function () {
                        debugger;
                        $scope.StudentAdvanceDetailsArray = null;
                        $('body').css('overflow', 'auto');
                        //$scope.Refresh();
                    }
                };

                $scope.BtnClose = {
                    text: 'رجوع',
                    type: "default",
                    width: "150",
                    icon: "fa fa-arrow-left",
                    onClick: function (e) {
                        debugger;
                        $scope.AdvanceDetailsPopupVisible = false;
                    }
                };
                //======================================================================
                // الجدول الخاص ببيانات الطالب الأساسية الموجودة بداخل تفاصيل كل طلب 
                //======================================================================
                var StudentBasicDataGridDataSource = new DevExpress.data.DataSource({
                    paginate: true,
                    loadMode: "raw",
                    cacheRawData: false,
                    load: function () {
                        debugger;
                        return $scope.StudentBasicDataArray;
                    }
                });
                $scope.StudentBasicDataGrid = function () {
                    debugger;
                    return {
                        dataSource: StudentBasicDataGridDataSource,
                        "export": {
                            enabled: true,
                            fileName: "StudentBasicData"
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
                        onSelectionChanged: function (selectedItems) {
                            // $scope.selectedRow = selectedItems.selectedRowsData[0];
                        },
                        onInitialized: function (e) {
                            $scope.StudentBasicDataGridInstance = e.component;
                        },
                        columns: [{
                            dataField: "StudentBasicData.StudentName",
                            caption: "إسم الطالب",
                            alignment: "center"
                        },
                        {
                            dataField: "StudentBasicData.FacultiyName",
                            caption: "الكلية",
                            alignment: "center",
                            width: 180
                        }, {
                            dataField: "StudentBasicData.DegreeName",
                            caption: "الدرجة العلمية",
                            alignment: "center",
                            width: 135
                        }, {
                            dataField: "StudentBasicData.LevelName",
                            caption: "المستوي",
                            alignment: "center",
                            width: 120
                        }, {
                            dataField: "StudentBasicData.StudyTypeName",
                            caption: "نوع الدراسة",
                            alignment: "center",
                            width: 120
                        },
                        {
                            dataField: "StudentBasicData.StatusName",
                            caption: "الحالة",
                            alignment: "center",
                            width: 110
                        },
                        {
                            dataField: "StudentAverageDegree",
                            caption: "GPA",
                            alignment: "center",
                            width: 100
                        }]
                    };
                };



                //============================================================
                // الجدول الخاص بتفاصيل الطلب الموجودة بداخل تفاصيل كل طلب 
                //============================================================
                var StudentAdvanceRequestGridDataSource = new DevExpress.data.DataSource({
                    paginate: true,
                    loadMode: "raw",
                    cacheRawData: false,
                    load: function () {
                        debugger;
                        return $scope.StudentAdvanceDetailsArray[0].studentAdvanceRequest;
                    }
                });
                $scope.StudentAdvanceRequestGrid = function () {
                    debugger;
                    return {
                        dataSource: StudentAdvanceRequestGridDataSource,
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
                        onSelectionChanged: function (selectedItems) {
                            // $scope.selectedEmployee = selectedItems.selectedRowsData[0];
                        },
                        onCellPrepared: (e) => {
                            if (e.rowType === 'data') {
                                debugger;
                                e.cellElement.css({ 'background-color': 'rgba(10, 77, 110, 0.03)', color: '#000000' });
                                e.cellElement.css({ 'font-weight': 'bold' });

                                if (e.column.dataField === "approvedStatus") {
                                    if (e.value === "تحت المراجعة") {
                                        e.cellElement.css({ color: '#c7bb27' });

                                    }
                                    if (e.value === "معتمد") {
                                        e.cellElement.css({ color: '#5cb85c' });

                                    }
                                    if (e.value === "مرفوض") {
                                        e.cellElement.css({ color: '#d9534f' });
                                    }
                                }
                            }
                        },
                        onInitialized: function (e) {
                            $scope.StudentAdvanceDetailsGridInstance = e.component;
                        },
                        columns: [
                            {
                                dataField: "AdvanceName",
                                caption: "النوع",
                                alignment: "center",
                                width: 160
                            },
                            {
                                dataField: "RequestedValue",
                                caption: "المبلغ المطلوب",
                                alignment: "center",
                                width: 140
                            },
                            {
                                dataField: "RequestedDate",
                                caption: "تاريخ الطلب",
                                alignment: "center",
                                dataType: "date",
                                width: 150
                            },
                            {
                                dataField: "ApprovedValue",
                                caption: "المبلغ المعتمد",
                                alignment: "center",
                                width: 175
                            },
                            {
                                dataField: "RequestNotes",
                                caption: "الملاحظـات",
                                alignment: "center"
                            },
                            {
                                dataField: "approvedStatus",
                                caption: "حالة الاعتماد",
                                alignment: "center"
                                //width: 175
                            },
                            {
                                caption: "المرفقات",
                                cssClass: "text-center",
                               // visible: $scope.ContentTemplateForDetailsPopup === "SubsidyDetailsPopupContent" ? true : false,
                                width: 100,
                                cellTemplate: function (container, options) {
                                    debugger;
                                    $("<div />").dxButton({
                                        text: '',
                                        hint: "تحميل",
                                        type: 'success',
                                        icon: 'fa fa-download',
                                        useSubmitBehavior: false,
                                        onClick: function (e) {
                                            debugger;
                                            return window.open('/Advances/DownloadAdvanceAttachment?advanceRequestId=' + $scope.StudentAdvanceDetailsArray[0].advanceRequestId, '_blank');
                                        }
                                    }).appendTo(container);
                                }
                            }
                        ]
                    };
                };



                //==================================================================
                // الجدول الخاص بتفاصيل سداد السلفة الموجودة بداخل تفاصيل كل طلب 
                //==================================================================
                var StudentAdvancePremiumsGridDataSource = new DevExpress.data.DataSource({
                    paginate: true,
                    loadMode: "raw",
                    cacheRawData: false,
                    load: function () {
                        debugger;
                        return $scope.StudentAdvanceDetailsArray[0].advancePremiums;
                    }
                });
                $scope.StudentAdvancePremiumsGrid = function () {
                    return {
                        dataSource: StudentAdvancePremiumsGridDataSource,
                        "export": {
                            enabled: true,
                            fileName: "AdvancePremiums"
                        },
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
                            showNavigationButtons: false,
                            showPageSizeSelector: false,
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
                            debugger;
                            $scope.StudentAdvancePremiumsGridInstance = e.component;
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
                                dataField: "PaidAmount",
                                caption: "القسط المسدد",
                                alignment: "center",
                                width: 120
                            },
                            {
                                dataField: "PaymentType",
                                caption: "طريقة السداد",
                                alignment: "center"
                            },
                            {
                                dataField: "PayRollNumber",
                                caption: "رقم المسير",
                                alignment: "center",
                                width: 120
                            },
                            {
                                dataField: "PayrollDate",
                                caption: "تاريخ السداد",
                                alignment: "center",
                                dataType: "date"
                            }
                        ]
                    };
                };



                //=================================================================
                // الجدول الخاص بتوضيح حالة الإعتماد الموجود بداخل تفاصيل كل طلب 
                //==================================================================
                var AdvanceRequestPhasesGridDataSource = new DevExpress.data.DataSource({
                    paginate: true,
                    loadMode: "raw",
                    cacheRawData: false,
                    load: function () {
                        debugger;
                        return $scope.StudentAdvanceDetailsArray[0].advanceApprovedPhases;
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
                        pager: {
                            allowedPageSizes: "auto",
                            infoText: "(صفحة  {0} من {1} ({2} عنصر",
                            showInfo: true,
                            showNavigationButtons: false,
                            showPageSizeSelector: false,
                            visible: false
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
                            debugger;
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
                            dataField: "Name",
                            caption: "المسوؤل",
                            alignment: "center",
                            //width: 100
                        },
                        {
                            dataField: "Reason",
                            caption: "ملاحظات الإعتماد",
                            alignment: "center",
                            visible: $scope.ContentTemplateForDetailsPopup === "SubsidyDetailsPopupContent" ? true : false
                        },
                        {
                            dataField: "ResponseDate",
                            caption: "تاريخ الإعتماد",
                            alignment: "center",
                            dataType: "date",
                            //visible: $scope.ContentTemplateForDetailsPopup === "SubsidyDetailsPopupContent" ? true : false
                        }]
                    };
                };



                //======================
                // إضافة طلب سلفة جديد
                //======================
                $scope.disabledSaveButtonOfAddNewAdvance = true;
                $scope.MDL_AdvanceValuePlaceHolder = "أدخل مبلغ السلفة المطلوب";
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
                                debugger;
                                //if (e.component._dataSource._items.find(x => x.Value === e.value).IsConditional) {

                                //    $scope.disabledSaveButtonOfAddNewAdvance = true;
                                //    $http({
                                //        method: "GET",
                                //        url: "/Advances/IsDueAdvance/",
                                //        params: {
                                //            studentId: $scope.MDL_StudentId
                                //        }
                                //    }).then(function (data) {
                                //        debugger;
                                //        if (data.data !== "") {
                                //            $scope.MDL_AdvanceTypeId = '';
                                //            $scope.MDL_AdvanceMaxValue = '';
                                //            $scope.MDL_AdvanceValuePlaceHolder = "أدخل مبلغ السلفة المطلوب";

                                //            return swal("تنبيه", data.data, "warning");
                                //        } else {
                                //            debugger;
                                //            $scope.disabledSaveButtonOfAddNewAdvance = false;

                                //            $scope.MDL_AdvanceTypeId = e.value;
                                //            $scope.MDL_AdvanceMaxValue = e.component._dataSource._items.find(x => x.Value === e.value).MaxRequestValue;
                                //            $scope.MDL_AdvanceValuePlaceHolder = "الحد الأقصي المسموح به لقيمة الطلب" + " " + $scope.MDL_AdvanceMaxValue;
                                //        }
                                //    });
                                //} else {
                                    debugger;
                                    $scope.disabledSaveButtonOfAddNewAdvance = false;
                                    $scope.disabledFileUploadButton = false;
                                    $scope.MDL_AdvanceTypeId = e.value;
                                    $scope.MDL_AdvanceMaxValue = e.component._dataSource._items.find(x => x.Value === e.value).MaxRequestValue;
                                    $scope.MDL_AdvanceValuePlaceHolder = "الحد الأقصي المسموح به لقيمة الطلب" + " " + $scope.MDL_AdvanceMaxValue;
                               // }
                            } else {
                                debugger;
                                $scope.MDL_AdvanceMaxValue = '';
                                $scope.disabledSaveButtonOfAddNewAdvance = true;
                                $scope.disabledFileUploadButton = true;
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
                                debugger;
                                if ($scope.MDL_AdvanceValue > 0 && ($scope.MDL_AdvanceTypeId === '' || $scope.MDL_AdvanceTypeId === null || $scope.MDL_AdvanceTypeId === undefined)) {
                                    swal("تنبيه", "لابد من اختيار نوع السلفة أولا", "warning");
                                    $scope.MDL_AdvanceValue = '';
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
                        type: "success",
                        validationGroup: "addAdvance",
                        useSubmitBehavior: true,
                        bindingOptions: { disabled: "disabledSaveButtonOfAddNewAdvance" },
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


                                    //$http({
                                    //    method: "Get",
                                    //    url: "/Advances/ExcludeAdvanceOrdersForStudentResons",
                                    //    params: {
                                    //        studentId: $scope.MDL_StudentId,
                                    //        AdvanceId: $scope.MDL_AdvanceTypeId
                                    //    }
                                    //}).then(function (data) {
                                    //    debugger;

                                    //    if (data.data.Item1 === true) {
                                    //        $scope.studentExcludedDetalie = data.data.Item2;
                                    //        $scope.PopupStudentExcludedResonShow = true;
                                    //        $scope.AddNewPopUpShow = false;
                                    //        return false;
                                    //    }
                                    //    else {
                                            $http({
                                                method: "POST",
                                                url: "/Advances/SaveAdvanceRequestByAdmin/",
                                                data: {
                                                    studentId: $scope.MDL_StudentId,
                                                    advanceTypeId: $scope.MDL_AdvanceTypeId,
                                                    advanceValue: $scope.MDL_AdvanceValue,
                                                    advanceRequestNotes: $scope.MDL_AdvanceRequestNotes
                                                }
                                            }).then(function (data) {
                                                if (data.data === "") {

                                                    $scope.ResetControls();

                                                    //validationGroup.reset();

                                                    //مش ليهم لازمة لاننا هنفضي كل الداتا بعض عملية الحفظ
                                                    //AdvancesGriddataSource.reload();
                                                    //SubsidiesGriddataSource.reload();
                                                    //$scope.AdvancesGridInstance.refresh();
                                                    //$scope.SubsidiesGridInstance.refresh();

                                                    swal("Done!", "تم الحفظ بنجاح", "success");
                                                    $scope.AddNewPopUpShow = false;
                                                    $scope.MDL_StudentId = null;
                                                    return;

                                                } else {
                                                    if (data.data !== "") { swal("حدث خطأ", data.data, "error"); }
                                                }
                                        //    });
                                        //}

                                    });
                                }
                            }
                        }
                    }
                };



                //==================================================
                // الخاصة بإضافة الطلبات سواء سلف أو إعانات Popup
                //==================================================
                $scope.AddNewPopupOptions = {
                    showTitle: true,
                    dragEnabled: false,
                    shadingColor: "rgba(0, 0, 0, 0.69)",
                    closeOnOutsideClick: false,
                    bindingOptions: {
                        visible: "AddNewPopUpShow",
                        contentTemplate: "PopupContent",
                        title: "PopupTitle",
                        width: "PopUpwidth",
                        height: "PopUpheight"
                    },
                    rtlEnabled: true,
                    onShowing: function () {
                        debugger;
                        $scope.ResetControls();
                    }
                };



                //======================
                // إضافة طلب إعانة جديد
                //======================
                $scope.MDL_SubsidyValuePlaceHolder = "أدخل مبلغ الإعانة المطلوب";
                $scope.Subsidy = {
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
                            debugger;
                            if ($scope.MDL_AdvanceValue > 0) {
                                $scope.MDL_AdvanceValue = '';
                            }

                            if (e.value !== '' && e.value !== null && e.value !== undefined) {
                                debugger;
                                $scope.MDL_AdvanceTypeId = e.value;
                                $scope.MDL_AdvanceMaxValue = e.component._dataSource._items.find(x => x.Value === e.value).MaxRequestValue;
                                $scope.MDL_SubsidyValuePlaceHolder = "الحد الأقصي المسموح به لقيمة الطلب" + " " + $scope.MDL_AdvanceMaxValue;
                            }
                            else {
                                $scope.MDL_AdvanceMaxValue = '';
                                $scope.MDL_SubsidyValuePlaceHolder = "أدخل مبلغ الإعانة المطلوب";
                            }
                        }
                    },
                    AdvanceValue:
                    {
                        bindingOptions: { value: "MDL_AdvanceValue", placeholder: "MDL_SubsidyValuePlaceHolder" },
                        showSpinButtons: true,
                        showClearButton: true,
                        rtlEnabled: true,
                        min: 1,
                        onValueChanged: function (e) {
                            debugger;
                            if (e.value > $scope.MDL_AdvanceMaxValue) {
                                swal("تنبيه", $scope.MDL_SubsidyValuePlaceHolder, "warning");
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
                                        url: "/Advances/SaveAdvanceRequestByAdmin/",
                                        data: {
                                            studentId: $scope.MDL_StudentId,
                                            advanceValue: $scope.MDL_AdvanceValue,
                                            advanceTypeId: $scope.MDL_AdvanceTypeId,
                                            advanceRequestNotes: $scope.MDL_AdvanceRequestNotes
                                        }
                                    }).then(function (data) {
                                        if (data.data === "") {
                                            $scope.ResetControls();

                                            //validationGroup.reset();
                                            //SubsidiesGriddataSource.reload();

                                            swal("Done!", "تم الحفظ بنجاح", "success");
                                            $scope.AddNewPopUpShow = false;
                                            $scope.MDL_StudentId = null;
                                            return;

                                        } else {
                                            if (data.data !== "") { swal("حدث خطأ", data.data, "error"); }
                                        }
                                    });
                                }
                            }
                        }
                    }
                };



                //==============
                // Button Upload
                //==============
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



                //=====================
                // File Uploading Popup
                //=====================
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



                //===============
                // File Uploading
                //===============
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
                        value: "UploadingFilesvalue"
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
                        }

                    }
                };



                //=====================
                // Remove Uploaded File
                //=====================
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
                    $scope.disabledSaveButton = true;
                    $scope.disabledFileUploadButton = true;
                    if ($scope.PopupContent === "AddNewSubsidyContent") {

                        $scope.btnUploadIcon = "upload";
                        $scope.btnUploadText = "رفع المرفق";

                        $scope.MDL_UploadingFilesvalue = null;
                        debugger;
                        if ($scope.FileUploadingOptionsInstance) {
                            $scope.FileUploadingOptionsInstance.reset();
                        }
                    }
                };
/********************************************************************* Upload advances file ***********************************************************************/
                $scope.btnUploadIcon1 = "upload";
                $scope.btnUploadText1 = "رفع المرفق ";
                $scope.btnUploadOptions1 = {
                    bindingOptions: {
                        text: "btnUploadText1",
                        icon: 'btnUploadIcon1',
                        disabled:"disabledFileUploadButton"
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



                //===============
                // File Uploading
                //===============
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

                $scope.ResetControls = function () {

                    $scope.MDL_AdvanceValue = null;
                    $scope.MDL_AdvanceTypeId = null;
                    $scope.MDL_AdvanceRequestNotes = null;
                    $scope.disabledSaveButton = true;
                    $scope.disabledFileUploadButton = true;
                    if ($scope.PopupContent === "AddNewSubsidyContent") {

                        $scope.btnUploadIcon = "upload";
                        $scope.btnUploadText = "رفع المرفق";

                        $scope.MDL_UploadingFilesvalue = null;
                        debugger;
                        if ($scope.FileUploadingOptionsInstance) {
                            $scope.FileUploadingOptionsInstance.reset();
                        }
                    }
                    if ($scope.PopupContent === "AddNewAdvanceContent") {

                        $scope.btnUploadIcon1 = "upload";
                        $scope.btnUploadText1 = "رفع المرفق";

                        $scope.MDL_UploadingFilesvalue1 = null;
                        debugger;
                        if ($scope.FileUploadingOptionsInstance1) {
                            $scope.FileUploadingOptionsInstance1.reset();
                        }
                    }
                };
                //==============================================
                // عمل تحديث للقوائم الخاصة بعرض تفاصيل الطلب  
                //==============================================
                $scope.Refresh = function () {
                    debugger;
                    StudentBasicDataGridDataSource.reload();
                    AdvanceRequestPhasesGridDataSource.reload();
                    StudentAdvanceRequestGridDataSource.reload();
                    StudentAdvancePremiumsGridDataSource.reload();
                };


                //==================================================
                // التحقق من سلف الطالب السابقة والشروط Popup
                //==================================================
                $scope.PopupStudentExcludedReson = {
                    showTitle: true,
                    dragEnabled: false,
                    shadingColor: "rgba(0, 0, 0, 0.69)",
                    closeOnOutsideClick: false,
                    bindingOptions: {
                        visible: "PopupStudentExcludedResonShow"
                    },
                    title: "الطالب لا يستحق السلفة للشروط التالية :-",
                    width: 600,
                    height: 300,
                    rtlEnabled: true,
                    onHiding: function () {
                        $scope.ResetControls();
                    }
                };


                $scope.AcceptButton = {
                    text: 'استثناء وحفظ',
                    type: "success",
                    width: "150",
                    icon: "fa fa-thumbs-up",
                    onClick: function (e) {

                        $http({
                            method: "POST",
                            url: "/Advances/SaveAdvanceRequestByAdmin/",
                            data: {
                                studentId: $scope.MDL_StudentId,
                                advanceTypeId: $scope.MDL_AdvanceTypeId,
                                advanceValue: $scope.MDL_AdvanceValue,
                                advanceRequestNotes: $scope.MDL_AdvanceRequestNotes
                            }
                        }).then(function (data) {
                            if (data.data === "") {
                                $scope.PopupStudentExcludedResonShow = false;
                                $scope.ResetControls();

                                //AdvancesGriddataSource.reload();
                                //$scope.AdvancesGridInstance.refresh();
                                debugger;
                                swal("Done!", "تم الحفظ بنجاح", "success");
                                $scope.MDL_StudentId = null;
                                return;
                            } else {
                                if (data.data !== "") { swal("حدث خطأ", data.data, "error"); }
                            }
                        });
                    }
                };


                $scope.BtnClosePopup = {
                    text: 'رجوع',
                    type: "default",
                    width: "150",
                    icon: "fa fa-arrow-left",
                    onClick: function (e) {
                        $scope.PopupStudentExcludedResonShow = false;
                    }
                };
            }




        ]);

})();






















//===============
// Operation Menu 
//===============
//const MenuDataSource = [
//    { key: "StudentSubsidyRequests", text: "طلب إعانة" },
//    { key: "StudentAdvanceRequests", text: "طلب سلفة" }
//];
//$scope.MenuOptions = {
//    dataSource: MenuDataSource,
//    itemTemplate: function (data) {
//        return $("<div><i class='icon dx-icon-plus'></i><span>" + data.text + "</span></div>");
//    },
//    onItemClick: function (e) {
//        $scope.selectedItem = e.itemData.key;

//        if ($scope.selectedItem === "StudentAdvanceRequests") {
//            $scope.PopUpwidth = 1000;
//            $scope.PopUpheight = 450;
//            $scope.PopupTitle = "إضافة طلب سلفة جديد";
//            $scope.PopupContent = "AddNewAdvanceContent";
//        } else {
//            $scope.PopUpwidth = 500;
//            $scope.PopUpheight = 450;
//            $scope.PopupTitle = "إضافة طلب إعانة جديد";
//            $scope.PopupContent = "AddNewSubsidyContent";
//        }
//        $scope.AddNewPopUpShow = true;
//    }
//};