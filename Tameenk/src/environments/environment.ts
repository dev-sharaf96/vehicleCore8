// This file can be replaced during build by using the `fileReplacements` array.
// `ng build ---prod` replaces `environment.ts` with `environment.prod.ts`.
// The list of file replacements can be found in `angular.json`.

export const environment = {
    production: false,
    inquiryApiUrl: '//www.bcare.com.sa/InquiryApi/api/',
    quotationApiUrl: '//www.bcare.com.sa/QuotationApi/api/',
    administrationApiUrl: '//www.bcare.com.sa/AdministrationApi/api/',
    QuotationSearchResult: '/Quotation/SearchResult?qtRqstExtrnlId=',
    identityUrl: '//www.bcare.com.sa/IdentityApi/api/',
    accessTokenUrl: '/home/GetAccessToken',
    googleCaptchaUrl: '/home/VerifyGoogleCaptcha',
    checkoutPath: '/ShoppingCart/AddItemToCart',
    googleCaptchaKey: '6LfiJHMUAAAAAHC1ypL30tqDeIIiLH_f_XK9-gsJ',
    termsAndConditionsFilePath: '\Companies-TermsAndConditions'
};


/*
 * In development mode, to ignore zone related error stack frames such as
 * `zone.run`, `zoneDelegate.invokeTask` for easier debugging, you can
 * import the following file, but please comment it out in production mode
 * because it will have performance impact when throw error
 */
// import 'zone.js/dist/zone-error';  // Included with Angular CLI.
