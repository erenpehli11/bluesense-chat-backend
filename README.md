# Bluesense Chat API

Bluesense Chat API is a real-time messaging platform built with ASP.NET Core 8, PostgreSQL, MongoDB, Redis, and SignalR. It supports secure authentication using JWT, scalable architecture, and clean separation of concerns using layered architecture.

---

## 🔧 Setup

### Requirements

- .NET 8 SDK
- Docker + Docker Compose
- Git

### Environment Variables

These are handled via `appsettings.json`, `appsettings.Docker.json`, or passed via Docker Compose. Key settings include:

- `ConnectionStrings__PostgreSQL`
- `Jwt__Key`, `Jwt__Issuer`, `Jwt__Audience`
- `MongoDb__ConnectionString`, `MongoDb__DatabaseName`
- `Redis__ConnectionString`

### Local Setup

```bash
git clone https://github.com/erenpehli11/bluesense-chat-backend.git
cd bluesense-chat-backend
docker-compose up --build
```

The API will be available at `http://localhost:5000/swagger`

---

## 🔌 API Usage

### 🛡️ AuthController (`/api/auth`)
| Endpoint             | Method | Auth | Description                   |
|----------------------|--------|------|-------------------------------|
| /register            | POST   | ❌   | Register new user             |
| /login               | POST   | ❌   | Login and receive JWT         |
| /refresh-token       | POST   | ❌   | Refresh access token          |

### 👤 UserController (`/api/user`) – JWT Required
| Endpoint             | Method | Description                          |
|----------------------|--------|--------------------------------------|
| /me                  | GET    | Get own profile                      |
| /update-profile      | PUT    | Update profile info                  |
| /update-password     | PUT    | Update password                      |
| /delete              | DELETE | Soft delete own account              |
| /private-chats       | GET    | Get private chat history             |
| /groups              | GET    | Get groups the user belongs to       |

### 💬 MessageController (`/api/message`) – JWT Required
| Endpoint                     | Method | Description                           |
|------------------------------|--------|---------------------------------------|
| /group/{groupId}             | GET    | Get messages of a group               |
| /private/{targetUserId}      | GET    | Get private messages with a user      |
| /group/send                  | POST   | Send message to a group               |
| /private/send                | POST   | Send private message                  |
| /delete/{messageId}          | DELETE | Delete (soft) a message               |
| /recent                      | GET    | Get recent messages                   |
| /group/{groupId}/attachments | GET    | Get group attachments                 |
| /private/{targetId}/attachments | GET | Get private chat attachments          |

### 💬 Hubs (`/hubs/message`) – JWT Required

| `/hubs/message` | WebSocket (SignalR) | Real-time messaging hub |


### 👥 GroupController (`/api/group`) – JWT Required
| Endpoint                  | Method | Description                                |
|---------------------------|--------|--------------------------------------------|
| /                         | GET    | Get all groups                             |
| /{id}                     | GET    | Get group by ID                            |
| /create                   | POST   | Create a new group                         |
| /{id}                     | PUT    | Update group                               |
| /{id}                     | DELETE | Delete group                               |
| /{groupId}/join           | POST   | Request to join a group                    |
| /{groupId}/approve/{userId} | POST | Approve a user's join request              |
| /{groupId}/reject/{userId}  | POST | Reject a user's join request               |
| /{groupId}/leave          | POST   | Leave the group                            |
| /{groupId}/members        | GET    | List members of a group                    |
| /{groupId}/pending        | GET    | List pending join requests                 |

### 🔍 SearchController (`/api/search`) – JWT Required
| Endpoint         | Method | Description                                 |
|------------------|--------|---------------------------------------------|
| /                | GET    | Search users, groups, and messages          |

> Use access token via `?access_token=` query for SignalR authentication.

---

## ⚙️ CI/CD Overview

CI/CD is handled via GitHub Actions:

### `.github/workflows/ci.yml` includes:

- Restores dependencies
- Builds project
- Runs unit tests (if present)
- Validates formatting
- Optionally publishes Docker image

> Deployment to Railway was attempted but later omitted; local Docker deployment is the preferred and working solution.

---

## 🚀 Deployment Steps

### 1. Clone and Build

```bash
git clone https://github.com/erenpehli11/bluesense-chat-backend.git
cd bluesense-chat-backend
docker-compose up --build
```

### 2. Test Endpoints

Open browser:
```
http://localhost:5000/swagger
```

Use Swagger UI to test all endpoints interactively.

---

## 📌 Assumptions and Tradeoffs

### Assumptions

- Application will run in a containerized environment
- MongoDB is used for message storage, PostgreSQL for user and group metadata
- Redis is used as caching layer for performance

### Tradeoffs

- Swagger is exposed even in Production for demonstration
- No external secrets manager (like Vault); environment vars used for config
- Authentication is based on symmetric key JWT; could migrate to IdentityServer for multi-tenant systems
- Railway deployment dropped due to persistent `ASPNETCORE_ENVIRONMENT` override issues

---

## 🛡️ Security Considerations

- JWT-based authentication with token validation middleware
- Passwords hashed using ASP.NET Core Identity's default `PasswordHasher`
- Redis and MongoDB access protected via password
- No open CORS configuration

---

## 🏗️ Architecture

- **Clean separation**: Domain, Application, Infrastructure, API
- **Repository pattern**: With generic repositories and Unit of Work
- **MongoDB**: Used only for chat messages
- **SignalR**: Real-time communication over WebSocket
- **Swagger**: Auto-generated API documentation

---

## ✅ Final Notes

This application is structured with production readiness in mind: scalability, secure endpoints, real-time messaging, and isolated services (Mongo, Redis, PostgreSQL). Easily extendable for multi-user, multi-tenant architecture.

---

