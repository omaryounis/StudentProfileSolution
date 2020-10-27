/// <reference path="../dx.all.debug.js" />
/// <reference path="../jquery-2.2.3.js" />

(function() {
    app.controller("AdvancesCtrl",
        [
            "$scope", "$http", "$timeout", function($scope, $http, $timeout) {
                var today = new Date();
                var amounts = [
                    100, 150, 200, 250, 300, 350, 400, 450, 500, 550, 600, 650, 700, 750, 800, 850, 900, 950, 1000
                ];

                function init() {
                    $scope.AdvanceValue = 100;
                    $scope.AdvanceDate = today;
                    $scope.AddNewAdvanceTypeShow = false;
                    $scope.AdvanceDueDate =
                        new Date($scope.AdvanceDate.getFullYear(), $scope.AdvanceDate.getMonth() + 1, 1);
                }

                $scope.Permissions = {
                    Create: false,
                    Read: false,
                    Update: false,
                    Delete: false,
                    View: false
                };
                $http({
                    method: "GET",
                    url: "/Advances/GetAdvancePermissions",
                    params: { screenId: 27 } //شاشة إضافة طلب سلفة
                }).then(function(data) {
                    $scope.Permissions.Create = data.data.Create;
                    $scope.Permissions.Read = data.data.Read;
                    $scope.Permissions.Update = data.data.Update;
                    $scope.Permissions.Delete = data.data.Delete;
                    $scope.Permissions.View = data.data.View;
                    $scope.dataGridInstance.columnOption("add", "visible", data.data.Create);
                    $scope.dataGridInstance.columnOption("edit", "visible", data.data.Update);
                    $scope.dataGridInstance.columnOption("delete", "visible", data.data.Delete);
                });
                init();
                var dataSourceNationalities = new DevExpress.data.DataSource({
                    loadMode: "raw",
                    paginate: true,
                    load: function() {
                        return $.getJSON("/Advances/GetNationalities");
                    }
                });
                var dataSourceDegrees = new DevExpress.data.DataSource({
                    loadMode: "raw",
                    paginate: true,
                    load: function() {
                        return $.getJSON("/Advances/GetDegrees");
                    }
                });
                var dataSourceLevels = new DevExpress.data.DataSource({
                    loadMode: "raw",
                    paginate: true,
                    load: function() {
                        return $.getJSON("/Advances/GetLevels");
                    }
                });
                var dataSourceAdvanceType = new DevExpress.data.DataSource({
                    loadMode: "raw",
                    paginate: true,
                    cacheRawData: true,
                    key: "Value",
                    load: function() {
                        return $.getJSON("/Advances/GetAdvancesTypes/");
                    }
                });

                $scope.AdvancesFilter = {
                    Nationality: {
                        dataSource: dataSourceNationalities,
                        bindingOptions: { value: "NATIONALITY_CODE" },
                        displayExpr: "NATIONALITY_DESC",
                        valueExpr: "NATIONALITY_CODE",
                        placeholder: "جنسية الطالب",
                        showClearButton: true,
                        noDataText: "لا يوجد بيانات",
                        searchEnabled: true,
                        rtlEnabled: true,
                        itemTemplate: function(data) {
                            return "<div title='" +
                                data.NATIONALITY_DESC +
                                "' value='" +
                                data.NATIONALITY_CODE +
                                "'>" +
                                data.NATIONALITY_DESC +
                                "</div>";
                        }
                    },
                    Degree: {
                        dataSource: dataSourceDegrees,
                        bindingOptions: { value: "DEGREE_CODE" },
                        displayExpr: "DEGREE_DESC",
                        valueExpr: "DEGREE_CODE",
                        placeholder: "الدرجة العلمية",
                        showClearButton: true,
                        noDataText: "لا يوجد بيانات",
                        searchEnabled: true,
                        rtlEnabled: true,
                        itemTemplate: function(data) {
                            return "<div title='" +
                                data.DEGREE_DESC +
                                "' value='" +
                                data.DEGREE_CODE +
                                "'>" +
                                data.DEGREE_DESC +
                                "</div>";
                        }
                    },
                    Level: {
                        dataSource: dataSourceLevels,
                        bindingOptions: { value: "LEVEL_CODE" },
                        displayExpr: "LEVEL_DESC",
                        valueExpr: "LEVEL_CODE",
                        placeholder: "المستوى الدراسي",
                        showClearButton: true,
                        noDataText: "لا يوجد بيانات",
                        searchEnabled: true,
                        rtlEnabled: true,
                        itemTemplate: function(data) {
                            return "<div title='" +
                                data.LEVEL_DESC +
                                "' value='" +
                                data.LEVEL_CODE +
                                "'>" +
                                data.LEVEL_DESC +
                                "</div>";
                        }
                    },
                    StudentId: {
                        bindingOptions: { value: "AdvancesFilterStudentId" },
                        placeholder: "الرقم الأكاديمي",
                        showClearButton: true,
                        min: 0
                    },
                    NationalId: {
                        bindingOptions: { value: "NationalId" },
                        placeholder: "رقم الهوية",
                        showClearButton: true,
                        min: 0
                    },
                    SearchButtonOptions: {
                        text: "بحث",
                        hint: "بحث",
                        type: "success",
                        icon: "search",
                        useSubmitBehavior: false,
                        onClick: function(e) {
                            //var grid = $("#gridContainer").dxDataGrid("instance");
                            //grid.refresh();
                            $scope.dataGridInstance.refresh();
                        }
                    }
                };

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
                        AdvanceDueDate: {
                            validationGroup: "addAdvance",
                            validationRules: [
                                {
                                    type: "required",
                                    message: "حقل اجبارى"
                                }
                            ]
                        },
                        AdvanceDate: {
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
                    //نوع السلفة
                    AdvanceType:
                    {
                        bindingOptions: { value: "AdvanceTypeId" },
                        dataSource: dataSourceAdvanceType,
                        valueExpr: "Value",
                        displayExpr: "Text",
                        placeholder: "نوع السلفة",
                        showClearButton: true,
                        itemTemplate: function(data) {
                            return "<div title='" + data.Text + "' value='" + data.Value + "'>" + data.Text + "</div>";
                        }
                    },
                    //شهر بداية الخصم
                    AdvanceDueDate:
                    {
                        bindingOptions: { value: "AdvanceDueDate" },
                        placeholder: "شهر بداية الخصم",
                        showClearButton: true,
                        displayFormat: "monthAndYear",
                        maxZoomLevel: "year",
                        minZoomLevel: "century",
                        disabled: true,
                        width: "100%"
                    },
                    //تاريخ الطلب
                    AdvanceDate:
                    {
                        bindingOptions: { value: "AdvanceDate" },
                        placeholder: "تاريخ الطلب",
                        displayFormat: "dd/MM/yyyy",
                        showClearButton: true,
                        min: today,
                        onValueChanged: function(e) {
                            $scope.AdvanceDueDate =
                                new Date($scope.AdvanceDate.getFullYear(), $scope.AdvanceDate.getMonth() + 1, 1);
                        },
                        width: "100%"
                    },
                    //المبلغ
                    AdvanceValue:
                    {
                        items: amounts,
                        bindingOptions: { value: "AdvanceValue" },
                        placeholder: "مبلغ السلفة",
                        showClearButton: true,
                        max: 1000
                    }
                };

                $scope.isUpdate = false;
                $scope.PopupTitle = "";
                $scope.PopUpAddShow = false;
                $scope.Popup = {
                    Options: {
                        bindingOptions: { visible: "PopUpAddShow", title: "PopupTitle" },
                        width: 800,
                        height: 300,
                        deferRendering: false,
                        showTitle: true,
                        dragEnabled: false,
                        contentTemplate: "info",
                        onHiding: function(e) {
                            $scope.AdvanceTypeId = null;
                            $scope.AdvanceValue = 100;
                        }
                    },
                    SaveButton: {
                        bindingOptions: { visible: "!isUpdate" },
                        text: "إضافة",
                        hint: "إضافة",
                        icon: "add",
                        type: "success",
                        validationGroup: "addAdvance",
                        useSubmitBehavior: true,
                        onClick: function(e) {
                            var validationGroup = DevExpress.validationEngine.getGroupConfig("addAdvance");
                            if (validationGroup) {
                                var result = validationGroup.validate();
                                if (result.isValid) {
                                    var advance = {
                                        studentId: $scope.StudentId,
                                        advanceType: $scope.AdvanceTypeId,
                                        advanceDate: $scope.AdvanceDate,
                                        value: $scope.AdvanceValue
                                    };
                                    DevExpress.ui.notify({
                                        message: "جارى الحفظ",
                                        type: "success",
                                        displayTime: 3000,
                                        closeOnClick: true
                                    });
                                    $http({
                                        method: "POST",
                                        url: "/Advances/AddAdvance/",
                                        data: advance
                                    }).then(function(data) {
                                        if (data.data === "") {
                                            DevExpress.ui.notify({
                                                message: "تم الحفظ بنجاح",
                                                type: "success",
                                                displayTime: 3000,
                                                closeOnClick: true
                                            });
                                            //validationGroup.reset();
                                            $timeout(function() {
                                                $scope.PopUpAddShow = false;
                                                $scope.dataGridInstance.refresh();
                                            });
                                            //window.location.href = "/Advances/AdvancesRequestsList";
                                        } else {
                                            DevExpress.ui.notify({
                                                message: data.data,
                                                type: "error",
                                                displayTime: 3000,
                                                closeOnClick: true
                                            });
                                        }
                                    });
                                }
                            }
                        }
                    },
                    UpdateButton: {
                        bindingOptions: { visible: "isUpdate" },
                        text: "تعديل",
                        hint: "تعديل",
                        icon: "edit",
                        type: "default",
                        elementAttr: { "class": "btn-warning" },
                        useSubmitBehavior: true,
                        onClick: function(e) {
                            var validationGroup = DevExpress.validationEngine.getGroupConfig("addAdvance");
                            if (validationGroup) {
                                var result = validationGroup.validate();
                                if (result.isValid) {
                                    var advance = {
                                        advanceRequestId: $scope.AdvanceRequestId,
                                        studentId: $scope.StudentId,
                                        advanceType: $scope.AdvanceTypeId,
                                        advanceDate: $scope.AdvanceDate,
                                        value: $scope.AdvanceValue
                                    };
                                    DevExpress.ui.notify({
                                        message: "جارى الحفظ",
                                        type: "success",
                                        displayTime: 3000,
                                        closeOnClick: true
                                    });
                                    $http({
                                        method: "POST",
                                        url: "/Advances/EditAdvanceRequest/",
                                        data: advance
                                    }).then(function(data) {
                                        if (data.data === "") {
                                            DevExpress.ui.notify({
                                                message: "تم الحفظ بنجاح",
                                                type: "success",
                                                displayTime: 3000,
                                                closeOnClick: true
                                            });
                                            //validationGroup.reset();
                                            $timeout(function() { $scope.PopUpAddShow = false; });
                                            $scope.dataGridInstance.refresh();
                                            //window.location.href = "/Advances/AdvancesRequestsList";
                                        } else {
                                            DevExpress.ui.notify({
                                                message: data.data,
                                                type: "error",
                                                displayTime: 3000,
                                                closeOnClick: true
                                            });
                                        }
                                    });
                                }
                            }
                        }
                    }
                }
                $scope.AddNewAdvanceType = function() {
                    $scope.AddNewAdvanceTypeShow = true;
                }
                $scope.AddNewAdvanceTypePopup = {
                    Options: {
                        bindingOptions: { visible: "AddNewAdvanceTypeShow" },
                        width: 600,
                        height: 222,
                        title: "اضافة نوع سلفة جديد",
                        deferRendering: false,
                        showTitle: true,
                        dragEnabled: false,
                        contentTemplate: "AddNewAdvanceTypePopup",
                        onHiding: function(e) {
                            $scope.AdvanceTypeName = "";
                            $scope.COAID = null;
                            var validationGroup = DevExpress.validationEngine.getGroupConfig("AddNewAdvanceTypePopup");
                            if (validationGroup) {
                                validationGroup.reset();
                            }
                        }
                    },
                    ValidationRules: {
                        AdvanceTypeName: {
                            validationGroup: "AddNewAdvanceTypePopup",
                            validationRules: [
                                {
                                    type: "required",
                                    message: "حقل اجبارى"
                                }
                            ]
                        },
                        COAID: {
                            validationGroup: "AddNewAdvanceTypePopup",
                            validationRules: [
                                {
                                    type: "required",
                                    message: "حقل اجبارى"
                                }
                            ]
                        },
                    },
                    SaveButton: {
                        text: "إضافة",
                        hint: "إضافة",
                        icon: "add",
                        type: "success",
                        validationGroup: "AddNewAdvanceTypePopup",
                        useSubmitBehavior: true,
                        onClick: function(e) {
                            var validationGroup = DevExpress.validationEngine.getGroupConfig("AddNewAdvanceTypePopup");
                            if (validationGroup) {
                                var result = validationGroup.validate();
                                if (result.isValid) {
                                    DevExpress.ui.notify({
                                        message: "جارى الحفظ",
                                        type: "success",
                                        displayTime: 3000,
                                        closeOnClick: true
                                    });
                                    $http({
                                        method: "POST",
                                        url: "/Advances/AddNewAdvanceType/",
                                        data: { typeName: $scope.AdvanceTypeName, coaId: $scope.COAID }
                                    }).then(function(data) {
                                        if (data.data === "") {
                                            DevExpress.ui.notify({
                                                message: "تم الحفظ بنجاح",
                                                type: "success",
                                                displayTime: 3000,
                                                closeOnClick: true
                                            });
                                            dataSourceAdvanceType.reload();
                                            $scope.AddNewAdvanceTypeShow = false;
                                        } else {
                                            DevExpress.ui.notify({
                                                message: data.data,
                                                type: "error",
                                                displayTime: 3000,
                                                closeOnClick: true
                                            });
                                        }
                                    });
                                }
                            }
                        }
                    },
                    AdvanceTypeName: {
                        bindingOptions: { value: "AdvanceTypeName" },
                        placeholder: "اسم النوع",
                        showClearButton: true,
                    },
                    COAID: {
                        dataSource: new DevExpress.data.DataSource({
                            key: "Value",
                            paginate: true,
                            cacheRawData: true,
                            pageSize: 10,
                            loadMode: "raw",
                            load: function(loadOptions) {
                                return $.getJSON("/Config/GetSecondaryAcctounts",
                                    function(data) {
                                    });
                            }
                        }),
                        itemTemplate: function(data) {
                            return "<div title='" + data.Text + "' value='" + data.Value + "'>" + data.Text + "</div>";
                        },
                        displayExpr: "Text",
                        valueExpr: "Value",
                        bindingOptions: { value: "COAID" },
                        placeholder: "حساب التهيئة",
                        showClearButton: true
                    }
                }
                var gridDataSource = new DevExpress.data.DataSource({
                    load: function(loadOptions) {
                        var d = $.Deferred();
                        $.getJSON("/Advances/Search",
                            {
                                Nationality: $scope.NATIONALITY_CODE,
                                Degree: $scope.DEGREE_CODE,
                                Level: $scope.LEVEL_CODE,
                                StudentId: $scope.AdvancesFilterStudentId,
                                NationalId: $scope.NationalId,
                                skip: loadOptions.skip,
                                take: loadOptions.take,
                                sort: loadOptions.sort ? JSON.stringify(loadOptions.sort) : "",
                                filter: loadOptions.filter ? JSON.stringify(loadOptions.filter) : "",
                                requireTotalCount: loadOptions.requireTotalCount,
                                totalSummary: loadOptions.totalSummary ? JSON.stringify(loadOptions.totalSummary) : "",
                                group: loadOptions.group ? JSON.stringify(loadOptions.group) : "",
                                groupSummary: loadOptions.groupSummary ? JSON.stringify(loadOptions.groupSummary) : ""
                            }).done(function(result) {

                            d.resolve(result.data,
                                {
                                    //data: response.data,
                                    totalCount: result.totalCount,
                                    summary: result.summary
                                });
                        });
                        return d.promise();
                    }
                });
                $scope.dataGridOptions = {
                    dataSource: gridDataSource,
                    noDataText: "لا يوجد بيانات",
                    columns: [
                        {
                            dataField: "STUDENT_NAME",
                            caption: "اسم الطالب"
                        },
                        {
                            dataField: "STUDENT_ID",
                            caption: "الرقم الجامعى"
                        },
                        {
                            dataField: "NATIONAL_ID",
                            caption: "رقم الهوية"
                        },
                        {
                            dataField: "DEGREE_DESC",
                            caption: "الدرجة العلمية"
                        },
                        {
                            dataField: "LEVEL_DESC",
                            caption: "المستوى"
                        },
                        {
                            dataField: "GPA",
                            caption: "المعدل التراكمى",
                            format: { type: "fixedPoint", precision: 2 },
                            allowSorting: false
                        },
                        {
                            cssClass: "text-center",
                            caption: "الباقى",
                            hint: "الباقى",
                            allowHiding: false,
                            cellTemplate: function(container, options) {
                                if (options.key.STUDENT_ID) {
                                    $http({
                                        method: "GET",
                                        url: "/Advances/GetRemainingPremiums",
                                        params: { studentId: options.key.STUDENT_ID }
                                    }).then(function(data) {
                                        $(container).text(data.data);
                                    });
                                }
                            },
                            minWidth: "100px",
                            width: "100px",
                            fixed: true
                        },
                        {
                            cssClass: "text-center",
                            name: "add",
                            caption: "اضافة",
                            hint: "اضافة طلب سلفة",
                            allowHiding: false,
                            fixedPosition: 100,
                            minWidth: "100px",
                            width: "100px",
                            fixed: true,
                            cellTemplate: function(container, options) {
                                $("<div />").dxButton({
                                    icon: "plus",
                                    type: "success",
                                    text: "",
                                    hint: "اضافة طلب سلفة",
                                    elementAttr: { "class": "btn-ds-st btn-sm-ds" },
                                    onClick: function(e) {
                                        $scope.isUpdate = false;
                                        $scope.StudentId = options.key.STUDENT_ID;
                                        $scope.PopupTitle = "اضافة سلفة";
                                        $timeout(function() { $scope.PopUpAddShow = true; });
                                    }
                                }).appendTo(container);
                            }
                        },
                        {
                            cssClass: "text-center",
                            name: "edit",
                            caption: "تعديل",
                            hint: "تعديل اخر طلب سلفة",
                            allowHiding: false,
                            fixedPosition: 100,
                            minWidth: "100px",
                            width: "100px",
                            fixed: true,
                            cellTemplate: function(container, options) {
                                if (options.key.STUDENT_ID) {
                                    $http({
                                        method: "GET",
                                        url: "/Advances/GetLatestAdvanceRequestData",
                                        params: { studentId: options.key.STUDENT_ID }
                                    }).then(function(data) {
                                        if (data.data !== "") {
                                            $("<div />").dxButton({
                                                icon: "edit",
                                                type: "default",
                                                text: "",
                                                hint: "تعديل اخر طلب سلفة",
                                                elementAttr: { "class": "btn-ds-st btn-sm-ds" },
                                                onClick: function(e) {
                                                    $timeout(function() {
                                                        $scope.PopUpAddShow = true;
                                                        $scope.isUpdate = true;
                                                        $scope.AdvanceRequestId = data.data.ID;
                                                        $scope.StudentId = options.key.STUDENT_ID;
                                                        $scope.PopupTitle = "تعديل اخر طلب سلفة";
                                                        $scope.AdvanceTypeId = data.data.AdvanceSettingId.toString();
                                                        $scope.AdvanceDate =
                                                            new Date(parseInt(data.data.Date.substr(6)));;
                                                        $scope.AdvanceValue = data.data.AdvanceValue;
                                                    });
                                                }
                                            }).appendTo(container);
                                        }
                                    });
                                }
                            }
                        },
                        {
                            cssClass: "text-center",
                            name: "delete",
                            caption: "حذف",
                            hint: "حذف اخر طلب سلفة",
                            allowHiding: false,
                            fixedPosition: 100,
                            minWidth: "100px",
                            width: "100px",
                            fixed: true,
                            cellTemplate: function(container, options) {
                                if (options.key.STUDENT_ID) {
                                    $http({
                                        method: "GET",
                                        url: "/Advances/CanDeleteLastAdvanceRequest",
                                        params: { studentId: options.key.STUDENT_ID }
                                    }).then(function(data) {

                                        if (data.data === true) {
                                            $("<div />").dxButton({
                                                icon: "trash",
                                                type: "danger",
                                                text: "",
                                                hint: "حذف اخر طلب سلفة",
                                                elementAttr: { "class": "btn-ds-st btn-sm-ds" },
                                                onClick: function(e) {
                                                    var resultt =
                                                        DevExpress.ui.dialog.confirm(
                                                            "هل تريد حذف اخر طلب سلفة لهذا الطالب",
                                                            "تأكيد");
                                                    resultt.done(function(dialogResult) {
                                                        if (dialogResult) {

                                                            $http({
                                                                method: "GET",
                                                                url: "/Advances/DeleteLastAdvanceReuqst",
                                                                params: { studentId: options.key.STUDENT_ID }
                                                            }).then(function(response) {

                                                                if (response.data !== "") {
                                                                    DevExpress.ui.notify({
                                                                        message: response.data,
                                                                        type: "error",
                                                                        displayTime: 3000,
                                                                        closeOnClick: true
                                                                    });
                                                                } else {
                                                                    DevExpress.ui.notify({
                                                                        message: "تم الحذف بنجاح",
                                                                        type: "success",
                                                                        displayTime: 3000,
                                                                        closeOnClick: true
                                                                    });
                                                                    $scope.dataGridInstance.refresh();
                                                                }
                                                            });
                                                        }
                                                    });
                                                }
                                            }).appendTo(container);
                                        }
                                    });
                                }
                            }
                        }
                    ],
                    remoteOperations: {
                        filtering: true,
                        grouping: true,
                        groupPaging: true,
                        paging: true,
                        sorting: true,
                        summary: true
                    },
                    headerFilter: {
                        allowSearch: true,
                        texts: { cancel: "الغاء", emptyValue: "", ok: "Ok" },
                        visible: true
                    },
                    loadPanel: {
                        enabled: "auto",
                        indicatorSrc: "",
                        showIndicator: true,
                        showPane: true,
                        text: "تحميل..."
                    },
                    searchPanel: {
                        visible: true,
                        placeholder: "بحث"
                    },
                    selection: { mode: "single" },
                    showRowLines: true,
                    columnAutoWidth: true,
                    scrolling: {
                        rtlEnabled: true,
                        useNative: true,
                        scrollByContent: true,
                        scrollByThumb: true,
                        showScrollbar: "onHover",
                        mode: "standard", // or "virtual"
                        direction: "both"
                    },
                    paging: { enabled: true, pageSize: 10 },
                    pager: {
                        allowedPageSizes: "auto",
                        infoText: "صفحة {0} من {1} - {2} عنصر",
                        showInfo: true,
                        showNavigationButtons: true,
                        showPageSizeSelector: true,
                        visible: "auto"
                    },
                    showBorders: true,
                    allowColumnReordering: true,
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
                    editing: {
                        allowUpdating: false,
                        allowDeleting: false,
                        texts: {
                            confirmDeleteMessage: "تأكيد حذف  !",
                            deleteRow: "حذف",
                            editRow: "تعديل",
                            addRow: "اضافة"
                        }
                    },
                    onInitialized: function(e) {
                        $scope.dataGridInstance = e.component;
                    }
                };
            }
        ]);
})();