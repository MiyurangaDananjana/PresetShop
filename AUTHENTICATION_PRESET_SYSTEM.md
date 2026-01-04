# Authentication & Preset System - Complete Documentation

## Overview

This document provides comprehensive documentation for the Authentication and Preset Management System added to the PresetShop project. The implementation includes admin authentication, user authentication, category management, preset management with file uploads, and a complete purchase flow.

---

## 📋 Table of Contents

1. [Database Schema](#database-schema)
2. [Backend APIs](#backend-apis)
3. [Frontend Components](#frontend-components)
4. [Authentication Flow](#authentication-flow)
5. [Admin Features](#admin-features)
6. [User Features](#user-features)
7. [Setup & Deployment](#setup--deployment)
8. [Testing Guide](#testing-guide)

---

## 🗄️ Database Schema

### New Tables

#### 1. **Admin**
```sql
- Id (int, PK)
- Username (string, 100)
- Email (string, 100, unique)
- PasswordHash (string)
- CreatedAt (datetime, nullable)
- LastLoginAt (datetime, nullable)
- IsActive (bool)
```

#### 2. **Category**
```sql
- Id (int, PK)
- Name (string, 100, unique)
- Description (string, 500, nullable)
- IsActive (bool)
- CreatedAt (datetime, nullable)
```

#### 3. **Preset**
```sql
- Id (int, PK)
- Name (string, 200)
- Description (string, 2000)
- Price (decimal 18,2)
- CategoryId (int, FK nullable)
- BeforeImageUrl (string, nullable)
- AfterImageUrl (string, nullable)
- PresetFileUrl (string, nullable)
- IsActive (bool)
- CreatedAt (datetime, nullable)
- UpdatedAt (datetime, nullable)
```

#### 4. **Purchase**
```sql
- Id (int, PK)
- UserId (int, FK)
- PresetId (int, FK)
- PurchasePrice (decimal 18,2)
- TransactionId (string, 100, nullable)
- PurchasedAt (datetime)
- IsCompleted (bool)
```

### Relationships

- **Category → Presets**: One-to-Many (SetNull on delete)
- **User → Purchases**: One-to-Many (Restrict on delete)
- **Preset → Purchases**: One-to-Many (Restrict on delete)

### Seed Data

**Default Admin:**
- Email: `admin@presetshop.com`
- Password: `Admin@123`

**Default Categories:**
- Mobile Presets
- Lightroom Presets

---

## 🔌 Backend APIs

### Base URL
```
http://localhost:8080/api
```

### 1. Authentication APIs

#### Admin Login
```http
POST /auth/admin/login
Content-Type: application/json

{
  "email": "admin@presetshop.com",
  "password": "Admin@123"
}

Response:
{
  "id": 1,
  "email": "admin@presetshop.com",
  "fullName": "admin",
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "role": "Admin"
}
```

#### User Login
```http
POST /auth/user/login
Content-Type: application/json

{
  "email": "user@example.com",
  "password": "password123"
}

Response: (Same as Admin Login with role: "User")
```

#### User Registration
```http
POST /auth/user/register
Content-Type: application/json

{
  "fullName": "John Doe",
  "email": "john@example.com",
  "password": "password123",
  "phoneNumber": "1234567890",
  "address": "123 Main St",
  "city": "New York",
  "country": "USA",
  "postalCode": "10001"
}

Response: (Same as Login)
```

### 2. Category APIs

#### Get All Categories (Public)
```http
GET /categories

Response:
[
  {
    "id": 1,
    "name": "Mobile Presets",
    "description": "Lightroom presets optimized for mobile photography",
    "isActive": true,
    "createdAt": "2024-01-01T00:00:00Z"
  }
]
```

#### Get Category by ID (Public)
```http
GET /categories/{id}
```

#### Create Category (Admin Only)
```http
POST /categories
Authorization: Bearer {admin_token}
Content-Type: application/json

{
  "name": "Vintage Presets",
  "description": "Classic vintage film look presets"
}
```

#### Update Category (Admin Only)
```http
PUT /categories/{id}
Authorization: Bearer {admin_token}
Content-Type: application/json

{
  "name": "Updated Name",
  "isActive": false
}
```

#### Delete Category (Admin Only)
```http
DELETE /categories/{id}
Authorization: Bearer {admin_token}
```

### 3. Preset APIs

#### Get All Presets (Public, with optional filter)
```http
GET /presets
GET /presets?categoryId=1

Response:
[
  {
    "id": 1,
    "name": "Moody Film Preset",
    "description": "Dark and moody film-inspired preset",
    "price": 29.99,
    "categoryId": 1,
    "categoryName": "Mobile Presets",
    "beforeImageUrl": "/uploads/presets/before/abc123.jpg",
    "afterImageUrl": "/uploads/presets/after/def456.jpg",
    "presetFileUrl": "/uploads/presets/files/preset789.zip",
    "isActive": true,
    "createdAt": "2024-01-01T00:00:00Z"
  }
]
```

#### Get Preset by ID (Public)
```http
GET /presets/{id}
```

#### Create Preset (Admin Only)
```http
POST /presets
Authorization: Bearer {admin_token}
Content-Type: multipart/form-data

Form Data:
- name: "Preset Name"
- description: "Preset description"
- price: 29.99
- categoryId: 1 (optional)
- beforeImage: File (optional)
- afterImage: File (optional)
- presetFile: File (.zip, .lrtemplate, .xmp, .dng)
```

#### Update Preset (Admin Only)
```http
PUT /presets/{id}
Authorization: Bearer {admin_token}
Content-Type: multipart/form-data

(Same form data as Create, all fields optional)
```

#### Delete Preset (Admin Only)
```http
DELETE /presets/{id}
Authorization: Bearer {admin_token}
```

### 4. Purchase APIs

#### Create Purchase (User Only)
```http
POST /purchases
Authorization: Bearer {user_token}
Content-Type: application/json

{
  "presetId": 1
}

Response:
{
  "id": 1,
  "userId": 1,
  "presetId": 1,
  "presetName": "Moody Film Preset",
  "purchasePrice": 29.99,
  "transactionId": "DEMO-ABC123XYZ456",
  "purchasedAt": "2024-01-01T00:00:00Z",
  "isCompleted": true
}
```

#### Get My Purchases (User Only)
```http
GET /purchases/my-purchases
Authorization: Bearer {user_token}
```

#### Download Purchased Preset (User Only)
```http
GET /purchases/download/{presetId}
Authorization: Bearer {user_token}

Response: Binary file download
```

---

## 🎨 Frontend Components

### Pages

#### 1. `/login` - Login Page
- **Features:**
  - Toggle between User and Admin login
  - Display default admin credentials
  - Form validation
  - Error handling
  - Redirect based on role (Admin → `/admin`, User → `/presets`)

#### 2. `/register` - Registration Page
- **Features:**
  - Complete user registration form
  - All required fields (name, email, password, address, etc.)
  - Form validation
  - Auto-login after registration
  - Redirect to `/presets`

#### 3. `/admin` - Admin Dashboard
- **Features:**
  - Protected route (Admin only)
  - Tabbed interface (Presets & Categories)
  - **Category Management:**
    - Create new categories
    - List all categories
    - Delete categories
  - **Preset Management:**
    - Create presets with images and files
    - Upload before/after images
    - Upload preset files (ZIP, LRTEMPLATE, XMP, DNG)
    - List all presets
    - Delete presets
  - Real-time success/error notifications
  - Logout functionality

#### 4. `/presets` - Public Presets Page
- **Features:**
  - Browse all presets
  - Filter by category
  - Before/After image comparison slider
  - Purchase button (demo payment)
  - Auto-download after purchase
  - Login prompt for unauthenticated users
  - Responsive grid layout

### Components

#### AuthProvider (`contexts/AuthContext.tsx`)
- Manages authentication state globally
- Stores user data and token in localStorage
- Provides `useAuth()` hook
- Properties:
  - `user`: Current user data
  - `isAuthenticated`: Boolean
  - `isAdmin`: Boolean
  - `login()`: Function
  - `logout()`: Function

#### ImageUploadPreview
- Image upload with live preview
- File validation
- Clear/reset functionality
- Size and type restrictions

#### BeforeAfterPreview
- Interactive image comparison slider
- Drag to compare
- Touch support for mobile

### API Client (`lib/api.ts`)
- Type-safe API calls
- Automatic token injection
- Error handling
- Classes:
  - `AuthAPI`
  - `CategoryAPI`
  - `PresetAPI`
  - `PurchaseAPI`

---

## 🔐 Authentication Flow

### JWT Token Structure
```json
{
  "sub": "userId",
  "email": "user@example.com",
  "role": "User|Admin",
  "jti": "unique-identifier",
  "exp": timestamp
}
```

### Token Configuration
- **Secret Key**: Configured in `appsettings.json` (JwtSettings:SecretKey)
- **Expiry**: 24 hours (configurable)
- **Storage**: LocalStorage (frontend)
- **Transmission**: Authorization Bearer header

### Protected Routes

**Backend:**
- Admin routes: Require `[Authorize(Roles = "Admin")]`
- User routes: Require `[Authorize(Roles = "User")]`

**Frontend:**
- Admin pages: Check `isAdmin` in useEffect, redirect to `/login` if not authorized
- Purchase actions: Check `isAuthenticated`, redirect to `/login` if needed

---

## 👨‍💼 Admin Features

### 1. Category Management
- ✅ Create categories with name and description
- ✅ View all categories
- ✅ Delete categories
- ✅ Categories automatically seeded (Mobile, Lightroom)

### 2. Preset Management
- ✅ Create presets with full details
- ✅ Upload before/after comparison images
- ✅ Upload preset files (ZIP, LRTEMPLATE, XMP, DNG, max 50MB)
- ✅ Assign category to presets
- ✅ Set pricing
- ✅ View all presets with status
- ✅ Delete presets (auto-cleanup files)
- ✅ Active/Inactive status

### 3. File Management
- **Image Storage:** `/backend/src/uploads/presets/before` & `/uploads/presets/after`
- **Preset Files:** `/backend/src/uploads/presets/files`
- **Allowed Image Types:** JPG, JPEG, PNG, WebP (max 10MB)
- **Allowed Preset Types:** ZIP, LRTEMPLATE, XMP, DNG (max 50MB)
- **Automatic Cleanup:** Files deleted when preset is removed

---

## 👤 User Features

### 1. Authentication
- ✅ User registration with complete profile
- ✅ Email/password login
- ✅ Persistent sessions (JWT token)
- ✅ Logout functionality

### 2. Browse Presets
- ✅ View all active presets
- ✅ Filter by category
- ✅ See before/after comparisons
- ✅ View pricing and descriptions

### 3. Purchase Flow
- ✅ Demo "Buy Now" button
- ✅ Simulated payment (no real gateway)
- ✅ Transaction ID generation
- ✅ Purchase record creation
- ✅ Automatic file download
- ✅ Purchase history tracking
- ✅ Prevent duplicate purchases

### 4. Download Purchased Presets
- ✅ Access to purchased presets
- ✅ Direct file download
- ✅ Purchase verification before download

---

## 🚀 Setup & Deployment

### Prerequisites
- .NET 8.0 SDK
- Node.js 18+
- PostgreSQL 15
- Docker & Docker Compose (optional)

### Backend Setup

1. **Install Dependencies:**
```bash
cd backend/src
dotnet restore
```

2. **Update Configuration:**
Edit `appsettings.json` to set JWT secret key (change the default!)

3. **Run Migration:**
```bash
dotnet ef database update
```

4. **Run Backend:**
```bash
dotnet run
```

Backend runs on: `http://localhost:8080`

### Frontend Setup

1. **Install Dependencies:**
```bash
cd frontend
npm install
```

2. **Configure API URL:**
Ensure `.env.local` or environment variable:
```
NEXT_PUBLIC_API_URL=http://localhost:8080/api
```

3. **Run Frontend:**
```bash
npm run dev
```

Frontend runs on: `http://localhost:3000`

### Docker Deployment

```bash
docker-compose up --build
```

Services:
- Frontend: `http://localhost:3000`
- Backend API: `http://localhost:8080`
- Database: PostgreSQL on port 5432

---

## 🧪 Testing Guide

### 1. Admin Flow

**Login:**
1. Go to `http://localhost:3000/login`
2. Click "Admin Login"
3. Use credentials:
   - Email: `admin@presetshop.com`
   - Password: `Admin@123`
4. Should redirect to `/admin`

**Create Category:**
1. Go to "Categories" tab
2. Click "Add New Category"
3. Enter name and description
4. Click "Create Category"
5. Verify category appears in list

**Create Preset:**
1. Go to "Presets" tab
2. Click "Add New Preset"
3. Fill form:
   - Name: "Test Preset"
   - Description: "Test description"
   - Price: 19.99
   - Category: Select created category
4. Upload before image
5. Upload after image
6. Upload preset file (any .zip file for testing)
7. Click "Create Preset"
8. Verify preset appears in list

### 2. User Flow

**Register:**
1. Go to `http://localhost:3000/register`
2. Fill all required fields
3. Click "Create account"
4. Should redirect to `/presets`

**Browse & Purchase:**
1. Go to `/presets`
2. See list of presets
3. Filter by category
4. Click "Buy Now (Demo)" on a preset
5. Should see success alert with transaction ID
6. File should automatically download

**Verify Download:**
1. Check that file downloaded to your downloads folder
2. Verify file is the uploaded preset file

### 3. API Testing with cURL

**Admin Login:**
```bash
curl -X POST http://localhost:8080/api/auth/admin/login \
  -H "Content-Type: application/json" \
  -d '{"email":"admin@presetshop.com","password":"Admin@123"}'
```

**Get All Presets:**
```bash
curl http://localhost:8080/api/presets
```

**Create Category (with token):**
```bash
curl -X POST http://localhost:8080/api/categories \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer YOUR_TOKEN_HERE" \
  -d '{"name":"Test Category","description":"Test Description"}'
```

---

## 📝 Key Implementation Details

### Security Features
- ✅ **JWT Authentication** with role-based access
- ✅ **Password Hashing** using BCrypt
- ✅ **File Validation** (type, size, content)
- ✅ **CORS Protection** configured for frontend origin
- ✅ **Authorization Middleware** on protected routes
- ✅ **Unique Email Constraints** (Admin & User tables)

### File Upload Features
- ✅ **Multiple File Types** supported
- ✅ **Unique Filenames** (GUID-based)
- ✅ **Automatic Cleanup** on delete
- ✅ **Size Limits** enforced
- ✅ **Organized Storage** structure

### Database Features
- ✅ **EF Core Migrations** for schema management
- ✅ **Automatic Seeding** of default data
- ✅ **Proper Relationships** with cascade rules
- ✅ **Indexes** on email and category name

### Frontend Features
- ✅ **Type-Safe API Client** (TypeScript)
- ✅ **Global Auth State** (Context API)
- ✅ **Protected Routes** with redirects
- ✅ **Real-time Validation** and feedback
- ✅ **Responsive Design** (Tailwind CSS)
- ✅ **Image Optimization** (Next.js Image)

---

## 📁 File Structure

```
backend/src/
├── Controllers/
│   ├── AuthController.cs          ✅ NEW
│   ├── CategoriesController.cs    ✅ NEW
│   ├── PresetsController.cs       ✅ NEW
│   ├── PurchasesController.cs     ✅ NEW
│   └── ProductsController.cs      (existing)
├── Data/
│   ├── Models/
│   │   ├── Admin.cs               ✅ NEW
│   │   ├── Category.cs            ✅ NEW
│   │   ├── Preset.cs              ✅ NEW
│   │   ├── Purchase.cs            ✅ NEW
│   │   └── (existing models)
│   ├── DTOs/
│   │   ├── Auth/
│   │   │   ├── LoginDto.cs        ✅ NEW
│   │   │   ├── RegisterDto.cs     ✅ NEW
│   │   │   └── AuthResponseDto.cs ✅ NEW
│   │   ├── CategoryDto.cs         ✅ NEW
│   │   ├── PresetDto.cs           ✅ NEW
│   │   └── PurchaseDto.cs         ✅ NEW
│   ├── ApplicationDbContext.cs    ✅ UPDATED
│   └── DbInitializer.cs           ✅ NEW
├── Service/
│   ├── IAuthService.cs            ✅ NEW
│   ├── AuthService.cs             ✅ NEW
│   ├── ITokenService.cs           ✅ NEW
│   ├── TokenService.cs            ✅ NEW
│   ├── ICategoryService.cs        ✅ NEW
│   ├── CategoryService.cs         ✅ NEW
│   ├── IPresetService.cs          ✅ NEW
│   ├── PresetService.cs           ✅ NEW
│   ├── IPurchaseService.cs        ✅ NEW
│   └── PurchaseService.cs         ✅ NEW
├── Program.cs                     ✅ UPDATED (JWT, services)
├── appsettings.json               ✅ UPDATED (JWT config)
└── uploads/                       ✅ NEW
    └── presets/
        ├── before/
        ├── after/
        └── files/

frontend/src/
├── app/
│   ├── login/
│   │   └── page.tsx               ✅ NEW
│   ├── register/
│   │   └── page.tsx               ✅ NEW
│   ├── admin/
│   │   └── page.tsx               ✅ UPDATED
│   ├── presets/
│   │   └── page.tsx               ✅ NEW
│   └── layout.tsx                 ✅ UPDATED (AuthProvider)
├── contexts/
│   └── AuthContext.tsx            ✅ NEW
├── lib/
│   └── api.ts                     ✅ UPDATED (full API client)
└── components/
    ├── ImageUploadPreview.tsx     (existing)
    └── BeforeAfterPreview.tsx     (existing)
```

---

## 🎯 Features Summary

### ✅ Completed Features

**Authentication:**
- [x] Admin Login/Logout
- [x] User Registration
- [x] User Login/Logout
- [x] JWT Token Management
- [x] Role-Based Access Control
- [x] Persistent Sessions

**Admin Dashboard:**
- [x] Category Management (CRUD)
- [x] Preset Management (CRUD)
- [x] Before/After Image Upload
- [x] Preset File Upload
- [x] Category Assignment
- [x] Active/Inactive Status

**User Features:**
- [x] Browse Presets
- [x] Filter by Category
- [x] Before/After Comparison
- [x] Demo Purchase Flow
- [x] Automatic File Download
- [x] Purchase History
- [x] Duplicate Purchase Prevention

**Technical:**
- [x] Database Schema & Migrations
- [x] Seed Data
- [x] File Storage & Management
- [x] API Documentation
- [x] Type-Safe Frontend
- [x] Responsive UI

---

## 🔧 Troubleshooting

### Backend Issues

**Build Errors:**
```bash
cd backend/src
dotnet clean
dotnet restore
dotnet build
```

**Database Issues:**
```bash
# Reset database
dotnet ef database drop
dotnet ef database update
```

**JWT Errors:**
- Ensure `JwtSettings:SecretKey` in appsettings.json is at least 32 characters
- Check token expiry time

### Frontend Issues

**Auth State Not Persisting:**
- Check browser localStorage
- Clear localStorage and re-login
- Ensure AuthProvider wraps the app

**File Upload Fails:**
- Check file size limits (images: 10MB, presets: 50MB)
- Verify file types are allowed
- Check backend uploads folder permissions

**API Calls Fail:**
- Verify `NEXT_PUBLIC_API_URL` environment variable
- Check backend is running on correct port
- Inspect browser network tab for CORS issues

---

## 📞 Support

For issues or questions:
1. Check this documentation
2. Review backend logs: `docker-compose logs backend`
3. Check frontend console for errors
4. Verify all services are running: `docker-compose ps`

---

## 🎉 Success!

You now have a fully functional authentication and preset management system with:
- ✅ Secure JWT authentication
- ✅ Admin dashboard for content management
- ✅ Public preset browsing and purchasing
- ✅ File upload and download capabilities
- ✅ Complete purchase flow with demo payment

**Next Steps:**
1. Change default admin password in production
2. Configure proper JWT secret key
3. Set up real payment gateway (Stripe, PayPal)
4. Add email notifications
5. Implement admin analytics dashboard
