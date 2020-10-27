app.controller("ViolationDescisionTakingCtrl", ["$scope", 'ViolationsDecisionsTakingSrvc',
    function ($scope, ViolationsDecisionsTakingSrvc) {

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
            ViolationsDecisionsTakingSrvc.GetHangedViolationsForDecisionTaking().then(function (data) {
                debugger;
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
                { caption: "تاريخ و وقت المخالفة", dataField: "ViolationDate", dataType: 'date'},
                { caption: "تاريخ الادخال", dataField: "InsertDate", dataType: 'date'},
                { caption: "رقم المخالفة", dataField: "ViolationNumber" },
                { caption: "نوع المخالفة", dataField: "IssueDescription" },
                { caption: "قسم المخالفة", dataField: "IssuesCategory" },
                { caption: "تفاصيل المخالفة", dataField: "ViolationDescription" },
                { caption: "عدد المخالفات", dataField: "ViolationCount" },
                { caption: "موجة الي", dataField: "ForwordUser" },
                {
                    caption: "مرفق المخالفة",
                    width: 110,
                    cssClass: "text-center",
                    cellTemplate: function (container, options) {
                        if (options.data.ViolationAttachment != null) {

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
                    }
                },
                { caption: "اتخاذ قرار", dataField: "Decision" },
                {
                    caption: "مرفق القرار",
                    width: 100,
                    cssClass: "text-center",
                    cellTemplate: function (container, options) {
                        if (options.data.ForwordFile != null) {
                            debugger;
                            $("<a />").dxButton({
                                icon: "fa fa-download",
                                //text: "تحميل",
                                type: "success",
                                //hint: "حذف",
                                elementAttr: { "class": "btn btn-danger btn-sm", "href": options.data.ForwordFile, "download": "مرفق قرار المخالفة الموجة رقم [" + options.data.Id + "]" },
                                onClick: function (e) {
                                }
                            }).appendTo(container);
                        }
                    }
                },

                {
                    caption: "رفض",
                    width: 100,
                    cssClass: "text-center",
                    cellTemplate: function (container, options) {
                        if (options.data.DecisionId == null) {
                            $("<div />").dxButton({
                                icon: "fa fa-thumbs-down",
                                //text: "حذف",
                                type: "danger",
                                hint: "حذف",
                                elementAttr: { "class": "btn btn-danger btn-sm" },
                                onClick: function (e) {
                                    debugger;
                                    $scope.refusePopupVisible = true;


                                    let obj = $scope.ViolationsArrey.find(o => o.Id === options.data.Id);
                                    $scope.violationToBeEditted = obj;

                                }
                            }).appendTo(container);
                        }
                    }
                },
                {
                    caption: "توجية",
                    width: 100,
                    cssClass: "text-center",
                    cellTemplate: function (container, options) {
                        if (options.data.DecisionId == null) {
                            $("<div />").dxButton({
                                icon: "fa fa-angle-double-left",
                                //text: "توجية",
                                type: "primary",
                                hint: "توجية",
                                elementAttr: { "class": "btn btn-primary btn-sm" },
                                onClick: function (e) {
                                    debugger;
                                    $scope.violationIdForword = options.data.Id;
                                    $scope.forwordPopupVisible = true;
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
                allowedPageSizes: [5, 10, 20]
            },
            "export": {
                enabled: true,
                fileName: "قائمة طلاب الدراسات العليا"
            },
            searchPanel: {
                visible: true,
                placeholder: "بحث",
                width: 300
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
            showRowLines: false,
            groupPanel: {
                visible: true,
                emptyPanelText: "اسحب عمود هنا"
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
        $scope.refusePopup = {
            bindingOptions: {
                visible: "refusePopupVisible"
            },
            height: 500,
            showTitle: true,
            title: "توصيات رفض المخالفة",
            dragEnabled: false
        };


        $scope.acceptPopup = {
            bindingOptions: {
                visible: "acceptPopupVisible"
            },
            height: 500,
            showTitle: true,
            title: "توصيات اقرار المخالفة",
            dragEnabled: false
        };
        $scope.recommendationsOfRefuseTextArea = {
            bindingOptions: {
                value: "recommendationsOfRefuse"
            },
            height: 120,
            placeholder: "ادخل توصيات المخالفة"
        };
        $scope.recommendationsOfAcceptTextArea = {
            bindingOptions: {
                value: "recommendationsOfAccept"
            },
            height: 120,
            placeholder: "ادخل توصيات المخالفة"
        };
        $scope.refuseButton = {
            text: "رفض",
            type: "danger",
            width: "150",
            onClick: function (e) {

                if ($scope.recommendationsOfRefuse === "") {
                    DevExpress.ui.notify({
                        message: "عفوا ادخل توصيات على رفض المخالفة",
                        type: "error",
                        displayTime: 10000,
                        closeOnClick: true
                    });
                    return;
                }

                ViolationsDecisionsTakingSrvc.RefuseViolation($scope.violationToBeEditted.Id, $scope.recommendationsOfRefuse)
                    .then(function (data) {
                        if (data.data == "") {
                            DevExpress.ui.notify({
                                message: "تم رفض المخالفة بنجاح",
                                type: "success",
                                displayTime: 10000,
                                closeOnClick: true
                            });
                            fillViolationsGrid();
                            $scope.refusePopupVisible = false;
                        } else {
                            DevExpress.ui.notify({
                                message: data.data,
                                type: "error",
                                displayTime: 10000,
                                closeOnClick: true
                            });
                        }
                    });
            }
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
                debugger;
                ViolationsDecisionsTakingSrvc
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
                debugger;
                ViolationsDecisionsTakingSrvc.GetDecisionList().then(function (data) {
                    debugger;
                    $scope.decisionList = data.data;
                });
            }
        };



        /***************************Forword Violation*************************/
        $scope.forwordPopupVisible = false;
        $scope.forwordViolationList = [];

        $scope.forwordPopup = {
            bindingOptions: {
                visible: "forwordPopupVisible"
            },
            height: 500,
            showTitle: true,
            title: "توجية المخالفات",
            onShowing: function (e) {
                debugger;
                //Referesh gridForwordViolation
                $('#gridForwordViolation').dxDataGrid('instance').refresh();
            }

        };

        //dataGrid
        $scope.gridForwordViolation = {
            bindingOptions: {
                dataSource: "forwordViolationList"
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
                    dataField: "Name",
                    cssClass: "text-right"
                },
                {
                    caption: "عدد المخالفات",
                    dataField: "countViolation",
                    cssClass: "text-right"
                },
                {
                    caption: "توجية",
                    width: 100,
                    cssClass: "text-center",
                    cellTemplate: function (container, options) {
                        debugger;
                        ViolationsDecisionsTakingSrvc.GetUserForForwordByViolationId($scope.violationIdForword).then(function (data) {
                            if (data.data !== options.data.ID) {
                                $("<div />").dxButton({
                                    icon: "fa fa-angle-double-left",
                                    //text: "توجية",
                                    type: "success",
                                    hint: "توجية",
                                    elementAttr: { "class": "btn btn-success btn-sm" },
                                    onClick: function (e) {
                                        debugger;
                                        ViolationsDecisionsTakingSrvc.ForwordViolationToUser({ UserId: options.data.ID, ViolationId: $scope.violationIdForword }).then(function (data) {
                                            if (data.data.status == 200) {
                                                debugger;
                                                DevExpress.ui.notify({ message: data.data.Message, type: data.data.Type, displayTime: 3000, closeOnClick: true });
                                                //Referesh Grid violation
                                                fillViolationsGrid();
                                                //close popup
                                                $scope.forwordPopupVisible = false;
                                            }
                                            if (data.data.status == 500) {
                                                DevExpress.ui.notify({ message: data.data.Message, type: data.data.Type, displayTime: 3000, closeOnClick: true });
                                            }
                                        });
                                    }
                                }).appendTo(container);
                            }
                        });
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
                ViolationsDecisionsTakingSrvc.GetUsersToForword().then(function (data) {
                    if (data) {
                        $scope.forwordViolationList = data.data;
                    }
                });
            }
        }

    }]);