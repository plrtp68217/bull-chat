import apiClient from "./apiClient";
import type { IToken } from "./interfaces/authorization/IToken";
import type { IAuthDto } from "./interfaces/authorization/IAuthDto";

export default {
  async login(dto: IAuthDto): Promise<IToken> {
    const response = await apiClient.post('/authentication/login', dto);
    const token = response.data.token;
    return token;
  },
  async register(dto: IAuthDto): Promise<string> {
    const response = await apiClient.post('/authentication/register', dto);
    return response.data;
  },
}