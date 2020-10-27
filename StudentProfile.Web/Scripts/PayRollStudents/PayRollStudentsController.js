app.controller("PayRollStudentsCtrl", ["$scope", 'PayRollStudentsSrvc', function ($scope, PayRollStudentsSrvc) {
    //Edit Advertisement

    $scope.FacultionList = [];
    $scope.FacultyID = null;
    $scope.DegreeList = [];
    $scope.DegreeID = null;
    $scope.YearList = [];
    $scope.YearID = null;
    $scope.MonthList = [];
    $scope.MonthID = null;

    $scope.FacultionSelectBox = {
        bindingOptions: {
            dataSource: "FacultionList",
            value: "FacultyID",
            items: "FacultionList"
        },

        placeholder: "--أختر--",
        noDataText: "لا يوجد بيانات",
        paginate: true, //Pagenation
        showClearButton: true,
        searchEnabled: true,
        displayExpr: "Text",
        valueExpr: "Value",
        disabled: false,

        onInitialized: function (e) {
            PayRollStudentsSrvc.GetFaculties().then(function (data) {
                $scope.FacultionList = data.data;
            });
        },
        onValueChanged: function (e) {
            $scope.FacultyID = e.value;
        }
    };

    $scope.DegreeSelectBox = {
        bindingOptions: {
            dataSource: "DegreeList",
            value: "DegreeID",
            items: "DegreeList"
        },

        placeholder: "--أختر--",
        noDataText: "لا يوجد بيانات",
        paginate: true, //Pagenation
        showClearButton: true,
        searchEnabled: true,
        displayExpr: "Text",
        valueExpr: "Value",
        disabled: false,

        onInitialized: function (e) {
            PayRollStudentsSrvc.GetDegrees().then(function (data) {
                $scope.DegreeList = data.data;
            });
        },
        onValueChanged: function (e) {
            $scope.DegreeID = e.value;
        }
    };

    $scope.YearSelectBox = {
        bindingOptions: {
            dataSource: "YearList",
            value: "YearID",
            items: "YearList"
        },

        placeholder: "--أختر--",
        noDataText: "لا يوجد بيانات",
        paginate: true, //Pagenation
        showClearButton: true,
        searchEnabled: true,
        displayExpr: "Text",
        valueExpr: "Value",
        disabled: false,

        onInitialized: function (e) {
            PayRollStudentsSrvc.GetYears().then(function (data) {
                $scope.YearList = data.data;
            });
        },
        onValueChanged: function (e) {
            $scope.YearID = e.value;
        }
    };

    $scope.MonthSelectBox = {
        bindingOptions: {
            dataSource: "MonthList",
            value: "MonthID",
            items: "MonthList"
        },

        placeholder: "--أختر--",
        noDataText: "لا يوجد بيانات",
        paginate: true, //Pagenation
        showClearButton: true,
        searchEnabled: true,
        displayExpr: "Text",
        valueExpr: "Value",
        disabled: false,

        onInitialized: function (e) {
            PayRollStudentsSrvc.GetMonths().then(function (data) {
                $scope.MonthList = data.data;
            });
        },
        onValueChanged: function (e) {
            $scope.MonthID = e.value;
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

    //btn View
    $scope.btnView = {
        text: 'عرض',
        type: 'success',
        useSubmitBehavior: true
    };


    $scope.ViewPayRollStudents = function () {
        PayRollStudentsSrvc.PayRollStudentsReport({ FacultyID: $scope.FacultyID, DegreeID: $scope.DegreeID }).then(function (data) {
            debugger;
            $('#reportData').html(data.data);
        });
    }
}]);
