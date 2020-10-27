/// <reference path="../dx.web.debug.js" />
/// <reference path="../jquery-2.2.3.js" />
(function() {
        app.controller("SubsidiesCtrl",
            [
                "$scope", "$http", "$timeout", function($scope, $http, $timeout) {
                    var today = new Date();
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
                        params: { screenId: 31 } //شاشة إضافة طلب اعانة
                    }).then(function(data) {
                        $scope.Permissions.Create = data.data.Create;
                        $scope.Permissions.Read = data.data.Read;
                        $scope.Permissions.Update = data.data.Update;
                        $scope.Permissions.Delete = data.data.Delete;
                        $scope.Permissions.View = data.data.View;
                        $scope.dataGridInstance.columnOption("add", "visible", data.data.Create);
                        $scope.dataGridInstance.columnOption("delete", "visible", data.data.Delete);
                    });

                    function init() {
                        $scope.AdvanceValue = 100;
                        $scope.AdvanceDate = today;
                        $scope.AddNewAdvanceTypeShow = false;
                        $scope.AdvanceDueDate =
                            new Date($scope.AdvanceDate.getFullYear(), $scope.AdvanceDate.getMonth() + 1, 1);
                    }

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
                            return $.getJSON("/Advances/GetSubsidiesTypes/");
                        }
                    });
                    $scope.AddNewSubsidyTypePopup = {
                        Options: {
                            bindingOptions: { visible: "AddNewAdvanceTypeShow" },
                            width: 500,
                            height: 222,
                            title: "اضافة نوع سلفة جديد",
                            deferRendering: false,
                            showTitle: true,
                            dragEnabled: false,
                            contentTemplate: "AddNewSubsidyTypeTemplate",
                            onHiding: function(e) {
                                $scope.AdvanceTypeName = "";
                                $scope.COAID = null;
                                var validationGroup =
                                    DevExpress.validationEngine.getGroupConfig("AddNewSubsidyTypePopup");
                                if (validationGroup) {
                                    validationGroup.reset();
                                }

                            }
                        },
                        ValidationRules: {
                            AdvanceTypeName: {
                                validationGroup: "AddNewSubsidyTypePopup",
                                validationRules: [
                                    {
                                        type: "required",
                                        message: "حقل اجبارى"
                                    }
                                ]
                            },
                            COAID: {
                                validationGroup: "AddNewSubsidyTypePopup",
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
                            validationGroup: "AddNewSubsidyTypePopup",
                            useSubmitBehavior: true,
                            onClick: function(e) {
                                var validationGroup =
                                    DevExpress.validationEngine.getGroupConfig("AddNewSubsidyTypePopup");
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
                                            url: "/Advances/AddNewSubsidyType/",
                                            data: { typeName: $scope.AdvanceTypeName, coaID: $scope.COAID }
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
                                return "<div title='" +
                                    data.Text +
                                    "' value='" +
                                    data.Value +
                                    "'>" +
                                    data.Text +
                                    "</div>";
                            },
                            displayExpr: "Text",
                            valueExpr: "Value",
                            bindingOptions: { value: "COAID" },
                            placeholder: "حساب التهيئة",
                            showClearButton: true
                        }
                    }

                    $scope.AddNewSubsidyType = function() {
                        $scope.AddNewAdvanceTypeShow = true;
                    }
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
                            },
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
                                $scope.dataGridInstance.refresh();
                            }
                        }
                    };
                    $scope.Advance = {
                        ValidationRules: {
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
                            },
                            AdvanceType: {
                                validationGroup: "addAdvance",
                                validationRules: [
                                    {
                                        type: "required",
                                        message: "حقل اجبارى"
                                    }
                                ]
                            },
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
                        //نوع الاعانة
                        AdvanceType:
                        {
                            bindingOptions: { value: "AdvanceTypeId" },
                            dataSource: dataSourceAdvanceType,
                            valueExpr: "Value",
                            displayExpr: "Text",
                            placeholder: "نوع الاعانة",
                            showClearButton: true,
                            itemTemplate: function(data) {
                                return "<div title='" +
                                    data.Text +
                                    "' value='" +
                                    data.Value +
                                    "'>" +
                                    data.Text +
                                    "</div>";
                            }
                        },
                        //المبلغ
                        AdvanceValue:
                        {
                            bindingOptions: { value: "AdvanceValue" },
                            placeholder: "مبلغ الاعانة",
                            showClearButton: true,
                            min: 0
                        },
                        //المرفقات
                        Attachments: {
                            multiple: true,
                            accept: "*",
                            allowCanceling: true,
                            showFileList: false,
                            selectButtonText: "استعراض",
                            labelText: "المرفقات",
                            uploadMode: "useForm",
                            onInitialized: function(e) {
                                $scope.AttachmentsInstance = e.component;
                            }
                        }
                    };
                    $scope.isUpdate = false;
                    $scope.PopupTitle = "";
                    $scope.PopUpAddShow = false;
                    $scope.Popup = {
                        Options: {
                            bindingOptions: { visible: "PopUpAddShow", title: "PopupTitle" },
                            //height: "333",
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
                                        DevExpress.ui.notify({
                                            message: "جارى الحفظ",
                                            type: "success",
                                            displayTime: 3000,
                                            closeOnClick: true
                                        });
                                        var formData = new FormData();
                                        var files = $scope.AttachmentsInstance._options.value;
                                        files.forEach(function(value, key) {
                                            formData.append(key, value);
                                        });

                                        $http({
                                            method: "POST",
                                            url: "/Advances/AddSubsidy/",
                                            headers: { 'Content-Type': undefined },
                                            params: {
                                                studentId: $scope.StudentId,
                                                advanceDate: $scope.AdvanceDate,
                                                value: $scope.AdvanceValue,
                                                subsidyType: $scope.AdvanceTypeId
                                            },
                                            data: formData
                                        }).then(function(data) {
                                            if (data.data === "") {
                                                DevExpress.ui.notify({
                                                    message: "تم الحفظ بنجاح",
                                                    type: "success",
                                                    displayTime: 3000,
                                                    closeOnClick: true
                                                });
                                                validationGroup.reset();
                                                $scope.PopUpAddShow = false;
                                                //window.location.href = "/Advances/SubsidiesRequestsList";
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
                            text: "تعديل",
                            hint: "تعديل",
                            icon: "edit",
                            type: "default",
                            elementAttr: { "class": "btn-warning" },
                            useSubmitBehavior: true
                        }
                    }
                    $scope.AddNewAdvanceType = function() {
                        $scope.AddNewAdvanceTypeShow = true;
                    }
                    $scope.AddNewAdvanceTypePopup = {
                        Options: {
                            bindingOptions: { visible: "AddNewAdvanceTypeShow" },
                            width: 300,
                            height: 222,
                            title: "اضافة نوع اعانة جديد",
                            deferRendering: false,
                            showTitle: true,
                            dragEnabled: false,
                            contentTemplate: "AddNewAdvanceTypePopup",
                            onHiding: function(e) {
                                $scope.AdvanceTypeName = "";
                                var validationGroup =
                                    DevExpress.validationEngine.getGroupConfig("AddNewAdvanceTypePopup");
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
                            }
                        },
                        SaveButton: {
                            text: "إضافة",
                            hint: "إضافة",
                            icon: "add",
                            type: "success",
                            validationGroup: "AddNewAdvanceTypePopup",
                            useSubmitBehavior: true,
                            onClick: function(e) {
                                var validationGroup =
                                    DevExpress.validationEngine.getGroupConfig("AddNewAdvanceTypePopup");
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
                                            data: { typeName: $scope.AdvanceTypeName }
                                        }).then(function(data) {
                                            if (data.data === "") {
                                                DevExpress.ui.notify({
                                                    message: "تم الحفظ بنجاح",
                                                    type: "success",
                                                    displayTime: 3000,
                                                    closeOnClick: true
                                                });
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
                            showClearButton: true
                        }
                    }
                    var gridDataSource = new DevExpress.data.DataSource({
                        load: function(loadOptions) {
                            var d = $.Deferred();
                            $.getJSON("/Advances/SubsidiesSearch",
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
                                    totalSummary: loadOptions.totalSummary
                                        ? JSON.stringify(loadOptions.totalSummary)
                                        : "",
                                    group: loadOptions.group ? JSON.stringify(loadOptions.group) : "",
                                    groupSummary: loadOptions.groupSummary
                                        ? JSON.stringify(loadOptions.groupSummary)
                                        : ""
                                }).done(function(result) {
                                d.resolve(result.data,
                                    {
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
                                allowSorting: false,
                                format: { type: "fixedPoint", precision: 2 }
                            },
                            {
                                cssClass: "text-center",
                                caption: "اضافة",
                                hint: "اضافة طلب اعانة",
                                allowHiding: false,
                                fixedPosition: 100,
                                minWidth: "100px",
                                width: "100px",
                                name: "add",
                                fixed: true,
                                cellTemplate: function(container, options) {
                                    $("<div />").dxButton({
                                        icon: "plus",
                                        type: "success",
                                        text: "",
                                        hint: "اضافة طلب اعانة",
                                        rtlEnabled: false,
                                        elementAttr: { "class": "btn-ds-st btn-sm-ds" },
                                        onClick: function(e) {
                                            $scope.isUpdate = false;
                                            $scope.StudentId = options.key.STUDENT_ID;
                                            $scope.PopupTitle = "اضافة اعانة";
                                            $timeout(function() { $scope.PopUpAddShow = true; });
                                        }
                                    }).appendTo(container);
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
                                            url: "/Advances/CanDeleteLastSubsidyRequest",
                                            params: { studentId: options.key.STUDENT_ID }
                                        }).then(function(data) {
                                            debugger;
                                            if (data.data === true) {
                                                $("<div />").dxButton({
                                                    icon: "trash",
                                                    type: "danger",
                                                    text: "",
                                                    hint: "حذف اخر طلب اعانة",
                                                    elementAttr: { "class": "btn-ds-st btn-sm-ds" },
                                                    onClick: function(e) {
                                                        var resultt =
                                                            DevExpress.ui.dialog.confirm(
                                                                "هل تريد حذف اخر طلب اعانة لهذا الطالب",
                                                                "تأكيد");
                                                        resultt.done(function(dialogResult) {
                                                            if (dialogResult) {
                                                                debugger;
                                                                $http({
                                                                    method: "GET",
                                                                    url: "/Advances/DeleteLastSubsidyReuqst",
                                                                    params: { studentId: options.key.STUDENT_ID }
                                                                }).then(function(response) {
                                                                    debugger;
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
                            grouping:
                                true,
                            groupPaging:
                                true,
                            paging:
                                true,
                            sorting:
                                true,
                            summary:
                                true
                        },
                        headerFilter: {
                            allowSearch: true,
                            texts:
                            {
                                cancel: "الغاء",
                                emptyValue:
                                    "",
                                ok:
                                    "Ok"
                            },
                            visible: true
                        },
                        loadPanel: {
                            enabled: "auto",
                            indicatorSrc:
                                "",
                            showIndicator:
                                true,
                            showPane:
                                true,
                            text:
                                "تحميل..."
                        },
                        searchPanel: {
                            visible: true,
                            placeholder:
                                "بحث"
                        },
                        selection: {
                            mode: "single"
                        },
                        showRowLines: true,
                        columnAutoWidth:
                            true,
                        scrolling:
                        {
                            rtlEnabled: true,
                            useNative:
                                true,
                            scrollByContent:
                                true,
                            scrollByThumb:
                                true,
                            showScrollbar:
                                "onHover",
                            mode:
                                "standard", // or "virtual"
                            direction:
                                "both"
                        },
                        paging: {
                            enabled: true,
                            pageSize:
                                10
                        },
                        pager: {
                            allowedPageSizes: "auto",
                            infoText:
                                "صفحة  {0} من {1} ({2} عنصر)",
                            showInfo:
                                true,
                            showNavigationButtons:
                                true,
                            showPageSizeSelector:
                                true,
                            visible:
                                "auto"
                        },
                        showBorders: true,
                        allowColumnReordering:
                            true,
                        filterRow:
                        {
                            visible: true,
                            operationDescriptions:
                            {
                                between: "بين",
                                contains:
                                    "تحتوى على",
                                endsWith:
                                    "تنتهي بـ",
                                equal:
                                    "يساوى",
                                greaterThan:
                                    "اكبر من",
                                greaterThanOrEqual:
                                    "اكبر من او يساوى",
                                lessThan:
                                    "اصغر من",
                                lessThanOrEqual:
                                    "اصغر من او يساوى",
                                notContains:
                                    "لا تحتوى على",
                                notEqual:
                                    "لا يساوى",
                                startsWith:
                                    "تبدأ بـ"
                            },
                            resetOperationText: "الوضع الافتراضى"
                        },
                        editing: {
                            allowUpdating: false,
                            allowDeleting:
                                false,
                            texts:
                            {
                                confirmDeleteMessage: "تأكيد حذف  !",
                                deleteRow:
                                    "حذف",
                                editRow:
                                    "تعديل",
                                addRow:
                                    "اضافة"
                            }
                        },
                        onInitialized: function(e) {
                            $scope.dataGridInstance = e.component;
                        },
                        onCellPrepared: function(e) {
                            if (e.rowType === "data" && e.column.command === "edit") {
                                var isEditing = e.row.isEditing,
                                    $links = e.cellElement.find(".dx-link");
                                $links.text("");
                                if (isEditing) {
                                    $links.filter(".dx-link-save").addClass("dx-icon-save");
                                    $links.filter(".dx-link-cancel").addClass("dx-icon-revert");
                                } else {
                                    $links.filter(".dx-link-edit").addClass("btn btn-dark-orange btn-sm fa fa-pencil");
                                    $links.filter(".dx-link-delete").addClass("btn btn-danger btn-sm fa fa-trash-o");
                                }
                            }
                        }
                    };
                    $scope.onFormSubmit = function(e) {

                        var subsidy = {
                            studentId: $scope.StudentId,
                            advanceDate: $scope.AdvanceDate,
                            subsidyType: $scope.AdvanceTypeId,
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
                            url: "/Advances/AddSubsidy/",
                            data: { subsidy, formData }
                        }).then(function(data) {
                            if (data.data === "") {
                                DevExpress.ui.notify({
                                    message: "تم الحفظ بنجاح",
                                    type: "success",
                                    displayTime: 3000,
                                    closeOnClick: true
                                });
                                window.location.href = "/Advances/SubsidiesRequestsList";
                            } else {
                                DevExpress.ui.notify({
                                    message: data.data,
                                    type: "error",
                                    displayTime: 3000,
                                    closeOnClick: true
                                });
                            }
                        });
                    };
                }
            ]);
    })
    ();