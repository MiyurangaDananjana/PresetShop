'use client';

import { useState, useEffect } from 'react';
import { useRouter } from 'next/navigation';
import Image from 'next/image';
import { useAuth } from '@/contexts/AuthContext';
import { PresetAPI, CategoryAPI, PurchaseAPI, Preset, Category } from '@/lib/api';
import BeforeAfter from '@/components/BeforeAfter';

export default function PresetsPage() {
  const router = useRouter();
  const { isAuthenticated, user } = useAuth();
  const [presets, setPresets] = useState<Preset[]>([]);
  const [categories, setCategories] = useState<Category[]>([]);
  const [selectedCategory, setSelectedCategory] = useState<number | null>(null);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [purchaseLoading, setPurchaseLoading] = useState<number | null>(null);

  useEffect(() => {
    fetchCategories();
    fetchPresets();
  }, []);

  const fetchCategories = async () => {
    try {
      const data = await CategoryAPI.getAll();
      setCategories(data);
    } catch (err) {
      console.error('Failed to fetch categories');
    }
  };

  const fetchPresets = async (categoryId?: number) => {
    setLoading(true);
    try {
      const data = await PresetAPI.getAll(categoryId);
      setPresets(data);
    } catch (err) {
      setError('Failed to fetch presets');
    } finally {
      setLoading(false);
    }
  };

  const handleCategoryFilter = (categoryId: number | null) => {
    setSelectedCategory(categoryId);
    fetchPresets(categoryId || undefined);
  };

  const handlePurchase = async (presetId: number) => {
    if (!isAuthenticated) {
      router.push('/login');
      return;
    }

    setPurchaseLoading(presetId);
    try {
      const purchase = await PurchaseAPI.create(presetId);
      alert(`Purchase successful! Transaction ID: ${purchase.transactionId}\nYou can now download your preset.`);

      // Download the preset
      const blob = await PurchaseAPI.download(presetId);
      const url = window.URL.createObjectURL(blob);
      const a = document.createElement('a');
      a.href = url;
      a.download = `preset-${presetId}.zip`;
      document.body.appendChild(a);
      a.click();
      window.URL.revokeObjectURL(url);
      document.body.removeChild(a);
    } catch (err) {
      alert(err instanceof Error ? err.message : 'Purchase failed');
    } finally {
      setPurchaseLoading(null);
    }
  };

  return (
    <div className="min-h-screen bg-gray-50 py-12 px-4">
      <div className="max-w-7xl mx-auto">
        {/* Header */}
        <div className="mb-8">
          <h1 className="text-4xl font-bold text-gray-900">Browse Presets</h1>
          <p className="text-gray-600 mt-2">
            {isAuthenticated
              ? `Welcome back, ${user?.fullName}!`
              : 'Please login to purchase presets'}
          </p>
        </div>

        {/* Category Filter */}
        <div className="mb-8 flex flex-wrap gap-3">
          <button
            onClick={() => handleCategoryFilter(null)}
            className={`px-4 py-2 rounded-md ${
              selectedCategory === null
                ? 'bg-primary-600 text-white'
                : 'bg-white text-gray-700 border border-gray-300 hover:bg-gray-50'
            }`}
          >
            All Presets
          </button>
          {categories.map((category) => (
            <button
              key={category.id}
              onClick={() => handleCategoryFilter(category.id)}
              className={`px-4 py-2 rounded-md ${
                selectedCategory === category.id
                  ? 'bg-primary-600 text-white'
                  : 'bg-white text-gray-700 border border-gray-300 hover:bg-gray-50'
              }`}
            >
              {category.name}
            </button>
          ))}
        </div>

        {/* Error Message */}
        {error && (
          <div className="mb-4 p-4 bg-red-50 border border-red-200 text-red-700 rounded-md">
            {error}
          </div>
        )}

        {/* Loading State */}
        {loading && (
          <div className="text-center py-12">
            <p className="text-gray-600">Loading presets...</p>
          </div>
        )}

        {/* Presets Grid */}
        {!loading && (
          <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
            {presets.map((preset) => (
              <div
                key={preset.id}
                className="bg-white rounded-lg shadow-lg overflow-hidden hover:shadow-xl transition-shadow"
              >
                {/* Before/After Images */}
                {preset.beforeImageUrl && preset.afterImageUrl && (
                  <div className="relative h-64">
                    <BeforeAfter
                      beforeImage={`http://localhost:8080${preset.beforeImageUrl}`}
                      afterImage={`http://localhost:8080${preset.afterImageUrl}`}
                    />
                  </div>
                )}

                {/* Content */}
                <div className="p-6">
                  <div className="flex justify-between items-start mb-2">
                    <h3 className="text-xl font-semibold text-gray-900">
                      {preset.name}
                    </h3>
                    <span className="text-2xl font-bold text-primary-600">
                      ${preset.price}
                    </span>
                  </div>

                  {preset.categoryName && (
                    <p className="text-sm text-gray-500 mb-2">
                      {preset.categoryName}
                    </p>
                  )}

                  <p className="text-gray-600 mb-4 line-clamp-3">
                    {preset.description}
                  </p>

                  <button
                    onClick={() => handlePurchase(preset.id)}
                    disabled={purchaseLoading === preset.id || !preset.presetFileUrl}
                    className="w-full px-4 py-2 bg-primary-600 text-white rounded-md hover:bg-primary-700 transition-colors disabled:opacity-50 disabled:cursor-not-allowed"
                  >
                    {purchaseLoading === preset.id
                      ? 'Processing...'
                      : !preset.presetFileUrl
                      ? 'No File Available'
                      : 'Buy Now (Demo)'}
                  </button>

                  {!isAuthenticated && (
                    <p className="text-sm text-gray-500 mt-2 text-center">
                      Login required to purchase
                    </p>
                  )}
                </div>
              </div>
            ))}
          </div>
        )}

        {/* Empty State */}
        {!loading && presets.length === 0 && (
          <div className="text-center py-12">
            <p className="text-gray-600">No presets found in this category.</p>
          </div>
        )}
      </div>
    </div>
  );
}
