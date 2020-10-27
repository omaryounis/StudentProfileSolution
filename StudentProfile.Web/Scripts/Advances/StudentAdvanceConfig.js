/// <reference path="../dx.web.debug.js" />
/// <reference path="../jquery-2.2.3.js" />
/// <reference path="../jquery-2.2.3.intellisense.js" />

(function () {
    app.controller("AdvanceConfigCtrl",
        [
            "$scope", "$http", "$timeout", "AdvanceConfigService",
            function ($scope, $http, $timeout, AdvanceConfigService) {

                // Initialization
                $scope.MDL_IsConditional = true;
                $scope.MDL_IsStudentVisible = true;
                $scope.AdvanceConfigGridInstance = '';
                $scope.MDL_SubsidyIsStudentVisible = true;
                $scope.AddAdvanceTypeTitle = "إضافة سلفة جديدة";


                /*********************************** Permissions *********************************/
                $scope.Permissions = {
                    Show: undefined,
                    OnOffBand: undefined,
                    EditBand: undefined,
                    AddBandType: undefined,
                    AddHelpType: undefined
                };

                 
                 
                /*--------------------------------* Permissions *--------------------------------*/

                // DataSources...
                var valueTypesDataSource =
                    [
                        { Text: 'مقطوع من صافي قيمة المكافأة', Value: 'V' },
                        { Text: 'نسبة من صافي قيمة المكافأة', Value: 'P' }
                    ];

                var gridDataSource = new DevExpress.data.DataSource({
                    key: "AdvanceConfigId",
                    paginate: true,
                    cacheRawData: true,
                    pageSize: 10,
                    loadMode: "raw",
                    load: function (loadOptions) {
                        return $.getJSON("/Advances/GetAdvancesConfigDataSource", function (data) { });
                    }
                });


                // DataGrid...
                $scope.AdvanceConfigGrid = {
                    dataSource: gridDataSource,
                    noDataText: "لا يوجد بيانات",
                    columns: [
                        {
                            dataField: "AdvanceType",
                            caption: "بند",
                            groupIndex: 0,
                            cssClass: "text-right"
                        },
                        {
                            dataField: "AdvanceName",
                            caption: "إسم البند",
                            cssClass: "text-right"
                        },
                        {
                            dataField: "COADescription",
                            caption: "الحساب المقابل",
                            cssClass: "text-right"
                        },
                        {
                            dataField: "Value",
                            caption: "مبلغ الخصم",
                            cssClass: "text-right"
                        },
                        {
                            dataField: "ValueType",
                            caption: "نوع القيمة",
                            cssClass: "text-right"
                        },
                        {
                            dataField: "MaxRequestValue",
                            caption: "الحد الأعلي للطلب",
                            cssClass: "text-right"

                        },
                        {
                            dataField: "Status",
                            cssClass: "text-center",
                            caption: "إيقاف و تفعيل",
                            hint: "إيقاف و تفعيل",
                            cellTemplate: function (container, options) {
                                //AdvanceConfigService.GetAllPermssions().then(function (data) {
                                //    debugger;
                            
                                //});
                                debugger;
                                //$http({
                                //    method: "Get",
                                //    url: "/Advances/StudentAdvanceConfigPerm"
                                //}).then(function (data) {
                                //    debugger;
                                //    $scope.Permissions.Show = data.data.Show;
                                //    $scope.Permissions.OnOffBand = data.data.OnOffBand;
                                //    $scope.Permissions.EditBand = data.data.EditBand;
                                //    $scope.Permissions.AddBandType = data.data.AddBandType;
                                //    $scope.Permissions.AddHelpType = data.data.AddHelpType;
                                //});
                                var div = $("<div />").appendTo(container);
                                div.dxSwitch({
                                    value: options.value,
                                    width: 50,
                                    disabled: !$scope.Permissions.OnOffBand,
                                    onOptionChanged: function (e) {
                                        debugger;
                                        //$http({
                                        //    method: "Get",
                                        //    url: "/Advances/StudentAdvanceConfigPerm"
                                        //}).then(function (data) {
                                        //    debugger; 
                                        //    $scope.Permissions.OnOffBand = data.data.OnOffBand; 
                                        //});
                                        if ($scope.Permissions.OnOffBand==false) {
                                            gridDataSource.reload();
                                            $scope.AdvanceConfigGridInstance.refresh();
                                            return swal("تنبيه", "عفوا ليس لديك صلاحية علي هذه العملية", "warning");
                                        }
                                        else {
                                            return $http({
                                                method: "Post",
                                                url: "/Advances/EditAdvanceConfigStatus",
                                                data: {
                                                    advanceConfigId: options.data.AdvanceConfigId,
                                                    status: e.value
                                                }
                                            }).then(function (data) {
                                                if (data.data !== "") { swal("حدث خطأ", data.data, "error"); }
                                                else {
                                                    swal("Done!", e.value === true ? "تم التفعيل" : "تم الإيقاف ", "success");
                                                    gridDataSource.reload();
                                                    $scope.AdvanceConfigGridInstance.refresh();
                                                }
                                            });
                                        }

                                    }
                                });
                            }
                        }
                    ],
                    wordWrapEnabled: false,
                    showBorders: true,
                    searchPanel: {
                        visible: true,
                        placeholder: "بحث",
                        width: 300
                    },
                    onCellPrepared: function (e) {

                        if (e.rowType === "header" && e.column.command === "edit") {

                            e.column.width = 80;
                            e.column.alignment = "center";
                            e.cellElement.text(" تعديل ");
                        }

                        if (e.rowType === "data" && e.column.command === "edit") {
                            $links = e.cellElement.find(".dx-link");
                            $links.text("");

                            $links.filter(".dx-link-edit").addClass("btn btn-dark-orange btn-sm fa fa-pencil");
                            $links.filter(".dx-link-delete").addClass("btn btn-danger btn-sm fa fa-trash-o");

                        }

                    },
                    onContentReady: function (e) {
                        debugger;
                        e.component.columnOption("command:edit", "visible", $scope.Permissions.EditBand);
                    },
                    loadPanel: {
                        enabled: "auto",
                        indicatorSrc: "",
                        showIndicator: true,
                        showPane: true,
                        text: "تحميل..."
                    },
                    headerFilter: {
                        visible: true
                    },
                    showRowLines: true,
                    groupPanel: {
                        visible: true,
                        emptyPanelText: "اسحب عمود هنا"
                    },
                    cssClass: "text-center",
                    columnAutoWidth: true,
                    width: "auto",
                    columnChooser: {
                        enabled: true
                    },
                    "export": {
                        enabled: true,
                        fileName: "تهيئة السلف والإعانات"
                    },
                    selection: { mode: "single" },
                    allowColumnResizing: true,
                    columnResizingMode: "widget",
                    showColumnLines: true,
                    rowAlternationEnabled: true,
                    paging: { enabled: true, pageSize: 10 },
                    pager: {
                        allowedPageSizes: "auto",
                        infoText: "صفحة {0} من {1} - {2} عنصر",
                        showInfo: true,
                        showNavigationButtons: true,
                        showPageSizeSelector: true,
                        visible: "auto"
                    },
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
                    grouping: {
                        allowCollapsing: true,
                        autoExpandAll: false,
                        texts: {
                            groupByThisColumn: "تجميع باستخدام هذا العمود",
                            groupContinuedMessage: "تكملة من الصفحة السابقة",
                            groupContinuesMessage: " التكملة فى الصفحة التالية ",
                            ungroup: "الغاء التجميع",
                            ungroupAll: "الغاء تجميع الكل"
                        }
                    },
                    editing: {
                        allowAdding: false,
                        allowUpdating: true,
                        allowDeleting: false,
                        texts: {
                            confirmDeleteMessage: "تأكيد حذف  !",
                            deleteRow: "حذف",
                            editRow: "تعديل",
                            addRow: "إضافة"
                        }
                    },
                    onInitialized: function (e) {
                        $scope.AdvanceConfigGridInstance = e.component;
                        $http({
                            method: "Get",
                            url: "/Advances/StudentAdvanceConfigPerm"
                        }).then(function (data) {
                            debugger;
                            $scope.Permissions.Show = data.data.Show;
                            $scope.Permissions.OnOffBand = data.data.OnOffBand;
                            $scope.Permissions.EditBand = data.data.EditBand;
                            $scope.Permissions.AddBandType = data.data.AddBandType;
                            $scope.Permissions.AddHelpType = data.data.AddHelpType;
                        });
                    },
                    onToolbarPreparing: function (e) {
                        e.toolbarOptions.items.unshift(
                            {
                                location: "after",
                                widget: "dxButton",
                                options: {
                                    text: "ربط حسم الصندوق بالقيد المحاسبي",
                                    icon: "plus",
                                    hint: "ربط حسم الصندوق بالقيد المحاسبي",
                                    onClick: function (event) { 
                                        debugger;
                                        $http({
                                            method: "Get",
                                            url: "/Advances/StudentAdvanceConfigSandok?screenId=130"
                                        }).then(function (data) {
                                            debugger;
                                            $scope.PermissionSandok = data.data;
                                        });
                                        if ($scope.PermissionSandok==false) {
                                            return swal("تنبيه", "عفوا ليس لديك صلاحية علي هذه العملية", "warning");
                                        } else {
                                            debugger
                                            $http({
                                                method: "GET",
                                                url: "/Advances/GetRewardItemTypeName/"
                                            }).then(function (data) {
                                                debugger;
                                                $scope.RewardItemName = data.data.rewardItemName;
                                                $scope.RewardItemCOAID = data.data.coaid;
                                                $scope.RewardItemRecieveFromPayroll_COAID = data.data.coaidReceive;
                                                $scope.AddNewRewardItemShow = true;
                                            });
                                        }
                                      
                                    }
                                }
                            },
                            {
                                location: "after",
                                widget: "dxButton",
                                options: {
                                    text: "إضافة نوع سلفة",
                                    icon: "plus",
                                    hint: "إضافة سلفة جديدة",
                                    onClick: function (event) {
                                        debugger 
                                        //$http({
                                        //    method: "Get",
                                        //    url: "/Advances/StudentAdvanceConfigPerm"
                                        //}).then(function (data) {
                                        //    debugger; 
                                        //    $scope.Permissions.AddBandType = data.data.AddBandType; 
                                        //});
                                        if ($scope.Permissions.AddBandType==false) {
                                            return swal("تنبيه", "عفوا ليس لديك صلاحية علي هذه العملية", "warning");
                                        }
                                        $timeout(function () {
                                            $scope.AddAdvanceTypeShow = true;
                                        });

                                    }
                                }
                            },
                            {
                                location: "after",
                                widget: "dxButton",
                                options: {
                                    text: "إضافة نوع إعانة",
                                    icon: "plus",
                                    hint: "إضافة إعانة جديدة",
                                    onClick: function (event) {
                                        debugger;
                                        //$http({
                                        //    method: "Get",
                                        //    url: "/Advances/StudentAdvanceConfigPerm"
                                        //}).then(function (data) {
                                        //    debugger; 
                                        //    $scope.Permissions.AddHelpType = data.data.AddHelpType;
                                        //});
                                        if ($scope.Permissions.AddHelpType==false) {
                                            return swal("تنبيه", "عفوا ليس لديك صلاحية علي هذه العملية", "warning");
                                        }
                                        $timeout(function () {
                                            $scope.AddNewSubsidyTypeShow = true;
                                        });
                                    }
                                }
                            }


                        );
                    },
                    onEditingStart: function (e) {
                        debugger;
                        e.cancel = true;
                        if (!e.data.Status) {
                            return swal("تنبيه", "برجاء تفعيل هذا النوع أولا حتي تتمكن من عملية التعديل", "warning");
                        }

                        $http({
                            method: "GET",
                            url: "/Advances/GetAdvanceConfigById/",
                            params: { advanceConfigId: e.data.AdvanceConfigId }
                        }).then(function (data) {
                            if (data.data !== "") {
                                debugger;
                                $scope.AdvanceConfigId = e.data.AdvanceConfigId;

                                if (data.data.AdvanceType === 'A') {
                                    $scope.MDL_AdvanceCOAID = data.data.COAID;
                                    $scope.MDL_AdvanceValue = data.data.Value;
                                    $scope.MDL_AdvanceValueType = data.data.ValueType;
                                    $scope.MDL_MaxRequestValue = data.data.MaxRequestValue;
                                    $scope.MDL_AdvanceTypeName = data.data.AdvanceSettingName;


                                    $scope.MDL_IsConditional = data.data.IsConditional;
                                    $scope.MDL_IsStudentVisible = data.data.IsStudentVisible;
                                    $scope.MDL_CoaId_RecieveFromPayroll = data.data.CoaId_RecieveFromPayroll;

                                    $timeout(function () {
                                        $scope.AddAdvanceTypeTitle = " تعديل " + "(" + $scope.MDL_AdvanceTypeName + ")";
                                        $scope.AddAdvanceTypeShow = true;
                                    });
                                } else {
                                    $scope.MDL_SubsidyIsStudentVisible = data.data.IsStudentVisible;
                                    $scope.MDL_SubsidyMaxRequestValue = data.data.MaxRequestValue;
                                    $scope.MDL_SubsidyTypeName = data.data.AdvanceSettingName;
                                    $scope.MDL_SubsidyCOAID = data.data.COAID;

                                    $timeout(function () {
                                        $scope.AddSubsidyTypeTitle = " تعديل " + "(" + $scope.MDL_SubsidyTypeName + ")";
                                        $scope.AddNewSubsidyTypeShow = true;
                                    });
                                }
                            } else {
                                return swal("تنبيه", "لا يمكن التعديل لارتباطه بسجلات أخري", "warning");
                            }
                        });
                    }
                };


                $scope.AddNewRewardItemPopup = {
                    Options: {
                        bindingOptions: { visible: "AddNewRewardItemShow"},
                        width: 850,
                        height: 500,
                        deferRendering: false,
                        showTitle: true,
                        dragEnabled: false,
                        shadingColor: "rgba(0, 0, 0, 0.69)",
                        closeOnOutsideClick: false,
                        contentTemplate: "AddNewRewardItemPopup",
                        title: "ربط حسم صندوق الطالب بقيد الإستحقاق" ,
                        onHiding: function (e) {
                            // Refresh Grid...
                            var validationGroup = DevExpress.validationEngine.getGroupConfig("AddNewRewardItemPopup_VG");
                            if (validationGroup) {
                                validationGroup.reset();
                            } 
                            // Refresh Grid...
                            gridDataSource.reload();

                        }
                    },
                    ValidationRules: {
                        RewardItemName: {
                            validationGroup: "AddNewRewardItemPopup_VG",
                            validationRules: [
                                {
                                    type: "required",
                                    message: "حقل اجبارى"
                                }
                            ] 
                        },
                        RewardItemCOAID: {
                            validationGroup: "AddNewRewardItemPopup_VG",
                            validationRules: [
                                {
                                    type: "required",
                                    message: "حقل اجبارى"
                                }
                            ]
                        },
                        RewardItemRecieveFromPayroll_COAID: {
                            validationGroup: "AddNewRewardItemPopup_VG",
                            validationRules: [
                                {
                                    type: "required",
                                    message: "حقل اجبارى"
                                }
                            ]
                        }
                    },
                    SaveButton: {
                        text: "تعديل",
                        hint: "تعديل",
                        icon: "edit",
                        type: "success",
                        validationGroup: "AddNewRewardItemPopup_VG",
                        useSubmitBehavior: true,
                        onClick: function (e) {
                            debugger;
                            var validationGroup = DevExpress.validationEngine.getGroupConfig("AddNewRewardItemPopup_VG");
                            if (validationGroup) {
                                var result = validationGroup.validate();
                                if (result.isValid) {
                                    debugger;
                                    $http({
                                        method: "POST",
                                        url: "/Advances/UpdateRewardItemType/",
                                        data: {
                                            RewardItemCOAID: $scope.RewardItemCOAID,
                                            RewardItemRecieveFromPayroll_COAID: $scope.RewardItemRecieveFromPayroll_COAID
                                        }
                                    }).then(function (data) {
                                        if (data.data === "") {
                                            swal("Done!", "تم التعديل بنجاح", "success");
                                           
                                        } else {
                                            swal("حدث خطأ", data.data, "error");
                                        }
                                    });
                                }
                            }
                        }
                    },
                    RewardItemName: {
                        bindingOptions: { value: "RewardItemName" },
                        showClearButton: true,
                        disabled:true
                    },
                    RewardItemCOAID: {
                        dataSource: new DevExpress.data.DataSource({
                            key: "Value",
                            paginate: true,
                            cacheRawData: true,
                            loadMode: "raw",
                            load: function (loadOptions) {
                                return $.getJSON("/Advances/GetChildAcctounts", function (data) { });
                            }
                        }),
                        itemTemplate: function (data) {
                            return "<div title='" + data.Text + "' value='" + data.Value + "'>" + data.Text + "</div>";
                        },
                        displayExpr: "Text",
                        valueExpr: "Value",
                        showBorders: true,
                        searchEnabled: true,
                        showClearButton: true,
                        rtlEnabled: true,
                        bindingOptions: { value: "RewardItemCOAID" },
                        placeholder: "اختر",
                        onValueChanged: function (e) {
                            debugger;
                            $scope.RewardItemCOAID = e.value;
                        }
                    },
                    RewardItemRecieveFromPayroll_COAID: {
                        dataSource: new DevExpress.data.DataSource({
                            key: "Value",
                            paginate: true,
                            cacheRawData: true,
                            loadMode: "raw",
                            load: function (loadOptions) {
                                return $.getJSON("/Advances/GetChildAcctounts", function (data) { });
                            }
                        }),
                        itemTemplate: function (data) {
                            return "<div title='" + data.Text + "' value='" + data.Value + "'>" + data.Text + "</div>";
                        },
                        displayExpr: "Text",
                        valueExpr: "Value",
                        showBorders: true,
                        searchEnabled: true,
                        showClearButton: true,
                        rtlEnabled: true,
                        bindingOptions: { value: "RewardItemRecieveFromPayroll_COAID" },
                        placeholder: "اختر",
                        onValueChanged: function (e) {
                            $scope.RewardItemRecieveFromPayroll_COAID = e.value;
                        }
                    }
                };


                //إضافة سلفة جديدة...
                $scope.AddNewAdvanceTypePopup = {
                    Options: {
                        bindingOptions: { visible: "AddAdvanceTypeShow", title: "AddAdvanceTypeTitle" },
                        width: 850,
                        height: 500,
                        deferRendering: false,
                        showTitle: true,
                        dragEnabled: false,
                        shadingColor: "rgba(0, 0, 0, 0.69)",
                        closeOnOutsideClick: false,
                        contentTemplate: "AddNewAdvanceTypePopup",
                        onHiding: function (e) {
                            $scope.AddAdvanceTypeTitle = "إضافة سلفة جديدة";

                            // Reset Controls...
                            $scope.AdvanceConfigId = '';
                            $scope.MDL_AdvanceCOAID = '';
                            $scope.MDL_AdvanceValue = '';
                            $scope.MDL_AdvanceTypeName = '';
                            $scope.MDL_MaxRequestValue = '';
                            $scope.MDL_AdvanceValueType = '';

                            $scope.MDL_IsConditional = '';
                            $scope.MDL_IsStudentVisible = '';
                            $scope.MDL_CoaId_RecieveFromPayroll = '';


                            var validationGroup = DevExpress.validationEngine.getGroupConfig("AddNewAdvanceTypePopup_VG");
                            if (validationGroup) {
                                validationGroup.reset();
                            }

                            // Refresh Grid...
                            gridDataSource.reload();

                        }
                    },
                    ValidationRules: {
                        AdvanceTypeName: {
                            validationGroup: "AddNewAdvanceTypePopup_VG",
                            validationRules: [
                                {
                                    type: "required",
                                    message: "حقل اجبارى"
                                }
                            ]
                        },
                        AdvanceCOAID: {
                            validationGroup: "AddNewAdvanceTypePopup_VG",
                            validationRules: [
                                {
                                    type: "required",
                                    message: "حقل اجبارى"
                                }
                            ]
                        },
                        AdvanceValue: {
                            validationGroup: "AddNewAdvanceTypePopup_VG",
                            validationRules: [
                                {
                                    type: "required",
                                    message: "حقل اجبارى"
                                }
                            ]
                        },
                        AdvanceValueType: {
                            validationGroup: "AddNewAdvanceTypePopup_VG",
                            validationRules: [
                                {
                                    type: "required",
                                    message: "حقل اجبارى"
                                }
                            ]
                        },
                        MaxRequestValue: {
                            validationGroup: "AddNewAdvanceTypePopup_VG",
                            validationRules: [
                                {
                                    type: "required",
                                    message: "حقل اجبارى"
                                }
                            ]
                        },
                        RecieveFromPayroll_COAID: {
                            validationGroup: "AddNewAdvanceTypePopup_VG",
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
                        validationGroup: "AddNewAdvanceTypePopup_VG",
                        useSubmitBehavior: true,
                        onClick: function (e) {
                            debugger;
                            if ($scope.MDL_AdvanceValueType === 'P') {

                                if ($scope.MDL_AdvanceValue > 100) {
                                    return DevExpress.ui.notify({
                                        message: "لا يمكن أن تزيد نسبة الخصم عن 100",
                                        type: "error",
                                        displayTime: 3000,
                                        closeOnClick: true
                                    });
                                }
                            }

                            if ($scope.MDL_CoaId_RecieveFromPayroll === $scope.MDL_AdvanceCOAID) {
                                debugger;
                                return DevExpress.ui.notify({
                                    message: "لا يمكن أن يكون حساب السلفة هو نفس حساب الخصم من مسير المكافأت",
                                    type: "error",
                                    displayTime: 3000,
                                    closeOnClick: true
                                });
                            }

                            var validationGroup = DevExpress.validationEngine.getGroupConfig("AddNewAdvanceTypePopup_VG");
                            if (validationGroup) {
                                var result = validationGroup.validate();
                                if (result.isValid) {
                                    //DevExpress.ui.notify({
                                    //    message: "جارى الإضافة",
                                    //    type: "success",
                                    //    displayTime: 3000,
                                    //    closeOnClick: true
                                    //});
                                    debugger;
                                    $http({
                                        method: "POST",
                                        url: "/Advances/AddNewAdvanceType/",
                                        data: {
                                            advanceSettingId: $scope.AdvanceConfigId,
                                            advanceTypeName: $scope.MDL_AdvanceTypeName,
                                            advanceCoaId: $scope.MDL_AdvanceCOAID,
                                            advanceValue: $scope.MDL_AdvanceValue,
                                            advanceValueType: $scope.MDL_AdvanceValueType,
                                            maxRequestValue: $scope.MDL_MaxRequestValue,
                                            IsConditional: $scope.MDL_IsConditional,
                                            IsStudentVisible: $scope.MDL_IsStudentVisible,
                                            CoaId_RecieveFromPayroll: $scope.MDL_CoaId_RecieveFromPayroll
                                        }
                                    }).then(function (data) {
                                        if (data.data === "") {
                                            swal("Done!", "تم الإضافة بنجاح", "success");

                                            // Reset Controls...
                                            $scope.AdvanceConfigId = '';
                                            $scope.MDL_AdvanceCOAID = '';
                                            $scope.MDL_AdvanceValue = '';
                                            $scope.MDL_AdvanceTypeName = '';
                                            $scope.MDL_MaxRequestValue = '';
                                            $scope.MDL_AdvanceValueType = '';

                                            $scope.MDL_IsConditional = '';
                                            $scope.MDL_IsStudentVisible = '';
                                            $scope.MDL_CoaId_RecieveFromPayroll = '';

                                            $scope.AddAdvanceTypeTitle = "إضافة سلفة جديدة";

                                            // Reset Validations...
                                            validationGroup.reset();
                                        } else {
                                            swal("حدث خطأ", data.data, "error");
                                        }
                                    });
                                }
                            }
                        }
                    },
                    AdvanceTypeName: {
                        bindingOptions: { value: "MDL_AdvanceTypeName" },
                        showClearButton: true
                    },
                    AdvanceCOAID: {
                        dataSource: new DevExpress.data.DataSource({
                            key: "Value",
                            paginate: true,
                            cacheRawData: true,
                            loadMode: "raw",
                            load: function (loadOptions) {
                                return $.getJSON("/Advances/GetChildAcctounts", function (data) { });
                            }
                        }),
                        itemTemplate: function (data) {
                            return "<div title='" + data.Text + "' value='" + data.Value + "'>" + data.Text + "</div>";
                        },
                        displayExpr: "Text",
                        valueExpr: "Value",
                        showBorders: true,
                        searchEnabled: true,
                        showClearButton: true,
                        rtlEnabled: true,
                        bindingOptions: { value: "MDL_AdvanceCOAID" },
                        placeholder: "اختر",
                        onValueChanged: function (e) {
                            debugger;
                            if (e.value === $scope.MDL_CoaId_RecieveFromPayroll && e.value > 0) {

                                return DevExpress.ui.notify({
                                    message: "لا يمكن أن يكون حساب السلفة هو نفس حساب التحصيل من مسير المكافأت",
                                    type: "error",
                                    displayTime: 5000,
                                    closeOnClick: true
                                });
                            }
                        }
                    },
                    RecieveFromPayroll_COAID: {
                        dataSource: new DevExpress.data.DataSource({
                            key: "Value",
                            paginate: true,
                            cacheRawData: true,
                            loadMode: "raw",
                            load: function (loadOptions) {
                                return $.getJSON("/Advances/GetChildAcctounts", function (data) { });
                            }
                        }),
                        itemTemplate: function (data) {
                            return "<div title='" + data.Text + "' value='" + data.Value + "'>" + data.Text + "</div>";
                        },
                        displayExpr: "Text",
                        valueExpr: "Value",
                        showBorders: true,
                        searchEnabled: true,
                        showClearButton: true,
                        rtlEnabled: true,
                        bindingOptions: { value: "MDL_CoaId_RecieveFromPayroll" },
                        placeholder: "اختر",
                        onValueChanged: function (e) {
                            if (e.value === $scope.MDL_AdvanceCOAID && e.value > 0) {
                                debugger;
                                return DevExpress.ui.notify({
                                    message: "لا يمكن أن يكون حساب السلفة هو نفس حساب التحصيل من مسير المكافأت",
                                    type: "error",
                                    displayTime: 5000,
                                    closeOnClick: true
                                });
                            }
                        }
                    },
                    AdvanceValue: {
                        bindingOptions: { value: "MDL_AdvanceValue" },
                        showSpinButtons: true,
                        showClearButton: true,
                        rtlEnabled: true,
                        min: 0.1
                    },
                    AdvanceValueType: {
                        items: valueTypesDataSource,
                        bindingOptions: { value: "MDL_AdvanceValueType" },
                        placeholder: "اختر",
                        displayExpr: "Text",
                        valueExpr: "Value",
                        showClearButton: true,
                        onValueChanged: function (e) {
                            debugger;
                            if (e.value === 'P') {
                                if ($scope.MDL_AdvanceValue > 100) {
                                    return DevExpress.ui.notify({
                                        message: "لا يمكن أن تزيد نسبة الخصم عن 100",
                                        type: "error",
                                        displayTime: 3000,
                                        closeOnClick: true
                                    });
                                }
                            }

                        }
                    },
                    MaxRequestValue: {
                        bindingOptions: { value: "MDL_MaxRequestValue" },
                        showSpinButtons: true,
                        showClearButton: true,
                        rtlEnabled: true,
                        min: 0.1
                    },
                    IsStudentVisible: {
                        bindingOptions: {
                            value: "MDL_IsStudentVisible"
                        },
                        onValueChanged: function (e) {
                            $scope.MDL_IsStudentVisible = e.value;
                        }
                    },
                    IsConditional: {
                        bindingOptions: {
                            value: "MDL_IsConditional"
                        },
                        onValueChanged: function (e) {
                            $scope.MDL_IsConditional = e.value;
                        }
                    }
                };


                //إضافة إعانة جديدة...
                $scope.AddNewSubsidyTypePopup = {
                    Options: {
                        bindingOptions: { visible: "AddNewSubsidyTypeShow", title: "AddSubsidyTypeTitle" },
                        width: 750,
                        height: 320,
                        deferRendering: false,
                        showTitle: true,
                        dragEnabled: false,
                        shadingColor: "rgba(0, 0, 0, 0.69)",
                        closeOnOutsideClick: false,
                        contentTemplate: "AddNewSubsidyTypePopup",
                        onHiding: function (e) {
                            debugger;
                            $scope.SubsidyCOAID = '';
                            $scope.SubsidyTypeName = '';
                            $scope.AdvanceConfigId = '';
                            $scope.MDL_SubsidyIsStudentVisible = '';
                            $scope.MDL_SubsidyMaxRequestValue = '';
                            $scope.AddSubsidyTypeTitle = "إضافة إعانة جديدة";

                            var validationGroup = DevExpress.validationEngine.getGroupConfig("AddNewSubsidyTypePopup");
                            if (validationGroup) {
                                validationGroup.reset();
                            }
                            gridDataSource.reload();
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
                        },
                        MaxRequestValue: {
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
                        onClick: function (e) {
                            var validationGroup = DevExpress.validationEngine.getGroupConfig("AddNewSubsidyTypePopup");
                            if (validationGroup) {
                                var result = validationGroup.validate();
                                if (result.isValid) {
                                    //DevExpress.ui.notify({
                                    //    message: "جارى الإضافة",
                                    //    type: "success",
                                    //    displayTime: 3000,
                                    //    closeOnClick: true
                                    //});
                                    debugger;
                                    $http({
                                        method: "POST",
                                        url: "/Advances/AddNewSubsidyType/",
                                        data: {
                                            IsStudentVisible: $scope.MDL_SubsidyIsStudentVisible,
                                            maxRequestValue: $scope.MDL_SubsidyMaxRequestValue,
                                            subsidyTypeName: $scope.MDL_SubsidyTypeName,
                                            advanceSettingId: $scope.AdvanceConfigId,
                                            subsidyCoaId: $scope.MDL_SubsidyCOAID
                                        }
                                    }).then(function (data) {
                                        if (data.data === "") {
                                            swal("Done!", "تم الإضافة بنجاح", "success");

                                            // Reset Controls...
                                            $scope.MDL_SubsidyIsStudentVisible = '';
                                            $scope.MDL_SubsidyMaxRequestValue = '';
                                            $scope.MDL_SubsidyTypeName = '';
                                            $scope.MDL_SubsidyCOAID = '';
                                            $scope.AddSubsidyTypeTitle = "إضافة إعانة جديدة";

                                            // Reset Validations...
                                            validationGroup.reset();
                                        } else {
                                            swal("حدث خطأ", data.data, "error");
                                        }
                                    });
                                }
                            }
                        }
                    },
                    SubsidyTypeName: {
                        bindingOptions: { value: "MDL_SubsidyTypeName" },
                        placeholder: "اسم الإعانة",
                        showClearButton: true
                    },
                    SubsidyCOAID: {
                        dataSource: new DevExpress.data.DataSource({
                            key: "Value",
                            paginate: true,
                            cacheRawData: true,
                            loadMode: "raw",
                            load: function (loadOptions) {
                                return $.getJSON("/Advances/GetChildAcctounts", function (data) { });
                            }
                        }),
                        itemTemplate: function (data) {
                            return "<div title='" + data.Text + "' value='" + data.Value + "'>" + data.Text + "</div>";
                        },
                        displayExpr: "Text",
                        valueExpr: "Value",
                        showBorders: true,
                        searchEnabled: true,
                        showClearButton: true,
                        rtlEnabled: true,
                        bindingOptions: { value: "MDL_SubsidyCOAID" },
                        placeholder: "حساب التهيئة"
                    },
                    MaxRequestValue: {
                        bindingOptions: { value: "MDL_SubsidyMaxRequestValue" },
                        showSpinButtons: true,
                        showClearButton: true,
                        rtlEnabled: true,
                        min: 0.1
                    },
                    IsStudentVisible: {
                        bindingOptions: {
                            value: "MDL_SubsidyIsStudentVisible"
                        },
                        onValueChanged: function (e) {
                            $scope.MDL_SubsidyIsStudentVisible = e.value;
                        }
                    }
                };
            }
        ]);




})();

//Service
app.service("AdvanceConfigService", ["$http", function ($http) {
    this.GetAllPermssions = function () {
        debugger;
        var response = $http({
            method: "Get",
            url: "/Advances/StudentAdvanceConfig?screenId=79"
        });
        return response;
    };// end function
    
}]);// End service