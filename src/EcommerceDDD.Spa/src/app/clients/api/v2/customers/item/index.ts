/* tslint:disable */
/* eslint-disable */
// Generated by Microsoft Kiota
// @ts-ignore
import { CreditRequestBuilderRequestsMetadata, type CreditRequestBuilder } from './credit/index';
// @ts-ignore
import { DetailsRequestBuilderRequestsMetadata, type DetailsRequestBuilder } from './details/index';
// @ts-ignore
import { type BaseRequestBuilder, type KeysToExcludeForNavigationMetadata, type NavigationMetadata } from '@microsoft/kiota-abstractions';

/**
 * Builds and executes requests for operations under /api/v2/customers/{customerId}
 */
export interface WithCustomerItemRequestBuilder extends BaseRequestBuilder<WithCustomerItemRequestBuilder> {
    /**
     * The credit property
     */
    get credit(): CreditRequestBuilder;
    /**
     * The details property
     */
    get details(): DetailsRequestBuilder;
}
/**
 * Uri template for the request builder.
 */
export const WithCustomerItemRequestBuilderUriTemplate = "{+baseurl}/api/v2/customers/{customerId}";
/**
 * Metadata for all the navigation properties in the request builder.
 */
export const WithCustomerItemRequestBuilderNavigationMetadata: Record<Exclude<keyof WithCustomerItemRequestBuilder, KeysToExcludeForNavigationMetadata>, NavigationMetadata> = {
    credit: {
        requestsMetadata: CreditRequestBuilderRequestsMetadata,
    },
    details: {
        requestsMetadata: DetailsRequestBuilderRequestsMetadata,
    },
};
/* tslint:enable */
/* eslint-enable */
