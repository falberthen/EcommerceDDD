"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.RestService = void 0;
var operators_1 = require("rxjs/operators");
var http_1 = require("@angular/common/http");
var common_1 = require("@angular/common");
var environment_1 = require("../../../../environments/environment");
var RestService = /** @class */ (function () {
    function RestService(http, baseUrl) {
        this.http = http;
        this.httpHeaders = {
            headers: new http_1.HttpHeaders({ 'Content-Type': 'application/json' })
        };
        this.apiBaseUrl = environment_1.environment.apiUrl;
    }
    RestService.prototype.get = function (relativeUrl, httpParams, responseTypeInput) {
        var fullUrl = common_1.Location.joinWithSlash(this.apiBaseUrl, relativeUrl);
        return this.http.get(fullUrl, { params: httpParams, responseType: responseTypeInput }).pipe(operators_1.map(function (response) {
            return response;
        }));
    };
    RestService.prototype.delete = function (relativeUrl, body) {
        var fullUrl = common_1.Location.joinWithSlash(this.apiBaseUrl, relativeUrl);
        return this.http.delete(fullUrl, body).pipe(operators_1.map(function (response) {
            return response;
        }));
    };
    RestService.prototype.post = function (relativeUrl, body, httpParams) {
        var fullUrl = common_1.Location.joinWithSlash(this.apiBaseUrl, relativeUrl);
        return this.http.post(fullUrl, body, this.httpHeaders).pipe(operators_1.map(function (response) {
            return response;
        }));
    };
    RestService.prototype.put = function (relativeUrl, body, httpParams) {
        var fullUrl = common_1.Location.joinWithSlash(this.apiBaseUrl, relativeUrl);
        return this.http.put(fullUrl, body, this.httpHeaders).pipe(operators_1.map(function (response) {
            return response;
        }));
    };
    RestService.prototype.patch = function (relativeUrl, body, httpParams) {
        var fullUrl = common_1.Location.joinWithSlash(this.apiBaseUrl, relativeUrl);
        return this.http.patch(fullUrl, body, this.httpHeaders).pipe(operators_1.map(function (response) {
            return response;
        }));
    };
    return RestService;
}());
exports.RestService = RestService;
//# sourceMappingURL=rest.service.js.map