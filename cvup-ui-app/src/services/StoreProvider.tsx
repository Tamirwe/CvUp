import { createContext, ReactNode } from "react";
import { RootStore } from "../store/RootStore";

const RootStoreContext = createContext({} as RootStore);

const StoreProvider = ({
  store,
  children,
}: {
  store: RootStore;
  children: ReactNode;
}) => {
  return (
    <RootStoreContext.Provider value={store}>
      {children}
    </RootStoreContext.Provider>
  );
};

// const StoreProvider: FC<{ store: RootStore; children: ReactNode }> = ({
//   store,
//   children,
// }) => {
//   return (
//     <RootStoreContext.Provider value={store}>
//       {children}
//     </RootStoreContext.Provider>
//   );
// };

export { StoreProvider, RootStoreContext };
