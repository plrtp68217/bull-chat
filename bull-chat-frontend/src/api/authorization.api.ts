import apiClient from "./apiClient";
import type { IAuthDto } from "./interfaces/authorization/IAuthDto";
import type { IAuthResponse } from "./interfaces/authorization/IAuthResponse";

export default {
  async login(dto: IAuthDto): Promise<IAuthResponse> {
    const response = await apiClient.post('/authentication/login', dto);
    return response.data;
  },
  async register(dto: IAuthDto): Promise<string> {
    const response = await apiClient.post('/authentication/register', dto);
    return response.data;
  },
}