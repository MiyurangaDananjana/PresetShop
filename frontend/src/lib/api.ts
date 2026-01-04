const API_URL = process.env.NEXT_PUBLIC_API_URL || 'http://localhost:8080/api';

// Auth Types
export interface LoginData {
  email: string;
  password: string;
}

export interface RegisterData {
  fullName: string;
  email: string;
  password: string;
  phoneNumber: string;
  address: string;
  city: string;
  country: string;
  postalCode: string;
}

export interface AuthResponse {
  id: number;
  email: string;
  fullName: string;
  token: string;
  role: string;
}

// Category Types
export interface Category {
  id: number;
  name: string;
  description?: string;
  isActive: boolean;
  createdAt: string;
}

export interface CreateCategoryData {
  name: string;
  description?: string;
}

// Preset Types
export interface Preset {
  id: number;
  name: string;
  description: string;
  price: number;
  categoryId?: number;
  categoryName?: string;
  beforeImageUrl?: string;
  afterImageUrl?: string;
  presetFileUrl?: string;
  isActive: boolean;
  createdAt: string;
}

export interface CreatePresetData {
  name: string;
  description: string;
  price: number;
  categoryId?: number;
  beforeImage?: File;
  afterImage?: File;
  presetFile?: File;
}

// Purchase Types
export interface Purchase {
  id: number;
  userId: number;
  presetId: number;
  presetName?: string;
  purchasePrice: number;
  transactionId?: string;
  purchasedAt: string;
  isCompleted: boolean;
}

// Helper function to get auth headers
function getAuthHeaders(): HeadersInit {
  const token = localStorage.getItem('token');
  return {
    'Authorization': token ? `Bearer ${token}` : '',
  };
}

// Auth API
export class AuthAPI {
  static async adminLogin(data: LoginData): Promise<AuthResponse> {
    const response = await fetch(`${API_URL}/auth/admin/login`, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify(data),
    });

    if (!response.ok) {
      const error = await response.json();
      throw new Error(error.message || 'Login failed');
    }

    return response.json();
  }

  static async userLogin(data: LoginData): Promise<AuthResponse> {
    const response = await fetch(`${API_URL}/auth/user/login`, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify(data),
    });

    if (!response.ok) {
      const error = await response.json();
      throw new Error(error.message || 'Login failed');
    }

    return response.json();
  }

  static async userRegister(data: RegisterData): Promise<AuthResponse> {
    const response = await fetch(`${API_URL}/auth/user/register`, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify(data),
    });

    if (!response.ok) {
      const error = await response.json();
      throw new Error(error.message || 'Registration failed');
    }

    return response.json();
  }
}

// Category API
export class CategoryAPI {
  static async getAll(): Promise<Category[]> {
    const response = await fetch(`${API_URL}/categories`);
    if (!response.ok) throw new Error('Failed to fetch categories');
    return response.json();
  }

  static async getById(id: number): Promise<Category> {
    const response = await fetch(`${API_URL}/categories/${id}`);
    if (!response.ok) throw new Error('Failed to fetch category');
    return response.json();
  }

  static async create(data: CreateCategoryData): Promise<Category> {
    const response = await fetch(`${API_URL}/categories`, {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
        ...getAuthHeaders(),
      },
      body: JSON.stringify(data),
    });

    if (!response.ok) {
      const error = await response.json();
      throw new Error(error.message || 'Failed to create category');
    }

    return response.json();
  }

  static async delete(id: number): Promise<void> {
    const response = await fetch(`${API_URL}/categories/${id}`, {
      method: 'DELETE',
      headers: getAuthHeaders(),
    });

    if (!response.ok) throw new Error('Failed to delete category');
  }
}

// Preset API
export class PresetAPI {
  static async getAll(categoryId?: number): Promise<Preset[]> {
    const url = categoryId
      ? `${API_URL}/presets?categoryId=${categoryId}`
      : `${API_URL}/presets`;

    const response = await fetch(url);
    if (!response.ok) throw new Error('Failed to fetch presets');
    return response.json();
  }

  static async getById(id: number): Promise<Preset> {
    const response = await fetch(`${API_URL}/presets/${id}`);
    if (!response.ok) throw new Error('Failed to fetch preset');
    return response.json();
  }

  static async create(data: CreatePresetData): Promise<Preset> {
    const formData = new FormData();
    formData.append('name', data.name);
    formData.append('description', data.description);
    formData.append('price', data.price.toString());

    if (data.categoryId) {
      formData.append('categoryId', data.categoryId.toString());
    }

    if (data.beforeImage) {
      formData.append('beforeImage', data.beforeImage);
    }

    if (data.afterImage) {
      formData.append('afterImage', data.afterImage);
    }

    if (data.presetFile) {
      formData.append('presetFile', data.presetFile);
    }

    const response = await fetch(`${API_URL}/presets`, {
      method: 'POST',
      headers: getAuthHeaders(),
      body: formData,
    });

    if (!response.ok) {
      const error = await response.json();
      throw new Error(error.message || 'Failed to create preset');
    }

    return response.json();
  }

  static async delete(id: number): Promise<void> {
    const response = await fetch(`${API_URL}/presets/${id}`, {
      method: 'DELETE',
      headers: getAuthHeaders(),
    });

    if (!response.ok) throw new Error('Failed to delete preset');
  }
}

// Purchase API
export class PurchaseAPI {
  static async create(presetId: number): Promise<Purchase> {
    const response = await fetch(`${API_URL}/purchases`, {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
        ...getAuthHeaders(),
      },
      body: JSON.stringify({ presetId }),
    });

    if (!response.ok) {
      const error = await response.json();
      throw new Error(error.message || 'Failed to purchase preset');
    }

    return response.json();
  }

  static async getMyPurchases(): Promise<Purchase[]> {
    const response = await fetch(`${API_URL}/purchases/my-purchases`, {
      headers: getAuthHeaders(),
    });

    if (!response.ok) throw new Error('Failed to fetch purchases');
    return response.json();
  }

  static async download(presetId: number): Promise<Blob> {
    const response = await fetch(`${API_URL}/purchases/download/${presetId}`, {
      headers: getAuthHeaders(),
    });

    if (!response.ok) throw new Error('Failed to download preset');
    return response.blob();
  }
}
