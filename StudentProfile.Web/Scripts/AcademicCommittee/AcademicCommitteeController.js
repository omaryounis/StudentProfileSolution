app.controller("AcademicCommitteeCtrl", ["$scope", 'AcademicCommitteeSrvc','SeparatedStudentsSrvc', function ($scope, AcademicCommitteeSrvc, SeparatedStudentsSrvc) {

    //DDL
    $scope.AcademicCommitteeList = [];

    $scope.gridSelectedRowsData = [];
    $scope.FacultionList = [];
    $scope.FacultionID = null;
    $scope.FacultionIds = null;

    $scope.DegreeList = [];
    $scope.DegreeID = null;
    $scope.DegreeIds = null;

    $scope.StudentList = [];
    $scope.StudentId = null;
    $scope.StudentIds = null;

    $scope.PopUpUploadDecisionsShow = false;
    $scope.DecisionNumber = null;


    $scope.DetailWarningList = [];
    $scope.PopupWarningShow = false;

    $scope.DetailExtraChancesList = [];
    $scope.PopupExtraChancesShow = false;

    $scope.DetailTransList = [];
    $scope.PopupTransShow = false;

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
            AcademicCommitteeSrvc.GetFaculties().then(function (data) {
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





    //dataGrid
    $scope.gridAcademicCommittee = {
        bindingOptions: {
            dataSource: "AcademicCommitteeList"
        },

        noDataText: "لا يوجد بيانات",
        selection: {
          //  fixed: true,
            mode: "multiple",
            showCheckBoxesMode: "always",
            selectAllMode: "allPages",
            width: "40px"
        },
        paging: {
            pageSize: 10
        },
        pager: {
            showPageSizeSelector: true,
            allowedPageSizes: [5, 10, 20],
            showInfo: true
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
        "export": {
            enabled: true,
            fileName: "الطلاب المطوي قيدهم والمفصولين"
            //allowExportSelectedData: true
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
                caption: "عددد الانذارات",
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
                caption: "اسم الموظف",
                dataField: "Employee",
                cssClass: "text-right",
                width: "200px"
            },
            {
                caption: "التاريخ",
                dataField: "EmployeeDate",
                cssClass: "text-right",
                width: "200px"
            },
            {
                caption: "ملاحظات الموظف",
                dataField: "EmployeeNote",
                cssClass: "text-right",
                width: "150px"
            },
            {
                caption: "الملاحظات",
                cssClass: "text-center",
                allowHiding: false,
                width: "150px",
                fixed: true,
                cellTemplate: function (container, options) {
                    $("<div />").dxTextArea({
                        name: "AcademicNote",
                        showSpinButtons: false,
                        showClearButton: false,
                        rtlEnabled: true,
                        hint: "ملاحظات",
                        onValueChanged: function (e) {
                            if (e.value) {
                                debugger;
                                options.data.AcademicNote = e.value;
                            }
                        }
                    }).appendTo(container);
                }
            },
            {
                caption: "التفاصيل",
                width: 100,
                //cssClass: "text-center",
                fixed: true,
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
                    id: selectedItems.selectedRowsData[i].Id,
                    Note: selectedItems.selectedRowsData[i].AcademicNote
                });
            }

        }
    };


    $scope.ViewAcademicCommitteeStudents = function () {
        AcademicCommitteeSrvc.GetStudentsForAcademicCommittee($scope.FacultionIds, $scope.DegreeIds).then(function (data) {
            $scope.AcademicCommitteeList = data.data;
        });
    }



    // UploadDecisions

    //btn ShowPopup
    $scope.btnShowPopup = {
        text: 'رفع ملف قرار اللجنة الاكاديمية',
        type: 'info',
        onClick: function (e) {
            $scope.PopUpUploadDecisionsShow = true;
            AcademicCommitteeSrvc.GetAcademicCommitteeStudents().then(function (data) {
                $scope.StudentList = data.data;
            });
        }
    };
    $scope.btnAcceptPopup = {
        text: 'موافقه',
        type: "success",
        onClick: function (e) {
            debugger;
            AcademicCommitteeSrvc.AcceptStudents($scope.gridSelectedRowsData).then(function (data) {
                if (data.data.status === 500) {
                    DevExpress.ui.notify({ message: data.data.Message, type: data.data.Type, displayTime: 3000, closeOnClick: true });
                }
                if (data.data.status === 200) {
                    DevExpress.ui.notify({ message: data.data.Message, type: data.data.Type, displayTime: 3000, closeOnClick: true });
                }
                AcademicCommitteeSrvc.GetStudentsForAcademicCommittee($scope.FacultionIds, $scope.DegreeIds).then(function (data) {
                    $scope.AcademicCommitteeList = data.data;
                });
                $scope.gridSelectedRowsData = null;
            });
        }
    };
    $scope.btnRefusePopup = {
        text: 'رفض',
        type: "danger",
        onClick: function (e) {
            var result = DevExpress.ui.dialog.confirm("<i>هل انت متاكد؟</i>", "تاكيد الرفض");
            result.done(function (dialogResult) {
                if (dialogResult) {
                    AcademicCommitteeSrvc.RejectStudents($scope.gridSelectedRowsData).then(function (data) {
                        if (data.data.status === 500) {
                            DevExpress.ui.notify({ message: data.data.Message, type: data.data.Type, displayTime: 3000, closeOnClick: true });
                        }
                        if (data.data.status === 200) {
                            DevExpress.ui.notify({ message: data.data.Message, type: data.data.Type, displayTime: 3000, closeOnClick: true });
                        }
                        //Referesh Grid
                        AcademicCommitteeSrvc.GetStudentsForAcademicCommittee($scope.FacultionIds, $scope.DegreeIds).then(function (data) {
                            $scope.AcademicCommitteeList = data.data;
                        });
                        //Referesh StudentList
                        AcademicCommitteeSrvc.GetAcademicCommitteeStudents().then(function (data) {
                            $scope.StudentList = data.data;
                        });
                        $scope.gridSelectedRowsData = null;
                    });

                }
            });
        }
    };
    //Popup Upload Decisions
    $scope.PopUpUploadDecisions = {
        width: 600,
        contentTemplate: "info",
        title: "رفع ملف قرار اللجنة الاكاديمية",
        showTitle: true,
        dragEnabled: false,
        bindingOptions: {
            visible: "PopUpUploadDecisionsShow"
        },
        rtlEnabled: true,
        onHiding: function (e) {
        }
    };

    $scope.StudentsSelectBox = {
        bindingOptions: {
            value: "StudentId",
            items: "StudentList"
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
            AcademicCommitteeSrvc.GetAcademicCommitteeStudents().then(function (data) {
                $scope.StudentList = data.data;
            });
        },
        onValueChanged: function (e) {
            $scope.StudentIds = e.value.join(',');
        }
    };

    $scope.txtDecisionNumber = {
        bindingOptions: {
            value: "DecisionNumber"
        },
        placeholder: "رقم القرار",
        onValueChanged: function (e) {
            $scope.DecisionNumber = e.value;
        }
    }


    //fileUploadAcademicDecision
    $scope.AcademicDecisionFilesvalue = '';
    $scope.multiple = false;
    $scope.accept = "image/*,.pdf,.doc,.docx,.xls,.xlsx",
        $scope.uploadMode = "useButtons";

    $scope.fileUploadAcademicDecision = {
        name: "fileUploadAcademicDecision",
        uploadUrl: "/AcademicCommittee/AcademicDecisionsFiles",
        allowCanceling: true,
        rtlEnabled: true,
        readyToUploadMessage: "استعداد للرفع",
        selectButtonText: "اختر الملف",
        labelText: "",
        uploadButtonText: "رفع",
        uploadedMessage: "تم الرفع",
        invalidFileExtensionMessage: "نوع الملف غير مسموح",
        uploadFailedMessage: "خطأ أثناء الرفع",
        onInitialized: function (e) {
            debugger;
            $scope.AcademicDecisionFilesInstance = e.component;
        },
        onUploaded: function (e) {
            debugger;
            var xhttp = e.request;
            if (xhttp.status === 200) {
                //
            }
            else if (xhttp.status === 404) {
                DevExpress.ui.notify({ message: "الملف المرفوع لايحتوي على بيانات" }, "Error", 10000);
                $scope.AcademicDecisionFilesvalue = '';
                $scope.AcademicDecisionFilesInstance.reset();

            } else {
                DevExpress.ui.notify({ message: "حدث خطأ أثناء رفع الصورة" }, "Error", 10000);
                $scope.AcademicDecisionFilesvalue = '';
                $scope.AcademicDecisionFilesInstance.reset();
            }
        },
        onUploadError: function (e) {
            debugger;
            var xhttp = e.request;
            if (xhttp.status === 500) {
                DevExpress.ui.notify({ message: "حدث خطأ أثناء رفع الملف" }, "Error", 10000);
                $scope.AcademicDecisionFilesvalue = '';
                $scope.AcademicDecisionFilesInstance.reset();
            }
        },
        bindingOptions: {
            multiple: "multiple",
            accept: "accept",
            value: "AcademicDecisionFilesvalue",
            uploadMode: "uploadMode"
        }
    };
    //Remove File
    $scope.RemoveAcademicDecisionFile = function (hashkey) {
        var nametoRemove = "";
        angular.forEach($scope.AcademicDecisionFilesvalue,
            function (file, indx) {
                if (file.$$hashKey === hashkey) {
                    nametoRemove = file.name;
                    $scope.AcademicDecisionFilesInstance.splice(indx, 1);
                }
            });
    };

    $scope.btnSaveDecisionFile = {
        text: 'حفظ',
        type: 'success',
        onClick: function (e) {
            if ($scope.StudentIds === null || $scope.StudentIds === '') {
                DevExpress.ui.notify({ message: "عقوا اختر الطلاب" }, "Error", 3000);
                return false;
            }
            if ($scope.DecisionNumber === null || $scope.DecisionNumber === '') {
                DevExpress.ui.notify({ message: "عفوا ادخل رقم القرار" }, "Error", 3000);
                return false;
            }

            if ($scope.AcademicDecisionFilesvalue === null || $scope.AcademicDecisionFilesvalue === '') {
                DevExpress.ui.notify({ message: "عفوا اختر الملف" }, "Error", 3000);
                return false;
            }

            AcademicCommitteeSrvc.SaveDecisions({ DecisionNumber: $scope.DecisionNumber, StudentIds: $scope.StudentIds }).then(function (data) {
                if (data.data.status === 500) {
                    DevExpress.ui.notify({ message: data.data.Message, type: data.data.Type, displayTime: 3000, closeOnClick: true });
                }
                if (data.data.status === 200) {
                    DevExpress.ui.notify({ message: data.data.Message, type: data.data.Type, displayTime: 3000, closeOnClick: true });
                }


                $scope.PopUpUploadDecisionsShow = false;
            });
        }
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
    $scope.btnPrintExtraChances = {
        icon: "print",
        text: "طباعة",
        onClick: function () {
            HelperServices.Print("تفاصيل الفرص", "DetailExtraChances");
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

}]);
