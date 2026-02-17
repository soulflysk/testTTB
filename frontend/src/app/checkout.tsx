'use client';

import { useState, useEffect } from 'react';
import axios from 'axios';

const API_BASE = 'http://localhost:5001/api';

interface CartItem {
  id: number;
  productCode: string;
  quantity: number;
  unitPrice: number;
  totalPrice: number;
  createdAt: string;
  product: {
    productCode: string;
    productName: string;
    unitPrice: number;
  };
}

interface StockStatus {
  productCode: string;
  productName: string;
  currentStock: number;
  lastUpdated: string;
}

export default function Checkout() {
  const [cart, setCart] = useState<CartItem[]>([]);
  const [stockStatus, setStockStatus] = useState<StockStatus[]>([]);
  const [loading, setLoading] = useState(true);
  const [processing, setProcessing] = useState(false);
  const [showCheckout, setShowCheckout] = useState(false);

  useEffect(() => {
    fetchCart();
    fetchStockStatus();
  }, []);

  const fetchCart = async () => {
    try {
      const response = await axios.get(`${API_BASE}/shoppingcart`);
      if (response.data && response.data.items && Array.isArray(response.data.items)) {
        setCart(response.data.items);
      }
    } catch (error) {
      console.error('Error fetching cart:', error);
    } finally {
      setLoading(false);
    }
  };

  const fetchStockStatus = async () => {
    try {
      const response = await axios.get(`${API_BASE}/checkout/stock-status`);
      if (Array.isArray(response.data)) {
        setStockStatus(response.data);
      }
    } catch (error) {
      console.error('Error fetching stock status:', error);
    }
  };

  const processCheckout = async () => {
    if (!cart.length) {
      alert('ตะกร้าสินค้าว่างเปล่า');
      return;
    }

    setProcessing(true);
    try {
      const response = await axios.post(`${API_BASE}/checkout`, {
        paymentMethod: 'cash',
        customerInfo: 'Walk-in customer'
      });

      if (response.data.message) {
        alert(`ชำระเงินสำเร็จ!\nยอดรวม: ${response.data.totalAmount.toFixed(2)} บาท\nรายการที่ดำเนินการ: ${response.data.itemsProcessed} รายการ`);
        
        // Refresh cart and stock status
        await fetchCart();
        await fetchStockStatus();
        setShowCheckout(false);
      }
    } catch (error: any) {
      console.error('Checkout error:', error);
      if (error.response?.data?.errors) {
        alert('เกิดข้อผิดพลาด:\n' + error.response.data.errors.join('\n'));
      } else {
        alert('ไม่สามารถดำเนินการชำระเงินได้ กรุณาลองใหม่');
      }
    } finally {
      setProcessing(false);
    }
  };

  const getTotalAmount = () => cart.reduce((sum, item) => sum + item.totalPrice, 0);

  if (loading) {
    return (
      <div className="min-h-screen flex items-center justify-center">
        <div className="text-xl">กำลังโหลด...</div>
      </div>
    );
  }

  return (
    <div className="min-h-screen bg-gray-50 p-8">
      <div className="max-w-6xl mx-auto">
        <h1 className="text-3xl font-bold mb-8">การชำระเงินและจัดการสต็อก</h1>
        
        <div className="grid grid-cols-1 lg:grid-cols-2 gap-8">
          {/* Cart Section */}
          <div className="bg-white rounded-lg shadow-md p-6">
            <h2 className="text-xl font-semibold mb-4">ตะกร้าสินค้า</h2>
            
            {cart.length === 0 ? (
              <p className="text-gray-500">ตะกร้าสินค้าว่างเปล่า</p>
            ) : (
              <>
                <div className="space-y-4 mb-6">
                  {cart.map((item) => (
                    <div key={item.id} className="border-b pb-4">
                      <div className="flex justify-between items-start">
                        <div>
                          <p className="font-medium">{item.product?.productName}</p>
                          <p className="text-sm text-gray-600">รหัส: {item.productCode}</p>
                          <p className="text-sm text-gray-600">จำนวน: {item.quantity}</p>
                          <p className="text-sm text-gray-600">ราคาต่อหน่วย: {item.unitPrice.toFixed(2)} บาท</p>
                        </div>
                        <div className="text-right">
                          <p className="font-semibold">{item.totalPrice.toFixed(2)} บาท</p>
                        </div>
                      </div>
                    </div>
                  ))}
                </div>
                
                <div className="border-t pt-4">
                  <div className="flex justify-between items-center mb-4">
                    <span className="text-lg font-semibold">ยอดรวม:</span>
                    <span className="text-lg font-bold text-blue-600">
                      {getTotalAmount().toFixed(2)} บาท
                    </span>
                  </div>
                  
                  <button
                    onClick={() => setShowCheckout(true)}
                    className="w-full bg-green-600 text-white py-3 px-4 rounded-lg hover:bg-green-700 transition-colors"
                  >
                    ดำเนินการชำระเงิน
                  </button>
                </div>
              </>
            )}
          </div>

          {/* Stock Status Section */}
          <div className="bg-white rounded-lg shadow-md p-6">
            <h2 className="text-xl font-semibold mb-4">สถานะสต็อกสินค้า</h2>
            
            <div className="space-y-3">
              {stockStatus.map((stock) => (
                <div key={stock.productCode} className="flex justify-between items-center p-3 border rounded">
                  <div>
                    <p className="font-medium">{stock.productName}</p>
                    <p className="text-sm text-gray-600">รหัส: {stock.productCode}</p>
                  </div>
                  <div className="text-right">
                    <span className={`inline-block px-3 py-1 rounded-full text-sm font-medium ${
                      stock.currentStock > 10 
                        ? 'bg-green-100 text-green-800' 
                        : stock.currentStock > 5 
                        ? 'bg-yellow-100 text-yellow-800'
                        : 'bg-red-100 text-red-800'
                    }`}>
                      คงเหลือ: {stock.currentStock} ชิ้น
                    </span>
                  </div>
                </div>
              ))}
            </div>
          </div>
        </div>

        {/* Checkout Modal */}
        {showCheckout && (
          <div className="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50">
            <div className="bg-white rounded-lg p-8 max-w-md w-full mx-4">
              <h3 className="text-xl font-semibold mb-4">ยืนยันการชำระเงิน</h3>
              
              <div className="mb-6">
                <p className="text-lg mb-2">ยอดรวม: {getTotalAmount().toFixed(2)} บาท</p>
                <p className="text-gray-600">จำนวนรายการ: {cart.length} รายการ</p>
              </div>
              
              <div className="flex gap-4">
                <button
                  onClick={() => setShowCheckout(false)}
                  className="flex-1 bg-gray-300 text-gray-700 py-2 px-4 rounded-lg hover:bg-gray-400"
                >
                  ยกเลิก
                </button>
                <button
                  onClick={processCheckout}
                  disabled={processing}
                  className="flex-1 bg-green-600 text-white py-2 px-4 rounded-lg hover:bg-green-700 disabled:opacity-50"
                >
                  {processing ? 'กำลังดำเนินการ...' : 'ยืนยันการชำระเงิน'}
                </button>
              </div>
            </div>
          </div>
        )}
      </div>
    </div>
  );
}
