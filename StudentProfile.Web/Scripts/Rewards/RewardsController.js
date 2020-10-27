(function () {
    app.controller("RewardCtrl", ["$scope", "$http", "$timeout", function ($scope, $http, $timeout) {

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
                url: "/Rewards/GetPermissionsFn",
                params: { screenId: 47 }
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

        $scope.MDL_RewardCategory = '';
        $scope.MDL_RewardCategoryText = null;
        $scope.DataSourceRewardCategories = [];

        $scope.MDL_FacultyBranch = '';
        $scope.MDL_FacultyBranchText = null;
        $scope.FacultyBranchesDataSource = [];

        $scope.MDL_EducationLevel = '';
        $scope.MDL_EducationLevelText = null;
        $scope.EducationLevelsDataSource = [];

        $scope.MDL_Allowance = '';
        $scope.MDL_AllowanceText = null;
        $scope.AllowancesDataSource = [];

        $scope.MDL_Amount = '';

        $scope.MDL_AmountType = '';
        $scope.MDL_AmountTypeText = null;
        $scope.AmountTypesDataSource = [];

        $scope.MDL_EducationTypeId = '';
        $scope.MDL_EducationTypeText = null;
        $scope.EducationTypesDataSource = [];

        $scope.MDL_FromRate = '';
        $scope.MDL_ToRate = '';

        $scope.MDL_StudentStatusAcademy = '';
        $scope.MDL_StudentStatusAcademyText = null;
        $scope.RegisterTypesDataSource = [];

        $scope.MDL_MiniHoursNo = '';
        $scope.MDL_ClassesNo = '';
        $scope.MDL_DuesNo = '';

        $scope.GridDataSource = [];
        $scope.EdittedRewardDetailsId = 0;

        $scope.MDL_DependOnEducationDays = true;
        $scope.MDL_DependOnEducationperiod = false;


        $scope.MDL_NationalityId = '';
        $scope.MDL_NationalityText = null;
        $scope.DataSourceNationalities = [];
        $scope.NationalityIdsSelectedValues = '';

        $scope.showCancelRewcatFormPopup = true;
        $scope.SaveRewcatFormPopup = true;
        $scope.NewRewcatFormPopup = true;
        $scope.DeleteRewcatFormPopup = false;

        $scope.RewardCategoryId = 0;
        $scope.ButtonName = ' حفظ ';


        $scope.RewardItemsId = 0;

        $scope.MDL_RewardItemName_Arb = null;
        $scope.MDL_RewardItemName_Eng = null;
        $scope.MDL_RewardItemTypeId = null;

        $scope.RewardItemTypeId = '';

        $scope.MDL_RewardItemExpensesTypeId = '';
        $scope.MDL_RewardItemExpensesTypeText = null;


        $scope.SaveRewItemsFormPopup = true;
        $scope.NewRewItemsFormPopup = true;
        $scope.DeleteRewItemsFormPopup = false;

        $scope.MDL_AfterCheckingResult = false;
        $scope.MDL_IsAdvanceReturn = false;
        $scope.btnRewardFormName = ' حفظ ';

        $scope.RewardDetailsId = 0;

        $scope.DependOnEducationDays = true;
        $scope.DependOnEducationPeriod = false;


        $scope.MDL_RateType = '';
        $scope.MDL_RateTypeText = null;
        $scope.RateTypesDataSource = [];

        $scope.BtnSubmitRewardItemName = ' حفظ ';
        $scope.SaveRewItemFormPopup = true;
        $scope.NewRewItemFormPopup = true;
        $scope.DeleteRewItemFormPopup = false;

        $scope.MDL_RegisterType = '';
        $scope.MDL_RegisterTypeText = null;
        $scope.DataSourceRegisterTypes = [];

        $scope.FacultyBranchesInstance = '';
        $scope.RewardGridInstance = '';

        $scope.TX_ClassesNoDisabled = false;
        $scope.TX_DuesNoFormatValue = '';

        $scope.RewardCategorySelectedValues = '';
        $scope.FacultyBranchSelectedValues = '';
        $scope.EducationLevelSelectedValues = '';
        $scope.StudentStatusAcademySelectedValues = '';

        $scope.RewardItemsGridInstance = '';
        $scope.ShowBtnCancelRewardDetails = false;

        $scope.MDL_Cat_Code_Integration = '';
        $scope.MDL_Cat_Text_Integration = '';

        /*showing Popup*/
        $scope.RewardCatPopUpShow = false;
        $scope.RewardItemsPopUpShow = false;
        $scope.RewardItemsPopUpTitle = null;
        $scope.RewardCatPopUpTitle = null;
        /**showing Popup **/

        /*--------------------------------* initial Variables *--------------------------------*/

        var DataSourceNationalities = new DevExpress.data.DataSource({
            paginate: false,
            cacheRawData: true,
            key: "NationalityId",
            loadMode: "raw",
            load: function (e) {
                return $.getJSON("/Rewards/GetAllNationalities", function (data) { });
            }
        });

        var DataSourceCategoriesIntegration = new DevExpress.data.DataSource({
            paginate: false,
            cacheRawData: false,
            key: "Cat_Code_Integration",
            loadMode: "raw",
            load: function (e) {
                return $.getJSON("/Rewards/GetAllRewardcategories_Integration", function (data) {
                    debugger;
                    if ($scope.CatFormIsEditing) {
                        data.push({ Cat_Code_Integration: $scope.MDL_Cat_Code_Integration, Cat_Name_Integration: $scope.MDL_Cat_Text_Integration });
                    }
                    return data;
                });
            }
        });

        var DataSourceRewardCategories = new DevExpress.data.DataSource({
            paginate: false,
            cacheRawData: true,
            key: "RewardCategoryId",
            loadMode: "raw",
            load: function () {
                return $.getJSON("/Rewards/GetAllRewardcategoriesDistinct", function (data) { });
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

        var DataSourceAllowances = new DevExpress.data.DataSource({
            paginate: false,
            cacheRawData: true,
            key: "AllowanceId",
            loadMode: "raw",
            load: function () {
                return $.getJSON("/Rewards/GetAllRewardItems", function (data) { });
            }
        });

        var DataSourceFacultyBranches = new DevExpress.data.DataSource({
            paginate: false,
            cacheRawData: true,
            key: "FacultyBranchId",
            loadMode: "raw",
            load: function () {
                return $.getJSON("/Rewards/GetAllFaculties", function (data) { });
            }
        });

        var DataSourceEducationLevels = new DevExpress.data.DataSource({
            paginate: false,
            cacheRawData: true,
            key: "EducationLevelId",
            loadMode: "raw",
            load: function () {
                return $.getJSON("/Rewards/GetAllEducationLevels", function (data) { });
            }
        });

        var DataSourceEducationTypes = new DevExpress.data.DataSource({
            paginate: false,
            cacheRawData: true,
            key: "EducationTypeId",
            loadMode: "raw",
            load: function () {
                return $.getJSON("/Rewards/GetAllEducationTypes", function (data) { });
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

        var DataSourceGrid = new DevExpress.data.DataSource({
            paginate: false,
            cacheRawData: true,
            key: "RewardDetailsId",
            loadMode: "raw",
            load: function () {
                return $.getJSON("/Rewards/GetAllRewardDetails",
                    function (data) { });
            }
        });

        var RewardItemTypesDataSource = new DevExpress.data.DataSource({
            paginate: false,
            cacheRawData: true,
            key: "ID",
            loadMode: "raw",
            load: function () {
                return $.getJSON("/Rewards/GetAllRewardItemTypes",
                    function (data) { });
            }
        });

        var RewardItemExpensesTypesDataSource = new DevExpress.data.DataSource({
            paginate: false,
            cacheRawData: true,
            key: "ID",
            loadMode: "raw",
            load: function () {
                return $.getJSON("/Rewards/GetAllRewardItemExpensesTypes",
                    function (data) { });
            }
        });

        $scope.AmountTypesDataSource = [{ AmountTypeId: 1, AmountTypeName: 'مقطوع' }, { AmountTypeId: 100, AmountTypeName: 'نسبة' } ];

        $scope.RateTypesDataSource = [{ RateTypeId: 1, RateTypeText: 'فصلي' }, { RateTypeId: 2, RateTypeText: 'تراكمي' }];

        $scope.Listradiogroup = ["الاحتساب طبقا لأيام الدراسـة", "الاحتساب طبقا لمدة الدراسـة"];

        $scope.DataSourceRegisterTypes = [{ RegisterTypeId: 1, RegisterTypeText: 'بعدد الساعات' }, { RegisterTypeId: 2, RegisterTypeText: 'المقررات' }];

        /*--------------------------------* DATASOURCES *--------------------------------*/


        /***************** Reward Category ********************/

        $scope.ResetReCatForm = function () {
            debugger;
            $scope.CatFormIsEditing = false;
            $scope.RewardCategoryId = 0;
            $scope.MDL_RewardCategory = '';
            $scope.MDL_NationalityId = '';
            $scope.MDL_CategoryName_Arb = null;
            $scope.MDL_CategoryName_Eng = null;
            $scope.MDL_Cat_Code_Integration = null;
            $scope.ButtonName = ' حفظ ';
            $scope.SaveRewcatFormPopup = true;
            $scope.NewRewcatFormPopup = false;
            $scope.DeleteRewcatFormPopup = false;
            $scope.RewardCatPopUpTitle = 'إضافة فئة مكافأة';
            $scope.NationalityIdsSelectedValues = '';
        };

        $scope.NewRewcatFormPopup = function () {
            $scope.ResetReCatForm();
            DevExpress.validationEngine.getGroupConfig("RewardCatForm_VR").reset();
        };

        $scope.btnShowRewardCategoryPopUp = {
            icon: 'fa fa-plus',
            type: 'success',
            useSubmitBehavior: false,
            onClick: function (e) {
                debugger;
                $scope.ResetReCatForm();
                $scope.RewardCatPopUpTitle = 'إضافة فئة مكافأة';
                $scope.RewardCatPopUpShow = true;
            }
        };

        $scope.popupOptions = {
            width: 1250,
            height: 600,
            contentTemplate: 'info',
            showTitle: true,
            dragEnabled: false,
            shadingColor: "rgba(0, 0, 0, 0.69)",
            closeOnOutsideClick: false,
            bindingOptions: {
                visible: "RewardCatPopUpShow",
                title: "RewardCatPopUpTitle"
            },
            rtlEnabled: true,
            onHiding: function () {
                $scope.ResetReCatForm();
                DataSourceRewardCategoriesDistinct.reload();
            }
        };

        $scope.submitButtonOptions_RewcatForm = {
            bindingOptions: {
                text: "ButtonName"
            },
            type: 'success',
            //icon: "check",
            useSubmitBehavior: true,
            validationGroup: "RewardCatForm_VR",
            onClick: function (e) {
                debugger;
                var PostRewardCatFormValidationGroup = DevExpress.validationEngine.getGroupConfig("RewardCatForm_VR");
                if (PostRewardCatFormValidationGroup) {
                    var result = PostRewardCatFormValidationGroup.validate();
                    if (result.isValid) {
                        if ($scope.NationalityIdsSelectedValues === null || $scope.NationalityIdsSelectedValues === undefined || $scope.NationalityIdsSelectedValues === '') {
                            return DevExpress.ui.notify({ message: "برجاء إختيار الجنسية" }, "Error", 10000);
                        }
                        if ($scope.MDL_Cat_Code_Integration === null || $scope.MDL_Cat_Code_Integration === undefined || $scope.MDL_Cat_Code_Integration === '') {
                            return DevExpress.ui.notify({ message: "برجاء إختيار إسم فئة المكافأة" }, "Error", 10000);
                        }

                        $http({
                            method: "Post",
                            url: "/Rewards/SaveRewardCaterory/",
                            data: {
                                RewardCategoryId: $scope.MDL_RewardCategory,
                                CategoryName_Arb: $scope.MDL_CategoryName_Arb,
                                CategoryName_Eng: $scope.MDL_CategoryName_Eng,
                                Nationality_ID: $scope.NationalityIdsSelectedValues, //$scope.MDL_NationalityId
                                Cat_Code_Integration: $scope.MDL_Cat_Code_Integration
                            }
                        }).then(function (data) {
                            debugger;
                            if (data.data !== "") {
                                DevExpress.ui.notify({ message: data.data }, "Error", 10000);
                            } else if (data.data === "") {
                                var _message = $scope.MDL_RewardCategory !== null ? "تم التعديل بنجاح" : "تم الحفظ بنجاح";
                                DevExpress.ui.notify({ message: _message }, "success", 10000);

                                $scope.CatFormIsEditing = false;
                                $scope.ResetReCatForm();
                                DevExpress.validationEngine.getGroupConfig("RewardCatForm_VR").reset();

                                DataSourceRewardCategories.reload();
                                $scope.RewardCategoryGridInstance.refresh();
                            }
                        });
                    }
                }
            }
        };

        $scope.btnNew_RewcatForm = {
            useSubmitBehavior: false,
            text: ' جديد ',
            type: 'default',
            onClick: function (e) {
                $scope.ResetReCatForm();
                DevExpress.validationEngine.getGroupConfig("RewardCatForm_VR").reset();
            }
        };

        $scope.btnDelete_RewcatForm = {
            useSubmitBehavior: false,
            text: ' حذف ',
            type: 'danger',
            onClick: function (e) { }
        };

        $scope.pop_Rewardcategory = {
            TX_CategoryName_Arb: {
                bindingOptions: { value: "MDL_CategoryName_Arb" },
                placeholder: "ادخل القيمة",
                showClearButton: true,
                rtlEnabled: true
            },
            VR_CategoryName_Arb: {
                validationRules: [{ type: "required", message: "حقل إلزامي" }],
                validationGroup: "RewardCatForm_VR"
            },
            TX_CategoryName_Eng: {
                bindingOptions: { value: "MDL_CategoryName_Eng" },
                placeholder: "ادخل القيمة",
                showClearButton: true,
                rtlEnabled: true
            },
            VR_CategoryName_Eng: {
                validationRules: [{ type: "required", message: "Required Field" }, {
                    type: "pattern",
                    pattern: /^[a-zA-Z0-9_ ]*$/i,
                    message: "only allow English characters and numbers for text input"
                }],
                validationGroup: "RewardCatForm_VR"
            },
            DL_Nationality: {
                dataSource: DataSourceNationalities,
                bindingOptions: {
                    value: "MDL_NationalityId",
                    text: "MDL_NationalityText"
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
                    debugger;
                    $scope.NationalityIdsSelectedValues = e.value.join(',');
                },
                onInitialized: function (e) {
                    $scope.DL_NationalityInstance = e.component;
                }
            },
            VR_Nationality: {
                validationRules: [{ type: "required", message: "حقل إلزامي" }],
                validationGroup: "RewardCatForm_VR"
            },
            DL_CategoriesIntegration: {
                dataSource: DataSourceCategoriesIntegration,
                bindingOptions: {
                    value: "MDL_Cat_Code_Integration",
                    text: "MDL_Cat_Name_Integration"
                },
                displayExpr: "Cat_Name_Integration",
                valueExpr: "Cat_Code_Integration",
                searchEnabled: true,
                showClearButton: true,
                rtlEnabled: true,
                placeholder: "اختر",
                noDataText: "لا يوجد بيانات",
                showBorders: true,
                onValueChanged: function () {
                    debugger;
                    DataSourceCategoriesIntegration.reload();
                }
            },
            VR_CategoriesIntegration: {
                validationRules: [{ type: "required", message: "حقل إلزامي" }],
                validationGroup: "RewardCatForm_VR"
            }
        };

        $scope.RewardCategoryGrid = {
            dataSource: DataSourceRewardCategories,
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
                placeholder: "بحث"
            },
            paging: {
                pageSize: 5
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
            noDataText: 'لا يوجد بيانات',
            columnAutoWidth: true,
            width: "auto",
            columnChooser: {
                enabled: true
            },
            "export": {
                enabled: true,
                fileName: "فئات المكافئات"
                //allowExportSelectedData: true
            },
            onCellPrepared: function (e) {
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
            },
            columns: [
                {
                    dataField: "RewardCategoryId",
                    visible: false
                },
                {
                    dataField: "Cat_Name_Integration",
                    caption: "إسم التصنيف"
                    //groupIndex: 0
                },
                {
                    dataField: "RewardCategoryName",
                    caption: "إسم الفئة بالعربي"
                    //groupIndex: 0
                },
                {
                    dataField: "RewardCategoryNameEng",
                    caption: "إسم الفئة بالإنجليزي"
                },
                {
                    dataField: "NationalityID",
                    visible: false
                },
                {
                    dataField: "NationalityDesc",
                    caption: "الجنسية"
                }
            ],
            allowColumnResizing: true,
            columnResizingMode: "widget",
            allowColumnReordering: true,
            showColumnLines: true,
            rowAlternationEnabled: true,
            onInitialized: function (e) {
                $scope.RewardCategoryGridInstance = e.component;
            },
            editing: {
                allowUpdating: true,
                allowAdding: false,
                allowDeleting: true,
                mode: "row",
                texts: {
                    confirmDeleteMessage: "تأكيد حذف  !",
                    addRow: "اضافة"
                },
                useIcons: true
            },
            onRowRemoving: function (e) {
                e.cancel = true;
                debugger;
                $http({
                    method: "Delete",
                    url: "/Rewards/DeleteRewardCategory",
                    data: { RewardCategoryId: e.data.RewardCategoryId }
                }).then(function (data) {
                    if (data.data !== "") {
                        return DevExpress.ui.notify({ message: data.data }, "Error", 10000);
                    } else {
                        DevExpress.ui.notify({ message: "تم الحذف بنجاح" }, "success", 10000);
                        DataSourceRewardCategories.reload();
                    }
                });
            },
            onRowRemoved: function (e) {
            },
            onEditingStart: function (e) {
                e.cancel = true;
                $scope.CatFormIsEditing = true;
                $scope.ButtonName = ' تعديل ';
                $scope.SaveRewcatFormPopup = true;
                $scope.NewRewcatFormPopup = true;
                $scope.RewardCatPopUpTitle = 'تعديل فئة مكافأة';

                $http({
                    method: "Get",
                    url: "/Rewards/GetRewardcategoryById",
                    params: { RewardcategoryId: e.data.RewardCategoryId }
                }).then(function (data) {
                    debugger;
                    $scope.MDL_RewardCategory = e.data.RewardCategoryId;
                    $scope.MDL_CategoryName_Arb = data.data.CategoryName_Arb;
                    $scope.MDL_CategoryName_Eng = data.data.CategoryName_Eng;
                    $scope.MDL_NationalityId = data.data.RewardNationaltities;
                    $scope.MDL_Cat_Code_Integration = data.data.Cat_Code_Integration;
                    $scope.MDL_Cat_Text_Integration = data.data.Cat_Name_Integration.toString();
                });
            }
        };
        /*--------------------------------* Reward Category *--------------------------------*/


        /***************** Reward Items ***********************/
        $scope.btnShowRewardItemsPopUp = {
            icon: 'fa fa-plus',
            type: 'success',
            useSubmitBehavior: false,
            onClick: function (e) {
                $scope.ResetRewardItemsForm();
                $scope.RewardItemsPopUpTitle = 'إضافة بنود المكافأة';
                $scope.RewardItemsPopUpShow = true;
            }
        };

        $scope.ResetRewardItemsForm = function () {

            $scope.MDL_Allowance = '';
            $scope.MDL_RewardItemName_Arb = null;
            $scope.MDL_RewardItemName_Eng = null;
            $scope.MDL_RewardItemTypeId = '';
            $scope.RewardItemsId = 0;
            $scope.MDL_RewardItemExpensesTypeId = '';
            $scope.MDL_AfterCheckingResult = false;
            $scope.MDL_IsAdvanceReturn = false;
            $scope.SaveRewItemFormPopup = true;
            $scope.NewRewItemFormPopup = false;
            $scope.DeleteRewItemFormPopup = false;

            $scope.BtnSubmitRewardItemName = ' حفظ ';
            $scope.RewardItemsPopUpTitle = 'إضافة بند مكافأة';
        };

        $scope.popupOptions_RewardItems = {
            width: 1100,
            height: 610,
            contentTemplate: 'information',
            showTitle: true,
            dragEnabled: false,
            shadingColor: "rgba(0, 0, 0, 0.69)",
            closeOnOutsideClick: false,
            bindingOptions: {
                visible: "RewardItemsPopUpShow",
                title: "RewardItemsPopUpTitle"
            },
            rtlEnabled: true,
            onHiding: function () {
                //DataSourceAllowances.reload();
                $scope.ResetRewardItemsForm();
            }
        };

        $scope.submitButtonOptions_RewItemForm = {
            text: $scope.BtnSubmitRewardItemName,
            type: 'success',
            icon: "check",
            useSubmitBehavior: true,
            validationGroup: "RewardItemForm_VR",
            onClick: function (e) {
                debugger;
                var PostRewardItemFormValidationGroup = DevExpress.validationEngine.getGroupConfig("RewardItemForm_VR");
                if (PostRewardItemFormValidationGroup) {
                    var result = PostRewardItemFormValidationGroup.validate();
                    if (result.isValid) {
                        debugger;
                        $http({
                            method: "Post",
                            url: "/Rewards/SaveRewardItem",
                            data: {
                                RewardItemsId: $scope.MDL_Allowance,
                                RewardItemName_Arb: $scope.MDL_RewardItemName_Arb,
                                RewardItemName_Eng: $scope.MDL_RewardItemName_Eng,
                                RewardItemTypeId: $scope.MDL_RewardItemTypeId,
                                RewardItemExpensesTypeId: $scope.MDL_RewardItemExpensesTypeId,
                                AfterCheckingResult: $scope.MDL_AfterCheckingResult,
                                IsAdvanceReturn:  $scope.MDL_IsAdvanceReturn
                            }
                        }).then(function (data) {
                            if (data.data !== "") {
                                DevExpress.ui.notify({ message: data.data }, "Error", 10000);
                            } else if (data.data === "") {
                                var _message = $scope.MDL_Allowance === null || $scope.MDL_Allowance === '' || $scope.MDL_Allowance === undefined ? "تم الحفظ بنجاح" : "تم التعديل بنجاح";
                                DevExpress.ui.notify({ message: _message }, "success", 10000);

                                $scope.ResetRewardItemsForm();
                                DevExpress.validationEngine.getGroupConfig("RewardItemForm_VR").reset();
                                DataSourceAllowances.reload();
                            }
                        });
                    }
                }

            }

        };

        $scope.btnNew_RewItemForm = {
            useSubmitBehavior: false,
            text: ' جديد ',
            type: 'default',
            onClick: function (e) {
                $scope.ResetRewardItemsForm();
                DevExpress.validationEngine.getGroupConfig("RewardItemForm_VR").reset();
            }
        };

        $scope.TX_RewardItemName_Arb = {
            bindingOptions: {
                value: "MDL_RewardItemName_Arb"
            },
            placeholder: "",
            showClearButton: true,
            rtlEnabled: true
        };
        $scope.VR_RewardItemName_Arb = {
            validationRules: [{ type: "required", message: "حقل إلزامي" }],
            validationGroup: "RewardItemForm_VR"
        };

        $scope.TX_RewardItemName_Eng = {
            bindingOptions: {
                value: "MDL_RewardItemName_Eng"
            },
            placeholder: "",
            showClearButton: true,
            rtlEnabled: true
        };
        $scope.VR_RewardItemName_Eng = {
            validationRules: [{ type: "required", message: "Required Field" }, {
                type: "pattern",
                pattern: /^[a-zA-Z0-9_ ]*$/i,
                message: "only allow English characters and numbers for text input"
            }],
            validationGroup: "RewardItemForm_VR"
        };

        $scope.DL_RewardItemTypes = {
            dataSource: RewardItemTypesDataSource,
            bindingOptions: {
                value: "MDL_RewardItemTypeId",
                text: "MDL_RewardItemTypeText"
            },
            displayExpr: "RewardItemTypeName",
            valueExpr: "ID",
            searchEnabled: true,
            showClearButton: true,
            rtlEnabled: true,
            placeholder: "اختر",
            noDataText: "لا يوجد بيانات"
        };
        $scope.VR_RewardItemTypes = {
            validationRules: [{ type: "required", message: "حقل إلزامي" }],
            validationGroup: "RewardItemForm_VR"
        };

        $scope.DL_RewardItemExpensesTypes = {
            dataSource: RewardItemExpensesTypesDataSource,
            bindingOptions: {
                value: "MDL_RewardItemExpensesTypeId",
                text: "MDL_RewardItemExpensesTypeText"
            },
            displayExpr: "RewardItemExpensesTypeName",
            valueExpr: "ID",
            searchEnabled: true,
            showClearButton: true,
            rtlEnabled: true,
            placeholder: "اختر",
            noDataText: "لا يوجد بيانات"

        };
        $scope.VR_RewardItemExpensesTypes = {
            validationRules: [{ type: "required", message: "حقل إلزامي" }],
            validationGroup: "RewardItemForm_VR"
        };

        $scope.CK_AfterCheckingResult = {
            bindingOptions: {
                value: "MDL_AfterCheckingResult"
            },
            text: "يتم الاحتساب بعد رصد الدرجات"
        };
        $scope.CK_IsAdvanceReturn = {
            bindingOptions: {
                value: "MDL_IsAdvanceReturn"
            },
            text: "يخصم من أقساط السلف"
        };
        $scope.RewardItemsGrid = {
            dataSource: DataSourceAllowances,
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
                placeholder: "بحث"
            },
            paging: {
                pageSize: 5
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
            noDataText: 'لا يوجد بيانات',
            columnAutoWidth: true,
            width: "auto",
            columnChooser: {
                enabled: true
            },
            "export": {
                enabled: true,
                fileName: "بنود المكافئات"
                //allowExportSelectedData: true
            },
            onCellPrepared: function (e) {
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
            },
            columns: [
                {
                    dataField: "AllowanceId",
                    visible: false
                },
                {
                    dataField: "AllowanceName",
                    caption: "إسم البند بالعربي"
                    //groupIndex: 0
                },
                {
                    dataField: "RewardItemName_Eng",
                    caption: "إسم البند بالإنجليزي"
                },
                {
                    dataField: "RewardItemTypeName",
                    caption: "نوع البند"
                },
                {
                    dataField: "RewardItemExpensesTypeName",
                    caption: "نوع الصرف"
                },
                {
                    dataField: "AfterCheckingResult",
                    caption: "بعد رصد الدرجات",
                    dataType: "boolean"
                }
                ,
                {
                    dataField: "IsAdvanceReturn",
                    caption: "خصم من السلف",
                    dataType: "boolean"
                }
            ],
            allowColumnResizing: true,
            columnResizingMode: "widget",
            allowColumnReordering: true,
            showColumnLines: true,
            rowAlternationEnabled: true,
            onInitialized: function (e) {
                $scope.RewardItemsGridInstance = e.component;
            },
            editing: {
                allowUpdating: true,
                allowAdding: false,
                allowDeleting: true,
                mode: "row",
                texts: {
                    confirmDeleteMessage: "تأكيد حذف  !",
                    addRow: "اضافة"
                },
                useIcons: true
            },
            onRowRemoving: function (e) {
                e.cancel = true;
                debugger;
                $http({
                    method: "Delete",
                    url: "/Rewards/DeleteRewardItem",
                    data: { RewardItemId: e.data.AllowanceId }
                }).then(function (data) {
                    if (data.data !== "") {
                        return DevExpress.ui.notify({ message: data.data }, "Error", 10000);
                    } else {
                        DevExpress.ui.notify({ message: "تم الحذف بنجاح" }, "success", 10000);
                        DataSourceAllowances.reload();
                    }
                });
            },
            onEditingStart: function (e) {
                e.cancel = true;

                $scope.SaveRewItemFormPopup = true;
                $scope.NewRewItemFormPopup = true;


                $scope.BtnSubmitRewardItemName = ' تعديل ';
                $scope.RewardItemsPopUpTitle = 'تعديل بنود المكافأة';

                $http({
                    method: "Get",
                    url: "/Rewards/GetRewardItemById",
                    params: { RewardItemId: e.data.AllowanceId }
                }).then(function (data) {
                    debugger;
                    $scope.MDL_Allowance = e.data.AllowanceId;
                    $scope.MDL_RewardItemName_Arb = data.data.RewardItemName_Arb;
                    $scope.MDL_RewardItemName_Eng = data.data.RewardItemName_Eng;
                    $scope.MDL_RewardItemTypeId = data.data.RewardItemType_Id;
                    $scope.MDL_RewardItemExpensesTypeId = data.data.RewardItemExpensesType_Id;
                    $scope.MDL_AfterCheckingResult = data.data.AfterCheckingResult;
                    $scope.MDL_IsAdvanceReturn = data.data.IsAdvanceReturn;
                });
            }
        };
        /*--------------------------------* Reward Items *--------------------------------*/


        /************************************* Reward Details **************************************/

        $(function () {
            $("#widget").dxRadioGroup({
                layout: "horizontal",
                items: $scope.Listradiogroup,
                value: $scope.Listradiogroup[0],
                onValueChanged: function (e) {
                    if (e.value === $scope.Listradiogroup[1]) {
                        $scope.DependOnEducationDays = false;
                        $scope.DependOnEducationPeriod = true;
                    }
                    else {
                        $scope.DependOnEducationDays = true;
                        $scope.DependOnEducationPeriod = false;
                    }
                }
            });
        });

        $scope.DL_RewardCategories = {
            dataSource: DataSourceRewardCategoriesDistinct,
            bindingOptions: {
                value: "MDL_RewardCategory"
                //text: "MDL_RewardCategoryText"
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
                //$scope.RewardCategorySelectedValues = e.model.MDL_RewardCategory.join(',');
            }
        };
        $scope.VR_RewardCategories = {
            validationRules: [{ type: "required", message: "حقل إلزامي" }],
            validationGroup: "RewardForm_VR"
        };

        $scope.DL_FacultyBranches = {
            dataSource: DataSourceFacultyBranches,
            bindingOptions: {
                value: "MDL_FacultyBranch"
                //text: "MDL_FacultyBranchText"
            },
            displayExpr: "FacultyBranchName",
            valueExpr: "FacultyBranchId",
            searchEnabled: true,
            showClearButton: true,
            rtlEnabled: true,
            placeholder: "اختر",
            noDataText: "لا يوجد بيانات",
            onInitialized: function (e) {
                $scope.FacultyBranchesInstance = e.component;
            },
            multiline: false,
            showBorders: true,
            showSelectionControls: true,
            showDropDownButton: true,
            onValueChanged: function (e) {
                $scope.FacultyBranchSelectedValues = e.value.join(',');
            },
            onClosed: function (e) {
                //$scope.FacultyBranchSelectedValues = e.model.MDL_FacultyBranch.join(',');
            }
        };
        $scope.VR_FacultyBranches = {
            validationRules: [{ type: "required", message: "حقل إلزامي" }],
            validationGroup: "RewardForm_VR"
        };

        $scope.DL_EducationLevels = {
            bindingOptions: {
                value: "MDL_EducationLevel"
                //text: "MDL_EducationLevelText"
            },
            dataSource: DataSourceEducationLevels,
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
                $scope.EducationLevelSelectedValues = e.value.join(',');
            },
            onClosed: function (e) {
                //$scope.EducationLevelSelectedValues = e.model.MDL_EducationLevel.join(',');
            }
        };
        $scope.VR_EducationLevels = {
            validationRules: [{ type: "required", message: "حقل إلزامي" }],
            validationGroup: "RewardForm_VR"
        };

        $scope.DL_RegisterTypes = {
            bindingOptions: {
                value: "MDL_RegisterType",
                text: "MDL_RegisterTypeText"
            },
            dataSource: $scope.DataSourceRegisterTypes,
            displayExpr: "RegisterTypeText",
            valueExpr: "RegisterTypeId",
            searchEnabled: true,
            showClearButton: true,
            rtlEnabled: true,
            placeholder: "اختر",
            noDataText: "لا يوجد بيانات",
            onItemClick: function (e) {
            }
        };
        $scope.VR_RegisterTypes = {
            validationRules: [{ type: "required", message: "حقل إلزامي" }],
            validationGroup: "RewardForm_VR"
        };

        $scope.DL_Allowances = {
            bindingOptions: {
                value: "MDL_Allowance",
                text: "MDL_AllowanceText"
            },
            dataSource: DataSourceAllowances,
            displayExpr: "AllowanceName",
            valueExpr: "AllowanceId",
            searchEnabled: true,
            showClearButton: true,
            rtlEnabled: true,
            placeholder: "اختر",
            noDataText: "لا يوجد بيانات",
            onItemClick: function (e) {
            },
            onValueChanged: function (e) {
                debugger;
                if (e.value > 0) {

                    $http({
                        method: "Get",
                        url: "/Rewards/CheckExpensesTypeById",
                        params: { RewardItemId: e.value }
                    }).then(function (data) {
                        if (data.data === "شهري") {
                            $scope.TX_DuesNoFormatValue = "شهر";
                        } else {
                            $scope.TX_DuesNoFormatValue = "سنة";
                        }
                    });
                }
            },
            onInitialized: function () {
            }
        };
        $scope.VR_Allowances = {
            validationRules: [{ type: "required", message: "حقل إلزامي" }],
            validationGroup: "RewardForm_VR"
        };

        $scope.TX_Amount_RewardForm = {
            bindingOptions: {
                value: "MDL_Amount"
            },
            min: 1,
            showClearButton: true,
            showSpinButtons: true,
            rtlEnabled: true
        };
        $scope.VR_Amount = {
            validationRules: [{ type: "required", message: "حقل إلزامي" }],
            validationGroup: "RewardForm_VR"
        };

        $scope.DL_AmountType = {
            bindingOptions: {
                items: "AmountTypesDataSource",
                value: "MDL_AmountType",
                text: "MDL_AmountTypeText"
            },
            displayExpr: "AmountTypeName",
            valueExpr: "AmountTypeId",
            searchEnabled: true,
            showClearButton: true,
            rtlEnabled: true,
            placeholder: "اختر",
            noDataText: "لا يوجد بيانات",
            onItemClick: function (e) {
            }
        };
        $scope.VR_AmountType = {
            validationRules: [{ type: "required", message: "حقل إلزامي" }],
            validationGroup: "RewardForm_VR"
        };

        $scope.DL_EducationTypes = {
            bindingOptions: {
                value: "MDL_EducationTypeId",
                text: "MDL_EducationTypeText"
            },
            dataSource: DataSourceEducationTypes,
            displayExpr: "EducationTypeName",
            valueExpr: "EducationTypeId",
            searchEnabled: true,
            showClearButton: true,
            rtlEnabled: true,
            placeholder: "اختر",
            noDataText: "لا يوجد بيانات",
            onItemClick: function (e) {
            }
        };
        $scope.VR_EducationTypes = {
            validationRules: [{ type: "required", message: "حقل إلزامي" }],
            validationGroup: "RewardForm_VR"
        };

        $scope.DL_RateType = {
            bindingOptions: {
                items: "RateTypesDataSource",
                value: "MDL_RateType",
                text: "MDL_RateTypeText"
            },
            displayExpr: "RateTypeText",
            valueExpr: "RateTypeId",
            searchEnabled: true,
            showClearButton: true,
            rtlEnabled: true,
            placeholder: "اختر",
            noDataText: "لا يوجد بيانات",
            onItemClick: function (e) {
            },
            onValueChanged: function (e) {
                if (e.value === 1) { // فصلي
                    $scope.TX_ClassesNoDisabled = false;
                } else {
                    $scope.TX_ClassesNoDisabled = true;
                    $scope.MDL_ClassesNo = '';
                }
            }
        };
        $scope.VR_RateType = {
            validationRules: [{ type: "required", message: "حقل إلزامي" }],
            validationGroup: "RewardForm_VR"
        };

        $scope.TX_FromRate = {
            bindingOptions: {
                value: "MDL_FromRate"
            },
            format: "#0%",
            min: 0,
            max: 5,
            step: 0.01,
            showClearButton: false,
            showSpinButtons: true,
            rtlEnabled: true

        };
        $scope.VR_FromRate = {
            validationRules: [{ type: "required", message: "حقل إلزامي" }],
            validationGroup: "RewardForm_VR"
        };

        $scope.TX_ToRate = {
            bindingOptions: {
                value: "MDL_ToRate",
                min: "MDL_FromRate"
            },
            format: "#0%",
            step: 0.01,
            max: 5,
            showSpinButtons: true,
            showClearButton: false,
            rtlEnabled: true
        };
        $scope.VR_ToRate = {
            validationRules: [{ type: "required", message: "حقل إلزامي" }],
            validationGroup: "RewardForm_VR"
        };

        $scope.DL_StudentStatusAcademyId = {
            bindingOptions: {
                value: "MDL_StudentStatusAcademy",
                text: "MDL_StudentStatusAcademyText"
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
            //applyValueMode: "useButtons",
            onValueChanged: function (e) {
                $scope.StudentStatusAcademySelectedValues = e.value.join(',');
            },
            onClosed: function (e) {
                //$scope.StudentStatusAcademySelectedValues = e.model.MDL_StudentStatusAcademy.join(',');
            }
        };
        $scope.VR_StudentStatusAcademyId = {
            validationRules: [{ type: "required", message: "حقل إلزامي" }],
            validationGroup: "RewardForm_VR"
        };

        $scope.TX_MiniHoursNo = {
            bindingOptions: {
                value: "MDL_MiniHoursNo"
            },
            placeholder: "",
            showClearButton: true,
            showSpinButtons: true,
            min: 0,
            rtlEnabled: true
        };

        $scope.TX_ClassesNo = {
            bindingOptions: {
                value: "MDL_ClassesNo",
                disabled: "TX_ClassesNoDisabled"
            },
            placeholder: "",
            showSpinButtons: true,
            showClearButton: true,
            rtlEnabled: true,
            min: 0
        };
        $scope.VR_ClassesNo = {
            validationRules: [{ type: "required", message: "حقل إلزامي" }],
            validationGroup: "RewardForm_VR"
        };

        $scope.TX_DuesNo = {
            bindingOptions: {
                value: "MDL_DuesNo"
            },
            placeholder: "",
            showClearButton: true,
            showSpinButtons: true,
            min: 0,
            rtlEnabled: true
        };

        $scope.TX_DuesNoFormat = {
            bindingOptions: {
                value: "TX_DuesNoFormatValue"
            },
            readOnly: true,
            rtlEnabled: true
        };

        $scope.ResetRewardDetailsForm = function () {
            debugger;
            $scope.btnRewardFormName = 'حفظ';
            $scope.MDL_RateType = '';
            $scope.MDL_RewardCategory = '';
            $scope.RewardCategorySelectedValues = '';
            $scope.MDL_FacultyBranch = '';
            $scope.FacultyBranchSelectedValues = '';
            $scope.MDL_EducationLevel = '';
            $scope.EducationLevelSelectedValues = '';
            $scope.MDL_Amount = '';
            $scope.MDL_AmountType = '';
            $scope.MDL_EducationTypeId = '';
            $scope.MDL_FromRate = '';
            $scope.MDL_ToRate = '';
            $scope.MDL_StudentStatusAcademy = '';
            $scope.StudentStatusAcademySelectedValues = '';
            $scope.MDL_MiniHoursNo = '';
            $scope.MDL_ClassesNo = '';
            $scope.MDL_DuesNo = '';
            $scope.MDL_Allowance = '';
            $scope.MDL_AmountTypeText = '';
            $scope.MDL_RateTypeText = '';
            $scope.MDL_RewardCategoryText = '';
            $scope.MDL_FacultyBranchText = '';
            $scope.MDL_EducationLevelText = '';
            $scope.MDL_EducationTypeText = '';
            $scope.MDL_StudentStatusAcademyText = '';

            $scope.MDL_RegisterType = '';
            $scope.MDL_RegisterTypeText = '';

            $scope.MDL_DependOnEducationDays = true;
            $scope.MDL_DependOnEducationperiod = false;

            $scope.EdittedRewardDetailsId = 0;
            $scope.RewardDetailsId = 0;
        };

        $scope.submitButtonOptions_RewardForm = {
            bindingOptions: {
                text: "btnRewardFormName"
            },
            type: 'success',
            icon: "check",
            useSubmitBehavior: true,
            validationGroup: "RewardForm_VR",
            onClick: function (e) {
                var PostRewardDetailsFormValidationGroup =
                    DevExpress.validationEngine.getGroupConfig("RewardForm_VR");
                if (PostRewardDetailsFormValidationGroup) {
                    var result = PostRewardDetailsFormValidationGroup.validate();
                    if (result.isValid) {
                        debugger;
                        if ($scope.RewardCategorySelectedValues === null || $scope.RewardCategorySelectedValues === undefined || $scope.RewardCategorySelectedValues === '') {
                            return DevExpress.ui.notify({ message: "برجاء إختيار فئة المكافأة" }, "Error", 10000);
                        }
                        if ($scope.FacultyBranchSelectedValues === null || $scope.FacultyBranchSelectedValues === undefined || $scope.FacultyBranchSelectedValues === '') {
                            return DevExpress.ui.notify({ message: "برجاء إختيار الكلية" }, "Error", 10000);
                        }
                        if ($scope.EducationLevelSelectedValues === null || $scope.EducationLevelSelectedValues === undefined || $scope.EducationLevelSelectedValues === '') {
                            return DevExpress.ui.notify({ message: "برجاء إختيار الدرجة العلمية" }, "Error", 10000);
                        }
                        if ($scope.StudentStatusAcademySelectedValues === null || $scope.StudentStatusAcademySelectedValues === undefined || $scope.StudentStatusAcademySelectedValues === '') {
                            return DevExpress.ui.notify({ message: "برجاء إختيار الحالة الأكاديمية" }, "Error", 10000);
                        }
                        $http({
                            method: "Post",
                            url: "/Rewards/SaveRewardDetails",
                            data: {
                                RewardDetailsId: $scope.RewardDetailsId,
                                RewardCategory: $scope.RewardCategorySelectedValues,
                                FacultyBranch: $scope.FacultyBranchSelectedValues,
                                EducationLevel: $scope.EducationLevelSelectedValues,
                                Allowance: $scope.MDL_Allowance,
                                Amount: $scope.MDL_Amount,
                                AmountType: $scope.MDL_AmountType,
                                EducationType: $scope.MDL_EducationTypeId,
                                FromRate: $scope.MDL_FromRate,
                                ToRate: $scope.MDL_ToRate,
                                StudentStatusAcademyId: $scope.StudentStatusAcademySelectedValues,
                                MiniHoursNo: $scope.MDL_MiniHoursNo,
                                ClassesNo: $scope.MDL_ClassesNo,
                                DuesNo: $scope.MDL_DuesNo,
                                DependOnEducationDays: $scope.DependOnEducationDays,
                                DependOnEducationPeriod: $scope.DependOnEducationPeriod,
                                RateType: $scope.MDL_RateType,
                                RegisterTypeId: $scope.MDL_RegisterType
                            }
                        }).then(function (data) {
                            if (data.data !== "") {
                                DevExpress.ui.notify({ message: data.data }, "Error", 10000);
                            } else {
                                var _message = $scope.RewardDetailsId !== null && $scope.RewardDetailsId !== undefined && $scope.RewardDetailsId > 0 ? "تم التعديل بنجاح" : "تم الحفظ بنجاح";
                                DevExpress.ui.notify({ message: _message }, "success", 10000);
                                //$scope.ResetRewardDetailsForm();
                                DataSourceGrid.reload();
                                //PostRewardDetailsFormValidationGroup.reset();
                            }
                        });
                    }
                }
            }

        };

        $scope.btnCancelRewardDetailsEdit = {
            useSubmitBehavior: false,
            bindingOptions: {
                visible: "ShowBtnCancelRewardDetails"
            },
            text: ' إلغاء ',
            type: 'default',
            onClick: function (e) {
                $scope.ResetRewardDetailsForm();
                DevExpress.validationEngine.getGroupConfig("RewardForm_VR").reset();
                $scope.ShowBtnCancelRewardDetails = false;
            }
        };
        /*--------------------------------* Reward Details *--------------------------------*/




        /********************************* Reward Grid ***********************************/
        $scope.RewardGrid = {

            dataSource: DataSourceGrid,
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
            noDataText: '',
            columnAutoWidth: true,
            width: "auto",
            columnChooser: {
                enabled: true
            },
            "export": {
                enabled: true,
                fileName: "تفاصيل المكافئات"
                //allowExportSelectedData: true
            },
            onCellPrepared: function (e) {
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
            },
            columns: [
                {
                    dataField: "RewardDetailsId",
                    visible: false
                },
                {
                    dataField: "RewardCategoryArbName",
                    caption: "فئة المكافأة",
                    groupIndex: 0
                },
                {
                    dataField: "FacultyName",
                    caption: "فرع الكلية"
                },
                {
                    dataField: "EducationLevelName",
                    caption: "الدرجة العلمية"
                },
                {
                    dataField: "RewardItemName_Arb",
                    caption: "اسم البند"
                },
                {
                    dataField: "Amount",
                    caption: "قيمة البند"
                },
                {
                    dataField: "AmountType",
                    caption: "نوع القيمة",
                    calculateCellValue: function (data) {
                        return (data.AmountType === 1) ? "مقطوع" : "";
                    }
                },
                {
                    dataField: "EducationTypeName",
                    caption: "نوع الدراسة"
                },
                {
                    dataField: "RateType",
                    caption: "نوع المعدل",
                    calculateCellValue: function (data) {
                        return (data.RateType > 0 && data.RateType !== null) ? (data.RateType === 1) ? "فصلي" : "تراكمي" : "";

                    }
                },
                {
                    dataField: "FromRate",
                    caption: "من معدل"
                },
                {
                    dataField: "ToRate",
                    caption: "إلي معدل"
                },
                {
                    dataField: "HoursNo",
                    caption: "نوع التسجيل",
                    calculateCellValue: function (data) {
                        return data.HoursNo === 1 ? "عدد ساعات" : "مقررات";
                    }
                },
                {
                    dataField: "RegisterTypeName",
                    caption: "الحالة الأكاديمية"
                },
                {
                    dataField: "MiniHoursNo",
                    caption: "أدني ساعات"
                },
                {
                    dataField: "ClassesNo",
                    caption: "عدد الفصول"
                },
                {
                    dataField: "DuesNo",
                    caption: "مرات الاستحقاق"
                },
                {
                    dataField: "DependOnEducationDays",
                    caption: "وفقا لأيام الدراسة",
                    dataType: "boolean"
                },
                {
                    dataField: "DependOnEducationPeriod",
                    caption: "وفقا لمدة الدراسة",
                    dataType: "boolean"
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
                allowUpdating: true,
                allowAdding: false,
                allowDeleting: true,
                mode: "row",
                texts: {
                    confirmDeleteMessage: "تأكيد حذف  !",
                    addRow: "اضافة"
                },
                useIcons: true
            },
            onRowRemoving: function (e) {
                e.cancel = true;
                debugger;
                $http({
                    method: "Delete",
                    url: "/Rewards/DeleteRewardDetails",
                    data: { RewardDetailsId: e.data.RewardDetailsId }
                }).then(function (data) {
                    if (data.data !== "") {
                        return DevExpress.ui.notify({ message: data.data }, "Error", 10000);
                    } else {
                        DevExpress.ui.notify({ message: "تم الحذف بنجاح" }, "success", 10000);
                        DataSourceGrid.reload();
                    }
                });
            },
            onRowRemoved: function (e) {
                DataSourceGrid.reload();
            },
            onEditingStart: function (e) {
                e.cancel = true;

                $http({
                    method: "Get",
                    url: "/Rewards/GetRewardDetailsById",
                    params: { RewardDetailsId: e.data.RewardDetailsId }
                }).then(function (data) {
                    debugger;
                    $scope.ShowBtnCancelRewardDetails = true;
                    $scope.btnRewardFormName = 'تعديل';
                    $scope.MDL_EducationTypeId = data.data.EducationTypeId;
                    $scope.RewardDetailsId = e.data.RewardDetailsId;

                    $scope.MDL_FacultyBranch = Array.from([data.data.FacultyId]);
                    $scope.MDL_EducationLevel = Array.from([data.data.EducationLevelId]);
                    $scope.MDL_RewardCategory = Array.from([data.data.RewardCategoryId]);
                    $scope.MDL_StudentStatusAcademy = Array.from([data.data.StudentStatusAcademyId]);

                    $scope.MDL_Allowance = data.data.ItemId;
                    $scope.MDL_Amount = data.data.Amount;
                    $scope.MDL_AmountType = data.data.AmountTypeId;

                    $scope.MDL_FromRate = data.data.FromRate;
                    $scope.MDL_ToRate = data.data.ToRate;

                    $scope.MDL_MiniHoursNo = data.data.MiniHoursNo;
                    $scope.MDL_ClassesNo = data.data.ClassesNo;
                    $scope.MDL_DuesNo = data.data.DuesNo;
                    $scope.MDL_DependOnEducationDays = data.data.DependOnEducationDays;
                    $scope.MDL_DependOnEducationperiod = data.data.DependOnEducationPeriod;
                    $scope.MDL_RateType = data.data.RateTypeId;
                    $scope.MDL_RegisterType = data.data.RegisterTypeId;


                });
            }
        };
        /*--------------------------------* Reward Grid *--------------------------------*/

    }]);
})();

















        //$scope.RewardItemTypesDataSource = [{ RewardItemTypeId: 1, RewardItemTypeText: 'مكافأة' },
        //{ RewardItemTypeId: 2, RewardItemTypeText: 'مكافأة تفوق' },
        //{ RewardItemTypeId: 3, RewardItemTypeText: 'بدل' },
        //{ RewardItemTypeId: -1, RewardItemTypeText: 'خصم' }];

        //$scope.RewardItemExpensesTypesDataSource = [{ RewardItemExpensesTypeId: 1, RewardItemExpensesTypeName: 'شهري' },
        ////{ RewardItemExpensesTypeId: 2, RewardItemExpensesTypeName: 'مرة واحدة' },
        //{ RewardItemExpensesTypeId: 2, RewardItemExpensesTypeName: 'سنوي' },
        //{ RewardItemExpensesTypeId: 3, RewardItemExpensesTypeName: 'عند الطلب' }];




              //if ($scope.MDL_RewardCategory === null || $scope.MDL_RewardCategory === '' || $scope.MDL_RewardCategory === undefined) {

                //    $scope.ResetReCatForm();
                //    $scope.RewardCatPopUpTitle = 'إضافة فئة مكافأة';

                //} else {
                //    $scope.ButtonName = ' تعديل ';
                //    $scope.SaveRewcatFormPopup = true;
                //    $scope.NewRewcatFormPopup = true;
                //    $scope.DeleteRewcatFormPopup = true;
                //    $scope.RewardCatPopUpTitle = 'تعديل فئة مكافأة';

                //    $http({
                //        method: "Get",
                //        url: "/Rewards/GetRewardcategoryById",
                //        params: { RewardcategoryId: $scope.MDL_RewardCategory }
                //    }).then(function (data) {
                //        debugger;
                //        $scope.MDL_CategoryName_Arb = data.data.CategoryName_Arb;
                //        $scope.MDL_CategoryName_Eng = data.data.CategoryName_Eng;
                //        $scope.MDL_NationalityId = data.data.RewardNationaltities;
                //    });
                //}


 //if ($scope.MDL_Allowance === null || $scope.MDL_Allowance === '' || $scope.MDL_Allowance === undefined) {

                //} else {
                //    $scope.SaveRewItemFormPopup = true;
                //    $scope.NewRewItemFormPopup = true;
                //    $scope.DeleteRewItemFormPopup = true;

                //    $scope.BtnSubmitRewardItemName = ' تعديل ';
                //    $scope.RewardItemsPopUpTitle = 'تعديل بنود المكافأة';
                //    $http({
                //        method: "Get",
                //        url: "/Rewards/GetRewardItemById",
                //        params: { RewardItemId: $scope.MDL_Allowance }
                //    }).then(function (data) {
                //        debugger;

                //        $scope.MDL_RewardItemName_Arb = data.data.RewardItemName_Arb;
                //        $scope.MDL_RewardItemName_Eng = data.data.RewardItemName_Eng;
                //        $scope.MDL_RewardItemTypeId = data.data.RewardItemType_Id;
                //        $scope.MDL_RewardItemExpensesTypeId = data.data.RewardItemExpensesType_Id;
                //        $scope.MDL_AfterCheckingResult = data.data.AfterCheckingResult;
                //    });

                //}


                    //$(function () {
                    //    DataSourceEducationTypes.reload();
                    //    DataSourceStudentStatusAcademy.reload();
                    //    DataSourceRewardCategoriesDistinct.reload();
                    //    DataSourceFacultyBranches.reload();
                    //    DataSourceEducationLevels.reload();
                    //    DataSourceAllowances.reload();
                    //});


                //new DevExpress.data.ArrayStore({
                //    key: "NationalityId",
                //    data: 

                //}),


                //DataSourceAllowances.filter("RewardItemExpensesTypeName", "<>", "");
                //DataSourceAllowances.reload();