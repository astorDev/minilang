```http
POST {{internalApi}}/Email/Send

{
    "templateName" : "Email_Template_KYCRejected",
    "to": "{{email}}",
    "bodyParams" : {
        "Reason" : "You're bad guy",
        "company_name" : "Unlimited LTD"
    }
}
```