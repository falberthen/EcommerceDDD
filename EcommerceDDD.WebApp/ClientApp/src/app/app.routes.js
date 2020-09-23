"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.APP_ROUTES = void 0;
var home_component_1 = require("./modules/ecommerce/components/home/home.component");
var login_component_1 = require("./modules/authentication/components/login/login.component");
var auth_guard_1 = require("./core/guards/auth.guard");
var account_component_1 = require("./modules/authentication/components/account/account.component");
var product_selection_component_1 = require("./modules/ecommerce/components/product-selection/product-selection.component");
var orders_component_1 = require("./modules/ecommerce/components/orders/orders.component");
var customer_profile_component_1 = require("./modules/ecommerce/components/customer-profile/customer-profile.component");
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
        path: 'home',
        component: home_component_1.HomeComponent,
        canActivate: [auth_guard_1.AuthGuard],
        pathMatch: 'full'
    },
    {
        path: 'products',
        component: product_selection_component_1.ProductSelectionComponent,
        canActivate: [auth_guard_1.AuthGuard],
        pathMatch: 'full'
    },
    {
        path: 'products/:cartId',
        component: product_selection_component_1.ProductSelectionComponent,
        canActivate: [auth_guard_1.AuthGuard],
        pathMatch: 'full'
    },
    {
        path: 'orders',
        component: orders_component_1.OrdersComponent,
        canActivate: [auth_guard_1.AuthGuard],
        pathMatch: 'full'
    },
    {
        path: 'orders/:orderId',
        component: orders_component_1.OrdersComponent,
        canActivate: [auth_guard_1.AuthGuard],
        pathMatch: 'full'
    },
    {
        path: 'customer-profile',
        component: customer_profile_component_1.CustomerProfileComponent,
        canActivate: [auth_guard_1.AuthGuard],
        pathMatch: 'full'
    },
    {
        path: '**',
        component: login_component_1.LoginComponent,
        canActivate: [auth_guard_1.AuthGuard],
        pathMatch: 'full'
    }
];
//# sourceMappingURL=app.routes.js.map