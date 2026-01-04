'use client';

import { useState, useEffect } from 'react';
import { useRouter } from 'next/navigation';
import { useAuth } from '@/contexts/AuthContext';
import { CategoryAPI, PresetAPI, Category, Preset, CreateCategoryData, CreatePresetData } from '@/lib/api';
import ImageUploadPreview from '@/components/ImageUploadPreview';

export default function AdminPage() {
  const router = useRouter();
  const { isAdmin, loading: authLoading, logout } = useAuth();
  const [activeTab, setActiveTab] = useState<'categories' | 'presets'>('presets');
  const [categories, setCategories] = useState<Category[]>([]);
  const [presets, setPresets] = useState<Preset[]>([]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [success, setSuccess] = useState<string | null>(null);

  // Category form state
  const [showCategoryForm, setShowCategoryForm] = useState(false);
  const [categoryForm, setCategoryForm] = useState<CreateCategoryData>({
    name: '',
    description: '',
  });

  // Preset form state
  const [showPresetForm, setShowPresetForm] = useState(false);
  const [presetForm, setPresetForm] = useState({
    name: '',
    description: '',
    price: '',
    categoryId: '',
  });
  const [beforeImage, setBeforeImage] = useState<File | null>(null);
  const [afterImage, setAfterImage] = useState<File | null>(null);
  const [presetFile, setPresetFile] = useState<File | null>(null);

  useEffect(() => {
    if (!authLoading && !isAdmin) {
      router.push('/login');
    }
  }, [isAdmin, authLoading, router]);

  useEffect(() => {
    if (isAdmin) {
      fetchCategories();
      fetchPresets();
    }
  }, [isAdmin]);

  const fetchCategories = async () => {
    try {
      const data = await CategoryAPI.getAll();
      setCategories(data);
    } catch (err) {
      setError('Failed to fetch categories');
    }
  };

  const fetchPresets = async () => {
    try {
      const data = await PresetAPI.getAll();
      setPresets(data);
    } catch (err) {
      setError('Failed to fetch presets');
    }
  };

  const handleCreateCategory = async (e: React.FormEvent) => {
    e.preventDefault();
    setError(null);
    setLoading(true);

    try {
      await CategoryAPI.create(categoryForm);
      setSuccess('Category created successfully!');
      setCategoryForm({ name: '', description: '' });
      setShowCategoryForm(false);
      fetchCategories();
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Failed to create category');
    } finally {
      setLoading(false);
    }
  };

  const handleDeleteCategory = async (id: number) => {
    if (!confirm('Are you sure you want to delete this category?')) return;

    try {
      await CategoryAPI.delete(id);
      setSuccess('Category deleted successfully!');
      fetchCategories();
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Failed to delete category');
    }
  };

  const handleCreatePreset = async (e: React.FormEvent) => {
    e.preventDefault();
    setError(null);
    setLoading(true);

    try {
      const data: CreatePresetData = {
        name: presetForm.name,
        description: presetForm.description,
        price: parseFloat(presetForm.price),
        categoryId: presetForm.categoryId ? parseInt(presetForm.categoryId) : undefined,
        beforeImage: beforeImage || undefined,
        afterImage: afterImage || undefined,
        presetFile: presetFile || undefined,
      };

      await PresetAPI.create(data);
      setSuccess('Preset created successfully!');
      setPresetForm({ name: '', description: '', price: '', categoryId: '' });
      setBeforeImage(null);
      setAfterImage(null);
      setPresetFile(null);
      setShowPresetForm(false);
      fetchPresets();
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Failed to create preset');
    } finally {
      setLoading(false);
    }
  };

  const handleDeletePreset = async (id: number) => {
    if (!confirm('Are you sure you want to delete this preset?')) return;

    try {
      await PresetAPI.delete(id);
      setSuccess('Preset deleted successfully!');
      fetchPresets();
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Failed to delete preset');
    }
  };

  if (authLoading) {
    return <div className="min-h-screen flex items-center justify-center">Loading...</div>;
  }

  if (!isAdmin) {
    return null;
  }

  return (
    <div className="min-h-screen bg-gray-50 py-12 px-4">
      <div className="max-w-7xl mx-auto">
        {/* Header */}
        <div className="flex justify-between items-center mb-8">
          <div>
            <h1 className="text-4xl font-bold text-gray-900">Admin Dashboard</h1>
            <p className="text-gray-600 mt-2">Manage categories and presets</p>
          </div>
          <button
            onClick={logout}
            className="px-4 py-2 bg-red-600 text-white rounded-md hover:bg-red-700"
          >
            Logout
          </button>
        </div>

        {/* Alerts */}
        {error && (
          <div className="mb-4 p-4 bg-red-50 border border-red-200 text-red-700 rounded-md">
            {error}
          </div>
        )}

        {success && (
          <div className="mb-4 p-4 bg-green-50 border border-green-200 text-green-700 rounded-md">
            {success}
          </div>
        )}

        {/* Tabs */}
        <div className="mb-6 border-b border-gray-200">
          <nav className="-mb-px flex space-x-8">
            <button
              onClick={() => setActiveTab('presets')}
              className={`py-4 px-1 border-b-2 font-medium text-sm ${
                activeTab === 'presets'
                  ? 'border-primary-500 text-primary-600'
                  : 'border-transparent text-gray-500 hover:text-gray-700 hover:border-gray-300'
              }`}
            >
              Presets ({presets.length})
            </button>
            <button
              onClick={() => setActiveTab('categories')}
              className={`py-4 px-1 border-b-2 font-medium text-sm ${
                activeTab === 'categories'
                  ? 'border-primary-500 text-primary-600'
                  : 'border-transparent text-gray-500 hover:text-gray-700 hover:border-gray-300'
              }`}
            >
              Categories ({categories.length})
            </button>
          </nav>
        </div>

        {/* Categories Tab */}
        {activeTab === 'categories' && (
          <div className="space-y-6">
            <button
              onClick={() => setShowCategoryForm(!showCategoryForm)}
              className="px-4 py-2 bg-primary-600 text-white rounded-md hover:bg-primary-700"
            >
              {showCategoryForm ? 'Cancel' : 'Add New Category'}
            </button>

            {showCategoryForm && (
              <div className="bg-white p-6 rounded-lg shadow">
                <h3 className="text-lg font-semibold mb-4">Create New Category</h3>
                <form onSubmit={handleCreateCategory} className="space-y-4">
                  <div>
                    <label className="block text-sm font-medium text-gray-700 mb-1">
                      Name
                    </label>
                    <input
                      type="text"
                      required
                      value={categoryForm.name}
                      onChange={(e) => setCategoryForm({ ...categoryForm, name: e.target.value })}
                      className="w-full px-3 py-2 border border-gray-300 rounded-md focus:ring-primary-500 focus:border-primary-500"
                    />
                  </div>

                  <div>
                    <label className="block text-sm font-medium text-gray-700 mb-1">
                      Description
                    </label>
                    <textarea
                      value={categoryForm.description}
                      onChange={(e) => setCategoryForm({ ...categoryForm, description: e.target.value })}
                      className="w-full px-3 py-2 border border-gray-300 rounded-md focus:ring-primary-500 focus:border-primary-500"
                      rows={3}
                    />
                  </div>

                  <button
                    type="submit"
                    disabled={loading}
                    className="px-4 py-2 bg-primary-600 text-white rounded-md hover:bg-primary-700 disabled:opacity-50"
                  >
                    {loading ? 'Creating...' : 'Create Category'}
                  </button>
                </form>
              </div>
            )}

            <div className="grid gap-4">
              {categories.map((category) => (
                <div key={category.id} className="bg-white p-6 rounded-lg shadow flex justify-between items-center">
                  <div>
                    <h3 className="text-lg font-semibold">{category.name}</h3>
                    <p className="text-gray-600">{category.description}</p>
                    <p className="text-sm text-gray-500 mt-1">
                      Status: {category.isActive ? 'Active' : 'Inactive'}
                    </p>
                  </div>
                  <button
                    onClick={() => handleDeleteCategory(category.id)}
                    className="px-4 py-2 bg-red-600 text-white rounded-md hover:bg-red-700"
                  >
                    Delete
                  </button>
                </div>
              ))}
            </div>
          </div>
        )}

        {/* Presets Tab */}
        {activeTab === 'presets' && (
          <div className="space-y-6">
            <button
              onClick={() => setShowPresetForm(!showPresetForm)}
              className="px-4 py-2 bg-primary-600 text-white rounded-md hover:bg-primary-700"
            >
              {showPresetForm ? 'Cancel' : 'Add New Preset'}
            </button>

            {showPresetForm && (
              <div className="bg-white p-6 rounded-lg shadow">
                <h3 className="text-lg font-semibold mb-4">Create New Preset</h3>
                <form onSubmit={handleCreatePreset} className="space-y-4">
                  <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
                    <div>
                      <label className="block text-sm font-medium text-gray-700 mb-1">
                        Name
                      </label>
                      <input
                        type="text"
                        required
                        value={presetForm.name}
                        onChange={(e) => setPresetForm({ ...presetForm, name: e.target.value })}
                        className="w-full px-3 py-2 border border-gray-300 rounded-md focus:ring-primary-500 focus:border-primary-500"
                      />
                    </div>

                    <div>
                      <label className="block text-sm font-medium text-gray-700 mb-1">
                        Price ($)
                      </label>
                      <input
                        type="number"
                        step="0.01"
                        required
                        value={presetForm.price}
                        onChange={(e) => setPresetForm({ ...presetForm, price: e.target.value })}
                        className="w-full px-3 py-2 border border-gray-300 rounded-md focus:ring-primary-500 focus:border-primary-500"
                      />
                    </div>
                  </div>

                  <div>
                    <label className="block text-sm font-medium text-gray-700 mb-1">
                      Category
                    </label>
                    <select
                      value={presetForm.categoryId}
                      onChange={(e) => setPresetForm({ ...presetForm, categoryId: e.target.value })}
                      className="w-full px-3 py-2 border border-gray-300 rounded-md focus:ring-primary-500 focus:border-primary-500"
                    >
                      <option value="">Select category</option>
                      {categories.map((cat) => (
                        <option key={cat.id} value={cat.id}>
                          {cat.name}
                        </option>
                      ))}
                    </select>
                  </div>

                  <div>
                    <label className="block text-sm font-medium text-gray-700 mb-1">
                      Description
                    </label>
                    <textarea
                      required
                      value={presetForm.description}
                      onChange={(e) => setPresetForm({ ...presetForm, description: e.target.value })}
                      className="w-full px-3 py-2 border border-gray-300 rounded-md focus:ring-primary-500 focus:border-primary-500"
                      rows={4}
                    />
                  </div>

                  <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
                    <ImageUploadPreview
                      label="Before Image"
                      onImageSelect={setBeforeImage}
                    />
                    <ImageUploadPreview
                      label="After Image"
                      onImageSelect={setAfterImage}
                    />
                  </div>

                  <div>
                    <label className="block text-sm font-medium text-gray-700 mb-1">
                      Preset File (ZIP, LRTEMPLATE, XMP, DNG)
                    </label>
                    <input
                      type="file"
                      accept=".zip,.lrtemplate,.xmp,.dng"
                      onChange={(e) => setPresetFile(e.target.files?.[0] || null)}
                      className="block w-full text-sm text-gray-500
                        file:mr-4 file:py-2 file:px-4
                        file:rounded-md file:border-0
                        file:text-sm file:font-semibold
                        file:bg-primary-50 file:text-primary-700
                        hover:file:bg-primary-100"
                    />
                  </div>

                  <button
                    type="submit"
                    disabled={loading}
                    className="px-4 py-2 bg-primary-600 text-white rounded-md hover:bg-primary-700 disabled:opacity-50"
                  >
                    {loading ? 'Creating...' : 'Create Preset'}
                  </button>
                </form>
              </div>
            )}

            <div className="grid gap-4">
              {presets.map((preset) => (
                <div key={preset.id} className="bg-white p-6 rounded-lg shadow">
                  <div className="flex justify-between items-start">
                    <div className="flex-1">
                      <h3 className="text-lg font-semibold">{preset.name}</h3>
                      <p className="text-gray-600 mt-1">{preset.description}</p>
                      <div className="mt-2 flex gap-4 text-sm text-gray-500">
                        <span>Price: ${preset.price}</span>
                        <span>Category: {preset.categoryName || 'None'}</span>
                        <span>Status: {preset.isActive ? 'Active' : 'Inactive'}</span>
                      </div>
                      {(preset.beforeImageUrl || preset.afterImageUrl) && (
                        <div className="mt-2 text-sm text-green-600">
                          ✓ Has before/after images
                        </div>
                      )}
                      {preset.presetFileUrl && (
                        <div className="mt-1 text-sm text-green-600">
                          ✓ Has preset file
                        </div>
                      )}
                    </div>
                    <button
                      onClick={() => handleDeletePreset(preset.id)}
                      className="px-4 py-2 bg-red-600 text-white rounded-md hover:bg-red-700"
                    >
                      Delete
                    </button>
                  </div>
                </div>
              ))}
            </div>
          </div>
        )}
      </div>
    </div>
  );
}
