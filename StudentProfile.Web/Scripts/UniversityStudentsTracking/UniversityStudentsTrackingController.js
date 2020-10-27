(function() {
    app.controller("UniversityStudentsTrackingCtrl",
        [
            "$scope", "$http", function($scope, $http) {
                $scope.defaultitem = {};
                $scope.UID = null;
                $scope.GroupName = "";
                $scope.School = "";
                $scope.IsAdminGroup = true;
                $scope.IsActive = true;


                $scope.Permissions = {
                    Create: false,
                    Read: false,
                    Update: false,
                    Delete: false,
                    View: false
                };




                $scope.Refresh = function() {
                    $http({
                        method: "Get",
                        url: "/UniversityStudentsTracking/GetUniversityStudents"
                    }).then(function(data) {
                        $scope.UniversityStudents = data.data;
                    });
                };

                $scope.Refresh();

                $scope.dataGridOptions = {

                    bindingOptions: {
                        dataSource: "UniversityStudents",
                        rtlEnabled: true,
                        rowAlternationEnabled: true
                    },
                    noDataText: "لا يوجد بيانات",
                    KeyExpr:"ID",
                    paging: {
                        pageSize: 10

                    },
                    pager: {
                        showPageSizeSelector: true,
                        allowedPageSizes: [5, 10, 20],
                        showInfo: true
                    },
                    "export": {
                        enabled: true,
                        fileName: "مسير مكافئات الطلاب"
                        //allowExportSelectedData: true
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
                    allowColumnResizing: true,
                    columnResizingMode: "widget",
                    allowColumnReordering: true,
                    showColumnLines: true,
                    sorting: {
                        mode: "multiple"
                    },
                    rowAlternationEnabled: true,
                    hoverStateEnabled: true,
                    columnChooser: {
                        enabled: false
                    }, headerFilter: {
                        visible: true
                    },
                    showRowLines: true, 
                    columns: [
                        {
                            dataField: "ID",
                            visible: false
                        },
                        {
                            dataField: "NamePerIdentity_Ar",
                            caption: "إسم الطالب"
                        },
                        {
                            dataField: "Student_ID",
                            caption: "الرقم الإكاديمي"
                        },
                        {
                            dataField: "National_ID",
                            caption: "رقم الهوية"
                        },
                        {
                            dataField: "MobileNumber",
                            caption: "رقم الجوال"
                        },
                        {
                            dataField: "PersonalEmail",
                            caption: "البريد الإلكتروني"
                        },
                        {
                            dataField: "InsertDate",
                            caption: "تاريخ الإدخال"
                        },

                        {
                            caption: "#",
                            width: 100,
                            cssClass: "text-center",
                            cellTemplate: function (container, options) {
                                debugger;
                               
                                $("<div />").dxButton({
                                    text: "",
                                    type: "danger",
                                    icon: "trash",
                                    hint: "حذف",
                                    elementAttr: { "class": "btn btn-danger" },

                                    onClick: function (e) {
                                        $scope.UID = options.data.ID;
                                        swal({
                                            title: "تنبيه",
                                            text: "هل أنت متأكد من الحذف سوف يتم حذف الوثائق الخاصه بهذا الطالب",
                                            icon: "warning",
                                            buttons: [
                                                'إلغاء',
                                                'موافقة'
                                            ],
                                            dangerMode: true,
                                        }).then(function (isConfirm) {
                                            if (isConfirm) {
                                                $http({
                                                    method: "Post",
                                                    url: "/UniversityStudentsTracking/DeleteStudent",
                                                    data: { UniversityStudentID: $scope.UID }
                                                }).then(function (data) {
                                                    if (data.data === "") {
                                                        DevExpress.ui.notify({ message: "تم الحذف بنجاح" }, "success", 3000);
                                                        $scope.UID = null;
                                                        $scope.Refresh();
                                                    } else if (data.data !== "") {
                                                        DevExpress.ui.notify({ message: data.data }, "Error", 3000);
                                                    }
                                                });
                                                e.cancel = true;
                                            }
                                        })
                                    }
                                }).appendTo(container);
                            }
                        }],
  
                }
            }
        ]);
})();