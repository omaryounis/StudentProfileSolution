app.controller("ViolationCtrl", ["$scope", 'ViolationSrvc', '$http',
    function ($scope, ViolationSrvc, $http) {
        //fields
        $scope.UsersArray = [];
        $scope.UserId = null;

        $scope.categoryIds = [];
        $scope.CategoriesArray = [];
        $scope.selectedItemKeys = [];

        $scope.selectionMode = "all";
        $scope.selectAllMode = "page";

        $scope.allUsersCategoriesPermissions = [];



        //controls
        $scope.categoriesOptions = {
            bindingOptions: {
                dataSource: "CategoriesArray",
                selectedItemKeys: "selectedItemKeys"
            },
            selectionMode: "selectionMode",
            selectAllMode: "selectAllMode",
            showSelectionControls: true,
            keyExpr: "id",
            onInitialized: function () {
                ViolationSrvc.GetCategories().then(function (dataReturned) {
                    $scope.CategoriesArray = dataReturned.data;
                });
            },
            onSelectionChanged: function (e) {
                //check if there were items added
                if (e.addedItems.length > 0) {
                    for (var i = 0; i < e.addedItems.length; i++) {
                        $scope.categoryIds.push(Number(e.addedItems[i].id));
                    }
                }

                //check if there were items removed
                if (e.removedItems.length > 0) {
                    var categoryIdsCopy = $scope.categoryIds; //copy of categoryIds to remove items from
                    for (k = 0; k < $scope.categoryIds.length; k++) {
                        for (var l = 0; l < e.removedItems.length; l++) {
                            if ($scope.categoryIds[k] === Number(e.removedItems[l].id)) {
                                categoryIdsCopy.splice(k, 1);
                            }
                        }
                    }
                    $scope.categoryIds = categoryIdsCopy; //new chosen categoryIds
                }
            }
        };

        $scope.UserSelectBox = {
            bindingOptions: {
                items: 'UsersArray',
                value: 'UserId',
                dataSource: 'UsersArray'
            },
            placeholder: "--أختر--",
            displayExpr: "Text",
            valueExpr: "Value",
            onInitialized: function (e) {
                ViolationSrvc.GetUsers().then(function (dataReturned) {
                    $scope.UsersArray = dataReturned.data;
                });
            },
            onValueChanged: function (e) {
                ViolationSrvc.getcategoriesPermissionsForUser(Number(e.value)).
                    then(function (dataReturned) {
                        $scope.categoriesPermissionsForUser = dataReturned.data;
                        $scope.selectedItemKeys = [];

                        for (var i = 0; i < dataReturned.data.length; i++) {
                            $scope.selectedItemKeys.
                                push(JSON.stringify(Number(dataReturned.data[i].CategoryId)));
                        }
                    });
            }
        };


        $scope.saveButton = {
            text: "حفظ",
            type: 'success',
            onClick: function () {

                if ($scope.UserId === null || $scope.UserId === '') {
                    DevExpress.ui.notify({
                        message: 'عفوا ادخل اسم المستخدم',
                        type: "error",
                        displayTime: 3000,
                        closeOnClick: true
                    });
                    return;
                }

                ViolationSrvc.AddViolations($scope.UserId, $scope.categoryIds).then(function (returnedData) {
                    if (returnedData.data === "0") {
                        DevExpress.ui.notify({
                            message: 'الرجاء اختيار الصلاحيات',
                            type: "error",
                            displayTime: 3000,
                            closeOnClick: true
                        });
                        return;
                    } else if (returnedData.data === '1') {
                        DevExpress.ui.notify({
                            message: 'تمت ازالة الصلاحيات',
                            type: "success",
                            displayTime: 3000,
                            closeOnClick: true
                        });
                        ViolationSrvc.getAllUsersCategoriesPermissions().then(function (dataReturned) {
                            $scope.allUsersCategoriesPermissions = dataReturned.data;
                        });
                        return;
                    } else if (returnedData.data === '2') {
                        DevExpress.ui.notify({
                            message: 'تمت اضافة الصلاحيات',
                            type: "success",
                            displayTime: 3000,
                            closeOnClick: true
                        });
                        ViolationSrvc.getAllUsersCategoriesPermissions().then(function (dataReturned) {
                            $scope.allUsersCategoriesPermissions = dataReturned.data;
                        });
                        return;
                    } else if (returnedData.data === '3') {
                        DevExpress.ui.notify({
                            message: 'تم تعديل الصلاحيات',
                            type: "success",
                            displayTime: 3000,
                            closeOnClick: true
                        });
                        ViolationSrvc.getAllUsersCategoriesPermissions().then(function (dataReturned) {
                            $scope.allUsersCategoriesPermissions = dataReturned.data;
                        });
                        return;
                    }
                    else {
                        DevExpress.ui.notify({
                            message: returnedData.data,
                            type: "error",
                            displayTime: 3000,
                            closeOnClick: true
                        });
                        ViolationSrvc.getAllUsersCategoriesPermissions().then(function (dataReturned) {
                            $scope.allUsersCategoriesPermissions = dataReturned.data;
                        });
                        return;
                    }
                });
            }
        };





        $scope.PermissionsForUserDataGrid = {
            width: 1450,
            bindingOptions: {
                dataSource: 'allUsersCategoriesPermissions'
            },
            showBorders: true,
            selection: {
                mode: "single"
            },
            columns: [{
                dataField: "Username",
                width: 130,
                caption: "اسم المستخدم",
                groupIndex: 0
            },
            {
                dataField: "CategoryName",
                width: 130,
                caption: "صلاحيات اقسام المخالفات المتاحه للمستخدمين"
            }],
            editing: {
                allowAdding: false,
                allowDeleting: true,
                allowUpdating: false,
                form: null,
                mode: "row",
                popup: null,
                refreshMode: "full",
                selectTextOnEditStart: false,
                startEditAction: "click",
                texts: {
                    cancelAllChanges: "Discard changes",
                    cancelRowChanges: "Cancel",
                    confirmDeleteMessage: "هل تريد الاستمرار وحذف الصلاحيه؟",
                    confirmDeleteTitle: "",
                    deleteRow: "حذف",
                    saveAllChanges: "Save changes",
                    saveRowChanges: "Save",
                    undeleteRow: "Undelete",
                    validationCancelChanges: "Cancel changes"
                },
                useIcons: false
            },
            onRowRemoving: function (e) {
                ViolationSrvc.DeleteCategoriesPermissionForUser(e.data.UserId, e.data.CategoryId)
                    .then(function (dataReturned) {
                        if (dataReturned.data.length > 0) {
                            DevExpress.ui.notify({
                                message: dataReturned.data,
                                type: "success",
                                displayTime: 3000,
                                closeOnClick: true
                            });
                            for (var i = 0; i < $scope.selectedItemKeys.length; i++) {
                                if ($scope.selectedItemKeys[i] === JSON.stringify(e.data.CategoryId)) {
                                    $scope.selectedItemKeys.splice(i, 1);
                                }
                            }
                        } else {
                            DevExpress.ui.notify({
                                message: "عفوا حدث خطأ",
                                type: "error",
                                displayTime: 3000,
                                closeOnClick: true
                            });
                        }
                    });
            },
            onInitialized: function (e) {
                ViolationSrvc.getAllUsersCategoriesPermissions().then(function (dataReturned) {
                    $scope.allUsersCategoriesPermissions = dataReturned.data;
                });
            }
        };




    }]);


