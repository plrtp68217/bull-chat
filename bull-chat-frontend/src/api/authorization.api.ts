import apiClient from "./apiClient";
import type { IAuthDto } from "./interfaces/authorization/IAuthDto";

export default {
  async login(dto: IAuthDto): Promise<Boolean> {
    const response = await apiClient.post('/authentication/login', dto);
    return response.data;
  },
  async logout(): Promise<string> {
    const response = await apiClient.post('/authentication/logout');
    return response.data;
  },
  async register(dto: IAuthDto): Promise<string> {
    const response = await apiClient.post('/authentication/register', dto);
    return response.data;
  },
  async validateJwt(): Promise<Boolean>  {
    const response = await apiClient.post('/authentication/validate-jwt');
    return response.data;
  }
}