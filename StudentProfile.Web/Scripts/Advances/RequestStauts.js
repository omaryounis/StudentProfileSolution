/// <reference path="../dx.all.debug.js" />
/// <reference path="../jquery-2.2.3.js" />
(function() {
    app.controller("RequestStautsCtrl",
        [
            "$scope", "$http", "$timeout", function($scope, $http, $timeout) {
                $scope.Permissions = {
                    Create: false,
                    Read: false,
                    Update: false,
                    Delete: false,
                    View: false
                };
                $http({
                    method: "GET",
                    url: "/Advances/GetAdvancePermissions",
                    params: { screenId: 27 } //شاشة إضافة طلب سلفة
                }).then(function(data) {
                    $scope.Permissions.Create = data.data.Create;
                    $scope.Permissions.Read = data.data.Read;
                    $scope.Permissions.Update = data.data.Update;
                    $scope.Permissions.Delete = data.data.Delete;
                    $scope.Permissions.View = data.data.View;
                    $scope.dataGridInstance.columnOption("add", "visible", data.data.Create);
                    $scope.dataGridInstance.columnOption("edit", "visible", data.data.Update);
                    $scope.dataGridInstance.columnOption("delete", "visible", data.data.Delete);
                });
                var requestsTypes = [{ Text: "سلفة", Value: "A" }, { Text: "اعانة", Value: "S" }];
                var reuqestsStatus = [
                    { Text: "تم الرفض", Value: "0" },
                    { Text: "تم الموافقة من مسؤولي الفرز", Value: "1" },
                    { Text: "تم الموافقة من مديري الفرز", Value: "2" },
                    { Text: "تم الموافقة من مديري الصرف", Value: "3" }
                ];
                $scope.Search = {
                    ValidationRules: {
                        AdvanceType: {
                            validationGroup: "addAdvance",
                            validationRules: [
                                {
                                    type: "required",
                                    message: "حقل اجبارى"
                                }
                            ]
                        },
                        AdvanceDueDate: {
                            validationGroup: "addAdvance",
                            validationRules: [
                                {
                                    type: "required",
                                    message: "حقل اجبارى"
                                }
                            ]
                        },
                        AdvanceDate: {
                            validationGroup: "addAdvance",
                            validationRules: [
                                {
                                    type: "required",
                                    message: "حقل اجبارى"
                                }
                            ]
                        },
                        AdvanceValue: {
                            validationGroup: "addAdvance",
                            validationRules: [
                                {
                                    type: "required",
                                    message: "حقل اجبارى"
                                }
                            ]
                        },
                    },
                    //نوع الطلب
                    RequestType:
                    {
                        bindingOptions: { value: "requestTypeId" },
                        dataSource: requestsTypes,
                        valueExpr: "Value",
                        displayExpr: "Text",
                        placeholder: "نوع الطلب",
                        showClearButton: true,
                        itemTemplate: function(data) {
                            return "<div title='" + data.Text + "' value='" + data.Value + "'>" + data.Text + "</div>";
                        }
                    },
                    //حالة الطلب
                    ReuqestStatus:
                    {
                        bindingOptions: { value: "reuqestStatusId" },
                        dataSource: reuqestsStatus,
                        valueExpr: "Value",
                        displayExpr: "Text",
                        placeholder: "حالة الطلب",
                        showClearButton: true,
                        itemTemplate: function(data) {
                            return "<div title='" + data.Text + "' value='" + data.Value + "'>" + data.Text + "</div>";
                        }
                    },
                    //الرقم الاكاديمى
                    StudentNumber:
                    {
                        bindingOptions: { value: "studentId" },
                        placeholder: "الرقم الاكاديمى",
                        showClearButton: true
                    },
                    SearchButtonOptions: {
                        text: "بحث",
                        hint: "بحث",
                        icon: "search",
                        type: "default",
                        useSubmitBehavior: true,
                        onClick: function(e) {
                            $scope.dataGridInstance.refresh();
                        }
                    }
                };
                var gridDataSource = new DevExpress.data.DataSource({
                    load: function(loadOptions) {
                        var d = $.Deferred();
                        $.getJSON("/Advances/RequestStautsSearch",
                            {
                                requestType: $scope.requestTypeId,
                                reuqestStatus: $scope.reuqestStatusId,
                                studentId: $scope.studentId,
                                skip: loadOptions.skip,
                                take: loadOptions.take,
                                sort: loadOptions.sort ? JSON.stringify(loadOptions.sort) : "",
                                filter: loadOptions.filter ? JSON.stringify(loadOptions.filter) : "",
                                requireTotalCount: loadOptions.requireTotalCount,
                                totalSummary: loadOptions.totalSummary ? JSON.stringify(loadOptions.totalSummary) : "",
                                group: loadOptions.group ? JSON.stringify(loadOptions.group) : "",
                                groupSummary: loadOptions.groupSummary ? JSON.stringify(loadOptions.groupSummary) : ""
                            }).done(function(result) {
                            d.resolve(result.data,
                                {
                                    //data: response.data.items,
                                    totalCount: result.totalCount,
                                    summary: result.summary
                                });
                        });
                        return d.promise();
                    },
                    map: function(itemData) {
                        debugger;
                        return {
                            AdvanceValue: itemData.AdvanceValue,
                            studentName: itemData.studentName,
                            studentId: itemData.studentId,
                            firstApproval: itemData.firstApproval,
                            secondApproval: itemData.secondApproval,
                            thirdApproval: itemData.thirdApproval,
                            type: itemData.type,
                            Date: new Date(parseInt(itemData.Date.substr(6)))
                        }
                    }
                });
                $scope.dataGridOptions = {
                    dataSource: gridDataSource,
                    noDataText: "لا يوجد بيانات",
                    columns: [
                        {
                            dataField: "Date",
                            caption: "تاريخ الطلب",
                            dataType: "date"
                        },
                        {
                            dataField: "AdvanceValue",
                            caption: "المبلغ"
                        },
                        {
                            dataField: "studentName",
                            caption: "اسم الطالب"
                        },
                        {
                            dataField: "studentId",
                            caption: "الرقم الجامعى"
                        },
                        {
                            dataField: "type",
                            caption: "نوع الطلب"
                        },
                        {
                            dataField: "firstApproval",
                            caption: "مسؤولي الفرز",
                            dataType: "boolean"
                        },
                        {
                            dataField: "secondApproval",
                            caption: "مديري الفرز",
                            dataType: "boolean"
                        },
                        {
                            dataField: "thirdApproval",
                            caption: "مديري الصرف",
                            dataType: "boolean"
                        }
                    ],
                    remoteOperations: {
                        filtering: true,
                        grouping: true,
                        groupPaging: true,
                        paging: true,
                        sorting: true,
                        summary: true
                    },
                    headerFilter: {
                        allowSearch: true,
                        texts: { cancel: "الغاء", emptyValue: "", ok: "Ok" },
                        visible: true
                    },
                    loadPanel: {
                        enabled: "auto",
                        indicatorSrc: "",
                        showIndicator: true,
                        showPane: true,
                        text: "تحميل..."
                    },
                    searchPanel: {
                        visible: true,
                        placeholder: "بحث"
                    },
                    selection: { mode: "single" },
                    showRowLines: true,
                    columnAutoWidth: true,
                    scrolling: {
                        rtlEnabled: true,
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
                        infoText: "صفحة {0} من {1} - {2} عنصر",
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
                    onInitialized: function(e) {
                        $scope.dataGridInstance = e.component;
                    }
                };
                $scope.onFormSubmit = function(e) {
                }
            }
        ]);
})();