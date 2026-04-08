import axios from 'axios';

const API_BASE = 'http://localhost:5049';

export interface Order {
  id: string;
  customerId: string;
  totalAmount: number;
  status: string;
  createdAt: string;
}

export const getOrders = async (): Promise<Order[]> => {
  const response = await axios.get(API_BASE + '/api/orders');
  return response.data;
};

export const getOrderById = async (id: string): Promise<Order> => {
  const response = await axios.get(API_BASE + '/api/orders/' + id);
  return response.data;
};
