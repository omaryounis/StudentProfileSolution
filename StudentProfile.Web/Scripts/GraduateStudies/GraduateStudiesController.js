app.controller("GraduateStudiesCtrl", ["$scope", 'GraduateStudiesSrvc', '$http' , 'HelperServices', function ($scope, GraduateStudiesSrvc, $http, HelperServices) {

    //DDL
    $scope.GraduateStudiesList = [];
    $scope.DetailWarningList = [];

    $scope.FacultionList = [];
    $scope.FacultionID = null;
    $scope.FacultionIds = null;

    $scope.DegreeList = [];
    $scope.DegreeID = null;
    $scope.DegreeIds = null;

    $scope.PopupWarningShow = false;


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
            GraduateStudiesSrvc.GetFaculties().then(function (data) {
                $scope.FacultionList = data.data;
            });
        },
        onValueChanged: function (e) {
            $scope.FacultionIds = e.value.join(',');
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
        onInitialized: function (e) {
            GraduateStudiesSrvc.GetDegrees().then(function (data) {
                $scope.DegreeList = data.data;
            });
        },
        onValueChanged: function (e) {
            $scope.DegreeIds = e.value.join(',');
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

    $scope.btnUpdateStudentsData = {
        text: 'تحديث بيانات الطلاب',
        type: 'primary',
        onClick: function (e) {
            GraduateStudiesSrvc.UpdateStudentsData().then(function (data) {
                if (data.data) 
                    DevExpress.ui.notify({ message: data.data.Message, type: data.data.Type, displayTime: 3000, closeOnClick: true });
            });
        }
    };
    //dataGrid
    $scope.gridGraduateStudies = {
        bindingOptions: {
            dataSource: "GraduateStudiesList"
        },
        noDataText: "لا يوجد بيانات",
        selection: {
            mode: "single"
        },
        showBorders: true,
        paging: {
            pageSize: 10
        },

        pager: {
            allowedPageSizes: [5, 10, 20]
        },
        "export": {
            enabled: true,
            fileName: "قائمة طلاب الدراسات العليا"
        },
        searchPanel: {
            visible: true,
            placeholder: "بحث",
            width: 300
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
            visible: true,
            allowSearch: true
        },
        showRowLines: false,
        groupPanel: {
            visible: true,
            emptyPanelText: "اسحب عمود هنا"
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
                caption: "تاريخ الميلاد",
                dataField: "BIRTH_DATE",
                cssClass: "text-right",
                dataType: 'date'
            },
            {
                caption: "تاريخ الالتحاق",
                width: "120px",
                dataField: "JOIN_DATE",
                cssClass: "text-right",
                dataType: 'date',
            },
            {
                caption: "نوع الدراسة",
                dataField: "STUDY_DESC",
                cssClass: "text-right"
            },
            {
                caption: "القسم",
                dataField: "DEPT_NAME",
                cssClass: "text-right"
            },
            {
                caption: "الكلية",
                dataField: "FACULTY_NAME",
                cssClass: "text-right"
            },
            {
                caption: "الدرجة العلمية",
                dataField: "DEGREE_DESC",
                cssClass: "text-right"
            },
            {
                caption: "التخصص",
                dataField: "MAJOR_NAME",
                cssClass: "text-right"
            },
            {
                caption: "الحالة",
                dataField: "STATUS_DESC",
                cssClass: "text-right"
            },
            {
                caption: "الساعات المتبقية",
                dataField: "REMAININGCREDITHOURSCOUNT",
                cssClass: "text-right"
            },
            {
                caption: "الجنسية",
                dataField: "NATIONALITY_DESC",
                cssClass: "text-right"
            },
            {
                caption: "فصل الالتحاق",
                dataField: "JOIN_SEMESTER",
                cssClass: "text-right"
            },
            {
                caption: "تاريخ حاله",
                dataField: "GRADUATE_DATE",
                cssClass: "text-right"
            },
            {
                caption: "رقم التخرج",
                dataField: "DEPT_CODE",
                cssClass: "text-right"
            },
            {
                caption: "تاريخ الرسالة",
                dataField: "MessageDate",
                cssClass: "text-right"
            },
            {
                caption: "حالة الرسالة",
                dataField: "MessageStauts",
                cssClass: "text-right"
            },
            {
                caption: "عدد الانذارات",
                dataField: "TOTAL_WARNINGS",
                cssClass: "text-right"
            },
            {
                caption: "موضوع الرساله",
                dataField: "THESIS_LABEL",
                cssClass: "text-right"
            },
            {
                caption: "تاريخ اعتماد الخطه",
                dataField: "DEAN_MEETING_DATE",
                cssClass: "text-right"
            },
            {
                caption: "مسئول اعنماد الخطه",
                dataField: "",
                cssClass: "text-right"
            },

            {
                caption: "تاريخ استلام الرساله",
                dataField: "DELIVERY_DATE",
                cssClass: "text-right"
            },
            {
                caption: "مسئول الاستلام",
                dataField: "",
                cssClass: "text-right"
            },
            {
                caption: "تاريخ تشكيل لجنه",
                dataField: "FORMATION_DATE",
                cssClass: "text-right"
            },
            {
                caption: "مسئول تشكيل اللجنه",
                dataField: "",
                cssClass: "text-right"
            },
            {
                caption: "تاريخ المناقشه",
                dataField: "PRESENTATION_DATE",
                cssClass: "text-right"
            },
            {
                caption: "مسئول المناقشه",
                dataField: "",
                cssClass: "text-right"
            },
            {
                caption: "المده النظاميه",
                dataField: "TOTAL_OFFICIAL_SEMESTER",
                cssClass: "text-right" 
            },
            {
                caption: "المشرف على الرساله",
                dataField: "SUPERVISOR_NAME",
                cssClass: "text-right"
            },
            {
                caption: "الانذارات",
                width: 200,
                cssClass: "text-center",
                cellTemplate: function (container, options) {
                    $("<div />").dxButton({
                        //icon: "fa fa-save",
                        text: "الانذارات",
                        type: "primary",
                        hint: "الانذارات",
                        elementAttr: { "class": "btn btn-primary" },

                        onClick: function (e) {
                            debugger;
                            GraduateStudiesSrvc.GetStudentWarning(options.data.STUDENT_ID).then(function (data) {
                                if (data.data) {
                                    $scope.DetailWarningList = data.data;
                                    $scope.PopupWarningShow = true;
                                }
                            });
                        }
                    }).appendTo(container);
                }

            }
        ],
        onContentReady: function (e) {
            e.component.columnOption("command:edit",
                {
                    visibleIndex: -1
                });
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


    $scope.btnPrint = {
        icon: "print",
        text: "طباعة",
        onClick: function () {
            HelperServices.Print("تفاصيل الانذارات", "DetailWarning");
        }
    };


    $scope.btnExportToExcel = {
        icon: "fa fa-file-excel-o",
        text: "تصدير Excel",
        onClick: function () {
            //HelperServices.ExportToExcel("DetailWarning");
        }
    };

    $scope.ViewGraduateStudiesStudents = function () {
        GraduateStudiesSrvc.GetStudentsForGraduateStudies({ facultyIds: $scope.FacultionIds, degreeIds: $scope.DegreeIds }).then(function (data) {
            $scope.GraduateStudiesList = data.data;
        });
    };

}]);
