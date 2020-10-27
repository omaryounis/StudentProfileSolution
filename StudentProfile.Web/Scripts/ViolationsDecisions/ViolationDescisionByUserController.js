app.controller("ViolationDescisionByUserCtrl", ["$scope", 'ViolationDescisionByUserSrvc',
    function ($scope, ViolationDescisionByUserSrvc) {

        $scope.ViolationsArrey = [];
        $scope.decisionList = [];
        $scope.decisionId = null;
        $scope.violationToBeEditted = {};
        $scope.acceptPopupVisible = false;
        $scope.refusePopupVisible = false;
        $scope.recommendationsOfRefuse = "";
        $scope.recommendationsOfAccept = "";

        $scope.violationIdForword = null;


        function fillViolationsGrid() {
            ViolationDescisionByUserSrvc.GetViolationsForDecisionByUser().then(function (data) {
                $scope.ViolationsArrey = data.data;
            });
        }

        $scope.violationsGridView = {
            bindingOptions: { dataSource: "ViolationsArrey" },
            width: "100%",
            columns: [
                { caption: "مسلسل", dataField: "Id", visible: false },
                { caption: "الطالب", dataField: "studentName" },
                { caption: "الرقم الجامعي", dataField: "studentId" },
                { caption: "رقم الهوية", dataField: "nationalId" },
                { caption: "تاريخ و وقت المخالفة", dataField: "ViolationDate" },
                { caption: "تاريخ الادخال", dataField: "InsertDate" },
                { caption: "رقم المخالفة", dataField: "ViolationNumber" },
                { caption: "نوع المخالفة", dataField: "IssueDescription" },
                { caption: "قسم المخالفة", dataField: "IssuesCategory" },
                { caption: "تفاصيل المخالفة", dataField: "ViolationDescription" },
                { caption: "عدد المخالفات", dataField: "ViolationCount" },
                {
                    caption: "المرفق",
                    width: 100,
                    cssClass: "text-center",
                    cellTemplate: function (container, options) {
                        debugger;
                        $("<a />").dxButton({
                            icon: "fa fa-download",
                            //text: "تحميل",
                            type: "success",
                            //hint: "حذف",
                            elementAttr: { "class": "btn btn-danger btn-sm", "href": options.data.ViolationAttachment, "download": "مرفق القرار رقم [" + options.data.Id + "]" },
                            onClick: function (e) {
                            }
                        }).appendTo(container);
                    }
                },
                {
                    caption: "اقرار المخالفة",
                    width: 100,
                    cssClass: "text-center",
                    cellTemplate: function (container, options) {
                        debugger;
                        if (options.data.IsAccepted == false) {
                            $("<div />").dxButton({
                                icon: "fa fa-check",
                                //text: "حذف",
                                type: "danger",
                                hint: "اقرار المخالفة",
                                elementAttr: { "class": "btn btn-danger btn-sm" },
                                onClick: function (e) {
                                    debugger;
                                    $scope.acceptPopupVisible = true;
                                    let obj = $scope.ViolationsArrey.find(o => o.Id === options.data.Id);
                                    $scope.violationToBeEditted = obj;

                                }
                            }).appendTo(container);
                        }
                    }
                }

            ],
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
            onInitialized: function (e) {
                fillViolationsGrid();

            }
        };

        $scope.acceptPopup = {
            bindingOptions: {
                visible: "acceptPopupVisible"
            },
            showTitle: true,
            title: "توصيات اقرار المخالفة",
            dragEnabled: false
        };
        $scope.recommendationsOfAcceptTextArea = {
            bindingOptions: {
                value: "recommendationsOfAccept"
            },
            height: 120,
            placeholder: "ادخل توصيات المخالفة"
        };

        $scope.acceptButton = {
            text: "اقرار",
            type: "danger",
            width: "150",
            onClick: function (e) {

                if ($scope.recommendationsOfAccept === "") {
                    DevExpress.ui.notify({
                        message: "عفوا ادخل توصيات على رفض المخالفة",
                        type: "error",
                        displayTime: 10000,
                        closeOnClick: true
                    });
                    return;
                }


                if ($scope.decisionId == null) {
                    DevExpress.ui.notify({
                        message: "عفوا اختر القرار",
                        type: "error",
                        displayTime: 10000,
                        closeOnClick: true
                    });
                    return;
                }
                ViolationDescisionByUserSrvc
                    .ApproveViolation($scope.violationToBeEditted.Id, $scope.decisionId, $scope.recommendationsOfAccept)
                    .then(function (date) {

                        DevExpress.ui.notify({
                            message: "تم اقرار المخالفة بنجاح",
                            type: "success",
                            displayTime: 10000,
                            closeOnClick: true
                        });
                        $scope.acceptPopupVisible = false;
                        fillViolationsGrid();
                        $scope.recommendationsOfAccept = null;
                        $scope.decisionId = null;
                    });


            }
        };

        $scope.decisionsSelectBox = {
            bindingOptions: {
                dataSource: "decisionList",
                value: "decisionId",
                items: "decisionList"
            },
            displayExpr: "Text",
            valueExpr: "Value",
            onInitialized: function (e) {
                ViolationDescisionByUserSrvc.GetDecisionList().then(function (data) {
                    $scope.decisionList = data.data;
                });
            }
        };


        //File Uploading
        $scope.ViolationForwordsFilesvalue = [];
        $scope.multiple = false;
        $scope.accept = "image/*,.pdf,.doc,.docx,.xls,.xlsx",
            $scope.uploadMode = "useButtons";

        $scope.fileUploadViolationForwords = {
            name: "fileUploadViolationForwords",
            uploadUrl: "/ViolationsDecisions/UploadViolationForwordsFiles",
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
                $scope.fileUploadViolationForwordsInstance = e.component;
            },
            onUploaded: function (e) {
                debugger;
                var xhttp = e.request;
                if (xhttp.status === 200) {
                    //$scope.ViolationForwordsFilesvalue = '';
                    //$scope.ViolationForwordsFilesvalue.reset();
                } else if (xhttp.status === 404) {
                    DevExpress.ui.notify({ message: "الملف المرفوع لايحتوي على بيانات" }, "Error", 10000);
                    $scope.ViolationForwordsFilesvalue = '';
                    $scope.ViolationForwordsFilesInstance.reset();

                } else {
                    DevExpress.ui.notify({ message: "حدث خطأ أثناء رفع الملف" }, "Error", 10000);
                    $scope.ViolationForwordsFilesvalue = '';
                    $scope.ViolationForwordsFilesInstance.reset();
                }
            },
            onUploadError: function (e) {
                debugger;
                var xhttp = e.request;
                if (xhttp.status === 500) {
                    DevExpress.ui.notify({ message: "حدث خطأ أثناء رفع الملف" }, "Error", 10000);
                    $scope.ViolationForwordsFilesvalue = '';
                    $scope.ViolationForwordsFilesInstance.reset();
                }
            },
            bindingOptions: {
                multiple: "multiple",
                accept: "accept",
                value: "ViolationForwordsFilesvalue",
                uploadMode: "uploadMode",
                disabled: "DisabledUploadSignature"
            }
        };
        //Remove File
        $scope.RemoveViolationForwordsFile = function (hashkey) {
            var nametoRemove = "";
            angular.forEach($scope.ViolationForwordsFilesvalue,
                function (file, indx) {
                    if (file.$$hashKey === hashkey) {
                        nametoRemove = file.name;
                        $scope.ViolationForwordsFilesInstance.splice(indx, 1);
                    }
                });
        };

    }]);