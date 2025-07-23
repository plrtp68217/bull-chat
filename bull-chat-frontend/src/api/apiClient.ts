import axios from 'axios'
const baseURL = `${import.meta.env.VITE_API_URL.replace(/\/+$/, '')}/api`;

const apiClient = axios.create({
  baseURL: baseURL,
  // baseURL: 'http://localhost:5081/api/',
  timeout: 5000,
  headers: {
    'Content-Type': 'application/json',
    'Access-Control-Allow-Origin': '*',
  },
  withCredentials: true
});

export default apiClient;