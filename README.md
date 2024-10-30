```markdown
# Electricity Bill Payment System - Microservices

This project provides a backend service for an electricity bill vending and payment system, designed with an event-driven architecture. The project is built using ASP.NET Core and follows modern backend principles with RESTful API design, dependency injection, and event handling through Azure Service Bus.

## Project Structure

- **Controllers**: Defines the API endpoints for creating bills, managing user wallets, and processing payments.
- **Services**: Contains business logic for billing, user management, and wallet transactions.
- **Event Handlers**: Listens for and responds to events like bill creation and successful payment completion.
- **Models**: Defines data models for bills, users, and wallets.
- **Integrations**: Manages external integrations (e.g., SMS notifications, Azure Service Bus).

## Getting Started

### Prerequisites

- [.NET SDK](https://dotnet.microsoft.com/download) (version 8 or above)
- [SQL Server](https://www.microsoft.com/en-us/sql-server/sql-server-downloads) (for database setup)
- [Azure Service Bus](https://azure.microsoft.com/en-us/services/service-bus/) or LocalStack (for event-driven architecture and messaging)
- [Twilio, vonage, or similar](https://www.twilio.com/) (for SMS notifications)

### Setup Instructions

#### 1. Clone the Repository

```bash
git clone <repository-url>
cd ElectricityBillPaymentSystem
```

#### 2. Configure Environment Variables

Create a `.env` file or use `appsettings.json` to set your database and Azure Service Bus connection strings.

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=your_server;Database=your_db;User Id=your_user;Password=your_password;"
  },
  "AzureServiceBus": {
    "ConnectionString": "Endpoint=sb://your-service-bus.servicebus.windows.net/;SharedAccessKeyName=your_key_name;SharedAccessKey=your_key"
  },
  "SMS": {
    "Provider": "Twilio",
    "TwilioAccountSid": "your_account_sid",
    "TwilioAuthToken": "your_auth_token",
    "TwilioPhoneNumber": "your_phone_number"
  }
}
```

#### 3. Install Dependencies

```bash
dotnet restore
```

#### 4. Database Setup

Run migrations to set up the initial database schema.

```bash
dotnet ef database update
```

#### 5. Run the Application

Start the application locally:

```bash
dotnet run
```

The API is now accessible at `http://localhost:5000`.

## API Endpoints

- **POST /api/electricity/verify** - Creates a new electricity bill
- **POST /api/Vend/{validationRef}/pay** - Processes payment for an electricity bill
- **POST /api/wallet/{id}/add-funds** - Adds funds to a user’s wallet
- **POST /api/createUser** - To create a user
- **GET /api/getAllUser** - To get all users

## Event-Driven Components

Events are published to Azure Service Bus at the following stages:

- **Bill Created** - Upon bill verification, a `bill_created` event is published.
- **Payment Completed** - On successful payment, a `payment_completed` event is published.
- **User Events** - On create a user, a user-event is published

### Event Handling

The application listens for wallet-related events such as `fund_added`, user-events and processes them asynchronously to keep the wallet balance up-to-date.

## SMS Notifications

The application sends SMS notifications at key points:

1. **Payment Successful**: Notifies the user upon successful payment and also send bill token.
2. **Create user**: Notifies the user upon create an account and wallet.
3. **Add Fund**: Sends credit alert message

### Configuring SMS Notifications

In `appsettings.json` or `.env`, set up your preferred SMS provider (Twilio, Nexmo, etc.), and input the necessary credentials.

## Testing

You can test the API endpoints using Postman or cURL. Use mock services or LocalStack to simulate Azure Service Bus events if you don't have a live Azure instance.
