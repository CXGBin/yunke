import { create } from 'zustand';
import type { API } from '@/../typings';

interface AppState {
  currentUser: API.CurrentUser | undefined;
  setCurrentUser: (user: API.CurrentUser | undefined) => void;
  collapsed: boolean;
  setCollapsed: (v: boolean) => void;
}

const useAppStore = create<AppState>((set) => ({
  currentUser: undefined,
  setCurrentUser: (user) => set({ currentUser: user }),
  collapsed: false,
  setCollapsed: (v) => set({ collapsed: v }),
}));

export default useAppStore;
