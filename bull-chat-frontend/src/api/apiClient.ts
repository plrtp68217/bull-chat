import axios from "axios";

const apiClient = axios.create({
  // baseURL: "/api",
  baseURL: 'http://localhost:5081/api/',
  timeout: 5000,
  headers: {
    "Content-Type": "application/json",
  },
  withCredentials: true,
});

apiClient.interceptors.request.use(
  (config) => {
    const token = localStorage.getItem('JWT_TOKEN');
    
    if (token) {
      config.headers.Authorization = `Bearer ${token}`;
    }
    
    return config;
  },
  (error) => {
    return Promise.reject(error);
  }
);


export default apiClient;