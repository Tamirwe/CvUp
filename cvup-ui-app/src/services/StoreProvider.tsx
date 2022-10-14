import { createContext, FC, ReactNode } from "react";
import { RootStore } from "../store/RootStore";

const RootStoreContext = createContext({} as RootStore);

const StoreProvider: FC<{ store: RootStore; children: ReactNode }> = ({
  store,
  children,
}) => {
  return (
    <RootStoreContext.Provider value={store}>
      {children}
    </RootStoreContext.Provider>
  );
};

export { StoreProvider, RootStoreContext };
