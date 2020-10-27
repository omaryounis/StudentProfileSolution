/// <reference path="../dx.web.debug.js" />
/// <reference path="../jquery-2.2.3.js" />
/// <reference path="../jquery-2.2.3.intellisense.js" />
(function() {
    app.controller("ConfigCtrl",
        [
            "$scope", "$http", "$timeout", function($scope, $http, $timeout) {
                var dataSourceAdvanceSettingId = new DevExpress.data.CustomStore({
                    key: "AdvanceSettingId",
                    paginate: true,
                    cacheRawData: true,
                    pageSize: 10,
                    loadMode: "raw",
                    load: function(loadOptions) {
                        return $.getJSON("/Config/GetAdvanceSettings",
                            function(data) {
                            });
                    }
                });

                $scope.dataGridOptions = {
                    dataSource: new DevExpress.data.DataSource({
                        paginate: true,
                        cacheRawData: true,
                        pageSize: 10,
                        key: "ID",
                        loadMode: "processed",
                        load: function(e) {
                            return $.getJSON("/Config/GetAdvanceSettingConfig",
                                function(data) {
                                });
                        },
                        byKey: function(key) {
                            return { ID: key };
                        },
                        insert: function(values) {
                            return $.post("/Config/UpdateAdvanceSettingConfig", values);
                        },
                        update: function(key, values) {
                            return $.post("/Config/UpdateAdvanceSettingConfig",
                                { id: key, advanceSettingId: values.AdvanceSettingId, coaId: values.COAID });
                        }
                    }),
                    noDataText: "لا يوجد بيانات",
                    columns: [
                        {
                            dataField: "AdvanceSettingId",
                            caption: "نوع الطلب",
                            validationRules: [{ type: "required" }],
                            lookup: {
                                allowClearing: false,
                                dataSource: dataSourceAdvanceSettingId,
                                displayExpr: "AdvanceSettingName",
                                valueExpr: "AdvanceSettingId"
                            },
                            allowEditing: false
                        },
                        {
                            dataField: "COAID",
                            caption: "الحساب",
                            validationRules: [{ type: "required" }],
                            lookup: {
                                allowClearing: false,
                                dataSource: new DevExpress.data.CustomStore({
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
                                displayExpr: "Text",
                                valueExpr: "Value"
                            }
                        },
                        {
                            dataField: "type",
                            caption: "النوع",
                            groupIndex: 0
                        }
                    ],
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
                    grouping: {
                        allowCollapsing: true,
                        autoExpandAll: false,
                        contextMenuEnabled: true,
                        expandMode: "rowClick",
                        texts: {
                            groupByThisColumn: "تجميع باستخدام هذا العمود",
                            groupContinuedMessage: "تكملة من الصفحة السابقة",
                            groupContinuesMessage: "التكملة فى الصفحة التالية",
                            ungroup: "الغاء التجميع",
                            ungroupAll: "الغاء تجميع الكل"
                        }
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
                    editing: {
                        allowUpdating: true,
                        allowDeleting: false,
                        texts: {
                            confirmDeleteMessage: "تأكيد حذف  !",
                            deleteRow: "حذف",
                            editRow: "تعديل",
                            addRow: "اضافة"
                        }
                    },
                    onToolbarPreparing: function(e) {
                        e.toolbarOptions.items.unshift(
                            {
                                location: "after",
                                widget: "dxButton",
                                options: {
                                    icon: "plus",
                                    hint: "اضافة نوع سلفة جديد",
                                    onClick: function(event) {
                                        $timeout(function() {
                                            $scope.AddNewAdvanceTypeShow = true;
                                        });
                                    }
                                }
                            },
                            {
                                location: "after",
                                widget: "dxButton",
                                options: {
                                    icon: "plus",
                                    hint: "اضافة نوع اعانة جديد",
                                    onClick: function(event) {
                                        $timeout(function() {
                                            $scope.AddNewSubsidyTypeShow = true;
                                        });
                                    }
                                }
                            });
                    },
                    onCellPrepared: function(e) {
                        if (e.rowType === "data" && e.column.command === "edit") {
                            var isEditing = e.row.isEditing,
                                $links = e.cellElement.find(".dx-link");
                            $links.text("");
                            if (isEditing) {
                                $links.filter(".dx-link-save").addClass("dx-icon-save btn btn-success btn-sm")
                                    .attr("title", "حفظ");
                                $links.filter(".dx-link-cancel").addClass("dx-icon-revert btn btn-danger btn-sm")
                                    .attr("title", "الغاء");
                            } else {
                                $links.filter(".dx-link-edit")
                                    .addClass("btn btn-dark-orange btn-sm fa fa-pencil").attr("title", "تعديل");
                                $links.filter(".dx-link-delete")
                                    .addClass("btn btn-danger btn-sm fa fa-trash-o").attr("title", "حذف");
                            }
                        }
                    },
                    onInitialized: function(e) {
                        $scope.dataGridInstance = e.component;
                    }
                }
                //اضافة نوع سلفة جديد
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
                            $scope.AdvanceCOAID = null;
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
                        AdvanceCOAID: {
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
                                    debugger;
                                    $http({
                                        method: "POST",
                                        url: "/Advances/AddNewAdvanceType/",
                                        data: { typeName: $scope.AdvanceTypeName, coaId: $scope.AdvanceCOAID }
                                    }).then(function(data) {
                                        if (data.data === "") {
                                            DevExpress.ui.notify({
                                                message: "تم الحفظ بنجاح",
                                                type: "success",
                                                displayTime: 3000,
                                                closeOnClick: true
                                            });
                                            $timeout(function() {
                                                $scope.AddNewAdvanceTypeShow = false;
                                                debugger;
                                                dataSourceAdvanceSettingId.clearRawDataCache();
                                                dataSourceAdvanceSettingId.load();
                                                $scope.dataGridInstance.refresh();
                                            });
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
                    AdvanceCOAID: {
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
                        bindingOptions: { value: "AdvanceCOAID" },
                        placeholder: "حساب التهيئة",
                        showClearButton: true
                    }
                }
                //اضافة نوع اعانة جديد
                $scope.AddNewSubsidyTypePopup = {
                    Options: {
                        bindingOptions: { visible: "AddNewSubsidyTypeShow" },
                        width: 600,
                        height: 222,
                        title: "اضافة نوع اعانة جديد",
                        deferRendering: false,
                        showTitle: true,
                        dragEnabled: false,
                        contentTemplate: "AddNewSubsidyTypePopup",
                        onHiding: function(e) {
                            $scope.SubsidyTypeName = "";
                            $scope.SubsidyCOAID = null;
                            var validationGroup = DevExpress.validationEngine.getGroupConfig("AddNewSubsidyTypePopup");
                            if (validationGroup) {
                                validationGroup.reset();
                            }
                        }
                    },
                    ValidationRules: {
                        SubsidyTypeName: {
                            validationGroup: "AddNewSubsidyTypePopup",
                            validationRules: [
                                {
                                    type: "required",
                                    message: "حقل اجبارى"
                                }
                            ]
                        },
                        SubsidyCOAID: {
                            validationGroup: "AddNewSubsidyTypePopup",
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
                        validationGroup: "AddNewSubsidyTypePopup",
                        useSubmitBehavior: true,
                        onClick: function(e) {
                            var validationGroup = DevExpress.validationEngine.getGroupConfig("AddNewSubsidyTypePopup");
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
                                        data: { typeName: $scope.SubsidyTypeName, coaId: $scope.SubsidyCOAID }
                                    }).then(function(data) {
                                        if (data.data === "") {
                                            DevExpress.ui.notify({
                                                message: "تم الحفظ بنجاح",
                                                type: "success",
                                                displayTime: 3000,
                                                closeOnClick: true
                                            });
                                            $timeout(function() {
                                                $scope.AddNewSubsidyTypeShow = false;
                                                dataSourceAdvanceSettingId.clearRawDataCache();
                                                dataSourceAdvanceSettingId.load();
                                                $scope.dataGridInstance.refresh();
                                            });
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
                    SubsidyTypeName: {
                        bindingOptions: { value: "SubsidyTypeName" },
                        placeholder: "اسم النوع",
                        showClearButton: true
                    },
                    SubsidyCOAID: {
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
                        bindingOptions: { value: "SubsidyCOAID" },
                        placeholder: "حساب التهيئة",
                        showClearButton: true
                    }
                }
            }
        ]);
})();