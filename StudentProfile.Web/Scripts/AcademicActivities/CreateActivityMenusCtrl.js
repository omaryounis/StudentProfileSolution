(function () {
    app.controller("CreateActivityMenusCtrl",
        ["$scope", "$http", "AcademicActivitiesSrvc",
            function ($scope, $http, AcademicActivitiesSrvc) {

                /*********************************** Permissions *********************************/
                $scope.Permissions = {
                    AddEvaluation: false,
                    AddActivityName: false,
                    AddActivityType: false,
                    DeleteEvaluation: false,
                    AddActivityMaster: false,
                    DeleteActivityName: false,
                    DeleteActivityType: false,
                    AddActivityLocation: false,
                    DeleteActivityMaster: false,
                    DeleteActivityLocation: false
                };
                $http({
                    method: "get",
                    url: "/AcademicActivities/GetCreateActivityMenusPermissions?screenId=99"
                }).then(function (data) {

                    $scope.Permissions.AddEvaluation = data.data.AddEvaluation;
                    $scope.Permissions.AddActivityName = data.data.AddActivityName;
                    $scope.Permissions.AddActivityType = data.data.AddActivityType;
                    $scope.Permissions.DeleteEvaluation = data.data.DeleteEvaluation;
                    $scope.Permissions.AddActivityMaster = data.data.AddActivityMaster;
                    $scope.Permissions.DeleteActivityName = data.data.DeleteActivityName;
                    $scope.Permissions.DeleteActivityType = data.data.DeleteActivityType;
                    $scope.Permissions.AddActivityLocation = data.data.AddActivityLocation;
                    $scope.Permissions.DeleteActivityMaster = data.data.DeleteActivityMaster;
                    $scope.Permissions.DeleteActivityLocation = data.data.DeleteActivityLocation;
                });
                /*--------------------------------* Permissions *--------------------------------*/

                var masterIdBeEditted = getUrlParameter("MasterIdBeEditted");
                
                $scope.MDL_PostActivityMenuCheck = false;
                $scope.MainPopup = {
                    Options: {
                        width: 700,
                        height: 640,
                        visible: true,
                        title: "إنشاء قوائم مشاركات الطلاب",
                        contentTemplate: "MainPopupContent",
                        showTitle: true,
                        shadingColor: "rgba(0, 0, 0, 0.69)",
                        dragEnabled: false,
                        closeOnOutsideClick: false,
                        onHiding: function () {
                            window.location.href = "/AcademicActivities/SearchActivityMenus";
                        }
                    },
                    PostActivityMenuCheckBox: {
                        bindingOptions: {
                            value: "MDL_PostActivityMenuCheck"
                        },
                        onValueChanged: function (e) {
                            $scope.MDL_PostActivityMenuCheck = e.value;
                        }
                    },
                    UploadFilePopup: {
                        showTitle: true,
                        dragEnabled: false,
                        shadingColor: "rgba(0, 0, 0, 0.69)",
                        closeOnOutsideClick: false,
                        bindingOptions: {
                            visible: "UploadPopUpShow"
                        },
                        contentTemplate: 'UploadFileContent',
                        title: "رفع مرفق قائمة المشاركة",
                        width: 750,
                        height: 610,
                        rtlEnabled: true,
                        onHiding: function () { }
                    },
                    UploadFileBtn: {
                        text: "المرفق",
                        icon: 'upload',
                        visible: true,
                        type: 'default',
                        useSubmitBehavior: false,
                        onClick: function (e) {
                            $scope.UploadPopUpShow = true;
                        }
                    }
                };


                //=================
                // أسماء المشاركات
                //=================
                var dataSourceActivityNames = new DevExpress.data.DataSource({
                    paginate: true,
                    cacheRawData: true,
                    key: "Value",
                    loadMode: "raw",
                    load: function () {
                        return $.getJSON(`/AcademicActivities/GetConfigerdActivityNames`, function (data) { });
                    }
                });
                $scope.ActivityNamesSelectBox = {
                    dataSource: dataSourceActivityNames,
                    bindingOptions: {
                        value: "MDL_ActivityConfigId",
                        readOnly: "MDL_ActivityNamesReadOnly"
                    },
                    placeholder: "اختر اسم المشاركة",
                    noDataText: "لا يوجد بيانات",
                    displayExpr: "Text",
                    valueExpr: "Value",
                    showBorders: true,
                    searchEnabled: true,
                    rtlEnabled: true,
                    showClearButton: true,
                    onOpened: function (e) {
                        debugger;
                        dataSourceActivityNames.reload();
                    },
                    onValueChanged: function (e) {
                        debugger;
                        if (!(masterIdBeEditted > 0)) {
                            if ($scope.MDL_ActivityMasterId > 0) {
                                $scope.MDL_ActivityMasterId = '';
                            }
                            //dataSourceActivityNumbers.reload();
                        }
                    }
                };


                //=================
                // أرقام المشاركات
                //=================
                var dataSourceActivityNumbers = new DevExpress.data.DataSource({
                    paginate: true,
                    cacheRawData: true,
                    key: "ID",
                    loadMode: "raw",
                    load: function () {
                        debugger;
                        if ($scope.MDL_ActivityConfigId !== '' && $scope.MDL_ActivityConfigId !== '' &&
                            $scope.MDL_ActivityConfigId !== null && $scope.MDL_ActivityConfigId !== null &&
                            $scope.MDL_ActivityConfigId !== undefined && $scope.MDL_ActivityConfigId !== undefined) {
                            if (masterIdBeEditted > 0) {
                                return [{ ID: $scope.MDL_ActivityMasterId, ActivityNo: $scope.MDL_ActivityNo }];
                            } else {

                                return $.getJSON(`/AcademicActivities/GetActivityNumbers?paramConfigId=${$scope.MDL_ActivityConfigId}`, function (data) { });
                            }
                        }
                    }
                });
                $scope.ActivityNoSelectBox = {
                    dataSource: dataSourceActivityNumbers,
                    bindingOptions: {
                        value: "MDL_ActivityMasterId",
                        readOnly: "MDL_ActivityNumbersReadOnly"
                    },
                    placeholder: "اختر رقم المشاركة",
                    noDataText: "لا يوجد بيانات",
                    displayExpr: "ActivityNo",
                    valueExpr: "ID",
                    showBorders: true,
                    searchEnabled: true,
                    rtlEnabled: true,
                    showClearButton: true,
                    onOpened: function (e) {
                        debugger;
                        dataSourceActivityNumbers.reload();
                    },
                    onValueChanged: function (e) {

                        $scope.MDL_ActivityMasterId = e.value;
                        //if (!(masterIdBeEditted > 0)) {
                        //    $scope.DataSourceActivityMenuBandsGrid = [];
                        //}
                    }
                };
               

                //=======
                // الطلاب
                //=======   
                $scope.StudentArray = [];
                var dataSourceStudents = new DevExpress.data.DataSource({
                    paginate: true,
                    cacheRawData: true,
                    key: "STUDENT_ID",
                    loadMode: "raw",
                    load: function () {                       
                        return $.getJSON("/Advances/GetAllStudents", function (data) {
                            debugger;
                            $scope.StudentArray = data;
                            return data;
                        });
                    }
                });
                $scope.StudentSelectBox = {
                    dataSource: dataSourceStudents,
                    bindingOptions: {
                        value: "MDL_StudentId",
                        text: "MDL_StudentName",
                        readOnly: "MDL_StudentSelectBox_ReadOnly"
                    },
                    placeholder: "--اختر--",
                    noDataText: "لا يوجد بيانات",
                    displayExpr: "STUDENT_NAME",
                    valueExpr: "STUDENT_ID",
                    searchEnabled: true,
                    searchExpr: ['STUDENT_ID', 'NATIONAL_ID', 'STUDENT_NAME'],
                    showBorders: true,
                    rtlEnabled: true,
                    showClearButton: false,
                    pagingEnabled: true,
                    onValueChanged: function (e) {
                        debugger;
                        $scope.MDL_StudentId = e.value;

                        //let model = $scope.StudentSelectBox.dataSource._items.find(x => x.STUDENT_ID === e.value);
                        let model = $scope.StudentArray.find(x => x.STUDENT_ID === e.value);
                       if (model) $scope.MDL_StudentName = model.STUDENT_NAME;
                       else $scope.MDL_StudentName = null;

                        //if (RowIsEditing === true) {
                        //    $scope.MDL_StudentName = $scope.StudentIsEditing_Name;
                        //} else {
                        //    $scope.MDL_StudentName = e.model.MDL_StudentName;
                        //}
                    }
                };

                //===========
                // التقييمات
                //===========
                var dataSourceEvaluations = new DevExpress.data.DataSource({
                    paginate: true,
                    cacheRawData: true,
                    key: "Value",
                    loadMode: "raw",
                    load: function () {
                        return $.getJSON(`/AcademicActivities/GetActivityConfig?paramKey=evaluation`, function (data) { });
                    }
                });
                $scope.EvaluationSelectBox = {
                    dataSource: dataSourceEvaluations,
                    bindingOptions: {
                        text: "MDL_EvaluationText",
                        value: "MDL_EvaluationId"
                    },
                    placeholder: "--اختر--",
                    noDataText: "لا يوجد بيانات",
                    displayExpr: "Text",
                    valueExpr: "Value",
                    rtlEnabled: true,
                    showBorders: true,
                    searchEnabled: true,
                    showClearButton: false,
                    onOpened: function (e) {
                        debugger;
                        dataSourceEvaluations.reload();
                    },
                    onValueChanged: function (e) {
                        debugger;
                        $scope.MDL_EvaluationId = e.value;

                        let model = $scope.EvaluationSelectBox.dataSource._items.find(x => x.Value === e.value);
                        if (model) $scope.MDL_EvaluationText = model.Text;
                        else $scope.MDL_EvaluationText = null;
                    }
                };


                //===========================
                // إسناد الطلاب علي المشاركة
                //===========================
                $scope.AddUpdateActivityBand = function (IsEditing) {

                    if (!($scope.MDL_ActivityMasterId && $scope.MDL_ActivityMasterId !== '')) {
                        return swal("تنبيه", "لابد من اختيار رقم المشاركة أولا ", "warning");
                    }

                    if (!($scope.MDL_EvaluationId && $scope.MDL_EvaluationId !== '')) {
                        return swal("تنبيه", "لابد من اختيار التقييم أولا ", "warning");
                    }

                    if (!($scope.MDL_StudentId && $scope.MDL_StudentId !== '')) {
                        return swal("تنبيه", "لابد من اختيار الطالب أولا ", "warning");
                    }

                    const index = $scope.DataSourceActivityMenuBandsGrid
                        .findIndex((e) => e.StudentId === $scope.MDL_StudentId);

                    if (index === -1) {
                        debugger;
                        $scope.DataSourceActivityMenuBandsGrid.push(new ActivityMenuBand(
                            $scope.MDL_StudentId,
                            $scope.MDL_StudentName,
                            $scope.MDL_EvaluationId,
                            $scope.MDL_EvaluationText
                        ));
                        $scope.MDL_StudentId = null;
                        $scope.MDL_StudentName = null;
                    } else {
                        if (IsEditing === true) {
                            debugger;
                            $scope.DataSourceActivityMenuBandsGrid[index] = new ActivityMenuBand(
                                $scope.MDL_StudentId,
                                $scope.MDL_StudentName,
                                $scope.MDL_EvaluationId,
                                $scope.MDL_EvaluationText
                            );
                            $scope.MDL_StudentId = null;
                            $scope.MDL_StudentName = null;

                            $scope.AddRowShow = true;
                            $scope.UpdateRowShow = false;
                            $scope.CancelRowShow = false;
                            $scope.clearFieldsShow = true;

                            $scope.MDL_StudentSelectBox_ReadOnly = false;

                            RowIsEditing = false;
                        } else {
                            return swal("تنبيه", "هذا الطالب مضاف من قبل علي نفس القائمة", "warning");
                        }
                    }

                };
                $scope.clearFields = function () {

                    RowIsEditing = false;

                    $scope.MDL_EvaluationId = null;
                    $scope.MDL_EvaluationText = null;

                    $scope.MDL_StudentId = null;
                    $scope.MDL_StudentName = null;

                    $scope.AddRowShow = true;
                    $scope.clearFieldsShow = true;
                    $scope.UpdateRowShow = false;
                    $scope.CancelRowShow = false;

                    $scope.MDL_StudentSelectBox_ReadOnly = false;
                };


                //=============================================================
                // القائمة الخاصة بإسناد الطلاب علي المشاركة علي حسب درجاتهم
                //=============================================================
                $scope.ActivityMenuBandsGrid = {
                    bindingOptions: {
                        rtlEnabled: true,
                        dataSource: "DataSourceActivityMenuBandsGrid"
                    },
                    sorting: {
                        mode: "multiple"
                    },
                    wordWrapEnabled: true,
                    showBorders: true,
                    searchPanel: {
                        visible: false,
                        placeholder: "بحث",
                        width: 300
                    },
                    paging: {
                        pageSize: 100
                    },
                    pager: {
                        allowedPageSizes: "auto",
                        infoText: "Page {0} of {2} (عدد الطلاب {2} طالب)",
                        //infoText: "(صفحة  {0} من {1} ({2} عنصر",
                        showInfo: true,
                        showNavigationButtons: false,
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
                        visible: false
                    },
                    showRowLines: true,
                    groupPanel: {
                        visible: false,
                        emptyPanelText: ""
                    },
                    noDataText: "لا يوجد بيانات",
                    columnAutoWidth: true,
                    width: '100%',
                    columnChooser: {
                        enabled: false
                    },
                    "export": {
                        enabled: false,
                        fileName: ""
                    },
                    onCellPrepared: function (e) {
                        if (e.rowType === "header" && e.column.command === "edit") {
                            e.column.width = 202;
                        }
                        if (e.rowType === "data" && e.column.command === "edit") {
                            var $links = e.cellElement.find(".dx-link");
                            $links.text("");
                            $links.filter(".dx-link-edit").addClass("btn btn-dark-orange btn-sm fa fa-pencil");
                            $links.filter(".dx-link-delete").addClass("btn btn-danger btn-sm fa fa-trash-o");
                        }
                    },
                    editing: {
                        allowUpdating: true,
                        allowAdding: false,
                        allowDeleting: true,
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
                            dataField: "StudentName",
                            caption: "",
                            alignment: "right"
                        },
                        {
                            dataField: "EvaluationName",
                            caption: "",
                            alignment: "right",
                            width: 436
                        }
                    ],
                    showColumnHeaders: false,
                    allowColumnResizing: true,
                    columnResizingMode: "widget",
                    allowColumnReordering: true,
                    showColumnLines: true,
                    rowAlternationEnabled: true,
                    onRowRemoving: function (e) {
                        e.cancel = true;

                        $scope.clearFields();
                        var index = $scope.DataSourceActivityMenuBandsGrid.findIndex(i => i.StudentId === e.data.StudentId);
                        $scope.DataSourceActivityMenuBandsGrid.splice(index, 1);
                    },
                    onEditingStart: function (e) {
                        debugger;
                        e.cancel = true;
                        RowIsEditing = true;

                        $scope.MDL_StudentId = e.data.StudentId;
                        $scope.MDL_EvaluationId = e.data.EvaluationId;
                        //$scope.StudentIsEditing_Name = e.data.StudentName;
                        $scope.MDL_StudentName = e.data.StudentName;
                        $scope.MDL_EvaluationText = e.data.EvaluationName;


                        $scope.AddRowShow = false;
                        $scope.UpdateRowShow = true;
                        $scope.CancelRowShow = true;
                        $scope.clearFieldsShow = false;

                        $scope.MDL_StudentSelectBox_ReadOnly = true;
                    },
                    onContentReady: function (e) {
                        e.component.columnOption("command:edit", "width", 202);
                    }
                };


                //============================
                // الجزء الخاص برفع المرفقات
                //============================

                $scope.multiple = false;
                $scope.accept = ".pdf";
                $scope.uploadMode = "useButtons";
                $scope.UploadingFilesvalue = [];
                $scope.FileUploadingOptions = {
                    name: "fileSent",
                    uploadUrl: "/AcademicActivities/UploadActivityMasterFile",
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
                        }
                        if (e.request.status === 400) {
                            $scope.UploadPopUpShow = false;
                            return DevExpress.ui.notify({ message: "خطأ" }, "Error", 10000);
                        }
                    }
                };

                //Remove Uploaded File
                $scope.RemoveUploadingFile = function (hashkey) {
                    $scope.MDL_UploadingFilesvalue = '';
                    $scope.FileUploadingOptionsInstance.reset();
                    $http({
                        method: "POST",
                        url: "/AcademicActivities/UploadActivityMasterFile"
                    });
                };
                $scope.ResetUploadingControls = function () {

                    $scope.MDL_UploadingFilesvalue = null;
                    if ($scope.FileUploadingOptionsInstance) {
                        $scope.FileUploadingOptionsInstance.reset();
                    }
                };
                $scope.RemoveId = function (id) {
                    $scope.RemovedFile = id;
                    var index = $scope.Paths.findIndex(i => i.ID === id);
                    $scope.Paths.splice(index, 1);
                };

                //=====================
                // حفظ قائمة المشاركة
                //=====================
                $scope.validationRequired = {
                    validationRules: [{ type: "required", message: "حقل إلزامي" }],
                    validationGroup: "AddActivityMenuForm_vg"
                };
                $scope.SaveActivityMenuFunc = function (IsPosted) {
                    var validationGroup = DevExpress.validationEngine.getGroupConfig("AddActivityMenuForm_vg");
                    if (validationGroup) {
                        var result = validationGroup.validate();
                        if (result.isValid) {

                            if ($scope.RemovedFile === 0 &&
                                $scope.Paths.length === 0 &&
                                $scope.MDL_UploadingFilesvalue !== '' &&
                                $scope.MDL_UploadingFilesvalue !== null &&
                                $scope.MDL_UploadingFilesvalue !== undefined &&
                                $scope.MDL_UploadingFilesvalue.length === 0) {
                                return swal("تنبيه", "لابد من رفع مرفق قائمة المشاركات لإتمام عملية الحفظ", "warning");
                            }

                            if ($scope.DataSourceActivityMenuBandsGrid.length === 0) {
                                return swal("تنبيه", "لابد من إضافة طالب واحد علي الأقل لإتمام عملية الحفظ", "warning");
                            }
                            debugger;
                            $http({
                                method: "POST",
                                url: `/AcademicActivities/${masterIdBeEditted > 0 ? "EditActivityDetails" : "SaveActivityDetails"}`,
                                data: {
                                    paramIsPosted: IsPosted,
                                    paramRemovedFile: $scope.RemovedFile,
                                    paramActivityMasterId: $scope.MDL_ActivityMasterId,
                                    paramActivityMaster: $scope.DataSourceActivityMenuBandsGrid
                                }
                            }).then(function (data) {
                                if (data.data === "") {
                                    $scope.RemovedFile = 0;
                                    $scope.visibleAlertPopUp = true;

                                    if (!(masterIdBeEditted > 0)) {
                                        // Reset Controls...
                                        $scope.MDL_ActivityMasterId = '';
                                        $scope.MDL_ActivityConfigId = '';
                                        $scope.MDL_ActivityMasterId = null;
                                        $scope.DataSourceActivityMenuBandsGrid = [];

                                        $scope.clearFields();
                                        $scope.ResetUploadingControls();

                                        // Reset Validations...
                                        validationGroup.reset();
                                    }
                                } else {
                                    swal("حدث خطأ", data.data, "error");
                                }
                            });
                        }
                    }
                };

                $scope.AlertPopup = {
                    Options: {
                        width: 640,
                        height: 175,
                        contentTemplate: "AlertPopupContent",
                        showTitle: false,
                        shadingColor: "rgba(0, 0, 0, 0.69)",
                        dragEnabled: false,
                        closeOnOutsideClick: false,
                        bindingOptions: {
                            visible: "visibleAlertPopUp"
                        }
                    },
                    Buttons: {
                        btnClose: {
                            type: "danger",
                            text: "إغلاق",
                            onClick: function (e) {
                                $scope.visibleAlertPopUp = false;
                            }
                        },
                        btnAddNewMenu: {
                            icon: "refresh",
                            type: "default",
                            text: "",
                            onClick: function (e) {
                                window.location.href = "/AcademicActivities/CreateActivityMenus";
                            }
                        },
                        btnSearch: {
                            icon: "search",
                            type: "success",
                            text: "",
                            onClick: function (e) {
                                window.location.href = "/AcademicActivities/SearchActivityMenus";
                            }
                        }
                    }
                };

                $scope.IndexButtons = {
                    btnAdd: {
                        bindingOptions: {
                        },
                        text: masterIdBeEditted > 0 ? 'تعديل' : 'حفظ',
                        icon: 'save',
                        type: 'success',
                        validationGroup: "AddActivityMenuForm_vg",
                        useSubmitBehavior: true,
                        onClick: function (e) {
                            debugger;
                            $scope.SaveActivityMenuFunc($scope.MDL_PostActivityMenuCheck);
                        }
                    },
                    btnSearch: {
                        bindingOptions: {},
                        text: '',
                        icon: 'search',
                        type: 'Normal',
                        onClick: function (e) {
                            window.location.href = "/AcademicActivities/SearchActivityMenus";
                        }
                    }
                };


                function getUrlParameter(param, dummyPath) {
                    var sPageURL = dummyPath || window.location.search.substring(1),
                        sURLVariables = sPageURL.split(/[&||?]/),
                        res;

                    for (var i = 0; i < sURLVariables.length; i += 1) {
                        var paramName = sURLVariables[i],
                            sParameterName = (paramName || "").split("=");

                        if (sParameterName[0] === param) {
                            res = sParameterName[1];
                        }
                    }

                    return res;
                }

                //========
                // Classes
                //========
                class ActivityMenuBand {

                    constructor(StudentId, StudentName, EvaluationId, EvaluationName) {
                        this.StudentId = StudentId;
                        this.StudentName = StudentName;
                        this.EvaluationId = EvaluationId;
                        this.EvaluationName = EvaluationName;
                    }
                }

                if (masterIdBeEditted > 0) {
                    debugger;

                    $scope.Paths = [];
                    $scope.RemovedFile = 0;
                    $scope.MDL_ActivityNamesReadOnly = true;
                    $scope.MDL_ActivityNumbersReadOnly = true;

                    $http({
                        method: "get",
                        url: "/AcademicActivities/GetEdittedActivityMenu",
                        params: { paramActivityMasterId: masterIdBeEditted }
                    }).then(function (data) {
                        debugger;
                        if (data.data.FilePath) {
                            $scope.Paths.push(data.data.FilePath);
                        }
                        $scope.MDL_InsertionDate = data.data.InsertionDate;
                        $scope.MDL_ActivityNo = data.data.ActivityNoFormated;
                        $scope.MDL_ActivityMasterId = data.data.ActivityRequestMasterId;
                        $scope.MDL_ActivityConfigId = data.data.ActivityConfigId.toString();
                        $scope.DataSourceActivityMenuBandsGrid = data.data.ActivityMenuBandList;
                    });
                } else {
                    $scope.DataSourceActivityMenuBandsGrid = [];
                    $scope.Paths = [];
                }

                dataSourceStudents.reload();
                dataSourceEvaluations.reload();
                dataSourceActivityNames.reload();
            }
        ]);
})();













 //$scope.RemoveId = function (id) {
                //    $scope.RemovedFiles.push(id);
                //    var index = $scope.Paths.findIndex(i => i.GjhAttachmentId === id);
                //    $scope.Paths.splice(index, 1);
                //};

//$scope.btnAddNewBand = {
//    bindingOptions: {
//    },
//    //text: 'مجموعة',
//    icon: 'plus',
//    type: 'success',
//    validationGroup: "AddActivityMenuForm_vg",
//    useSubmitBehavior: true,
//    onClick: function (e) {
//        
//        //for (let MDL_StudentId of $scope.MDL_StudentIds) {
//        //    
//        //    $scope.DataSourceActivityMenuBandsGrid.push(new ActivityMenuBand(
//        //        MDL_StudentId,
//        //        $scope.StudentSelectBox
//        //              .dataSource._items
//        //              .find(x => x.Value === MDL_StudentId).Text,
//        //        $scope.MDL_EvaluationText,
//        //        $scope.MDL_FacultyName,
//        //        $scope.MDL_DegreeName
//        //    ));
//        //}                     

//    }
//};
//==============
// تاريخ الإدخال 
//==============
//$scope.ActivityInsertionDate = {
//    bindingOptions: {
//        value: "MDL_InsertionDate"
//    },
//    type: "date",
//    readOnly: true,
//    displayFormat: "dd/MM/yyyy",
//    onValueChanged: function (e) {
//    }
//};



//=========
// الكليات 
//=========
//var dataSourceFaculties = new DevExpress.data.DataSource({
//    paginate: true,
//    cacheRawData: true,
//    key: "Value",
//    loadMode: "raw",
//    load: function () {
//        return $.getJSON("/AcademicActivities/GetFaculties", function (data) { });
//    }
//});
//$scope.FacultySelectBox = {
//    dataSource: dataSourceFaculties,
//    bindingOptions: {
//        text: "MDL_FacultyName",
//        value: "MDL_FacultyId",
//        readOnly: "MDL_FacultySelectBox_ReadOnly"
//    },
//    placeholder: "--اختر--",
//    noDataText: "لا يوجد بيانات",
//    displayExpr: "Text",
//    valueExpr: "Value",
//    showBorders: true,
//    rtlEnabled: true,
//    searchEnabled: true,
//    showClearButton: false,
//    onValueChanged: function (e) {

//        dataSourceStudents.reload();
//    }
//};


//=========
// المراحل
//=========
//var dataSourceDegrees = new DevExpress.data.DataSource({
//    paginate: true,
//    cacheRawData: true,
//    key: "Value",
//    loadMode: "raw",
//    load: function () {
//        return $.getJSON("/AcademicActivities/GetDegrees", function (data) { });
//    }
//});
//$scope.DegreeSelectBox = {
//    dataSource: dataSourceDegrees,
//    bindingOptions: {
//        text: "MDL_DegreeName",
//        value: "MDL_DegreeId",
//        readOnly: "MDL_DegreeSelectBox_ReadOnly"
//    },
//    placeholder: "--اختر--",
//    noDataText: "لا يوجد بيانات",
//    displayExpr: "Text",
//    valueExpr: "Value",
//    showBorders: true,
//    rtlEnabled: true,
//    searchEnabled: true,
//    showClearButton: false,
//    onValueChanged: function (e) {

//        dataSourceStudents.reload();
//    }

//};
//var RowIsEditing = false;


                        //if (RowIsEditing === false) {
                        //    $scope.MDL_StudentId = null;
                        //    $scope.MDL_StudentName = null;
                        //}
                        //if ($scope.MDL_FacultyId !== '' && $scope.MDL_DegreeId !== '' &&
                        //    $scope.MDL_FacultyId !== null && $scope.MDL_DegreeId !== null &&
                        //    $scope.MDL_FacultyId !== undefined && $scope.MDL_DegreeId !== undefined) {
                        // return $.getJSON(`/AcademicActivities/GetStudents?facultyId=${$scope.MDL_FacultyId}&degreeId=${$scope.MDL_DegreeId}`, function (data) { });
                        //}
                        //return [];

//showSelectionControls: true,
                    //maxDisplayedTags: 1,
                    //showMultiTagOnly: false,
                    //selectAllMode: "Page",//"allPages",
                    //multiline: false,
                    //showDropDownButton: true,
                    //onOpened: function (e) {
                    //    //$('.dx-list-select-all').hide();  
                    //    //e.component.content().querySelector('.dx-list-select-all').style.display = 'none';  
                    //    dataSourceStudents.reload();
                    //},


                    //if (!($scope.MDL_FacultyId && $scope.MDL_FacultyId !== '')) {
                    //    return swal("تنبيه", "لابد من اختيار الكلية أولا ", "warning");
                    //}

                    //if (!($scope.MDL_DegreeId && $scope.MDL_DegreeId !== '')) {
                    //    return swal("تنبيه", "لابد من اختيار المرحلة أولا ", "warning");
                    //}



                    //btnAddWithPost: {
                    //    text: 'حفظ مع توجيه للإعتماد',
                    //    icon: 'chevrondoubleleft',
                    //    validationGroup: "AddActivityMenuForm_vg",
                    //    useSubmitBehavior: true,
                    //    onClick: function (e) {
                    //        $scope.SaveActivityMenuFunc(true);
                    //    }
                    //},

//,
                    //btnAddNew: {
                    //    text: 'جديد',
                    //    icon: 'refresh',
                    //    type: 'Normal',
                    //    onClick: function (e) {
                    //        window.location.href = "/AcademicActivities/CreateActivityMenus";
                    //    }
                    //}