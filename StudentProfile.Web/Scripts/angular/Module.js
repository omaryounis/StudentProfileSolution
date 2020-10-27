var app = angular.module("app", ["ngCookies", "dx", "ngLoadingSpinner"]);
app.config([
    "$httpProvider", function($httpProvider, $q) {
        $httpProvider.defaults.headers.common["X-Requested-With"] = 'XMLHttpRequest';
        $httpProvider.defaults.headers.common['Accept'] = 'application/json, text/javascript , */*';
        $httpProvider.defaults.headers.common['Content-Type'] = 'application/json; charset=utf-8';
        $httpProvider.defaults.headers.common['X-CSRF-TOKEN'] = $('meta[name="csrf-token"]').attr('content');
        $httpProvider.defaults.headers.post['Accept'] = 'application/json, text/javascript , */*';
        $httpProvider.defaults.headers.post['Content-Type'] = 'application/json; charset=utf-8';
        $httpProvider.defaults.headers.post['X-CSRF-TOKEN'] = $('meta[name="csrf-token"]').attr('content');
        $httpProvider.defaults.headers.put['Content-Type'] = 'application/json; charset=utf-8';
        $httpProvider.interceptors.push('my401Detector');
    }
]);

app.factory("my401Detector",
    [
        '$window', '$q', function($window, $q) {
            return {
                'request': function(request) {
                    return request;
                },
                'response': function(response) {
                    return response;
                },
                'responseError': function(response) {
                    if (response.status === 401) {
                        $window.location.href = '/Login/Login';
                    }
                    return $q.reject(response);
                }
            };
        }
    ]);