(function () {
    app.controller("UploadBankFiles", ["$scope", "$http", "$timeout", function ($scope, $http, $timeout) {

        /*********************************** Permissions *********************************/
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
                url: "/Rewards/GetPermissions",
                params: { screenId: 92 }
            }).then(function (data) {
                $scope.Permissions.Create = data.data.CreateReward;
                $scope.Permissions.Read = data.data.ReadReward;
                $scope.Permissions.Update = data.data.UpdateReward;
                $scope.Permissions.Delete = data.data.DeleteReward;
                $scope.Permissions.View = data.data.ViewReward;
            });
        };
        $scope.GetPermssions();
        /*--------------------------------* Permissions *--------------------------------*/

        //File Uploading

        $scope.multiple = false;
        $scope.accept = ".txt";  //image/*,.pdf,.doc,.docx,.xls,.xlsx
        $scope.uploadMode = "useButtons";
        $scope.AcceptedFilesvalue = [];

        $scope.AcceptedFilesOptions = {
            name: "AcceptedFileList",
            uploadUrl: "/Rewards/UploadAcceptedFiles",
            allowCanceling: true,
            rtlEnabled: true,
            readyToUploadMessage: "استعداد للرفع",
            selectButtonText: "اختر الملف",
            labelText: "",
            uploadButtonText: "رفع",
            uploadedMessage: "تم الرفع",
            invalidFileExtensionMessage: "نوع الملف غير مسموح",
            uploadFailedMessage: "خطأ أثناء الرفع",
            onInitialized: function (e) {
                $scope.AcceptedFilesInstance = e.component;
            },
            onUploaded: function (e) {
                debugger;
                var xhttp = e.request;
                if (xhttp.status === 200) {
                    DevExpress.ui.notify({ message: "تم رفع ملف البنك بنجاح" }, "success", 10000);
                    $scope.AcceptedFilesvalue = '';
                    $scope.AcceptedFilesInstance.reset();
                } else if (xhttp.status === 404) {
                    DevExpress.ui.notify({ message: "الملف المرفوع لايحتوي على بيانات" }, "Error", 10000);
                    $scope.AcceptedFilesvalue = '';
                    $scope.AcceptedFilesInstance.reset();

                } else {
                    DevExpress.ui.notify({ message: "حدث خطأ أثناء رفع الملف" }, "Error", 10000);
                    $scope.AcceptedFilesvalue = '';
                    $scope.AcceptedFilesInstance.reset();
                }
            },
            onUploadError: function (e) {
                debugger;
                var xhttp = e.request;
                if (xhttp.status === 500) {
                    DevExpress.ui.notify({ message: "حدث خطأ أثناء رفع الملف" }, "Error", 10000);
                    $scope.AcceptedFilesvalue = '';
                    $scope.AcceptedFilesInstance.reset();
                }
            },
            bindingOptions: {
                multiple: "multiple",
                accept: "accept",
                value: "AcceptedFilesvalue",
                uploadMode: "uploadMode"
            }
        };
        //Remove File
        $scope.RemoveAcceptedFile = function (hashkey) {
            var nametoRemove = "";
            angular.forEach($scope.AcceptedFilesvalue,
                function (file, indx) {
                    if (file.$$hashKey === hashkey) {
                        nametoRemove = file.name;
                        $scope.AcceptedFilesvalue.splice(indx, 1);
                    }
                });
        };
    }]);
})();