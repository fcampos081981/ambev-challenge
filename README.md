# Order Service API (.NET C#)

## Structure of API Order Service to Handle 150M ~ 200M Requests per Day

```
/OrderService/
├── OrderService.sln
│   ├── /OrderService.Api/
│   │   ├── Controllers/
│   │   │   └── OrdersController.cs
│   │   ├── Dtos/
│   │   │   ├── Dtos.cs
│   │   ├── Services/
│   │   │   └── OrderConsumerService.cs
│   │   ├── Program.cs
│   │   ├── appsettings.json
│   │   └── OrderService.Api.csproj
│   │
│   ├── /OrderService.Application/
│   │   ├── Interfaces/
│   │   │   ├── IOrderRepository.cs
│   │   │   └── IKafkaProducer.cs
│   │   ├── Services/
│   │   │   └── OrderProcessingService.cs
│   │   └── OrderService.Application.csproj
│   │
│   ├── /OrderService.Domain/
│   │   ├── Entities/
│   │   │   ├── Order.cs
│   │   │   ├── OrderItem.cs
|   │   ├── Enums/
│   │   │   └── OrderStatus.cs  
│   │   └── OrderService.Domain.csproj
│   │
│   └── /OrderService.Infrastructure/
│       ├── Data/
│       │   ├── OrderDbContext.cs
|       |   └── Migrations
│       │       └── 20250713172215_OrderService.cs
│       │       └── OrderDbContextModelSnapshot.cs
│       │   └── Repositories/
│       │       └── OrderRepository.cs
│       ├── Messaging/
│       │   └── KafkaProducer.cs
│       └── OrderService.Infrastructure.csproj
│
└── docker-compose.yml
```

---
## Running applicatio in Docker

### Docker official website
Get docker to your especific O.S.
👉 [Download Docker official distribution](https://www.docker.com/)

## Running project in Docker
After clone project:

```
cd ~/ambev-challenge/OrderService
docker compose up -d
```
Open your browse and type: http://127.0.0.1:8080/swagger/index.html

---
## Frontend for data insertion

**Change directory to frontend insert:**
```bash
cd ~/ambev-challenge/order-frontend
docker compose up -d
```
Open your browse and type: http://localhost:3000/

## Frontend for list all data inserted or get ByOrderId

**Change directory to frontend insert:**
```bash
cd ~/ambev-challenge/list-orders-frontend
docker compose up -d
```
Open your browse and type: http://localhost:3001/


