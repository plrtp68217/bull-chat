import apiClient from "./apiClient";
import type { IAuthDto } from "./interfaces/authorization/IAuthDto";
import type { IAuthResponse } from "./interfaces/authorization/IAuthResponse";

export default {
  async login(dto: IAuthDto): Promise<IAuthResponse> {
    const response = await apiClient.post('/authentication/login', dto);
    return response.data;
  },
  async logout(clientHash: string): Promise<string> {
    const response = await apiClient.post('/authentication/logout', clientHash);
    return response.data;
  },
  async register(dto: IAuthDto): Promise<string> {
    const response = await apiClient.post('/authentication/register', dto);
    return response.data;
  },
  async validateJwt(token: string): Promise<Boolean>  {
    const response = await apiClient.post('/authentication/validate-jwt', token);
    return response.data;
  }
}