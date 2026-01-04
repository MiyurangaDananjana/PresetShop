# Image Upload Feature Documentation

## Overview

This feature allows you to upload preset products with main images, before/after comparison images, and detailed product information through both frontend and backend components.

## Features Implemented

### Backend (.NET)

1. **Enhanced Product Model** (`backend/src/Data/Models/Product.cs`)
   - `BeforeImageUrl` - URL for before image
   - `AfterImageUrl` - URL for after image
   - `Category` - Product category
   - `PresetCount` - Number of presets in the pack

2. **DTOs** (Data Transfer Objects)
   - `CreateProductDto` - For creating new products with image uploads
   - `UpdateProductDto` - For updating existing products
   - `ProductResponseDto` - For returning product data

3. **Services**
   - **ImageService** (`backend/src/Service/ImageService.cs`)
     - Validates images (type, size, MIME type)
     - Saves images to `uploads/` folder
     - Supports JPG, JPEG, PNG, WEBP formats
     - Max file size: 10MB
     - Generates unique filenames using GUIDs

   - **ProductService** (`backend/src/Service/ProductService.cs`)
     - CRUD operations for products
     - Handles multiple image uploads
     - Automatic image cleanup on delete

4. **API Endpoints** (`backend/src/Controllers/ProductsController.cs`)
   - `GET /api/products` - Get all active products
   - `GET /api/products/{id}` - Get product by ID
   - `POST /api/products` - Create new product with images
   - `PUT /api/products/{id}` - Update product
   - `DELETE /api/products/{id}` - Delete product and images

### Frontend (Next.js)

1. **Components**
   - **ProductUploadForm** (`frontend/src/components/ProductUploadForm.tsx`)
     - Complete form for uploading products
     - Validation and error handling
     - Success notifications
     - Form reset functionality

   - **ImageUploadPreview** (`frontend/src/components/ImageUploadPreview.tsx`)
     - Individual image upload with preview
     - File validation (type and size)
     - Clear/reset functionality

   - **BeforeAfterPreview** (`frontend/src/components/BeforeAfterPreview.tsx`)
     - Interactive slider to compare before/after images
     - Drag to compare functionality
     - Touch support for mobile devices

2. **API Client** (`frontend/src/lib/api.ts`)
   - Type-safe API calls
   - FormData handling for file uploads
   - Error handling

3. **Admin Page** (`frontend/src/app/admin/page.tsx`)
   - Admin dashboard for uploading products
   - Access via: `http://localhost:3000/admin`

## How to Use

### Starting the Application

1. **Start with Docker Compose:**
   ```bash
   docker-compose up --build
   ```

2. **Or start services individually:**

   **Backend:**
   ```bash
   cd backend/src
   dotnet run
   ```

   **Frontend:**
   ```bash
   cd frontend
   npm run dev
   ```

### Uploading a Product

1. Navigate to: `http://localhost:3000/admin`

2. Fill in the product details:
   - **Product Name** (required) - e.g., "Moody Film Presets"
   - **Description** (required) - Detailed description
   - **Price** (required) - e.g., 29.99
   - **Category** (optional) - Select from dropdown
   - **Number of Presets** (required) - e.g., 15

3. Upload images:
   - **Main Product Image** (required) - Primary product showcase image
   - **Before Image** (optional) - Original unedited photo
   - **After Image** (optional) - Photo with preset applied

4. If both before and after images are uploaded, you'll see a live comparison slider

5. Click "Upload Preset" to create the product

### API Usage Examples

**Create Product with cURL:**
```bash
curl -X POST http://localhost:8080/api/products \
  -F "name=Moody Film Presets" \
  -F "description=Professional film presets" \
  -F "price=29.99" \
  -F "category=Film" \
  -F "presetCount=15" \
  -F "mainImage=@/path/to/main.jpg" \
  -F "beforeImage=@/path/to/before.jpg" \
  -F "afterImage=@/path/to/after.jpg"
```

**Get All Products:**
```bash
curl http://localhost:8080/api/products
```

**Get Product by ID:**
```bash
curl http://localhost:8080/api/products/1
```

**Delete Product:**
```bash
curl -X DELETE http://localhost:8080/api/products/1
```

## File Structure

```
PresetShop_Project/
├── backend/
│   ├── src/
│   │   ├── Controllers/
│   │   │   └── ProductsController.cs      # API endpoints
│   │   ├── Data/
│   │   │   ├── Models/
│   │   │   │   └── Product.cs             # Enhanced model
│   │   │   └── DTOs/
│   │   │       ├── CreateProductDto.cs    # Create DTO
│   │   │       ├── UpdateProductDto.cs    # Update DTO
│   │   │       └── ProductResponseDto.cs  # Response DTO
│   │   ├── Service/
│   │   │   ├── ImageService.cs            # Image handling
│   │   │   ├── IImageService.cs
│   │   │   ├── ProductService.cs          # Product logic
│   │   │   └── IProductService.cs
│   │   └── uploads/                       # Image storage
│   │       ├── products/                  # Main images
│   │       ├── before/                    # Before images
│   │       └── after/                     # After images
│
├── frontend/
│   ├── src/
│   │   ├── app/
│   │   │   └── admin/
│   │   │       └── page.tsx               # Admin upload page
│   │   ├── components/
│   │   │   ├── ProductUploadForm.tsx      # Main form
│   │   │   ├── ImageUploadPreview.tsx     # Image upload
│   │   │   └── BeforeAfterPreview.tsx     # Comparison slider
│   │   └── lib/
│   │       └── api.ts                     # API client
```

## Image Upload Specifications

### Supported Formats
- JPEG (.jpg, .jpeg)
- PNG (.png)
- WebP (.webp)

### File Size Limits
- Maximum: 10MB per image
- Recommended: 2-5MB for optimal performance

### Image Organization
- Main images: `backend/src/uploads/products/`
- Before images: `backend/src/uploads/before/`
- After images: `backend/src/uploads/after/`

### File Naming
- Files are automatically renamed with GUIDs
- Original extensions are preserved
- Example: `a1b2c3d4-e5f6-7890-abcd-ef1234567890.jpg`

## Security Features

1. **File Validation**
   - Extension checking
   - MIME type validation
   - File size limits

2. **Secure Storage**
   - Files stored outside web root
   - Served through static file middleware
   - CORS protection enabled

3. **Input Validation**
   - Server-side validation on all fields
   - Maximum length constraints
   - Required field enforcement

## Database Migration

The database migration has been created. To apply it:

```bash
cd backend/src
dotnet ef database update
```

This adds the new fields to the Product table:
- `BeforeImageUrl`
- `AfterImageUrl`
- `Category`
- `PresetCount`

## Testing

### Manual Testing

1. **Upload Test:**
   - Go to `/admin`
   - Fill form with test data
   - Upload 3 images
   - Verify success message

2. **Validation Test:**
   - Try uploading without required fields
   - Try uploading non-image files
   - Try uploading files > 10MB

3. **Preview Test:**
   - Upload before and after images
   - Drag the comparison slider
   - Verify smooth interaction

4. **API Test:**
   - Use Swagger UI at `http://localhost:8080/swagger`
   - Test all endpoints
   - Verify responses

## Troubleshooting

### Images not uploading
- Check `uploads/` folder has write permissions
- Verify file size is under 10MB
- Check file format is supported

### CORS errors
- Ensure backend CORS is configured for frontend URL
- Check `Program.cs` has correct frontend origin

### Database errors
- Run migrations: `dotnet ef database update`
- Check PostgreSQL is running
- Verify connection string

### Preview not showing
- Check browser console for errors
- Verify images are valid files
- Clear browser cache

## Next Steps

### Recommended Enhancements

1. **Authentication**
   - Add user authentication
   - Protect admin routes
   - Role-based access control

2. **Image Optimization**
   - Automatic image resizing
   - WebP conversion
   - Thumbnail generation

3. **Cloud Storage**
   - Integrate AWS S3 or Azure Blob
   - CDN for faster delivery
   - Automatic backups

4. **Advanced Features**
   - Bulk upload
   - Drag and drop
   - Image cropping
   - Multiple before/after pairs

## Support

For issues or questions:
- Check backend logs: `docker-compose logs backend`
- Check frontend logs: `docker-compose logs frontend`
- Review API docs: `http://localhost:8080/swagger`
