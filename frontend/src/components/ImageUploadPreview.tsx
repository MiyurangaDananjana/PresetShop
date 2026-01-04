'use client';

import Image from 'next/image';
import { useState } from 'react';

interface ImageUploadPreviewProps {
  label: string;
  onImageSelect: (file: File | null) => void;
  required?: boolean;
}

export default function ImageUploadPreview({
  label,
  onImageSelect,
  required = false
}: ImageUploadPreviewProps) {
  const [preview, setPreview] = useState<string | null>(null);

  const handleImageChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const file = e.target.files?.[0];

    if (file) {
      if (!file.type.startsWith('image/')) {
        alert('Please select an image file');
        return;
      }

      if (file.size > 10 * 1024 * 1024) {
        alert('Image size should be less than 10MB');
        return;
      }

      const reader = new FileReader();
      reader.onloadend = () => {
        setPreview(reader.result as string);
      };
      reader.readAsDataURL(file);
      onImageSelect(file);
    } else {
      setPreview(null);
      onImageSelect(null);
    }
  };

  const clearImage = () => {
    setPreview(null);
    onImageSelect(null);
  };

  return (
    <div className="space-y-2">
      <label className="block text-sm font-medium text-gray-700">
        {label} {required && <span className="text-red-500">*</span>}
      </label>

      <div className="flex items-center gap-4">
        <input
          type="file"
          accept="image/*"
          onChange={handleImageChange}
          className="block w-full text-sm text-gray-500
            file:mr-4 file:py-2 file:px-4
            file:rounded-md file:border-0
            file:text-sm file:font-semibold
            file:bg-primary-50 file:text-primary-700
            hover:file:bg-primary-100
            cursor-pointer"
          required={required}
        />

        {preview && (
          <button
            type="button"
            onClick={clearImage}
            className="text-sm text-red-600 hover:text-red-700 font-medium"
          >
            Clear
          </button>
        )}
      </div>

      {preview && (
        <div className="mt-4 relative w-full h-64 rounded-lg overflow-hidden border-2 border-gray-200">
          <Image
            src={preview}
            alt="Preview"
            fill
            className="object-contain"
          />
        </div>
      )}
    </div>
  );
}
