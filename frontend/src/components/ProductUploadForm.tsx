'use client';

import { useState } from 'react';
import ImageUploadPreview from './ImageUploadPreview';
import BeforeAfterPreview from './BeforeAfterPreview';
import { ProductAPI, CreateProductData } from '@/lib/api';

export default function ProductUploadForm() {
  const [formData, setFormData] = useState({
    name: '',
    description: '',
    price: '',
    category: '',
    presetCount: ''
  });

  const [mainImage, setMainImage] = useState<File | null>(null);
  const [beforeImage, setBeforeImage] = useState<File | null>(null);
  const [afterImage, setAfterImage] = useState<File | null>(null);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [success, setSuccess] = useState(false);

  const handleInputChange = (
    e: React.ChangeEvent<HTMLInputElement | HTMLTextAreaElement | HTMLSelectElement>
  ) => {
    const { name, value } = e.target;
    setFormData(prev => ({ ...prev, [name]: value }));
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setError(null);
    setSuccess(false);
    setLoading(true);

    try {
      if (!mainImage) {
        throw new Error('Please select a main product image');
      }

      const productData: CreateProductData = {
        name: formData.name,
        description: formData.description,
        price: parseFloat(formData.price),
        category: formData.category || undefined,
        presetCount: parseInt(formData.presetCount),
        mainImage: mainImage,
        beforeImage: beforeImage || undefined,
        afterImage: afterImage || undefined
      };

      await ProductAPI.createProduct(productData);

      setSuccess(true);

      // Reset form
      setFormData({
        name: '',
        description: '',
        price: '',
        category: '',
        presetCount: ''
      });
      setMainImage(null);
      setBeforeImage(null);
      setAfterImage(null);

      setTimeout(() => setSuccess(false), 5000);
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Failed to create product');
    } finally {
      setLoading(false);
    }
  };

  const categories = [
    'Outdoor',
    'Portrait',
    'Street',
    'Wedding',
    'Film',
    'Travel',
    'Landscape',
    'Urban',
    'Nature'
  ];

  return (
    <div className="max-w-4xl mx-auto p-6 bg-white rounded-lg shadow-lg">
      <h2 className="text-3xl font-bold text-gray-900 mb-6">
        Upload New Preset
      </h2>

      {error && (
        <div className="mb-4 p-4 bg-red-50 border border-red-200 text-red-700 rounded-md">
          {error}
        </div>
      )}

      {success && (
        <div className="mb-4 p-4 bg-green-50 border border-green-200 text-green-700 rounded-md">
          Product created successfully!
        </div>
      )}

      <form onSubmit={handleSubmit} className="space-y-6">
        {/* Basic Information */}
        <div className="space-y-4">
          <h3 className="text-xl font-semibold text-gray-800">
            Basic Information
          </h3>

          <div>
            <label htmlFor="name" className="block text-sm font-medium text-gray-700 mb-1">
              Product Name <span className="text-red-500">*</span>
            </label>
            <input
              type="text"
              id="name"
              name="name"
              value={formData.name}
              onChange={handleInputChange}
              required
              maxLength={200}
              className="w-full px-4 py-2 border border-gray-300 rounded-md focus:ring-2 focus:ring-primary-500 focus:border-transparent"
              placeholder="e.g., Moody Film Presets Collection"
            />
          </div>

          <div>
            <label htmlFor="description" className="block text-sm font-medium text-gray-700 mb-1">
              Description <span className="text-red-500">*</span>
            </label>
            <textarea
              id="description"
              name="description"
              value={formData.description}
              onChange={handleInputChange}
              required
              maxLength={2000}
              rows={4}
              className="w-full px-4 py-2 border border-gray-300 rounded-md focus:ring-2 focus:ring-primary-500 focus:border-transparent"
              placeholder="Describe your preset collection..."
            />
          </div>

          <div className="grid grid-cols-1 md:grid-cols-3 gap-4">
            <div>
              <label htmlFor="price" className="block text-sm font-medium text-gray-700 mb-1">
                Price ($) <span className="text-red-500">*</span>
              </label>
              <input
                type="number"
                id="price"
                name="price"
                value={formData.price}
                onChange={handleInputChange}
                required
                min="0.01"
                step="0.01"
                className="w-full px-4 py-2 border border-gray-300 rounded-md focus:ring-2 focus:ring-primary-500 focus:border-transparent"
                placeholder="29.99"
              />
            </div>

            <div>
              <label htmlFor="category" className="block text-sm font-medium text-gray-700 mb-1">
                Category
              </label>
              <select
                id="category"
                name="category"
                value={formData.category}
                onChange={handleInputChange}
                className="w-full px-4 py-2 border border-gray-300 rounded-md focus:ring-2 focus:ring-primary-500 focus:border-transparent"
              >
                <option value="">Select category</option>
                {categories.map(cat => (
                  <option key={cat} value={cat}>{cat}</option>
                ))}
              </select>
            </div>

            <div>
              <label htmlFor="presetCount" className="block text-sm font-medium text-gray-700 mb-1">
                Number of Presets <span className="text-red-500">*</span>
              </label>
              <input
                type="number"
                id="presetCount"
                name="presetCount"
                value={formData.presetCount}
                onChange={handleInputChange}
                required
                min="1"
                className="w-full px-4 py-2 border border-gray-300 rounded-md focus:ring-2 focus:ring-primary-500 focus:border-transparent"
                placeholder="15"
              />
            </div>
          </div>
        </div>

        {/* Image Uploads */}
        <div className="space-y-4">
          <h3 className="text-xl font-semibold text-gray-800">
            Images
          </h3>

          <ImageUploadPreview
            label="Main Product Image"
            onImageSelect={setMainImage}
            required
          />

          <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
            <ImageUploadPreview
              label="Before Image"
              onImageSelect={setBeforeImage}
            />

            <ImageUploadPreview
              label="After Image"
              onImageSelect={setAfterImage}
            />
          </div>
        </div>

        {/* Before/After Preview */}
        {beforeImage && afterImage && (
          <div className="space-y-4">
            <h3 className="text-xl font-semibold text-gray-800">
              Before/After Preview
            </h3>
            <BeforeAfterPreview
              beforeImage={beforeImage}
              afterImage={afterImage}
            />
          </div>
        )}

        {/* Submit Button */}
        <div className="flex justify-end gap-4 pt-6 border-t">
          <button
            type="button"
            onClick={() => {
              setFormData({
                name: '',
                description: '',
                price: '',
                category: '',
                presetCount: ''
              });
              setMainImage(null);
              setBeforeImage(null);
              setAfterImage(null);
            }}
            className="px-6 py-2 border border-gray-300 rounded-md text-gray-700 hover:bg-gray-50 transition-colors"
          >
            Reset
          </button>

          <button
            type="submit"
            disabled={loading}
            className="px-6 py-2 bg-primary-600 text-white rounded-md hover:bg-primary-700 transition-colors disabled:opacity-50 disabled:cursor-not-allowed"
          >
            {loading ? 'Uploading...' : 'Upload Preset'}
          </button>
        </div>
      </form>
    </div>
  );
}
