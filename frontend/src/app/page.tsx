import Hero from "@/components/Hero";
import ProductCard from "@/components/ProductCard";
import BeforeAfter from "@/components/BeforeAfter";
import Testimonial from "@/components/Testimonial";
import Newsletter from "@/components/Newsletter";
import { getFeaturedProducts, testimonials } from "@/lib/data";
import Link from "next/link";

export default function Home() {
  const featuredProducts = getFeaturedProducts();

  return (
    <>
      {/* Hero Section */}
      <Hero
        title="Professional Photo Presets for Creators"
        subtitle="Transform your photos with one click. Professional-grade Lightroom presets designed for mobile photography, optimized for Sri Lankan lighting and skin tones."
        ctaText="View Presets"
        ctaLink="/products"
        showBeforeAfter={true}
        titleClassName= "text-2xl md:text-3xl lg:text-4xl font-bold"
      />

      {/* Featured Collections */}
      <section id="featured" className="section-padding bg-white">
        <div className="container-custom">
          <div className="text-center mb-16">
            <h2 className="heading-2 mb-4">Featured Collections</h2>
            <p className="text-lg text-neutral-600 max-w-2xl mx-auto">
              Handpicked presets designed to bring out the best in your photos.
              From portraits to landscapes, we&apos;ve got you covered.
            </p>
          </div>

          <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-8">
            {featuredProducts.map((product) => (
              <ProductCard
                key={product.id}
                id={product.id}
                image={product.image}
                name={product.name}
                description={product.description}
                price={product.price}
                presetCount={product.presetCount}
              />
            ))}
          </div>

          <div className="text-center mt-12">
            <Link href="/products" className="btn-primary">
              View All Presets
            </Link>
          </div>
        </div>
      </section>

      {/* How It Works Section */}
      <section className="section-padding bg-neutral-50">
        <div className="container-custom">
          <div className="text-center mb-20">
            <h2 className="heading-2 mb-4">How It Works</h2>
            <p className="text-body text-neutral-600 max-w-2xl mx-auto">
              Get professional results in just three simple steps
            </p>
          </div>

          <div className="grid grid-cols-1 md:grid-cols-3 gap-12 max-w-5xl mx-auto">
            {/* Step 1 */}
            <div className="text-center group">
              <div className="relative mb-8">
                <div className="w-20 h-20 bg-primary-100 rounded-2xl flex items-center justify-center mx-auto transition-all duration-300 group-hover:scale-110 group-hover:bg-primary-200 group-hover:shadow-lg">
                  <svg className="w-10 h-10 text-primary-600" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M7 16a4 4 0 01-.88-7.903A5 5 0 1115.9 6L16 6a5 5 0 011 9.9M15 13l-3-3m0 0l-3 3m3-3v12" />
                  </svg>
                </div>
                <div className="absolute -top-3 -right-3 w-10 h-10 bg-primary-600 text-white rounded-full flex items-center justify-center font-bold text-lg">
                  1
                </div>
              </div>
              <h3 className="heading-3 mb-3 text-xl">Choose Preset</h3>
              <p className="text-neutral-600">
                Browse our collection and select the perfect preset for your style
              </p>
            </div>

            {/* Step 2 */}
            <div className="text-center group">
              <div className="relative mb-8">
                <div className="w-20 h-20 bg-primary-100 rounded-2xl flex items-center justify-center mx-auto transition-all duration-300 group-hover:scale-110 group-hover:bg-primary-200 group-hover:shadow-lg">
                  <svg className="w-10 h-10 text-primary-600" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M4 16l4.586-4.586a2 2 0 012.828 0L16 16m-2-2l1.586-1.586a2 2 0 012.828 0L20 14m-6-6h.01M6 20h12a2 2 0 002-2V6a2 2 0 00-2-2H6a2 2 0 00-2 2v12a2 2 0 002 2z" />
                  </svg>
                </div>
                <div className="absolute -top-3 -right-3 w-10 h-10 bg-primary-600 text-white rounded-full flex items-center justify-center font-bold text-lg">
                  2
                </div>
              </div>
              <h3 className="heading-3 mb-3 text-xl">Apply in Lightroom</h3>
              <p className="text-neutral-600">
                Import to Lightroom Mobile or Desktop with one click
              </p>
            </div>

            {/* Step 3 */}
            <div className="text-center group">
              <div className="relative mb-8">
                <div className="w-20 h-20 bg-primary-100 rounded-2xl flex items-center justify-center mx-auto transition-all duration-300 group-hover:scale-110 group-hover:bg-primary-200 group-hover:shadow-lg">
                  <svg className="w-10 h-10 text-primary-600" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M5 3v4M3 5h4M6 17v4m-2-2h4m5-16l2.286 6.857L21 12l-5.714 2.143L13 21l-2.286-6.857L5 12l5.714-2.143L13 3z" />
                  </svg>
                </div>
                <div className="absolute -top-3 -right-3 w-10 h-10 bg-primary-600 text-white rounded-full flex items-center justify-center font-bold text-lg">
                  3
                </div>
              </div>
              <h3 className="heading-3 mb-3 text-xl">Get Professional Results</h3>
              <p className="text-neutral-600">
                Enjoy stunning, professionally-edited photos instantly
              </p>
            </div>
          </div>
        </div>
      </section>

      {/* Before/After Section */}
      <section id="transformation" className="section-padding bg-white">
        <div className="container-custom">
          <div className="text-center mb-16">
            <h2 className="heading-2 mb-4">See The Transformation</h2>
            <p className="text-lg text-neutral-600 max-w-2xl mx-auto">
              Experience the dramatic difference our presets make. Drag the slider
              to reveal the before and after.
            </p>
          </div>

          <div className="max-w-5xl mx-auto space-y-12">
            <BeforeAfter
              beforeImage="https://images.unsplash.com/photo-1506905925346-21bda4d32df4?w=1200&q=80"
              afterImage="https://images.unsplash.com/photo-1506905925346-21bda4d32df4?w=1200&q=80&sat=20"
              alt="Golden Hour transformation"
            />
            <BeforeAfter
              beforeImage="https://images.unsplash.com/photo-1534528741775-53994a69daeb?w=1200&q=80"
              afterImage="https://images.unsplash.com/photo-1534528741775-53994a69daeb?w=1200&q=80&sat=5"
              alt="Portrait transformation"
            />
          </div>
        </div>
      </section>

      {/* Why Choose Us */}
      <section className="section-padding bg-neutral-50">
        <div className="container-custom">
          <div className="text-center mb-20">
            <h2 className="heading-2 mb-4">Why Choose Us</h2>
            <p className="text-body text-neutral-600 max-w-2xl mx-auto">
              Professional presets designed specifically for your needs
            </p>
          </div>

          <div className="grid grid-cols-1 md:grid-cols-3 gap-8 max-w-6xl mx-auto">
            <div className="bg-white rounded-2xl p-8 shadow-lg hover:shadow-xl transition-all duration-300 hover:-translate-y-2">
              <div className="w-16 h-16 bg-primary-100 rounded-2xl flex items-center justify-center mb-6">
                <svg className="w-8 h-8 text-primary-600" fill="none" strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" viewBox="0 0 24 24" stroke="currentColor">
                  <path d="M12 18h.01M8 21h8a2 2 0 002-2V5a2 2 0 00-2-2H8a2 2 0 00-2 2v14a2 2 0 002 2z"></path>
                </svg>
              </div>
              <h3 className="heading-3 text-xl mb-3">Optimized for Mobile Photography</h3>
              <p className="text-neutral-600 leading-relaxed">
                Designed specifically for mobile photographers who want professional results on the go.
              </p>
            </div>

            <div className="bg-white rounded-2xl p-8 shadow-lg hover:shadow-xl transition-all duration-300 hover:-translate-y-2">
              <div className="w-16 h-16 bg-primary-100 rounded-2xl flex items-center justify-center mb-6">
                <svg className="w-8 h-8 text-primary-600" fill="none" strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" viewBox="0 0 24 24" stroke="currentColor">
                  <path d="M9.75 17L9 20l-1 1h8l-1-1-.75-3M3 13h18M5 17h14a2 2 0 002-2V5a2 2 0 00-2-2H5a2 2 0 00-2 2v10a2 2 0 002 2z"></path>
                </svg>
              </div>
              <h3 className="heading-3 text-xl mb-3">Works with Lightroom Mobile & Desktop</h3>
              <p className="text-neutral-600 leading-relaxed">
                Seamlessly compatible with both Lightroom Mobile and Desktop versions for ultimate flexibility.
              </p>
            </div>

            <div className="bg-white rounded-2xl p-8 shadow-lg hover:shadow-xl transition-all duration-300 hover:-translate-y-2">
              <div className="w-16 h-16 bg-primary-100 rounded-2xl flex items-center justify-center mb-6">
                <svg className="w-8 h-8 text-primary-600" fill="none" strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" viewBox="0 0 24 24" stroke="currentColor">
                  <path d="M12 3v1m0 16v1m9-9h-1M4 12H3m15.364 6.364l-.707-.707M6.343 6.343l-.707-.707m12.728 0l-.707.707M6.343 17.657l-.707.707M16 12a4 4 0 11-8 0 4 4 0 018 0z"></path>
                </svg>
              </div>
              <h3 className="heading-3 text-xl mb-3">Designed for Sri Lankan Lighting & Skin Tones</h3>
              <p className="text-neutral-600 leading-relaxed">
                Expertly calibrated for tropical lighting conditions and South Asian skin tones for natural, beautiful results.
              </p>
            </div>
          </div>
        </div>
      </section>

      {/* Testimonials */}
      <section id="testimonials" className="section-padding bg-white">
        <div className="container-custom">
          <div className="text-center mb-16">
            <h2 className="heading-2 mb-4">Loved by Photographers</h2>
            <p className="text-lg text-neutral-600 max-w-2xl mx-auto">
              Don&apos;t just take our word for it. Here&apos;s what our customers have to say.
            </p>
          </div>

          <div className="grid grid-cols-1 md:grid-cols-3 gap-8">
            {testimonials.map((testimonial, index) => (
              <Testimonial
                key={index}
                name={testimonial.name}
                role={testimonial.role}
                image={testimonial.image}
                content={testimonial.content}
                rating={testimonial.rating}
              />
            ))}
          </div>
        </div>
      </section>

      {/* Newsletter */}
      <Newsletter />

      {/* Final CTA */}
      <section className="section-padding bg-gradient-to-br from-primary-600 to-primary-800 text-white relative overflow-hidden">
        {/* Decorative Elements */}
        <div className="absolute inset-0 opacity-20">
          <div className="absolute top-0 left-0 w-96 h-96 bg-white rounded-full blur-3xl"></div>
          <div className="absolute bottom-0 right-0 w-96 h-96 bg-white rounded-full blur-3xl"></div>
        </div>

        <div className="container-custom text-center relative z-10">
          <h2 className="heading-2 mb-6">Ready to Transform Your Photos?</h2>
          <p className="text-body text-white/90 mb-10 max-w-2xl mx-auto">
            Join thousands of photographers creating stunning images with our premium presets.
          </p>
          <Link href="/products" className="inline-block px-10 py-5 bg-white text-primary-700 font-bold rounded-full hover:bg-neutral-50 transition-all duration-300 hover:scale-105 shadow-2xl text-lg">
            Browse All Collections →
          </Link>
        </div>
      </section>
    </>
  );
}
