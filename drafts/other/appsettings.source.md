```csharp
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft": "Warning",
      "Microsoft.Hosting.Lifetime": "Information",
      "Yarp": "Warning"
    }
  },
  "FIXSettingsFSRA": {
    "CounterParty": "FSRA",
    "Connection": "main",
    "SenderCompID": "ABXXDEV1",
    "TargetCompID": "FMSSTEST",
    "IPAddress": "192.168.117.32",
    "Port": "56001",
    "FixVersion": "Fix50Sp2",
    "HeartBeatInterval": 30,
    "Type": "initiator"
  },
  "AuditLogService": {
    "AuditIndex": "audit_log"
  },
  "AllowedHosts": "*",
  "ReverseProxy": {
    "Routes": {
      "routeCard": {
        "ClusterId": "adminPaymentActions",
        "Match": {
          "Path": "/AdminPanel/PaymentRequisite/{**catch-all}"
        },
        "Transforms": [
          {
            "ResponseHeader": "Source",
            "Append": "YARP",
            "When": "Success"
          }
        ]
      },
      "routeBank": {
        "ClusterId": "adminPaymentActions",
        "Match": {
          "Path": "/AdminPanel/BankPaymentRequisites/{**catch-all}"
        },
        "Transforms": [
          {
            "ResponseHeader": "Source",
            "Append": "YARP",
            "When": "Success"
          }
        ]
      }
    },
    "Clusters": {
      "adminPaymentActions": {
        "Swagger": {
          "Endpoint": "/swagger/paymentService/v1/swagger.json",
          "Spec": "http://pay:7197/swagger/v1/swagger.json",
          "TargetPaths": [
            "/AdminPanel"
          ],
          "OriginPaths": [
            "/AdminPanel"
          ]
        },
        "Destinations": {
          "destination1": {
            "Address": "http://pay:7197/"
          }
        }
      }
    }
  }
}
```