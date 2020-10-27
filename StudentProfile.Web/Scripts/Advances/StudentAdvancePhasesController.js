(function () {
    app.controller("StudentAdvancePhasesCtrl", ["$scope", "$http", "StudentAdvanceSrvc",
        function ($scope, $http, StudentAdvanceSrvc) {

            debugger;
            $scope.PopUpShow = false;

            // MenuHandling
            const Menu = [
                { key: 1, text: "إضافة مرحلة جديدة" },
                { key: 3, text: "تعديل ترتيب المراحل" },
                { key: 2, text: "إسناد المسـتخدمين" }
            ];


            /*********************************** $scope.Permissions *********************************/
            $scope.Permissions = {
                View: false,
                OnOffPhase: false,
                EditPhase: false,
                AddNewPhase: false,
                ReorderPhases: false,
                AssignUsers: false
            };

            $http({
                method: "Get",
                url: "/Advances/GetStudentAdvancePhasesPermissions?screenId=81"
            }).then(function (data) {
                debugger;
                $scope.Permissions.View = data.data.View;
                $scope.Permissions.OnOffPhase = data.data.OnOffPhase;
                $scope.Permissions.EditPhase = data.data.EditPhase;
                $scope.Permissions.AddNewPhase = data.data.AddNewPhase;
                $scope.Permissions.ReorderPhases = data.data.ReorderPhases;
                $scope.Permissions.AssignUsers = data.data.AssignUsers;

                debugger;
            });

            /*--------------------------------* $scope.Permissions *--------------------------------*/


            $scope.MenuOptions = {
                dataSource: Menu,
                itemTemplate: function (data) {
                    if (data.key === 1) {
                        return $("<div><i class='icon dx-icon-plus'></i><span> </span>" + data.text + "</div>");
                    }
                    else if (data.key === 3) {
                        return $("<div><i class='icon dx-icon-edit'></i><span> </span>" + data.text + "</div>");
                    }
                    else if (data.key === 2) {
                        return $("<div><i class='icon dx-icon-user'></i><span> </span>" + data.text + "</div>");
                    }

                },
                onItemClick: function (e) {
                    debugger;
                    $scope.selectedItem = e.itemData.key;

                    if ($scope.selectedItem === 2) {
                        if ($scope.Permissions.AssignUsers) {
                            $scope.PopUpwidth = 450;
                            $scope.PopUpheight = 400;
                            $scope.PopupTitle = "إسناد المستخدم علي مرحلة إعتماد";
                            $scope.PopupContent = "AdvanceUsersContent";
                            $scope.ButtonIcon = "save";
                            $scope.ButtonText = "إسناد";
                        }
                        else {
                            return swal("تنبيه", "عفوا ليس لديك صلاحية علي هذه العملية", "warning");

                        }

                    }
                    else if ($scope.selectedItem === 1) {
                        if ($scope.Permissions.AddNewPhase) {
                            $scope.PopUpwidth = 450;
                            $scope.PopUpheight = 400;
                            $scope.PopupTitle = "إضافة مرحلة إعتماد جديدة";
                            $scope.PopupContent = "AdvancePhasesContent";
                            $scope.ButtonIcon = "save";
                            $scope.ButtonText = "إضافة";
                        }
                        else {
                            return swal("تنبيه", "عفوا ليس لديك صلاحية علي هذه العملية", "warning");

                        }
                    }
                    else if ($scope.selectedItem === 3) {
                        if ($scope.Permissions.ReorderPhases) {
                            $scope.PopUpwidth = 500;
                            $scope.PopUpheight = 480;
                            $scope.PopupTitle = "تعديل ترتيب المراحل";
                            $scope.PopupContent = "AdvancePhasesEditingContent";
                            $scope.ButtonIcon = "edit";
                            $scope.ButtonText = "تعديل";
                        }
                        else {
                            return swal("تنبيه", "عفوا ليس لديك صلاحية علي هذه العملية", "warning");

                        }
                    }
                    $scope.PopUpShow = true;
                }
            };

            // Users Control...

            var DataSourceUsers = new DevExpress.data.DataSource({
                paginate: true,
                cacheRawData: true,
                key: "ID",
                loadMode: "raw",
                group: "IsAdmin",
                load: function () {
                    debugger;
                    return $.getJSON("/Advances/GetAllUsers", function (data) { debugger; });
                }
            });
            $scope.DL_Users = {
                dataSource: DataSourceUsers,
                bindingOptions: {
                    value: "MDL_UserId"
                },
                placeholder: 'اختر',
                noDataText: "لا يوجد بيانات",
                displayExpr: "Name",
                valueExpr: "ID",
                showBorders: true,
                searchEnabled: true,
                showClearButton: true,
                rtlEnabled: true,
                grouped: true,
                onValueChanged: function (e) {
                    $scope.MDL_UserId = e.value;
                }
            };


            //AdvancePhases Control...

            var DataSourceAdvancePhases = new DevExpress.data.DataSource({
                paginate: true,
                cacheRawData: false,
                key: "ID",
                loadMode: "raw",
                group: "Order",
                load: function () {
                    debugger;
                    return $.getJSON("/Advances/GetAllAdvancePhasesDDL", function (data) { });
                }
            });
            $scope.DL_AdvancePhases = {
                dataSource: DataSourceAdvancePhases,
                bindingOptions: {
                    value: "MDL_AdvancePhaseId",
                    text: "MDL_AdvancePhaseName"
                },
                placeholder: 'اختر',
                noDataText: "لا يوجد بيانات",
                displayExpr: "PhaseName",
                valueExpr: "ID",
                showBorders: true,
                searchEnabled: true,
                showClearButton: true,
                rtlEnabled: true,
                grouped: true,
                onValueChanged: function (e) {
                    debugger;
                    $scope.MDL_AdvancePhaseId = e.value;
                    $scope.AdvancePhaseId = e.value;

                    if ($scope.selectedItem === 3) {
                        if (e.value !== null && e.value !== '' && e.value !== undefined) {
                            StudentAdvanceSrvc.GetAdvancePhaseById($scope.MDL_AdvancePhaseId)
                                .then(function (data) {
                                    debugger;
                                    $scope.MDL_IsFinancialPhase = data.data.IsFinancialPhase;
                                    $scope.MDL_AdvancePhaseName = data.data.PhaseName;
                                    $scope.MDL_PhaseOrder = data.data.Order;
                                });
                        } else {
                            $scope.MDL_PhaseOrder = '';
                            $scope.MDL_AdvancePhaseName = '';
                            $scope.MDL_IsFinancialPhase = '';
                        }
                    }
                }
            };



            $scope.PopupOptions = {
                showTitle: true,
                dragEnabled: false,
                shadingColor: "rgba(0, 0, 0, 0.69)",
                closeOnOutsideClick: false,
                bindingOptions: {
                    visible: "PopUpShow",
                    contentTemplate: "PopupContent",
                    title: "PopupTitle",
                    width: "PopUpwidth",
                    height: "PopUpheight"
                },
                width: 1240,
                height: 500,
                rtlEnabled: true,
                onHiding: function () {

                    if ($scope.selectedItem === 1 || $scope.selectedItem === 3) {
                        $scope.AdvancePhaseId = '';
                        $scope.MDL_PhaseOrder = '';
                        $scope.MDL_IsFinancialPhase = '';
                        $scope.MDL_AdvancePhaseName = '';
                    }
                    else if ($scope.selectedItem === 2) {
                        $scope.MDL_UserId = '';
                        $scope.AdvanceUserId = '';
                        $scope.MDL_AdvancePhaseId = '';
                    }
                    var validationGroup = DevExpress.validationEngine.getGroupConfig("AdvancePhasesForm_VR");
                    validationGroup.reset();

                }
            };


            $scope.TB_AdvancePhase = {
                bindingOptions: {
                    value: "MDL_AdvancePhaseName"
                },
                onValueChanged: function (e) {
                    $scope.MDL_AdvancePhaseName = e.value;
                },
                placeholder: "",
                rtlEnabled: true
            };

            $scope.NB_PhaseOrder = {
                bindingOptions: {
                    value: "MDL_PhaseOrder"
                },
                onValueChanged: function (e) {
                    $scope.MDL_PhaseOrder = e.value;
                },
                placeholder: "",
                min: 1,
                rtlEnabled: true,
                showSpinButtons: true,
                showClearButton: true
            };

            $scope.MDL_IsFinancialPhase = false;
            $scope.CB_IsFinancialPhase = {
                bindingOptions: {
                    value: "MDL_IsFinancialPhase"
                },
                onValueChanged: function (e) {
                    $scope.MDL_IsFinancialPhase = e.value;
                }
            };

            // Saving button ...
            $scope.btnSaveOptions = {
                bindingOptions: {
                    //icon: "ButtonIcon",
                    text: "ButtonText"
                },
                visible: true,
                type: 'success',
                validationGroup: "AdvancePhasesForm_VR",
                useSubmitBehavior: true,
                onClick: function (e) {
                    debugger;
                    var validationGroup = DevExpress.validationEngine.getGroupConfig("AdvancePhasesForm_VR");
                    if (validationGroup) {
                        var result = validationGroup.validate();
                        if (result.isValid) {
                            debugger;
                            if ($scope.selectedItem === 1 || $scope.selectedItem === 3) {

                                if ($scope.MDL_AdvancePhaseName === '' || $scope.MDL_AdvancePhaseName === null || $scope.MDL_AdvancePhaseName === undefined) return;
                                if ($scope.MDL_PhaseOrder === '' || $scope.MDL_PhaseOrder === null || $scope.MDL_PhaseOrder === undefined) return;
                                debugger;
                                StudentAdvanceSrvc.SaveAdvancePhases($scope.AdvancePhaseId, $scope.MDL_AdvancePhaseName, $scope.MDL_PhaseOrder, $scope.MDL_IsFinancialPhase)
                                    .then(function (data) {
                                        if (data.data !== "") { swal("حدث خطأ", data.data, "error"); }
                                        else {
                                            swal("Done!", "تم الحفظ بنجاح", "success");

                                            $scope.AdvancePhaseId = '';
                                            $scope.MDL_PhaseOrder = '';
                                            $scope.MDL_AdvancePhaseName = '';

                                            validationGroup.reset();
                                            DataSourceAdvancePhases.reload();
                                            DataSourceAdvancePhasesGrid.reload();
                                        }
                                    });
                            } else if ($scope.selectedItem === 2) {

                                if ($scope.MDL_AdvancePhaseId === '' || $scope.MDL_AdvancePhaseId === null || $scope.MDL_AdvancePhaseId === undefined) return;
                                if ($scope.MDL_UserId === '' || $scope.MDL_UserId === null || $scope.MDL_UserId === undefined) return;

                                StudentAdvanceSrvc.SaveAdvanceUser($scope.AdvanceUserId, $scope.MDL_AdvancePhaseId, $scope.MDL_UserId)
                                    .then(function (data) {
                                        if (data.data !== "") { swal("حدث خطأ", data.data, "error"); }
                                        else {
                                            swal("Done!", "تم الحفظ بنجاح", "success");

                                            $scope.MDL_UserId = '';
                                            $scope.AdvanceUserId = '';
                                            $scope.MDL_AdvancePhaseId = '';

                                            validationGroup.reset();
                                            DataSourceAdvancePhasesGrid.reload();
                                        }
                                    });
                            }
                        }
                    }
                }
            };

            var DataSourceAdvancePhasesGrid = new DevExpress.data.DataSource({
                paginate: true,
                cacheRawData: false,
                key: "AdvanceUserId",
                loadMode: "raw",
                load: function () {
                    return $.getJSON("/Advances/GetAllAdvancePhasesGridSource", function (data) { });
                }
            });


            $scope.AdvancePhasesGrid = {
                dataSource: DataSourceAdvancePhasesGrid,
                bindingOptions: {
                    rtlEnabled: true
                },
                sorting: {
                    mode: "multiple"
                },
                wordWrapEnabled: false,
                showBorders: false,
                searchPanel: {
                    visible: true,
                    placeholder: "بحث"
                    //width: '100%'
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
                    visible: true,
                    emptyPanelText: "اسحب عمود هنا"
                },
                noDataText: "لايوجد بيانات",
                columnAutoWidth: true,
                //width: 100,
                columnChooser: {
                    enabled: true
                },
                "export": {
                    enabled: true,
                    fileName: "AdvancePhases"
                },
                onCellPrepared: function (e) {

                    if (e.rowType === "header" && e.column.command === "edit") {
                        debugger;
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
                editing: {
                    allowUpdating: true,
                    allowAdding: false,
                    allowDeleting: false,
                    texts: {
                        confirmDeleteMessage: "تأكيد حذف  !",
                        deleteRow: "حذف",
                        editRow: "تعديل",
                        addRow: "إضافة"
                    },
                    useIcons: true
                },
                columns: [
                    {
                        dataField: "Order",
                        caption: " المرحلة رقم  ",
                        alignment: "center",
                        groupIndex: 0
                    },
                    {
                        dataField: "PhaseName",
                        caption: "مرحلة",
                        alignment: "center",
                        groupIndex: 1
                    },
                    {
                        dataField: "UserName",
                        caption: "إسم المستخدم",
                        alignment: "center"
                    },
                    {
                        dataField: "IsAdmin",
                        caption: "من مدراء النظام",
                        alignment: "center",
                        dataType: "boolean"
                    },
                    {
                        dataField: "UserStatus",
                        alignment: "center",

                        caption: "تفعيل و إيقاف المستخدم",
                        cssClass: "text-center",
                        //width: 220,
                        cellTemplate: function (container, options) {
                            debugger;
                            var div = $("<div />").appendTo(container);
                            div.dxSwitch({
                                value: options.value,
                                width: 50,
                                disabled: !$scope.Permissions.OnOffPhase,
                                onOptionChanged: function (e) {
                                    debugger;
                                    if (!$scope.Permissions.OnOffPhase) {
                                        gridDataSource.reload();
                                        $scope.AdvanceConfigGridInstance.refresh();
                                        return swal("تنبيه", "عفوا ليس لديك صلاحية علي هذه العملية", "warning");

                                    }
                                    else {
                                        StudentAdvanceSrvc.EditAdvanceUserStatus(options.data.AdvanceUserId, e.value)
                                            .then(function (data) {
                                                if (data.data !== "") { swal("حدث خطأ", data.data, "error"); }
                                                else {
                                                    swal("Done!", e.value === true ? "تم تفعيل الموظف" : "تم إيقاف الموظف", "success");
                                                    DataSourceAdvancePhasesGrid.reload();
                                                }
                                            });
                                    }
                                }
                            });
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
                onEditingStart: function (e) {
                    debugger;
                    e.cancel = true;
                    if (!e.data.UserStatus) {
                        return swal("تنبيه", "برجاء تفعيل المسؤول أولا حتي تتمكن من عملية التعديل", "warning");
                    }

                    StudentAdvanceSrvc.GetAdvanceUser(e.data.AdvanceUserId)
                        .then(function (data) {
                            debugger
                            $scope.PopUpShow = true;

                            $scope.selectedItem = 2;
                            $scope.PopUpwidth = 480;
                            $scope.PopUpheight = 400;
                            $scope.ButtonText = "تعديل";
                            $scope.PopupTitle = "تعديل إسناد المسؤول علي مرحلة الإعتماد";
                            $scope.PopupContent = "AdvanceUsersContent";

                            $scope.MDL_AdvancePhaseId = data.data.AdvancePhases_Id;
                            $scope.MDL_UserId = data.data.UserID;
                        });
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
                onContentReady: function (e) {
                    debugger;
                    e.component.columnOption("command:edit", "visible", $scope.Permissions.EditPhase);
                    e.component.columnOption("command:edit", "width", 80);
                }
            };

            $scope.VR_AdvancePhasesForm_Required = {
                validationRules: [{ type: "required", message: "حقل إلزامي" }],
                validationGroup: "AdvancePhasesForm_VR"
            };

        }]);
})();
