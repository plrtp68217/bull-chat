import { defineStore } from 'pinia'

import type { IUser } from './interfaces/IUser';


export const useUserStore = defineStore('user', {
  state: () => ({
    user: null as IUser | null
  }),

  getters: {
    getUserId: (state) => state.user!.id,
    getName: (state) => state.user!.name,
    getClientHash: (state) => state.user!.clientHash,
  },

  actions: {
    setUser(user: IUser) {
      this.user = user;
    },
  }
});