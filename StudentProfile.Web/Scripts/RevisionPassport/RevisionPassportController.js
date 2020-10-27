app.controller("RevisionPassportCtrl", ["$scope", 'RevisionPassportSrvc', function ($scope, RevisionPassportSrvc) {

    //Filed
    $scope.RevisionPassportList = [];
    $scope.UnderRevisionPassportList = [];
    $scope.RevisionPassportUser = [];
    $scope.gridSelectedRowKeys = [];
    $scope.IsApproved = false;

    $scope.CurrentUniversityStudentId = null;

    $scope.PopUpDocumentsShow = false;
    $scope.popuResonOfRefuseShow = false;
    $scope.popuResonOfRefuseIdentityShow = false;
    $scope.CurrentradioPassport = null;
    $scope.CurrentradioIdentity = null;
    //btn Save
    $scope.btnSavePassport = {
        text: 'اعتماد',
        type: 'success',
        onClick: function (e) {
            debugger;
            var radioPassport = $("input[name='Passpor_2']:checked").val();
            if (radioPassport) {
                RevisionPassportSrvc.ActiveDocument(Number(radioPassport)).then(function (data) {
                    if (data.data.status === 500) {
                        DevExpress.ui.notify({ message: data.data.Message, type: data.data.Type, displayTime: 3000, closeOnClick: true });
                    }
                    if (data.data.status === 200) {
                        DevExpress.ui.notify({ message: data.data.Message, type: data.data.Type, displayTime: 3000, closeOnClick: true });
                    }
                    //GetStudentsDocuments
                    RevisionPassportSrvc.GetStudentsDocuments($scope.IsApproved).then(function (data) {
                        if ($scope.IsApproved == true) {

                            $scope.RevisionPassportList = data.data;
                        } else {

                            $scope.UnderRevisionPassportList = data.data;
                        }
                    });
                    RevisionPassportSrvc.GetRevisionPassportByUser($scope.CurrentUniversityStudentId, $scope.IsApproved).then(function (data) {
                        $scope.RevisionPassportUser = data.data;
                    });
                });
            } else {
                DevExpress.ui.notify({ message: 'اختر مستند للحفظ', type: 'error', displayTime: 3000, closeOnClick: true });
            }
        }
    };

    //btn Refuse
    $scope.RefusePassportNotes = "";
    $scope.btnOpenRefusePassportPopup = {
        text: 'رفض',
        type: 'danger',
        onClick: function (e) {
            debugger;
            $scope.CurrentradioPassport = $("input[name='Passpor_2']:checked").val();
            //var radioPassport = $("input[name='Passpor_2']:checked").val();
            if ($scope.CurrentradioPassport) {
                $scope.popuResonOfRefuseShow = true;
            } else {
                DevExpress.ui.notify({ message: 'اختر مستند للرفض', type: 'error', displayTime: 3000, closeOnClick: true });
            }
        }
    };

    $scope.btnOpenRefuseIdentityPopup = {
        text: 'رفض',
        type: 'danger',
        onClick: function (e) {
            debugger;
            //var radioIdentity = $("input[name='Identity_1']:checked").val();
            $scope.CurrentradioIdentity = $("input[name='Identity_1']:checked").val();
            if ($scope.CurrentradioIdentity) {
                $scope.popuResonOfRefuseIdentityShow = true;
                console.log($scope.CurrentradioIdentity);
            } else {
                DevExpress.ui.notify({ message: 'اختر مستند للرفض', type: 'error', displayTime: 3000, closeOnClick: true });
            }
        }
    };

    $scope.RefuseNotesTextArea = {
        height: 90,
        bindingOptions: {
            value: "RefusePassportNotes",
            onValueChanged: function (e) {
                $scope.RefusePassportNotes = e.value;
            }
        }
    };

    $scope.btnRefusePassport = {
        text: 'حفظ',
        type: 'success',
        onClick: function (e) {
            debugger;
            console.log($scope.RefusePassportNotes);
            console.log($scope.CurrentradioPassport);
            //
            if ($scope.CurrentradioPassport) {
                RevisionPassportSrvc.RefusedDocuments({ studentDocumentId: Number($scope.CurrentradioPassport), refusedNotes: $scope.RefusePassportNotes }).then(function (data) {
                    if (data.data.status === 500) {
                        DevExpress.ui.notify({ message: data.data.Message, type: data.data.Type, displayTime: 3000, closeOnClick: true });
                    }
                    if (data.data.status === 200) {
                        DevExpress.ui.notify({ message: data.data.Message, type: data.data.Type, displayTime: 3000, closeOnClick: true });
                        $scope.RefusePassportNotes = "";
                        $scope.popuResonOfRefuseShow = false;
                    }

                });
            }
        }
    };

    $scope.btnRefuseIdentity = {
        text: 'حفظ',
        type: 'success',
        onClick: function (e) {
            debugger;
            //$scope.CurrentradioIdentity = $("input[name='Identity_1']:checked").val();
            console.log($scope.RefusePassportNotes);
            console.log($scope.CurrentradioIdentity);
            //
            if ($scope.CurrentradioIdentity) {
                RevisionPassportSrvc.RefusedDocuments({ studentDocumentId: Number($scope.CurrentradioIdentity), refusedNotes: $scope.RefusePassportNotes }).then(function (data) {
                    if (data.data.status === 500) {
                        DevExpress.ui.notify({ message: data.data.Message, type: data.data.Type, displayTime: 3000, closeOnClick: true });
                    }
                    if (data.data.status === 200) {
                        DevExpress.ui.notify({ message: data.data.Message, type: data.data.Type, displayTime: 3000, closeOnClick: true });
                        $scope.RefusePassportNotes = "";
                        $scope.popuResonOfRefuseIdentityShow = false;
                    }

                });
            }
        }
    };

    



    $scope.btnSaveIdentity = {
        text: 'اعتماد',
        type: 'success',
        onClick: function (e) {
            var radioIdentity = $("input[name='Identity_1']:checked").val();
            if (radioIdentity) {
                RevisionPassportSrvc.ActiveDocument(Number(radioIdentity)).then(function (data) {
                    if (data.data.status === 500) {
                        DevExpress.ui.notify({ message: data.data.Message, type: data.data.Type, displayTime: 3000, closeOnClick: true });
                    }
                    if (data.data.status === 200) {
                        DevExpress.ui.notify({ message: data.data.Message, type: data.data.Type, displayTime: 3000, closeOnClick: true });
                    }
                    //GetStudentsDocuments
                    RevisionPassportSrvc.GetStudentsDocuments($scope.IsApproved).then(function (data) {
                        if ($scope.IsApproved == true) {

                            $scope.RevisionPassportList = data.data;
                        } else {

                            $scope.UnderRevisionPassportList = data.data;
                        }
                    });
                    RevisionPassportSrvc.GetRevisionPassportByUser($scope.CurrentUniversityStudentId, $scope.IsApproved).then(function (data) {
                        $scope.RevisionPassportUser = data.data;
                    });
                });
            } else {
                DevExpress.ui.notify({ message: 'اختر مستند للحفظ', type: 'error', displayTime: 3000, closeOnClick: true });
            }
        }
    };

    $scope.btnSaveVisa = {
        text: 'اعتماد',
        type: 'success',
        onClick: function (e) {
            var radioVisa = $("input[name='Visa_3']:checked").val();
            if (radioVisa) {
                RevisionPassportSrvc.ActiveDocument(Number(radioVisa)).then(function (data) {
                    if (data.data.status === 500) {
                        DevExpress.ui.notify({ message: data.data.Message, type: data.data.Type, displayTime: 3000, closeOnClick: true });
                    }
                    if (data.data.status === 200) {
                        DevExpress.ui.notify({ message: data.data.Message, type: data.data.Type, displayTime: 3000, closeOnClick: true });
                    }
                    //GetStudentsDocuments
                    RevisionPassportSrvc.GetStudentsDocuments($scope.IsApproved).then(function (data) {
                         
                        if ($scope.IsApproved == true) {

                            $scope.RevisionPassportList = data.data;
                        } else {

                            $scope.UnderRevisionPassportList = data.data;
                        }
                    });
                    RevisionPassportSrvc.GetRevisionPassportByUser($scope.CurrentUniversityStudentId, $scope.IsApproved).then(function (data) {
                        $scope.RevisionPassportUser = data.data;
                    });
                });
            } else {
                DevExpress.ui.notify({ message: 'اختر مستند للحفظ', type: 'error', displayTime: 3000, closeOnClick: true });
            }
        }
    };

    $scope.removeDocument = function (StudentDocumentID) {

        swal({
            title: "تنبيه",
            text: "هل أنت متأكد من الحذف",
            icon: "warning",
            buttons: [
                'إلغاء',
                'موافقة'
            ],
            dangerMode: true,
        }).then(function (isConfirm) {
            if (isConfirm) {
                RevisionPassportSrvc.RemoveDocument(StudentDocumentID).then(function (data) {
                    if (data.data.status === 500) {
                        DevExpress.ui.notify({
                            message: data.data.Message,
                            type: data.data.Type,
                            displayTime: 3000,
                            closeOnClick: true
                        });
                    }
                    if (data.data.status === 200) {
                        DevExpress.ui.notify({
                            message: data.data.Message,
                            type: data.data.Type,
                            displayTime: 3000,
                            closeOnClick: true
                        });
                    }
                    RevisionPassportSrvc.GetRevisionPassportByUser($scope.CurrentUniversityStudentId, $scope.IsApproved).then(function (data) {

                        $scope.RevisionPassportUser = data.data;
                    });
                });
            } else {
            }
        })

    }
    //Popup Documents
    $scope.PopUpDocuments = {
        width: 1000,
        height: 700,
        contentTemplate: "info",
        title: "وثائق الطلاب",
        showTitle: true,
        dragEnabled: false,
        bindingOptions: {
            visible: "PopUpDocumentsShow"
        },
        rtlEnabled: true,
        onHiding: function (e) {
            $scope.CurrentUniversityStudentId = null;
        }
    };
    //poppup refuse passport
    $scope.popuResonOfRefuse = {
        width: 850,
        height: 700,
        contentTemplate: "info",
        title: "سبب الرفض",
        showTitle: true,
        dragEnabled: false,
        bindingOptions: {
            visible: "popuResonOfRefuseShow"
        },
        rtlEnabled: true,
        onHiding: function (e) {
            $scope.CurrentradioPassport = null;
            $scope.RefusePassportNotes = "";
        }
    };

    //poppup refuse identity
    $scope.popuResonOfRefuseIdentity = {
        width: 850,
        height: 700,
        contentTemplate: "info",
        title: "سبب الرفض",
        showTitle: true,
        dragEnabled: false,
        bindingOptions: {
            visible: "popuResonOfRefuseIdentityShow"
        },
        rtlEnabled: true,
        onHiding: function (e) {
            $scope.CurrentradioIdentity = null;
            $scope.RefusePassportNotes = "";
        }
    };
    
    $scope.btnApproved = {
        text: 'الوثائق المعتمدة',
        type: 'primary',
        onClick: function (e) {
            $scope.IsApproved = true;
            RevisionPassportSrvc.GetStudentsDocuments($scope.IsApproved).then(function (data) {
                $scope.RevisionPassportList = data.data;
            });
        }
    };

    $scope.btnNotApproved = {
        text: 'وثائق تحت المراجعة',
        type: 'primary',
        onClick: function (e) {
            debugger;
            $scope.IsApproved = false;
            RevisionPassportSrvc.GetStudentsDocuments($scope.IsApproved).then(function (data) {
                $scope.UnderRevisionPassportList = data.data;
            });
        }
    };

   
    //dataGrid
    $scope.gridRevisionPassport = {
        bindingOptions: {
            dataSource: "RevisionPassportList"
        },

        noDataText: "لا يوجد بيانات",
        selection: {
            fixed: false,
            mode: "multiple",
            showCheckBoxesMode: "always",
            selectAllMode: "allPages",
            width: "40px"
        }, "export": {
            enabled: true,
            fileName: "الوثائق المعتمده"
        }, sorting: {
            mode: "multiple"
        },
        showBorders: true,
        paging: {
            pageSize:5
        },
        pager: { 
            infoText: "(صفحة  {0} من {1} ({2} عنصر",
            showInfo: true,
            showNavigationButtons: true,
            showPageSizeSelector: true,
            visible: true,
            allowedPageSizes: [5, 10, 20]
        },
        headerFilter: {
            visible: true
        },
        showRowLines: true,
        groupPanel: {
            visible: false,
            emptyPanelText: "اسحب عمود هنا"
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
            enabled: true
        },
        columns: [
            {
                caption: "اسم الطالب",
                dataField: "NamePerIdentity_Ar",
                cssClass: "text-right"
            },
            {
                caption: "الرقم الجامعي",
                dataField: "Student_ID",
                cssClass: "text-right"
            },
            {
                caption: "رقم الهوية",
                dataField: "National_ID",
                cssClass: "text-right"
            },
            {
                caption: "رقم الجوال",
                dataField: "MobileNumber",
                cssClass: "text-right"
            },
            {
                caption: "الجنسيه",
                dataField: "Nationality",
                cssClass: "text-right"
            },
            {
                caption: "البريد الإلكتروني",
                dataField: "UniversityEmail",
                cssClass: "text-right"
            },
            {
                caption: "تاريخ الاضافه",
                dataField: "InsertDate",
                cssClass: "text-right",
                //dataType:"date"
            },
            
            {
                caption: "معاينة الوثائق",
                width: 150,
                cssClass: "text-center",
                cellTemplate: function (container, options) {
                    $("<div />").dxButton({
                        //icon: "fa fa-save",
                        text: "معاينة",
                        type: "primary",
                        hint: "معاينة",
                        elementAttr: { "class": "btn btn-primary" },

                        onClick: function (e) {
                            $scope.CurrentUniversityStudentId = options.data.UniversityStudent_ID;
                            RevisionPassportSrvc.GetRevisionPassportByUser(options.data.UniversityStudent_ID, $scope.IsApproved).then(function (data) {
                                $scope.PopUpDocumentsShow = true;

                                $scope.RevisionPassportUser = data.data;
                            });
                        }
                    }).appendTo(container);
                }
            }
        ],
        summary: {
            totalItems: [{
                column: 'Student_ID',
                summaryType: 'count',
                displayFormat: "العدد: {0}",
            }
                // ...
            ]
        },
        onContentReady: function (e) {
            e.component.columnOption("command:edit", {
                visibleIndex: -1
            });
        },
        onSelectionChanged: function (info) {
            debugger;
            $scope.gridSelectedRowKeys = [];
            for (var i = 0; i < info.selectedRowKeys.length; i++)
                $scope.gridSelectedRowKeys.push(info.selectedRowKeys[i].UniversityStudent_ID);
        },
        onInitialized: function (e) {
            debugger;
            $scope.IsApproved = true;
            RevisionPassportSrvc.GetStudentsDocuments($scope.IsApproved).then(function (data) {
                $scope.RevisionPassportList = data.data;
            });
        }
    };

    $scope.gridUnderRevisionPassport = {
        bindingOptions: {
            dataSource: "UnderRevisionPassportList"
        },
        width: "100%",
        height: "auto",
        noDataText: "لا يوجد بيانات",
        selection: {
            fixed: false,
            mode: "multiple",
            showCheckBoxesMode: "always",
            selectAllMode: "allPages",
            width: "40px"
        }, "export": {
            enabled: true,
            fileName:"وثائق تحت المراجعه"
        }, sorting: {
            mode: "multiple"
        },
        showBorders: true,
        paging: {
            pageSize: 5
        },
        pager: {

            infoText: "(صفحة  {0} من {1} ({2} عنصر",
            showInfo: true,
            showNavigationButtons: true,
            showPageSizeSelector: true,
            visible: true,
            allowedPageSizes: [5, 10, 20]
        },
        headerFilter: {
            visible: true
        },
        showRowLines: true,
        groupPanel: {
            visible: false,
            emptyPanelText: "اسحب عمود هنا"
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
                useNative: false,
                //scrollByContent: true,
                //scrollByThumb: true,
                showScrollbar: "always",
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
            enabled: true
        },
        columns: [
            {
                caption: "اسم الطالب",
                dataField: "NamePerIdentity_Ar",
                cssClass: "text-right",
                columnAutoWidth:true
            },
            {
                caption: "الرقم الجامعي",
                dataField: "Student_ID",
                cssClass: "text-right",
                columnAutoWidth:true
            },
            {
                caption: "رقم الهوية",
                dataField: "National_ID",
                cssClass: "text-right",
                columnAutoWidth:true
            },
            {
                caption: "رقم الجوال",
                dataField: "MobileNumber",
                cssClass: "text-right",
                columnAutoWidth:true
            },
            {
                caption: "الجنسيه",
                dataField: "Nationality",
                cssClass: "text-right",
                columnAutoWidth:true
            },
            {
                caption: "البريد الإلكتروني",
                dataField: "UniversityEmail",
                cssClass: "text-right",
                columnAutoWidth:true
            },
            {
                caption: "تاريخ الاضافه",
                dataField: "InsertDate",
                cssClass: "text-right",
                columnAutoWidth:true
                //dataType:"date"
            },
            {
                caption: "معاينة الوثائق", 
                cssClass: "text-center",
                columnAutoWidth:true,
                cellTemplate: function (container, options) {
                    $("<div />").dxButton({
                        //icon: "fa fa-save",
                        text: "معاينة",
                        type: "primary",
                        hint: "معاينة",
                        elementAttr: { "class": "btn btn-primary" },

                        onClick: function (e) {
                            $scope.CurrentUniversityStudentId = options.data.UniversityStudent_ID;
                            RevisionPassportSrvc.GetRevisionPassportByUser(options.data.UniversityStudent_ID, $scope.IsApproved).then(function (data) {
                                $scope.PopUpDocumentsShow = true;
                                debugger;
                                $scope.RevisionPassportUser = data.data;
                            });
                        }
                    }).appendTo(container);
                }
            }
        ],
        summary: {
            totalItems: [{
                column: 'Student_ID',
                summaryType: 'count',
                displayFormat: "العدد: {0}",
            }
                // ...
            ]
        },
        onContentReady: function (e) {
            e.component.columnOption("command:edit", {
                visibleIndex: -1
            });
        },
        onSelectionChanged: function (info) {
            debugger;
            $scope.gridSelectedRowKeys = [];
            for (var i = 0; i < info.selectedRowKeys.length; i++)
                $scope.gridSelectedRowKeys.push(info.selectedRowKeys[i].UniversityStudent_ID);
        },
        onInitialized: function (e) {
            debugger;
            $scope.IsApproved = false;
            RevisionPassportSrvc.GetStudentsDocuments($scope.IsApproved).then(function (data) {
                $scope.UnderRevisionPassportList = data.data;
            });
        }
    };
    $scope.PreviewImage = function (id) {
        RevisionPassportSrvc.PreviewImage(id).then(function (data) {
            //var url = URL.createObjectURL(data.data);
            var image = new Image();
            image.src = "data:image/jpeg;base64," +data.data;
            var w = window.open("");
            w.document.write(image.outerHTML);
        });

    };
    $scope.btnApprovedAll = {
        text: 'اعتماد المحدد',
        type: 'success',
        onClick: function (e) {
            debugger;
            if ($scope.gridSelectedRowKeys.length === 0)
                return DevExpress.ui.notify({ message: 'اختر طلاب للموافقة', type: 'error', displayTime: 3000, closeOnClick: true });

            RevisionPassportSrvc.ApprovedDocuments({ UniversityStudentIds: $scope.gridSelectedRowKeys.join(',') }).then(function (data) {
                if (data.data.status === 500) {
                    DevExpress.ui.notify({ message: data.data.Message, type: data.data.Type, displayTime: 3000, closeOnClick: true });
                }
                if (data.data.status === 200) {
                    DevExpress.ui.notify({ message: data.data.Message, type: data.data.Type, displayTime: 3000, closeOnClick: true });
                }
                //GetStudentsDocuments
                if ($scope.IsApproved == true) {
                    RevisionPassportSrvc.GetStudentsDocuments($scope.IsApproved).then(function (data) {
                        $scope.RevisionPassportList = data.data;
                    });
                } else {
                    RevisionPassportSrvc.GetStudentsDocuments($scope.IsApproved).then(function (data) {
                        $scope.UnderRevisionPassportList = data.data;
                    });
                }
                
            });
        }
    };


}]);
