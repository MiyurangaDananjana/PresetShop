'use client';

import Image from 'next/image';
import { useState, useEffect } from 'react';

interface BeforeAfterPreviewProps {
  beforeImage: File | string;
  afterImage: File | string;
}

export default function BeforeAfterPreview({
  beforeImage,
  afterImage
}: BeforeAfterPreviewProps) {
  const [sliderPosition, setSliderPosition] = useState(50);
  const [isDragging, setIsDragging] = useState(false);
  const [beforeUrl, setBeforeUrl] = useState<string>('');
  const [afterUrl, setAfterUrl] = useState<string>('');

  useEffect(() => {
    if (beforeImage instanceof File) {
      const url = URL.createObjectURL(beforeImage);
      setBeforeUrl(url);
      return () => URL.revokeObjectURL(url);
    } else {
      setBeforeUrl(beforeImage);
    }
  }, [beforeImage]);

  useEffect(() => {
    if (afterImage instanceof File) {
      const url = URL.createObjectURL(afterImage);
      setAfterUrl(url);
      return () => URL.revokeObjectURL(url);
    } else {
      setAfterUrl(afterImage);
    }
  }, [afterImage]);

  const handleMove = (clientX: number, container: HTMLDivElement) => {
    const rect = container.getBoundingClientRect();
    const x = Math.max(0, Math.min(clientX - rect.left, rect.width));
    const percent = Math.max(0, Math.min((x / rect.width) * 100, 100));
    setSliderPosition(percent);
  };

  const handleMouseDown = () => {
    setIsDragging(true);
  };

  const handleMouseUp = () => {
    setIsDragging(false);
  };

  const handleMouseMove = (e: React.MouseEvent<HTMLDivElement>) => {
    if (!isDragging) return;
    handleMove(e.clientX, e.currentTarget);
  };

  const handleTouchMove = (e: React.TouchEvent<HTMLDivElement>) => {
    if (!isDragging) return;
    const touch = e.touches[0];
    handleMove(touch.clientX, e.currentTarget);
  };

  return (
    <div className="space-y-2">
      <div
        className="relative w-full h-96 overflow-hidden rounded-lg border-2 border-gray-200 select-none cursor-col-resize"
        onMouseMove={handleMouseMove}
        onMouseUp={handleMouseUp}
        onMouseLeave={handleMouseUp}
        onTouchMove={handleTouchMove}
        onTouchEnd={handleMouseUp}
      >
        {/* After Image (Bottom Layer) */}
        <div className="absolute inset-0">
          {afterUrl && (
            <Image
              src={afterUrl}
              alt="After"
              fill
              className="object-cover"
              unoptimized
            />
          )}
          <div className="absolute bottom-4 right-4 bg-black/70 text-white px-3 py-1 rounded-md text-sm font-medium">
            After
          </div>
        </div>

        {/* Before Image (Top Layer with Clip) */}
        <div
          className="absolute inset-0 overflow-hidden"
          style={{ clipPath: `inset(0 ${100 - sliderPosition}% 0 0)` }}
        >
          {beforeUrl && (
            <Image
              src={beforeUrl}
              alt="Before"
              fill
              className="object-cover"
              unoptimized
            />
          )}
          <div className="absolute bottom-4 left-4 bg-black/70 text-white px-3 py-1 rounded-md text-sm font-medium">
            Before
          </div>
        </div>

        {/* Slider Line */}
        <div
          className="absolute top-0 bottom-0 w-1 bg-white cursor-col-resize"
          style={{ left: `${sliderPosition}%` }}
          onMouseDown={handleMouseDown}
          onTouchStart={handleMouseDown}
        >
          {/* Slider Handle */}
          <div className="absolute top-1/2 left-1/2 -translate-x-1/2 -translate-y-1/2 w-12 h-12 bg-white rounded-full shadow-lg flex items-center justify-center">
            <svg
              className="w-6 h-6 text-gray-600"
              fill="none"
              stroke="currentColor"
              viewBox="0 0 24 24"
            >
              <path
                strokeLinecap="round"
                strokeLinejoin="round"
                strokeWidth={2}
                d="M8 9l4-4 4 4m0 6l-4 4-4-4"
              />
            </svg>
          </div>
        </div>
      </div>

      <p className="text-sm text-gray-500 text-center">
        Drag the slider to compare before and after images
      </p>
    </div>
  );
}
