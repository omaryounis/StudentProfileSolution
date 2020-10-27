app.controller("AddViolationCtrl", ["$scope", 'AddViolatioSrvc',
    function ($scope, addViolatioSrvc) {

        $scope.studentsList = [];
        $scope.studentID = null;

        $scope.IssuesCategoriesList = [];
        $scope.IssuesCategoryId = null;

        $scope.IssuesList = [];
        $scope.IssuesId = null;
        $scope.violationNumber = null;


        $scope.violationDescription = "";

        $scope.IssuesCategoriesSelectBox = {
            bindingOptions: {
                dataSource: "IssuesCategoriesList",
                value: "IssuesCategoryId",
                items: "IssuesCategoriesList"
            },
            onInitialized: function (e) {
                addViolatioSrvc.GetIssuesCategories().then(function (data) {
                    debugger;
                    $scope.IssuesCategoriesList = data.data;
                });
            },
            onValueChanged: function (e) {
                debugger;
                addViolatioSrvc.GetIssuesByCategoryId(e.value).then(function (data) {
                    debugger;
                    $scope.IssuesList = data.data;
                });
            },
            placeholder: "--أختر--",
            noDataText: "لا يوجد بيانات",
            pagingEnabled: true, //Pagenation
            showClearButton: true,
            searchEnabled: true,
            displayExpr: "Text",
            valueExpr: "Value",
            searchExpr: ['Text', 'Value', 'NationalID']
        };
        $scope.IssuesSelectBox = {
            bindingOptions: {
                dataSource: "IssuesList",
                value: "IssuesId",
                items: "IssuesList"
            },
            onValueChanged: function (e) {
                debugger;
                addViolatioSrvc.GetIssueNumber(e.value).then(function (data) {
                    debugger;
                    $scope.violationNumber = data.data;
                });
            },

            placeholder: "--أختر--",
            noDataText: "لا يوجد بيانات",
            pagingEnabled: true, //Pagenation
            showClearButton: true,
            searchEnabled: true,
            displayExpr: "Text",
            valueExpr: "Value",
            searchExpr: ['Text', 'Value']
        };


        var DDL_StudentsDataSource = new DevExpress.data.DataSource({
            paginate: true,
            cacheRawData: true,
            key: "STUDENT_ID",
            loadMode: "raw",
            load: function () {
                debugger;
                return $.getJSON("/Housing/GetStudentForHousingViolations", function (data) {
                });
            }
        });
        DDL_StudentsDataSource.load();
        $scope.StudentsSelectBox = {
            dataSource: DDL_StudentsDataSource,
            bindingOptions: {
                //dataSource: "studentsList",
                value: "studentID",
                //items: "studentsList"
            },
            placeholder: "--أختر--",
            noDataText: "لا يوجد بيانات",
            pagingEnabled: true, //Pagenation
            showClearButton: true,
            searchEnabled: true,
            displayExpr: "Text",
            valueExpr: "Value",
            searchExpr: ['Text', 'Value', 'NationalID']
            //onInitialized: function (e) {
            //    addViolatioSrvc.GetStudentForHousingViolations().then(function (data) {
            //        $scope.studentsList = data.data;
            //    });
            //},
        };


        $scope.violationDateDatePicker = {
            bindingOptions: {
                value: "violationDate"
            },
            type: "datetime",
            onInitialized: function (e) {
                $scope.violationDate = new Date();
            }
        };


        $scope.violationDescriptionTextArea = {
            bindingOptions: {
                text: "violationDescription"
            }
        };


        $scope.fileValues = [];
        $scope.options = {
            uploadUrl: "/ViolationsDecisions/Upload",
            bindingOptions: {
                value: "fileValues"
            },
            name: "fileSent",
            multiple: true,
            accept: "*",
            uploadMode: "instantly"

        };

        $scope.btnSave = {
            text: 'حفظ',
            type: 'success',
            onClick: function (e) {
                debugger;
                if ($scope.studentID == null || $scope.studentID == '') {
                    DevExpress.ui.notify({
                        message: 'عفوا ادخل اسم الطالب',
                        type: "error",
                        displayTime: 3000,
                        closeOnClick: true
                    });
                    return;
                }


                if ($scope.IssuesCategoryId == null || $scope.IssuesCategoryId == '') {
                    DevExpress.ui.notify({
                        message: 'عفواادخل قسم المخالفة',
                        type: "error",
                        displayTime: 3000,
                        closeOnClick: true
                    });
                    return;
                }



                if ($scope.IssuesId == null || $scope.IssuesId == '') {
                    DevExpress.ui.notify({
                        message: 'عفوا ادخل نوع المخالفة',
                        type: "error",
                        displayTime: 3000,
                        closeOnClick: true
                    });
                    return;
                }


                if ($scope.violationDate == null || $scope.violationDate == '') {
                    DevExpress.ui.notify({
                        message: 'عفوا ادخل وقت وتاريخ المخالفة',
                        type: "error",
                        displayTime: 3000,
                        closeOnClick: true
                    });
                    return;
                }


                if ($scope.violationDescription == "") {
                    DevExpress.ui.notify({
                        message: 'عفوا ادخل تفاصيل المخالفة',
                        type: "error",
                        displayTime: 3000,
                        closeOnClick: true
                    });
                    return;
                }

                addViolatioSrvc
                    .SaveViolation($scope.studentID, $scope.IssuesId, $scope.violationDate, $scope.violationDescription)
                    .then(function (data) {
                        if (data.data === "") {
                            DevExpress.ui.notify({
                                message: 'تم حفظ المخالفة بنجاح',
                                type: "success",
                                displayTime: 3000,
                                closeOnClick: true
                            });
                        }
                    });
            }
        };

    }]);