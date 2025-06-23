# 📚 Library Management API

This is a C# ASP.NET Core Web API for managing a digital library system. It supports user roles (Admin, Reader), JWT authentication, and full CRUD operations for books and authors. Ideal for building secure, scalable library systems.

---

## ✨ Features

- ✅ **JWT Authentication** for secure API access
- ✅ **Role-based access**:
  - **Admin**: Full access (add/edit/delete books & authors)
  - **Reader**: Read-only access
- ✅ Manage **Books** (create, update, delete, list)
- ✅ Manage **Authors**
- ✅ View books by title
- ✅ Clean RESTful endpoints
- ✅ Built using **Entity Framework Core** and **SQL Server**
- ✅ Fully documented with **Swagger UI**

---

## 🧱 Technologies Used

- ASP.NET Core Web API (.NET 8)
- Entity Framework Core (Code First)
- SQL Server
- JWT Bearer Authentication
- Swagger 
- Visual Studio 2022

## 🚀 How to Run the Project

1. Clone the repository:
   ```bash
   git clone https://github.com/YourUsername/LibraryAPI.git
2. Open the project in Visual Studio 2022
3. Update appsettings.json with your SQL Server connection string
4. Apply EF Core migrations (in Package Manager Console):
  ```bash
  Update-Database
  ```
5. Run the project 
6. **register** then **login** to receive a JWT token.
7. Use the API endpoints via Swagger UI or Postman with your token.
