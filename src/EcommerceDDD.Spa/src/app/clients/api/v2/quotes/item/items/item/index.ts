/* tslint:disable */
/* eslint-disable */
// Generated by Microsoft Kiota
// @ts-ignore
import { createProblemDetailsFromDiscriminatorValue, type ProblemDetails } from '../../../../../../models/index';
// @ts-ignore
import { type BaseRequestBuilder, type Parsable, type ParsableFactory, type RequestConfiguration, type RequestInformation, type RequestsMetadata } from '@microsoft/kiota-abstractions';

/**
 * Builds and executes requests for operations under /api/v2/quotes/{quoteId}/items/{productId}
 */
export interface WithProductItemRequestBuilder extends BaseRequestBuilder<WithProductItemRequestBuilder> {
    /**
     * @param requestConfiguration Configuration for the request such as headers, query parameters, and middleware options.
     * @returns {Promise<string>}
     * @throws {ProblemDetails} error when the service returns a 400 status code
     */
     delete(requestConfiguration?: RequestConfiguration<object> | undefined) : Promise<string | undefined>;
    /**
     * @param requestConfiguration Configuration for the request such as headers, query parameters, and middleware options.
     * @returns {RequestInformation}
     */
     toDeleteRequestInformation(requestConfiguration?: RequestConfiguration<object> | undefined) : RequestInformation;
}
/**
 * Uri template for the request builder.
 */
export const WithProductItemRequestBuilderUriTemplate = "{+baseurl}/api/v2/quotes/{quoteId}/items/{productId}";
/**
 * Metadata for all the requests in the request builder.
 */
export const WithProductItemRequestBuilderRequestsMetadata: RequestsMetadata = {
    delete: {
        uriTemplate: WithProductItemRequestBuilderUriTemplate,
        errorMappings: {
            400: createProblemDetailsFromDiscriminatorValue as ParsableFactory<Parsable>,
        },
        adapterMethodName: "sendPrimitive",
        responseBodyFactory:  "string",
    },
};
/* tslint:enable */
/* eslint-enable */
