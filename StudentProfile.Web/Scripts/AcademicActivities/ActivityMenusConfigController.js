(function () {
    app.controller("ActivityMenusConfigCtrl", ["$scope", "$http", "AcademicActivitiesSrvc",
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
                url: "/AcademicActivities/GetCreateActivityMenusPermissions?screenId=102"
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


            //=============
            // MenuHandling
            //=============
            const Menu = [
                { key: 2, text: "تهيئة أسماء المشاركات" },
                { key: 3, text: "تهيئة أنواع المشاركات" },
                { key: 4, text: "تهيئة أماكن المشاركات" },
                { key: 1, text: "تهيئة تقييمات المشاركة" }
            ];
            $scope.MenuOptions = {
                dataSource: Menu,
                itemTemplate: function (data) {
                    if (data.key === 1) {
                        return $("<div><i class='icon dx-icon-plus'></i><span> </span>" + data.text + "</div>");
                    }
                    else if (data.key === 2) {
                        return $("<div><i class='icon dx-icon-plus'></i><span> </span>" + data.text + "</div>");
                    }
                    else if (data.key === 3) {
                        return $("<div><i class='icon dx-icon-plus'></i><span> </span>" + data.text + "</div>");
                    }
                    else if (data.key === 4) {
                        return $("<div><i class='icon dx-icon-plus'></i><span> </span>" + data.text + "</div>");
                    }
                },
                onItemClick: function (e) {
                    $scope.selectedItem = e.itemData.key;

                    if ($scope.selectedItem === 1) {
                        if ($scope.Permissions.AddEvaluation === false) {
                            return swal("تنبيه", "عفوا لايوجد لديك صلاحية علي هذه الشاشة", "warning");
                        }
                        $scope.AddActivityEvaluationPopupShow = true;

                    }
                    else if ($scope.selectedItem === 2) {
                        if ($scope.Permissions.AddActivityName === false) {
                            return swal("تنبيه", "عفوا لايوجد لديك صلاحية علي هذه الشاشة", "warning");
                        }
                        $scope.AddActivityNamePopupShow = true;
                       
                    }
                    else if ($scope.selectedItem === 3) {
                        if ($scope.Permissions.AddActivityType === false) {
                            return swal("تنبيه", "عفوا لايوجد لديك صلاحية علي هذه الشاشة", "warning");
                        }
                        $scope.AddActivityTypePopupShow = true;

                    }
                    else if ($scope.selectedItem === 4) {
                        if ($scope.Permissions.AddActivityLocation === false) {
                            return swal("تنبيه", "عفوا لايوجد لديك صلاحية علي هذه الشاشة", "warning");
                        }
                        $scope.AddActivityLocationPopupShow = true;

                    }
                }
            };


            //====================================
            //الشاشة الخاصة بتهيئة مشاركة جديدة
            //====================================
            var now = new Date();

            var dataSourceActivityNames = new DevExpress.data.DataSource({
                paginate: true,
                cacheRawData: true,
                key: "Value",
                loadMode: "raw",
                load: function () {
                    return $.getJSON(`/AcademicActivities/GetActivityConfig?paramKey=activity`, function (data) { });
                }
            });
            var dataSourceActivityTypes = new DevExpress.data.DataSource({
                paginate: true,
                cacheRawData: false,
                key: "Value",
                loadMode: "raw",
                load: function () {
                    return $.getJSON(`/AcademicActivities/GetActivityConfig?paramKey=type`, function (data) { });
                }
            });
            var dataSourceActivityLocations = new DevExpress.data.DataSource({
                paginate: true,
                cacheRawData: false,
                key: "Value",
                loadMode: "raw",
                load: function () {
                    return $.getJSON(`/AcademicActivities/GetActivityConfig?paramKey=location`, function (data) { });
                }
            });
            var dataSourceActivityMasterGrid = new DevExpress.data.DataSource({
                paginate: true,
                cacheRawData: false,
                key: "ID",
                loadMode: "raw",
                load: function () {
                    return $.getJSON("/AcademicActivities/GetActivityMasterDataSource", function (data) { });
                }
            });

            $scope.AddActivityMaster = {               
                ValidationRules: {
                    Required: {
                        validationGroup: "AddActivityMasterPopup_vg",
                        validationRules: [
                            {
                                type: "required",
                                message: "حقل اجبارى"
                            }
                        ]
                    }
                },
                HoursNo: {
                    bindingOptions: { value: "MDL_HoursNo" },
                    showSpinButtons: true,
                    showClearButton: true,
                    rtlEnabled: true,
                    min: 1,
                    max: 100
                },
                ActivityNo: {
                    bindingOptions: { value: "MDL_ActivityNo" },
                    readOnly: true
                },
                ActivityLocations: {
                    dataSource: dataSourceActivityLocations,
                    bindingOptions: {
                        value: "MDL_ActivityLocationId"
                    },
                    placeholder: "--اختر--",
                    noDataText: "لا يوجد بيانات",
                    displayExpr: "Text",
                    valueExpr: "Value",
                    showBorders: true,
                    searchEnabled: true,
                    rtlEnabled: true,
                    showClearButton: true,
                    onOpened: function (e) {
                        debugger;
                        dataSourceActivityLocations.reload();
                    }
                },                
                ActivityStartDate: {
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
                ActivityEndDate: {
                    bindingOptions: {
                        value: "MDL_ActivityEndDate",
                        min: "MDL_ActivityStartDate"
                    },
                    type: "date",
                    max: now,
                    displayFormat: "dd/MM/yyyy",
                    onValueChanged: function (e) {
                        $scope.MDL_ActivityEndDate = e.value;
                    }
                },
                ActivityNames: {
                    dataSource: dataSourceActivityNames,
                    bindingOptions: {
                        value: "MDL_ActivityNameId",
                        readOnly: "MDL_ActivityName_ReadOnly"
                    },
                    placeholder: "--اختر--",
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
                        if (!$scope.ActivityMasterPopupIsEditing) {
                            $scope.MDL_ActivityNo = '';
                            if (e.value) {
                                $http({
                                    method: "get",
                                    url: "/AcademicActivities/GetNextActivityNo",
                                    params: {
                                        ParamConfigId: e.value
                                    }
                                }).then(function (data) {
                                    $scope.MDL_ActivityNo = data.data;
                                });
                            }
                        }
                    }

                }, 
                ActivityTypes: {
                    dataSource: dataSourceActivityTypes,
                    bindingOptions: {
                        value: "MDL_ActivityTypeId"
                    },
                    placeholder: "--اختر--",
                    noDataText: "لا يوجد بيانات",
                    displayExpr: "Text",
                    valueExpr: "Value",
                    showBorders: true,
                    searchEnabled: true,
                    rtlEnabled: true,
                    showClearButton: true,
                    onOpened: function (e) {
                        debugger;
                        dataSourceActivityTypes.reload();
                    }
                },
                SaveButton: {
                    text: "حفظ المشاركة",
                    hint: "إضافة",
                    type: "success",
                    validationGroup: "AddActivityMasterPopup_vg",
                    useSubmitBehavior: true,
                    onClick: function (e) {

                        var validationGroup = DevExpress.validationEngine.getGroupConfig("AddActivityMasterPopup_vg");
                        if (validationGroup) {
                            var result = validationGroup.validate();
                            if (result.isValid) {

                                if (new Date($scope.MDL_ActivityEndDate) < new Date($scope.MDL_ActivityStartDate)) {
                                    return DevExpress.ui.notify({ message: "لا يمكن أن يكون تاريخ بداية المشاركة بعد تاريخ نهاية المشاركة" }, "Error", 10000);
                                }

                                $http({
                                    method: "POST",
                                    url: "/AcademicActivities/SaveActivityMaster",
                                    data: {
                                        paramHoursNo: $scope.MDL_HoursNo,
                                        paramActivityNameId: $scope.MDL_ActivityNameId,
                                        paramActivityTypeId: $scope.MDL_ActivityTypeId,
                                        paramActivityEndDate: $scope.MDL_ActivityEndDate,
                                        paramActivityMasterId: $scope.MDL_ActivityMasterId,
                                        paramActivityStartDate: $scope.MDL_ActivityStartDate,
                                        paramActivityLocationId: $scope.MDL_ActivityLocationId
                                    }
                                }).then(function (data) {
                                    if (data.data === "") {
                                        swal("Done!", "تم الحفظ بنجاح", "success");

                                        // Reset Controls...
                                        $scope.MDL_ActivityName_ReadOnly = false;
                                        $scope.ActivityMasterPopupIsEditing = false;

                                        $scope.MDL_HoursNo = '';
                                        $scope.MDL_ActivityNameId = '';
                                        $scope.MDL_ActivityTypeId = '';
                                        $scope.MDL_ActivityEndDate = '';
                                        $scope.MDL_ActivityMasterId = '';
                                        $scope.MDL_ActivityStartDate = '';
                                        $scope.MDL_ActivityLocationId = '';

                                        // Reset Validations...
                                        validationGroup.reset();

                                        dataSourceActivityMasterGrid.reload();


                                        if (IsEditing !== true) {
                                            dataSourceActivityNames.reload();
                                        }

                                        $scope.ActivityMasterGridInstance.refresh();
                                    } else {
                                        swal("حدث خطأ", data.data, "error");
                                    }
                                });
                            }
                        }
                    }
                },
                ActivityMasterGrid: {
                    dataSource: dataSourceActivityMasterGrid,
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
                        width: 300
                    },
                    paging: {
                        pageSize: 4
                    },
                    pager: {
                        allowedPageSizes: "auto",
                        infoText: "(صفحة  {0} من {1} ({2} عنصر",
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
                        visible: true
                    },
                    showRowLines: true,
                    groupPanel: {
                        visible: false,
                        emptyPanelText: "اسحب عمود هنا"
                    },
                    noDataText: "",
                    columnAutoWidth: true,
                    width: '100%',
                    columnChooser: {
                        enabled: false
                    },
                    "export": {
                        enabled: false,
                        fileName: "المشاركات التي تم تهيئتها"
                    },
                    onCellPrepared: function (e) {
                        if (e.rowType === "header" && e.column.command === "edit") {
                            e.column.width = 55;
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
                            confirmDeleteMessage: "تأكيد حذف هذه المشاركة ؟",
                            deleteRow: "",
                            editRow: "",
                            addRow: ""
                        },
                        useIcons: true
                    },
                    columns: [
                        {
                            caption: "مكان المشاركة",
                            dataField: "ActivityLocation",
                            alignment: "right"
                        },
                        {
                            caption: "اسم المشاركة",
                            dataField: "ActivityName",
                            alignment: "right"
                        },
                        {
                            caption: "النوع",
                            dataField: "ActivityType",
                            alignment: "right"
                        },                        
                        {
                            caption: "عدد الساعات",
                            dataField: "HoursNo",
                            alignment: "right"
                        },
                        {
                            caption: "رقم المشاركة",
                            dataField: "ActivityNoFormated",
                            alignment: "right"
                        },
                        {
                            caption: "من تاريخ",
                            dataField: "StartDate",
                            alignment: "right",
                            dataType: "date"
                        },
                        {
                            caption: "حتي تاريخ",
                            dataField: "EndDate",
                            alignment: "right",
                            dataType: "date"
                        }
                    ],
                    allowColumnResizing: true,
                    columnResizingMode: "widget",
                    allowColumnReordering: true,
                    showColumnLines: true,
                    rowAlternationEnabled: true,
                    onInitialized: function (e) {
                        $scope.ActivityMasterGridInstance = e.component;
                    },
                    onRowRemoving: function (e) {
                        e.cancel = true;
                        $http({
                            method: "Delete",
                            url: "/AcademicActivities/DeleteActivityMaster",
                            data: { paramActivityMasterId: e.data.ID }
                        }).then(function (data) {
                            if (data.data !== "") {
                                swal("حدث خطأ", data.data, "error");
                            } else {
                                dataSourceActivityMasterGrid.reload();
                                swal("Done!", "تم الحذف بنجاح", "success");
                            }
                        });
                    },
                    onEditingStart: function (e) {
                        e.cancel = true;

                        $http({
                            method: "Get",
                            url: "/AcademicActivities/GetActivityMasterById",
                            params: { paramActivityMasterId: e.data.ID }
                        }).then(function (data) {

                            $scope.MDL_ActivityName_ReadOnly = true;
                            $scope.ActivityMasterPopupIsEditing = true;

                            $scope.MDL_HoursNo = data.data.HoursNo;
                            $scope.MDL_ActivityMasterId = data.data.ID;
                            $scope.MDL_ActivityNo = data.data.ActivityNoFormated;
                            $scope.MDL_ActivityEndDate = new Date(data.data.EndDate);
                            $scope.MDL_ActivityStartDate = new Date(data.data.StartDate);
                            $scope.MDL_ActivityNameId = data.data.ActivityNameID.toString();
                            $scope.MDL_ActivityTypeId = data.data.ActivityTypeID.toString();
                            $scope.MDL_ActivityLocationId = data.data.ActivityLocationID.toString();
                        });
                    },
                    onContentReady: function (e) {
                        e.component.columnOption("command:edit", "visible", $scope.Permissions.DeleteActivityMaster);
                        e.component.columnOption("command:edit", "width", 55);
                    }
                }
            };

            // الشاشة الخاصة بإضافة أسماء مشاركات                 
            var dataSourceActivityNamesGrid = new DevExpress.data.DataSource({
                paginate: true,
                cacheRawData: false,
                key: "ConfigId",
                loadMode: "raw",
                load: function () {
                    return $.getJSON(`/AcademicActivities/GetActivityConfigDataSource?paramKey=activity`, function (data) { });
                }
            });
            $scope.AddActivityNamePopup = {
                Options: {
                    bindingOptions: { visible: "AddActivityNamePopupShow" },
                    width: 800,
                    height: 600,
                    deferRendering: false,
                    showTitle: true,
                    dragEnabled: false,
                    closeOnOutsideClick: false,
                    title: "إضافة أسماء مشاركات",
                    shadingColor: "rgba(0, 0, 0, 0.69)",
                    contentTemplate: "AddActivityNamePopupContent",
                    onHiding: function (e) {

                        // Reset Controls...
                        $scope.MDL_ConfigId = '';
                        $scope.MDL_ActivityNameNotes = '';
                        $scope.MDL_ActivityNameNotes = '';

                        var validationGroup = DevExpress.validationEngine.getGroupConfig("AddActivityNamePopup_vg");
                        if (validationGroup) {
                            validationGroup.reset();
                        }

                    }
                },
                ValidationRules: {
                    Required: {
                        validationGroup: "AddActivityNamePopup_vg",
                        validationRules: [
                            {
                                type: "required",
                                message: "حقل اجبارى"
                            }
                        ]
                    }
                },
                ActivityName: {
                    bindingOptions: { value: "MDL_ActivityName" },
                    placeholder: "",
                    rtlEnabled: true
                },
                ActivityNameNotes: {
                    bindingOptions: { value: "MDL_ActivityNameNotes" },
                    placeholder: "",
                    rtlEnabled: true,
                    onValueChanged: function (e) {
                        $scope.MDL_ActivityNameNotes = e.value;
                    }
                },
                SaveButton: {
                    text: "حفظ",
                    hint: "إضافة",
                    type: "success",
                    validationGroup: "AddActivityNamePopup_vg",
                    useSubmitBehavior: true,
                    onClick: function (e) {

                        var validationGroup = DevExpress.validationEngine.getGroupConfig("AddActivityNamePopup_vg");
                        if (validationGroup) {
                            var result = validationGroup.validate();
                            if (result.isValid) {

                                $http({
                                    method: "POST",
                                    url: "/AcademicActivities/SaveActivityConfig",
                                    data: {
                                        paramKey: 'activity',
                                        paramId: $scope.MDL_ConfigId,
                                        paramName: $scope.MDL_ActivityName,
                                        paramNotes: $scope.MDL_ActivityNameNotes
                                    }
                                }).then(function (data) {
                                    if (data.data === "") {
                                        swal("Done!", "تم الحفظ بنجاح", "success");

                                        // Reset Controls...
                                        $scope.MDL_ConfigId = '';
                                        $scope.MDL_ActivityName = '';
                                        $scope.MDL_ActivityNameNotes = '';

                                        // Reset Validations...
                                        validationGroup.reset();
                                        dataSourceActivityNamesGrid.reload();
                                        $scope.ActivityNamesGridInstance.refresh();
                                    } else {
                                        swal("حدث خطأ", data.data, "error");
                                    }
                                });
                            }
                        }
                    }
                },
                ActivityNamesGrid: {
                    dataSource: dataSourceActivityNamesGrid,
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
                        width: 300
                    },
                    paging: {
                        pageSize: 4
                    },
                    pager: {
                        allowedPageSizes: "auto",
                        infoText: "(صفحة  {0} من {1} ({2} عنصر",
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
                        visible: true
                    },
                    showRowLines: true,
                    groupPanel: {
                        visible: false,
                        emptyPanelText: "اسحب عمود هنا"
                    },
                    noDataText: "",
                    columnAutoWidth: true,
                    width: '100%',
                    columnChooser: {
                        enabled: false
                    },
                    "export": {
                        enabled: false,
                        fileName: "أسماء المشاركات"
                    },
                    onCellPrepared: function (e) {
                        if (e.rowType === "header" && e.column.command === "edit") {
                            e.column.width = 100;
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
                            confirmDeleteMessage: "تأكيد حذف اسم المشاركة؟",
                            deleteRow: "",
                            editRow: "",
                            addRow: ""
                        },
                        useIcons: true
                    },
                    columns: [
                        {
                            caption: "اسم المشاركة",
                            dataField: "Name",
                            alignment: "right"
                        },
                        {
                            caption: "ملاحظات",
                            dataField: "Notes",
                            alignment: "right"
                        }
                    ],
                    allowColumnResizing: true,
                    columnResizingMode: "widget",
                    allowColumnReordering: true,
                    showColumnLines: true,
                    rowAlternationEnabled: true,
                    onInitialized: function (e) {
                        $scope.ActivityNamesGridInstance = e.component;
                    },
                    onRowRemoving: function (e) {
                        e.cancel = true;
                        $http({
                            method: "Delete",
                            url: "/AcademicActivities/DeleteActivityConfig",
                            data: {
                                paramActivityConfigId: e.data.ConfigId
                            }
                        }).then(function (data) {
                            if (data.data !== "") {
                                swal("حدث خطأ", data.data, "error");
                            } else {
                                dataSourceActivityNamesGrid.reload();
                                swal("Done!", "تم الحذف بنجاح", "success");
                            }
                        });
                    },
                    onEditingStart: function (e) {
                        e.cancel = true;

                        $http({
                            method: "Get",
                            url: "/AcademicActivities/GetActivityConfigById",
                            params: { paramActivityConfigId: e.data.ConfigId }
                        }).then(function (data) {

                            $scope.MDL_ConfigId = e.data.ConfigId;
                            $scope.MDL_ActivityName = data.data.Name;
                            $scope.MDL_ActivityNameNotes = data.data.Notes;
                        });
                    },
                    onContentReady: function (e) {
                        e.component.columnOption("command:edit", "visible", $scope.Permissions.DeleteActivityName);
                        e.component.columnOption("command:edit", "width", 100);
                    }
                }
            };


            // الشاشة الخاصة بإضافة أنواع مشاركات
            var dataSourceActivityTypesGrid = new DevExpress.data.DataSource({
                paginate: true,
                cacheRawData: false,
                key: "ConfigId",
                loadMode: "raw",
                load: function () {
                    return $.getJSON(`/AcademicActivities/GetActivityConfigDataSource?paramKey=type`, function (data) { });
                }
            });
            $scope.AddActivityTypePopup = {
                Options: {
                    bindingOptions: { visible: "AddActivityTypePopupShow" },
                    width: 800,
                    height: 600,
                    deferRendering: false,
                    showTitle: true,
                    dragEnabled: false,
                    closeOnOutsideClick: false,
                    title: "إضافة أنواع المشاركات",
                    shadingColor: "rgba(0, 0, 0, 0.69)",
                    contentTemplate: "AddActivityTypePopupContent",
                    onHiding: function (e) {

                        // Reset Controls...                           
                        $scope.MDL_ConfigId = '';
                        $scope.MDL_ActivityTypeName = '';
                        $scope.MDL_ActivityTypeNotes = '';

                        var validationGroup = DevExpress.validationEngine.getGroupConfig("AddActivityTypePopup_vg");
                        if (validationGroup) {
                            validationGroup.reset();
                        }

                    }
                },
                ValidationRules: {
                    Required: {
                        validationGroup: "AddActivityTypePopup_vg",
                        validationRules: [
                            {
                                type: "required",
                                message: "حقل اجبارى"
                            }
                        ]
                    }
                },
                ActivityTypeName: {
                    bindingOptions: { value: "MDL_ActivityTypeName" },
                    placeholder: "",
                    rtlEnabled: true
                },
                ActivityTypeNotes: {
                    bindingOptions: { value: "MDL_ActivityTypeNotes" },
                    placeholder: "",
                    rtlEnabled: true,
                    onValueChanged: function (e) {
                        $scope.MDL_ActivityTypeNotes = e.value;
                    }
                },
                SaveButton: {
                    text: "حفظ",
                    hint: "إضافة",
                    type: "success",
                    validationGroup: "AddActivityTypePopup_vg",
                    useSubmitBehavior: true,
                    onClick: function (e) {

                        var validationGroup = DevExpress.validationEngine.getGroupConfig("AddActivityTypePopup_vg");
                        if (validationGroup) {
                            var result = validationGroup.validate();
                            if (result.isValid) {

                                $http({
                                    method: "POST",
                                    url: "/AcademicActivities/SaveActivityConfig",
                                    data: {
                                        paramKey: 'type',
                                        paramId: $scope.MDL_ConfigId,
                                        paramName: $scope.MDL_ActivityTypeName,
                                        paramNotes: $scope.MDL_ActivityTypeNotes
                                    }
                                }).then(function (data) {
                                    if (data.data === "") {
                                        swal("Done!", "تم الحفظ بنجاح", "success");

                                        // Reset Controls...
                                        $scope.MDL_ConfigId = '';
                                        $scope.MDL_ActivityTypeName = '';
                                        $scope.MDL_ActivityTypeNotes = '';

                                        // Reset Validations...
                                        validationGroup.reset();
                                        dataSourceActivityTypesGrid.reload();
                                        $scope.ActivityTypesGridInstance.refresh();
                                    } else {
                                        swal("حدث خطأ", data.data, "error");
                                    }
                                });
                            }
                        }
                    }
                },
                ActivityTypesGrid: {
                    dataSource: dataSourceActivityTypesGrid,
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
                        width: 300
                    },
                    paging: {
                        pageSize: 4
                    },
                    pager: {
                        allowedPageSizes: "auto",
                        infoText: "(صفحة  {0} من {1} ({2} عنصر",
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
                        visible: true
                    },
                    showRowLines: true,
                    groupPanel: {
                        visible: false,
                        emptyPanelText: "اسحب عمود هنا"
                    },
                    noDataText: "",
                    columnAutoWidth: true,
                    width: '100%',
                    columnChooser: {
                        enabled: false
                    },
                    "export": {
                        enabled: false,
                        fileName: "أنواع المشاركات"
                    },
                    onCellPrepared: function (e) {
                        if (e.rowType === "header" && e.column.command === "edit") {
                            e.column.width = 100;
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
                            confirmDeleteMessage: "تأكيد حذف هذا النوع ؟",
                            deleteRow: "",
                            editRow: "",
                            addRow: ""
                        },
                        useIcons: true
                    },
                    columns: [
                        {
                            caption: "نوع المشاركة",
                            dataField: "Name",
                            alignment: "right"
                        },
                        {
                            caption: "ملاحظات",
                            dataField: "Notes",
                            alignment: "right"
                        }
                    ],
                    allowColumnResizing: true,
                    columnResizingMode: "widget",
                    allowColumnReordering: true,
                    showColumnLines: true,
                    rowAlternationEnabled: true,
                    onInitialized: function (e) {
                        $scope.ActivityTypesGridInstance = e.component;
                    },
                    onRowRemoving: function (e) {
                        e.cancel = true;
                        $http({
                            method: "Delete",
                            url: "/AcademicActivities/DeleteActivityConfig",
                            data: {
                                paramActivityConfigId: e.data.ConfigId
                            }
                        }).then(function (data) {
                            if (data.data !== "") {
                                swal("حدث خطأ", data.data, "error");
                            } else {
                                dataSourceActivityTypesGrid.reload();
                                swal("Done!", "تم الحذف بنجاح", "success");
                            }
                        });
                    },
                    onEditingStart: function (e) {
                        e.cancel = true;

                        $http({
                            method: "Get",
                            url: "/AcademicActivities/GetActivityConfigById",
                            params: { paramActivityConfigId: e.data.ConfigId }
                        }).then(function (data) {

                            $scope.MDL_ConfigId = e.data.ConfigId;
                            $scope.MDL_ActivityTypeName = data.data.Name;
                            $scope.MDL_ActivityTypeNotes = data.data.Notes;
                        });
                    },
                    onContentReady: function (e) {
                        e.component.columnOption("command:edit", "visible", $scope.Permissions.DeleteActivityType);
                        e.component.columnOption("command:edit", "width", 100);
                    }
                }
            };


            // الشاشة الخاصة بإضافة أماكن المشاركات
            var dataSourceActivityLocationsGrid = new DevExpress.data.DataSource({
                paginate: true,
                cacheRawData: false,
                key: "ConfigId",
                loadMode: "raw",
                load: function () {
                    return $.getJSON(`/AcademicActivities/GetActivityConfigDataSource?paramKey=location`, function (data) { });
                }
            });
            $scope.AddActivityLocationPopup = {
                Options: {
                    bindingOptions: { visible: "AddActivityLocationPopupShow" },
                    width: 800,
                    height: 600,
                    deferRendering: false,
                    showTitle: true,
                    dragEnabled: false,
                    closeOnOutsideClick: false,
                    title: "إضافة أماكن المشاركات",
                    shadingColor: "rgba(0, 0, 0, 0.69)",
                    contentTemplate: "AddActivityLocationPopupContent",
                    onHiding: function (e) {
                        // Reset Controls...                           
                        $scope.MDL_ConfigId = '';
                        $scope.MDL_ActivityLocationName = '';
                        $scope.MDL_ActivityLocationNotes = '';

                        var validationGroup = DevExpress.validationEngine.getGroupConfig("AddActivityLocationPopup_vg");
                        if (validationGroup) {
                            validationGroup.reset();
                        }
                    }
                },
                ValidationRules: {
                    Required: {
                        validationGroup: "AddActivityLocationPopup_vg",
                        validationRules: [
                            {
                                type: "required",
                                message: "حقل اجبارى"
                            }
                        ]
                    }
                },
                ActivityLocationName: {
                    bindingOptions: { value: "MDL_ActivityLocationName" },
                    placeholder: "",
                    rtlEnabled: true
                },
                ActivityLocationNotes: {
                    bindingOptions: { value: "MDL_ActivityLocationNotes" },
                    placeholder: "",
                    rtlEnabled: true,
                    onValueChanged: function (e) {
                        $scope.MDL_ActivityLocationNotes = e.value;
                    }
                },
                SaveButton: {
                    text: "حفظ",
                    hint: "إضافة",
                    type: "success",
                    validationGroup: "AddActivityLocationPopup_vg",
                    useSubmitBehavior: true,
                    onClick: function (e) {

                        var validationGroup = DevExpress.validationEngine.getGroupConfig("AddActivityLocationPopup_vg");
                        if (validationGroup) {
                            var result = validationGroup.validate();
                            if (result.isValid) {

                                $http({
                                    method: "POST",
                                    url: "/AcademicActivities/SaveActivityConfig",
                                    data: {
                                        paramKey: 'location',
                                        paramId: $scope.MDL_ConfigId,
                                        paramName: $scope.MDL_ActivityLocationName,
                                        paramNotes: $scope.MDL_ActivityLocationNotes
                                    }
                                }).then(function (data) {
                                    if (data.data === "") {
                                        swal("Done!", "تم الحفظ بنجاح", "success");

                                        // Reset Controls...
                                        $scope.MDL_ConfigId = '';
                                        $scope.MDL_ActivityLocationName = '';
                                        $scope.MDL_ActivityLocationNotes = '';

                                        // Reset Validations...
                                        validationGroup.reset();
                                        dataSourceActivityTypesGrid.reload();
                                        $scope.ActivityLocationsGridInstance.refresh();
                                    } else {
                                        swal("حدث خطأ", data.data, "error");
                                    }
                                });
                            }
                        }
                    }
                },
                ActivityLocationsGrid: {
                    dataSource: dataSourceActivityLocationsGrid,
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
                        width: 300
                    },
                    paging: {
                        pageSize: 4
                    },
                    pager: {
                        allowedPageSizes: "auto",
                        infoText: "(صفحة  {0} من {1} ({2} عنصر",
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
                        visible: true
                    },
                    showRowLines: true,
                    groupPanel: {
                        visible: false,
                        emptyPanelText: "اسحب عمود هنا"
                    },
                    noDataText: "",
                    columnAutoWidth: true,
                    width: '100%',
                    columnChooser: {
                        enabled: false
                    },
                    "export": {
                        enabled: false,
                        fileName: "أماكن المشاركات"
                    },
                    onCellPrepared: function (e) {
                        if (e.rowType === "header" && e.column.command === "edit") {
                            e.column.width = 100;
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
                            confirmDeleteMessage: "تأكيد حذف هذا المكان ؟",
                            deleteRow: "",
                            editRow: "",
                            addRow: ""
                        },
                        useIcons: true
                    },
                    columns: [
                        {
                            caption: "مكان المشاركة",
                            dataField: "Name",
                            alignment: "right"
                        },
                        {
                            caption: "ملاحظات",
                            dataField: "Notes",
                            alignment: "right"
                        }
                    ],
                    allowColumnResizing: true,
                    columnResizingMode: "widget",
                    allowColumnReordering: true,
                    showColumnLines: true,
                    rowAlternationEnabled: true,
                    onInitialized: function (e) {
                        $scope.ActivityLocationsGridInstance = e.component;
                    },
                    onRowRemoving: function (e) {
                        e.cancel = true;
                        $http({
                            method: "Delete",
                            url: "/AcademicActivities/DeleteActivityConfig",
                            data: {
                                paramActivityConfigId: e.data.ConfigId
                            }
                        }).then(function (data) {
                            if (data.data !== "") {
                                swal("حدث خطأ", data.data, "error");
                            } else {
                                dataSourceActivityLocationsGrid.reload();
                                swal("Done!", "تم الحذف بنجاح", "success");
                            }
                        });
                    },
                    onEditingStart: function (e) {
                        e.cancel = true;

                        $http({
                            method: "Get",
                            url: "/AcademicActivities/GetActivityConfigById",
                            params: { paramActivityConfigId: e.data.ConfigId }
                        }).then(function (data) {

                            $scope.MDL_ConfigId = e.data.ConfigId;
                            $scope.MDL_ActivityLocationName = data.data.Name;
                            $scope.MDL_ActivityLocationNotes = data.data.Notes;
                        });
                    },
                    onContentReady: function (e) {
                        e.component.columnOption("command:edit", "visible", $scope.Permissions.DeleteActivityLocation);
                        e.component.columnOption("command:edit", "width", 100);
                    }
                }
            };


            // الشاشة الخاصة بإضافة تقييمات
            var dataSourceActivityEvaluationsGrid = new DevExpress.data.DataSource({
                paginate: true,
                cacheRawData: false,
                key: "ConfigId",
                loadMode: "raw",
                load: function () {
                    return $.getJSON(`/AcademicActivities/GetActivityConfigDataSource?paramKey=evaluation`, function (data) { });
                }
            });
            $scope.AddActivityEvaluationPopup = {
                Options: {
                    bindingOptions: { visible: "AddActivityEvaluationPopupShow" },
                    width: 800,
                    height: 600,
                    deferRendering: false,
                    showTitle: true,
                    dragEnabled: false,
                    closeOnOutsideClick: false,
                    title: "إضافة تقييمات المشاركات",
                    shadingColor: "rgba(0, 0, 0, 0.69)",
                    contentTemplate: "AddActivityEvaluationPopupContent",
                    onHiding: function (e) {

                        // Reset Controls... 
                        $scope.MDL_ConfigId = '';
                        $scope.MDL_ActivityEvaluationName = '';
                        $scope.MDL_ActivityEvaluationPercent = '';

                        var validationGroup = DevExpress.validationEngine.getGroupConfig("AddActivityEvaluationPopup_vg");
                        if (validationGroup) {
                            validationGroup.reset();
                        }
                    }
                },
                ValidationRules: {
                    Required: {
                        validationGroup: "AddActivityEvaluationPopup_vg",
                        validationRules: [
                            {
                                type: "required",
                                message: "حقل اجبارى"
                            }
                        ]
                    }
                },
                ActivityEvaluationName: {
                    bindingOptions: { value: "MDL_ActivityEvaluationName" },
                    placeholder: "",
                    rtlEnabled: true
                },
                ActivityEvaluationPercent: {
                    bindingOptions: { value: "MDL_ActivityEvaluationPercent" },
                    placeholder: "",
                    rtlEnabled: true,
                    min: 1,
                    onValueChanged: function (e) {
                        $scope.MDL_ActivityEvaluationPercent = e.value;
                    }
                },
                SaveButton: {
                    text: "حفظ",
                    hint: "إضافة",
                    type: "success",
                    validationGroup: "AddActivityEvaluationPopup_vg",
                    useSubmitBehavior: true,
                    onClick: function (e) {

                        var validationGroup = DevExpress.validationEngine.getGroupConfig("AddActivityEvaluationPopup_vg");
                        if (validationGroup) {
                            var result = validationGroup.validate();
                            if (result.isValid) {

                                $http({
                                    method: "POST",
                                    url: "/AcademicActivities/SaveActivityConfig",
                                    data: {
                                        paramKey: 'evaluation',
                                        paramId: $scope.MDL_ConfigId,
                                        paramName: $scope.MDL_ActivityEvaluationName,
                                        paramNotes: $scope.MDL_ActivityEvaluationPercent
                                    }
                                }).then(function (data) {
                                    if (data.data === "") {
                                        swal("Done!", "تم الحفظ بنجاح", "success");

                                        // Reset Controls...
                                        $scope.MDL_ConfigId = '';
                                        $scope.MDL_ActivityEvaluationName = '';
                                        $scope.MDL_ActivityEvaluationPercent = '';


                                        // Reset Validations...
                                        validationGroup.reset();
                                        dataSourceActivityEvaluationsGrid.reload();
                                        $scope.ActivityEvaluationsGridInstance.refresh();
                                    } else {
                                        swal("حدث خطأ", data.data, "error");
                                    }
                                });
                            }
                        }
                    }
                },
                ActivityEvaluationsGrid: {
                    dataSource: dataSourceActivityEvaluationsGrid,
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
                        width: 300
                    },
                    paging: {
                        pageSize: 4
                    },
                    pager: {
                        allowedPageSizes: "auto",
                        infoText: "(صفحة  {0} من {1} ({2} عنصر",
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
                        visible: true
                    },
                    showRowLines: true,
                    groupPanel: {
                        visible: false,
                        emptyPanelText: "اسحب عمود هنا"
                    },
                    noDataText: "",
                    columnAutoWidth: true,
                    width: '100%',
                    columnChooser: {
                        enabled: false
                    },
                    "export": {
                        enabled: false,
                        fileName: "تقييمات المشاركات"
                    },
                    onCellPrepared: function (e) {
                        if (e.rowType === "header" && e.column.command === "edit") {
                            e.column.width = 100;
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
                            confirmDeleteMessage: "تأكيد حذف هذا التقييم ؟",
                            deleteRow: "",
                            editRow: "",
                            addRow: ""
                        },
                        useIcons: true
                    },
                    columns: [
                        {
                            caption: "التقييم",
                            dataField: "Name",
                            alignment: "right"
                        },
                        {
                            caption: "النسبة",
                            dataField: "Notes",
                            alignment: "right"
                        }
                    ],
                    allowColumnResizing: true,
                    columnResizingMode: "widget",
                    allowColumnReordering: true,
                    showColumnLines: true,
                    rowAlternationEnabled: true,
                    onInitialized: function (e) {
                        $scope.ActivityEvaluationsGridInstance = e.component;
                    },
                    onRowRemoving: function (e) {
                        e.cancel = true;
                        $http({
                            method: "Delete",
                            url: "/AcademicActivities/DeleteActivityConfig",
                            data: {
                                paramActivityConfigId: e.data.ConfigId
                            }
                        }).then(function (data) {
                            if (data.data !== "") {
                                swal("حدث خطأ", data.data, "error");
                            } else {
                                dataSourceActivityEvaluationsGrid.reload();
                                swal("Done!", "تم الحذف بنجاح", "success");
                            }
                        });
                    },
                    onEditingStart: function (e) {
                        e.cancel = true;

                        $http({
                            method: "Get",
                            url: "/AcademicActivities/GetActivityConfigById",
                            params: { paramActivityConfigId: e.data.ConfigId }
                        }).then(function (data) {

                            $scope.MDL_ConfigId = e.data.ConfigId;
                            $scope.MDL_ActivityEvaluationName = data.data.Name;
                            $scope.MDL_ActivityEvaluationPercent = data.data.Notes;
                        });
                    },
                    onContentReady: function (e) {
                        e.component.columnOption("command:edit", "visible", $scope.Permissions.DeleteEvaluation);
                        e.component.columnOption("command:edit", "width", 100);
                    }
                }
            };

        }
    ]);
})();
