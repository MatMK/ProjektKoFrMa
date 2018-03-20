"use strict";
var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
var __metadata = (this && this.__metadata) || function (k, v) {
    if (typeof Reflect === "object" && typeof Reflect.metadata === "function") return Reflect.metadata(k, v);
};
Object.defineProperty(exports, "__esModule", { value: true });
var core_1 = require("@angular/core");
var http_1 = require("@angular/http");
var AppComponent = /** @class */ (function () {
    function AppComponent(http) {
        this.http = http;
        this.name = 'KoFrMa-AdminApp';
        this.token = "";
    }
    AppComponent.prototype.Login = function () {
        var _this = this;
        var url = "http://localhost:50576/api/AdminApp/RegisterToken";
        this.http.post(url, { "UserName": "Pepa", "Password": 123456 }).toPromise()
            .then(function (res) { return _this.token = res.json(); })
            .catch(function (msg) { return console.log('Error: ' + msg.status + ' ' + msg.statusText); });
    };
    AppComponent = __decorate([
        core_1.Component({
            selector: 'my-app',
            template: "<h1 (click)=\"Login()\">Welcome to {{token}}</h1>",
        }),
        __metadata("design:paramtypes", [http_1.Http])
    ], AppComponent);
    return AppComponent;
}());
exports.AppComponent = AppComponent;
//# sourceMappingURL=app.component.js.map