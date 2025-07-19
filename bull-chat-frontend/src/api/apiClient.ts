import axios from 'axios'

const apiClient = axios.create({
  // baseURL: import.meta.env.VITE_API_URL,
  baseURL: 'http://localhost:5081/api/',
  timeout: 5000,
  headers: {
    'Content-Type': 'application/json',
    'Access-Control-Allow-Origin': '*',
  },
  withCredentials: true
});

apiClient.interceptors.request.use((config) => {
  const token = localStorage.getItem('authToken')
  if (token) {
    config.headers.Authorization = `Bearer ${token}`
  }
  return config
})

export default apiClient;