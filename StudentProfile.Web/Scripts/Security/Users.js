(function () {
    app.controller("SecurityCtrl",
        [
            "$scope", "$http", function ($scope, $http) {
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
                        url: "/Security/GetPermissions",
                        params: { screenId: 13 } //شاشة المستخدمين
                    }).then(function (data) {
                        $scope.Permissions.Create = data.data.Create;
                        $scope.Permissions.Read = data.data.Read;
                        $scope.Permissions.Update = data.data.Update;
                        $scope.Permissions.Delete = data.data.Delete;
                        $scope.Permissions.View = data.data.View;
                    });
                };
                $scope.GetPermssions();
                $scope.UserId = null;
                $scope.UserName = "";
                $scope.Password = "";
                $scope.ConfirmPassword = "";
                $scope.IsAdmin = true;
                $scope.SaveButtonShow = true;
                $scope.UpdateButtonShow = false;
                $scope.UserNameIsValid = true;
                $scope.PasswordIsValid = true;
                $scope.ConfirmPasswordIsValid = true;
                $scope.User = {
                    ValidationRules: {
                        Name: {
                            validationGroup: "userValidationGroup",
                            validationRules: [
                                {
                                    type: "required",
                                    message: "حقل اجبارى"
                                }
                            ]
                        },
                        UserName: {
                            validationGroup: "userValidationGroup",
                            validationRules: [
                                {
                                    type: "required",
                                    message: "حقل اجبارى"
                                },
                                {
                                    type: "pattern",
                                    pattern: "^([a-zA-Z_0-9]+ ?)*$",
                                    message: "حروف وارقام فقط"
                                }
                            ]
                        },
                        Mobile: {
                            validationGroup: "userValidationGroup",
                            validationRules: [
                                {
                                    type: "required",
                                    message: "حقل اجبارى"
                                }
                            ]
                        },
                        IsAdmin: {
                            validationGroup: "userValidationGroup",
                            validationRules: [
                                {
                                    type: "required",
                                    message: "حقل اجبارى"
                                }
                            ]
                        },
                        Password: [
                            {
                                type: "required",
                                message: "من فضلك ادخل كلمة المرور"
                            }
                        ],
                        ConfirmPassword: {
                            validationGroup: "userValidationGroup",
                            validationRules: [
                                {
                                    type: "compare",
                                    comparisonTarget: function () {
                                        var password = $("#password-validation").dxTextBox("instance");
                                        if (password) {
                                            return password.option("value");
                                        }
                                        return password.option("value");
                                    },
                                    message: "كلمة المرور والتأكيد لايتطابقان"
                                },
                                {
                                    type: "required",
                                    message: "من فضلك ادخل تاكيد كلمة المرور"
                                }
                            ]
                        },
                        Group: {
                            validationGroup: "userValidationGroup",
                            validationRules: [
                                {
                                    type: "required",
                                    message: "حقل اجبارى"
                                }
                            ]
                        }
                    },
                    Name: {
                        bindingOptions: { value: "Name" },
                        placeholder: "الاسم",
                        showClearButton: true
                    },
                    UserName: {
                        bindingOptions: { value: "UserName" },
                        placeholder: "اسم المستخدم",
                        showClearButton: true
                    },
                    Mobile: {
                        bindingOptions: { value: "Mobile" },
                        placeholder: "رقم الجوال",
                        min: 000000000,
                        mode: "tel",
                        //mask: "+(000)000-0000'",
                        //maskRules: {"X": /^[^0-9]+$/},
                        maskInvalidMessage: "ادخل رقم جوال صحيح",
                        showClearButton: true
                    },
                    IsAdmin:
                    {
                        bindingOptions: { value: "IsAdmin" },
                        text: "حساب مدير"
                    },
                    Password:
                    {
                        bindingOptions: { value: "Password", isValid: "PasswordIsValid" },
                        mode: "password",
                        placeholder: "كلمة المرور",
                        showClearButton: true,
                        onValueChanged: function (e) {
                            debugger;
                        }
                    },
                    ConfirmPassword: {
                        bindingOptions: { value: "ConfirmPassword", isValid: "ConfirmPasswordIsValid" },
                        mode: "password",
                        placeholder: "تأكيد كلمة المرور",
                        showClearButton: true
                    },
                    Group: {
                        dataSource: new DevExpress.data.DataSource({
                            loadMode: "raw",
                            key: "ID",
                            load: function () {
                                return $.getJSON("/Security/GetGroups", function (data) { });
                            }
                        }),
                        bindingOptions: { value: "GroupId" },
                        displayExpr: "Name",
                        valueExpr: "ID",
                        placeholder: "المجموعة",
                        noDataText: "لا يوجد بيانات",
                        searchEnabled: true
                    },
                    SaveButtonOptions: {
                        text: "حفظ",
                        hint: "حفظ",
                        type: "success",
                        icon: "save",
                        useSubmitBehavior: true
                    },
                    UpdateButtonOptions: {
                        text: "تعديل",
                        hint: "تعديل",
                        type: "default",
                        icon: "edit",
                        useSubmitBehavior: true
                    }
                };
                //$http({
                //    method: "GET",
                //    url: "/Security/GetUsers"
                //}).then(function (data) {
                //    debugger;
                //    $scope.Users = data.data;
                //});

                $scope.Reset = function () {
                    $scope.UserNameIsValid = true;
                    $scope.PasswordIsValid = true;
                    $scope.ConfirmPasswordIsValid = true;
                    $scope.UserId = null;
                    $scope.UserName = "";
                    $scope.Mobile = "";
                    $scope.Password = "";
                    $scope.ConfirmPassword = "";
                    $scope.Group = "";
                    $scope.GroupId = null;
                    var popupValidationGroup = DevExpress.validationEngine.getGroupConfig("userValidationGroup");
                    if (popupValidationGroup) {
                        popupValidationGroup.reset();
                    }
                };
                $scope.dataGridOptions = {
                    dataSource: new DevExpress.data.DataSource({
                        loadMode: "raw",
                        key: "ID",
                        load: function () {
                            return $.getJSON("/Security/GetUsers", function (data) { debugger; });
                        }
                    }),
                    bindingOptions: {
                        "editing.editingEnabled": "Permissions.Update",
                        "editing.allowUpdating": "Permissions.Update",
                        "editing.allowDeleting": "Permissions.Delete"
                    },
                    columns: [
                        {
                            dataField: "Name",
                            caption: "الاسم"
                        },
                        {
                            dataField: "Username",
                            caption: "اسم المستخدم"
                        },
                        {
                            dataField: "Mobile",
                            caption: "رقم الجوال"
                        },
                        {
                            dataField: "IsAdmin",
                            caption: "حساب مدير"
                        },
                        {
                            dataField: "GroupName",
                            caption: "مجموعة المستخدم"
                        }
                    ],
                    editing: {
                        texts: {
                            addRow: "اضافة مستخدم",
                            cancelAllChanges: "تجاهل التعديلات",
                            cancelRowChanges: "الغاء",
                            confirmDeleteMessage: "هل تريد حذف هذا المستخدم؟",
                            confirmDeleteTitle: "حذف مستخدم",
                            deleteRow: "حذف",
                            editRow: "تعديل",
                            saveAllChanges: "حفظ التعديلات",
                            saveRowChanges: "حفظ",
                            undeleteRow: "الغاء الحذف",
                            validationCancelChanges: "الغاء التعديلات"
                        }
                    },
                    searchPanel: {
                        visible: true,
                        placeholder: "بحث"
                    },
                    selection: {
                        mode: "single"
                    },
                    showRowLines: true,
                    columnAutoWidth: true,
                    scrolling: {
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
                        infoText: "صفحة  {0} من {1} - {2} عنصر",
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
                    onEditingStart: function (e) {
                        e.cancel = true;
                        $scope.UserId = e.data.ID;
                        $scope.Name = e.data.Name;
                        $scope.UserName = e.data.Username;
                        $scope.Mobile = e.data.Mobile;
                        //$scope.Password = e.data.Password;
                        //$scope.ConfirmPassword = e.data.Password;
                        $scope.GroupId = e.data.Group_ID;
                        $scope.IsAdmin = e.data.IsAdmin;
                        $scope.SaveButtonShow = false;
                        $scope.UpdateButtonShow = true;
                    },
                    onRowRemoving: function (e) {
                        $scope.UserId = e.data.ID;
                        $http({
                            method: "Post",
                            url: "/Security/DeleteUser",
                            data: { id: $scope.UserId }
                        }).then(function (data) {
                            if (data.data === "") {
                                DevExpress.ui.notify({ message: "تم الحذف بنجاح" }, "success", 3000);
                                $scope.DataGridInstance.refresh();
                                $scope.Reset();
                            } else if (data.data !== "") {
                                DevExpress.ui.notify({ message: data.data }, "Error", 3000);
                            }
                        });
                        e.cancel = true;
                    },
                    onInitialized: function (e) {
                        $scope.DataGridInstance = e.component;
                    }
                };
                $scope.onFormSubmit = function (e) {
                    var validationGroup = DevExpress.validationEngine.getGroupConfig("userValidationGroup");
                    if (validationGroup) {
                        var result = validationGroup.validate();
                        if (result.isValid) {
                            DevExpress.ui.notify({ message: "جارى الحفظ" }, "success", 3000);
                            e.preventDefault();
                            if ($scope.UserId === null) {
                                $http({
                                    method: "Post",
                                    url: "/Security/SaveUser/",
                                    data: {
                                        name: $scope.Name,
                                        accName: $scope.UserName,
                                        mobile: $scope.Mobile,
                                        isAdmin: $scope.IsAdmin,
                                        Password: $scope.Password,
                                        groupId: $scope.GroupId
                                    },
                                    headers: {
                                        'RequestVerificationToken': $scope.antiForgeryToken
                                    }

                                }).then(function (data) {
                                    if (data.data === "") {
                                        DevExpress.ui.notify({ message: "تم الحفظ بنجاح" }, "success", 3000);
                                        $scope.DataGridInstance.refresh();
                                        $scope.Reset();
                                    } else if (data.data !== "") {
                                        DevExpress.ui.notify({ message: data.data }, "error", 3000);
                                    }
                                });
                            } else {
                                $http({
                                    method: "Post",
                                    url: "/Security/EditUser/",
                                    data: {
                                        id: $scope.UserId,
                                        name: $scope.Name,
                                        accName: $scope.UserName,
                                        mobile: $scope.Mobile,
                                        isAdmin: $scope.IsAdmin,
                                        Password: $scope.Password,
                                        groupId: $scope.GroupId
                                    },
                                    headers: {
                                        'RequestVerificationToken': $scope.antiForgeryToken
                                    }
                                }).then(function (data) {
                                    if (data.data === "") {
                                        DevExpress.ui.notify({ message: "تم التعديل بنجاح" }, "success", 3000);
                                        $scope.SaveButtonShow = true;
                                        $scope.UpdateButtonShow = false;
                                        $scope.DataGridInstance.refresh();
                                        $scope.Reset();
                                    } else if (data.data !== "") {
                                        DevExpress.ui.notify({ message: data.data }, "error", 3000);
                                    }
                                });
                            }
                        }
                    }
                };
            }
        ]);
})();
function DecryptData(encryptedData) {
    debugger;
    var decryptedText = null;
    try {
        //Creating the Vector Key
        //var iv = CryptoJS.enc.Hex.parse('a5s8d2e9c1721ae0e84ad660c472y1f3');
        //Encoding the Password in from UTF8 to byte array
        var Pass = CryptoJS.enc.Utf8.parse('Crypto');
        //Encoding the Salt in from UTF8 to byte array
        var Salt = CryptoJS.enc.Utf8.parse("cryptography123example");
        //Creating the key in PBKDF2 format to be used during the decryption
        var key128Bits1000Iterations = CryptoJS.PBKDF2(Pass.toString(CryptoJS.enc.Utf8), Salt, { keySize: 128 / 32, iterations: 1000 });
        //Enclosing the test to be decrypted in a CipherParams object as supported by the CryptoJS libarary
        var cipherParams = CryptoJS.lib.CipherParams.create({
            ciphertext: CryptoJS.enc.Base64.parse(encryptedData)
        });

        //Decrypting the string contained in cipherParams using the PBKDF2 key
        var decrypted = CryptoJS.AES.decrypt(cipherParams, key128Bits1000Iterations, { mode: CryptoJS.mode.CBC, padding: CryptoJS.pad.Pkcs7 });
        decryptedText = decrypted.toString(CryptoJS.enc.Utf8);
        return decryptedText;
    }
    //Malformed UTF Data due to incorrect password
    catch (err) {
        return "";
    }
}