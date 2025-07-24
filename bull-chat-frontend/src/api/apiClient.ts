import axios from "axios";

const apiClient = axios.create({
  baseURL: "/api",
  // baseURL: 'http://localhost:5081/api/',
  timeout: 5000,
  headers: {
    "Content-Type": "application/json",
  },
  withCredentials: true,
});

export default apiClient;