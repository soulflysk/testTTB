'use client';

import { useState, useEffect } from 'react';
import axios from 'axios';

interface Product {
  productCode: string;
  productName: string;
  unitPrice: number;
  stock?: Stock;
}

interface Stock {
  productCode: string;
  quantity: number;
}

interface CartItem {
  id: number;
  productCode: string;
  quantity: number;
  unitPrice: number;
  totalPrice: number;
  createdAt: string;
  product: Product;
}

export default function Home() {
  const [products, setProducts] = useState<Product[]>([]);
  const [cart, setCart] = useState<CartItem[]>([]);
  const [loading, setLoading] = useState(true);
  const [cartLoading, setCartLoading] = useState(false);
  const [showCart, setShowCart] = useState(false);

  const API_BASE = 'http://localhost:5001/api';

  useEffect(() => {
    fetchProducts();
    fetchCart();
  }, []);

  const fetchProducts = async () => {
    try {
      const response = await axios.get(`${API_BASE}/product`);
      console.log('API Response:', response.data);
      
      // Handle DTO response structure
      let productsData = response.data;
      if (productsData && productsData.$values) {
        productsData = productsData.$values;
      } else if (!Array.isArray(productsData)) {
        productsData = [];
      }
      
      setProducts(productsData);
    } catch (error) {
      console.error('Error fetching products:', error);
      setProducts([]);
    } finally {
      setLoading(false);
    }
  };

  const fetchCart = async () => {
    try {
      const response = await axios.get(`${API_BASE}/shoppingcart`);
      console.log('Cart Response:', response.data);
      
      // Handle DTO response structure
      let cartData = response.data;
      if (cartData && cartData.items) {
        let items = cartData.items;
        if (items && items.$values) {
          items = items.$values;
        }
        setCart(Array.isArray(items) ? items : []);
      } else {
        setCart([]);
      }
    } catch (error) {
      console.error('Error fetching cart:', error);
      setCart([]);
    }
  };

  const addToCart = async (productCode: string, quantity: number = 1) => {
    setCartLoading(true);
    try {
      const response = await axios.post(`${API_BASE}/shoppingcart`, {
        productCode,
        quantity
      });
      
      if (response.data.success) {
        // Refresh cart after successful addition
        await fetchCart();
        alert('เพิ่มสินค้าลงตะกร้าเรียบร้อย');
      } else {
        alert('ไม่สามารถเพิ่มสินค้าในตะกร้าได้');
      }
    } catch (error: any) {
      console.error('Error adding to cart:', error);
      const errorMessage = error.response?.data || 'ไม่สามารถเพิ่มสินค้าในตะกร้าได้';
      alert(errorMessage);
    } finally {
      setCartLoading(false);
    }
  };

  const updateCartItem = async (id: number, quantity: number) => {
    try {
      await axios.put(`${API_BASE}/shoppingcart/${id}`, { quantity });
      await fetchCart();
    } catch (error: any) {
      console.error('Error updating cart:', error);
      const errorMessage = error.response?.data || 'ไม่สามารถอัปเดตตะกร้าได้';
      alert(errorMessage);
    }
  };

  const removeFromCart = async (id: number) => {
    try {
      await axios.delete(`${API_BASE}/shoppingcart/${id}`);
      await fetchCart();
    } catch (error: any) {
      console.error('Error removing from cart:', error);
      const errorMessage = error.response?.data || 'ไม่สามารถลบสินค้าออกจากตะกร้าได้';
      alert(errorMessage);
    }
  };

  const clearCart = async () => {
    try {
      await axios.delete(`${API_BASE}/shoppingcart`);
      await fetchCart();
    } catch (error: any) {
      console.error('Error clearing cart:', error);
      const errorMessage = error.response?.data || 'ไม่สามารถล้างตะกร้าได้';
      alert(errorMessage);
    }
  };

  const getTotalItems = () => cart.reduce((sum, item) => sum + item.quantity, 0);
  const getTotalAmount = () => cart.reduce((sum, item) => sum + item.totalPrice, 0);

  if (loading) {
    return (
      <div className="min-h-screen flex items-center justify-center">
        <div className="text-xl">กำลังโหลด...</div>
      </div>
    );
  }

  return (
    <div className="min-h-screen bg-gray-50">
      <header className="bg-white shadow-sm">
        <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
          <div className="flex justify-between items-center h-16">
            <h1 className="text-2xl font-bold text-gray-900">ร้านค้าออนไลน์</h1>
            <button
              onClick={() => setShowCart(!showCart)}
              className="relative bg-blue-600 text-white px-4 py-2 rounded-lg hover:bg-blue-700"
            >
              ตะกร้าสินค้า ({getTotalItems()})
            </button>
          </div>
        </div>
      </header>

      <main className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
        <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 xl:grid-cols-4 gap-6">
          {products.map((product) => (
            <div key={product.productCode} className="bg-white rounded-lg shadow-md overflow-hidden">
              <div className="p-4">
                <h3 className="text-lg font-semibold text-gray-900 mb-2">{product.productName}</h3>
                <p className="text-2xl font-bold text-blue-600 mb-2">
                  ฿{product.unitPrice.toLocaleString()}
                </p>
                <p className="text-sm text-gray-600 mb-4">
                  สต็อก: {product.stock?.quantity || 0} ชิ้น
                </p>
                <button
                  onClick={() => addToCart(product.productCode)}
                  disabled={!product.stock?.quantity || cartLoading}
                  className="w-full bg-green-600 text-white py-2 px-4 rounded hover:bg-green-700 disabled:bg-gray-400 disabled:cursor-not-allowed"
                >
                  {product.stock?.quantity ? 'เพิ่มลงตะกร้า' : 'สินค้าหมด'}
                </button>
              </div>
            </div>
          ))}
        </div>
      </main>

      {showCart && (
        <div className="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50">
          <div className="bg-white rounded-lg p-6 max-w-2xl w-full max-h-[80vh] overflow-y-auto">
            <div className="flex justify-between items-center mb-4">
              <h2 className="text-xl font-bold">ตะกร้าสินค้า</h2>
              <button
                onClick={() => setShowCart(false)}
                className="text-gray-500 hover:text-gray-700"
              >
                ✕
              </button>
            </div>

            {cart.length === 0 ? (
              <p className="text-gray-600">ตะกร้าสินค้าว่าง</p>
            ) : (
              <>
                <div className="space-y-4 mb-4">
                  {cart.map((item) => (
                    <div key={item.id} className="flex items-center justify-between border-b pb-4">
                      <div className="flex-1">
                        <h4 className="font-semibold">{item.product.productName}</h4>
                        <p className="text-gray-600">฿{item.unitPrice.toLocaleString()} ต่อชิ้น</p>
                      </div>
                      <div className="flex items-center space-x-2">
                        <button
                          onClick={() => updateCartItem(item.id, Math.max(1, item.quantity - 1))}
                          className="bg-gray-200 text-gray-700 px-2 py-1 rounded"
                        >
                          -
                        </button>
                        <span className="w-12 text-center">{item.quantity}</span>
                        <button
                          onClick={() => updateCartItem(item.id, item.quantity + 1)}
                          className="bg-gray-200 text-gray-700 px-2 py-1 rounded"
                        >
                          +
                        </button>
                        <button
                          onClick={() => removeFromCart(item.id)}
                          className="bg-red-500 text-white px-2 py-1 rounded ml-2"
                        >
                          ลบ
                        </button>
                      </div>
                      <div className="text-right ml-4">
                        <p className="font-semibold">฿{item.totalPrice.toLocaleString()}</p>
                      </div>
                    </div>
                  ))}
                </div>

                <div className="border-t pt-4">
                  <div className="flex justify-between items-center mb-4">
                    <span className="text-lg font-semibold">รวมทั้งหมด:</span>
                    <span className="text-xl font-bold text-blue-600">
                      ฿{getTotalAmount().toLocaleString()}
                    </span>
                  </div>
                  <div className="flex space-x-2">
                    <button
                      onClick={clearCart}
                      className="flex-1 bg-gray-500 text-white py-2 px-4 rounded hover:bg-gray-600"
                    >
                      ล้างตะกร้า
                    </button>
                    <button
                      className="flex-1 bg-blue-600 text-white py-2 px-4 rounded hover:bg-blue-700"
                    >
                      ชำระเงิน
                    </button>
                  </div>
                </div>
              </>
            )}
          </div>
        </div>
      )}
    </div>
  );
}
