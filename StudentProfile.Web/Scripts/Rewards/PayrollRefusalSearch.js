(function () {
    app.controller("PayrollRefusalSearch", ["$scope", "$http", "$timeout", function ($scope, $http, $timeout) {
        $scope.PayRollDateFrom = null;
        $scope.PayRollDateTo = null;
        /*********************************** Permissions *********************************/
        $scope.Permissions = {
            Create: false,
            Read: false,
            Update: false,
            Delete: false,
            View: false
        };
        $scope.GetPermssions = function () {
            $http({
                method: "POST",
                url: "/Rewards/GetPermissions",
                params: { screenId: 91 }
            }).then(function (data) {
                $scope.Permissions.Create = data.data.CreateReward;
                $scope.Permissions.Read = data.data.ReadReward;
                $scope.Permissions.Update = data.data.UpdateReward;
                $scope.Permissions.Delete = data.data.DeleteReward;
                $scope.Permissions.View = data.data.ViewReward;
            });
        };
        //$scope.GetPermssions();
        /*--------------------------------* Permissions *--------------------------------*/


        /********************************** initial Variables *****************************/

        $scope.MDL_NationalityId = '';
        $scope.DL_NationalitiesInstance = '';
        $scope.NationalityIdsSelectedValues = '';

        $scope.MDL_FacultyBranch = '';
        $scope.FacultiesInstance = '';
        $scope.FacultiesSelectedValues = '';

        $scope.MDL_EducationLevel = '';
        $scope.StudentDegreesSelectedValues = '';

        $scope.MDL_StudentIds = '';
        $scope.StudentIdsSelectedValues = '';
        $scope.MDL_StudentStatusAcademy = '';
        $scope.StudentStatusAcademySelectedValues = '';

        $scope.RewardCategorySelectedValues = '';
        $scope.MDL_RewardCategory = '';

        $scope.PayrollRefusalList = [];
        $scope.RewardGridInstance = '';
        /*--------------------------------* initial Variables *--------------------------------*/


        /********************************** Data Source *****************************/

        var DataSourceFaculties = new DevExpress.data.DataSource({
            paginate: false,
            cacheRawData: true,
            key: "FacultyBranchId",
            loadMode: "raw",
            load: function () {
                return $.getJSON("/Rewards/GetAllFaculties", function (data) { });
            }
        });

        var DataSourceStudentDegrees = new DevExpress.data.DataSource({
            paginate: false,
            cacheRawData: true,
            key: "EducationLevelId",
            loadMode: "raw",
            load: function () {
                return $.getJSON("/Rewards/GetAllEducationLevels", function (data) { });
            }
        });

        var DataSourceStudentStatusAcademy = new DevExpress.data.DataSource({
            paginate: false,
            cacheRawData: true,
            key: "RegisterTypeId",
            loadMode: "raw",
            load: function () {
                return $.getJSON("/Rewards/GetAllRegisterTypes", function (data) { });
            }
        });

        var DataSourceNationalities = new DevExpress.data.DataSource({
            paginate: false,
            cacheRawData: true,
            key: "NationalityId",
            loadMode: "raw",
            load: function (e) {
                return $.getJSON("/Rewards/GetAllNationalities", function (data) { });
            }
        });

        var DataSourceRewardCategoriesDistinct = new DevExpress.data.DataSource({
            paginate: false,
            cacheRawData: true,
            key: "RewardCategoryId",
            loadMode: "raw",
            load: function () {
                return $.getJSON("/Rewards/GetAllRewardcategoriesDistinct", function (data) { });
            }
        });

        var DataSourceStudents = new DevExpress.data.DataSource({
            paginate: true,
            cacheRawData: true,
            key: "StudentId",
            loadMode: "raw",
            load: function () {
                return $.getJSON("/Rewards/GetAllStudents", function (data) { });
            }
        });
        //DataSourceStudents.reload();
        var DataSourceRewardItems = new DevExpress.data.DataSource({
            paginate: false,
            cacheRawData: true,
            key: "AllowanceId",
            loadMode: "raw",
            load: function () {
                return $.getJSON("/Rewards/GetAllRewardItems", function (data) { });
            }
        });

        $scope.firstLoad = true;
        var DataSourcePayrollRefusalGrid = new DevExpress.data.DataSource({
            paginate: true,
            cacheRawData: true,
            type: "array",
            key: "PayrollRefusalID",
            loadMode: "raw",
            load: function () {
                if ($scope.firstLoad) {
                    $scope.firstLoad = false;
                    return []
                } else {
                    return $scope.PayrollRefusalList;
                }

            }
        });
        /*--------------------------------* Data Source *--------------------------------*/

        $scope.DL_Faculties = {
            dataSource: DataSourceFaculties,
            bindingOptions: {
                value: "MDL_FacultyBranch"
            },
            displayExpr: "FacultyBranchName",
            valueExpr: "FacultyBranchId",
            searchEnabled: true,
            showClearButton: true,
            rtlEnabled: true,
            placeholder: "اختر",
            noDataText: "لا يوجد بيانات",
            onInitialized: function (e) {
                $scope.FacultiesInstance = e.component;
            },
            multiline: false,
            showBorders: true,
            showSelectionControls: true,
            showDropDownButton: true,
            onValueChanged: function (e) {
                $scope.FacultiesSelectedValues = e.value.join(',');
            },
            onClosed: function (e) {
                //  $scope.FacultiesSelectedValues = e.model.MDL_FacultyBranch.join(',');
            }
        };
        $scope.DL_StudentDegrees = {
            bindingOptions: {
                value: "MDL_EducationLevel"
            },
            dataSource: DataSourceStudentDegrees,
            displayExpr: "EducationLevelName",
            valueExpr: "EducationLevelId",
            searchEnabled: true,
            showClearButton: true,
            rtlEnabled: true,
            placeholder: "اختر",
            noDataText: "لا يوجد بيانات",
            multiline: false,
            showBorders: true,
            showSelectionControls: true,
            showDropDownButton: true,
            onValueChanged: function (e) {
                //  $scope.StudentDegreesSelectedValues = e.value.join(',');
            },
            onClosed: function (e) {
                $scope.StudentDegreesSelectedValues = e.model.MDL_EducationLevel.join(',');
            }
        };
        $scope.DL_Students = {
            dataSource: DataSourceStudents,
            bindingOptions: {
                value: "MDL_StudentIds"
            },
            placeholder: "بحث بالإسـم   _   برقم الهوية   _   بالرقم الجامعي",
            noDataText: "لا يوجد بيانات",
            multiline: false,
            displayExpr: "StudentName",
            valueExpr: "StudentID",
            showBorders: true,
            searchEnabled: true,
            showClearButton: true,
            rtlEnabled: true,
            showSelectionControls: true,
            maxDisplayedTags: 0,
            showMultiTagOnly: false,
            onValueChanged: function (e) {
                $scope.StudentIdsSelectedValues = e.value.join(',');
            },
            onClosed: function (e) {
                debugger;
                //  $scope.StudentIdsSelectedValues = e.model.MDL_StudentIds.join(',');
            }
        };
        $scope.DL_StudentStatusAcademy = {
            bindingOptions: {
                value: "MDL_StudentStatusAcademy"
            },
            dataSource: DataSourceStudentStatusAcademy,
            displayExpr: "RegisterTypeName",
            valueExpr: "RegisterTypeId",
            searchEnabled: true,
            showClearButton: true,
            rtlEnabled: true,
            placeholder: "اختر",
            noDataText: "لا يوجد بيانات",
            showMultiTagOnly: false,
            multiline: false,
            showBorders: true,
            showSelectionControls: true,
            maxDisplayedTags: 1,
            selectAllMode: "allPages",
            showDropDownButton: true,
            onValueChanged: function (e) {
                $scope.StudentStatusAcademySelectedValues = e.value.join(',');
            },
            onClosed: function (e) {
                // $scope.StudentStatusAcademySelectedValues = e.model.MDL_StudentStatusAcademy.join(',');
            }
        };
        $scope.DL_Nationalities = {
            dataSource: DataSourceNationalities,
            bindingOptions: {
                value: "MDL_NationalityId"
            },
            displayExpr: "NationalityName",
            valueExpr: "NationalityId",
            searchEnabled: true,
            showClearButton: true,
            rtlEnabled: true,
            placeholder: "اختر",
            noDataText: "لا يوجد بيانات",
            multiline: false,
            showBorders: true,
            maxDisplayedTags: 0,
            showSelectionControls: true,
            showDropDownButton: true,
            onValueChanged: function (e) {
                $scope.NationalityIdsSelectedValues = e.value.join(',');
            },
            onInitialized: function (e) {
                $scope.DL_NationalitiesInstance = e.component;
            },
            onClosed: function (e) {
                // $scope.NationalityIdsSelectedValues = e.model.MDL_NationalityId.join(',');
            }
        };
        $scope.DL_RewardItems = {
            bindingOptions: {
                value: "MDL_Allowance"
            },
            dataSource: DataSourceRewardItems,
            displayExpr: "AllowanceName",
            valueExpr: "AllowanceId",
            searchEnabled: true,
            showClearButton: true,
            rtlEnabled: true,
            placeholder: "اختر",
            noDataText: "لا يوجد بيانات",
            multiline: false,
            showBorders: true,
            showSelectionControls: true,
            maxDisplayedTags: 1,
            paginate: true,
            showDropDownButton: true,
            onValueChanged: function (e) {
                $scope.RewardItemsSelectedValues = e.value.join(',');
            },
            onClosed: function (e) {
                //  $scope.RewardItemsSelectedValues = e.model.MDL_Allowance.join(',');
            },
            onInitialized: function (e) {
            }
        };
        $scope.DL_RewardCategories = {
            dataSource: DataSourceRewardCategoriesDistinct,
            bindingOptions: {
                value: "MDL_RewardCategory"
            },
            displayExpr: "RewardCategoryName",
            valueExpr: "RewardCategoryId",
            searchEnabled: true,
            showClearButton: true,
            rtlEnabled: true,
            placeholder: "اختر",
            noDataText: "لا يوجد بيانات",
            multiline: false,
            showBorders: true,
            showSelectionControls: true,
            maxDisplayedTags: 1,
            showDropDownButton: true,
            onValueChanged: function (e) {
                $scope.RewardCategorySelectedValues = e.value.join(',');
            },
            onClosed: function (e) {
                //   $scope.RewardCategorySelectedValues = e.model.MDL_RewardCategory.join(',');
            }
        };

        // Date...
        $scope.Db_PayRollDateFrom = {
            bindingOptions: {
                value: "PayRollDateFrom"
            },
            type: "date",
            displayFormat: "dd/MM/yyyy",

            onValueChanged: function (e) {
                debugger;
                $scope.PayRollDateFrom = e.value;
            }
        };

        $scope.Db_PayRollDateTo = {
            bindingOptions: {
                value: "PayRollDateTo"
            },
            type: "date",
            displayFormat: "dd/MM/yyyy",
            onValueChanged: function (e) {
                debugger;
                $scope.PayRollDateTo = e.value;
            }
        };
        function convert(str) {
            var date = new Date(str),
                mnth = ("0" + (date.getMonth() + 1)).slice(-2),
                day = ("0" + date.getDate()).slice(-2);
            return [day, mnth, date.getFullYear()].join("/");
        }

        $scope.btn_PayrollRefusalForm = {
            text: "بحث",
            type: 'success',
            icon: 'search',
            useSubmitBehavior: true,
            onClick: function (e) {
                debugger;

                
                if ($scope.PayRollDateFrom === null || $scope.PayRollDateFrom === '' || $scope.PayRollDateFrom ===undefined ) {
                    return DevExpress.ui.notify({ message: "برجاء إختيار تاريخ البداية" }, "Error", 10000);
                }

                if ($scope.PayRollDateTo === null || $scope.PayRollDateTo === '' || $scope.PayRollDateTo === undefined) {
                    return DevExpress.ui.notify({ message: "برجاء إختيار تاريخ النهاية" }, "Error", 10000);
                }
                if (new Date($scope.PayRollDateFrom) > new Date($scope.PayRollDateTo)) {
                    return DevExpress.ui.notify({ message: "لا يمكن أن يكون تاريخ البداية بعد تاريخ النهاية" }, "Error", 10000);
                }

                if ($scope.PayRollDateFrom && !$scope.PayRollDateTo) {
                    return DevExpress.ui.notify({ message: "برجاء إختيار تاريخ النهاية" }, "Error", 10000);
                }

                if (!$scope.PayRollDateFrom && $scope.PayRollDateTo) {
                    return DevExpress.ui.notify({ message: "برجاء إختيار تاريخ البداية" }, "Error", 10000);
                }

                debugger;
                $http({
                    method: "GET",
                    url: "/Rewards/GetPayrollRefusal",
                    params: {
                        FacultyIds: $scope.FacultiesSelectedValues === '' ? null : $scope.FacultiesSelectedValues,
                        StudentDegrees: $scope.StudentDegreesSelectedValues === '' ? null : $scope.StudentDegreesSelectedValues,
                        StudentIds: $scope.StudentIdsSelectedValues === '' ? null : $scope.StudentIdsSelectedValues,
                        StudentStatusAcademyIds: $scope.StudentStatusAcademySelectedValues === '' ? null : $scope.StudentStatusAcademySelectedValues,
                        NationalityIds: $scope.NationalityIdsSelectedValues === '' ? null : $scope.NationalityIdsSelectedValues,
                        RewardItemIds: $scope.RewardItemsSelectedValues === '' ? null : $scope.RewardItemsSelectedValues,
                        RewardCategoryIds: $scope.RewardCategorySelectedValues === '' ? null : $scope.RewardCategorySelectedValues,
                        PayRollDateFrom: $scope.PayRollDateFrom === '' ? null : $scope.PayRollDateFrom,
                        PayRollDateTo: $scope.PayRollDateTo === '' ? null : $scope.PayRollDateTo
                    }
                }).then(function (data) {
                    $scope.PayrollRefusalList = '';
                    $scope.PayrollRefusalList = data.data;
                    $scope.RewardGridInstance.refresh();
                });
            }
        };

        /********************************* PayrollRefusal Grid ***********************************/

        $scope.PayrollRefusalGrid = {
            dataSource: DataSourcePayrollRefusalGrid,

            bindingOptions: {
                rtlEnabled: true
            },
            sorting: {
                mode: "multiple"
            },
            wordWrapEnabled: false,
            showBorders: true,
            searchPanel: {
                visible: true,
                placeholder: "بحث",
                width: 300
            },
            paging: {
                pageSize: 15
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
            noDataText: 'لايوجد بيانات',
            columnAutoWidth: true,
            cssClass: "text-center",
            width: "auto",
            columnChooser: {
                enabled: true
            },
            "export": {
                enabled: true,
                fileName: "أسباب عدم الاستحقاق"
                //allowExportSelectedData: true
            },
            columns: [
                {
                    dataField: "student_id",
                    caption: "الرقم الجامعي"

                }, {
                    dataField: "student_name",
                    caption: "اسم الطالب"

                },
                {
                    dataField: "national_id",
                    caption: "رقم الهوية",
                    groupIndex: 0
                },
                {
                    dataField: "payrollnumber",
                    caption: "رقم المسير"
                },
                {
                    dataField: "RewardItemName",
                    caption: "اسم البند"
                },
                {
                    dataField: "refusalreason",
                    caption: "السبب"
                }
            ],
            allowColumnResizing: true,
            columnResizingMode: "widget",
            allowColumnReordering: true,
            showColumnLines: true,
            rowAlternationEnabled: true,
            onInitialized: function (e) {
                $scope.RewardGridInstance = e.component;
            },
            editing: {
                allowUpdating: false,
                allowAdding: false,
                allowDeleting: false,
                mode: "row",
                useIcons: true
            }
        };

        /*--------------------------------* PayrollRefusal Grid *--------------------------------*/
    }]);
})();

