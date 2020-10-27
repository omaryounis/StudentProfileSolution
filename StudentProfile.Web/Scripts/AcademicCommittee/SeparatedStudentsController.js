app.controller("SeparatedStudentsCtrl", ["$scope", 'SeparatedStudentsSrvc', 'HelperServices', function ($scope, SeparatedStudentsSrvc, HelperServices) {
    //DDL
    $scope.SeparatedStudentsList = [];

    $scope.FacultionList = [];
    $scope.FacultionID = null;
    $scope.FacultionIds = null;

    $scope.DegreeList = [];
    $scope.DegreeID = null;
    $scope.DegreeIds = null;

    $scope.StudentList = [];
    var StudentList = [];
    $scope.StudentId = null;
    $scope.StudentIds = null;
    $scope.showchance = false;
    $scope.showtrans = false;
    $scope.showwarning = false;
    $scope.gridSelectedRowsData = [];


    $scope.DetailWarningList = [];
    $scope.PopupWarningShow = false;

    $scope.DetailExtraChancesList = [];
    $scope.PopupExtraChancesShow = false;

    $scope.PopupDetailsShow = false;

    $scope.DetailTransList = [];
    $scope.PopupTransShow = false;


    $scope.btnUpdateStudentsData = {
        text: 'تحديث بيانات الطلاب',
        type: 'primary',
        onClick: function (e) {
            SeparatedStudentsSrvc.UpdateStudentsData().then(function (data) {
                if (data.data)
                    DevExpress.ui.notify({ message: data.data.Message, type: data.data.Type, displayTime: 3000, closeOnClick: true });
            });
        }
    };

    //Control
    $scope.FacultionSelectBox = {
        bindingOptions: {
            value: "FacultionID",
            items: "FacultionList"
        },

        placeholder: "--أختر--",
        noDataText: "لا يوجد بيانات",
        displayExpr: "Text",
        valueExpr: "Value",
        paginate: true, //Pagenation
        showBorders: true,
        searchEnabled: true,
        showSelectionControls: true,
        maxDisplayedTags: 2,
        showMultiTagOnly: false,

        onInitialized: function (e) {
            SeparatedStudentsSrvc.GetFaculties().then(function (data) {
                $scope.FacultionList = data.data;
            });
        },
        onValueChanged: function (e) {
            debugger;
            if (e != undefined && e != null && e != '') {
                if (e.value != null) {
                    if (e.value.length != 0) {
                        $scope.FacultionIds = e.value.join(',');

                        $scope.DegreeList = '';
                        SeparatedStudentsSrvc.GetDegrees($scope.FacultionIds).then(function (data) {
                            debugger;
                            $scope.DegreeList = data.data;
                        });
                    }
                }

            }
        }
    };

    $scope.DegreeSelectBox = {
        bindingOptions: {
            value: "DegreeID",
            items: "DegreeList"
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
            $scope.DegreeIds = e.value.join(',');
        },
        onInitialized: function (e) {
            debugger;
            if ($scope.FacultionIds != null) {
                SeparatedStudentsSrvc.GetDegrees($scope.FacultionIds).then(function (data) {
                    debugger;
                    $scope.DegreeList = data.data;
                });
            }
        },
        onClick: function (e) {
            debugger;
            if ($scope.FacultionIds != null) {
                SeparatedStudentsSrvc.GetDegrees($scope.FacultionIds).then(function (data) {
                    debugger;
                    $scope.DegreeList = data.data;
                });
            }
        },
        onClosed: function (e) {
            debugger;
            $scope.DegreeIds = e.model.DegreeID.join(',');
        }
    };

    //validation 
    $scope.validationRequired = {
        validationRules: [
            {
                type: "required",
                message: "الحقل مطلوب"
            }
        ]
    };

    //btn View
    $scope.btnView = {
        text: 'عرض',
        type: 'success',
        useSubmitBehavior: true
    };

    //var DetailTransList = '';
    //var dataSourceStudentsDetailTrans = new DevExpress.data.DataSource({
    //    paginate: true,
    //    cacheRawData: false,
    //    loadMode: "raw",
    //    load: function () {
    //        if ($scope.FacultionIds !== null && $scope.DegreeIds !== null) {
    //            return $.getJSON("/AcademicCommittee/GetStudentsSemesterTrans", function (data) { debugger; DetailTransList = data });
    //        }
    //    }
    //});


    //var DetailExtraChanceList = '';
    //var dataSourceStudentsDetailExtraChance = new DevExpress.data.DataSource({
    //    paginate: true,
    //    cacheRawData: false,
    //    loadMode: "raw",
    //    load: function () {
    //        if ($scope.FacultionIds !== null && $scope.DegreeIds !== null) {
    //            return $.getJSON("/AcademicCommittee/GetStudentsChances", function (data) {debugger;  DetailExtraChanceList = data });
    //        }
    //    }
    //});

    //dataGrid
    $scope.gridSeparatedStudents = {
        keyExpr: "STUDENT_ID",

        bindingOptions: {
            dataSource: "SeparatedStudentsList"
        },

        showBorders: true,
        paging: {
            pageSize: 10
        },
        pager: {
            showPageSizeSelector: true,
            allowedPageSizes: [5, 10, 20],
            showInfo: true
        },
        "export": {
            enabled: true,
            fileName: "الطلاب المطوي قيدهم والمفصولين"
            //allowExportSelectedData: true
        },

        noDataText: "لا يوجد بيانات",
        selection: {
           //  fixed: true,
            mode: "multiple",
            showCheckBoxesMode: "always",
            selectAllMode: "allPages",
            width: "40px"
        },
        filterRow: {
            visible: false,
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
            scrolling: {
                rtlEnabled: true,
                useNative: true,
                scrollByContent: true,
                scrollByThumb: true,
                showScrollbar: "onHover",
                mode: "standard", // or "virtual"
                direction: "both"
            },
            resetOperationText: "الوضع الافتراضى"
        },
        headerFilter: {
            visible: true,
            allowSearch: true
        },
        showRowLines: false,
        groupPanel: {
            visible: true,
            emptyPanelText: "اسحب عمود هنا"
        },
        columnChooser: {
            allowSearch: true,
            enabled: true,
            emptyPanelText: "اسحب هنا",
            height: 500,
            mode: "dragAndDrop",
            searchTimeout: 500,
            title: "",
            width: 250
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
        rowAlternationEnabled: true,
        allowColumnReordering: true,
        allowColumnResizing: true,
        columnAutoWidth: true,
        columns: [

            
            {
                caption: "اسم الطالب",
                dataField: "STUDENT_NAME",
                cssClass: "text-right"
            },
            {
                caption: "الرقم الجامعي",
                dataField: "STUDENT_ID",
                cssClass: "text-right"
            },
            {
                caption: "رقم الهوية",
                dataField: "NATIONAL_ID",
                cssClass: "text-right"
            },
            {
                caption: "الجنسية",
                dataField: "NATIONALITY_DESC",
                cssClass: "text-right"
            },
            {
                caption: "الكلية",
                dataField: "FACULTY_NAME",
                cssClass: "text-right"
            },
            {
                caption: "المرحلة العلمية",
                dataField: "DEGREE_DESC",
                cssClass: "text-right"
            },
            {
                caption: "الفصل",
                dataField: "JOIN_SEMESTER",
                cssClass: "text-right"
            },
            {
                caption: "عدد الساعات",
                dataField: "REMAININGCREDITHOURSCOUNT",
                cssClass: "text-right"
            },
            {
                caption: "عدد الفصول",
                dataField: "TOTALSEMESTER",
                cssClass: "text-right"
            },
            {
                caption: "سبب الفصل",
                dataField: "REASON_NAME",
                cssClass: "text-right"
            },
            {
                caption: "نوع الدراسة",
                dataField: "STUDY_DESC",
                cssClass: "text-right"
            },
            {
                caption: "نوع المنحة",
                dataField: "CATEGORY_NAME",
                cssClass: "text-right"
            },
            {
                caption: "المعدل التراكمي",
                dataField: "cum_gpa",
                cssClass: "text-right"
            },
            {
                caption: "عدد الانذارات",
                dataField: "TOTAL_WARNINGS",
                cssClass: "text-right"
            },
            {
                caption: "عدد الاعتذارات",
                dataField: "TOTAL_Apology",
                cssClass: "text-right"
            },
            {
                caption: "مرات طي القيد",
                dataField: "TOTAL_Separated",
                cssClass: "text-right"
            },
            {
                caption: "حالة الطالب",
                dataField: "STATUS_DESC",
                cssClass: "text-right"
            },
            {
                caption: "المتوقع",
                dataField: "Expected",
                cssClass: "text-right"
            },

            {
                caption: "الملاحظات",
                cssClass: "text-center",
                allowHiding: false,
                fixed:true,
                width: "200px",
                cellTemplate: function (container, options) {
                    $("<div />").dxTextArea({
                        name: "Note",
                        showSpinButtons: false,
                        showClearButton: false,
                        rtlEnabled: true,
                        hint: "ملاحظات",
                        onValueChanged: function (e) {
                            if (e.value) {
                                debugger;
                                options.data.Note = e.value;
                                for (var i = 0; i < $scope.gridSelectedRowsData.length; i++) {
                                    if ($scope.gridSelectedRowsData[i].StudentId === options.data.STUDENT_ID) {
                                        $scope.gridSelectedRowsData[i].EmployeeNote = e.value;
                                    }
                                }
                            }
                        },
                        elementAttr: {
                            "class": "btn-ds-st"
                            //"id": "Note" + key
                        }
                    }).appendTo(container);
                }
            },
            {
                caption: "التفاصيل",
              fixed: true,
                width: 100,
                cssClass: "text-center",
                cellTemplate: function (container, options) {
                    $("<div />").dxButton({
                        text: "التفاصيل",
                        type: "primary",
                        hint: "التفاصيل",
                        elementAttr: { "class": "btn btn-primary" },
                        onClick: function (e) {
                            debugger;
                            $scope.$apply(function () {

                                $scope.PopupDetailsShow = true;
                                $scope.StudentId = options.data.STUDENT_ID;
                            });
                        }
                    }).appendTo(container);
                }

            }

        ],

        //masterDetail: {
        //    enabled: true,
        //    template: "detail"
        //},
        //onContentReady: function (e) {
        //    e.component.columnOption("command:edit",
        //        {
        //            visibleIndex: 99
        //        });
        //},

        onContentReady: function (e) {
            if (e.component.shouldSkipNextReady) {
                e.component.shouldSkipNextReady = false;
            }
            else {
                e.component.shouldSkipNextReady = true;
                e.component.columnOption("command:select", "visibleIndex", 99);
                e.component.updateDimensions();
            }
        },  
        onSelectionChanged: function (selectedItems) {
            debugger;
            $scope.gridSelectedRowsData = [];

            for (var i = 0; i < selectedItems.selectedRowsData.length; i++) {
                $scope.gridSelectedRowsData.push({
                    StudentId: selectedItems.selectedRowsData[i].STUDENT_ID,
                    StudentName: selectedItems.selectedRowsData[i].STUDENT_NAME,
                    EmployeeNote: selectedItems.selectedRowsData[i].Note,
                    IsAccept: null
                });
            }

        } 
    };

    $scope.btnRelayStudents = {
        text: 'ترحيل لقائمة الطلاب',
        type: 'success',
        onClick: function (e) {
            debugger;

            SeparatedStudentsSrvc.RelaySeparatedStudents($scope.gridSelectedRowsData).then(function (data) {
                if (data.data.status === 500) {
                    DevExpress.ui.notify({ message: data.data.Message, type: data.data.Type, displayTime: 3000, closeOnClick: true });
                }
                if (data.data.status === 200) {
                    DevExpress.ui.notify({ message: data.data.Message, type: data.data.Type, displayTime: 3000, closeOnClick: true });
                    //Referesh Grid
                    SeparatedStudentsSrvc.GetStudents($scope.FacultionIds, $scope.DegreeIds).then(function (data) {
                        $scope.SeparatedStudentsList = data.data;
                    });
                }
            });
        }
    };

    $scope.ViewSeparatedStudents = function () {
        SeparatedStudentsSrvc.GetStudents($scope.FacultionIds, $scope.DegreeIds).then(function (data) {
            $scope.SeparatedStudentsList = data.data;
        });
        //dataSourceStudentsDetailTrans.load();
        //dataSourceStudentsDetailExtraChance.load();
    };


    //popup
    $scope.PopupExtraChances = {
        showTitle: true,
        bindingOptions: {
            visible: "PopupExtraChancesShow"
        },
        title: "تفاصيل الفرص",
        rtlEnabled: true,
        width: 800,
        height: 700,
        onHiding: function () {

        }
    };

    $scope.PopupDetails = {
        showTitle: true,
        bindingOptions: {
            visible: "PopupDetailsShow"
        },
        title: "التفاصيل",
        rtlEnabled: true,
        width: 800,
        height: 700,
        onHiding: function () {

        }
    };
    $scope.btnPrintExtraChances = {
        icon: "print",
        text: "طباعة",
        onClick: function () {
            HelperServices.Print("تفاصيل الفرص", "DetailExtraChances");
        }
    };

    $scope.btnExtraChances = {
        icon: "الفرص",
        type: "default",
        text: "الفرص",
        onClick: function () {
            SeparatedStudentsSrvc.GetStudentsChances($scope.StudentId).then(function (data) {
                        if (data.data) {
                            debugger;
                            $scope.DetailExtraChancesList = data.data;
                            $scope.showchance = true;
                            $scope.showtrans = false;
                            $scope.showwarning = false;
                            //$scope.PopupExtraChancesShow = true;
                        }
                    });
        }
    };

    $scope.btnTrans = {
        icon: "الحركات",
        type: "success",
        text: "الحركات",
        onClick: function () {
            SeparatedStudentsSrvc.GetStudentsSemesterTrans($scope.StudentId).then(function (data) {
                    if (data.data) {
                        debugger;
                        $scope.DetailTransList = data.data;
                        $scope.showtrans = true;
                        $scope.showchance = false;
                        $scope.showwarning = false;
                       // $scope.PopupTransShow = true;
                    }
                });
        }
    };

    $scope.btnWarn = {
        icon: "الإنذارات",
        type: "danger",
        text: "الإنذارات",
        onClick: function () {
            SeparatedStudentsSrvc.GetStudentWarning($scope.StudentId).then(function (data) {
                        if (data.data) {
                            debugger;
                            $scope.DetailWarningList = data.data;
                           // $scope.PopupWarningShow = true;
                            $scope.showtrans = false;
                            $scope.showchance = false;
                            $scope.showwarning = true;
                        }
                    });
        }
    };

    $scope.PopupTrans = {
        showTitle: true,
        bindingOptions: {
            visible: "PopupTransShow"
        },
        title: "تفاصيل الحركات",
        rtlEnabled: true,
        width: 800,
        height: 700,
        onHiding: function () {

        }
    };
    $scope.btnPrintTrans = {
        icon: "print",
        text: "طباعة",
        onClick: function () {
            HelperServices.Print("تفاصيل الحركات", "DetailTrans");
        }
    };

    $scope.PopupWarning = {
        showTitle: true,
        bindingOptions: {
            visible: "PopupWarningShow"
        },
        title: "تفاصيل الانذارات",
        rtlEnabled: true,
        width: 800,
        height: 700,
        onHiding: function () {

        }
    };
    $scope.btnPrintWarning = {
        icon: "print",
        text: "طباعة",
        onClick: function () {
            HelperServices.Print("تفاصيل الانذارات", "DetailWarning");
        }
    };

}]);
