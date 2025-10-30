# ChatSystem

A real-time chat application built with ASP.NET Core and SignalR, featuring user authentication, group messaging, and file sharing capabilities.

## 🌟 Features

- **Real-time Communication**: Built with SignalR for instant message delivery
- **User Authentication**: Secure JWT-based authentication system
- **Group Messaging**: Create and manage chat groups
- **File Sharing**: Upload and share files in conversations
- **User Management**: User registration, login, and profile management
- **Message History**: Persistent message storage with SQL Server
- **CORS Support**: Configured for web-based frontend integration

## 🛠️ Technology Stack

### Backend
- **Framework**: ASP.NET Core 3.1
- **Real-time Communication**: SignalR
- **Authentication**: JWT (JSON Web Tokens)
- **ORM**: Entity Framework Core 3.1.4
- **Database**: SQL Server
- **Identity Management**: ASP.NET Core Identity
- **Mapping**: AutoMapper 7.0.0

### Frontend
- **Framework**: Angular
- **Deployment**: Netlify (https://chatf.netlify.app)

## 📋 Prerequisites

Before running this project, ensure you have the following installed:

- [.NET Core 3.1 SDK](https://dotnet.microsoft.com/download/dotnet/3.1)
- [SQL Server](https://www.microsoft.com/en-us/sql-server/sql-server-downloads) (or SQL Server Express)
- [Node.js](https://nodejs.org/) (for frontend development)

## 🚀 Installation & Setup

### 1. Clone the Repository

```bash
git clone https://github.com/celilseref/ChatSystem.git
cd ChatSystem
```

### 2. Database Configuration

Update the connection string in `ServerApp/Startup.cs` (line 45):

```csharp
services.AddDbContext<ChatContext>(options =>
    options.UseSqlServer("YOUR_CONNECTION_STRING"));
```

### 3. Apply Database Migrations

```bash
cd ServerApp
dotnet ef database update
```

### 4. Configure JWT Secret

Update the JWT secret in `ServerApp/appsettings.json`:

```json
{
  "AppSettings": {
    "Secret": "YOUR_SECRET_KEY_HERE"
  }
}
```

### 5. Install Dependencies

```bash
dotnet restore
```

### 6. Run the Application

```bash
dotnet run
```

The server will start on `http://localhost:5000` (or `https://localhost:5001` for HTTPS).

## 📁 Project Structure

```
ChatSystem/
├── ServerApp/
│   ├── Controllers/           # API Controllers
│   │   ├── UserController.cs
│   │   ├── MessageController.cs
│   │   └── GroupController.cs
│   ├── Hubs/                  # SignalR Hubs
│   │   └── ChatHub.cs
│   ├── Models/                # Data Models
│   │   ├── User.cs
│   │   ├── Message.cs
│   │   ├── Group.cs
│   │   └── GroupUser.cs
│   ├── Data/                  # Database Context & Repositories
│   ├── DTO/                   # Data Transfer Objects
│   ├── Helpers/               # Helper Classes
│   ├── Program.cs
│   └── Startup.cs
└── README.md
```

## 🔌 API Endpoints

### Authentication
- `POST /api/user/register` - Register a new user
- `POST /api/user/login` - User login

### Messages
- `GET /api/message` - Get messages
- `POST /api/message` - Send a message
- `PUT /api/message` - Update a message
- `DELETE /api/message/{id}` - Delete a message

### Groups
- `GET /api/group` - Get groups
- `POST /api/group` - Create a group
- `PUT /api/group` - Update a group
- `DELETE /api/group/{id}` - Delete a group

### SignalR Hub
- **Hub URL**: `/ChatHub`
- Supports real-time message broadcasting and notifications

## 🔒 Security Features

- JWT-based authentication
- Password requirements:
  - Minimum 6 characters
  - At least one digit
  - At least one lowercase letter
- Secure token validation
- CORS policy configured for authorized origins

## ⚙️ Configuration

### File Upload Limits
- Maximum request body size: 100 MB
- Multipart body length limit: 60 MB

### Identity Options
- Unique email required
- Username characters: alphanumeric, dash, underscore, period, @, +

## 🌐 CORS Configuration

The API is configured to accept requests from:
- `https://chatf.netlify.app`

To modify allowed origins, update the CORS policy in `Startup.cs`.

## 🤝 Contributing

Contributions are welcome! Please feel free to submit a Pull Request.

## 📝 License

This project is open source and available under the [MIT License](LICENSE).

## 👤 Author

**Celil Seref**

- GitHub: [@celilseref](https://github.com/celilseref)

---

*Bu proje ASP.NET Core ve SignalR kullanılarak geliştirilmiş bir gerçek zamanlı sohbet uygulamasıdır. Frontend Angular ile yazılmış ve veritabanı olarak SQL Server kullanılmıştır.*
