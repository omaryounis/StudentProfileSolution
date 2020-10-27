(function () {
    app.controller("AcademicActivitiesCtrl",
        ["$scope", "$http", "AcademicActivitiesSrvc",
            function ($scope, $http, AcademicActivitiesSrvc) {


                /*********************************** Permissions *********************************/
                $scope.Permissions = {
                    CreateActivityRequest: false,
                    SearchActivityRequest: false,
                    DeleteActivityRequest: false
                };
                $http({
                    method: "get",
                    url: "/AcademicActivities/GetAdminActivityRequestsPermissions?screenId=71"
                }).then(function (data) {
                    $scope.Permissions.CreateActivityRequest = data.data.CreateActivityRequest;
                    $scope.Permissions.SearchActivityRequest = data.data.SearchActivityRequest;
                    $scope.Permissions.DeleteActivityRequest = data.data.DeleteActivityRequest;
                });
                /*--------------------------------* Permissions *--------------------------------*/


                //===========================
                // الجزء الخاص بقائمة الطلاب 
                //===========================                
                let DataSourceStudents = new DevExpress.data.DataSource({
                    paginate: true,
                    cacheRawData: true,
                    key: "STUDENT_ID",
                    loadMode: "raw",
                    load: function () {
                        return $.getJSON("/Advances/GetAllStudents", function () { });
                    }
                });
                DataSourceStudents.load();

                $scope.ImageSrc = '';
                $scope.Menu_StudentBasicData = {
                    DL_Students: {
                        dataSource: DataSourceStudents,
                        bindingOptions: {
                            value: "MDL_StudentId",
                            text: "MDL_StudentName"
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
                            $scope.MDL_StudentId = e.value;
                            if (e.value !== null && e.value !== '' && e.value !== undefined) {

                                // * Filling Personal Controls...
                                AcademicActivitiesSrvc.GetStudentDataByID(e.value).then(function (data) {
                                    
                                    $scope.StudentBasicDataArray = data.data;
                                    debugger;
                                    $scope.ImageSrc = `/Content/UserFiles/${e.value}/${data.data[0].ImageName}`;

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
                                debugger;
                                $scope.ImageSrc = 'https://cdn131.picsart.com/289212614039201.png?r1024x1024';
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
                        }
                    },
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


                //================================================================
                // الجزء الخاص بقائمة المشاركات الخاصة بالطالب الذي تم اختياره 
                //================================================================
                var DataSourceActivitiesGrid = new DevExpress.data.DataSource({
                    paginate: true,
                    cacheRawData: true,
                    key: "AcademicActivitiesId",
                    loadMode: "raw",
                    load: function () {
                        if ($scope.MDL_StudentId !== '' &&
                            $scope.MDL_StudentId !== null &&
                            $scope.MDL_StudentId !== undefined) {
                            return $.getJSON(`/AcademicActivities/GetActivitiesByAdmin?studentId=${$scope.MDL_StudentId}`, function () { });
                        }
                        return [];
                    }
                });
                $scope.ActivitiesGrid = {
                    dataSource: DataSourceActivitiesGrid,
                    bindingOptions: {
                        rtlEnabled: true
                    },
                    sorting: {
                        mode: "multiple"
                    },
                    wordWrapEnabled: true,
                    showBorders: true,
                    searchPanel: {
                        visible: false,
                        placeholder: "بحث",
                        width: 250
                    },
                    paging: {
                        pageSize: 10
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
                    width: '100%',
                    columnChooser: {
                        enabled: true
                    },
                    "export": {
                        enabled: true,
                        fileName: "StudentActivities"
                    },
                    onCellPrepared: function (e) {
                        //if (e.rowType === "header" && e.column.command === "edit") {
                        //    e.column.width = 60;
                        //    e.column.alignment = "center";
                        //    e.cellElement.text(" حذف ");
                        //}

                        //if (e.rowType === "data" && e.column.command === "edit") {
                        //    $links = e.cellElement.find(".dx-link");
                        //    $links.text("");
                            
                        //    if (e.data.ApprovedStatus !== "معتمد" && e.data.ApprovedStatus !== "مرفوض") {
                        //        $links.filter(".dx-link-delete").addClass("btn btn-danger btn-sm fa fa-trash-o");
                        //    }

                        //}

                        if (e.rowType === "data" && e.column.dataField === "ApprovedStatus") {
                            if (e.value === "تحت المراجعة") {
                                e.cellElement.css({ color: '#c7bb27', "font-weight": "bold" });

                            }
                            else if (e.value === "معتمد") {
                                e.cellElement.css({ color: '#5cb85c', "font-weight": "bold" });

                            }
                            else if (e.value === "مرفوض") {
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
                            confirmDeleteMessage: "تأكيد حذف هذه المشاركة ؟",
                            deleteRow: "",
                            editRow: "",
                            addRow: ""
                        },
                        useIcons: true
                    },
                    columns: [
                        {
                            caption: '#',
                            width: 30,
                            alignment: "right",
                            cellTemplate: function (cellElement, cellInfo) {
                                cellElement.text(cellInfo.row.rowIndex + 1);
                            }
                        },
                        {
                            dataField: "Name",
                            caption: "اسم المشاركة",
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
                            caption: "مكان المشاركة",
                            alignment: "right"
                        },
                        {
                            dataField: "Type",
                            caption: "نوع المشاركة",
                            alignment: "right",
                            visible: true
                        },
                        {
                            dataField: "Degree",
                            caption: "التقييم",
                            alignment: "right",
                            visible: true
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
                            dataType: "date"
                        },
                        {
                            dataField: "EndDate",
                            caption: "تاريخ النهاية",
                            alignment: "right",
                            dataType: "date"
                        },
                        {
                            dataField: "ApprovedStatus",
                            caption: "حالة الإعتماد",
                            alignment: "right"
                        },
                        {
                            dataField: "RefusalReason",
                            caption: "الملاحظات",
                            alignment: "right",
                            visible: false
                        },
                        {
                            caption: "المرفقات",
                            cssClass: "text-center",
                            //allowHiding: false,
                            width: 80,
                            cellTemplate: function (container, options) {
                                $("<div />").dxButton({
                                    text: '',
                                    hint: "تحميل",
                                    type: 'success',
                                    icon: 'fa fa-download',
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
                        },
                        {
                            caption: "حذف",
                            cssClass: "text-center",
                            //allowHiding: false,
                            width: 75,
                            cellTemplate: function (container, options) {
                                debugger;
                                if (e.data.ApprovedStatus !== "معتمد" && e.data.ApprovedStatus !== "مرفوض") {
                                    $("<div />").dxButton({
                                        text: '',
                                        hint: "حذف",
                                        type: 'danger',
                                        icon: 'fa fa-trash-o',
                                        useSubmitBehavior: false,
                                        onClick: function (e) {



                                            var result = DevExpress.ui.dialog.confirm("<i>تأكيد حذف هذه المشاركة؟</i>", "تاكيد الحذف");
                                            result.done(function (dialogResult) {
                                                if (dialogResult) {
                                                    $http({
                                                        method: "Delete",
                                                        url: "/AcademicActivities/DeleteActivityRequestArchive",
                                                        data: { AcademicActivityId: options.data.AcademicActivitiesId }
                                                    }).then(function (data) {
                                                        if (data.data !== "") {
                                                            if (data.data !== "") { swal("حدث خطأ", data.data, "error"); }
                                                        } else {
                                                            DataSourceActivitiesGrid.reload();
                                                            swal("Done!", "تم الحذف بنجاح", "success");
                                                        }
                                                    });
                                                }
                                            });

                                        }
                                    }).appendTo(container);
                                }
                            }
                        }
                    ],
                    allowColumnResizing: true,
                    columnResizingMode: "widget",
                    allowColumnReordering: true,
                    showColumnLines: true,
                    rowAlternationEnabled: true,
                    onInitialized: function (e) {
                        $scope.ActivitiesGridInstance = e.component;
                    },
                    onContentReady: function (e) {

                        e.component.columnOption("command:edit", "visible", $scope.Permissions.DeleteActivityRequest);
                        e.component.columnOption("command:edit", "width", 60);

                    },
                    onRowRemoving: function (e) {
                        e.cancel = true;
                       
                    }
                };



                //=======================================================
                // الشاشة الخاصة بإضافة مشاركة للطالب الذي تم اختياره
                //=======================================================
                var now = new Date();
                $scope.Activity = {
                    AddActivityPopupOptions: {
                        showTitle: true,
                        dragEnabled: false,
                        shadingColor: "rgba(0, 0, 0, 0.69)",
                        closeOnOutsideClick: false,
                        bindingOptions: {
                            visible: "AddActivityPopUpShow"
                        },
                        title: "إضافة مشاركة للطالب",
                        contentTemplate: 'AddActivityContent',
                        width: 1280,
                        height: 510,
                        rtlEnabled: true,
                        onHiding: function () {
                            $scope.ResetControls();
                            if ($scope.FileUploadingOptionsInstance) {
                                $scope.FileUploadingOptionsInstance.reset();
                            }
                            var validationGroup = DevExpress.validationEngine.getGroupConfig("AddActivityForm_vg");
                            if (validationGroup) {
                                validationGroup.reset();
                            }
                        }
                    },
                    SearchActivityPopupOptions: {
                        showTitle: true,
                        dragEnabled: false,
                        shadingColor: "rgba(0, 0, 0, 0.69)",
                        closeOnOutsideClick: false,
                        bindingOptions: {
                            visible: "SearchActivityPopUpShow"
                        },
                        title: "بيانات المشاركات",
                        contentTemplate: 'SearchActivityContent',
                        width: 1280,
                        height: 550,
                        rtlEnabled: true,
                        onHiding: function () {
                            $scope.ResetControls();
                            if ($scope.FileUploadingOptionsInstance) {
                                $scope.FileUploadingOptionsInstance.reset();
                            }
                            var validationGroup = DevExpress.validationEngine.getGroupConfig("AddActivityForm_vg");
                            if (validationGroup) {
                                validationGroup.reset();
                            }
                        }
                    },
                    VR_Required: {
                        validationRules: [{ type: "required", message: "حقل إلزامي" }],
                        validationGroup: "AddActivityForm_vg"
                    },
                    TB_ActivityName: {
                        bindingOptions: {
                            value: "MDL_ActivityName"
                        },
                        onValueChanged: function (e) {
                            $scope.MDL_ActivityName = e.value;
                        },
                        placeholder: "",
                        rtlEnabled: true
                    },
                    NB_ActivityDuration: {
                        bindingOptions: {
                            value: "MDL_ActivityDuration"
                        },
                        onValueChanged: function (e) {
                            $scope.MDL_ActivityDuration = e.value;
                        },
                        placeholder: "",
                        min: 1,
                        rtlEnabled: true,
                        showSpinButtons: true,
                        showClearButton: true
                    },
                    TB_ActivityLocation: {
                        bindingOptions: {
                            value: "MDL_ActivityLocation"
                        },
                        onValueChanged: function (e) {
                            $scope.MDL_ActivityLocation = e.value;
                        },
                        placeholder: "",
                        rtlEnabled: true
                    },
                    TB_ActivityType: {
                        bindingOptions: {
                            value: "MDL_ActivityType"
                        },
                        onValueChanged: function (e) {
                            $scope.MDL_ActivityType = e.value;
                        },
                        placeholder: "",
                        rtlEnabled: true
                    },
                    TB_ActivityDegree: {
                        bindingOptions: {
                            value: "MDL_ActivityDegree"
                        },
                        onValueChanged: function (e) {
                            $scope.MDL_ActivityDegree = e.value;
                        },
                        placeholder: "",
                        rtlEnabled: true
                    },
                    NB_ActivityRatio: {
                        bindingOptions: {
                            value: "MDL_ActivityRatio"
                        },
                        onValueChanged: function (e) {
                            $scope.MDL_ActivityRatio = e.value;
                        },
                        placeholder: "",
                        max: 100,
                        min: 1,
                        rtlEnabled: true,
                        showSpinButtons: true,
                        showClearButton: true
                    },
                    DB_ActivityStartDate: {
                        bindingOptions: {
                            value: "MDL_ActivityStartDate"
                        },
                        max: now,
                        type: "date",
                        displayFormat: "dd/MM/yyyy",
                        onValueChanged: function (e) {
                            $scope.MDL_ActivityStartDate = e.value;
                            if (new Date($scope.MDL_ActivityStartDate) > new Date($scope.MDL_ActivityEndDate)) {
                                $scope.MDL_ActivityEndDate = '';
                            }
                        }
                    },
                    DB_ActivityEndDate: {
                        bindingOptions: {
                            value: "MDL_ActivityEndDate",
                            min: "MDL_ActivityStartDate"
                        },
                        max: now,
                        type: "date",
                        displayFormat: "dd/MM/yyyy",
                        onValueChanged: function (e) {
                            $scope.MDL_ActivityEndDate = e.value;
                        }
                    },
                    UploadFilePopupOptions: {
                        showTitle: true,
                        dragEnabled: false,
                        shadingColor: "rgba(0, 0, 0, 0.69)",
                        closeOnOutsideClick: false,
                        bindingOptions: {
                            visible: "UploadPopUpShow"
                        },
                        contentTemplate: 'UploadFileContent',
                        title: "رفع نموذج المشاركة",
                        width: 800,
                        height: 550,
                        rtlEnabled: true,
                        onHiding: function () { }
                    },
                    btnSaveOptions: {
                        bindingOptions: {
                        },
                        text: 'حفظ المشاركة',
                        visible: true,
                        type: 'success',
                        validationGroup: "AddActivityForm_vg",
                        icon: 'save',
                        useSubmitBehavior: true,
                        onClick: function (e) {
                            

                            var validationGroup = DevExpress.validationEngine.getGroupConfig("AddActivityForm_vg");
                            if (validationGroup) {
                                var result = validationGroup.validate();
                                if (result.isValid) {

                                    if ($scope.MDL_StudentId === '' || $scope.MDL_StudentId === null || $scope.MDL_StudentId === undefined) {
                                        return DevExpress.ui.notify({ message: "لابد من اختيار الطالب أولا" }, "Error", 5000);
                                    }

                                    if (new Date($scope.MDL_ActivityEndDate) < new Date($scope.MDL_ActivityStartDate)) {
                                        return DevExpress.ui.notify({ message: "لا يمكن أن يكون تاريخ بداية المشاركة بعد تاريخ نهاية المشاركة" }, "Error", 5000);
                                    }

                                    if ($scope.MDL_UploadingFilesvalue === '' || $scope.MDL_UploadingFilesvalue === null || $scope.MDL_UploadingFilesvalue === undefined) {
                                        return DevExpress.ui.notify({ message: " لابد من رفع نموذج المشاركة (الشهادة)" }, "Error", 5000);
                                    }

                                    AcademicActivitiesSrvc.SaveActivityArchiveByAdmin(
                                        $scope.MDL_StudentId,
                                        $scope.MDL_ActivityDegree,
                                        $scope.MDL_ActivityDuration,
                                        $scope.MDL_ActivityLocation,
                                        $scope.MDL_ActivityType,
                                        $scope.MDL_ActivityStartDate,
                                        $scope.MDL_ActivityEndDate,
                                        $scope.MDL_ActivityName,
                                        $scope.MDL_ActivityRatio).then(function (data) {
                                            if (data.data !== "") { swal("حدث خطأ", data.data, "error"); }
                                            else {
                                                $scope.ResetControls();
                                                validationGroup.reset();
                                                swal("Done!", "تم الحفظ بنجاح", "success");
                                                $scope.FileUploadingOptionsInstance.reset();
                                                $scope.AddActivityPopUpShow = false;
                                                DataSourceActivitiesGrid.reload();
                                                $scope.SearchActivityPopUpShow = true;
                                            }
                                        });
                                }
                            }
                        }
                    },
                    btnAddActivityOptions: {
                        bindingOptions: {
                            visible: "Permissions.CreateActivityRequest"
                        },
                        text: 'إضافة مشاركات',
                        type: 'success',
                        icon: 'plus',
                        onClick: function (e) {
                            if ($scope.MDL_StudentId === '' || $scope.MDL_StudentId === null || $scope.MDL_StudentId === undefined) {
                                return swal("تنبيه", "لابد من اختيار الطالب أولا", "warning");
                            }
                            $scope.AddActivityPopUpShow = true;
                        }
                    },
                    btnSearchOptions: {
                        bindingOptions: {
                            visible: "Permissions.SearchActivityRequest"
                        },
                        text: 'استعراض المشاركات',
                        elementAttr: { "class": "btn-info" },
                        icon: 'search',
                        onClick: function (e) {
                            if ($scope.MDL_StudentId === '' || $scope.MDL_StudentId === null || $scope.MDL_StudentId === undefined) {
                                return swal("تنبيه", "لابد من اختيار الطالب أولا", "warning");
                            }
                            DataSourceActivitiesGrid.reload();
                            $scope.SearchActivityPopUpShow = true;
                        }
                    },
                    btnPrintOptions: {
                        text: 'النموذج',
                        visible: true,
                        elementAttr: { "class": "btn-dark" },
                        //type: 'default',
                        icon: 'print',
                        useSubmitBehavior: true,
                        onClick: function (e) {
                            if ($scope.MDL_StudentId === '' || $scope.MDL_StudentId === null || $scope.MDL_StudentId === undefined) {
                                return swal("تنبيه", "لابد من اختيار الطالب أولا", "warning");
                            }
                        }
                    }
                };


                //==========================================================
                // الجزء الخاص برفع المرفقات والتابعة لشاشة إضافة مشاركة
                //==========================================================
                $scope.uploadMode = "useButtons";
                $scope.btnUploadText = "رفع المرفق";
                $scope.btnUploadOptions = {
                    bindingOptions: {
                        text: 'btnUploadText',
                        icon: 'btnUploadIcon'
                    },
                    visible: true,
                    type: 'default',
                    useSubmitBehavior: false,
                    onClick: function (e) {
                        $scope.UploadPopUpShow = true;
                    }
                };

                //Upload Options...
                $scope.multiple = false;
                $scope.accept = ".pdf";
                $scope.btnUploadIcon = "upload";
                $scope.UploadingFilesvalue = [];
                $scope.FileUploadingOptions = {
                    name: "fileSent",
                    uploadUrl: "/AcademicActivities/UploadFiles",
                    allowCanceling: true,
                    rtlEnabled: true,
                    readyToUploadMessage: "استعداد للرفع",
                    selectButtonText: "رفع المرفق",
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
                        
                        if (e.value.length > 0) {
                            if (e.value[0].type === "application/pdf") {
                                $scope.MDL_UploadingFilesvalue = e.value;
                            } else {                                

                                $scope.MDL_UploadingFilesvalue = null;
                                $scope.btnUploadIcon = "upload";
                                $scope.btnUploadText = "رفع المرفق";

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
                        
                        if (e.request.status === 200) {
                            
                            $scope.UploadPopUpShow = false;
                            $scope.btnUploadText = "تم رفع المرفق بنجاح";
                            $scope.btnUploadIcon = "check";
                        }
                        if (e.request.status === 400) {
                            $scope.UploadPopUpShow = false;
                            return DevExpress.ui.notify({ message: "غير مسموح برفع هذا الملف" }, "Error", 10000);
                        }

                    }
                };

                //Remove Uploaded File...
                $scope.RemoveUploadingFile = function (hashkey) {
                    $scope.MDL_UploadingFilesvalue = '';
                    $scope.FileUploadingOptionsInstance.reset();
                    $http({
                        method: "POST",
                        url: "/AcademicActivities/UploadFiles"
                    });
                    $scope.btnUploadText = "رفع المرفق";
                    $scope.btnUploadIcon = "upload";
                };
                $scope.ResetUploadingControls = function () {

                    $scope.MDL_UploadingFilesvalue = null;
                    $scope.btnUploadIcon = "upload";
                    $scope.btnUploadText = "رفع المرفق";

                    if ($scope.FileUploadingOptionsInstance) {
                        $scope.FileUploadingOptionsInstance.reset();
                    }
                };


                //Functions...
                $scope.ResetControls = function () {
                    $scope.MDL_ActivityType = '';
                    $scope.MDL_ActivityName = '';
                    $scope.MDL_ActivityRatio = '';
                    $scope.MDL_ActivityDegree = '';
                    $scope.MDL_ActivityEndDate = '';
                    $scope.MDL_ActivityDuration = '';
                    $scope.MDL_ActivityLocation = '';
                    $scope.MDL_ActivityStartDate = '';
                    $scope.MDL_UploadingFilesvalue = '';

                    $scope.btnUploadIcon = "upload";
                    $scope.btnUploadText = "رفع المرفق";
                };
            }
        ]);
})();
