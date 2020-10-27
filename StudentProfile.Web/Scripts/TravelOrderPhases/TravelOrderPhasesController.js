app.controller("TravelOrderPhasesCtrl", ["$scope", '$location', 'TravelOrderPhasesSrvc', function ($scope, $location, TravelOrderPhasesSrvc) {

    //Filed
    $scope.ID = null;
    $scope.UsersList = [];
    $scope.UserID = null;
    $scope.PhasesList = [];
    $scope.PhaseID = null;
    $scope.PhasesUsersList = [];

    //Control
    $scope.UsersSelectBox = {
        bindingOptions: {
            dataSource: "UsersList",
            value: "UserID",
            items: "UsersList"
        },

        placeholder: "--أختر--",
        noDataText: "لا يوجد بيانات",
        pagingEnabled: true, //Pagenation
        showClearButton: true,
        searchEnabled: true,
        displayExpr: "Text",
        valueExpr: "Value",
        disabled: false,

        onInitialized: function (e) {
            //Get Users
            TravelOrderPhasesSrvc.GetUsers(e.value).then(function (data) {
                $scope.UsersList = data.data;
            });
        },
        onValueChanged: function (e) {
            $scope.UserID = e.value;
            if ($scope.UserID == null) {
                $scope.DisabledUploadSignature = true;
                $scope.DisabledUploadStampOne = true;
                $scope.DisabledUploadStampTow = true;
            }
            else {
                $scope.DisabledUploadSignature = false;
                $scope.DisabledUploadStampOne = false;
                $scope.DisabledUploadStampTow = false;
            }
        }
    };

    $scope.PhasesSelectBox = {
        bindingOptions: {
            dataSource: "PhasesList",
            value: "PhaseID",
            items: "PhasesList"
        },

        placeholder: "--أختر--",
        noDataText: "لا يوجد بيانات",
        pagingEnabled: true, //Pagenation
        showClearButton: true,
        searchEnabled: true,
        displayExpr: "Text",
        valueExpr: "Value",
        disabled: false,

        onInitialized: function (e) {
            //Get Phases
            TravelOrderPhasesSrvc.GetPhases(e.value).then(function (data) {
                $scope.PhasesList = data.data;
            });
        },
        onValueChanged: function (e) {
            $scope.PhaseID = e.value;
        }
    };

    //validation 
    $scope.validationRequired = {
        validationRules: [
            {
                type: "required",
                message: "الحقل مطلوب"
            }
        ]
    };

    //File Uploading

    function readUrl(input,imgeId) {
        debugger;
        if (input.files && input.files[0]) {
            var reader = new FileReader();
            reader.onload = function (e) {
                debugger;
                $(imgeId).attr("src", e.target.result);
            };
            reader.readAsDataURL(input.files[0]);
        }
    };


    $scope.multiple = false;
    $scope.accept = "image/*";  //*,.pdf,.doc,.docx,.xls,.xlsx
    $scope.uploadMode = "useButtons";

    $scope.AcceptedSignatureFilesvalue = [];
    $scope.UrlUploadSignature = "";
    $scope.DisabledUploadSignature = true;

    $scope.fileUploadSignature = {
        name: "UploadSignature",
        //uploadUrl: "UrlUploadSignature",
        allowCanceling: true,
        rtlEnabled: true,
        readyToUploadMessage: "استعداد للرفع",
        selectButtonText: "اختر الصورة",
        labelText: "",
        uploadButtonText: "رفع",
        uploadedMessage: "تم الرفع",
        invalidFileExtensionMessage: "نوع الصورة غير مسموح",
        uploadFailedMessage: "خطأ أثناء الرفع",
        onInitialized: function (e) {
            debugger;
            $scope.AcceptedSignatureFilesInstance = e.component;
        },
        onValueChanged: function (e) {
            $scope.UrlUploadSignature = "/TravelOrderPhases/UploadSignature/" + Number($scope.UserID);

            if (e.value !== null) {
                var input = $("#fileUploadSignature").dxFileUploader("instance")._$fileInput[0];
                readUrl(input, "#imgSignature");
                if ($scope.AcceptedSignatureFilesvalue.length === 0)
                    $("#imgSignature").attr("src", "/assets/images/imgSignature.png");
            }
        },
        onUploaded: function (e) {
            debugger;
            var xhttp = e.request;
            if (xhttp.status === 200) {
                //$scope.AcceptedSignatureFilesvalue = '';
                //$scope.AcceptedSignatureFilesvalue.reset();
            } else if (xhttp.status === 404) {
                DevExpress.ui.notify({ message: "الملف المرفوع لايحتوي على بيانات" }, "Error", 10000);
                $scope.AcceptedSignatureFilesvalue = '';
                $scope.AcceptedSignatureFilesInstance.reset();

            } else {
                DevExpress.ui.notify({ message: "حدث خطأ أثناء رفع الصورة" }, "Error", 10000);
                $scope.AcceptedSignatureFilesvalue = '';
                $scope.AcceptedSignatureFilesInstance.reset();
            }
        },
        onUploadError: function (e) {
            debugger;
            var xhttp = e.request;
            if (xhttp.status === 500) {
                DevExpress.ui.notify({ message: "حدث خطأ أثناء رفع الملف" }, "Error", 10000);
                $scope.AcceptedSignatureFilesvalue = '';
                $scope.AcceptedSignatureFilesInstance.reset();
            }
        },
        bindingOptions: {
            uploadUrl: "UrlUploadSignature",
            multiple: "multiple",
            accept: "accept",
            value: "AcceptedSignatureFilesvalue",
            uploadMode: "uploadMode",
            disabled: "DisabledUploadSignature"
        }
    };
    //Remove File
    $scope.RemoveAcceptedSignatureFile = function (hashkey) {
        var nametoRemove = "";
        angular.forEach($scope.AcceptedSignatureFilesvalue,
            function (file, indx) {
                if (file.$$hashKey === hashkey) {
                    nametoRemove = file.name;
                    $scope.AcceptedSignatureFilesInstance.splice(indx, 1);
                }
            });
    };

    $scope.AcceptedStampOneFilesvalue = [];
    $scope.UrlUploadStampOne = "";
    $scope.DisabledUploadStampOne = true;

    $scope.fileUploadStampOne = {
        name: "UploadStampOne",
        //uploadUrl: "UrlUploadStampOne",
        allowCanceling: true,
        rtlEnabled: true,
        readyToUploadMessage: "استعداد للرفع",
        selectButtonText: "اختر الصورة",
        labelText: "",
        uploadButtonText: "رفع",
        uploadedMessage: "تم الرفع",
        invalidFileExtensionMessage: "نوع الصورة غير مسموح",
        uploadFailedMessage: "خطأ أثناء الرفع",
        onInitialized: function (e) {
            $scope.AcceptedStampOneFilesInstance = e.component;
        },
        onValueChanged: function (e) {
            $scope.UrlUploadStampOne = "/TravelOrderPhases/UploadStampOne/" + Number($scope.UserID);
            if (e.value !== null) {
                var input = $("#fileUploadStampOne").dxFileUploader("instance")._$fileInput[0];
                readUrl(input, "#imgStampOne");
                if ($scope.AcceptedStampOneFilesvalue.length === 0)
                    $("#imgStampOne").attr("src", "/assets/images/imgStampOne.png");
            }

        },
        onUploaded: function (e) {
            debugger;
            var xhttp = e.request;
            if (xhttp.status === 200) {
                //$scope.AcceptedStampOneFilesvalue = '';
                //$scope.AcceptedStampOneFilesvalue.reset();
            } else if (xhttp.status === 404) {
                DevExpress.ui.notify({ message: "الملف المرفوع لايحتوي على بيانات" }, "Error", 10000);
                $scope.AcceptedStampOneFilesvalue = '';
                $scope.AcceptedStampOneFilesInstance.reset();

            } else {
                DevExpress.ui.notify({ message: "حدث خطأ أثناء رفع الصورة" }, "Error", 10000);
                $scope.AcceptedStampOneFilesvalue = '';
                $scope.AcceptedStampOneFilesInstance.reset();
            }
        },
        onUploadError: function (e) {
            debugger;
            var xhttp = e.request;
            if (xhttp.status === 500) {
                DevExpress.ui.notify({ message: "حدث خطأ أثناء رفع الملف" }, "Error", 10000);
                $scope.AcceptedStampOneFilesvalue = '';
                $scope.AcceptedStampOneFilesInstance.reset();
            }
        },
        bindingOptions: {
            uploadUrl: "UrlUploadStampOne",
            multiple: "multiple",
            accept: "accept",
            value: "AcceptedStampOneFilesvalue",
            uploadMode: "uploadMode",
            disabled: "DisabledUploadStampOne"
        }
    };
    //Remove File
    $scope.RemoveAcceptedStampOneFile = function (hashkey) {
        var nametoRemove = "";
        angular.forEach($scope.AcceptedStampOneFilesvalue,
            function (file, indx) {
                if (file.$$hashKey === hashkey) {
                    nametoRemove = file.name;
                    $scope.AcceptedStampOneFilesInstance.splice(indx, 1);
                }
            });
    };

    $scope.AcceptedStampTowFilesvalue = [];
    $scope.UrlUploadStampTow = "";
    $scope.DisabledUploadStampTow = true;

    $scope.fileUploadStampTow = {
        name: "UploadStampTow",
        //uploadUrl: "UrlUploadStampTow",
        allowCanceling: true,
        rtlEnabled: true,
        readyToUploadMessage: "استعداد للرفع",
        selectButtonText: "اختر الصورة",
        labelText: "",
        uploadButtonText: "رفع",
        uploadedMessage: "تم الرفع",
        invalidFileExtensionMessage: "نوع الصورة غير مسموح",
        uploadFailedMessage: "خطأ أثناء الرفع",
        onInitialized: function (e) {
            $scope.AcceptedStampTowFilesInstance = e.component;
        },
        onValueChanged: function (e) {
            $scope.UrlUploadStampTow = "/TravelOrderPhases/UploadStampTow/" + Number($scope.UserID);
            if (e.value !== null) {
                var input = $("#fileUploadStampTow").dxFileUploader("instance")._$fileInput[0];
                readUrl(input, "#imgStampTow");
                if ($scope.AcceptedStampTowFilesvalue.length === 0)
                    $("#imgStampTow").attr("src", "/assets/images/imgStampTow.png");
            }
        },
        onUploaded: function (e) {
            debugger;
            var xhttp = e.request;
            if (xhttp.status === 200) {
                //$scope.AcceptedStampTowFilesvalue = '';
                //$scope.AcceptedStampTowFilesvalue.reset();
            } else if (xhttp.status === 404) {
                DevExpress.ui.notify({ message: "الملف المرفوع لايحتوي على بيانات" }, "Error", 10000);
                $scope.AcceptedStampTowFilesvalue = '';
                $scope.AcceptedStampTowFilesInstance.reset();

            } else {
                DevExpress.ui.notify({ message: "حدث خطأ أثناء رفع الصورة" }, "Error", 10000);
                $scope.AcceptedStampTowFilesvalue = '';
                $scope.AcceptedStampTowFilesInstance.reset();
            }
        },
        onUploadError: function (e) {
            debugger;
            var xhttp = e.request;
            if (xhttp.status === 500) {
                DevExpress.ui.notify({ message: "حدث خطأ أثناء رفع الملف" }, "Error", 10000);
                $scope.AcceptedStampTowFilesvalue = '';
                $scope.AcceptedStampTowFilesInstance.reset();
            }
        },
        bindingOptions: {
            uploadUrl: "UrlUploadStampTow",
            multiple: "multiple",
            accept: "accept",
            value: "AcceptedStampTowFilesvalue",
            uploadMode: "uploadMode",
            disabled: "DisabledUploadStampTow"
        }
    };
    //Remove File
    $scope.RemoveAcceptedStampTowFile = function (hashkey) {
        var nametoRemove = "";
        angular.forEach($scope.AcceptedStampTowFilesvalue,
            function (file, indx) {
                if (file.$$hashKey === hashkey) {
                    nametoRemove = file.name;
                    $scope.AcceptedStampTowFilesInstance.splice(indx, 1);
                }
            });
    };


    //btn Save
    $scope.btnSave = {
        text: 'حفظ',
        type: 'success',
        useSubmitBehavior: true
    };


    //Function
    $scope.SavePhasesUsers = function () {

        debugger;

        if ($scope.AcceptedSignatureFilesvalue.length <= 0)
            return DevExpress.ui.notify({ message: "ارفع صورة التوقيع", type: "error", displayTime: 3000, closeOnClick: true });

        if ($scope.AcceptedStampOneFilesvalue.length <= 0)

            return DevExpress.ui.notify({ message: "ارفع صورة الختم الاول", type: "error", displayTime: 3000, closeOnClick: true });

        if ($scope.AcceptedStampTowFilesvalue.length <= 0)
            return DevExpress.ui.notify({ message: "ارفع صورة الختم الثاني", type: "error", displayTime: 3000, closeOnClick: true });


        debugger;
        TravelOrderPhasesSrvc.SavePhasesUsers({ ID: Number($scope.ID), UserID: Number($scope.UserID), PhaseID: Number($scope.PhaseID) }).then(function (data) {
            if (data.data.status == 500) {
                DevExpress.ui.notify({ message: data.data.Message, type: data.data.Type, displayTime: 3000, closeOnClick: true });
            }
            if (data.data.status == 200) {
                DevExpress.ui.notify({ message: data.data.Message, type: data.data.Type, displayTime: 3000, closeOnClick: true });
                debugger;
                $scope.ID = null;
                $scope.UserID = null;
                $scope.PhaseID = null;
                $scope.AcceptedSignatureFilesvalue = '';
                $scope.AcceptedStampOneFilesvalue = '';
                $scope.AcceptedStampTowFilesvalue = '';
                //Get Phases Users
                TravelOrderPhasesSrvc.GetPhasesUsers().then(function (data) {
                    $scope.PhasesUsersList = data.data;
                });
            }
        });

    }
    //dataGrid
    $scope.gridPhasesUsers = {
        bindingOptions: {
            dataSource: "PhasesUsersList"
        },
        grouping: {
            autoExpandAll: false
        },
        noDataText: "لا يوجد بيانات",
        selection: {
            mode: "single"
        },
        showBorders: true,
        paging: {
            pageSize: 10
        },
        pager: {
            showPageSizeSelector: true,
            allowedPageSizes: [5, 10, 20],
            showInfo: true
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
            scrolling: {
                rtlEnabled: true,
                useNative: true,
                scrollByContent: true,
                scrollByThumb: true,
                showScrollbar: "onHover",
                mode: "standard", // or "virtual"
                direction: "both"
            },
            resetOperationText: "الوضع الافتراضى"
        },
        rowAlternationEnabled: true,
        allowColumnReordering: true,
        allowColumnResizing: true,
        columnAutoWidth: true,
        columnChooser: {
            enabled: false
        },
        columns: [
            {
                caption: "اسم المسؤول",
                dataField: "UserName",
                cssClass: "text-right",
                groupIndex: 0
            },
            {
                caption: "المرحلة",
                dataField: "PhaseName",
                cssClass: "text-right"
            },
            {
                caption: "التوقيع",
                width: 200,
                cellTemplate: function (container, options) {
                    $('<img class="img-responsive"/>')
                        .height(100)
                        .attr('src', "../Content/UserFiles/" + options.data.UserID + "/المستندات/" + options.data.Signature)
                        .appendTo(container);
                }
            },
            {
                caption: "الختم الاول",
                width: 200,
                cellTemplate: function (container, options) {
                    $('<img class="img-responsive"/>')
                        .height(100)
                        .attr('src', "../Content/UserFiles/" + options.data.UserID + "/المستندات/" + options.data.StampOne)
                        .appendTo(container);
                }
            },
            {
                caption: "الختم الثاني",
                width: 200,
                cellTemplate: function (container, options) {
                    $('<img class="img-responsive"/>')
                        .height(100)
                        .attr('src', "../Content/UserFiles/" + options.data.UserID + "/المستندات/" + options.data.StampTow)
                        .appendTo(container);
                }
            },
            {
                caption: "تعديل",
                width: 100,
                cssClass: "text-center",
                cellTemplate: function (container, options) {
                    $("<div />").dxButton({
                        icon: "fa fa-edit",
                        //text: "تعديل",
                        type: "warning",
                        hint: "تعديل",
                        elementAttr: { "class": "btn btn-warning" },
                        onClick: function (e) {
                            debugger;
                            $scope.ID = options.data.ID;
                            $scope.UserID = options.data.UserID.toString();
                            $scope.PhaseID = options.data.PhaseID.toString();
                            $scope.AcceptedSignatureFilesvalue = options.data.Signature;
                            $scope.AcceptedStampOneFilesvalue = options.data.StampOne;
                            $scope.AcceptedStampTowFilesvalue = options.data.StampTow;
                        }
                    }).appendTo(container);
                }
            },
            {
                caption: "تنشيط",
                width: 100,
                cssClass: "text-center",
                cellTemplate: function (container, options) {
                    if (options.data.IsActive == true) {
                        $("<div />").dxButton({
                            icon: "fa fa-unlock",
                            //text: "ايقاف",
                            type: "danger",
                            hint: "ايقاف",
                            elementAttr: { "class": "btn btn-danger" },

                            onClick: function (e) {
                                TravelOrderPhasesSrvc.ActivePhasesUsers(options.data.ID).then(function (data) {
                                    if (data.data.status == 500) {
                                        DevExpress.ui.notify({ message: data.data.Message, type: data.data.Type, displayTime: 3000, closeOnClick: true });
                                    }
                                    if (data.data.status == 200) {
                                        DevExpress.ui.notify({ message: data.data.Message, type: data.data.Type, displayTime: 3000, closeOnClick: true });
                                        //Get Phases Users
                                        TravelOrderPhasesSrvc.GetPhasesUsers().then(function (data) {
                                            $scope.PhasesUsersList = data.data;
                                        });
                                    }
                                });
                            }
                        }).appendTo(container);
                    }
                    else {
                        $("<div />").dxButton({
                            icon: "fa fa-unlock-alt",
                            //text: "تنشيط",
                            type: "success",
                            hint: "تنشيط",
                            elementAttr: { "class": "btn btn-success" },
                            onClick: function (e) {
                                TravelOrderPhasesSrvc.ActivePhasesUsers(options.data.ID).then(function (data) {
                                    if (data.data.status == 500) {
                                        DevExpress.ui.notify({ message: data.data.Message, type: data.data.Type, displayTime: 3000, closeOnClick: true });
                                    }
                                    if (data.data.status == 200) {
                                        DevExpress.ui.notify({ message: data.data.Message, type: data.data.Type, displayTime: 3000, closeOnClick: true });
                                        //Get Phases Users
                                        TravelOrderPhasesSrvc.GetPhasesUsers().then(function (data) {
                                            $scope.PhasesUsersList = data.data;
                                        });
                                    }
                                });
                            }
                        }).appendTo(container);
                    }
                }
            },
            {
                caption: "حذف",
                width: 100,
                cssClass: "text-center",
                cellTemplate: function (container, options) {
                    $("<div />").dxButton({
                        icon: "fa fa-trash-o",
                        //text: "حذف",
                        type: "danger",
                        hint: "حذف",
                        elementAttr: { "class": "btn btn-danger btn-sm" },
                        onClick: function (e) {
                            var result = DevExpress.ui.dialog.confirm("<i>هل انت متاكد؟</i>", "تاكيد الحذف");
                            result.done(function (dialogResult) {
                                if (dialogResult) {
                                    TravelOrderPhasesSrvc.DeletePhasesUsers(options.data.ID).then(function (data) {
                                        if (data.data.status == 500) {
                                            DevExpress.ui.notify({ message: data.data.Message, type: data.data.Type, displayTime: 3000, closeOnClick: true });
                                        }
                                        if (data.data.status == 200) {
                                            DevExpress.ui.notify({ message: data.data.Message, type: data.data.Type, displayTime: 3000, closeOnClick: true });
                                            //Get Phases Users
                                            TravelOrderPhasesSrvc.GetPhasesUsers().then(function (data) {
                                                $scope.PhasesUsersList = data.data;
                                            });
                                        }
                                    });
                                }
                            });
                        }
                    }).appendTo(container);
                }
            }
        ],

        onContentReady: function (e) {
            e.component.columnOption("command:edit", {
                visibleIndex: -1
            });
        },
        onSelectionChanged: function (info) {
            $scope.GridRowData = info.selectedRowKeys;
        },
        onInitialized: function (e) {
            //Get Phases Users
            TravelOrderPhasesSrvc.GetPhasesUsers().then(function (data) {
                $scope.PhasesUsersList = data.data;
            });
        }
    };

}]);


