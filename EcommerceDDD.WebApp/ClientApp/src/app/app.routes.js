"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.APP_ROUTES = void 0;
var home_component_1 = require("./modules/ecommerce/components/home/home.component");
var login_component_1 = require("./modules/authentication/components/login/login.component");
var auth_guard_1 = require("./core/guards/auth.guard");
var account_component_1 = require("./modules/authentication/components/account/account.component");
exports.APP_ROUTES = [
    {
        path: 'login',
        component: login_component_1.LoginComponent
    },
    {
        path: 'account',
        component: account_component_1.AccountComponent
    },
    {
        path: '**',
        component: home_component_1.HomeComponent,
        canActivate: [auth_guard_1.AuthGuard],
        pathMatch: 'full'
    }
];
//# sourceMappingURL=app.routes.js.map