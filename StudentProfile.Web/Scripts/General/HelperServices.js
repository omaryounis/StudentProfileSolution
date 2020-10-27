(function () {
    var HelperServices = function () {

        function Print(Title, Content) {
            debugger;
            var contents = $("#" + Content).html();
            var frame1 = $('<iframe />');
            frame1[0].name = "frame1";
            frame1.css({ "position": "absolute", "top": "-1000000px" });
            $("body").append(frame1);
            var frameDoc = frame1[0].contentWindow ? frame1[0].contentWindow : frame1[0].contentDocument.document ? frame1[0].contentDocument.document : frame1[0].contentDocument;
            frameDoc.document.open();
            //Create a new HTML document.
            frameDoc.document.write('<html><head><title>' + Title + '</title>');
            frameDoc.document.write('</head><body>');
            //Append the external CSS file.
            frameDoc.document.write('<link href="../../assets/vendors/bootstrap/dist/css/bootstrap.min.css" rel="stylesheet"><link href="../../assets/css/rtl.css" rel="stylesheet"><link href="../../assets/css/style.css" rel="stylesheet">');
            //Append the DIV contents.
            frameDoc.document.write(contents);
            frameDoc.document.write('</body></html>');
            frameDoc.document.close();
            setTimeout(function () {
                window.frames["frame1"].focus();
                window.frames["frame1"].print();
                frame1.remove();
            }, 500);
        }


        function ExportToExcel(Content) {
            debugger;
            $('#' + Content).css('direction', 'right');
            var html = document.getElementById(Content);
            html = html.outerHTML;
            if (html.length > 0) {
                var css = ('<style> ' +
                    'table {direction:rtl}' +
                    ' </style>');
                var tab_text = '\ufeff';
                tab_text = tab_text +
                    '<html xmlns:v="urn:schemas-microsoft-com:vml" xmlns:o="urn:schemas-microsoft-com:office:office" xmlns:x="urn:schemas-microsoft-com:office:excel" xmlns="http://www.w3.org/TR/REC-html40">';
                tab_text = tab_text + '<head>';
                tab_text = tab_text + '<meta http-equiv="Content-Type" content="text/html; charset=utf-8" />';
                tab_text = tab_text + '<meta name="ProgId" content="Excel.Sheet" />';
                tab_text = tab_text + '<meta name="Generator" content="Microsoft Excel 11" />';
                tab_text = tab_text + '<title>Sample</title>';
                tab_text = tab_text + '<!--[if gte mso 9]><xml><x:ExcelWorkbook><x:ExcelWorksheets><x:ExcelWorksheet>';
                tab_text = tab_text + '<x:Name>Sheet1</x:Name>';
                tab_text = tab_text + '<x:WorksheetOptions><x:Panes></x:Panes></x:WorksheetOptions></x:ExcelWorksheet>';
                tab_text = tab_text + '</x:ExcelWorksheets></x:ExcelWorkbook>';
                tab_text = tab_text + '</xml><![endif]--></head><body>';
                //tab_text = tab_text + '<div dir="rtl" width="100%" border="1px">';
                tab_text = tab_text + html;
                tab_text = tab_text + '</body></html>';
                var date = new Date();
                var SheetYear = date.getFullYear();
                var SheetMonth = (date.getMonth() + 1);
                var SheetDay = date.getDate();
                var fileName = 'Sheet_' + SheetDay + '-' + SheetMonth + '-' + SheetYear + '.xls';

                var blob = new Blob(['\ufeff', css + tab_text], { type: "application/vnd.ms-excel" });
                saveAs(blob, fileName);
            }

        }


        return {
            Print: Print,
            ExportToExcel: ExportToExcel
        };

    };

    var module = angular.module("app");
    module.factory("HelperServices", [HelperServices]);

}());
