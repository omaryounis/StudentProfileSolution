(function () {
    app.controller("ActivityPhasesCtrl", ["$scope", "$http", "AcademicActivitiesSrvc",
        function ($scope, $http, AcademicActivitiesSrvc) {

            /*********************************** Permissions *********************************/
            $scope.Permissions = {
                AddActivityPhase: false,
                ActiveAndStopUser: false,
                EditActivityPhaseOrder: false,
                AttributeUserToActivityPhase: false,
                EditAttributeUserToActivityPhase: false
            };
            $http({
                method: "get",
                url: "/AcademicActivities/GetActivityPhasesPermissions?screenId=72"
            }).then(function (data) {                
                $scope.Permissions.AddActivityPhase = data.data.AddActivityPhase;
                $scope.Permissions.ActiveAndStopUser = data.data.ActiveAndStopUser;
                $scope.Permissions.EditActivityPhaseOrder = data.data.EditActivityPhaseOrder;
                $scope.Permissions.AttributeUserToActivityPhase = data.data.AttributeUserToActivityPhase;
                $scope.Permissions.EditAttributeUserToActivityPhase = data.data.EditAttributeUserToActivityPhase;
            });
            /*--------------------------------* Permissions *--------------------------------*/



            //=============
            // MenuHandling
            //=============
            const Menu = [
                { key: 1, text: "إضافة مرحلة جديدة" },
                { key: 3, text: "تعديل ترتيب المراحل" },
                { key: 2, text: "إسناد المسـتخدمين" }
            ];
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
                    $scope.selectedItem = e.itemData.key;

                    if ($scope.selectedItem === 2) {
                        if ($scope.Permissions.AttributeUserToActivityPhase === false) {
                            return swal("تنبيه", "لايوجد لديك صلاحية علي هذه الشاشة", "warning");
                        }
                        $scope.PopUpwidth = 450;
                        $scope.PopUpheight = 400;
                        $scope.PopupTitle = "إسناد المستخدم علي مرحلة إعتماد";
                        $scope.PopupContent = "ActivityUsersContent";
                        $scope.ButtonIcon = "save";
                        $scope.ButtonText = "إسناد";
                    }
                    else if ($scope.selectedItem === 1) {
                        if ($scope.Permissions.AddActivityPhase === false) {
                            return swal("تنبيه", "لايوجد لديك صلاحية علي هذه الشاشة", "warning");
                        }
                        $scope.PopUpwidth = 450;
                        $scope.PopUpheight = 400;
                        $scope.PopupTitle = "إضافة مرحلة إعتماد جديدة";
                        $scope.PopupContent = "ActivityPhasesContent";
                        $scope.ButtonIcon = "save";
                        $scope.ButtonText = "إضافة";
                    }
                    else if ($scope.selectedItem === 3) {
                        if ($scope.Permissions.EditActivityPhaseOrder === false) {
                            return swal("تنبيه", "لايوجد لديك صلاحية علي هذه الشاشة", "warning");
                        }
                        $scope.PopUpwidth = 500;
                        $scope.PopUpheight = 480;
                        $scope.PopupTitle = "تعديل ترتيب المراحل";
                        $scope.PopupContent = "ActivityPhasesEditingContent";
                        $scope.ButtonIcon = "edit";
                        $scope.ButtonText = "تعديل";
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
                    return $.getJSON("/AcademicActivities/GetAllUsers", function (data) { });
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


            //ActivityPhases Control...

            var DataSourceActivityPhases = new DevExpress.data.DataSource({
                paginate: true,
                cacheRawData: false,
                key: "ID",
                loadMode: "raw",
                group: "Order",
                load: function () {

                    return $.getJSON("/AcademicActivities/GetAllActivityPhasesDDL", function (data) { });
                }
            });
            $scope.DL_ActivityPhases = {
                dataSource: DataSourceActivityPhases,
                bindingOptions: {
                    value: "MDL_ActivityPhaseId",
                    text: "MDL_ActivityPhaseName"
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

                    $scope.MDL_ActivityPhaseId = e.value;
                    $scope.ActivityPhaseId = e.value;

                    if ($scope.selectedItem === 3) {
                        if (e.value !== null && e.value !== '' && e.value !== undefined) {
                            AcademicActivitiesSrvc.GetActivityPhaseById($scope.MDL_ActivityPhaseId)
                                .then(function (data) {

                                    $scope.MDL_ActivityPhaseName = data.data.PhaseName;
                                    $scope.MDL_PhaseOrder = data.data.Order;
                                });
                        } else {
                            $scope.MDL_PhaseOrder = '';
                            $scope.MDL_ActivityPhaseName = '';
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
                        $scope.ActivityPhaseId = '';
                        $scope.MDL_PhaseOrder = '';
                        $scope.MDL_ActivityPhaseName = '';
                    }
                    else if ($scope.selectedItem === 2) {
                        $scope.MDL_UserId = '';
                        $scope.ActivityUserId = '';
                        $scope.MDL_ActivityPhaseId = '';
                    }
                    var validationGroup = DevExpress.validationEngine.getGroupConfig("ActivityPhasesForm_vg");
                    validationGroup.reset();

                }
            };

            $scope.TB_ActivityPhase = {
                bindingOptions: {
                    value: "MDL_ActivityPhaseName"
                },
                onValueChanged: function (e) {
                    $scope.MDL_ActivityPhaseName = e.value;
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


            // Saving button ...
            $scope.btnSaveOptions = {
                bindingOptions: {
                    //icon: "ButtonIcon",
                    text: "ButtonText"
                },
                visible: true,
                type: 'success',
                validationGroup: "ActivityPhasesForm_vg",
                useSubmitBehavior: true,
                onClick: function (e) {

                    var validationGroup = DevExpress.validationEngine.getGroupConfig("ActivityPhasesForm_vg");
                    if (validationGroup) {
                        var result = validationGroup.validate();
                        if (result.isValid) {
                            
                            // خلي بالك أنا بستخدم نفس
                            // Action سواء في عملية الإضافة أو عملية تبديل ترتيب المراحل

                            if ($scope.selectedItem === 1 || $scope.selectedItem === 3) {

                                if ($scope.MDL_ActivityPhaseName === '' || $scope.MDL_ActivityPhaseName === null || $scope.MDL_ActivityPhaseName === undefined) return;
                                if ($scope.MDL_PhaseOrder === '' || $scope.MDL_PhaseOrder === null || $scope.MDL_PhaseOrder === undefined) return;

                                if ($scope.selectedItem === 3) {
                                    debugger;
                                    var dresult = DevExpress.ui.dialog.confirm(`هل تريد تبديل ترتيب هذه المرحلة مع المرحلة رقم ${$scope.MDL_PhaseOrder}؟`, `تأكيد عملية التبديل`);
                                    dresult.done(function (dialogResult) {
                                        if (dialogResult === true) {
                                            AcademicActivitiesSrvc.SaveActivityPhases($scope.ActivityPhaseId, $scope.MDL_ActivityPhaseName, $scope.MDL_PhaseOrder)
                                                .then(function (data) {
                                                    if (data.data !== "") { swal("حدث خطأ", data.data, "error"); }
                                                    else {
                                                        swal("Done!", "تم الحفظ بنجاح", "success");

                                                        $scope.ActivityPhaseId = '';
                                                        $scope.MDL_PhaseOrder = '';
                                                        $scope.MDL_ActivityPhaseName = '';

                                                        validationGroup.reset();
                                                        DataSourceActivityPhases.reload();
                                                        DataSourceActivityPhasesGrid.reload();
                                                    }
                                                });
                                        }
                                    });
                                } else {
                                    AcademicActivitiesSrvc.SaveActivityPhases($scope.ActivityPhaseId, $scope.MDL_ActivityPhaseName, $scope.MDL_PhaseOrder)
                                        .then(function (data) {
                                            if (data.data !== "") { swal("حدث خطأ", data.data, "error"); }
                                            else {
                                                swal("Done!", "تم الحفظ بنجاح", "success");

                                                $scope.ActivityPhaseId = '';
                                                $scope.MDL_PhaseOrder = '';
                                                $scope.MDL_ActivityPhaseName = '';

                                                validationGroup.reset();
                                                DataSourceActivityPhases.reload();
                                                DataSourceActivityPhasesGrid.reload();
                                            }
                                        });
                                }

                               
                            } else if ($scope.selectedItem === 2) {

                                if ($scope.MDL_ActivityPhaseId === '' || $scope.MDL_ActivityPhaseId === null || $scope.MDL_ActivityPhaseId === undefined) return;
                                if ($scope.MDL_UserId === '' || $scope.MDL_UserId === null || $scope.MDL_UserId === undefined) return;

                                AcademicActivitiesSrvc.SaveActivityUser($scope.ActivityUserId, $scope.MDL_ActivityPhaseId, $scope.MDL_UserId)
                                    .then(function (data) {
                                        if (data.data !== "") { swal("حدث خطأ", data.data, "error"); }
                                        else {
                                            swal("Done!", "تم الحفظ بنجاح", "success");

                                            $scope.MDL_UserId = '';
                                            $scope.ActivityUserId = '';
                                            $scope.MDL_ActivityPhaseId = '';

                                            validationGroup.reset();
                                            DataSourceActivityPhasesGrid.reload();
                                        }
                                    });
                            }
                        }
                    }
                }
            };

            $scope.VR_ActivityPhasesForm_Required = {
                validationRules: [{ type: "required", message: "حقل إلزامي" }],
                validationGroup: "ActivityPhasesForm_vg"
            };


            var DataSourceActivityPhasesGrid = new DevExpress.data.DataSource({
                paginate: true,
                cacheRawData: false,
                key: "ActivityUserId",
                loadMode: "raw",
                load: function () {
                    return $.getJSON("/AcademicActivities/GetAllActivityPhasesGridSource", function (data) { });
                }
            });
            $scope.ActivityPhasesGrid = {
                dataSource: DataSourceActivityPhasesGrid,
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
                columnChooser: {
                    enabled: true
                },
                "export": {
                    enabled: true,
                    fileName: "ActivityPhases"
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
                        alignment: "right",
                        groupIndex: 0
                    },
                    {
                        dataField: "PhaseName",
                        caption: "مرحلة",
                        alignment: "right",
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
                        cellTemplate: function (container, options) {
                            var div = $("<div />").appendTo(container);
                            div.dxSwitch({
                                bindingOptions: {
                                    disabled: "!Permissions.ActiveAndStopUser"
                                },
                                value: options.value,
                                width: 50,
                                onOptionChanged: function (e) {

                                    AcademicActivitiesSrvc.EditActivityUserStatus(options.data.ActivityUserId, e.value)
                                        .then(function (data) {
                                            if (data.data !== "") { swal("حدث خطأ", data.data, "error"); }
                                            else {
                                                swal("Done!", e.value === true ? "تم تفعيل الموظف" : "تم إيقاف الموظف", "success");
                                                DataSourceActivityPhasesGrid.reload();
                                            }
                                        });
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

                    e.cancel = true;

                    AcademicActivitiesSrvc.GetActivityUser(e.data.ActivityUserId)
                        .then(function (data) {

                            $scope.PopUpShow = true;

                            $scope.selectedItem = 2;
                            $scope.PopUpwidth = 480;
                            $scope.PopUpheight = 400;
                            $scope.ButtonText = "تعديل";
                            $scope.PopupTitle = "تعديل إسناد المسؤول علي مرحلة الإعتماد";
                            $scope.PopupContent = "ActivityUsersContent";

                            $scope.MDL_ActivityPhaseId = data.data.ActivityPhases_Id;
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
                    e.component.columnOption("command:edit", "visible", $scope.Permissions.EditAttributeUserToActivityPhase);
                    e.component.columnOption("command:edit", "width", 80);
                }
            };

        }
    ]);
})();
